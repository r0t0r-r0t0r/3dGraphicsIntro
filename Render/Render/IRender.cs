using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Render.Shaders;

namespace Render
{
    public interface IRender
    {
        void Init(int width, int height);

        unsafe void Draw(int faceIndex, byte* bitmap, IShader shader, ShaderState shaderState, int startY, int endY);
    }
}
