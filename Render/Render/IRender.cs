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

        unsafe void Draw(int faceIndex, Face face, Vector3 a, Vector3 b, Vector3 c, byte* bitmap, IShader shader, int startY, int endY, ShaderState shaderState);
    }
}
