using System.Drawing;
using System.Numerics;

namespace Render.Shaders
{
    public class EmptyShader: IShader
    {
        private readonly Model _model;
        private readonly Matrix4x4 _transformation;

        public EmptyShader(Model model, Matrix4x4 transformation)
        {
            _model = model;
            _transformation = transformation;
        }

        public object OnFace(Face face)
        {
            return null;
        }

        public void Face(FaceShaderState state, int face)
        {
        }

        public Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            return Matrix4x4Utils.Mul(_transformation, new Vector4(_model.GetVertex(face, vert), 1));
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
