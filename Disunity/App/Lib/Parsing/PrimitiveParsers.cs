using System;
using System.Text.RegularExpressions;

namespace Disunity.App.Lib.Parsing
{
    public static class PrimitiveParsers
    {
        public static Parser<TOut> SelectMany<TIn, TOut>(this Parser<TIn> p, Func<TIn, Parser<TOut>> func)
        {
            return new Parser<TOut>(state =>
            {
                return p.Run(state).SelectMany(result1 =>
                {
                    return result1.Match(
                        leftCase: l => TailRec.Return(Either.Left<Exception, Tuple<TOut, ParserState>>(l)),
                        rightCase: r => func(r.Item1).Run(r.Item2)
                    );
                });
            });
        }

        public static Parser<string> String(string str)
        {
            return new Parser<string>(state =>
            {
                return TailRec.Suspend(() =>
                {
                    var found = state.String.Substring(state.Position).StartsWith(str);
                    if (found)
                    {
                        var newState = state.Copy(position: state.Position + str.Length);

                        return Right(str, newState);
                    }
                    else
                    {
                        return LeftStr("String not found");
                    }
                });
            });
        }

        public static Parser<string> Regex(string pattern)
        {
            return new Parser<string>(state =>
            {
                return TailRec.Suspend(() =>
                {
                    var regex = new Regex(pattern);
                    var match = regex.Match(state.String, state.Position);
                    if (match.Success)
                    {
                        return Right(match.Value, state.Copy(position: state.Position + match.Value.Length));
                    }
                    else
                    {
                        return LeftStr("Pattern does not match");
                    }
                });
            });
        }

        public static Parser<string> Slice<T>(Parser<T> p)
        {
            return new Parser<string>(state =>
            {
                return p.Run(state).SelectMany(result =>
                {
                    return TailRec.Suspend(() =>
                    {
                        return result.Select(r =>
                        {
                            var newState = r.Item2;

                            var length = newState.Position - state.Position;
                            var consumedString = state.String.Substring(state.Position, length);

                            return Tuple.Create(consumedString, newState);
                        });
                    });
                });
            });
        }

        public static Parser<T> Return<T>(T value)
        {
            return new Parser<T>(state => TailRec.Return(Right(value, state)));
        }

        public static Parser<T> Fail<T>(Exception error)
        {
            return new Parser<T>(state => TailRec.Return(Either.Left<Exception, Tuple<T, ParserState>>(error)));
        }

        public static Parser<T> Or<T>(this Parser<T> p1, Parser<T> p2)
        {
            return new Parser<T>(state =>
            {
                return p1.Run(state).SelectMany(result =>
                {
                    return result.Select(x => TailRec.Return(result)).GetOrElse(() => p2.Run(state));
                });
            });
        }

        public static Parser<T> Scope<T>(this Parser<T> p, string name)
        {
            return new Parser<T>(state => p.Run(state.Copy(scope: name)));
        }

        private static Either<Exception, Tuple<string, ParserState>> LeftStr(string error)
        {
            return Either.Left<Exception, Tuple<string, ParserState>>(new Exception(error));
        }

        private static Either<Exception, Tuple<T, ParserState>> Right<T>(T value, ParserState state)
        {
            return Either.Right<Exception, Tuple<T, ParserState>>(Tuple.Create(value, state));
        }
    }
}
