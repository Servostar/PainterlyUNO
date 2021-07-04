
using System;
using System.Windows.Forms;

namespace Matrix_App.PregeneratedMods
{
    public sealed class UvGrid : MatrixGifGenerator
    {
        protected override void ColorFragment(in int x, in int y, in float u, in float v, in int frame, out float r, out float g, out float b)
        {
            r = u;
            g = v;
            b = MathF.Sin(frame / (float) totalFrames * MathF.PI);
        }

        protected override void CreateUi(FlowLayoutPanel anchor, update invokeGenerator)
        {
            
        }
    }
}
