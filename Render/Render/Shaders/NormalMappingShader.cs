using System;
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
        private Matrix4x4 _transformation;
        private Geometry _geometry;

        public override void World(World world)
        {
            _innerShader.World(world);

            _normalMap = world.WorldObject.Model.NormalMap;
            _specularMap = world.WorldObject.Model.SpecularMap;
            var light4 = world.ProjectionTransform.Mul(world.ViewTransform.Mul(new Vector4(world.LightDirection, 0)));
            _light = new Vector3(light4.X, light4.Y, light4.Z);
            _v = world.CameraDirection;
            _transformation = world.NormalTransform;
            _geometry = world.WorldObject.Model.Geometry;
        }

        public override void Face(FaceShaderState state, int face)
        {
        }

        public override Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var textureVertex = _geometry.GetTextureVertex(face, vert);

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

            var tcolor = _normalMap[tx, ty];
            var normalColor = new IntColor {Color = tcolor};
            var normal4 = new Vector4(normalColor.Red, normalColor.Green, normalColor.Blue, 0);
            normal4 = normal4/127.5f;
            normal4 = new Vector4(normal4.X - 1, normal4.Y - 1, normal4.Z - 1, 0);
            normal4 = _transformation.Mul(normal4);
            var normal = Vector3.Normalize(new Vector3(normal4.X, normal4.Y, normal4.Z));

            var intensity = Vector3.Dot(normal, _light);

            var power = _specularMap[tx, ty].GetRed();
            var r = Vector3.Subtract(Vector3.Multiply(2*Vector3.Dot(normal, _light), normal), _light);

            var specular = Vector3.Dot(_v, r);
            specular = (float) Math.Pow(specular, power);

//            intensity += 0.6f*specular;
            intensity = specular;

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