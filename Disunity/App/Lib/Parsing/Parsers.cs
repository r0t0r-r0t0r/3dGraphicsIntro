using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Render.Lib.Parsing.PrimitiveParsers;

namespace Render.Lib.Parsing
{
    public static class Parsers
    {
        // For LINQ
        public static Parser<TOut> SelectMany<TIn, TMid, TOut>(this Parser<TIn> p, Func<TIn, Parser<TMid>> func, Func<TIn, TMid, TOut> selector)
        {
            return p.SelectMany(x => func(x).Select(y => selector(x, y)));
        }

        public static Parser<TOut> Select<TIn, TOut>(this Parser<TIn> p, Func<TIn, TOut> func)
        {
            return p.SelectMany(x => Return(func(x)));
        }

        // Unused
        public static Parser<char> Char(char c)
        {
            throw new NotImplementedException();
        }

        // Unused
        public static Parser<ImmutableList<T>> ListOfN<T>(int n, Parser<T> p)
        {
            throw new NotImplementedException();
        }

        public static Parser<ImmutableList<T>> Many<T>(Parser<T> p)
        {
            return Select2(p, () => Many(p), (x, xs) => xs.Add(x)).Or(Return(ImmutableList.Create<T>()));
        }

        // Unused
        public static Parser<ImmutableList<T>> Many1<T>(Parser<T> p)
        {
            throw new NotImplementedException();
        }

        public static Parser<TOut> Select2<T1, T2, TOut>(Parser<T1> p1, Func<Parser<T2>> p2, Func<T1, T2, TOut> func)
        {
            return
                from x1 in p1
                from x2 in p2()
                select func(x1, x2);
        }

        public static Parser<int> Int()
        {
            throw new NotImplementedException();
        }

        public static Parser<long> Long()
        {
            throw new NotImplementedException();
        }

        public static Parser<double> Double()
        {
            throw new NotImplementedException();
        }

        public static Parser<float> Float()
        {
            throw new NotImplementedException();
        }

        public static Parser<Unit> NewLine()
        {
            return String(Environment.NewLine).Select(_ => Unit.Value);
        }
    }
}
