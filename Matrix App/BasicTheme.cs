using System.Drawing;
using System.Windows.Forms;

namespace Matrix_App
{
    public class BasicTheme
    {
        public Color Foreground { get; set; }
        public Color Background { get; set; }

        public Color ButtonBackground { get; set; }
        public Color ButtonBorder { get; set; }

        public Color TextFieldBackground { get; set; }

        public bool IsFlat { get; set; }

        protected BasicTheme()
        {

        }

        public void ApplyTheme(Control control)
        {
            foreach (Control c in control.Controls)
            {
                this.ApplyTheme(c);
            }

            control.BackColor = this.Background;
            control.ForeColor = this.Foreground;

            if (control is Button button)
            {
                button.FlatStyle = FlatStyle.Flat;
                button.BackColor = this.ButtonBackground;
                button.FlatAppearance.BorderColor = this.ButtonBorder;
            }
            else if (control is GroupBox group)
            {
                group.FlatStyle = FlatStyle.Flat;
            }
            else if (control is TextBox box)
            {
                box.BackColor = this.TextFieldBackground;
                box.Margin = new Padding(4, 4, 4, 4);

                if (this.IsFlat)
                {
                    box.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (!(control.Parent is ComboBox || control.Parent is NumericUpDown))
                {
                    box.BorderStyle = BorderStyle.FixedSingle;
                }
            }
            else if (control is NumericUpDown upDown)
            {
                upDown.Margin = new Padding(4, 4, 4, 4);
                upDown.BackColor = this.TextFieldBackground;

                if (this.IsFlat)
                {
                    upDown.BorderStyle = BorderStyle.None;
                }
                else
                {
                    upDown.BorderStyle = BorderStyle.Fixed3D;
                }
            }
            else if (control is ComboBox combos)
            {
                if (this.IsFlat)
                {
                    combos.FlatStyle = FlatStyle.Flat;
                }
                else
                {
                    combos.FlatStyle = FlatStyle.System;
                }
                
                combos.Margin = new Padding(4, 4, 4, 4);
                combos.BackColor = this.ButtonBackground;
            }
            else if (control is RichTextBox richText)
            {
                if (this.IsFlat)
                {
                    richText.BorderStyle = BorderStyle.None;
                }
                else
                {
                    richText.BorderStyle = BorderStyle.Fixed3D;
                }
                richText.BackColor = this.TextFieldBackground;
            }
        }
    }
}
