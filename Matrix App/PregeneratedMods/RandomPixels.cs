using System;
using Matrix_App.PregeneratedMods.reflection;

namespace Matrix_App.PregeneratedMods
{
    public class RandomPixels : MatrixGifGenerator
    {
        [UiWidget]
        [UiDescription(title: "Seed", description: "Just a seed for a bad deterministic random function")]
        private int seed = 0;

        protected override void ColorFragment(in int x, in int y, in float u, in float v, in int frame, out float r, out float g, out float b)
        {
            r = Next(frame, x, y);
            g = Next(frame, x, y + 67);
            b = Next(frame, x, y + 34968);
        }

        private float Next(int frame, int x, int y)
        {
            var k = MathF.Sin(frame * 2356f + (x + y) * 5334f + (y * x) * 534f + 78.0f + seed * 435f) * 567f;
            return k - MathF.Floor(k);
        }
    }
}
