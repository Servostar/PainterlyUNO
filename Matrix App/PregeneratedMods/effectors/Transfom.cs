using System;
using System.Windows.Forms;
using Matrix_App.PregeneratedMods.reflection;
using static Matrix_App.GifGeneratorUtils;

namespace Matrix_App.PregeneratedMods
{
    public sealed class Transfom : MatrixGifGenerator
    {
        [UiWidget]
        [UiDescription(title: "Flip Horizontally", description: "Flips the image in the middle on the horizontal axis")]
        private bool flipHorizontally = false;
        
        [UiWidget]
        [UiDescription(title: "Flip Vertically", description: "Flips the image in the middle on the vertical axis")]
        private bool flipVertically = false;
        
        [UiWidget]
        [UiDescription(title: "Mirror Horizontally", description: "Mirrors the image in the middle on the horizontal axis")]
        private bool mirrorHorizontally = false;
        
        [UiWidget]
        [UiDescription(title: "Mirror Vertically", description: "Mirrors the image in the middle on the vertical axis")]
        private bool mirrorVertically = false;
        
        [UiWidget]
        [UiDescription(title: "Rotation", description: "Rotate counter-clock-wise, repeating the image where needed (at corners)")]
        private int rotation = 0;
        
        [UiWidget]
        [UiDescription(title: "Skew X", description: "Skew the image on the x-axis")]
        private float skewX = 0.0f;
        
        [UiWidget]
        [UiDescription(title: "Skew Y", description: "Skew the image on the y-axis")]
        private float skewY = 0.0f;

        [UiWidget]
        [UiDescription(title: "Scale", description: "Scale up or down")]
        private float scale = 0.5f;
        
        protected override void ColorFragment(in int x, in int y, in float u, in float v, in int frame, out float r, out float g, out float b)
        {
            var sint = MathF.Sin(rotation / 180.0f * MathF.PI);
            var cost = MathF.Cos(rotation / 180.0f * MathF.PI);

            var tx = x;
            var ty = y;
            
            var otx = x - (width >> 1);
            var oty = y - (height >> 1);

            tx = (int) (otx * cost - oty * sint);
            ty = (int) (otx * sint + oty * cost);

            tx += width >> 1;
            ty += height >> 1;
          
            tx = flipVertically ? width - tx - 1 : tx;
            ty = flipHorizontally ? height - ty - 1 : ty;
            tx = mirrorVertically ? tx % (width >> 1) : tx;
            ty = mirrorHorizontally ? ty % (height >> 1) : ty;
          
            tx += (int) (ty * skewX * 2.0f);
            ty += (int) (tx * skewY * 2.0f);
            
            tx = (int) ((tx - (width >> 1)) * scale * 2.0f) + (width >> 1);
            ty = (int) ((ty - (height >> 1)) * scale * 2.0f) + (height >> 1);
            
            tx = Math.Abs(tx) % width;
            ty = Math.Abs(ty) % height;

            SampleFrame(actualStore!, frame, tx, ty, width, out float lr, out float lg, out float lb);

            r = lr;
            g = lg;
            b = lb;
        }

        protected override void CreateUi(FlowLayoutPanel anchor, update invokeGenerator)
        {
            
        }
    }
}