using System.Numerics;
using Disunity.Data;
using Disunity.Data.Shaders;

namespace Disunity.App.Shaders
{
    class ZBufferShader: Shader
    {
        private Geometry _geometry;
        private Matrix4x4 _transformation;

        public override void World(World world)
        {
            _geometry = world.WorldObject.Model.Geometry;
            var lightSourceView = Matrix4x4Utils.LookAt(new Vector3(0, 0, 0), world.LightDirection, new Vector3(0, 1, 0));
            _transformation = world.ViewportTransform.Mul(world.ProjectionTransform.Mul(lightSourceView.Mul(world.WorldObject.ModelTransform)));
        }

        public override void Face(FaceShaderState state, int face)
        {
        }

        public override Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var vertex = _geometry.GetVertex(face, vert);
            var transformed = _transformation.Mul(new Vector4(vertex, 1));
            state.Varying.Push(vert, transformed.Z/transformed.W);

            return transformed;
        }

        public override int? Fragment(FragmentShaderState state)
        {
            var z = (byte)state.Varying.PopFloat();
            var color = new IntColor {Red = z, Green = z, Blue = z};
            return color.Color;
        }
    }
}
