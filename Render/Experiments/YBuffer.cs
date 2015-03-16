using System.Drawing;

namespace Experiments
{
    public class YBuffer
    {
        public static Bitmap Render()
        {
            var b = new Bitmap(800, 800);

            using (var g = Graphics.FromImage(b))
            {
                g.FillRectangle(Brushes.Black, 0, 0, b.Width, b.Height);
            }

            TestYBuffer(b);

            return b;
        }

        private static void TestYBuffer(Bitmap bmp)
        {
            var yBuffer = new double[bmp.Width];
            for (int i = 0; i < yBuffer.Length; i++)
            {
                yBuffer[i] = double.NegativeInfinity;
            }

            Rasterize(20, 34, 744, 400, bmp, Color.Red, yBuffer);
            Rasterize(120, 434, 444, 400, bmp, Color.Green, yBuffer);
            Rasterize(330, 463, 594, 200, bmp, Color.Blue, yBuffer);

            Rasterize(10, 10, 790, 10, bmp, Color.White, yBuffer);
        }

        private static void Rasterize(int x0, int y0, int x1, int y1, Bitmap bmp, Color color, double[] yBuffer)
        {
            if (x0 == x1)
            {
                return;
            }

            if (x1 < x0)
            {
                int buf;

                buf = x0;
                x0 = x1;
                x1 = buf;

                buf = y0;
                y0 = y1;
                y1 = buf;
            }

            double k = ((double)y1 - y0) / (x1 - x0);
            double y = y0;

            for (int x = x0; x <= x1; x++)
            {
                if (yBuffer[x] < y)
                {
                    yBuffer[x] = y;
                    for (int yOut = 200; yOut < 210; yOut++)
                    {
                        bmp.SetPixel(x, yOut, color);
                    }
                }
                y += k;
            }
        }

    }
}
