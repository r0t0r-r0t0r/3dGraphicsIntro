namespace Disunity.Parsing
{
    public static class AndParsers
    {
        public static String1 String1(string s) =>
            new String1(PrimitiveParsers.String(s));

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
}