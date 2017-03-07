using NUnit.Framework;
using static Disunity.App.Lib.Parsing.TailRec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Disunity.App.Lib.Parsing;

namespace ParserTests
{
    [TestFixture]
    class TailRecTests
    {
        [Test]
        public void TailRec_1()
        {
            var value = "foobar";

            Func<string, TailRec<string>> f = x => Return(x);

            var g = Enumerable.Repeat(f, 100000).Aggregate((a, b) => x => SuspendTailRec(() => a(x).SelectMany(b)));

            var result = g(value).Run();

            Assert.That(result, Is.EqualTo(value));
        }

        [Test]
        public void TailRec_2()
        {
            var a = Suspend(() => "a");
            var b = Suspend(() => "b");
            var c = Suspend(() => "c");

            var abc =
                from x in a
                from y in b
                from z in c
                select x + y + z;

            var result = abc.Run();

            Assert.That(result, Is.EqualTo("abc"));
        }

    }
}
