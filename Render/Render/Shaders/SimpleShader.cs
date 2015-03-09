using System.Drawing;
using System.Numerics;

namespace Render.Shaders
{
    public class SimpleShader : IShader
    {
        private readonly Geometry _geometry;
        private readonly Vector3 _light;
        private readonly IShader _innerShader;

        public SimpleShader(Model model, Vector3 light, IShader innerShader)
        {
            _geometry = model.Geometry;
            _light = light;
            _innerShader = innerShader;
        }

        public void Face(FaceShaderState state, int face)
        {
            var v0 = _geometry.GetVertex(face, 0);
            var v1 = _geometry.GetVertex(face, 1);
            var v2 = _geometry.GetVertex(face, 2);

            var foo1 = Vector3.Subtract(v1, v0);
            var foo2 = Vector3.Subtract(v2, v1);

            var normal = Vector3.Cross(foo1, foo2);
            normal = Vector3.Normalize(normal);

            var intensity = Vector3.Dot(normal, _light);

            if (intensity < 0)
                intensity = 0;

            state.Intensity = intensity;
        }

        public Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            return _innerShader.Vertex(state, face, vert);
        }

        public Color? Fragment(FragmentShaderState state)
        {
            var color = _innerShader.Fragment(state);
            if (color == null)
                return null;

            var intensity = state.Intensity;

            var resR = (byte)(color.Value.R * intensity);
            var resG = (byte)(color.Value.G * intensity);
            var resB = (byte)(color.Value.B * intensity);

            return Color.FromArgb(resR, resG, resB);
        }
    }
}