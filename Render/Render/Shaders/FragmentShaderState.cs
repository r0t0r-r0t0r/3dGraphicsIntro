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

        public float Intensity
        {
            get { return _intensity; }
        }

        public List<float> Varying
        {
            get { return _varying; }
        }
    }
}
