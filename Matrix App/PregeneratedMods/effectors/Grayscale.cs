﻿using System.Windows.Forms;
using Matrix_App.PregeneratedMods.reflection;
using static Matrix_App.GifGeneratorUtils;

namespace Matrix_App.PregeneratedMods
{
    public sealed class Grayscale : MatrixGifGenerator
    {
        [UiWidget]
        [UiDescription(title: "use Luminance", description: "Use luminance as defined by ITU-R BT.709 as grayscale output")]
        private bool byLuminance = false;

        protected override void ColorFragment(in int x, in int y, in float u, in float v, in int frame, out float r, out float g, out float b)
        {
            SampleFrame(actualStore!, frame, x, y, width, out float lr, out float lg, out float lb);

            if (byLuminance)
            {
                float luminance = 0.2126f * lr + 0.7152f * lg + 0.0722f * lb;

                r = luminance;
                g = luminance;
                b = luminance;
            } else
            {
                float average = (lr + lg + lb) * 0.3333f;

                r = average;
                g = average;
                b = average;
            }
        }

        protected override void CreateUi(FlowLayoutPanel anchor, update invokeGenerator)
        {
            
        }
    }
}
