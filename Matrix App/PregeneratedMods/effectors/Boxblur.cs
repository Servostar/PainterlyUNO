﻿using System;
using System.Windows.Forms;
using Matrix_App.PregeneratedMods.reflection;
using static Matrix_App.GifGeneratorUtils;

namespace Matrix_App.PregeneratedMods
{
    public sealed class Boxblur : MatrixGifGenerator
    {
        [UiWidget]
        [UiDescription(title: "Blur size", description: "The side length of the bounding square used to blur in pixels")]
        private int blurSize = 2;

        protected override void ColorFragment(in int x, in int y, in float u, in float v, in int frame, out float r, out float g, out float b)
        {
            float avrR = 0;
            float avrG = 0;
            float avrB = 0;

            if (blurSize > 0)
            {
                for (int i = -blurSize; i < blurSize; i++)
                {
                    int s = Math.Clamp(x + i, 0, width - 1);

                    for (int j = -blurSize; j < blurSize; j++)
                    {
                        int t = Math.Clamp(y + j, 0, height - 1);

                        SampleFrame(actualStore, frame, s, t, width, out float tmpR, out float tmpG, out float tmpB);

                        avrR += tmpR;
                        avrG += tmpG;
                        avrB += tmpB;
                    }
                }

                float blurKernelArea = 1.0f / ((blurSize << 1) * (blurSize << 1));

                r = avrR * blurKernelArea;
                g = avrG * blurKernelArea;
                b = avrB * blurKernelArea;
            } else
            {
                SampleFrame(actualStore, frame, x, y, width, out float tmpR, out float tmpG, out float tmpB);

                r = tmpR;
                g = tmpG;
                b = tmpB;
            }
        }

        protected override void CreateUi(FlowLayoutPanel anchor, update invokeGenerator)
        {
            
        }
    }
}
