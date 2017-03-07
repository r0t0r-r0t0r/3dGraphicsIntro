using System;
using System.Drawing;
using System.Numerics;

namespace Render.Shaders
{
    public class NormalMappingShader : Shader
    {
        private readonly Shader _innerShader;

        public NormalMappingShader(Shader innerShader)
        {
            _innerShader = innerShader;
        }

        private Texture _normalMap;
        private Texture _specularMap;
        private Vector3 _light;
        private Vector3 _v;
        private Matrix4x4 _normalTransform;
        private Matrix4x4 _transform;
        private Geometry _geometry;
        private Texture _shadowBuffer;
        private Matrix4x4 _shadowBufferTransform;

        public override void World(World world)
        {
            _innerShader.World(world);

            _normalMap = world.WorldObject.Model.NormalMap;
            _specularMap = world.WorldObject.Model.SpecularMap;
            var light4 = world.ProjectionTransform.Mul(world.ViewTransform.Mul(new Vector4(world.LightDirection, 0)));
            _light = new Vector3(light4.X, light4.Y, light4.Z);
            _v = world.CameraDirection;
            _normalTransform = world.GetNormalTransform();
            _transform = world.GetTransform();
            _geometry = world.WorldObject.Model.Geometry;
        }

        public override void World(World world, Texture firstPahseResult)
        {
            World(world);
            _shadowBuffer = firstPahseResult;

            var lightSourceView = Matrix4x4Utils.LookAt(new Vector3(0, 0, 0), world.LightDirection, new Vector3(0, 1, 0));
            var shadowBufferTransform = world.ViewportTransform.Mul(world.ProjectionTransform.Mul(lightSourceView.Mul(world.WorldObject.ModelTransform)));
            Matrix4x4 inverseTransform;
            if (!Matrix4x4.Invert(world.GetTransform(), out inverseTransform))
            {
                throw new InvalidOperationException();
            }

            _shadowBufferTransform = shadowBufferTransform.Mul(inverseTransform);
        }

        public override void Face(FaceShaderState state, int face)
        {
        }

        public override Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var textureVertex = _geometry.GetTextureVertex(face, vert);

            var vertex = _geometry.GetVertex(face, vert);
            var foo = _transform.Mul(new Vector4(vertex, 1));

            state.Varying.Push(vert, foo.Z/foo.W);
            state.Varying.Push(vert, foo.Y/foo.W);
            state.Varying.Push(vert, foo.X/foo.W);
            state.Varying.Push(vert, textureVertex.Y);
            state.Varying.Push(vert, textureVertex.X);

            return _innerShader.Vertex(state, face, vert);
        }

        public override int? Fragment(FragmentShaderState state)
        {
            var color = _innerShader.Fragment(state);

            if (color == null)
                return null;
            
            var tx = (int)(state.Varying.PopFloat() * (_normalMap.Width - 1));
            var ty = (int)(state.Varying.PopFloat() * (_normalMap.Height - 1));
            var x = state.Varying.PopFloat();
            var y = state.Varying.PopFloat();
            var z = state.Varying.PopFloat();
            var shadowBufPoint = _shadowBufferTransform.Mul(new Vector4(x, y, z, 1));
            var shX = (int)(shadowBufPoint.X/shadowBufPoint.W);
            var shY = (int)(shadowBufPoint.Y/shadowBufPoint.W);
            var shZ = (int)(shadowBufPoint.Z/shadowBufPoint.W);
            var shadowPresent = (shZ + 3f) <
                                new IntColor {Color = _shadowBuffer[_shadowBuffer.ClipX(shX), _shadowBuffer.ClipY(shY)]}.Red;

            var tcolor = _normalMap[tx, ty];
            var normalColor = new IntColor {Color = tcolor};
            var normal4 = new Vector4(normalColor.Red, normalColor.Green, normalColor.Blue, 0);
            normal4 = normal4/127.5f;
            normal4 = new Vector4(normal4.X - 1, normal4.Y - 1, normal4.Z - 1, 0);
            normal4 = _normalTransform.Mul(normal4);
            var normal = Vector3.Normalize(new Vector3(normal4.X, normal4.Y, normal4.Z));

            var intensity = Vector3.Dot(normal, _light);

            var power = _specularMap[tx, ty].GetRed();
            var r = Vector3.Subtract(Vector3.Multiply(2*Vector3.Dot(normal, _light), normal), _light);

            var specular = Vector3.Dot(_v, r);

            specular = specular < 0 ? 0 : (float) Math.Pow(specular, power);

            intensity += 0.6f*specular;

            if (shadowPresent)
                intensity *= 0.3f;

            var intColor = new IntColor { Color = color.Value };

            var red = (intColor.Red * intensity);
            var green = (intColor.Green * intensity);
            var blue = (intColor.Blue * intensity);

            intColor.Red = (byte) Math.Max(Math.Min(red, 255), 0);
            intColor.Green = (byte) Math.Max(Math.Min(green, 255), 0);
            intColor.Blue = (byte) Math.Max(Math.Min(blue, 255), 0);

            return intColor.Color;
        }
    }
}