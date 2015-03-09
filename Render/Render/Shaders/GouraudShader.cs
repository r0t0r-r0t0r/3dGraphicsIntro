using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Render.Shaders
{
    public class GouraudShader : IShader
    {
        private readonly Geometry _geometry;
        private readonly Vector3 _light;
        private readonly IShader _innerShader;

        public GouraudShader(Geometry geometry, Vector3 light, IShader innerShader)
        {
            _geometry = geometry;
            _light = light;
            _innerShader = innerShader;
        }

        public void Face(FaceShaderState state, int face)
        {
        }

        public Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var normal = _geometry.GetVertexNormal(face, vert);
            var intensity = Vector3.Dot(normal, _light);

            state.Varying.Push(vert, intensity);

            return _innerShader.Vertex(state, face, vert);
        }

        public Color? Fragment(FragmentShaderState state)
        {
            var color = _innerShader.Fragment(state);

            if (color == null)
                return null;

            var intensity = state.Varying.PopFloat();

            if (intensity < 0)
                return Color.Black;

            if (intensity > 1)
                intensity = 1;

            var resR = (byte)(color.Value.R * intensity);
            var resG = (byte)(color.Value.G * intensity);
            var resB = (byte)(color.Value.B * intensity);

            return Color.FromArgb(resR, resG, resB);
        }
    }
}