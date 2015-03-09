using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Render.Shaders
{
    public class TextureShader : IShader
    {
        private readonly Geometry _geometry;
        private readonly Matrix4x4 _transformation;
        private readonly Texture _texture;

        public TextureShader(Model model, Matrix4x4 transformation)
        {
            _geometry = model.Geometry;
            _texture = model.Texture;
            _transformation = transformation;
        }

        public void Face(FaceShaderState state, int face)
        {
        }

        public Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var textureVertex = _geometry.GetTextureVertex(face, vert);

            state.Varying.Push(vert, textureVertex.Y);
            state.Varying.Push(vert, textureVertex.X);

            return Matrix4x4Utils.Mul(_transformation, new Vector4(_geometry.GetVertex(face, vert), 1));
        }

        public Color? Fragment(FragmentShaderState state)
        {
            var tx = (int)(state.Varying.PopFloat() * (_texture.Width - 1));
            var ty = (int)(state.Varying.PopFloat() * (_texture.Height - 1));

            var tcolor = _texture[tx, ty];

            return Color.FromArgb(tcolor);
        }
    }
}