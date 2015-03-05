using System.Collections.Generic;
using System.Drawing;

namespace Render
{
    public interface IShader
    {
        object OnFace(Face face);
        void Vertex(VertexShaderState state, int face, int vert);
        Color? OnPixel(object state, float a, float b, float c);
    }

    public sealed class VertexShaderState
    {
        private readonly List<float>[] _varying = new []
            {
                new List<float>(30),
                new List<float>(30),
                new List<float>(30)
            };
        private readonly List<float> _uniform = new List<float>(30);

        public List<float>[] Varying
        {
            get { return _varying; }
        }

        public List<float> Uniform
        {
            get { return _uniform; }
        }
    }
}