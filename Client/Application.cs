using System;

namespace Squirrel.Client
{
    public static class Application
    {
        public static Client m_client { get; private set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            m_client = new Client();
            m_client.run();

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new Interface.Interface());
        }
    }
}
