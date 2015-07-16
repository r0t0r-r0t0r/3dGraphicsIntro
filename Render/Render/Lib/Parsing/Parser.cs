using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Lib.Parsing
{
    public class Parser<T>
    {
    }

    public class ParserString1 : Parser<string>
    {
        public Parser<T> Product<T>(Parser<T> p)
        {
            throw new NotImplementedException();
        }
    }

    public static class ParserOps
    {
        public static Parser<TOut> Select<TIn, TOut>(this Parser<TIn> p, Func<TIn, TOut> func)
        {
            throw new NotImplementedException();
        }

        public static Parser<TOut> SelectMany<TIn, TOut>(this Parser<TIn> p, Func<TIn, Parser<TOut>> func)
        {
            throw new NotImplementedException();
        }

        public static Parser<T1> Product1<T1, T2>(this Parser<T1> p1, Parser<T2> p2)
        {
            throw new NotImplementedException();
        }

        public static Parser<Tuple<T1, T2>> Product1<T1, T2, T3>(this Parser<Tuple<T1, T2>> p1, Parser<T3> p2)
        {
            throw new NotImplementedException();
        }

        public static Parser<Tuple<T1, T2, T3>> Product1<T1, T2, T3, T4>(this Parser<Tuple<T1, T2, T3>> p1, Parser<T4> p4) 
        {
            throw new NotImplementedException();
        }

        public static Parser<Tuple<T1, T2>> Product<T1, T2>(this Parser<T1> p1, Parser<T2> p2)
        {
            throw new NotImplementedException();
        }

        public static Parser<Tuple<T1, T2, T3>> Product<T1, T2, T3>(this Parser<Tuple<T1, T2>> p1, Parser<T3> p2)
        {
            throw new NotImplementedException();
        }

        public static Parser<Tuple<T1, T2, T3, T4>> Product<T1, T2, T3, T4>(this Parser<Tuple<T1, T2, T3>> p1, Parser<T4> p2)
        {
            throw new NotImplementedException();
        }

        public static Parser<Tuple<T1, T2, T3, T4, T5>> Product<T1, T2, T3, T4, T5>(this Parser<Tuple<T1, T2, T3, T4>> p1, Parser<T5> p2)
        {
            throw new NotImplementedException();
        }

        public static Parser<T> Or<T>(this Parser<T> p1, Parser<T> p2)
        {
            throw new NotImplementedException();
        }

        public static Parser<T> Scope<T>(this Parser<T> p, string name)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class Parsers
    {
        #region Primitives

        public static Parser<String> String(string s)
        {
            throw new NotImplementedException();
        }

        public static ParserString1 String1(string s)
        {
            throw new NotImplementedException();
        }

        public static Parser<string> Regex(string regex)
        {
            throw new NotImplementedException();
        }

        public static Parser<string> Slice<T>(Parser<T> p)
        {
            throw new NotImplementedException();
        }

        public static Parser<T> Succeed<T>(T value)
        {
            throw new NotImplementedException();
        }

        public static Parser<T> Or<T>(Parser<T> p1, Func<Parser<T>> p2)
        {
            throw new NotImplementedException();
        }

        #endregion

        public static Parser<Char> Char(Char c)
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

        public static Parser<int> Int
        {
            get { throw new NotImplementedException(); }
        }

        public static Parser<long> Long
        {
            get { throw new NotImplementedException(); }
        }

        public static Parser<double> Double
        {
            get { throw new NotImplementedException(); }
        }

        public static Parser<float> Float
        {
            get { throw new NotImplementedException(); }
        }

        public static Parser<Unit> NewLine
        {
            get { throw new NotImplementedException(); }
        }

//        public static Parser<Tuple<T1, T2>> Product<T1, T2>(Parser<T1> p1, Parser<T2> p2)
//        {
//            throw new NotImplementedException();
//        }
//
//        public static Parser<Tuple<T1, T2, T3>> Product<T1, T2, T3>(Parser<T1> p1, Parser<T2> p2, Parser<T3> p3)
//        {
//            throw new NotImplementedException();
//        }
//
//        public static Parser<Tuple<T1, T2, T3, T4>> Product<T1, T2, T3, T4>(Parser<T1> p1, Parser<T2> p2, Parser<T3> p3, Parser<T4> p4)
//        {
//            throw new NotImplementedException();
//        }
    }
}
