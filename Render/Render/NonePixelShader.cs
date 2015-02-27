using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Render
{
    public class TexturePixelShader : IPixelShader
    {
        private readonly Model _model;
        private readonly unsafe int* _texture;
        private readonly int _textureWidth;
        private readonly int _textureHeight;

        unsafe public TexturePixelShader(Model model, byte* texture, int textureWidth, int textureHeight)
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