using System;

namespace Render
{
    public class Face
    {
        private readonly int _a;
        private readonly int _b;
        private readonly int _c;
        private readonly int _ta;
        private readonly int _tb;
        private readonly int _tc;
        private readonly int _na;
        private readonly int _nb;
        private readonly int _nc;

        public Face(int a, int b, int c, int ta, int tb, int tc, int na, int nb, int nc)
        {
            _a = a;
            _b = b;
            _c = c;
            _ta = ta;
            _tb = tb;
            _tc = tc;
            _na = na;
            _nb = nb;
            _nc = nc;
        }

        public int A { get { return _a; } }
        public int B { get { return _b; } }
        public int C { get { return _c; } }

        public int this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        return _a;
                    case 1:
                        return _b;
                    case 2:
                        return _c;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }

        public int GetVtIndex(int i)
        {
            switch (i)
            {
                case 0:
                    return _ta;
                case 1:
                    return _tb;
                case 2:
                    return _tc;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public int GetNormalIndex(int i)
        {
            switch (i)
            {
                case 0:
                    return _na;
                case 1:
                    return _nb;
                case 2:
                    return _nc;
                default:
                    throw new IndexOutOfRangeException();
            } 
        }
    }
}