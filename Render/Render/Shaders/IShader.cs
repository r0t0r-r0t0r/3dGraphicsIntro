using System.Drawing;
using System.Numerics;

namespace Render.Shaders
{
    public interface IShader
    {
        void Face(FaceShaderState state, int face);
        Vector4 Vertex(VertexShaderState state, int face, int vert);
        Color? Fragment(FragmentShaderState state);
    }
}