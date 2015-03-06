using System.Collections.Generic;

namespace Render.Shaders
{
    public sealed class VertexShaderState
    {
        private readonly List<float>[] _varying;

        public VertexShaderState(int size)
        {
            _varying = new List<float>[]
            {
                new List<float>(size),
                new List<float>(size),
                new List<float>(size)
            };
        }

        public List<float>[] Varying
        {
            get { return _varying; }
        }

        public void Clear()
        {
            _varying[0].Clear();
            _varying[1].Clear();
            _varying[2].Clear();
        }
    }
}