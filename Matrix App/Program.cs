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
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SplashScreen.ShowSplashScreen();
            
            var designer = new MatrixDesignerMain();
            
            SplashScreen.CloseForm();
            
            designer.StartPosition = FormStartPosition.CenterScreen;
            designer.WindowState = FormWindowState.Minimized;
            designer.Show();
            designer.WindowState = FormWindowState.Normal;
            
            Application.Run(designer);
        }
    }
}
