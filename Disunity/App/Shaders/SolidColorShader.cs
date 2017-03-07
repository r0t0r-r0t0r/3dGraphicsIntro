using System.Drawing;
using System.Numerics;

namespace Disunity.App.Shaders
{
    public class SolidColorShader : Shader
    {
        private readonly int _color = Color.White.ToArgb();

        private Matrix4x4 _transformation;
        private Geometry _geometry;

        public override void World(World world)
        {
            _transformation = world.GetTransform();
            _geometry = world.WorldObject.Model.Geometry;
        }

        public override void Face(FaceShaderState state, int face)
        {
        }

        public override Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            return _transformation.Mul(new Vector4(_geometry.GetVertex(face, vert), 1));
        }

        public override int? Fragment(FragmentShaderState state)
        {
            return _color;
        }
    }
}