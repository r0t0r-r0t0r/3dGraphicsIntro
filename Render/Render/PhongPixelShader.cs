using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Render
{
    public class PhongPixelShader : IPixelShader
    {
        private readonly Model _model;
        private readonly Vector3 _light;
        private readonly IPixelShader _innerShader;

        public PhongPixelShader(Model model, Vector3 light, IPixelShader innerShader)
        {
            _model = model;
            _light = light;
            _innerShader = innerShader;
        }

        public object OnFace(Face face)
        {
            var vertexNormals = Enumerable.Range(0, 3).Select(face.GetNormalIndex).Select(x => _model.VertexNormals[x]).ToArray();

            return new PhongPixelShaderState
            {
                VertexNormals = vertexNormals,
                InnerState = _innerShader.OnFace(face)
            };
        }

        public Color? OnPixel(object state, float a, float b, float c)
        {
            var s = (PhongPixelShaderState) state;
            var color = _innerShader.OnPixel(s.InnerState, a, b, c);

            if (color == null)
                return null;

            var vns = s.VertexNormals;

            var nx = vns[0].X * a + vns[1].X * b + vns[2].X * c;
            var ny = vns[0].Y * a + vns[1].Y * b + vns[2].Y * c;
            var nz = vns[0].Z * a + vns[1].Z * b + vns[2].Z * c;
            var normal = new Vector3(nx, ny, nz);

            var intensity = Vector3.Dot(normal, _light);
            if (intensity < 0)
                return null;

            if (intensity > 1)
                intensity = 1;
            var resR = (byte)(color.Value.R * intensity);
            var resG = (byte)(color.Value.G * intensity);
            var resB = (byte)(color.Value.B * intensity);

            var resColor = Color.FromArgb(resR, resG, resB);

            return resColor;
        }
    }
}