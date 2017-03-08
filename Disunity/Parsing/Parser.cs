using System;
using Disunity.Data.Common;

namespace Disunity.Parsing
{
    public class ParserState
    {
        public ParserState(int position, string str, string scope)
        {
            Position = position;
            String = str;
            Scope = scope;
        }

        public int Position { get; }
        public string String { get; }
        public string Scope { get; }

        public ParserState Copy(int? position = null, string str = null, string scope = null)
        {
            var newPos = position ?? Position;
            var newStr = str ?? String;
            var newScope = scope ?? Scope;

            return new ParserState(newPos, newStr, newScope);
        }
    }

    public class Parser<T>
    {
        private readonly Func<ParserState, TailRec<Either<Exception, Tuple<T, ParserState>>>> _func;

        internal Parser(Func<ParserState, TailRec<Either<Exception, Tuple<T, ParserState>>>> func)
        {
            _func = func;
        }

        public TailRec<Either<Exception, Tuple<T, ParserState>>> Run(ParserState state) => _func(state);

        public T Parse(string str)
        {
            return Try.FromEither(Run(new ParserState(0, str, "root")).Run().Select(x => x.Item1)).GetValue();
        }
    }
}
