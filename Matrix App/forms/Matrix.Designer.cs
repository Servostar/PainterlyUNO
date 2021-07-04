
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Matrix_App
{
    partial class Matrix
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private bool Editable = true;
        public bool DrawGrid = true;
        public float brushSize = 1f;
        private bool HasFocus = false;

        private int oldX;
        private int oldY;

        private int mouseX;
        private int mouseY;

        private bool toggled = false;

        private float pixelScale;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                           ControlStyles.UserPaint |
                           ControlStyles.DoubleBuffer,
                           true);


        }

        public void Instance(MatrixDesignerMain form1)
        {
            this.form = form1;
        }

        public void SetEditable(bool b) => Editable = b;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private DirectBitmap matrix = new DirectBitmap(10, 10);

        private Rectangle bounds = new Rectangle();

        private Color paintColor = Color.Black;
        private Color inverseAverage = Color.White;

        private Rectangle cursorPoint = new Rectangle();
        private Rectangle focusPoint = new Rectangle();

        private float pixelWidth;
        private float pixelHeight;

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);

            if (matrix.Bitmap.Width > 20 || matrix.Bitmap.Height > 20)
            {
                if (form != null)
                {
                    form.showGridCheckbox.Checked = false;
                }
                DrawGrid = false;
            }

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            e.Graphics.DrawImageUnscaledAndClipped(matrix.Bitmap, bounds);

            if (HasFocus)
            {
                e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.Aquamarine), 4f), this.bounds);
            }

            if (Editable && DrawGrid)
            {
                var linePen = new Pen(new SolidBrush(inverseAverage));

                int r = 0;
                int g = 0;
                int b = 0;

                for (int x = 0; x < matrix.Width; x++)
                {
                    int px = (int)(x * pixelWidth) + bounds.X;
                    for (int y = 0; y < matrix.Height; y++)
                    {
                        int py = (int)(y * pixelHeight) + bounds.Y;

                        e.Graphics.DrawLine(linePen, new Point(px, bounds.Y), new Point(px, bounds.Y + bounds.Height));
                        e.Graphics.DrawLine(linePen, new Point(bounds.X, py), new Point(bounds.X + bounds.Width, py));

                        var pixel = matrix.GetPixel(x, y);
                        r += pixel.R;
                        g += pixel.G;
                        b += pixel.B;
                    }
                }
                e.Graphics.DrawRectangle(new Pen(new SolidBrush(inverseAverage), 3.5f), cursorPoint);
                e.Graphics.DrawRectangle(new Pen(new SolidBrush(inverseAverage), 2.0f), focusPoint);

                int count = matrix.Width * matrix.Height;
                r = 255 - r / count;
                g = 255 - g / count;
                b = 255 - b / count;

                inverseAverage = Color.FromArgb(r, g, b);
            }

            if (Editable)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                int brushSizeHalf = (int)(brushSize * 0.5f * pixelScale);
                int brushSizeI = (int)(brushSize * pixelScale);
                e.Graphics.DrawEllipse(new Pen(new SolidBrush(inverseAverage), 2f), new Rectangle(mouseX - brushSizeHalf, mouseY - brushSizeHalf, brushSizeI, brushSizeI));
            }
        }

        public void SetPaintColor(Color color)
        {
            paintColor = color;
        }

        private bool mouseDown;
        private bool leftDown;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            toggled = true;
            mouseDown = true;
            leftDown = e.Button == MouseButtons.Left;
            processMouse(e);

            if (Editable && !leftDown)
            {
                float u = (e.X - bounds.X) / (float)bounds.Width;
                float v = (e.Y - bounds.Y) / (float)bounds.Height;

                if (u < 1.0f && v < 1.0f && u > 0.0f && v > 0.0f)
                {
                    // remap to pixel coordinates
                    int x = (int)(u * matrix.Width);
                    int y = (int)(v * matrix.Height);

                    paintColor = matrix.GetPixel(x, y);
                    form.SetColor(paintColor);
                }
            }
            HasFocus = true;
            Refresh();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            mouseDown = false;

            if (Editable)
            {
                form.EnqueuePixelSet();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            HasFocus = true;
            Refresh();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            HasFocus = false;
            Refresh();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            HasFocus = true;
            DrawGrid ^= e.KeyCode == Keys.Space;
            Refresh();
        }

        public Int32[] getPixels()
        {
            return matrix.Bits;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            processMouse(e);
        }

        private void processMouse(MouseEventArgs e)
        {
            float u = (e.X - bounds.X) / (float)bounds.Width;
            float v = (e.Y - bounds.Y) / (float)bounds.Height;

            mouseX = e.X;
            mouseY = e.Y;

            if (u < 1.0f && v < 1.0f && u > 0.0f && v > 0.0f)
            {
                // remap to pixel coordinates
                int x = (int)(u * matrix.Width);
                int y = (int)(v * matrix.Height);

                focusPoint.X = (int)(x * pixelWidth) + bounds.X;
                focusPoint.Y = (int)(y * pixelHeight) + bounds.Y;

                if (Editable && mouseDown && leftDown)
                {
                    cursorPoint.X = focusPoint.X;
                    cursorPoint.Y = focusPoint.Y;

                    var g = Graphics.FromImage(matrix.Bitmap);
                    if (toggled)
                    {
                        if (brushSize < 2.0f)
                        {
                            SetPixel(x, y, paintColor);
                        } else
                        {
                            int brushSizeHalf = (int)(brushSize * 0.5f);
                            g.FillEllipse(new SolidBrush(paintColor), new Rectangle(x - brushSizeHalf, y - brushSizeHalf, (int)brushSize, (int)brushSize));
                        }

                    } else
                    {
                        int brushSizeHalf = (int)(brushSize * 0.5f);
                        g.FillEllipse(new SolidBrush(paintColor), new Rectangle(x - brushSizeHalf, y - brushSizeHalf, (int)brushSize, (int)brushSize));
                        g.FillEllipse(new SolidBrush(paintColor), new Rectangle(oldX - brushSizeHalf, oldY - brushSizeHalf, (int)brushSize, (int)brushSize));

                        g.DrawLine(new Pen(new SolidBrush(paintColor), brushSize), new Point(oldX, oldY), new Point(x, y));
                    }
                    toggled = false;

                    g.Dispose();

                    oldX = x;
                    oldY = y;

//                    SetPixel(x, y, paintColor);
                }
            } else
            {
                oldX = mouseX = e.X;
                oldY = mouseY = e.Y;
            }
            Refresh();
        }

        protected override void OnResize(EventArgs e)
        {
            if (e != null)
            {
                base.OnResize(e);
            }

            float aspect = matrix.Height / (float)matrix.Width;

            if ((int)(aspect * Width) < Height)
            {
                bounds.Width = Width;
                bounds.Height = (int)(aspect * Width);
            }
            else
            {
                bounds.Height = Height;
                bounds.Width = (int)(Height / aspect);
            }
            pixelScale = bounds.Width / (float) matrix.Width;

            bounds.X = Width / 2 - bounds.Width / 2;
            bounds.Y = Height / 2 - bounds.Height / 2;

            pixelWidth = bounds.Width / (float)matrix.Width;
            pixelHeight = bounds.Height / (float)matrix.Height;

            cursorPoint.Width = (int)pixelWidth;
            cursorPoint.Height = (int)pixelHeight;
            cursorPoint.X = bounds.X;
            cursorPoint.Y = bounds.Y;

            focusPoint.Width = cursorPoint.Width;
            focusPoint.Height = cursorPoint.Height;
            focusPoint.X = bounds.X;
            focusPoint.Y = bounds.Y;

            Refresh();
        }

        public void Fill(Color color)
        {
            for (int x = 0; x < matrix.Width; x++)
            {
                for (int y = 0; y < matrix.Height; y++)
                {
                    matrix.SetPixel(x, y, color);
                }
            }
            Refresh();
        }

        public void SetPixel(int x, int y, Color color)
        {
            matrix.SetPixel(x, y, color);
            Refresh();
        }

        public void SetPixelNoRefresh(int x, int y, Color color)
        {
            matrix.SetPixel(x, y, color);
        }

        public void SetPixel(int i, Color color)
        {
            int y = i / matrix.Width;
            int x = matrix.Width - y * matrix.Width;    // better than i % matrix.Width

            SetPixel(x, y, color);
        }

        public void SetImage(byte[] buffer)
        {
            for (int y = 0; y < matrix.Height; y++)
            {
                int index = y * matrix.Width;

                for (int x = 0; x < matrix.Width; x++)
                {
                    int tmp = (index + x) * 3;

                    matrix.SetPixel(x, y, Color.FromArgb(
                            buffer[tmp + 0],
                            buffer[tmp + 1],
                            buffer[tmp + 2]
                        ));
                }
            }
            Refresh();
        }

        public void resize(int width, int height)
        {
            matrix = new DirectBitmap(width, height);

            if (width > 16 || height > 16)
            {
                if (form != null)
                {
                    form.showGridCheckbox.Checked = false;
                }
                DrawGrid = false;
            }

            Fill(Color.Black);
            OnResize(null);
            Refresh();
        }

        public void SetImage(Bitmap image)
        {
            matrix.SetImage(image);
        }

        public int matrixWidth()
        {
            return matrix.Width;
        }

        public int matrixHeight()
        {
            return matrix.Height;
        }
    }
}
