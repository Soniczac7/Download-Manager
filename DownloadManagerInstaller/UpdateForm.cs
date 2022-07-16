using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Xml;

namespace DownloadManagerInstaller
{
    public partial class UpdateForm : Form
    {
        public static readonly string installationPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
        bool installing = false;

        public UpdateForm()
        {
            InitializeComponent();
        }

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

        private void UpdateForm_Load(object sender, EventArgs e)
        {
            this.Show();
            DownloadProgress xmlProgress = new DownloadProgress("https://raw.githubusercontent.com/Soniczac7/app-update/main/DownloadManager.xml", System.IO.Path.GetTempPath(), "");
            xmlProgress.ShowDialog();
            string version = "";
            string url = "";
            string mandatory = "";


            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(System.IO.Path.GetTempPath() + "DownloadManager.xml");
                version = xml.DocumentElement.ParentNode.ChildNodes.Item(1).ChildNodes.Item(0).InnerText;
                url = xml.DocumentElement.ParentNode.ChildNodes.Item(1).ChildNodes.Item(1).InnerText;
                mandatory = xml.DocumentElement.ParentNode.ChildNodes.Item(1).ChildNodes.Item(2).InnerText;

            }
            catch
            {
                MessageBox.Show("An error occurred while reading the update XML file.", "Download Manager Setup - Malformed XML", MessageBoxButtons.OK, MessageBoxIcon.Error);
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.Value = 3;
                ProgressBarColor.SetState(progressBar1, 2);
                Environment.Exit(3);
            }

            string fileVersion = "";

            try
            {
                fileVersion = FileVersionInfo.GetVersionInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\DownloadManager.exe").FileVersion;

                Debug.WriteLine(version + Environment.NewLine + url + Environment.NewLine + mandatory + Environment.NewLine + fileVersion);

            }
            catch
            {
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.Value = 3;
                ProgressBarColor.SetState(progressBar1, 2);
                MessageBox.Show("Setup could not retrieve the version of the existing Download Manager installation.\nEnsure setup is in the same directory as Download Manager and try again.", "Download Manager Setup", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(5);
            }

            if (fileVersion != version)
            {
                if (mandatory == "true")
                {

                    DialogResult result = MessageBox.Show("A important update is available: " + version + ".\nThe current installed version is: " + fileVersion + ".\nPress OK to continue.", "Download Manager Setup - Mandatory Update", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (result == DialogResult.OK)
                    {

                        Update(url);
                    }
                }
                else
                {

                    DialogResult result = MessageBox.Show("Version " + version + " is available.\nWould you like to install it?", "Download Manager Setup - Update Available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (result == DialogResult.Yes)
                    {

                        Update(url);
                    }
                    else
                    {
                        Environment.Exit(2);
                    }
                }
            }
            else
            {
                MessageBox.Show("You are on the latest version of Download Manager.", "Download Manager Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(1);
            }
        }

        void Update(string url)
        {
            installing = true;
            label1.Text = "Updating...";
            progressBar1.Style = ProgressBarStyle.Blocks;

            if (url == "" || url.Length < 10)
            {
                MessageBox.Show("Setup could not retrieve the update URL.\nEnsure setup is in the same directory as Download Manager and try again.", "Download Manager Setup - Malformed XML", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(3);
            }
            else
            {
                try
                {
                    // Download the update
                    DownloadProgress progress = new DownloadProgress(url, System.IO.Path.GetTempPath(), "");
                    progress.ShowDialog();

                    while (progress.downloading)
                    {
                        Application.DoEvents();
                    }

                    if (progress.error == true)
                    {
                        progressBar1.Value = 3;
                        throw new WebException("Failed to download: " + url);
                    }

                    progressBar1.Value = 1;

                    // Check if DownloadManager.exe is running
                    Process[] processes = Process.GetProcessesByName("DownloadManager");
                    if (processes.Length > 0)
                    {
                        foreach (Process process in processes)
                        {
                            Application.DoEvents();
                            process.Kill();
                        }
                    }

                    progressBar1.Value = 2;

                    // Install the update
                    ZipFile.ExtractToDirectory(System.IO.Path.GetTempPath() + "DownloadManager.zip", System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), true);
                    File.Delete(System.IO.Path.GetTempPath() + "DownloadManager.zip");

                    progressBar1.Value = 3;

                    // Finish install
                    label1.Text = "Update complete!";
                    DialogResult result = MessageBox.Show("Update complete.\nWould you like to open Download Manager?", "Download Manager Setup - Update Complete", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (result == DialogResult.Yes)
                    {
                        Process.Start(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\DownloadManager.exe");
                    }
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                    ProgressBarColor.SetState(progressBar1, 2);
                    MessageBox.Show("A " + ex.GetType().ToString() + " occurred while installing.\nPlease create a bug report and reinstall Download Manager.", "Download Manager Setup - Install Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(4);
                }
            }
        }

        private void UpdateForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (installing == true)
            {
                DialogResult result = MessageBox.Show("Download Manager is currently installing.\nIf you cancel the installation now your Download Manager installation will be corrupted or incomplete.\nAre you sure you want to cancel the installation?", "Download Manager Setup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    Environment.Exit(2);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                Environment.Exit(2);
            }
        }
    }

    public static class ProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar p, int state)
        {
            SendMessage(p.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }
}