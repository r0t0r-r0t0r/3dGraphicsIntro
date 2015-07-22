using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Lib
{
    public class Either<TL, TR>
    {
        internal Either()
        {
        }
    }

    public static class Either
    {
        public static Either<TL, TR> Left<TL, TR>(TL left)
        {
            throw new NotImplementedException();
        }

        public static Either<TL, TR> Right<TL, TR>(TR Right)
        {
            throw new NotImplementedException();
        }
    }
}
