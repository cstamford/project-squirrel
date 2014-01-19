// Project Squirrel 
// Copyright 2013-2014 Chris Stamford

using System;

namespace Squirrel.Client
{
    public static class Application
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new Interface.Interface());
        }
    }
}
