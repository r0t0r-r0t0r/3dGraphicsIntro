using NUnit.Framework;
using System;
using Disunity.App;
using Disunity.App.Lib.Parsing;
using static Disunity.App.Lib.Parsing.PrimitiveParsers;

namespace ParserTests
{
    [TestFixture]
    public class PrimitiveParsersTests
    {
        [Test]
        public void StringTest_1()
        {
            const string str = "foobar";

            var parser = String(str);

            ParserAssert.That(parser.OnInput(str), p => p.ConsumesAndProduces(str));
        }

        [Test]
        public void StringTest_2()
        {
            var parser = String("foobar");

            ParserAssert.That(parser.OnInput("fizzbuzz"), p => p.Fails());
        }

        [Test]
        public void StringTest_3()
        {
            var parser = String("foobar");

            ParserAssert.That(parser.OnInput("foobargggtralala"), p => p.ConsumesAndProduces("foobar"));
        }

        [Test]
        public void StringTest_4()
        {
            var parser = String("foobar");

            ParserAssert.That(parser.OnInput("_foobargggtralala"), p => p.Fails());
        }

        [Test]
        public void SelectMany_1()
        {
            var parser1 = Return(1);
            var parser2 = Return(2);

            var composite = parser1.SelectMany(_ => parser2);

            ParserAssert.That(composite.OnAnyInput(), p => p.ConsumesNothing().And(p.Produces(2)));
        }

        [Test]
        public void SelectMany_2()
        {
            var parser1 = Fail<int>(new Exception("Error!"));
            var parser2 = Return(2);

            var composite = parser1.SelectMany(_ => parser2);

            ParserAssert.That(composite.OnAnyInput(), p => p.Fails());
        }

        [Test]
        public void Regex_1()
        {
            var parser = Regex(@"\d+");

            ParserAssert.That(parser.OnInput("1234567890adfdfasf"), p => p.ConsumesAndProduces("1234567890"));
        }

        [Test]
        public void Slice_1()
        {
            var silentParser = IgnoreResult(String("foobar"));

            var slice = Slice(silentParser);

            ParserAssert.That(slice.OnInput("foobartralala"), p => p.ConsumesAndProduces("foobar"));
        }

        private static Parser<Unit> IgnoreResult<T>(Parser<T> parser)
        {
            return parser.SelectMany(_ => Return(Unit.Value));
        }

        [Test]
        public void Or_1()
        {
            var parser1 = Fail<int>(new Exception("Error!"));
            var parser2 = Return(2);

            ParserAssert.That(parser1.Or(parser2).OnAnyInput(), p => p.ConsumesNothing().And(p.Produces(2)));
        }

        [Test]
        public void Or_2()
        {
            var parser1 = Return(1);
            var parser2 = Return(2);

            ParserAssert.That(parser1.Or(parser2).OnAnyInput(), p => p.ConsumesNothing().And(p.Produces(1)));
        }

        [Test]
        public void Or_3()
        {
            var parser = String("str1").Or(String("some str2"));
            ParserAssert.That(parser.OnInput("some str2 tralala tralala"), p => p.ConsumesAndProduces("some str2"));
        }
    }
}
