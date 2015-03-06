using System.Drawing;
using System.Linq;

namespace Render.Shaders
{
    public class TextureShader : IShader
    {
        private readonly Model _model;
        private readonly unsafe int* _texture;
        private readonly int _textureWidth;
        private readonly int _textureHeight;

        unsafe public TextureShader(Model model, byte* texture, int textureWidth, int textureHeight)
        {
            _model = model;
            _texture = (int*)texture;
            _textureWidth = textureWidth;
            _textureHeight = textureHeight;
        }

        public object OnFace(Face face)
        {
            var textureVertices = Enumerable.Range(0, 3).Select(face.GetVtIndex).Select(x => _model.TextureVertices[x]).ToArray();

            return new TexturePixelShaderState
            {
                TextureVertices = textureVertices,
            };
        }

        public void Face(FaceShaderState state, int face)
        {
        }

        public void Vertex(VertexShaderState state, int face, int vert)
        {
            var textureVertex = _model.GetTextureVertex(face, vert);

            state.Varying.Push(vert, textureVertex.Y);
            state.Varying.Push(vert, textureVertex.X);
        }

        public unsafe Color? Fragment(FragmentShaderState state)
        {
            var tx = (int)state.Varying.PopFloat();
            var ty = (int)state.Varying.PopFloat();

            var pos = ((_textureHeight - ty - 1) * _textureWidth + tx);
            var tcolor = _texture[pos];
            var color = Color.FromArgb(tcolor);

            return Color.FromArgb(color.R, color.G, color.B);
        }

        public unsafe Color? OnPixel(object state, float a, float b, float c)
        {
            var s = (TexturePixelShaderState)state;

            var textureVertices = s.TextureVertices;
            var tx0 = textureVertices[0].X;
            var ty0 = textureVertices[0].Y;
            var tx1 = textureVertices[1].X;
            var ty1 = textureVertices[1].Y;
            var tx2 = textureVertices[2].X;
            var ty2 = textureVertices[2].Y;

            var tx = (int)((a * tx0 + b * tx1 + c * tx2) * (_textureWidth - 1));
            var ty = (int)((a * ty0 + b * ty1 + c * ty2) * (_textureHeight - 1));

            var pos = ((_textureHeight - ty - 1)*_textureWidth + tx);
            var tcolor = _texture[pos];
            var color = Color.FromArgb(tcolor);

            return Color.FromArgb(color.R, color.G, color.B);
        }
    }
}