using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Render.Lib.Parsing
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
        private readonly Func<ParserState, Either<Exception, Tuple<T, ParserState>>> _func;

        internal Parser(Func<ParserState, Either<Exception, Tuple<T, ParserState>>> func)
        {
            _func = func;
        }

        public Either<Exception, Tuple<T, ParserState>> Run(ParserState state) => _func(state);

        public static T Parse(string str)
        {
            throw new NotImplementedException();
        }
    }
}
