using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Shaders
{
    public sealed class ShaderState
    {
        private readonly FaceShaderState _face;
        private readonly VertexShaderState _vertex;
        private readonly FragmentShaderState _fragment;

        public ShaderState(int size)
        {
            _face = new FaceShaderState();
            _vertex = new VertexShaderState(size);
            _fragment = new FragmentShaderState(size);
        }

        public FaceShaderState Face
        {
            get { return _face; }
        }

        public VertexShaderState Vertex
        {
            get { return _vertex; }
        }

        public FragmentShaderState Fragment
        {
            get { return _fragment; }
        }
    }
}
