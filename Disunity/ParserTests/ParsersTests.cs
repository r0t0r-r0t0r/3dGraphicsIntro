using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Disunity.App.Lib.Parsing.Parsers;
using static Disunity.App.Lib.Parsing.PrimitiveParsers;

namespace ParserTests
{
    [TestFixture]
    class ParsersTests
    {
        [Test]
        public void Many_1()
        {
            var ab = String("ab");

            var abs = Many(ab);

            var expected = ImmutableList.Create<string>();

            ParserAssert.That(abs.OnInput("xyz"), p => p.ConsumesNothing().And(p.Produces(expected, ComparingBy.Items<string>())));
        }

        [Test]
        public void Many_2()
        {
            var ab = String("ab");

            var abs = Many(ab);

            var expected = ImmutableList.Create(new[] { "ab" });

            ParserAssert.That(abs.OnInput("abcdef"), p => p.Consumes("ab").And(p.Produces(expected, ComparingBy.Items<string>())));
        }

        [Test]
        public void Many_3()
        {
            var ab = String("ab");

            var abs = Many(ab);

            var expected = ImmutableList.Create(new[] { "ab", "ab" });

            ParserAssert.That(abs.OnInput("ababttt"), p => p.Consumes("abab").And(p.Produces(expected, ComparingBy.Items<string>())));
        }

        [Test]
        public void Select2_1()
        {
            var p1 = Return(1);
            var p2 = Return(2);

            var select2 = Select2(p1, () => p2, Tuple.Create);

            var expected = Tuple.Create(1, 2);

            ParserAssert.That(select2.OnAnyInput(), p => p.ConsumesNothing().And(p.Produces(expected)));
        }

        [Test]
        public void Select2_2()
        {
            var p1 = Fail<int>(new Exception("Error!"));
            var p2 = Return(2);

            var select2 = Select2(p1, () => p2, Tuple.Create);

            ParserAssert.That(select2.OnAnyInput(), p => p.Fails());
        }

        [Test]
        public void Select2_3()
        {
            var p1 = Return(2);
            var p2 = Fail<int>(new Exception("Error!"));

            var select2 = Select2(p1, () => p2, Tuple.Create);

            ParserAssert.That(select2.OnAnyInput(), p => p.Fails());
        }

        [Test]
        public void Select2_4()
        {
            var p1 = String("ab");
            var p2 = String("cd");

            var select2 = Select2(p1, () => p2, Tuple.Create);

            var expected = Tuple.Create("ab", "cd");

            ParserAssert.That(select2.OnInput("abcdefghijklmnopqrstuvwxyz"), p => p.Consumes("abcd").And(p.Produces(expected)));
        }
    }
}
