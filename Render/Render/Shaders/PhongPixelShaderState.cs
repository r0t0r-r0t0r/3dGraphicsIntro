using System.Numerics;

namespace Render.Shaders
{
    public class PhongPixelShaderState
    {
        public Vector3[] VertexNormals { get; set; }
        public object InnerState { get; set; }
    }
}