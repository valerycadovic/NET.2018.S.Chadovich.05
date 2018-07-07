namespace Day5.StringExtensions
{
    using System;

    /// <summary>
    /// Set of extension methods for strings
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Converts string representation of the n-base number (where n is in range of (2 to 16)) to the decimal notation.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="notation">The notation.</param>
        /// <returns>The represented number.</returns>
        /// <exception cref="ArgumentException">Throws when the source is null or empty</exception>
        /// <exception cref="ArgumentNullException">Throws when the notation is null</exception>
        /// <exception cref="FormatException">
        /// Throws when the source has a symbol not from the range of notation digits
        /// </exception>
        public static int ToDecimalNotation(this string source, Notation notation)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentException($"{nameof(source)} is null or empty");
            }

            if (notation == null)
            {
                throw new ArgumentNullException($"{nameof(notation)} is null");
            }

            string toIterate = source.ToUpper().TrimStart('0');         // convert no normalized form
                                                                        // to prevent false overflow
            int result = 0, digitRank = 1;

            for (int i = toIterate.Length - 1; i >= 0; i--)
            {
                int value = notation.Digits.IndexOf(toIterate[i]);

                if (value < 0)
                {
                    throw new FormatException($"{nameof(source)} has a wrong format.");
                }

                checked
                {
                    result += value * digitRank;
                    digitRank *= notation.Base;
                }
            }

            return result;
        }
    }
}