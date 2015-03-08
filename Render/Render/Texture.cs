using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    public struct Texture
    {
        private readonly unsafe int* _data;
        private readonly int _width;
        private readonly int _height;

        public unsafe Texture(int* data, int width, int height)
        {
            _data = data;
            _width = width;
            _height = height;
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
    }

    public static class ArgbUtils
    {
        public static byte GetRed(this int argb)
        {
            return (byte)((argb & 0x00ff0000) >> 16);
        }
    }
}
