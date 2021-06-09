using System;
using System.Windows.Forms;

namespace Matrix_App
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            SplashScreen.ShowSplashScreen();
            
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var designer = new MatrixDesignerMain();
            
            SplashScreen.CloseForm();
            
            Application.Run(designer);
        }
    }
}
