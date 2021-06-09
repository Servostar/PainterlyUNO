using System;
using System.Collections.Generic;
using System.Text;

namespace Matrix_App.PregeneratedMods
{
    public sealed class Rain : MatrixGifGenerator
    {
        private float Fract(float x)
        {
            return x - MathF.Floor(x);
        }

        private float Step(float x, float y)
        {
            return y > x ? 1.0f : 0.0f;
        }

        private float Shower(float u, float v, float x, float y, float t, float frame)
        {
            float movY = Fract(v - frame * t + y + MathF.Sin(u * 5.0f) * 0.3f);

            float opacityX = Step(0.7f, Fract((u + x) * 93.0f));
            float opacityY = Step(0.6f, movY);

            float drop = MathF.Pow(movY, 6.0f);

            return opacityX * opacityY * drop;
        }

        protected override void ColorFragment(in int x, in int y, in float u, in float v, in int frame, out float r, out float g, out float b)
        {
            float time = frame / (totalFrames * 0.5f);

            float s1 = Shower(u, v, 0.00f, 0.0f, 1.0f, time);
            float s2 = Shower(u, v, 3.11f, 0.5f, 1.0f, time);
            float s3 = Shower(u, v, 3.40f, 0.2f, 0.6f, time);
            float s4 = Shower(u, v, 3.20f, 0.7f, 0.6f, time);

            float skyLight = (Fract(MathF.Sin(u * v + v) * 67128.0f + time) * 0.3f + 0.7f) * (1.3f - v);

            r = skyLight * 0.1f;
            g = skyLight * 0.2f;
            b = skyLight * 0.3f;

            g += 0.5f * s1;
            b += s1;

            g += 0.4f * s2;
            b += s2;

            g += 0.2f * s3;
            b += 0.3f * s3;

            g += 0.1f * s4;
            b += 0.2f * s4;
        }
    }
}
