using System;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Render.Shaders
{
    public class PhongShader : IShader
    {
        private readonly IShader _innerShader;

        public PhongShader(IShader innerShader)
        {
            _innerShader = innerShader;
        }

        public void Face(FaceShaderState state, int face)
        {
        }

        public Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var geometry = state.World.WorldObject.Model.Geometry;
            var normal = geometry.GetVertexNormal(face, vert);
            state.Varying[vert].Push(normal);

            return _innerShader.Vertex(state, face, vert);
        }

        public Color? Fragment(FragmentShaderState state)
        {
            var light = state.World.LightDirection;

            var color = _innerShader.Fragment(state);

            if (color == null)
                return null;

            var normal = state.Varying.PopVector3();
            normal = Vector3.Normalize(normal);

            var intensity = Vector3.Dot(normal, light);
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