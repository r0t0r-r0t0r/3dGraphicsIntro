using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Shaders
{
    public sealed class FragmentShaderState
    {
        private float _intensity;
        private List<float> _varying;

        public FragmentShaderState(int size)
        {
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

        public void Clear()
        {
            _intensity = 0;
            _varying.Clear();
        }
    }
}
