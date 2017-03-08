using Disunity.Data;
using Disunity.Data.Shaders;

namespace Disunity.Rendering
{
    sealed class ShaderState
    {
        private readonly World _world;
        private readonly FaceShaderState _face;
        private readonly VertexShaderState _vertex;
        private readonly FragmentShaderState _fragment;

        public ShaderState(int size, World world)
        {
            _world = world;
            _face = new FaceShaderState(world);
            _vertex = new VertexShaderState(size, world);
            _fragment = new FragmentShaderState(size, world);
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

        public World World
        {
            get { return _world; }
        }
    }
}
