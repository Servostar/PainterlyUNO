using System;
using System.Collections.Generic;
using System.Text;

using static Matrix_App.GifGeneratorUtils;

namespace Matrix_App
{
    public class SimpleRainbow : MatrixGifGenerator
    {
        public bool radial = false;

        public float saturation = 1.0f;
        public float value      = 1.0f;
        public float rotation   = 0.0f;

        protected override void ColorFragment(in int x, in int y, 
                                           in float u, in float v, 
                                           in int frame, 
                                           out float r, out float g, out float b)
        {
            if (radial)
            {
                CartesianToPolar(u - 0.5f, v - 0.5f, out float angle, out float _);

                RgbFromHsv(AddHueOffset((angle + MathF.PI) / MathF.PI * 180.0f + frame / (float)totalFrames * 360.0f), saturation, value, out r, out g, out b);
            } else
            {
                RgbFromHsv(AddHueOffset(u * 360.0f + frame / (float) (totalFrames + 0.001) * 360.0f), saturation, value, out r, out g, out b);
            }
        }

        private float AddHueOffset(float hue)
        {
            return MathF.Abs(hue + rotation * 360) % 360.0f;
        }
    }
}
