using System;
using System.Collections.Generic;
using System.Text;
using static Matrix_App.Defaults;

namespace Matrix_App
{
    public static class GifGeneratorUtils
    {
        public static float Dot(in float x0, in float y0, in float x1, in float y1)
        {
            return x0 * x1 + y0 * y1;
        }

        public static void CartesianToPolar(in float x, in float y, out float theta, out float r) 
        {
            r = MathF.Sqrt(x * x + y * y);
            theta = MathF.Atan2(y, x);
        }

        public static void PolarToCartesian(in float theta, in float r, out float x, out float y)
        {
            x = MathF.Sin(theta) * r;
            y = MathF.Cos(theta) * r;
        }

        public static void Normalize(ref float x, ref float y)
        {
            var len = MathF.Sqrt(x * x + y * y);

            x /= len;
            y /= len;
        }

        public static void RgbFromHsv(float h, float s, float v, out float r, out float g, out float b)
        {
            var c = v * s;
            var x = c * (1.0f - Math.Abs((h / 60.0f) % 2.0f - 1.0f));
            var m = v - c;

            if (h < 60)       { r = c; g = x; b = 0; }
            else if (h < 120) { r = x; g = c; b = 0; }
            else if (h < 180) { r = 0; g = c; b = x; }
            else if (h < 240) { r = 0; g = x; b = c; }
            else if (h < 300) { r = x; g = 0; b = c; }
            else              { r = c; g = 0; b = x; }

            r += m;
            g += m;
            b += m;
        }

        public static void HsvFromRgb(float r, float g, float b, out float h, out float s, out float v)
        {
            var cmax = Math.Max(Math.Max(r, g), b);
            var cmin = Math.Min(Math.Min(r, g), b);

            var delta = cmax - cmin;

            if (delta < 1e-2)
            {
                h = 0;
            }
            else if (MathF.Abs(cmax - r) < 1e-3)
            {
                if (r < b)
                {
                    h = 360 - MathF.Abs(60 * ((g - b) / delta));
                }
                else
                {
                    h = 60 * ((g - b) / delta + 0f);
                }
            }
            else if (MathF.Abs(cmax - g) < 1e-3)
            {
                h = 60 * ((b - r) / delta + 2f);
            }
            else if (MathF.Abs(cmax - b) < 1e-3)
            {
                h = 60 * ((r - g) / delta + 4f);
            } else
            {
                h = 0;
            }

            s = delta / cmax;
            if (cmax == 0)
            {
                s = 0;
            }
            v = cmax;
        }

        public static void SampleFrame(in byte[][] sampler, int frame, int x, int y, int width, out float r, out float g, out float b)
        {
            var index = (x + y * width) * Bpp;
            
            // normalize pixel value to [0, 1]
            r = sampler[frame][index + 0] * 0.00392156862745f;
            g = sampler[frame][index + 1] * 0.00392156862745f;
            b = sampler[frame][index + 2] * 0.00392156862745f;
        }

        public static float Saturate(float x)
        {
            return x < 0.0f ? 0.0f : x > 1.0f ? 1.0f : x;
        }
    }
}
