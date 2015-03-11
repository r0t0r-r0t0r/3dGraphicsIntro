using System.Drawing;
using System.Numerics;

namespace Render.Shaders
{
    public class SolidColorShader : Shader
    {
        private readonly Color _color = Color.White;

        public override void Face(FaceShaderState state, int face)
        {
        }

        public override Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var transformation = state.World.Transformation;
            var geometry = state.World.WorldObject.Model.Geometry;
            return transformation.Mul(new Vector4(geometry.GetVertex(face, vert), 1));
        }

        public override Color? Fragment(FragmentShaderState state)
        {
            return _color;
        }
    }
}