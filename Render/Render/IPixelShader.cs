using System.Drawing;

namespace Render
{
    public interface IPixelShader
    {
        object OnFace(Face face);
        Color? OnPixel(object state, float a, float b, float c);
    }
}