using System.Drawing;

namespace Render.Shaders
{
    public interface IShader
    {
        object OnFace(Face face);
        void Face(FaceShaderState state, int face);
        void Vertex(VertexShaderState state, int face, int vert);
        Color? Fragment(FragmentShaderState state);
        Color? OnPixel(object state, float a, float b, float c);
    }
}