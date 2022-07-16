using IWshRuntimeLibrary;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using File = System.IO.File;

namespace DownloadManager
{
    public partial class Settings : Form
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
        HistoryEditor historyEditor = new HistoryEditor();

        public Settings()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));

            label12.Text = "Version: " + Assembly.GetEntryAssembly().GetName().Version;

            // Start menu shortcut
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\Download Manager\\Download Manager.lnk"))
            {
                label8.Text = "Start Menu Shortcut: " + Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\Download Manager\\Download Manager.lnk";
                button6.Enabled = false;
                button7.Enabled = true;
            }
            else
            {
                label8.Text = "Start Menu Shortcut: None Set";
                button6.Enabled = true;
                button7.Enabled = false;
            }

            // Desktop shortcut
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\Download Manager.lnk"))
            {
                label9.Text = "Desktop Shortcut: " + Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\Download Manager.lnk";
                button8.Enabled = true;
                button9.Enabled = false;
            }
            else
            {
                label9.Text = "Desktop Shortcut: None Set";
                button8.Enabled = false;
                button9.Enabled = true;
            }

            Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));

            // Startup shortcut
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + "\\Download Manager.lnk"))
            {
                checkBox3.Checked = true;
            }

            if (IsAdministrator() == false)
            {
                checkBox3.Enabled = false;
            }
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

        private void button5_Click(object sender, EventArgs e)
        {
            // Browse default download location
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath + @"\";
            }
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            // Load settings
            if (Settings1.Default.defaultDownload == "" || Settings1.Default.defaultDownload == null)
            {
                Settings1.Default.defaultDownload = DownloadForm.downloadsFolder;
                Settings1.Default.Save();
            }
            textBox1.Text = Settings1.Default.defaultDownload;
            checkBox1.Checked = Settings1.Default.closeOnComplete;
            checkBox2.Checked = Settings1.Default.keepOnTop;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Default download location
            Settings1.Default.defaultDownload = textBox1.Text;
            Settings1.Default.Save();
            DownloadForm._instance.textBox2.Text = textBox1.Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Close progress window on complete
            Settings1.Default.closeOnComplete = checkBox1.Checked;
            Settings1.Default.Save();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            // Keep progress window on top
            Settings1.Default.keepOnTop = checkBox2.Checked;
            Settings1.Default.Save();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Remove Start Menu Shortcut
            try
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\Download Manager\\Download Manager.lnk");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\Download Manager\\Download Manager.lnk"))
            {
                label8.Text = "Start Menu Shortcut: " + Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\Download Manager\\Download Manager.lnk";
                button6.Enabled = false;
                button7.Enabled = true;
            }
            else
            {
                label8.Text = "Start Menu Shortcut: None Set";
                button6.Enabled = true;
                button7.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Create Start Menu Shortcut
            try
            {
                string pathToExe = DownloadForm.installationPath + "DownloadManager.exe";
                string commonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
                string appStartMenuPath = Path.Combine(commonStartMenuPath, "Programs", "Download Manager");

                if (!Directory.Exists(appStartMenuPath))
                    Directory.CreateDirectory(appStartMenuPath);

                string shortcutLocation = Path.Combine(appStartMenuPath, "Download Manager" + ".lnk");
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                shortcut.Description = "Download Manager helps you download your files faster.";
                shortcut.TargetPath = pathToExe;
                shortcut.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\Download Manager\\Download Manager.lnk"))
            {
                label8.Text = "Start Menu Shortcut: " + Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\Download Manager\\Download Manager.lnk";
                button6.Enabled = false;
                button7.Enabled = true;
            }
            else
            {
                label8.Text = "Start Menu Shortcut: None Set";
                button6.Enabled = true;
                button7.Enabled = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Remove Desktop Shortcut
            try
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\Download Manager.lnk");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\Download Manager.lnk"))
            {
                label9.Text = "Desktop Shortcut: " + Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\Download Manager.lnk";
                button8.Enabled = true;
                button9.Enabled = false;
            }
            else
            {
                label9.Text = "Desktop Shortcut: None Set";
                button8.Enabled = false;
                button9.Enabled = true;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Create Desktop Shortcut
            string pathToExe = DownloadForm.installationPath + "DownloadManager.exe";
            string commonDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);

            if (!Directory.Exists(commonDesktopPath))
                Directory.CreateDirectory(commonDesktopPath);

            string shortcutLocation = Path.Combine(commonDesktopPath, "Download Manager" + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = "Download Manager helps you download your files faster.";
            shortcut.TargetPath = pathToExe;
            shortcut.Save();

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\Download Manager.lnk"))
            {
                label9.Text = "Desktop Shortcut: " + Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\Download Manager.lnk";
                button8.Enabled = true;
                button9.Enabled = false;
            }
            else
            {
                label9.Text = "Desktop Shortcut: None Set";
                button8.Enabled = false;
                button9.Enabled = true;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            string commonStartupPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
            try
            {
                if (checkBox3.Checked)
                {
                    string pathToExe = DownloadForm.installationPath + "DownloadManager.exe";

                    if (!Directory.Exists(commonStartupPath))
                        Directory.CreateDirectory(commonStartupPath);

                    string shortcutLocation = Path.Combine(commonStartupPath, "Download Manager" + ".lnk");
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                    shortcut.Description = "Download Manager helps you download your files faster.";
                    shortcut.TargetPath = pathToExe;
                    shortcut.Save();
                }
                else
                {
                    if (File.Exists(commonStartupPath + "\\Download Manager.lnk"))
                    {
                        try
                        {
                            File.Delete(commonStartupPath + "\\Download Manager.lnk");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Could not delete startup shortcut. No such file exists.", "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // Check for updates
            Logging.Log("Checking for updates...", Color.Black);
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "updateHandler.bat";
            startInfo.Arguments = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            startInfo.Verb = "runas";
            Process process = new Process();
            process.StartInfo = startInfo;

            Thread thread = new Thread(() =>
            {
                try
                {
                    process.Start();
                    process.WaitForExit();

                    // Check exit code
                    if (process.ExitCode == 0 || process.ExitCode == 1)
                    {
                        Logging.Log("Checking for updates succeeded with exit code: " + process.ExitCode, Color.Green);
                    }
                    else if (process.ExitCode == 2)
                    {
                        Logging.Log("Update error: Setup exited before the installation could complete.", Color.Red);
                    }
                    else if (process.ExitCode == 3)
                    {
                        Logging.Log("Update error: The update XML file is malformed.", Color.Red);
                    }
                    else if (process.ExitCode == 4)
                    {
                        Logging.Log("Update error: The update failed due to an error during the installation process.", Color.Red);
                    }
                    else if (process.ExitCode == 5)
                    {
                        Logging.Log("Update error: Setup could not retrieve the existing Download Manager installation. Ensure setup is in the same path as Download Manager and try again.", Color.Red);
                    }
                    else
                    {
                        Logging.Log("Update error: An unknown error occurred while checking for updates.", Color.Red);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Download Manager - Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Logging.Log("The setup application encountered a fatal error.", Color.Red);
                }
            });
            thread.Start();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            // Clear download history
            foreach (var item in DownloadForm._instance.textBox1.Items)
            {
                Application.DoEvents();
                DownloadForm._instance.textBox1.Items.Remove(item);
            }
            Settings1.Default.downloadHistory = new System.Collections.Specialized.StringCollection();
            Settings1.Default.Save();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            // Open history editor
            historyEditor.Show();
        }
    }
}
