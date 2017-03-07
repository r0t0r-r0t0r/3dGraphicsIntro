using System.Collections.Generic;

namespace Disunity.App.Shaders
{
    public sealed class VertexShaderState
    {
        private readonly World _world;
        private readonly List<float>[] _varying;

        public VertexShaderState(int size, World world)
        {
            _world = world;
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

        public World World
        {
            get { return _world; }
        }

        public void Clear()
        {
            _varying[0].Clear();
            _varying[1].Clear();
            _varying[2].Clear();
        }
    }
}