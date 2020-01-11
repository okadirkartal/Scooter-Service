using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Application.Core.Validation
{
    public static class GuardClauseExtensions
    {
        /// <summary>
        ///     Throws an <see cref="ArgumentNullException" /> if <paramref name="input" /> is null.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Null([NotNull] this IGuardClause guardClause, [NotNull] object input)
        {
            if (null == input) throw new ArgumentNullException(nameof(input));
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException" /> if <paramref name="input" /> is null.
        ///     Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is an empty string.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void Empty([NotNull] this IGuardClause guardClause, [NotNull] string input)
        {
            if (input == string.Empty)
                throw new ArgumentException($"Required input {nameof(input)} was empty.", nameof(input));
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException" /> if <paramref name="input" /> is null.
        ///     Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is an empty enumerable.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static void NullOrEmpty<T>([NotNull] this IGuardClause guardClause, [NotNull] IEnumerable<T> input)
        {
            Guard.Against.Null(input);
            if (!input.Any()) throw new ArgumentException($"Required input {nameof(input)} was empty.", nameof(input));
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentNullException" /> if <paramref name="input" /> is null.
        ///     Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is an empty or white space string.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void NullOrWhiteSpace([NotNull] this IGuardClause guardClause, [NotNull] string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException($"Required input {nameof(input)} was empty.", nameof(input));
        }


        /// <summary>
        ///     Throws an <see cref="ArgumentOutOfRangeException" /> if <see cref="input" /> is less than <see cref="rangeFrom" />
        ///     or greater than <see cref="rangeTo" />.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <param name="rangeFrom"></param>
        /// <param name="rangeTo"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void OutOfRange(this IGuardClause guardClause, decimal input, decimal rangeFrom, decimal rangeTo)
        {
            OutOfRange<decimal>(guardClause, input, rangeFrom, rangeTo);
        }

        private static void OutOfRange<T>(this IGuardClause guardClause, T input, T rangeFrom, T rangeTo)
        {
            var comparer = Comparer<T>.Default;


            if (comparer.Compare(input, rangeFrom) < 0 || comparer.Compare(input, rangeTo) > 0)
                throw new ArgumentOutOfRangeException($"Input {nameof(T)} was out of range");
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is zero.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void Zero([NotNull] this IGuardClause guardClause, decimal input)
        {
            Zero<decimal>(guardClause, input);
        }

        /// <summary>
        ///     Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is zero.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        /// <exception cref="ArgumentException"></exception>
        private static void Zero<T>([NotNull] this IGuardClause guardClause, [NotNull] T input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (EqualityComparer<T>.Default.Equals(input, default))
                throw new ArgumentException($"Required input {nameof(input)} cannot be zero.", nameof(input));
        }


        /// <summary>
        ///     Throws an <see cref="ArgumentException" /> if <paramref name="input" /> is default for that type.
        /// </summary>
        /// <param name="guardClause"></param>
        /// <param name="input"></param>
        //// <exception cref="ArgumentException"></exception>
        public static void Default<T>([NotNull] this IGuardClause guardClause, [NotNull] T input)
        {
            if (EqualityComparer<T>.Default.Equals(input, default))
                throw new ArgumentException($"Parameter [{nameof(input)}] is default value for type {typeof(T).Name}",
                    nameof(input));
        }
    }
}