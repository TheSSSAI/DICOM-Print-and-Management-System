// DMPS.Shared.Core/Domain/Primitives/ValueObject.cs
using System.Reflection;

namespace DMPS.Shared.Core.Domain.Primitives
{
    /// <summary>
    /// A base class for creating value objects, which are objects defined by their attributes rather than a unique identity.
    /// This class provides value-based equality comparison by comparing all public properties of the object.
    /// </summary>
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        /// <summary>
        /// Gets the properties of the value object that contribute to its equality.
        /// </summary>
        /// <returns>An enumerable of the object's properties.</returns>
        protected abstract IEnumerable<object> GetEqualityComponents();

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((ValueObject)obj);
        }

        /// <summary>
        /// Determines whether the specified value object is equal to the current value object.
        /// </summary>
        /// <param name="other">The value object to compare with the current object.</param>
        /// <returns>true if the specified value object is equal to the current value object; otherwise, false.</returns>
        public bool Equals(ValueObject? other)
        {
            if (other is null)
            {
                return false;
            }

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate((x, y) => x ^ y);
        }

        /// <summary>
        /// Determines whether two value objects are equal.
        /// </summary>
        /// <param name="left">The first value object to compare.</param>
        /// <param name="right">The second value object to compare.</param>
        /// <returns>true if the value objects are equal; otherwise, false.</returns>
        public static bool operator ==(ValueObject? left, ValueObject? right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
                return true;

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two value objects are not equal.
        /// </summary>
        /// <param name="left">The first value object to compare.</param>
        /// <param name="right">The second value object to compare.</param>
        /// <returns>true if the value objects are not equal; otherwise, false.</returns>
        public static bool operator !=(ValueObject? left, ValueObject? right)
        {
            return !(left == right);
        }
    }
}