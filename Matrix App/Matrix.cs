using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Matrix_App
{
    public partial class Matrix : UserControl
    {
        private MatrixDesignerMain form;

        public Matrix()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Matrix
            // 
            this.Name = "Matrix";
            this.Size = new System.Drawing.Size(104, 102);
            this.ResumeLayout(false);
        }

        internal Color GetPixel(int x, int y)
        {
            return matrix.GetPixel(x, y);
        }
    }
}
