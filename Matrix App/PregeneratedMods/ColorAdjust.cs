using System;
using System.Collections.Generic;
using System.Text;
using static Matrix_App.GifGeneratorUtils;

namespace Matrix_App.PregeneratedMods
{
    public sealed class ColorAdjust : MatrixGifGenerator
    {
        public float hueOffset       = 0.0f;
        public float saturationBoost = 0.5f;
        public float valueBoost      = 0.5f;

        public float redBoost   = 0.5f;
        public float greenBoost = 0.5f;
        public float blueBoost  = 0.5f;

        private float boost(float x, float y)
        {
            return Math.Clamp(x + (y - 0.5f) * 2.0f, 0.0f, 1.0f);
        }

        protected override void ColorFragment(in int x, in int y, in float u, in float v, in int frame, out float r, out float g, out float b)
        {
            SampleFrame(actualStore, frame, x, y, width, out float tr, out float tg, out float tb);

            HsvFromRgb(tr, tg, tb, out float h, out float s, out float value);

            h = h / 360.0f + hueOffset;
            h = (h - MathF.Floor(h)) * 360.0f;
            s = boost(s, saturationBoost);
            value = boost(value, valueBoost);

            RgbFromHsv(h, s, value, out tr, out tg, out tb);

            r = boost(tr, redBoost);
            g = boost(tg, greenBoost);
            b = boost(tb, blueBoost);
        }
    }
}
