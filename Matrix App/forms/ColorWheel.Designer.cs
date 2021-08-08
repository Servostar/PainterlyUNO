using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Matrix_App
{
    partial class ColorWheel
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private static readonly int WHEEL_PIXEL_RESOLUTION = 256;
        private static readonly int WHEEL_BORDER_SUBDIVISIONS = 6;
        private static readonly float RADIUS = WHEEL_PIXEL_RESOLUTION / (float) WHEEL_BORDER_SUBDIVISIONS * 4 * 0.5f;

        private Bitmap wheel;

        /// transformed triangle vertices in screen space
        private PointF[] transformed = new PointF[3];
        /// triangle verticies in object "local" space
        private PointF[] vertices = {
            new PointF(RADIUS * fcos(0),   RADIUS * fsin(0)  ),
            new PointF(RADIUS * fcos(120), RADIUS * fsin(120)),
            new PointF(RADIUS * fcos(240), RADIUS * fsin(240))
        };

        /// gradient between the fully saturated and brightned tone color and white (desaturated)
        private LinearGradientBrush whiteGradient;
        /// gradient between black and transparent, darkens the corner in which value = 0
        private LinearGradientBrush colorGradient;
        /// gradient between transparent and gray
        private LinearGradientBrush grayGradient;

        // stored HSV values
        private float angle = 180;
        private float saturation = 1f;
        private float value = 1f;

        // current cursor position
        private Point cursor = new Point();
        private Point origCursor = new Point();

        public EventHandler handler;

        private int sRGB = 0;

        private Boolean entered = false;
        private Boolean release = true;

        /// <summary>
        /// Slider types:
        /// <ul>
        /// <li>TRIANGLE: for the center triangle</li>
        /// <li>WHEEL: for the outside "wheel" controlling the tone</li>
        /// </ul>
        /// </summary>
        private enum Slider
        {
            TRIANGLE,
            WHEEL
        }

        private Slider selection = Slider.TRIANGLE;
        private System.Drawing.Drawing2D.Matrix transform;

        private float ctheta;

        #region Math-utils
        /// convert degrees to radians
        /// by mapping [0, 360] to [0, 2pi]
        private static float ToRadians(float degree)
        {
            // about equal to pi / 180
            const float PI_OVER_180 = 0.0174532925199f;

            return degree * PI_OVER_180;
        }

        /// convert radians to degrees
        /// by mapping [0, 2pi] to [0, 360]
        private static float ToDegree(float radians)
        {
            // about equal to 180 / pi
            const float HUNDERETEIGTHY_OVER_PI = 57.2957795131f;

            return radians * HUNDERETEIGTHY_OVER_PI;
        }

        /// Calculates the cosine of degrees as a float
        private static float fcos(float degree)
        {
            return MathF.Cos(ToRadians(degree));
        }

        /// Calculates the sine of degrees as a float
        private static float fsin(float degree)
        {
            return MathF.Sin(ToRadians(degree));
        }

        /// <summary>
        /// Dotproduct of a and b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private float dot(in PointF a, in PointF b)
        {
            return a.X * b.X + a.Y + b.Y;
        }

        /// Calculates the hypotenuse of a and b
        /// as sqrt(a² + b²)
        private static float hypot(float a, float b)
        {
            return MathF.Sqrt(a * a + b * b);
        }

        /// Divides the floor of the sum of a and b by 2
        private static int floorMiddle(float a, float b)
        {
            return (int)(a + b) >> 1;
        }

        /// <summary>
        /// Computes the traingle area of the pointes a,b,(x | y)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private float triangleArea(PointF a, PointF b, float x, float y)
        {
            return MathF.Abs((a.X - x) * (b.Y - y)
                           - (b.X - x) * (a.Y - y));
        }

        /// <summary>
        /// Tests if the point (x | y) lies inside of the transformed triangle
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool triangleIntersect(double x, double y)
        {
            int originalArea = (int)triangleArea(transformed[1], transformed[2], transformed[0].X, transformed[0].Y);

            int area1 = (int)triangleArea(transformed[0], transformed[1], (float)x, (float)y);

            int area2 = (int)triangleArea(transformed[1], transformed[2], (float)x, (float)y);

            int area3 = (int)triangleArea(transformed[2], transformed[0], (float)x, (float)y);

            return Math.Abs(area1 + area2 + area3 - originalArea) < 3;
        }

        /// <summary>
        /// Convert cartesian coordinates (point p) to barycentric coordinates (u,v,w) relative to triangle with verticies a,b,c
        /// </summary>
        /// <param name="p"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        private void cartesianToBarycnetric(in PointF p, in PointF a, in PointF b, in PointF c, out float u, out float v, out float w)
        {
            var v0 = subtract(b, a);
            var v1 = subtract(c, a);
            var v2 = subtract(p, a);

            float denom = v0.X * v1.Y - v1.X * v0.Y;

            v = (v2.X * v1.Y - v1.X * v2.Y) / denom;
            w = (v0.X * v2.Y - v2.X * v0.Y) / denom;
            u = 1.0f - v - w;
        }

        /// <summary>
        /// Clamp barycentric coordinates of toClamp to the triangle specified by a,b,c, so that for every barycenttric coordinates is:
        /// u + v + w == 0 and none of u,v,w is greater than 1 or less than zero.
        /// </summary>
        /// <param name="toClamp"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        private void clampToBarycentricCoordinates(ref PointF toClamp, in PointF a, in PointF b, in PointF c)
        {
            cartesianToBarycnetric(toClamp, a, b, c, out float u, out float v, out float w);

            clampBarycentric(ref u, ref v, ref w);

            barycentricToCartesian(ref toClamp, a, b, c, u, v, w);
        }

        /// <summary>
        /// Convert barycentric coordinates u,v,w relative to triangle with verticies a,b,c to cartesian coordinates.
        /// </summary>
        /// <param name="toClamp"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        private void barycentricToCartesian(ref PointF toClamp, PointF a, PointF b, PointF c, float u, float v, float w)
        {
            toClamp.X = a.X * u + b.X * v + c.X * w;
            toClamp.Y = a.Y * u + b.Y * v + c.Y * w;
        }

        /// <summary>
        /// Clamp x to the range [0, 1]
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private float saturate(float x)
        {
            return MathF.Max(MathF.Min(x, 1.0f), 0.0f);
        }

        /// <summary>
        /// Clamp the barycentric coordinates, so that
        /// u + v + w == 1
        /// without any of u,v,w being greater than 1 or less than zero.
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="w"></param>
        private void clampBarycentric(ref float u, ref float v, ref float w)
        {
            u = saturate(u);
            v = saturate(v);
            w = 1 - u - v;

            u = saturate(u);
            w = saturate(w);
            v = 1 - u - w;

            v = saturate(v);
            w = saturate(w);
            u = 1 - v - w;
        }

        /// <summary>
        /// Linearlly interpolate between x and y by factor k
        /// If k == 0 then 100% of x and 0% of y
        /// If k == 1 then 0% of x and 100% of y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        private float lerp(float x, float y, float k)
        {
            return x * (1 - k) + y * k;
        }

        /// <summary>
        /// Normalize point f, so that its length is 1
        /// </summary>
        /// <param name="b"></param>
        private void normalize(ref PointF b)
        {
            float length = hypot(b.X, b.Y);

            b.X = b.X / length;
            b.Y = b.Y / length;
        }

        /// <summary>
        /// Subtract point a from b, so that the result is (x0 - x1 | y0 - y1)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private PointF subtract(in PointF a, in PointF b)
        {
            var result = new PointF();

            result.X = a.X - b.X;
            result.Y = a.Y - b.Y;

            return result;
        }

        /// <summary>
        /// Convert from HSV color space to sRGB. 
        /// s,v are expected to be in range [0, 1]. 
        /// h is expected to be in range [0, 1]. 
        /// Any other values results in undefiened behavior
        /// </summary>
        /// <param name="h"></param>
        /// <param name="s"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        private int sRGBfromHSV(float h, float s, float v)
        {
            float c = v * s;
            float x = c * (1.0f - Math.Abs((h / 60.0f) % 2.0f - 1.0f));
            float m = v - c;

            float r = 0, g = 0, b = 0;

            if (h < 60) { r = c; g = x; b = 0; }
            else if (h < 120) { r = x; g = c; b = 0; }
            else if (h < 180) { r = 0; g = c; b = x; }
            else if (h < 240) { r = 0; g = x; b = c; }
            else if (h < 300) { r = x; g = 0; b = c; }
            else { r = c; g = 0; b = x; }

            return 0xFF << 24 |
                (int)((r + m) * 255.0f) << 16 |
                (int)((g + m) * 255.0f) << 8 |
                (int)((b + m) * 255.0f);
        }

        /// <summary>
        /// Convert sRGB color space to HSV
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        private void HSVfromRGB(byte r, byte g, byte b)
        {
            const float INV_255 = 0.00392156862745f;

            float R = r * INV_255;
            float G = g * INV_255;
            float B = b * INV_255;

            byte Cmax = Math.Max(Math.Max(r, g), b);
            byte Cmin = Math.Min(Math.Min(r, g), b);

            float delta = (Cmax - Cmin) * INV_255;

            if (delta < 1e-2)
            {
                angle = 0;
            }
            else if (Cmax == r)
            {
                if (G < B)
                {
                    angle = 360 - MathF.Abs(60 * ((G - B) / delta));
                }
                else
                {
                    angle = 60 * ((G - B) / delta + 0f);
                }
            }
            else if (Cmax == g)
            {
                angle = 60 * ((B - R) / delta + 2f);
            }
            else if (Cmax == b)
            {
                angle = 60 * ((R - G) / delta + 4f);
            }

            saturation = delta / Cmax * 255.0f;
            if (Cmax == 0)
            {
                saturation = 0;
            }
            value = Cmax * INV_255;
        }

        #endregion

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            BackColorChanged += (a, b) => generateWheel();

            grayGradient = new LinearGradientBrush(
                // point A
                new Point((int)vertices[0].X,
                           (int)vertices[0].Y),
                // point B
                new Point(floorMiddle(vertices[2].X, vertices[1].X),
                           floorMiddle(vertices[2].Y, vertices[1].Y)),
                Color.Red,
                Color.Gray
                );
            whiteGradient = new LinearGradientBrush(
                // point A
                new Point((int)vertices[2].X,
                           (int)vertices[2].Y),
                // point B
                new Point(floorMiddle(vertices[0].X, vertices[1].X),
                           floorMiddle(vertices[0].Y, vertices[1].Y)),
                Color.White,
                Color.Transparent
                );
            colorGradient = new LinearGradientBrush(
                // point A
                new Point((int)vertices[1].X,
                           (int)vertices[1].Y),
                // point B
                new Point(floorMiddle(vertices[2].X, vertices[0].X),
                           floorMiddle(vertices[2].Y, vertices[0].Y)),
                Color.Black,
                Color.Transparent
                );

            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                            ControlStyles.UserPaint |
                            ControlStyles.DoubleBuffer,
                            true);

            this.MouseMove += new System.Windows.Forms.MouseEventHandler(OnMouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(OnMouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(OnMouseUp);

            generateWheel();

            setRGB(0, 0, 0);
        }
        
        private void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            release = true;
        }

        private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            release = false;
            entered = true;
        }

        private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            double u = e.X / (double)this.Width * 2.0 - 1.0;
            double v = e.Y / (double)this.Height * 2.0 - 1.0;

            double theta = Math.Atan2(v, u);
            double radius = Math.Sqrt(u * u + v * v);

            if (entered)
            {
                selection = Slider.WHEEL;
                if (radius < 0.6)
                {
                    selection = Slider.TRIANGLE;
                }

                entered = false;
            }
            else if (!release)
            {
                if (selection == Slider.TRIANGLE)
                {
                    if (transform != null)
                    {
                        PointF tempCursor = new PointF(e.X, e.Y);

                        clampToBarycentricCoordinates(ref tempCursor, transformed[0], transformed[1], transformed[2]);

                        if (true)
                        {
                            cursor.X = (int)tempCursor.X;
                            cursor.Y = (int)tempCursor.Y;

                            origCursor.X = cursor.X;
                            origCursor.Y = cursor.Y;

                            ctheta = angle;

                            cartesianToBarycnetric(cursor, transformed[0], transformed[1], transformed[2], out float bu, out float bv, out float bw);

                            value = MathF.Round(MathF.Max(MathF.Min(1.0f - bv, 1.0f), 0.0f) * 1e2f) / 1e2f;

                            saturation = MathF.Round(MathF.Max(MathF.Min(bu, 1.0f), 0.0f) * 1e2f) / 1e2f;
                        }
                    }
                }
                else
                {
                    angle = ToDegree((float)theta);
                    grayGradient = new LinearGradientBrush(
                        new Point((int)vertices[0].X,
                                   (int)vertices[0].Y),
                        new Point(floorMiddle(vertices[2].X, vertices[1].X),
                                   floorMiddle(vertices[2].Y, vertices[1].Y)
                                   ),
                        Color.FromArgb(sRGBfromHSV(180 + angle, 1, 1)),
                        Color.Gray
                        );

                    PointF[] c = { new PointF(origCursor.X, origCursor.Y) };

                    transform.Reset();
                    transform.RotateAt(angle - ctheta, new Point(this.Width >> 1, this.Height >> 1));
                    transform.TransformPoints(c);

                    cursor.X = (int)c[0].X;
                    cursor.Y = (int)c[0].Y;
                }

                sRGB = sRGBfromHSV(angle + 180, saturation, value);
                this.Refresh();
                handler.Invoke(this, null);
            }
        }

        /// <summary>
        /// Generate the color wheel "wheel" texture
        /// </summary>
        private void generateWheel()
        {
            wheel = new Bitmap(WHEEL_PIXEL_RESOLUTION, WHEEL_PIXEL_RESOLUTION);
            wheel.MakeTransparent();    // make transparent

            var g = Graphics.FromImage(wheel);

            // draw tone gradient by varying hue by the current angle of the pixel relative to the origin
            for (int x = wheel.Width; x-- > -1;)
            {
                float u = x / (float)wheel.Width * 2.0f - 1.0f;

                for (int y = wheel.Height; y-- > -1;)
                {
                    float v = y / (float)wheel.Height * 2.0f - 1.0f;

                    // cartesian to polar
                    float theta = ToDegree(MathF.Atan2(v, u) + MathF.PI);

                    int rgb = sRGBfromHSV(theta, 1.0f, 1.0f);
                    // draw pixel
                    g.FillRectangle(new SolidBrush(Color.FromArgb(rgb)), x, y, 1, 1);
                }
            }
            var step = wheel.Width / WHEEL_BORDER_SUBDIVISIONS;

            var strip = (int)Math.Sqrt(wheel.Width * wheel.Width + wheel.Height * wheel.Height) - wheel.Width;
            // cut out center circle
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.FillEllipse(new SolidBrush(this.BackColor), new Rectangle(step, step, step * (WHEEL_BORDER_SUBDIVISIONS - 2), step * (WHEEL_BORDER_SUBDIVISIONS - 2)));
            g.DrawEllipse(new Pen(new SolidBrush(this.BackColor), strip), new Rectangle(-strip / 2, -strip / 2, wheel.Width + strip, wheel.Height + strip));

            g.Dispose();
        }

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

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            // adjust for arbitrary component scale
            float scaleX = this.Width / (float) wheel.Width;
            float scaleY = this.Height / (float) wheel.Height;

            // transform local verticies to screen space
            g.ScaleTransform(scaleX, scaleY);
            g.DrawImage(wheel, new Point(0, 0));
            g.ResetTransform();

            g.RotateTransform(angle);

            float step = (wheel.Width / (float) WHEEL_BORDER_SUBDIVISIONS * (WHEEL_BORDER_SUBDIVISIONS - 3));
            float off = WHEEL_PIXEL_RESOLUTION / (float)(WHEEL_BORDER_SUBDIVISIONS - 2) * scaleX;

            g.ScaleTransform(wheel.Width / (float) Width * 4 / 6, wheel.Width / (float) Width * 4 / 6);
            g.TranslateTransform(wheel.Width * scaleX * 0.5f, wheel.Width * scaleX * 0.5f, MatrixOrder.Append);

            transformed[0].X = vertices[0].X;
            transformed[0].Y = vertices[0].Y;
            transformed[1].X = vertices[1].X;
            transformed[1].Y = vertices[1].Y;
            transformed[2].X = vertices[2].X;
            transformed[2].Y = vertices[2].Y;
            g.Transform.TransformPoints(transformed);

            // draw triangle
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.FillPolygon(grayGradient, vertices);
            g.FillPolygon(whiteGradient, vertices);
            g.FillPolygon(colorGradient, vertices);
          
            // paint hue line
            g.DrawLine(new Pen(new SolidBrush(Color.Black), 2.0f), vertices[0].X, vertices[0].Y, vertices[0].X + RADIUS / 2, vertices[0].Y);
           
            // paint bordered shadowed cursor
            g.ResetTransform();
            transform = g.Transform;
            g.FillEllipse(new SolidBrush(Color.FromArgb(-2013265920)), new Rectangle(cursor.X - 10, cursor.Y - 10, 22, 22));
            g.FillEllipse(new SolidBrush(Color.White), new Rectangle(cursor.X - 10, cursor.Y - 10, 20, 20));
            g.FillEllipse(new SolidBrush(Color.FromArgb(sRGB)), new Rectangle(cursor.X - 8, cursor.Y - 8, 16, 16));
        }
        
        #region Getter-Setter
        public int getRGB()
        {
            return sRGB;
        }
     
        public int getRed()
        {
            return sRGB >> 16 & 0xFF;
        }
        public int getGreen()
        {
            return sRGB >> 8 & 0xFF;
        }
        public int getBlue()
        {
            return sRGB & 0xFF;
        }
     
        public float getHue()
        {
            return angle;
        }
        public float getSaturation()
        {
            return saturation;
        }
        public float getValue()
        {
            return value;
        }

        public void setHue(float hue)
        {
            angle = hue;
            showHSV();
        }
        public void setSaturation(float s)
        {
            saturation = s;
            showHSV();
        }
        public void setValue(float v)
        {
            value = v;
            showHSV();
        }

        public void setRGB(byte red, byte green, byte blue)
        {
            HSVfromRGB(red, green, blue);

            showHSV();
        }
        #endregion

        /// <summary>
        /// Show the color wheel and recompute triangle gradient
        /// and calculate new cursor position
        /// </summary>
        private void showHSV()
        {
            // get sRGB color
            sRGB = sRGBfromHSV(angle, saturation, value);

            // recompute correct triangle vertex transformation
            // to screen space
            System.Drawing.Drawing2D.Matrix m = new System.Drawing.Drawing2D.Matrix();

            transformed[0].X = vertices[0].X;
            transformed[0].Y = vertices[0].Y;
            transformed[1].X = vertices[1].X;
            transformed[1].Y = vertices[1].Y;
            transformed[2].X = vertices[2].X;
            transformed[2].Y = vertices[2].Y;

            // adjust for custom component width
            float scaleX = this.Width / (float)wheel.Width;
            float scaleY = this.Height / (float)wheel.Height;

            m.Rotate(angle - 180);

            float step = (wheel.Width / (float)WHEEL_BORDER_SUBDIVISIONS * (WHEEL_BORDER_SUBDIVISIONS - 3));
            float off = WHEEL_PIXEL_RESOLUTION / (float)(WHEEL_BORDER_SUBDIVISIONS - 2) * scaleX;

            m.Scale(wheel.Width / (float)Width * 4 / 6, wheel.Width / (float)Width * 4 / 6);
            m.Translate(wheel.Width * scaleX * 0.5f, wheel.Width * scaleX * 0.5f, MatrixOrder.Append);

            transformed[0].X = vertices[0].X;
            transformed[0].Y = vertices[0].Y;
            transformed[1].X = vertices[1].X;
            transformed[1].Y = vertices[1].Y;
            transformed[2].X = vertices[2].X;
            transformed[2].Y = vertices[2].Y;
            m.TransformPoints(transformed);

            // linearly interpolate cursor location according to saturation
            origCursor.X = (int)lerp(transformed[2].X, transformed[0].X, saturation);
            origCursor.Y = (int)lerp(transformed[2].Y, transformed[0].Y, saturation);
            // linearly interpolate cursor location according to value
            origCursor.X = (int)lerp(transformed[1].X, origCursor.X, value);
            origCursor.Y = (int)lerp(transformed[1].Y, origCursor.Y, value);
            // apply cursor location
            cursor.X = origCursor.X;
            cursor.Y = origCursor.Y;

            // recompute new gradient for mixing fully saturated color with gray
            sRGB = sRGBfromHSV(angle, saturation, value);
            grayGradient = new LinearGradientBrush(new Point((int)vertices[0].X, (int)vertices[0].Y), new Point((int)(vertices[2].X + vertices[1].X) / 2, (int)(vertices[2].Y + vertices[1].Y) / 2), Color.FromArgb(sRGBfromHSV(angle, 1f, 1f)), Color.Gray);

            // adjust angle values
            angle -= 180;
            ctheta = angle;
            // repaint
            this.Refresh();
        }

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }
    }
}
