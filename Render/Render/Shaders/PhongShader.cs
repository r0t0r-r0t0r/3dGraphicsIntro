using System;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Render.Shaders
{
    public class PhongShader : IShader
    {
        private readonly Geometry _geometry;
        private readonly Vector3 _light;
        private readonly IShader _innerShader;

        public PhongShader(Model model, Vector3 light, IShader innerShader)
        {
            _geometry = model.Geometry;
            _light = light;
            _innerShader = innerShader;
        }

        public void Face(FaceShaderState state, int face)
        {
        }

        public Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var normal = _geometry.GetVertexNormal(face, vert);
            state.Varying[vert].Push(normal);

            return _innerShader.Vertex(state, face, vert);
        }

        public Color? Fragment(FragmentShaderState state)
        {
            var color = _innerShader.Fragment(state);

            if (color == null)
                return null;

            var normal = state.Varying.PopVector3();
            normal = Vector3.Normalize(normal);

            var intensity = Vector3.Dot(normal, _light);
            if (intensity < 0)
                return Color.Black;

            if (intensity >= 1)
                intensity = 1;

            //const float intensityStepsCount = 5;
            //intensity = (float)Math.Floor(intensity * intensityStepsCount) / intensityStepsCount;

            var resR = (byte)(color.Value.R * intensity);
            var resG = (byte)(color.Value.G * intensity);
            var resB = (byte)(color.Value.B * intensity);

            return Color.FromArgb(resR, resG, resB);
        }
    }
}