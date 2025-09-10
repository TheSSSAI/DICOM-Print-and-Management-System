// DMPS.Shared.Core/Common/Guard.cs
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace DMPS.Shared.Core.Common
{
    /// <summary>
    /// A static helper class that provides methods for checking method preconditions (guard clauses).
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the input is null.
        /// </summary>
        /// <param name="input">The object to check.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="ArgumentNullException">Thrown if input is null.</exception>
        public static void AgainstNull([NotNull] object? input, [CallerArgumentExpression("input")] string? parameterName = null)
        {
            if (input is null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the input string is null or consists only of white-space characters.
        /// </summary>
        /// <param name="input">The string to check.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="ArgumentException">Thrown if input is null or white-space.</exception>
        public static void AgainstNullOrWhiteSpace([NotNull] string? input, [CallerArgumentExpression("input")] string? parameterName = null)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Parameter cannot be null or consist of only white-space characters.", parameterName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the input string is null or empty.
        /// </summary>
        /// <param name="input">The string to check.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="ArgumentException">Thrown if input is null or empty.</exception>
        public static void AgainstNullOrEmpty([NotNull] string? input, [CallerArgumentExpression("input")] string? parameterName = null)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Parameter cannot be null or empty.", parameterName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if the input is outside the specified range.
        /// </summary>
        /// <typeparam name="T">The type of the input, which must be comparable.</typeparam>
        /// <param name="input">The value to check.</param>
        /// <param name="rangeFrom">The lower bound of the allowed range.</param>
        /// <param name="rangeTo">The upper bound of the allowed range.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if input is not within the specified range.</exception>
        public static void AgainstOutOfRange<T>(T input, T rangeFrom, T rangeTo, [CallerArgumentExpression("input")] string? parameterName = null) where T : IComparable<T>
        {
            if (input.CompareTo(rangeFrom) < 0 || input.CompareTo(rangeTo) > 0)
            {
                throw new ArgumentOutOfRangeException(parameterName, $"Input was out of the allowed range [{rangeFrom} - {rangeTo}].");
            }
        }
        
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the input Guid is empty.
        /// </summary>
        /// <param name="input">The Guid to check.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="ArgumentException">Thrown if the Guid is empty.</exception>
        public static void AgainstEmptyGuid(Guid input, [CallerArgumentExpression("input")] string? parameterName = null)
        {
            if (input == Guid.Empty)
            {
                throw new ArgumentException("Parameter cannot be an empty GUID.", parameterName);
            }
        }
    }
}