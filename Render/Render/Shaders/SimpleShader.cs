using System.Drawing;
using System.Numerics;

namespace Render.Shaders
{
    public class SimpleShader : Shader
    {
        private readonly Shader _innerShader;

        public SimpleShader(Shader innerShader)
        {
            _innerShader = innerShader;
        }

        public override void Face(FaceShaderState state, int face)
        {
            var geometry = state.World.WorldObject.Model.Geometry;
            var light = state.World.LightDirection;

            var v0 = geometry.GetVertex(face, 0);
            var v1 = geometry.GetVertex(face, 1);
            var v2 = geometry.GetVertex(face, 2);

            var foo1 = Vector3.Subtract(v1, v0);
            var foo2 = Vector3.Subtract(v2, v1);

            var normal = Vector3.Cross(foo1, foo2);
            normal = Vector3.Normalize(normal);

            var intensity = Vector3.Dot(normal, light);

            if (intensity < 0)
                intensity = 0;

            state.Intensity = intensity;
        }

        public override Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            return _innerShader.Vertex(state, face, vert);
        }

        public override int? Fragment(FragmentShaderState state)
        {
            var color = _innerShader.Fragment(state);
            if (color == null)
                return null;

            var intensity = state.Intensity;

            var intColor = new IntColor { Color = color.Value };

            intColor.Red = (byte)(intColor.Red * intensity);
            intColor.Green = (byte)(intColor.Green * intensity);
            intColor.Blue = (byte)(intColor.Blue * intensity);

            return intColor.Color;
        }
    }
}