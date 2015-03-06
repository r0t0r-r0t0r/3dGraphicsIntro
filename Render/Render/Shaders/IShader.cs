using System.Drawing;

namespace Render.Shaders
{
    public interface IShader
    {
        object OnFace(Face face);
        void Vertex(VertexShaderState state, int face, int vert);
        Color? OnPixel(object state, float a, float b, float c);
    }
}