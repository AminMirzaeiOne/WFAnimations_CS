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

        public static void DoMosaic(NonLinearTransfromNeededEventArg e, Animation animation, ref Point[] buffer, ref byte[] pixelsBuffer)
        {
            if (animation.MosaicCoeff == PointF.Empty || animation.MosaicSize == 0)
                return;

            var pixels = e.Pixels;
            System.Int32 sx = e.ClientRectangle.Width;
            System.Int32 sy = e.ClientRectangle.Height;
            System.Int32 s = e.Stride;
            System.Single a = e.CurrentTime;
            System.Int32 count = pixels.Length;
            System.Single opacity = 1 - e.CurrentTime;
            if (opacity < 0f) opacity = 0f;
            if (opacity > 1f) opacity = 1f;
            System.Single mkx = animation.MosaicCoeff.X;
            System.Single mky = animation.MosaicCoeff.Y;

            if (buffer == null)
            {
                buffer = new Point[pixels.Length];
                for (int i = 0; i < pixels.Length; i++)
                    buffer[i] = new Point((int)(mkx * (rnd.NextDouble() - 0.5)), (int)(mky * (rnd.NextDouble() - 0.5)));
            }

            if (pixelsBuffer == null)
                pixelsBuffer = (byte[])pixels.Clone();


            for (int i = 0; i < count; i += bytesPerPixel)
            {
                pixels[i + 0] = 255;
                pixels[i + 1] = 255;
                pixels[i + 2] = 255;
                pixels[i + 3] = 0;
            }

            var ms = animation.MosaicSize;
            var msx = animation.MosaicShift.X;
            var msy = animation.MosaicShift.Y;

            for (int y = 0; y < sy; y++)
                for (int x = 0; x < sx; x++)
                {
                    int yi = (y / ms);
                    int xi = (x / ms);
                    int i = y * s + x * bytesPerPixel;
                    int j = yi * s + xi * bytesPerPixel;

                    var newX = x + (int)(a * (buffer[j].X + xi * msx));
                    var newY = y + (int)(a * (buffer[j].Y + yi * msy));

                    if (newX >= 0 && newX < sx)
                        if (newY >= 0 && newY < sy)
                        {
                            int newI = newY * s + newX * bytesPerPixel;
                            pixels[newI + 0] = pixelsBuffer[i + 0];
                            pixels[newI + 1] = pixelsBuffer[i + 1];
                            pixels[newI + 2] = pixelsBuffer[i + 2];
                            pixels[newI + 3] = (byte)(pixelsBuffer[i + 3] * opacity);
                        }
                }
        }

    }
}
