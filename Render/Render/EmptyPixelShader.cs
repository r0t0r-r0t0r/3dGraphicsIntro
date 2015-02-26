using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    public class EmptyPixelShader: IPixelShader
    {
        public object OnFace(Face face)
        {
            return null;
        }

        public Color? OnPixel(object state, float a, float b, float c)
        {
            return null;
        }
    }
}
