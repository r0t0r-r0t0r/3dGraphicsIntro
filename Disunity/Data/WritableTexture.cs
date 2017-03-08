using System.Drawing;
using System.Drawing.Imaging;

namespace Disunity.Models
{
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
}