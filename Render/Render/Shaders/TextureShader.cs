using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Render.Shaders
{
    public class TextureShader : IShader
    {
        private readonly Model _model;
        private readonly unsafe int* _texture;
        private readonly int _textureWidth;
        private readonly int _textureHeight;
        private readonly Matrix4x4 _transformation;

        unsafe public TextureShader(Model model, byte* texture, int textureWidth, int textureHeight, Matrix4x4 transformation)
        {
            _model = model;
            _texture = (int*)texture;
            _textureWidth = textureWidth;
            _textureHeight = textureHeight;
            _transformation = transformation;
        }

        public void Face(FaceShaderState state, int face)
        {
        }

        public Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var textureVertex = _model.GetTextureVertex(face, vert);

            state.Varying.Push(vert, textureVertex.Y);
            state.Varying.Push(vert, textureVertex.X);

            return Matrix4x4Utils.Mul(_transformation, new Vector4(_model.GetVertex(face, vert), 1));
        }

        public unsafe Color? Fragment(FragmentShaderState state)
        {
            var tx = (int)(state.Varying.PopFloat() * (_textureWidth - 1));
            var ty = (int)(state.Varying.PopFloat() * (_textureHeight - 1));

            var pos = ((_textureHeight - ty - 1) * _textureWidth + tx);
            var tcolor = _texture[pos];

            return Color.FromArgb(tcolor);
        }
    }
}