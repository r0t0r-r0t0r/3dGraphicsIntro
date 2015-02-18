using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Experiments
{
    public class Line
    {
        public static Bitmap Render()
        {
            var b = new Bitmap(100, 100);

            using (var g = Graphics.FromImage(b))
            {
                g.FillRectangle(Brushes.Black, 0, 0, b.Width, b.Height);
            }

            TestLine(b);

            return b;
        }

        private static void TestLine(Bitmap b)
        {
            DrawLine(50, 50, 50, 80, b, Color.White);

            DrawLine(50, 50, 60, 80, b, Color.White);
            DrawLine(50, 50, 80, 80, b, Color.White);
            DrawLine(50, 50, 80, 60, b, Color.White);

            DrawLine(50, 50, 80, 50, b, Color.White);

            DrawLine(50, 50, 80, 40, b, Color.White);
            DrawLine(50, 50, 80, 20, b, Color.White);
            DrawLine(50, 50, 60, 20, b, Color.White);

            DrawLine(50, 50, 50, 20, b, Color.White);

            DrawLine(50, 50, 40, 20, b, Color.White);
            DrawLine(50, 50, 20, 20, b, Color.White);
            DrawLine(50, 50, 20, 40, b, Color.White);

            DrawLine(50, 50, 20, 50, b, Color.White);

            DrawLine(50, 50, 20, 60, b, Color.White);
            DrawLine(50, 50, 20, 80, b, Color.White);
            DrawLine(50, 50, 40, 80, b, Color.White);
        }

        private static void DrawLine(int x0, int y0, int x1, int y1, Bitmap bmp, Color color)
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
                    bmp.SetPixel(y, x, color);
                }
                else
                {
                    bmp.SetPixel(x, y, color);
                }
            }
        }
    }
}
