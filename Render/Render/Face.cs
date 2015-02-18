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

        public Face(int a, int b, int c, int ta, int tb, int tc)
        {
            _a = a;
            _b = b;
            _c = c;
            _ta = ta;
            _tb = tb;
            _tc = tc;
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
    }
}