using System.Drawing;
using System.Numerics;

namespace Render.Shaders
{
    public class EmptyShader: IShader
    {
        public void Face(FaceShaderState state, int face)
        {
        }

        public Vector4 Vertex(VertexShaderState state, int face, int vert)
        {
            var geometry = state.World.WorldObject.Model.Geometry;
            var transformation = state.World.Transformation;
            return transformation.Mul(new Vector4(geometry.GetVertex(face, vert), 1));
        }

        public Color? Fragment(FragmentShaderState state)
        {
            return null;
        }
    }
}
