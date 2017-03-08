using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Disunity.WorldManaging.Loading
{
    public sealed class TextParser
    {
        private static readonly TextParser EmptyParser = new TextParser(new LineParser[]{});

        private readonly IEnumerable<LineParser> _parsers;

        private TextParser(IEnumerable<LineParser> parsers)
        {
            _parsers = parsers;
        }

        public static TextParser Empty
        {
            get { return EmptyParser; }
        }

        public TextParser AddLineParser<T>(Regex regex, Func<Match, T> parser, Action<T> storer)
        {
            var lineParsers = _parsers.Concat(new[] {LineParser.Create(regex, parser, storer)});
            return new TextParser(lineParsers);
        }

        public void Parse(string[] lines)
        {
            foreach (var line in lines)
            {
                foreach (var lineDescription in _parsers)
                {
                    if (lineDescription.ParseLine(line))
                        break;
                }
            }
        }

        private abstract class LineParser
        {
            private readonly Regex _regex;

            private LineParser(Regex regex)
            {
                _regex = regex;
            }

            protected abstract void ParseMatch(Match match);

            public bool ParseLine(string line)
            {
                var match = _regex.Match(line);

                if (!match.Success)
                    return false;

                ParseMatch(match);
                return true;
            }

            public static LineParser Create<T>(Regex regex, Func<Match, T> parser, Action<T> storer)
            {
                return new TypedLineParser<T>(regex, parser, storer);
            }

            private class TypedLineParser<T> : LineParser
            {
                private readonly Func<Match, T> _parser;
                private readonly Action<T> _storer;

                public TypedLineParser(Regex regex, Func<Match, T> parser, Action<T> storer)
                    : base(regex)
                {
                    _parser = parser;
                    _storer = storer;
                }

                protected override void ParseMatch(Match match)
                {
                    var value = _parser(match);
                    _storer(value);
                }
            }
        }
    }
}