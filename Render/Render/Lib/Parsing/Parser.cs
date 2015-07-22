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
        public ParserState(int position, string str)
        {
            Position = position;
            String = str;
        }

        public int Position { get; }
        public string String { get; }
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
