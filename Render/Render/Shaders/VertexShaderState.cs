using System.Collections.Generic;

namespace Render.Shaders
{
    public sealed class VertexShaderState
    {
        private readonly List<float>[] _varying = {
            new List<float>(30),
            new List<float>(30),
            new List<float>(30)
        };

        public List<float>[] Varying
        {
            get { return _varying; }
        }

    }
}