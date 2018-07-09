using System.Globalization;

namespace Day5.ImmutablePolynomial
{
    using System;
    using System.Text;

    /// <summary>
    /// Represents an immutable polynomial with floating-point coefficients
    /// </summary>
    public sealed class Polynomial : IEquatable<Polynomial>, ICloneable
    {
        #region local variables
        /// <summary>
        /// The epsilon
        /// </summary>
        private const double Epsilon = 10e-10;

        /// <summary>
        /// The coeffs
        /// </summary>
        private readonly double[] coeffs;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Polynomial"/> class.
        /// </summary>
        /// <param name="coeffs">The coefficients.</param>
        /// <exception cref="ArgumentException">throws when coefficients have no values</exception>
        /// <exception cref="ArgumentNullException">throws when coefficients is null</exception>
        public Polynomial(params double[] coeffs)
        {
            ValidateIsNull(coeffs, nameof(coeffs));

            if (coeffs.Length == 0)
            {
                throw new ArgumentException($"{coeffs} must have at least one argument");
            }

            this.Power = FindFirstUnsignificantZero(coeffs);

            this.coeffs = new double[this.Power + 1];
            Array.Copy(coeffs, this.coeffs, this.Power + 1);
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the power of the polynomial
        /// </summary>
        /// <value>
        /// The power.
        /// </value>
        public int Power { get; }

        /// <summary>
        /// Gets the <see cref="System.Double"/> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="System.Double"/>.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns>Value at preset index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is out of polynomial</exception>
        public double this[int index]
        {
            get
            {
                if (index < 0 || index > this.coeffs.Length)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(index)} is out of polynomial");
                }

                return this.coeffs[index];
            }
        }
        #endregion

