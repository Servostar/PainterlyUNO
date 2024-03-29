﻿using System;
using System.Windows.Forms;
using Matrix_App.PregeneratedMods.reflection;
using static Matrix_App.GifGeneratorUtils;

namespace Matrix_App.PregeneratedMods
{
    public class ColorAdjust : MatrixGifGenerator
    {
        [UiWidget]
        [UiDescription(title: "Contrast", description: "Constrasts the image, or loweres difference")]
        private float contrast = 0.5f;
        
        [UiWidget]
        [UiDescription(title: "Tone offset", description: "Sets an additional offset to the pixels hue")]
        private float hueOffset = 0.0f;
        
        [UiWidget]
        [UiDescription(title: "Saturation boost", description: "Decreases or increases (chroma-based) saturation")]
        private float saturationBoost = 0.5f;
        
        [UiWidget]
        [UiDescription(title: "Luminance boost", description: "Decreases or increases luminance")]
        private float valueBoost = 0.5f;

        [UiWidget]
        [UiDescription(title: "Red boost", description: "Decreases or increases Red")]
        private float redBoost = 0.5f;
        
        [UiWidget]
        [UiDescription(title: "Green boost", description: "Decreases or increases Green")]
        private float greenBoost = 0.5f;
        
        [UiWidget]
        [UiDescription(title: "Blue boost", description: "Decreases or increases Blue")]
        private float blueBoost = 0.5f;

        private static float Boost(float x, float y)
        {
            return Math.Clamp(x + (y - 0.5f) * 2.0f, 0.0f, 1.0f);
        }

        protected override void ColorFragment(in int x, in int y, in float u, in float v, in int frame, out float r, out float g, out float b)
        {
            SampleFrame(actualStore!, frame, x, y, width, out float tr, out float tg, out float tb);

            // Adjust HSV
            HslFromRgb(tr, tg, tb, out float h, out float s, out float value);

            h = h / 360.0f + hueOffset;
            h = (h - MathF.Floor(h)) * 360.0f;
            s = Boost(s, saturationBoost);
            value = Boost(value, valueBoost);

            // Adjust RGB
            RgbFromHsl(h, s, value, out tr, out tg, out tb);

            r = Boost(tr, redBoost);
            g = Boost(tg, greenBoost);
            b = Boost(tb, blueBoost);

            float enhancedContrast = contrast * 10.0f;
            
            r = MathF.Pow(r, enhancedContrast) * enhancedContrast;
            g = MathF.Pow(g, enhancedContrast) * enhancedContrast;
            b = MathF.Pow(b, enhancedContrast) * enhancedContrast;
        }

        protected override void CreateUi(FlowLayoutPanel anchor, update invokeGenerator)
        {
            
        }
    }
}
