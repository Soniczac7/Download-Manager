using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DownloadManager
{
    public partial class DownloadForm : Form
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

        public static readonly string installationPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
        public static DownloadForm _instance;
        public Logging logging = new Logging();
        Settings settings = new Settings();
        BrowserIntercept browserIntercept = new BrowserIntercept();
        public static int downloadsAmount = 0;
        public static string downloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Replace("Desktop", "Downloads") + "\\";

        public DownloadForm()
        {
            _instance = this;
            Logging.Log("Downloads folder: " + downloadsFolder, Color.Black);
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            browserIntercept.StartServer();
            if (Settings1.Default.downloadHistory != null)
            {
                foreach (var item in Settings1.Default.downloadHistory)
                {
                    Application.DoEvents();
                    textBox1.Items.Add(item);
                }
            }
            textBox2.Text = Settings1.Default.defaultDownload;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Close
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Minimize
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            // Titlebar Drag
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!textBox1.Items.Contains(textBox1.Text))
            {
                textBox1.Items.Add(textBox1.Text);
                Settings1.Default.downloadHistory.Add(textBox1.Text);
                Settings1.Default.Save();
            }
            DownloadProgress downloadProgress = new DownloadProgress(textBox1.Text, textBox2.Text, textBox3.Text, comboBox1.SelectedIndex);
            downloadProgress.Show();
            textBox1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath + @"\";
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label3.Text = "Downloading " + downloadsAmount + " files...";
            if (downloadsAmount > 0)
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                progressBar1.Style = ProgressBarStyle.Blocks;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // Debug log
            logging.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            // Settings
            settings.Show();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            // Calculate MD5 of a file
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    HashCalculator hashCalc = new HashCalculator(openFileDialog1.FileName);
                    hashCalc.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Open Form
            this.Show();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // Report a bug
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "https://github.com/Soniczac7/Download-Manager/issues/new?assignees=&labels=bug&template=bug_report.md&title=",
                Arguments = "",
                UseShellExecute = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                RedirectStandardInput = false
            };
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            // Exit
            Environment.Exit(0);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "https://microsoftedge.microsoft.com/addons/detail/download-manager/facopbimneimllhcabghncloejfeficd?hl=en-GB",
                Arguments = "",
                UseShellExecute = true,
                RedirectStandardError = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = false
            };
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
