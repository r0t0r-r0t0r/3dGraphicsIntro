using System.Drawing;
using System.Numerics;

namespace Render.Shaders
{
    public class SimpleShader : IShader
    {
        private readonly Model _model;
        private readonly Vector3 _light;
        private readonly IShader _innerShader;

        public SimpleShader(Model model, Vector3 light, IShader innerShader)
        {
            _model = model;
            _light = light;
            _innerShader = innerShader;
        }

        public object OnFace(Face face)
        {
            var v0 = _model.Vertices[face[0]];
            var v1 = _model.Vertices[face[1]];
            var v2 = _model.Vertices[face[2]];

            var foo1 = Vector3.Subtract(v1, v0);
            var foo2 = Vector3.Subtract(v2, v1);

            var normal = Vector3.Cross(foo1, foo2);
            normal = Vector3.Normalize(normal);

            var intensity = Vector3.Dot(normal, _light);

            if (intensity < 0)
                intensity = 0;

            return new SimplePixelShaderState
            {
                Intensity = intensity,
                InnerState = _innerShader.OnFace(face)
            };
        }

        public void Vertex(VertexShaderState state, int face, int vert)
        {
            var vertex = _model.GetVertex(face, vert);

            state.Uniform.Push(vertex);

            if (vert == 2)
            {
                var v2 = state.Uniform.PopVector3();
                var v1 = state.Uniform.PopVector3();
                var v0 = state.Uniform.PopVector3();

                var foo1 = Vector3.Subtract(v1, v0);
                var foo2 = Vector3.Subtract(v2, v1);

                var normal = Vector3.Cross(foo1, foo2);
                normal = Vector3.Normalize(normal);

                var intensity = Vector3.Dot(normal, _light);

                if (intensity < 0)
                    intensity = 0;

                state.Uniform.Push(intensity);
            }
        }

        public Color? OnPixel(object state, float a, float b, float c)
        {
            var s = (SimplePixelShaderState)state;
            var color = _innerShader.OnPixel(s.InnerState, a, b, c);

            if (color == null)
                return null;

            var intensity = s.Intensity;

            var resR = (byte)(color.Value.R * intensity);
            var resG = (byte)(color.Value.G * intensity);
            var resB = (byte)(color.Value.B * intensity);

            return Color.FromArgb(resR, resG, resB);
        }
    }
}