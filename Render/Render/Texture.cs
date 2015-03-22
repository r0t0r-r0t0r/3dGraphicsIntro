using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
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

    public class WritableTexture: Texture
    {
        public WritableTexture(Bitmap bitmap, bool allowRead) : base(bitmap, allowRead ? ImageLockMode.ReadWrite : ImageLockMode.WriteOnly)
        {
        }

        public unsafe TextureWriter GetWriter(int minY)
        {
            return new TextureWriter(_data, _width, _height, minY);
        }

        public unsafe void Write(int x, int y, int color)
        {
            var foo = ((_height - y - 1) * _width + x);
            _data[foo] = color;
        }

        public unsafe void Clear()
        {
            var length = _width * _height;
            for (var i = 0; i < length; i++)
            {
                _data[i] = 0;
            }
        }
    }

    public struct TextureWriter
    {
        private readonly int _width;
        private unsafe int* _line;

        public unsafe TextureWriter(int* data, int width, int height, int minY)
        {
            _width = width;
            _line = data + (height - minY - 1) * width;
        }

        public unsafe void Write(int x, int color)
        {
            _line[x] = color;
        }

        public unsafe void NextLine()
        {
            _line -= _width;
        }
    }

    public static class ArgbUtils
    {
        public static byte GetRed(this int argb)
        {
            return (byte)((argb & 0x00ff0000) >> 16);
        }
    }

    public static class TextureUtils
    {
        public static int ClipX(this Texture texture, int x)
        {
            return Math.Min(Math.Max(0, x), texture.Width - 1);
        }

        public static int ClipY(this Texture texture, int y)
        {
            return Math.Min(Math.Max(0, y), texture.Height - 1);
        }
    }
}
