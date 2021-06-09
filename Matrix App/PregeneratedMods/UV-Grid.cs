using System;
using System.Collections.Generic;
using System.Text;

namespace Matrix_App.PregeneratedMods
{
    class UvGrid : MatrixGifGenerator
    {
        protected override void ColorFragment(in int x, in int y, in float u, in float v, in int frame, out float r, out float g, out float b)
        {
            r = u;
            g = v;
            b = 0.5f;
        }
    }
}
