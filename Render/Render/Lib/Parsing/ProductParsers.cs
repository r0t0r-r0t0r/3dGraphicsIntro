using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Lib.Parsing
{
    public static class ProductParsers
    {
        public static ParserString1 String1(string s)
        {
            throw new NotImplementedException();
        }

        public static Parser<T1> Product1<T1, T2>(this Parser<T1> p1, Parser<T2> p2)
        {
            throw new NotImplementedException();
        }

        public static Parser<ProductResult<T1, T2>> Product1<T1, T2, T3>(this Parser<ProductResult<T1, T2>> p1, Parser<T3> p2)
        {
            throw new NotImplementedException();
        }

        public static Parser<ProductResult<T1, T2, T3>> Product1<T1, T2, T3, T4>(this Parser<ProductResult<T1, T2, T3>> p1, Parser<T4> p4) 
        {
            throw new NotImplementedException();
        }

        public static Parser<ProductResult<T1, T2>> Product<T1, T2>(this Parser<T1> p1, Parser<T2> p2)
        {
            throw new NotImplementedException();
        }

        public static Parser<ProductResult<T1, T2, T3>> Product<T1, T2, T3>(this Parser<ProductResult<T1, T2>> p1, Parser<T3> p2)
        {
            throw new NotImplementedException();
        }

        public static Parser<ProductResult<T1, T2, T3, T4>> Product<T1, T2, T3, T4>(this Parser<ProductResult<T1, T2, T3>> p1, Parser<T4> p2)
        {
            throw new NotImplementedException();
        }

        public static Parser<ProductResult<T1, T2, T3, T4, T5>> Product<T1, T2, T3, T4, T5>(this Parser<ProductResult<T1, T2, T3, T4>> p1, Parser<T5> p2)
        {
            throw new NotImplementedException();
        }
    }

    public class ParserString1 : Parser<string>
    {
        internal ParserString1(): base(String1Func)
        {
        }

        private static Either<Exception, Tuple<string, ParserState>> String1Func(ParserState state)
        {
            throw new NotImplementedException();
        }

        public Parser<T> Product<T>(Parser<T> p)
        {
            throw new NotImplementedException();
        }
    }

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
