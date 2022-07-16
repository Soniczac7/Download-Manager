namespace DownloadManagerInstaller
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[]? args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            ApplicationConfiguration.Initialize();

            if (args != null)
            {
                if (args.Length > 0)
                {
                    if (args[0] == "--install")
                    {
                        // Install
                        Application.Run(new Form1("install"));
                    }
                    else if (args[0] == "--update")
                    {
                        // Update
                        Application.Run(new Form1("update"));
                    }
                    else
                    {
                        Application.Run(new Form1(null));
                    }
                }
                else
                {
                    Application.Run(new Form1(null));
                }
            }
            else
            {
                Application.Run(new Form1(null));
            }
        }
    }
}