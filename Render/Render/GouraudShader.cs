using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Render
{
    public class GouraudShader : IShader
    {
        private readonly Model _model;
        private readonly Vector3 _light;
        private readonly IShader _innerShader;

        public GouraudShader(Model model, Vector3 light, IShader innerShader)
        {
            _model = model;
            _light = light;
            _innerShader = innerShader;
        }

        public object OnFace(Face face)
        {
            var intensities = Enumerable.Range(0, 3).Select(face.GetNormalIndex).Select(x => _model.VertexNormals[x]).Select(x => Vector3.Dot(x, _light)).ToArray();

            return new GouraudPixelShaderState
            {
                Intensities = intensities,
                InnerState = _innerShader.OnFace(face)
            };
        }

        public void Vertex(VertexShaderState state, int face, int vert)
        {
            var normal = _model.GetVertexNormal(face, vert);
            var intensity = Vector3.Dot(normal, _light);

            state.Varying.Push(vert, intensity);
        }

        public Color? OnPixel(object state, float a, float b, float c)
        {
            var s = (GouraudPixelShaderState)state;
            var color = _innerShader.OnPixel(s.InnerState, a, b, c);

            if (color == null)
                return null;

            var ins = s.Intensities;

            var intensity = ins[0]*a + ins[1]*b + ins[2]*c;
            if (intensity < 0)
                return Color.Black;

            if (intensity > 1)
                intensity = 1;

            var resR = (byte)(color.Value.R * intensity);
            var resG = (byte)(color.Value.G * intensity);
            var resB = (byte)(color.Value.B * intensity);

            return Color.FromArgb(resR, resG, resB);
        }
    }

    public static class ShaderStateUtils
    {
        public static void Push(this List<float>[] list, int vert, float value)
        {
            list[vert].Add(value);
        }

        public static void Push(this List<float> list, float value)
        {
            list.Add(value);
        }

        public static void Push(this List<float> list, Vector3 vector)
        {
            list.Add(vector.X);
            list.Add(vector.Y);
            list.Add(vector.Z);
        }

        public static Vector3 PopVector3(this List<float> list)
        {
            var xIndex = list.Count - 3;
            var x = list[xIndex];
            var y = list[xIndex + 1];
            var z = list[xIndex + 2];

            list.RemoveRange(xIndex, 3);

            return new Vector3(x, y, z);
        }
    }
}