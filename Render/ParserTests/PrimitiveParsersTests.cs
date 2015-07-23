using NUnit.Framework;
using Render.Lib.Parsing;
using System;
using static Render.Lib.Parsing.PrimitiveParsers;

namespace ParserTests
{
    [TestFixture]
    public class PrimitiveParsersTests
    {
        [Test]
        public void StringTest()
        {
            var parser = String("foobar");

            Assert.That(parser.Feed("foobar").Success);
        }

        [Test]
        public void StringTest1()
        {
            var parser = String("foobar");

            Assert.That(parser.Feed("fizzbuzz").Fail);
        }

        [Test]
        public void SelectMany()
        {
            var parser1 = String("foo");
            var parser2 = String("bar");

            var composite = parser1.SelectMany(_ => parser2);

            Assert.That(composite.Feed("foobarbuzz").Success);
        }
    }

    internal class ParsingResult<T>
    {
        public ParsingResult(bool success)
        {
            Success = success;
        }

        public bool Success { get; }

        public bool Fail => !Success;
    }

    internal static class TestParsers
    {
        public static ParsingResult<T> Feed<T>(this Parser<T> p, string input)
        {
            var result = p.Run(new ParserState(0, input));
            return new ParsingResult<T>(result.IsRight);
        }
    }
}
