using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Render
{
    public interface IRender
    {
        void Init(Model model, Bitmap texture, int width, int height, string rootDir);

        void Draw(Face face, Vector3 a, Vector3 b, Vector3 c, Bitmap bitmap, byte lightLevel);

        void Finish();
    }
}
