using System.Drawing;
using System.Numerics;

namespace Render.Shaders
{
    public abstract class Shader
    {
        public abstract void Face(FaceShaderState state, int face);
        public abstract Vector4 Vertex(VertexShaderState state, int face, int vert);
        public abstract int? Fragment(FragmentShaderState state);
    }
}