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
        public static Parser<TOut> Select<TIn, TOut>(this Parser<TIn> p, Func<TIn, TOut> func)
        {
            return p.SelectMany(x => Succeed(func(x)));
        }

        public static Parser<char> Char(char c)
        {
            throw new NotImplementedException();
        }

        public static Parser<ImmutableList<T>> ListOfN<T>(int n, Parser<T> p)
        {
            throw new NotImplementedException();
        }

        public static Parser<ImmutableList<T>> Many<T>(Parser<T> p)
        {
            throw new NotImplementedException();
        }

        public static Parser<ImmutableList<T>> Many1<T>(Parser<T> p)
        {
            throw new NotImplementedException();
        }

        public static Parser<TOut> Select2<T1, T2, TOut>(Parser<T1> p1, Func<Parser<T2>> p2, Func<T1, T2, TOut> func)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
