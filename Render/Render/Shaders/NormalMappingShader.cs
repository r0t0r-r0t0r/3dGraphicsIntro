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

        public override void Face(FaceShaderState state, int face)
        {
        }

        public override Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var geometry = state.World.WorldObject.Model.Geometry;

            var textureVertex = geometry.GetTextureVertex(face, vert);

            state.Varying.Push(vert, textureVertex.Y);
            state.Varying.Push(vert, textureVertex.X);

            return _innerShader.Vertex(state, face, vert);
        }

        public override int? Fragment(FragmentShaderState state)
        {
            var normalMap = state.World.WorldObject.Model.NormalMap;
            var specularMap = state.World.WorldObject.Model.SpecularMap;
            var light = state.World.LightDirection;
            var v = state.World.CameraDirection;

            var transformation = state.World.NormalTransform;

            var color = _innerShader.Fragment(state);

            if (color == null)
                return null;
            
            var tx = (int)(state.Varying.PopFloat() * (normalMap.Width - 1));
            var ty = (int)(state.Varying.PopFloat() * (normalMap.Height - 1));

            var tcolor = normalMap[tx, ty];
            var normalColor = new IntColor {Color = tcolor};
            var normal4 = new Vector4(normalColor.Red, normalColor.Green, normalColor.Blue, 0);
            normal4 = transformation.Mul(normal4);
            var normal = Vector3.Normalize(new Vector3(normal4.X, normal4.Y, normal4.Z));

            var intensity = Vector3.Dot(normal, light);

            var power = specularMap[tx, ty].GetRed();
            var r = Vector3.Subtract(Vector3.Multiply(2*Vector3.Dot(normal, light), normal), light);

            var specular = Vector3.Dot(v, r);
            specular = (float) Math.Pow(specular, power);

            intensity += 0.6f*specular;

            var intColor = new IntColor { Color = color.Value };

            var red = (intColor.Red * intensity);
            var green = (intColor.Green * intensity);
            var blue = (intColor.Blue * intensity);

            intColor.Red = (byte) Math.Max(Math.Min(5+red, 255), 0);
            intColor.Green = (byte) Math.Max(Math.Min(5+green, 255), 0);
            intColor.Blue = (byte) Math.Max(Math.Min(5+blue, 255), 0);

            return intColor.Color;
        }
    }
}