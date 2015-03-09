using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace Render.Shaders
{
    public class NormalMappingShader : IShader
    {
        private readonly Geometry _geometry;
        private readonly Vector3 _light;
        private readonly IShader _innerShader;
        private readonly unsafe int* _normalMap;
        private readonly int _width;
        private readonly int _height;
        private readonly Texture _specularMap;
        private readonly Matrix4x4 _transformation;

        public unsafe NormalMappingShader(Geometry geometry, Vector3 light, IShader innerShader, byte* normalMap, int width, int height, Matrix4x4 transformation, Texture specularMap)
        {
            _geometry = geometry;
            _light = light;
            _innerShader = innerShader;
            _normalMap = (int*)normalMap;
            _width = width;
            _height = height;
            _specularMap = specularMap;

            transformation = Matrix4x4.Transpose(transformation);
            if (!Matrix4x4.Invert(transformation, out _transformation))
            {
                throw new ArgumentException();
            }
        }

        public void Face(FaceShaderState state, int face)
        {
        }

        public Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var textureVertex = _geometry.GetTextureVertex(face, vert);

            state.Varying.Push(vert, textureVertex.Y);
            state.Varying.Push(vert, textureVertex.X);

            return _innerShader.Vertex(state, face, vert);
        }

        public unsafe Color? Fragment(FragmentShaderState state)
        {
            var color = _innerShader.Fragment(state);

            if (color == null)
                return null;
            
            var tx = (int)(state.Varying.PopFloat() * (_width - 1));
            var ty = (int)(state.Varying.PopFloat() * (_height - 1));

            var pos = ((_height - ty - 1) * _width + tx);
            var tcolor = _normalMap[pos];
            var normalColor = Color.FromArgb(tcolor);
            var normal4 = new Vector4(normalColor.R, normalColor.G, normalColor.B, 0);
            normal4 = Matrix4x4Utils.Mul(_transformation, normal4);
            var normal = Vector3.Normalize(new Vector3(normal4.X, normal4.Y, normal4.Z));

            var intensity = Vector3.Dot(normal, _light);

            var power = _specularMap[tx, ty].GetRed();
            var r = Vector3.Subtract(Vector3.Multiply(2*Vector3.Dot(normal, _light), normal), _light);

            var center = new Vector3(0, 0, 0);
            var eye = new Vector3(3, 3, 10);
            var v = Vector3.Normalize(Vector3.Subtract(eye, center));

            var specular = Vector3.Dot(v, r);
            specular = (float) Math.Pow(specular, power);

            intensity += 0.6f*specular;

            var resR = (int)(color.Value.R * intensity);
            var resG = (int)(color.Value.G * intensity);
            var resB = (int)(color.Value.B * intensity);

            resR = Math.Max(Math.Min(5+resR, 255), 0);
            resG = Math.Max(Math.Min(5+resG, 255), 0);
            resB = Math.Max(Math.Min(5+resB, 255), 0);

            return Color.FromArgb(resR, resG, resB);
        }
    }
}