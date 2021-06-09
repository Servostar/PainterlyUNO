using System;

namespace Matrix_App.PregeneratedMods
{
    public class RandomPixels : MatrixGifGenerator
    {
        public int seed = 0;

        protected override void ColorFragment(in int x, in int y, in float u, in float v, in int frame, out float r, out float g, out float b)
        {
            r = next(frame, x, y);
            g = next(frame, x, y + 67);
            b = next(frame, x, y + 34968);
        }

        private float next(int frame, int x, int y)
        {
            float k = MathF.Sin(frame * 2356f + (x + y) * 5334f + (y * x) * 534f + 78.0f + seed * 435f) * 567f;
            return k - MathF.Floor(k);
        }
    }
}
