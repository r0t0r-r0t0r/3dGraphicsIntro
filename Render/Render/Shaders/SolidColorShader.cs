using System.Drawing;
using System.Numerics;

namespace Render.Shaders
{
    public class SolidColorShader : IShader
    {
        private readonly Color _color;
        private readonly Geometry _geometry;
        private readonly Matrix4x4 _transformation;

        public SolidColorShader(Color color, Model model, Matrix4x4 transformation)
        {
            _color = color;
            _geometry = model.Geometry;
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
            return Matrix4x4Utils.Mul(_transformation, new Vector4(_geometry.GetVertex(face, vert), 1));
        }

        public Color? Fragment(FragmentShaderState state)
        {
            return _color;
        }

        public Color? OnPixel(object state, float a, float b, float c)
        {
            return _color;
        }
    }
}