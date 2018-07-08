using System;
using System.Collections.Generic;
using NUnit.Framework;
using Day5.ImmutablePolynomial;

namespace Day5.Tests.Polynomial_Tests
{
    [TestFixture]
    public class Polynomial_Tests
    {
        [TestCase()]
        public void Ctor_Throws_FormatException_When_All_Zeros(params double[] values) =>
            Assert.Throws<ArgumentException>(() => new Polynomial(values));

        [TestCase(new double[] { 1d, 2d, 3d }, new double[] { 1d, 2d, 3d }, ExpectedResult = true)]
        [TestCase(new double[] { 0d, 1d, 2d, 3d }, new double[] { 1d, 2d, 3d }, ExpectedResult = true)]
        [TestCase(new double[] { 1d, 2d }, new double[] { 1d, 2d, 3d }, ExpectedResult = false)]
        [TestCase(new double[] { 1d, 2d }, new double[] { 2d, 3d }, ExpectedResult = false)]
        [TestCase(new double[] { 1.1999999999, -5.89999999999 }, new double[] { 1.2, -5.9 }, ExpectedResult = true)]
        public bool Operator_Equals_Is_Correct(double[] l, double[] r)
        {
            Polynomial lhs = new Polynomial(l);
            Polynomial rhs = new Polynomial(r);

            return lhs == rhs;
        }

        [Test]
        public void Operator_Equals_Is_Correct_With_Refs()
        {
            Polynomial a = new Polynomial(3);
            Polynomial b = a;

            Assert.IsTrue(a == b);
        }

        [Test]
        public void Operator_Equals_Is_Correct_With_One_Null()
        {
            Polynomial a = new Polynomial(3);
            Polynomial b = null;

            Assert.IsFalse(a == b);
        }

        [TestCase(new double[] { 1d, 2d }, new double[] { 1d, 2d, 3d }, new double[] { 1d, 3d, 5d })]
        [TestCase(new double[] { 15.5, -27.1, 0.0, 0.0, 345.223 }, new double[] { 35, 0.0, 1, 56 }, new double[] { 15.5, 7.9, 0.0, 1.0, 401.223 })]
        public void Operator_Plus_Sums(double[] l, double[] r, double[] expectedResult)
        {
            Polynomial lhs = new Polynomial(l);
            Polynomial rhs = new Polynomial(r);
            Polynomial expected = new Polynomial(expectedResult);

            Polynomial result = lhs + rhs;

            Assert.IsTrue(result == expected);
        }

        [TestCase(new double[] { 1d, 2d }, new double[] { 1d, 2d, 3d }, new double[] { -1d, -1d, -1d })]
        [TestCase(
            new double[] { 15.5, -27.1, 0.0, 0.0, 345.223 },
            new double[] { 35, 0.0, 1, 56 },
            new double[] { 15.5, -62.1, 0.0, -1.0, 289.223 })]
        public void Operator_Minus_Takes_Away(double[] l, double[] r, double[] expectedResult)
        {
            Polynomial lhs = new Polynomial(l);
            Polynomial rhs = new Polynomial(r);
            Polynomial expected = new Polynomial(expectedResult);

            Polynomial result = lhs - rhs;

            Assert.IsTrue(result == expected);
        }

        [TestCase(new double[] { 1d, 2d }, new double[] { 1d, 2d, 3d }, new double[] { 1d, 4d, 7d, 6d })]
        [TestCase(
            new double[] { 15.5, -27.1, 0.0, 0.0, 345.223 },
            new double[] { 35, 0.0, 1, 56 },
            new double[] { 542.5, -948.5, 15.5, 840.9, 10565.205, 0.0, 345.223, 19332.488 })]
        public void Operator_Multiply_With_Polynomial(double[] l, double[] r, double[] expectedResult)
        {
            Polynomial lhs = new Polynomial(l);
            Polynomial rhs = new Polynomial(r);
            Polynomial expected = new Polynomial(expectedResult);

            Polynomial result = lhs * rhs;

            Assert.IsTrue(result == expected);
        }


        [TestCase(new double[] { 1, 2, 3 }, 2, new double[] { 2, 4, 6 })]
        [TestCase(
            new double[] { 15.5, -27.1, 0.0, 0.0, 345.223 },
            5.2,
            new double[] { 80.6, -140.92, 0.0, 0.0, 1795.1596 })]
        public void Operator_Multiply_With_Number(double[] l, double r, double[] expectedResult)
        {
            Polynomial lhs = new Polynomial(l);
            Polynomial expected = new Polynomial(expectedResult);

            Polynomial result = lhs * r;

            Assert.IsTrue(result == expected);
        }

        [TestCase(new double[] { 1d, 2d, 3d }, new double[] { 1d, 2d, 3d }, ExpectedResult = true)]
        [TestCase(new double[] { 0d, 1d, 2d, 3d }, new double[] { 1d, 2d, 3d }, ExpectedResult = true)]
        [TestCase(new double[] { 1d, 2d }, new double[] { 1d, 2d, 3d }, ExpectedResult = false)]
        [TestCase(new double[] { 1d, 2d }, new double[] { 2d, 3d }, ExpectedResult = false)]
        [TestCase(new double[] { 1.19999999999, -5.89999999999 }, new double[] { 1.2, -5.9 }, ExpectedResult = true)]
        public bool Overrided_Equals_Is_Correct(double[] l, double[] r)
        {
            Polynomial lhs = new Polynomial(l);
            Polynomial rhs = new Polynomial(r);

            return lhs.Equals(rhs);
        }

        [Test]
        public void Overrided_GetHashCode_Is_Correct()
        {
            Polynomial a = new Polynomial(new double[] { 1.1 });
            Polynomial b = new Polynomial(new double[] { 1.1 });
            Polynomial c = a;

            HashSet<Polynomial> hs = new HashSet<Polynomial>(new Polynomial[] { a, b, c });

            Assert.AreEqual(2, hs.Count);
        }

        [TestCase(new double[] { 1d, 2d, 3d }, ExpectedResult = "x^2 + 2x + 3")]
        [TestCase(new double[] { 1d, 2d }, ExpectedResult = "x + 2")]
        [TestCase(new double[] { 1.19999999999, -5.89999999999 }, ExpectedResult = "1,19999999999x - 5,89999999999")]
        public string Overrided_ToString_Is_Correct(double[] l)
        {
            Polynomial lhs = new Polynomial(l);

            return lhs.ToString();
        }
    }
}
