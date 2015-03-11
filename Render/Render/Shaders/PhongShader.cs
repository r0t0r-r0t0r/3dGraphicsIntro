using System;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Render.Shaders
{
    public class PhongShader : Shader
    {
        private static readonly int BlackColor = Color.Black.ToArgb();
        private readonly Shader _innerShader;

        public PhongShader(Shader innerShader)
        {
            _innerShader = innerShader;
        }

        public override void Face(FaceShaderState state, int face)
        {
        }

        public override Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var geometry = state.World.WorldObject.Model.Geometry;
            var normal = geometry.GetVertexNormal(face, vert);
            state.Varying[vert].Push(normal);

            return _innerShader.Vertex(state, face, vert);
        }

        public override int? Fragment(FragmentShaderState state)
        {
            var light = state.World.LightDirection;

            var color = _innerShader.Fragment(state);

            if (color == null)
                return null;

            var normal = state.Varying.PopVector3();
            normal = Vector3.Normalize(normal);

            var intensity = Vector3.Dot(normal, light);
            if (intensity < 0)
                return BlackColor;

            if (intensity >= 1)
                intensity = 1;

            //const float intensityStepsCount = 5;
            //intensity = (float)Math.Floor(intensity * intensityStepsCount) / intensityStepsCount;

            var intColor = new IntColor {Color = color.Value};

            intColor.Red = (byte)(intColor.Red * intensity);
            intColor.Green = (byte)(intColor.Green * intensity);
            intColor.Blue = (byte)(intColor.Blue * intensity);

            return intColor.Color;
        }
    }
}