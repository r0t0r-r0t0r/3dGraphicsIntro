namespace Disunity.Parsing
{
    public class String1
    {
        private readonly Parser<string> _parser;

        internal String1(Parser<string> parser)
        {
            _parser = parser;
        }

        public Parser<T> And<T>(Parser<T> p) => _parser.SelectMany(_ => p);
    }
}