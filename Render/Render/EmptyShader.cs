using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    public class EmptyShader: IShader
    {
        public object OnFace(Face face)
        {
            return null;
        }

        public void Vertex(VertexShaderState state, int face, int vert)
        {
        }

        public Color? OnPixel(object state, float a, float b, float c)
        {
            return null;
        }
    }
}
