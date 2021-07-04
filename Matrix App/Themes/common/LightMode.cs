using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Matrix_App.Themes
{
    class LightMode : BasicTheme
    {
        public LightMode()
        {
            Foreground = Color.FromArgb(0, 0, 0);
            Background = Color.FromArgb(245, 245, 245);

            ButtonBackground = Color.FromArgb(224, 223, 218);
            ButtonBorder = Color.FromArgb(158, 157, 152);

            TextFieldBackground = Color.FromArgb(255, 255, 255);

            IsFlat = false;
        }
    }
}
