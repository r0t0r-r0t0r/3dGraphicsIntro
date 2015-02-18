using System;
using System.Drawing;
using System.Numerics;

namespace Render
{
    public class WireRender: IRender
    {
        public void Init(Model model, Bitmap texture, int width, int height, string rootDir)
        {
        }

        public void Draw(Face face, Vector3 a, Vector3 b, Vector3 c, Bitmap bitmap, byte lightLevel)
        {
            var screenCoords = new[] {a, b, c};

            Line((int)screenCoords[0].X, (int)screenCoords[0].Y, (int)screenCoords[1].X, (int)screenCoords[1].Y, bitmap, Color.White);
            Line((int)screenCoords[1].X, (int)screenCoords[1].Y, (int)screenCoords[2].X, (int)screenCoords[2].Y, bitmap, Color.White);
            Line((int)screenCoords[2].X, (int)screenCoords[2].Y, (int)screenCoords[0].X, (int)screenCoords[0].Y, bitmap, Color.White);
        }

        public void Finish()
        {
        }

        private static void Line(int x0, int y0, int x1, int y1, Bitmap bmp, Color color)
        {
            var vertOrientation = Math.Abs(x1 - x0) < Math.Abs(y1 - y0);

            if (vertOrientation)
            {
                int buf;

                buf = x0;
                x0 = y0;
                y0 = buf;

                buf = x1;
                x1 = y1;
                y1 = buf;
            }
            if (x0 > x1)
            {
                int buf;

                buf = x0;
                x0 = x1;
                x1 = buf;

                buf = y0;
                y0 = y1;
                y1 = buf;
            }

            int errorMul = 0;

            int sign = Math.Sign(y1 - y0);

            int kMul = 2 * (y1 - y0);
            int halfMul = x1 - x0;
            int oneMul = 2 * halfMul;

            int y = y0;
            for (var x = x0; x <= x1; x++)
            {
                errorMul += kMul;
                if (Math.Abs(errorMul) > halfMul)
                {
                    y += sign;
                    errorMul -= sign * oneMul;
                }

                if (vertOrientation)
                {
                    if (y >= 0 && y < bmp.Width && x >= 0 && x < bmp.Height)
                        bmp.SetPixel(y, x, color);
                }
                else
                {
                    if (x >= 0 && x < bmp.Width && y >= 0 && y < bmp.Height)
                        bmp.SetPixel(x, y, color);
                }
            }
        }
    }
}