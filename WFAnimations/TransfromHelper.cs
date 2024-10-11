using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFAnimations
{
    public static class TransfromHelper
    {
        const int bytesPerPixel = 4;
        static Random rnd = new Random();

        public static void DoScale(TransfromNeededEventArg e, Animation animation)
        {
            var rect = e.ClientRectangle;
            var center = new PointF(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            e.Matrix.Translate(center.X, center.Y);
            var kx = 1f - animation.ScaleCoeff.X * e.CurrentTime;
            var ky = 1f - animation.ScaleCoeff.X * e.CurrentTime;
            if (Math.Abs(kx) <= 0.001f) kx = 0.001f;
            if (Math.Abs(ky) <= 0.001f) ky = 0.001f;
            e.Matrix.Scale(kx, ky);
            e.Matrix.Translate(-center.X, -center.Y);
        }

        public static void DoSlide(TransfromNeededEventArg e, Animation animation, bool upside)
        {
            var k = e.CurrentTime;
            if (!upside)
            {
                e.Matrix.Translate(-e.ClientRectangle.Width * k * animation.SlideCoeff.X, -e.ClientRectangle.Height * k * animation.SlideCoeff.Y);
            }
            else
            {
                e.Matrix.Translate(e.ClientRectangle.Width * k * animation.SlideCoeff.X, -e.ClientRectangle.Height * k * animation.SlideCoeff.Y);
            }
        }

        public static void DoBlind(NonLinearTransfromNeededEventArg e, Animation animation)
        {
            if (animation.BlindCoeff == PointF.Empty)
                return;

            var pixels = e.Pixels;
            var sx = e.ClientRectangle.Width;
            var sy = e.ClientRectangle.Height;
            var s = e.Stride;
            var kx = animation.BlindCoeff.X;
            var ky = animation.BlindCoeff.Y;
            var a = (int)((sx * kx + sy * ky) * (1 - e.CurrentTime));

            for (int x = 0; x < sx; x++)
                for (int y = 0; y < sy; y++)
                {
                    int i = y * s + x * bytesPerPixel;
                    var d = x * kx + y * ky - a;
                    if (d >= 0)
                        pixels[i + 3] = (byte)0;
                }
        }

    }
}
