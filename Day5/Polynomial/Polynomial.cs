using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day5.ImmutablePolynomial
{
    public sealed class Polynomial
    {
        private const double epsilon = 10e-10;
        private readonly string stringRepresentation;

        public Polynomial(params double[] coeffs)
        {
            if (coeffs.Length == 0)
            {
                throw new ArgumentException($"{coeffs} must have at least one argument");
            }

            int firstNonZero = FindFirstValue(coeffs);

            if (firstNonZero == -1)
            {
                this.Coeffs = new double[] { 0.0 };
                this.stringRepresentation = FormStringRepresentation();
                return;
            }

            this.Power = coeffs.Length - firstNonZero - 1;

            this.Coeffs = new double[this.Power + 1];
            Array.Copy(coeffs, firstNonZero, this.Coeffs, 0, this.Power + 1);

            this.stringRepresentation = FormStringRepresentation();
        }

        public double[] Coeffs { get; }

        public int Power { get; private set; }

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

        public override int GetHashCode() => this.Coeffs.GetHashCode();

        public override string ToString()
        {
            return this.stringRepresentation;
        }

        private string FormStringRepresentation()
        {
            if (this.Power == 0)
            {
                return this.Coeffs[0].ToString();
            }

            StringBuilder sb = new StringBuilder();

            sb.Append(FormFirst());

            if (this.Power != 1)
            {
                sb.Append(FormMiddle());
            }

            sb.Append(FormLast());

            return sb.ToString();
        }

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
                sb.Append(FormTerm(i));
            }

            return sb.ToString();
        }

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

        #region Operators overloads
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

        public static bool operator !=(Polynomial lhs, Polynomial rhs)
        {
            return !(lhs == rhs);
        }

        public static Polynomial operator +(Polynomial lhs, Polynomial rhs)
        {
            IsNull(lhs, nameof(lhs));
            IsNull(rhs, nameof(rhs));

            double[] result = Sum(lhs.Coeffs, rhs.Coeffs);

            return new Polynomial(result);
        }

        public static Polynomial operator +(Polynomial lhs, double[] rhs)
        {
            IsNull(lhs, nameof(lhs));
            IsNull(rhs, nameof(rhs));

            double[] result = Sum(lhs.Coeffs, rhs);

            return new Polynomial(result);
        }

        public static Polynomial operator +(double[] lhs, Polynomial rhs)
        {
            IsNull(lhs, nameof(lhs));
            IsNull(rhs, nameof(rhs));

            double[] result = Sum(lhs, rhs.Coeffs);

            return new Polynomial(result);
        }

        public static Polynomial operator -(Polynomial lhs, Polynomial rhs)
        {
            IsNull(lhs, nameof(lhs));
            IsNull(rhs, nameof(rhs));

            double[] result = Diff(lhs.Coeffs, rhs.Coeffs);

            return new Polynomial(result);
        }

        public static Polynomial operator -(Polynomial lhs, double[] rhs)
        {
            IsNull(lhs, nameof(lhs));
            IsNull(rhs, nameof(rhs));

            double[] result = Diff(lhs.Coeffs, rhs);

            return new Polynomial(result);
        }

        public static Polynomial operator -(double[] lhs, Polynomial rhs)
        {
            IsNull(lhs, nameof(lhs));
            IsNull(rhs, nameof(rhs));

            double[] result = Diff(lhs, rhs.Coeffs);

            return new Polynomial(result);
        }

        public static Polynomial operator *(Polynomial lhs, Polynomial rhs)
        {
            IsNull(lhs, nameof(lhs));
            IsNull(rhs, nameof(rhs));

            double[] result = Mul(lhs.Coeffs, rhs.Coeffs);

            return new Polynomial(result);
        }

        public static Polynomial operator *(Polynomial lhs, double rhs)
        {
            IsNull(lhs, nameof(lhs));

            double[] result = Mul(lhs.Coeffs, rhs);

            return new Polynomial(result);
        }

        public static Polynomial operator *(double lhs, Polynomial rhs)
        {
            IsNull(rhs, nameof(rhs));

            double[] result = Mul(rhs.Coeffs, lhs);

            return new Polynomial(result);
        }
        #endregion

        #region Private operations with arrays
        private static double[] Mul(double[] lhs, double rhs)
        {
            double[] result = new double[lhs.Length];

            for (int i = 0; i < lhs.Length; i++)
            {
                result[i] = lhs[i] * rhs;
            }

            return result;
        }

        private static double[] Mul(double[] lhs, double[] rhs)
        {
            double[] result = new double[lhs.Length + rhs.Length - 1];

            for (int i = 0; i < lhs.Length; i++)
            {
                if (Math.Abs(lhs[i]) > epsilon)
                {
                    for (int j = 0; j < rhs.Length; j++)
                    {
                        result[i + j] += lhs[i] * rhs[j];
                    }
                }
            }

            return result;
        }

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

        private static double[] Sum(double[] lhs, double[] rhs)
        {
            var (longer, shorter) = FindLongerAndShorter(lhs, rhs);

            int difference = CopyDifference(longer, shorter, out double[] result);

            for (int i = difference; i < longer.Length; i++)
            {
                result[i] = longer[i] + shorter[i - difference];
            }

            return result;
        }

        private static double[] Diff(double[] lhs, double[] rhs)
        {
            var (longer, shorter) = FindLongerAndShorter(lhs, rhs);

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

        private static double[] InverseValues(double[] array)
        {
            double[] result = new double[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = -1 * array[i];
            }

            return result;
        }

        private static bool CompareDouble(double a, double b) => Math.Abs(a - b) < epsilon;

        private static void IsNull(object o, string name)
        {
            if (o == null)
            {
                throw new ArgumentNullException($"{name} is null");
            }
        }
        #endregion
    }
}
