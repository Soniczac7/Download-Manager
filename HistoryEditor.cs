using System.Runtime.InteropServices;

namespace DownloadManager
{
    public partial class HistoryEditor : Form
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

        public HistoryEditor()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));

            if (Settings1.Default.downloadHistory != null)
            {
                foreach (var item in Settings1.Default.downloadHistory)
                {
                    Application.DoEvents();
                    listBox1.Items.Add(item);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Close
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
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
            // Remove Item
            if (listBox1.Items.Count < 1)
            {
                MessageBox.Show("Cannot remove an item that does not exist!", "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("No item selected.", "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Settings1.Default.downloadHistory.RemoveAt(listBox1.SelectedIndex);
                Settings1.Default.Save();
                DownloadForm._instance.textBox1.Items.RemoveAt(listBox1.SelectedIndex);
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Refresh
            listBox1.Items.Clear();
            if (Settings1.Default.downloadHistory != null)
            {
                foreach (var item in Settings1.Default.downloadHistory)
                {
                    Application.DoEvents();
                    listBox1.Items.Add(item);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Clear
            Settings1.Default.downloadHistory.Clear();
            Settings1.Default.Save();
            DownloadForm._instance.textBox1.Items.Clear();
            listBox1.Items.Clear();
        }
    }
}
