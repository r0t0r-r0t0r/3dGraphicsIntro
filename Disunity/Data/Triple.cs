using System;

namespace Disunity.Data
{
    public struct Triple<T> where T : struct
    {
        private readonly T _a;
        private readonly T _b;
        private readonly T _c;

        public Triple(T a, T b, T c)
        {
            _a = a;
            _b = b;
            _c = c;
        }

        public T A
        {
            get { return _a; }
        }

        public T B
        {
            get { return _b; }
        }

        public T C
        {
            get { return _c; }
        }

        public T GetValue(int index)
        {
            switch (index)
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
}