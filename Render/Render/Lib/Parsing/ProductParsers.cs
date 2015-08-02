using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Render.Lib.Parsing.PrimitiveParsers;

namespace Render.Lib.Parsing
{
    public static class AndParsers
    {
        public static String1 String1(string s) =>
            new String1(String(s));

        public static Parser<T1> And1<T1, T2>(this Parser<T1> p1, Parser<T2> p2) =>
            from x1 in p1
            from x2 in p2
            select x1;

        public static Parser<ProductResult<T1, T2>> And1<T1, T2, T3>(this Parser<ProductResult<T1, T2>> p1, Parser<T3> p2) =>
            from x1 in p1
            from x2 in p2
            select x1;

        public static Parser<ProductResult<T1, T2, T3>> And1<T1, T2, T3, T4>(this Parser<ProductResult<T1, T2, T3>> p1, Parser<T4> p2) =>
            from x1 in p1
            from x2 in p2
            select x1;

        public static Parser<ProductResult<T1, T2>> And<T1, T2>(this Parser<T1> p1, Parser<T2> p2) =>
            from x1 in p1
            from x2 in p2
            select new ProductResult<T1, T2>(x1, x2);

        public static Parser<ProductResult<T1, T2, T3>> And<T1, T2, T3>(this Parser<ProductResult<T1, T2>> p1, Parser<T3> p2) =>
            from x in p1
            from x3 in p2
            select new ProductResult<T1, T2, T3>(x._1, x._2, x3);

        public static Parser<ProductResult<T1, T2, T3, T4>> And<T1, T2, T3, T4>(this Parser<ProductResult<T1, T2, T3>> p1, Parser<T4> p2) =>
            from x in p1
            from x4 in p2
            select new ProductResult<T1, T2, T3, T4>(x._1, x._2, x._3, x4);

        public static Parser<ProductResult<T1, T2, T3, T4, T5>> And<T1, T2, T3, T4, T5>(this Parser<ProductResult<T1, T2, T3, T4>> p1, Parser<T5> p2) =>
            from x in p1
            from x5 in p2
            select new ProductResult<T1, T2, T3, T4, T5>(x._1, x._2, x._3, x._4, x5);
    }

    public class String1
    {
        private readonly Parser<string> _parser;

        internal String1(Parser<string> parser)
        {
            _parser = parser;
        }

        public Parser<T> And<T>(Parser<T> p) => _parser.SelectMany(_ => p);
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