        #region Overloaded operators
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Polynomial lhs, Polynomial rhs)
        {
            if (ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (lhs is null || rhs is null)
            {
                return false;
            }

            return ValueEquals(lhs.ToArray(), rhs.ToArray());
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Polynomial lhs, Polynomial rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when LHS or RHS is null
        /// </exception>
        public static Polynomial operator +(Polynomial lhs, Polynomial rhs)
        {
            ValidateIsNull(lhs, nameof(lhs));
            ValidateIsNull(rhs, nameof(rhs));

            double[] result = Sum(lhs.ToArray(), rhs.ToArray());

            return new Polynomial(result);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when LHS or RHS is null
        /// </exception>
        public static Polynomial operator +(Polynomial lhs, double[] rhs)
        {
            ValidateIsNull(lhs, nameof(lhs));
            ValidateIsNull(rhs, nameof(rhs));

            double[] result = Sum(lhs.ToArray(), rhs);

            return new Polynomial(result);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when LHS or RHS is null
        /// </exception>
        public static Polynomial operator +(double[] lhs, Polynomial rhs)
        {
            ValidateIsNull(lhs, nameof(lhs));
            ValidateIsNull(rhs, nameof(rhs));

            double[] result = Sum(lhs, rhs.ToArray());

            return new Polynomial(result);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when RHS or LHS is null
        /// </exception>
        public static Polynomial operator -(Polynomial lhs, Polynomial rhs)
        {
            ValidateIsNull(lhs, nameof(lhs));
            ValidateIsNull(rhs, nameof(rhs));

            double[] result = Diff(lhs.ToArray(), rhs.ToArray());

            return new Polynomial(result);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when RHS or LHS is null
        /// </exception>
        public static Polynomial operator -(Polynomial lhs, double[] rhs)
        {
            ValidateIsNull(lhs, nameof(lhs));
            ValidateIsNull(rhs, nameof(rhs));

            double[] result = Diff(lhs.ToArray(), rhs);

            return new Polynomial(result);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when RHS or LHS is null
        /// </exception>
        public static Polynomial operator -(double[] lhs, Polynomial rhs)
        {
            ValidateIsNull(lhs, nameof(lhs));
            ValidateIsNull(rhs, nameof(rhs));

            double[] result = Diff(lhs, rhs.ToArray());

            return new Polynomial(result);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Polynomial operator -(Polynomial p)
        {
            return new Polynomial(InverseValues(p.ToArray()));
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when LHS or RHS is null
        /// </exception>
        public static Polynomial operator *(Polynomial lhs, Polynomial rhs)
        {
            ValidateIsNull(lhs, nameof(lhs));
            ValidateIsNull(rhs, nameof(rhs));

            double[] result = Mul(lhs.ToArray(), rhs.ToArray());

            return new Polynomial(result);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when LHS is null
        /// </exception>
        public static Polynomial operator *(Polynomial lhs, double rhs)
        {
            ValidateIsNull(lhs, nameof(lhs));

            double[] result = Mul(lhs.ToArray(), rhs);

            return new Polynomial(result);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when RHS is null
        /// </exception>
        public static Polynomial operator *(double lhs, Polynomial rhs)
        {
            ValidateIsNull(rhs, nameof(rhs));

            double[] result = Mul(rhs.ToArray(), lhs);

            return new Polynomial(result);
        }
        #endregion

        #region Operators proxies for languages which do not support operators overloading
        /// <summary>
        /// Adds the specified LHS to RHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when LHS or RHS is null
        /// </exception>
        public static Polynomial Add(Polynomial lhs, Polynomial rhs) => lhs + rhs;

        /// <summary>
        /// Adds the specified RHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when LHS or RHS is null
        /// </exception>
        public static Polynomial Add(Polynomial lhs, double[] rhs) => lhs + rhs;

        /// <summary>
        /// Adds the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when LHS or RHS is null
        /// </exception>
        public static Polynomial Add(double[] lhs, Polynomial rhs) => lhs + rhs;

        /// <summary>
        /// Subtracts the specified RHS from LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when LHS or RHS is null
        /// </exception>
        public static Polynomial Subtract(Polynomial lhs, Polynomial rhs) => lhs - rhs;

        /// <summary>
        /// Subtracts the specified RHS from LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when LHS or RHS is null
        /// </exception>
        public static Polynomial Subtract(Polynomial lhs, double[] rhs) => lhs - rhs;

        /// <summary>
        /// Subtracts the specified RHS from LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when LHS or RHS is null
        /// </exception>
        public static Polynomial Subtract(double[] lhs, Polynomial rhs) => lhs - rhs;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool Compare(Polynomial lhs, Polynomial rhs) => lhs == rhs;

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when LHS or RHS is null
        /// </exception>
        public static Polynomial Multiply(Polynomial lhs, Polynomial rhs) => lhs * rhs;

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when LHS or RHS is null
        /// </exception>
        public static Polynomial Multiply(Polynomial lhs, double rhs) => lhs * rhs;

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Throws when LHS or RHS is null
        /// </exception>
        public static Polynomial Multiply(double lhs, Polynomial rhs) => lhs * rhs;
        #endregion

        #region Non-overrided Instance Method
        /// <summary>
        /// Gets the set of coefficients if polynomial
        /// </summary>
        /// <value>
        /// The coefficients.
        /// </value>
        public double[] ToArray()
        {
            double[] result = new double[this.coeffs.Length];
            Array.Copy(this.coeffs, result, this.coeffs.Length);

            return result;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public Polynomial Clone() => new Polynomial(this.coeffs);
        #endregion

        #region Overrided methods of object
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return (Polynomial)obj == this;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                foreach (var item in this.coeffs)
                {
                    hash = hash * 23 + item.GetHashCode();
                }

                return hash;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.FormStringRepresentation();
        }
        #endregion

        #region Interfaces implementation
        /// <inheritdoc/>
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone() => Clone();
        
        /// <inheritdoc />
        /// <summary>
        /// Equalses the specified p.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="T:Day5.ImmutablePolynomial.Polynomial" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(Polynomial p)
        {
            return p == this;
        }
        #endregion

        #region Private static methods
        /// <summary>
        /// Compares the double.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>result of comparison</returns>
        private static bool CompareDouble(double a, double b) => Math.Abs(a - b) < Epsilon;

        /// <summary>
        /// Determines whether the specified o is null.
        /// </summary>
        /// <param name="obj">The o.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">throws when o is null</exception>
        private static void ValidateIsNull<T>(T obj, string name) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException($"{name} is null");
            }
        }

        /// <summary>
        /// Values the equals.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>result of value comparison</returns>
        private static bool ValueEquals(double[] lhs, double[] rhs)
        {
            if (rhs.Length != lhs.Length)
            {
                return false;
            }

            for (int i = 0; i < rhs.Length; i++)
            {
                if (!CompareDouble(lhs[i], rhs[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sums the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>Sum of polynomial inner arrays</returns>
        private static double[] Sum(double[] lhs, double[] rhs)
        {
            var (longer, shorter) = FindLongerAndShorter(lhs, rhs);

            int difference = CopyDifference(longer, shorter, out double[] result);
            for (int i = 0; i < difference; i++)
            {
                result[i] = longer[i] + shorter[i];
            }

            return result;
        }

        /// <summary>
        /// Differences the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>Difference of polynomial inner arrays</returns>
        private static double[] Diff(double[] lhs, double[] rhs)
        {
            var (longer, shorter) = FindLongerAndShorter(lhs, rhs);

            if (longer == rhs)
            {
                longer = InverseValues(longer);
                shorter = InverseValues(shorter);
            }

            int difference = CopyDifference(longer, shorter, out double[] result);

            for (int i = 0; i < difference; i++)
            {
                result[i] = longer[i] - shorter[i];
            }

            return result;
        }

        /// <summary>
        /// Multiplies the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>Composition of polynomial inner arrays</returns>
        private static double[] Mul(double[] lhs, double[] rhs)
        {
            double[] result = new double[lhs.Length + rhs.Length - 1];

            for (int i = 0; i < lhs.Length; i++)
            {
                if (!CompareDouble(lhs[i], 0.0))
                {
                    for (int j = 0; j < rhs.Length; j++)
                    {
                        result[i + j] += lhs[i] * rhs[j];
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Multiplies the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>Composition of polynomial inner array and <see cref="double"/> value</returns>
        private static double[] Mul(double[] lhs, double rhs)
        {
            double[] result = new double[lhs.Length];

            for (int i = 0; i < lhs.Length; i++)
            {
                result[i] = lhs[i] * rhs;
            }

            return result;
        }

        /// <summary>
        /// Inverses the values.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>The array with inversed values</returns>
        private static double[] InverseValues(double[] array)
        {
            double[] result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = -1 * array[i];
            }

            return result;
        }

        /// <summary>
        /// Copies the difference.
        /// </summary>
        /// <param name="longer">The longer.</param>
        /// <param name="shorter">The shorter.</param>
        /// <param name="result">The result.</param>
        /// <returns>The index of the element which is in shorter's length + 1 position</returns>
        private static int CopyDifference(double[] longer, double[] shorter, out double[] result)
        {
            int size = longer.Length;
            result = new double[size];

            if (size == shorter.Length)
            {
                return size;
            }

            int difference = shorter.Length;
            Array.Copy(longer, difference, result, difference, size - difference);

            return difference;
        }

        /// <summary>
        /// Finds the longer and shorter.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns>references on longer and shorter arrays</returns>
        private static (double[] longer, double[] shorter) FindLongerAndShorter(double[] lhs, double[] rhs)
        {
            double[] shorter, longer;

            if (lhs.Length > rhs.Length)
            {
                shorter = rhs;
                longer = lhs;
            }
            else
            {
                shorter = lhs;
                longer = rhs;
            }

            return (longer, shorter);
        }

        /// <summary>
        /// Finds the first unsignificant zero.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>The index of the first unsignificant zero</returns>
        private static int FindFirstUnsignificantZero(double[] array)
        {
            for (int i = array.Length - 1; i >= 0; i--)
            {
                if (!CompareDouble(array[i], 0.0))
                {
                    return i;
                }
            }

            return array.Length - 1;
        }
        #endregion

        #region Private instance methods
        /// <summary>
        /// Forms the string representation.
        /// </summary>
        /// <returns>Returns the string representation</returns>
        private string FormStringRepresentation()
        {
            if (this.Power == 0)
            {
                return this[0].ToString(CultureInfo.InvariantCulture);
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(this.FormFirst());

            if (this.Power != 1)
            {
                sb.Append(this.FormMiddle());
            }

            sb.Append(this.FormLast());

            return sb.ToString();
        }

        /// <summary>
        /// Forms the first term of the polynomial
        /// </summary>
        /// <returns>Formed the first term of the polynomial</returns>
        private string FormFirst()
        {
            StringBuilder sb = new StringBuilder();

            if (!CompareDouble(this[this.Power], 1))
            {
                sb.Append(this[this.Power]);
            }

            sb.Append(this.Power > 1 ? $"x^{this.Power}" : "x");

            return sb.ToString();
        }

        /// <summary>
        /// Forms the middle.
        /// </summary>
        /// <returns>Formed middle term of the polynomial</returns>
        private string FormMiddle()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = Power - 1; i >= 1; i--)
            {
                if (CompareDouble(this[i], 0.0))
                {
                    continue;
                }

                sb.Append(this[i] < 0 ? " - " : " + ");
                sb.Append(this.FormTerm(i));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Forms the last.
        /// </summary>
        /// <returns>Formed last term of the polynomial</returns>
        private string FormLast()
        {
            int last = 0;

            if (CompareDouble(this[last], 0.0))
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(this[last] < 0 ? " - " : " + ");
            sb.Append(Math.Abs(this[last]));

            return sb.ToString();
        }

        /// <summary>
        /// Forms the term.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Formed term</returns>
        private string FormTerm(int index)
        {
            StringBuilder sb = new StringBuilder();

            if (!CompareDouble(Math.Abs(this[index]), 1))
            {
                sb.Append(Math.Abs(this[index]));
            }

            sb.Append(index == 1 ? "x" : $"x^{index}");

            return sb.ToString();
        }
        #endregion
    }
}
// 877 rows ))))))