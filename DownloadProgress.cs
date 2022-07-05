﻿using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using static DownloadManager.Logging;

namespace DownloadManager
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

        string url;
        string location;
        string fileName;
        byte[] md5Hash;
        bool isUrlInvalid = false;
        bool downloading = true;
        bool doFileVerify = false;
        WebClient client = new WebClient();

        public DownloadProgress(string urlArg, string locationArg, byte[]? md5HashArg)
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
            md5Hash = md5HashArg;
            if (md5Hash != null)
            {
                textBox2.Text = Encoding.Default.GetString(md5Hash);
                if (textBox2.Text == "" || textBox2.Text == "System.Byte[]")
                {
                    textBox2.Text = "MD5 hash is set but could not find such hash!";
                }
                else
                {
                    doFileVerify = true;
                    this.Size = new System.Drawing.Size(635, 203);
                }
            }
            else
            {
                doFileVerify = false;
                textBox2.Visible = false;
                label5.Visible = false;
                this.Size = new System.Drawing.Size(635, 173);
            }
            DownloadForm.downloadsAmount += 1;
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
        }

        private void progress_Load(object sender, EventArgs e)
        {
            Log("Preparing to start downloading...", Color.Black);
            checkBox2.Checked = Settings1.Default.closeOnComplete;
            checkBox1.Checked = Settings1.Default.keepOnTop;
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
                    if (this.IsHandleCreated == true)
                    {
                        Invoke(new MethodInvoker(delegate ()
                        {
                            DownloadForm.downloadsAmount -= 1;
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
                Log("Downloading file " + uri + " to " + location + fileName, Color.Black);
                try
                {
                    client.DownloadFileAsync(uri, location + fileName);
                }
                catch (Exception ex)
                {
                    Log(ex.Message, Color.Red);
                    MessageBox.Show(ex.Message, "Download Manager - Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Invoke(new MethodInvoker(delegate ()
                    {
                        DownloadForm.downloadsAmount -= 1;
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
                                DownloadForm.downloadsAmount -= 1;
                                Log(ex.Message, Color.Red);
                                MessageBox.Show(ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                client.Dispose();
                                this.Close();
                                this.Dispose();
                                return;
                            }
                            else { }
                        }
                    }));
                }
                catch (Exception ex)
                {
                    Log(ex.Message + Environment.NewLine + ex.StackTrace, Color.Red);
                }
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (this.IsHandleCreated)
            {
                try
                {
                    Invoke(new MethodInvoker(delegate ()
                    {
                        progressBar1.Style = ProgressBarStyle.Marquee;
                        if (!isUrlInvalid)
                        {
                            if (doFileVerify)
                            {
                                byte[] fileData = File.ReadAllBytes(location + fileName);
                                byte[] myHash = MD5.Create().ComputeHash(fileData);
                                if (myHash.SequenceEqual(md5Hash))
                                {
                                    Log("File verification OK.", Color.Black);
                                    checkBox2.Enabled = false;
                                    button2.Enabled = false;
                                    button3.Enabled = false;
                                    downloading = false;
                                    DownloadForm.downloadsAmount -= 1;
                                    Log("Finished downloading file.", Color.Black);
                                    progressBar1.Value = 100;
                                    progressBar1.Style = ProgressBarStyle.Blocks;
                                    if (checkBox2.Checked == true)
                                    {
                                        client.Dispose();
                                        this.Close();
                                        this.Dispose();
                                        return;
                                    }
                                }
                                else
                                {
                                    Log("Failed to verify file. The file will be re-downloaded.", Color.Red);
                                    DownloadForm.downloadsAmount -= 1;
                                    DownloadProgress download = new DownloadProgress(url, location, md5Hash);
                                    download.Show();
                                    client.CancelAsync();
                                    client.Dispose();
                                    this.Close();
                                    this.Dispose();
                                    return;
                                }
                            }
                            else
                            {
                                checkBox2.Enabled = false;
                                button2.Enabled = false;
                                button3.Enabled = false;
                                downloading = false;
                                DownloadForm.downloadsAmount -= 1;
                                Log("Finished downloading file.", Color.Black);
                                progressBar1.Value = 100;
                                progressBar1.Style = ProgressBarStyle.Blocks;
                                if (checkBox2.Checked == true)
                                {
                                    client.Dispose();
                                    this.Close();
                                    this.Dispose();
                                    return;
                                }
                            }
                        }
                        else
                        {
                            Log("Download failed.", Color.Red);
                            progressBar1.Value = 0;
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
                    Log(ex.Message + Environment.NewLine + ex.StackTrace, Color.Red);
                    MessageBox.Show(ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // TopMost
            if (checkBox1.Checked == true)
            {
                TopMost = true;
            }
            else
            {
                TopMost = false;
            }
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
                    DownloadForm.downloadsAmount -= 1;
                    client.CancelAsync();
                    client.Dispose();
                    this.Close();
                    this.Dispose();
                }
            }
        }
    }
}
