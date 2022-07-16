namespace DownloadManager
{
    internal static class Program
    {
        private static bool DEBUG = false;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            if (DEBUG == true)
            {
                MessageBox.Show("Debug mode is on. You can now attach a debugger.\nTo turn it off, go to Program.cs and set DEBUG to false.\nPress OK to continue...", "Download Manager - DEBUG", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Application.Run(new DownloadForm());
        }
    }
}