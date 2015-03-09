using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    public struct Texture: IDisposable
    {
        private readonly Bitmap _bitmap;
        private readonly BitmapData _bitmapData;
        private readonly unsafe int* _data;
        private readonly int _width;
        private readonly int _height;

        public unsafe Texture(Image image)
        {
            _bitmap = new Bitmap(image);
            _width = _bitmap.Width;
            _height = _bitmap.Height;
            _bitmapData = _bitmap.LockBits(new Rectangle(0, 0, _width, _height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppRgb);
            _data = (int*) _bitmapData.Scan0;
        }

        public unsafe int* Data
        {
            get { return _data; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public unsafe int this[int x, int y]
        {
            get
            {
                var pos = ((_height - y - 1)*_width + x);
                return _data[pos];
            }
        }

        public void Dispose()
        {
            _bitmap.UnlockBits(_bitmapData);
        }
    }

    public static class ArgbUtils
    {
        public static byte GetRed(this int argb)
        {
            return (byte)((argb & 0x00ff0000) >> 16);
        }
    }
}
