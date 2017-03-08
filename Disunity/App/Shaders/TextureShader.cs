using System.Numerics;
using Disunity.Data;
using Disunity.Data.Shaders;

namespace Disunity.App.Shaders
{
    public class TextureShader : Shader
    {
        private Geometry _geometry;
        private Matrix4x4 _transformation;
        private Texture _texture;

        public override void World(World world)
        {
            _geometry = world.WorldObject.Model.Geometry;
            _transformation = world.GetTransform();
            _texture = world.WorldObject.Model.Texture;
        }

        public override void Face(FaceShaderState state, int face)
        {
        }

        public override Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var textureVertex = _geometry.GetTextureVertex(face, vert);

            state.Varying.Push(vert, textureVertex.Y);
            state.Varying.Push(vert, textureVertex.X);

            return _transformation.Mul(new Vector4(_geometry.GetVertex(face, vert), 1));
        }

        public override int? Fragment(FragmentShaderState state)
        {
            var tx = (int)(state.Varying.PopFloat() * (_texture.Width - 1));
            var ty = (int)(state.Varying.PopFloat() * (_texture.Height - 1));

            return _texture[tx, ty];
        }
    }
}