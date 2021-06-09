using System;
using System.Collections.Generic;
using System.Text;

namespace Matrix_App.PregeneratedMods
{
    public sealed class Spiral : MatrixGifGenerator
    {
        /**
         float spiral(vec2 m) {
	        float r = length(m);
	        float a = atan(m.y, m.x);
	        float v = sin(100.*(sqrt(r)-0.02*a-.3*t));
	        return clamp(v,0.,1.);
        }
         */
        private float SpiralCurve(float s, float t, float time)
        {
            float r = MathF.Sqrt(s * s + t * t);
            float a = MathF.Atan2(t, s);
            float v = MathF.Sin(50 * (MathF.Sqrt(r) - 0.02f * a - time));

            return v;
        }

        protected override void ColorFragment(in int x, in int y, in float u, in float v, in int frame, out float r, out float g, out float b)
        {
            float sp = SpiralCurve((u - 0.5f) * 0.1f, (v - 0.5f) * 0.1f, frame / (float) totalFrames);

            r = sp;
            g = sp;
            b = sp;
        }
    }
}
