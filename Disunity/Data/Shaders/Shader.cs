using System.Numerics;

namespace Disunity.Data.Shaders
{
    public abstract class Shader
    {
        public abstract void World(World world);

        public virtual void World(World world, Texture firstPahseResult)
        {
            World(world);
        }

        public abstract void Face(FaceShaderState state, int face);
        public abstract Vector4 Vertex(VertexShaderState state, int face, int vert);
        public abstract int? Fragment(FragmentShaderState state);
    }
}