using System;
using System.Collections.Immutable;
using NUnit.Framework;

namespace Disunity.Parsing
{
    [TestFixture]
    class ParsersTests
    {
        [Test]
        public void Many_1()
        {
            var ab = PrimitiveParsers.String("ab");

            var abs = Parsers.Many(ab);

            var expected = ImmutableList.Create<string>();

            ParserAssert.That(abs.OnInput("xyz"), p => p.ConsumesNothing().And(p.Produces(expected, ComparingBy.Items<string>())));
        }

        [Test]
        public void Many_2()
        {
            var ab = PrimitiveParsers.String("ab");

            var abs = Parsers.Many(ab);

            var expected = ImmutableList.Create(new[] { "ab" });

            ParserAssert.That(abs.OnInput("abcdef"), p => p.Consumes("ab").And(p.Produces(expected, ComparingBy.Items<string>())));
        }

        [Test]
        public void Many_3()
        {
            var ab = PrimitiveParsers.String("ab");

            var abs = Parsers.Many(ab);

            var expected = ImmutableList.Create(new[] { "ab", "ab" });

            ParserAssert.That(abs.OnInput("ababttt"), p => p.Consumes("abab").And(p.Produces(expected, ComparingBy.Items<string>())));
        }

        [Test]
        public void Select2_1()
        {
            var p1 = PrimitiveParsers.Return(1);
            var p2 = PrimitiveParsers.Return(2);

            var select2 = Parsers.Select2(p1, () => p2, Tuple.Create);

            var expected = Tuple.Create(1, 2);

            ParserAssert.That(select2.OnAnyInput(), p => p.ConsumesNothing().And(p.Produces(expected)));
        }

        [Test]
        public void Select2_2()
        {
            var p1 = PrimitiveParsers.Fail<int>(new Exception("Error!"));
            var p2 = PrimitiveParsers.Return(2);

            var select2 = Parsers.Select2(p1, () => p2, Tuple.Create);

            ParserAssert.That(select2.OnAnyInput(), p => p.Fails());
        }

        [Test]
        public void Select2_3()
        {
            var p1 = PrimitiveParsers.Return(2);
            var p2 = PrimitiveParsers.Fail<int>(new Exception("Error!"));

            var select2 = Parsers.Select2(p1, () => p2, Tuple.Create);

            ParserAssert.That(select2.OnAnyInput(), p => p.Fails());
        }

        [Test]
        public void Select2_4()
        {
            var p1 = PrimitiveParsers.String("ab");
            var p2 = PrimitiveParsers.String("cd");

            var select2 = Parsers.Select2(p1, () => p2, Tuple.Create);

            var expected = Tuple.Create("ab", "cd");

            ParserAssert.That(select2.OnInput("abcdefghijklmnopqrstuvwxyz"), p => p.Consumes("abcd").And(p.Produces(expected)));
        }
    }
}
