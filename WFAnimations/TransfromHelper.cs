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

    }
}
