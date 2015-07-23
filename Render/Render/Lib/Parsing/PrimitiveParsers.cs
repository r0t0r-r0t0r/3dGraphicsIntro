using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Render.Lib.Parsing
{
    public static class PrimitiveParsers
    {
        public static Parser<TOut> SelectMany<TIn, TOut>(this Parser<TIn> p, Func<TIn, Parser<TOut>> func)
        {
            return new Parser<TOut>(state =>
            {
                var result1 = p.Run(state);

                var result2 = result1.SelectMany(r => func(r.Item1).Run(r.Item2));

                return result2;
            });
        }

        public static Parser<string> String(string str)
        {
            return new Parser<string>(state =>
            {
                var index = state.String.IndexOf(str, state.Position);
                if (index >= 0)
                {
                    var newState = state.Copy(position: state.Position + str.Length);

                    return Either.Right<Exception, Tuple<string, ParserState>>(Tuple.Create(str, newState));
                }
                else
                {
                    return Either.Left<Exception, Tuple<string, ParserState>>(new Exception("String not found"));
                }
            });
        }

        public static Parser<string> Regex(string pattern)
        {
            return new Parser<string>(state =>
            {
                var regex = new Regex(pattern);
                var match = regex.Match(state.String, state.Position);
                if (match.Success)
                {
                    return Either.Right<Exception, Tuple<string, ParserState>>(Tuple.Create(match.Value, state.Copy(position: state.Position + match.Value.Length)));
                }
                else
                {
                    return Either.Left<Exception, Tuple<string, ParserState>>(new Exception("Pattern does not match"));
                }
            });
        }

        public static Parser<string> Slice<T>(Parser<T> p)
        {
            return new Parser<string>(state =>
            {
                var result = p.Run(state);
                return result.Select(r =>
                {
                    var newState = r.Item2;

                    var length = newState.Position - state.Position;
                    var consumedString = state.String.Substring(state.Position, length);

                    return Tuple.Create(consumedString, newState);
                });
            });
        }

        public static Parser<T> Succeed<T>(T value)
        {
            return new Parser<T>(state => Either.Right<Exception, Tuple<T, ParserState>>(Tuple.Create(value, state)));
        }

        public static Parser<T> Or<T>(this Parser<T> p1, Parser<T> p2)
        {
            return new Parser<T>(state => p1.Run(state).OrElse(() => p2.Run(state)));
        }

        public static Parser<T> Scope<T>(this Parser<T> p, string name)
        {
            return new Parser<T>(state => p.Run(state.Copy(scope: name)));
        }
    }
}
