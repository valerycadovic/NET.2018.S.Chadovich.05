namespace Day5.StringExtensions
{
    using System;

    /// <summary>
    /// Provides a set of digits for determined notation and operations th them
    /// </summary>
    public class Notation
    {
        /// <summary>
        /// All digits available
        /// </summary>
        private const string AllDigits = "0123456789ABCDEF";

        /// <summary>
        /// The minimal base
        /// </summary>
        private const int Min = 2;

        /// <summary>
        /// The maximal base
        /// </summary>
        private const int Max = 16;

        /// <summary>
        /// Initializes a new instance of the <see cref="Notation"/> class.
        /// </summary>
        /// <param name="base">The base.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws when notation is out of the range of 2 to 16</exception>
        public Notation(int @base)
        {
            if (@base > Max || @base < Min)
            {
                throw new ArgumentOutOfRangeException($"{nameof(@base)} must be in the range of 2 to 16");
            }

            this.Base = @base;
        }

        /// <summary>
        /// Gets the digits.
        /// </summary>
        /// <value>
        /// The digits.
        /// </value>
        public string Digits
        {
            get => AllDigits.Substring(0, this.Base);
        }

        /// <summary>
        /// Gets the base.
        /// </summary>
        /// <value>
        /// The base.
        /// </value>
        public int Base { get; }
    }
}
