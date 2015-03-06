using System.Drawing;
using System.Numerics;

namespace Render.Shaders
{
    public class SolidColorShader : IShader
    {
        private readonly Color _color;
        private readonly Model _model;

        public SolidColorShader(Color color, Model model)
        {
            _color = color;
            _model = model;
        }

        public object OnFace(Face face)
        {
            return null;
        }

        public void Face(FaceShaderState state, int face)
        {
        }

        public Vector3 Vertex(VertexShaderState state, int face, int vert)
        {
            return _model.GetVertex(face, vert);
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