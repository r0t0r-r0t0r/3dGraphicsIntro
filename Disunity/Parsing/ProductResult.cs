namespace Disunity.Parsing
{
    public class ProductResult<T1, T2>
    {
        internal ProductResult(T1 value1, T2 value2)
        {
            _1 = value1;
            _2 = value2;
        }

        public T1 _1 { get; }
        public T2 _2 { get; }
    }

    public class ProductResult<T1, T2, T3>
    {
        internal ProductResult(T1 value1, T2 value2, T3 value3)
        {
            _1 = value1;
            _2 = value2;
            _3 = value3;
        }

        public T1 _1 { get; }
        public T2 _2 { get; }
        public T3 _3 { get; }
    }

    public class ProductResult<T1, T2, T3, T4>
    {
        internal ProductResult(T1 value1, T2 value2, T3 value3, T4 value4)
        {
            _1 = value1;
            _2 = value2;
            _3 = value3;
            _4 = value4;
        }

        public T1 _1 { get; }
        public T2 _2 { get; }
        public T3 _3 { get; }
        public T4 _4 { get; }
    }

    public class ProductResult<T1, T2, T3, T4, T5>
    {
        internal ProductResult(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5)
        {
            _1 = value1;
            _2 = value2;
            _3 = value3;
            _4 = value4;
            _5 = value5;
        }

        public T1 _1 { get; }
        public T2 _2 { get; }
        public T3 _3 { get; }
        public T4 _4 { get; }
        public T5 _5 { get; }
    }
}
