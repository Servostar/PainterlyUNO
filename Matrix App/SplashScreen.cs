using System.Threading;
using System.Windows.Forms;

namespace Matrix_App
{
    public class SplashScreen : Form
    {
        //Delegate for cross thread call to close
        private delegate void CloseDelegate();

        //The type of form to be displayed as the splash screen.
        private static SplashScreen? _splashForm;

        private SplashScreen()
        {
            FormBorderStyle = FormBorderStyle.None;
            
            Controls.Add(new Label()
            {
                Image = Properties.Resources.Pfüsikuh
            });
        }

        public static void ShowSplashScreen()
        {
            // Make sure it is only launched once.    
            if (_splashForm != null) 
                return;
            
            _splashForm = new SplashScreen();

            Thread thread = new Thread(ShowForm)
            {
                IsBackground = true, Name = "Splash screen management thread"
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private static void ShowForm()
        {
            if (_splashForm != null) Application.Run(_splashForm);
        }

        public static void CloseForm()
        {
            _splashForm?.Invoke(new CloseDelegate(CloseFormInternal));
        }

        private static void CloseFormInternal()
        {
            if (_splashForm != null)
            {
                _splashForm.Close();
                _splashForm = null;
            }
        }
    }
}