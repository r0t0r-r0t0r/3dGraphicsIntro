using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Lib.Parsing
{
    public static class PrimitiveParsers
    {
        public static Parser<TOut> SelectMany<TIn, TOut>(this Parser<TIn> p, Func<TIn, Parser<TOut>> func)
        {
            throw new NotImplementedException();
        }

        public static Parser<string> String(string s)
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

        public static Parser<T> Or<T>(this Parser<T> p1, Parser<T> p2)
        {
            throw new NotImplementedException();
        }

        public static Parser<T> Scope<T>(this Parser<T> p, string name)
        {
            throw new NotImplementedException();
        }
    }
}
