using System.Drawing;

namespace Render
{
    public class SolidColorShader : IShader
    {
        private readonly Color _color;

        public SolidColorShader(Color color)
        {
            _color = color;
        }

        public object OnFace(Face face)
        {
            return null;
        }

        public Color? OnPixel(object state, float a, float b, float c)
        {
            return _color;
        }
    }
}