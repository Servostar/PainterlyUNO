using System.Windows.Forms;
using static Matrix_App.GifGeneratorUtils;

namespace Matrix_App.PregeneratedMods
{
    public sealed class Invert : MatrixGifGenerator
    {
        protected override void ColorFragment(in int x, in int y, in float u, in float v, in int frame, out float r, out float g, out float b)
        {
            SampleFrame(actualStore!, frame, x, y, width, out float lr, out float lg, out float lb);

            r = 1 - lr;
            g = 1 - lg;
            b = 1 - lb;
        }

        protected override void CreateUi(FlowLayoutPanel anchor, update invokeGenerator)
        {
            
        }
    }
}
