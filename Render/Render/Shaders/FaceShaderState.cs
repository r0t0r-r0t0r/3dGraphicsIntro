using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Render.Shaders
{
    public sealed class FaceShaderState
    {
        private readonly World _world;

        public FaceShaderState(World world)
        {
            _world = world;
        }

        public float Intensity { get; set; }

        public World World
        {
            get { return _world; }
        }

        public void Clear()
        {
            Intensity = 0;
        }
    }
}
