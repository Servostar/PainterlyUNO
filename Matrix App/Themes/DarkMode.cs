using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Matrix_App.Themes
{
    public sealed class DarkMode : BasicTheme
    {
        public DarkMode()
        {
            Foreground = Color.FromArgb(245, 245, 245);
            Background = Color.FromArgb(25, 26, 28);

            ButtonBackground = Color.FromArgb(39, 40, 43);
            ButtonBorder = Color.FromArgb(31, 32, 36);

            TextFieldBackground = Color.FromArgb(25, 25, 28);

            IsFlat = true;
        }
    }
}
