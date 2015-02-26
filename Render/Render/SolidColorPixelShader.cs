using System.Drawing;

namespace Render
{
    public class SolidColorPixelShader : IPixelShader
    {
        private readonly Color _color;

        public SolidColorPixelShader(Color color)
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