using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Render.Shaders
{
    public class TextureShader : Shader
    {
        public override void Face(FaceShaderState state, int face)
        {
        }

        public override Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var geometry = state.World.WorldObject.Model.Geometry;
            var transformation = state.World.Transformation;
            var textureVertex = geometry.GetTextureVertex(face, vert);

            state.Varying.Push(vert, textureVertex.Y);
            state.Varying.Push(vert, textureVertex.X);

            return transformation.Mul(new Vector4(geometry.GetVertex(face, vert), 1));
        }

        public override int? Fragment(FragmentShaderState state)
        {
            var texture = state.World.WorldObject.Model.Texture;
            var tx = (int)(state.Varying.PopFloat() * (texture.Width - 1));
            var ty = (int)(state.Varying.PopFloat() * (texture.Height - 1));

            return texture[tx, ty];
        }
    }
}