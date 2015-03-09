using System.Drawing;
using System.Numerics;

namespace Render.Shaders
{
    public class SolidColorShader : IShader
    {
        private readonly Color _color = Color.White;

        public void Face(FaceShaderState state, int face)
        {
        }

        public Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var transformation = state.World.Transformation;
            var geometry = state.World.WorldObject.Model.Geometry;
            return transformation.Mul(new Vector4(geometry.GetVertex(face, vert), 1));
        }

        public Color? Fragment(FragmentShaderState state)
        {
            return _color;
        }
    }
}