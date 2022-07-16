using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace DownloadManagerInstaller
{
    public partial class DownloadProgress : Form
    {
        #region DLL Import
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr one, int two, int three, int four);
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );
        #endregion

        string hash;
        string url;
        string location;
        string fileName;
        bool isUrlInvalid = false;
        public bool downloading = true;
        public bool error = false;
        WebClient client = new WebClient();

        //Disable close button
        private const int CP_DISABLE_CLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle = cp.ClassStyle | CP_DISABLE_CLOSE_BUTTON;
                return cp;
            }
        }

        public DownloadProgress(string urlArg, string locationArg, string hashArg)
        {
            InitializeComponent();
            client.Headers.Add("Cache-Control", "no-cache");
            client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
            textBox1.Text = urlArg;
            if (!locationArg.EndsWith("\\"))
            {
                location = locationArg + @"\";
            }
            else
            {
                location = locationArg;
            }
            url = urlArg;
            hash = hashArg;
        }

        private void progress_Load(object sender, EventArgs e)
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            Thread thread = new Thread(() =>
            {
                try
                {
                    client.OpenRead(url);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Download Manager - Error Fetching File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    error = true;
                    downloading = false;
                    if (this.IsHandleCreated == true)
                    {
                        Invoke(new MethodInvoker(delegate ()
                        {
                            client.CancelAsync();
                            client.Dispose();
                            this.Close();
                            this.Dispose();
                        }));
                    }
                    return;
                }
                Int64 bytes_total = Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
                Action action = () => label4.Text = "File Size: " + bytes_total.ToString() + " bytes";
                this.Invoke(action);
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                client.DownloadProgressChanged += Client_DownloadProgressChanged;

                Uri uri = new Uri(url);

                fileName = System.IO.Path.GetFileName(uri.AbsolutePath);
                Action action1 = () => progressBar1.Style = ProgressBarStyle.Blocks;
                this.Invoke(action1);
                try
                {
                    client.DownloadFileAsync(uri, location + fileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Download Manager - Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    error = true;
                    downloading = false;
                    Invoke(new MethodInvoker(delegate ()
                    {
                        client.CancelAsync();
                        client.Dispose();
                        this.Close();
                        this.Dispose();
                    }));
                    return;
                }
            });
            thread.Start();
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //Update progress bar & label
            if (this.IsHandleCreated)
            {
                try
                {
                    if (IsHandleCreated)
                    {
                        Invoke(new MethodInvoker(delegate ()
                        {
                            progressBar1.Minimum = 0;
                            double receive = double.Parse(e.BytesReceived.ToString());
                            double total = double.Parse(e.TotalBytesToReceive.ToString());
                            double percentage = receive / total * 100;
                            label3.Text = "Percentage Complete: " + $"{string.Format("{0:0.##}", percentage)}%";
                            try
                            {
                                progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
                            }
                            catch (Exception ex)
                            {
                                if (isUrlInvalid == false)
                                {
                                    isUrlInvalid = true;
                                    error = true;
                                    MessageBox.Show(ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    downloading = false;
                                    client.Dispose();
                                    this.Close();
                                    this.Dispose();
                                    return;
                                }
                                else { }
                            }
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + ex.StackTrace);
                    //MessageBox.Show(ex.Message);
                }
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (hash != "" && hash != null)
            {
                if (this.IsHandleCreated)
                {
                    try
                    {
                        Invoke(new MethodInvoker(delegate ()
                        {
                            byte[] fileData = File.ReadAllBytes(location + fileName);
                            byte[] myHash = MD5.Create().ComputeHash(fileData);
                            StringBuilder result = new StringBuilder(myHash.Length * 2);

                            for (int i = 0; i < myHash.Length; i++)
                            {
                                result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                            }

                            if (result.ToString() == hash)
                            {
                                // Success
                                downloading = false;
                                progressBar1.Style = ProgressBarStyle.Blocks;
                                progressBar1.Value = 100;
                                client.CancelAsync();
                                client.Dispose();
                                Form1._instance.LicenseDownloaded(System.IO.Path.GetTempPath() + fileName);
                                this.Close();
                                this.Dispose();
                                return;
                            }
                            else
                            {
                                // Fail
                                downloading = false;
                                Form1._instance.LicenseFailed();
                                client.CancelAsync();
                                client.Dispose();
                                this.Close();
                                this.Dispose();
                                return;
                            }
                        }));
                    }
                    catch (Exception ex)
                    {
                        error = true;
                        MessageBox.Show(ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        downloading = false;
                    }
                }
            }
            else
            {
                Invoke(new MethodInvoker(delegate ()
                {
                    // Success
                    downloading = false;
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    progressBar1.Value = 100;
                    client.CancelAsync();
                    client.Dispose();
                    this.Close();
                    this.Dispose();
                    return;
                }));
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            // Title-bar Drag
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Minimize
            WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Close
            if (!downloading)
            {
                client.Dispose();
                this.Close();
                this.Dispose();
            }
            else
            {
                DialogResult result = MessageBox.Show("Are you sure you want to cancel the download?", "Download Manager", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.OK)
                {
                    client.CancelAsync();
                    client.Dispose();
                    this.Close();
                    this.Dispose();
                }
            }
        }
    }
}
