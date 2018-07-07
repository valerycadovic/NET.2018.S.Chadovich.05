using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Day5.StringExtensions;

namespace Day5.Tests.StringExtensions_Tests
{
    [TestFixture]
    public class Notation_Tests
    {
        [TestCase(19)]
        [TestCase(1)]
        public void Ctor_Throws_When_Base_In_Out_Of_Range_From_2_To_16(int @base) =>
            Assert.Throws<ArgumentOutOfRangeException>(() => new Notation(@base));

        [TestCase(2, ExpectedResult = "01")]
        [TestCase(10, ExpectedResult = "0123456789")]
        [TestCase(16, ExpectedResult = "0123456789ABCDEF")]
        public string Digits_Returns_Valid_Set(int @base) =>
            new Notation(@base).Digits;
    }
}
