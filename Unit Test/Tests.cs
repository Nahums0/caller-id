using System;
using NUnit.Framework;
using Called_Id;

namespace Unit_Test
{
    [TestFixture]
    public class TestsSample
    {
        LoginPage lp = new LoginPage();
        [SetUp]
        public void Setup()
        {

        }
        [Test]
        public void ValidateNumber()
        {
            var valid = lp.ValidateNumber("+972 549648822");
            Assert.IsTrue(valid);
            valid = lp.ValidateNumber("+54 341 512 2188");
            Assert.IsTrue(valid);
            valid = lp.ValidateNumber("+56 41 256 0288");
            Assert.IsTrue(valid);
            valid = !lp.ValidateNumber("972 549648822");
            Assert.IsTrue(valid);
        }



    }
}