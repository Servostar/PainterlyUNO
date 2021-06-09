using System;
using static Matrix_App.GifGeneratorUtils;

namespace Matrix_App.PregeneratedMods
{
    public class SimpleRainbow : MatrixGifGenerator
    {
        [UiDescription(title: "Radial", description: "Uses the angle to alter hues")]
        public bool radial = false;

        [UiDescription(title: "Saturation", description: "Overall saturation")]
        public float saturation = 1.0f;
        [UiDescription(title: "Brightness", description: "Overall brightness")]
        public float value      = 1.0f;
        [UiDescription(title: "Hue rotation", description: "Offset for hue calculation")]
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
