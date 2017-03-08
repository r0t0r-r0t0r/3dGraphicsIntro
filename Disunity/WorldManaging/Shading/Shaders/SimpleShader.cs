using System.Numerics;
using Disunity.Data;
using Disunity.Data.Shaders;

namespace Disunity.WorldManaging.Shading.Shaders
{
    public class SimpleShader : Shader
    {
        private readonly Shader _innerShader;
        private Geometry _geometry;
        private Vector3 _light;

        public override void World(World world)
        {
            _geometry = world.WorldObject.Model.Geometry;
            _light = world.LightDirection;
            
            _innerShader.World(world);
        }

        public SimpleShader(Shader innerShader)
        {
            _innerShader = innerShader;
        }

        public override void Face(FaceShaderState state, int face)
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