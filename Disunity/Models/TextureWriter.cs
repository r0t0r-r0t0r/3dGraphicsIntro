namespace Disunity.Models
{
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
}