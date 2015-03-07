using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace Render.Shaders
{
    public class NormalMappingShader : IShader
    {
        private readonly Model _model;
        private readonly Vector3 _light;
        private readonly IShader _innerShader;
        private readonly unsafe int* _normalMap;
        private readonly int _width;
        private readonly int _height;
        private readonly Matrix4x4 _transformation;

        public unsafe NormalMappingShader(Model model, Vector3 light, IShader innerShader, byte* normalMap, int width, int height, Matrix4x4 transformation)
        {
            _model = model;
            _light = light;
            _innerShader = innerShader;
            _normalMap = (int*)normalMap;
            _width = width;
            _height = height;

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
            var textureVertex = _model.GetTextureVertex(face, vert);

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
            if (intensity < 0)
                return Color.Black;

            if (intensity >= 1)
                intensity = 1;

            var resR = (byte)(color.Value.R * intensity);
            var resG = (byte)(color.Value.G * intensity);
            var resB = (byte)(color.Value.B * intensity);

            return Color.FromArgb(resR, resG, resB);
        }
    }
}