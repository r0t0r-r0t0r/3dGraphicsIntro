using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    public interface IRender
    {
        unsafe void Init(Model model, byte* texture, int textureWidts, int textureHeight, int width, int height, string rootDir, Vector3 light);

        unsafe void Draw(Face face, Vector3 a, Vector3 b, Vector3 c, byte* bitmap);

        void Finish();
    }
}
