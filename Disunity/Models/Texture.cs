using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Disunity.Models
{
    public class Texture: IDisposable
    {
        private readonly Bitmap _bitmap;
        protected readonly BitmapData _bitmapData;
        protected readonly unsafe int* _data;
        protected readonly int _width;
        protected readonly int _height;

        public Texture(Image image) : this(new Bitmap(image), ImageLockMode.ReadOnly)
        {
        }

        protected unsafe Texture(Bitmap bitmap, ImageLockMode imageLockMode)
        {
            _bitmap = bitmap;
            _width = _bitmap.Width;
            _height = _bitmap.Height;
            _bitmapData = _bitmap.LockBits(new Rectangle(0, 0, _width, _height), imageLockMode,
                PixelFormat.Format32bppRgb);
            _data = (int*)_bitmapData.Scan0;
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
}
