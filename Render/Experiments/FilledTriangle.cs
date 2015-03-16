using System;
using System.Drawing;
using System.Numerics;

namespace Experiments
{
    public class FilledTriangle
    {
        public static Bitmap Render()
        {
            var b = new Bitmap(300, 300);

            using (var g = Graphics.FromImage(b))
            {
                g.FillRectangle(Brushes.Black, 0, 0, b.Width, b.Height);
            }

            TestTriangle(b);

            return b;
        }

        private static void TestTriangle(Bitmap bmp)
        {
            Vector2[] t0 = { new Vector2(10, 70), new Vector2(50, 160), new Vector2(70, 80) };
            Vector2[] t1 = { new Vector2(180, 50), new Vector2(150, 1), new Vector2(70, 180) };
            Vector2[] t2 = { new Vector2(180, 150), new Vector2(120, 160), new Vector2(130, 180) };
            Vector2[] t3 = { new Vector2(200, 100), new Vector2(200, 150), new Vector2(220, 180) };
            Vector2[] t4 = { new Vector2(200, 50), new Vector2(220, 50), new Vector2(150, 100) };

            Triangle(t0[0], t0[1], t0[2], bmp, Color.Red);
            Triangle(t1[0], t1[1], t1[2], bmp, Color.White);
            Triangle(t2[0], t2[1], t2[2], bmp, Color.Green);
            Triangle(t3[0], t3[1], t3[2], bmp, Color.Blue);
            Triangle(t4[0], t4[1], t4[2], bmp, Color.Yellow);
        }

        private static void Triangle(Vector2 t0, Vector2 t1, Vector2 t2, Bitmap bmp, Color color)
        {
            Triangle((int)t0.X, (int)t0.Y, (int)t1.X, (int)t1.Y, (int)t2.X, (int)t2.Y, bmp, color);
        }

        private static void Triangle(int x0, int y0, int x1, int y1, int x2, int y2, Bitmap bmp, Color color)
        {
            int minX = Math3(x0, x1, x2, Math.Min);
            int minY = Math3(y0, y1, y2, Math.Min);
            int maxX = Math3(x0, x1, x2, Math.Max);
            int maxY = Math3(y0, y1, y2, Math.Max);

            double xmid = ((double)x0 + x1 + x2) / 3;
            double ymid = ((double)y0 + y1 + y2) / 3;

            var v0 = new Vector3((float)(x0 - xmid), (float)(y0 - ymid), 0);
            var v1 = new Vector3((float)(x1 - xmid), (float)(y1 - ymid), 0);

            var direction = Vector3.Cross(v0, v1).Z;
            if (direction < 0)
            {
                int buf;

                buf = x1;
                x1 = x2;
                x2 = buf;

                buf = y1;
                y1 = y2;
                y2 = buf;
            }

            var line1 = MakeLineFunc(x0, y0, x1, y1);
            var line2 = MakeLineFunc(x1, y1, x2, y2);
            var line3 = MakeLineFunc(x2, y2, x0, y0);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    var curr1 = line1(x, y);
                    var curr2 = line2(x, y);
                    var curr3 = line3(x, y);

                    if (curr1 >= 0 && curr2 >= 0 && curr3 >= 0)
                    {
                        bmp.SetPixel(x, y, color);
                    }
                }
            }
        }

        private static Func<double, double, double> MakeLineFunc(double x0, double y0, double x1, double y1)
        {
            return (x, y) => y * (x1 - x0) - x * (y1 - y0) - y0 * (x1 - x0) + x0 * (y1 - y0);
        }

        private static T Math3<T>(T a, T b, T c, Func<T, T, T> func)
        {
            var result = func(a, b);
            return func(result, c);
        }

    }
}
