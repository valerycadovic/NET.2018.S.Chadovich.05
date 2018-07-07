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
    public class StringHelper_Tests
    {
        [TestCase("0110111101100001100001010111111", 2, ExpectedResult = 934331071)]
        [TestCase("01101111011001100001010111111", 2, ExpectedResult = 233620159)]
        [TestCase("11101101111011001100001010", 2, ExpectedResult = 62370570)]
        [TestCase("1aCb67", 16, ExpectedResult = 1756007)]
        [TestCase("1ACB67", 16, ExpectedResult = 1756007)]
        [TestCase("764241", 8, ExpectedResult = 256161)]
        [TestCase("10", 5, ExpectedResult = 5)]
        [TestCase("23423523", 10, ExpectedResult = 23423523)]
        public int ToDecimalNotation_Can_Convert_String_Representation_To_Int32(string source, int notationBase) =>
            source.ToDecimalNotation(new Notation(notationBase));

        [TestCase("1111111111111111111111111111111111111111111111111111111111111111111111111", 2)]
        public void ToDecimalNotation_Throws_OverflowException_When_Source_Is_Bigger_Than_Int32_MaxValue(string source, int @base) => 
            Assert.Throws<OverflowException>(() => source.ToDecimalNotation(new Notation(@base)));

        [TestCase("54fg4", 16)]
        [TestCase("123", 2)]
        [TestCase("...", 7)]
        public void ToDecimalNotation_Throws_FormatException_If_Source_Number_Contains_Wrong_Symbols(string source, int @base) =>
            Assert.Throws<FormatException>(() => source.ToDecimalNotation(new Notation(@base)));

        [TestCase("", 4)]
        [TestCase(null, 5)]
        public void ToDecimalNotation_Throws_ArgumentException_If_Source_Is_Null_Or_Empty(string source, int @base) =>
            Assert.Throws<ArgumentException>(() => source.ToDecimalNotation(new Notation(@base)));

        public void ToDecimalNotation_Throws_ArgumentNullException_If_Notation_Is_Null(string source) =>
            Assert.Throws<ArgumentNullException>(() => source.ToDecimalNotation(null));
    }
}