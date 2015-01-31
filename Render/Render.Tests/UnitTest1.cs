using NUnit.Framework;
using System;
using System.Drawing;

namespace Render.Tests
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void Test()
        {
            var bitmap = new Bitmap(50, 50);
            RenderCore.Triangle(10, 20, 40, 20, 60, 60, bitmap, Color.Red);
        }
    }
}
