using System.Collections.Generic;

namespace Disunity.Models.Shaders
{
    public sealed class FragmentShaderState
    {
        private readonly World _world;
        private float _intensity;
        private List<float> _varying;

        public FragmentShaderState(int size, World world)
        {
            _world = world;
            _varying = new List<float>(size);
        }

        public float Intensity
        {
            get { return _intensity; }
            set { _intensity = value; }
        }

        public List<float> Varying
        {
            get { return _varying; }
        }

        public World World
        {
            get { return _world; }
        }

        public void Clear()
        {
            _intensity = 0;
            _varying.Clear();
        }
    }
}
