using System.Drawing;
using System.Numerics;

namespace Render.Shaders
{
    public class EmptyShader: IShader
    {
        public object OnFace(Face face)
        {
            return null;
        }

        public void Face(FaceShaderState state, int face)
        {
        }

        public Vector3 Vertex(VertexShaderState state, int face, int vert)
        {
            return new Vector3();
        }

        public Color? Fragment(FragmentShaderState state)
        {
            return null;
        }

        public Color? OnPixel(object state, float a, float b, float c)
        {
            return null;
        }
    }
}
