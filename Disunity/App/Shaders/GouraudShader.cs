using System.Drawing;
using System.Numerics;
using Disunity.Models;
using Disunity.Models.Shaders;

namespace Disunity.App.Shaders
{
    public class GouraudShader : Shader
    {
        private static readonly int BlackColor = Color.Black.ToArgb();
        private readonly Shader _innerShader;

        public GouraudShader(Shader innerShader)
        {
            _innerShader = innerShader;
        }

        public override void World(World world)
        {
            _innerShader.World(world);
        }

        public override void Face(FaceShaderState state, int face)
        {
        }

        public override Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var geometry = state.World.WorldObject.Model.Geometry;
            var light = state.World.LightDirection;

            var normal = geometry.GetVertexNormal(face, vert);
            var intensity = Vector3.Dot(normal, light);

            state.Varying.Push(vert, intensity);

            return _innerShader.Vertex(state, face, vert);
        }

        public override int? Fragment(FragmentShaderState state)
        {
            var color = _innerShader.Fragment(state);

            if (color == null)
                return null;

            var intensity = state.Varying.PopFloat();

            if (intensity < 0)
                return BlackColor;

            if (intensity > 1)
                intensity = 1;

            var intColor = new IntColor { Color = color.Value };

            intColor.Red = (byte)(intColor.Red * intensity);
            intColor.Green = (byte)(intColor.Green * intensity);
            intColor.Blue = (byte)(intColor.Blue * intensity);

            return intColor.Color; ;
        }
    }
}