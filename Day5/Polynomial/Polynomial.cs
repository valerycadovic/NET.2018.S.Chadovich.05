namespace Day5.ImmutablePolynomial
{
    using System;
    using System.Text;

    /// <summary>
    /// Represents an immutable polynomial with floating-point coefficients
    /// </summary>
    public sealed class Polynomial
    {
        #region Local variables and constatns
        /// <summary>
        /// The epsilon
        /// </summary>
        private const double Epsilon = 10e-10;

        /// <summary>
        /// The string representation of the polynomial
        /// </summary>
        private readonly string stringRepresentation;
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
            if (coeffs == null)
            {
                throw new ArgumentNullException($"{nameof(coeffs)} is null");
            }

            if (coeffs.Length == 0)
            {
                throw new ArgumentException($"{coeffs} must have at least one argument");
            }

            int firstNonZero = this.FindFirstValue(coeffs);

            if (firstNonZero == -1)
            {
                this.Coeffs = new double[] { 0.0 };
                this.stringRepresentation = this.FormStringRepresentation();
                return;
            }

            this.Power = coeffs.Length - firstNonZero - 1;

            this.Coeffs = new double[this.Power + 1];
            Array.Copy(coeffs, firstNonZero, this.Coeffs, 0, this.Power + 1);

            this.stringRepresentation = this.FormStringRepresentation();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the set of coefficients if polynomial
        /// </summary>
        /// <value>
        /// The coefficients.
        /// </value>
        public double[] Coeffs { get; }

        /// <summary>
        /// Gets the power of the polynomial
        /// </summary>
        /// <value>
        /// The power.
        /// </value>
        public int Power { get; private set; }
        #endregion

        #region Operators overloads        
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

            return ValueEquals(lhs.Coeffs, rhs.Coeffs);
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
            IsNull(lhs, nameof(lhs));
            IsNull(rhs, nameof(rhs));

            double[] result = Sum(lhs.Coeffs, rhs.Coeffs);

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
            IsNull(lhs, nameof(lhs));
            IsNull(rhs, nameof(rhs));

            double[] result = Sum(lhs.Coeffs, rhs);

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
            IsNull(lhs, nameof(lhs));
            IsNull(rhs, nameof(rhs));

            double[] result = Sum(lhs, rhs.Coeffs);

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
            IsNull(lhs, nameof(lhs));
            IsNull(rhs, nameof(rhs));

            double[] result = Diff(lhs.Coeffs, rhs.Coeffs);

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
            IsNull(lhs, nameof(lhs));
            IsNull(rhs, nameof(rhs));

            double[] result = Diff(lhs.Coeffs, rhs);

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
            IsNull(lhs, nameof(lhs));
            IsNull(rhs, nameof(rhs));

            double[] result = Diff(lhs, rhs.Coeffs);

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
        /// Throws when LHS or RHS is null
        /// </exception>
        public static Polynomial operator *(Polynomial lhs, Polynomial rhs)
        {
            IsNull(lhs, nameof(lhs));
            IsNull(rhs, nameof(rhs));

            double[] result = Mul(lhs.Coeffs, rhs.Coeffs);

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
            IsNull(lhs, nameof(lhs));

            double[] result = Mul(lhs.Coeffs, rhs);

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
            IsNull(rhs, nameof(rhs));

            double[] result = Mul(rhs.Coeffs, lhs);

            return new Polynomial(result);
        }
        #endregion

        #region System.Object overloads
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case Polynomial p when p == this:
                    return true;
                default: return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() => this.Coeffs.GetHashCode();

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.stringRepresentation;
        }
        #endregion

        #region Private operations with arrays        
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
                if (Math.Abs(lhs[i]) > Epsilon)
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
            var(longer, shorter) = FindLongerAndShorter(lhs, rhs);

            int difference = CopyDifference(longer, shorter, out double[] result);

            for (int i = difference; i < longer.Length; i++)
            {
                result[i] = longer[i] + shorter[i - difference];
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
            var(longer, shorter) = FindLongerAndShorter(lhs, rhs);

            if (longer == rhs)
            {
                longer = InverseValues(longer);
                shorter = InverseValues(shorter);
            }

            int difference = CopyDifference(longer, shorter, out double[] result);

            for (int i = difference; i < longer.Length; i++)
            {
                result[i] = longer[i] - shorter[i - difference];
            }

            return result;
        }
        #endregion

        #region Other private helpers        
        /// <summary>
        /// Copies the difference.
        /// </summary>
        /// <param name="longer">The longer.</param>
        /// <param name="shorter">The shorter.</param>
        /// <param name="result">The result.</param>
        /// <returns>The difference of arrays lengths</returns>
        private static int CopyDifference(double[] longer, double[] shorter, out double[] result)
        {
            int size = longer.Length;
            result = new double[size];

            int difference = longer.Length - shorter.Length;
            if (difference != 0)
            {
                Array.Copy(longer, result, difference);
            }

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
        /// Compares the double.
        /// </summary>
        /// <param name="a">The a.</param>
        /// <param name="b">The b.</param>
        /// <returns>result of comparison</returns>
        private static bool CompareDouble(double a, double b) => Math.Abs(a - b) < Epsilon;

        /// <summary>
        /// Determines whether the specified o is null.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="name">The name.</param>
        /// <exception cref="ArgumentNullException">throws when o is null</exception>
        private static void IsNull(object o, string name)
        {
            if (o == null)
            {
                throw new ArgumentNullException($"{name} is null");
            }
        }
        #endregion

        #region Private instance helpers
        /// <summary>
        /// Forms the string representation.
        /// </summary>
        /// <returns>Returns the string representation</returns>
        private string FormStringRepresentation()
        {
            if (this.Power == 0)
            {
                return this.Coeffs[0].ToString();
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

            if (!CompareDouble(this.Coeffs[0], 1))
            {
                sb.Append(this.Coeffs[0]);
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

            for (int i = 1; i < this.Coeffs.Length - 1; i++)
            {
                if (CompareDouble(this.Coeffs[i], 0.0))
                {
                    continue;
                }

                sb.Append(this.Coeffs[i] < 0 ? " - " : " + ");
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
            int last = this.Coeffs.Length - 1;

            if (CompareDouble(this.Coeffs[last], 0.0))
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(this.Coeffs[last] < 0 ? " - " : " + ");
            sb.Append(Math.Abs(this.Coeffs[last]));

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
            int currentPower = this.Power - index;

            if (!CompareDouble(this.Coeffs[index], 1.0))
            {
                sb.Append(Math.Abs(this.Coeffs[index]));
            }

            sb.Append(currentPower == 1 ? "x" : $"x^{currentPower}");

            return sb.ToString();
        }

        /// <summary>
        /// Finds the first value.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns>The index of the first value</returns>
        private int FindFirstValue(double[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (!CompareDouble(array[i], 0.0))
                {
                    return i;
                }
            }

            return -1;
        }
        #endregion
    }
}
