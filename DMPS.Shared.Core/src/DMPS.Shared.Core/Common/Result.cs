// DMPS.Shared.Core/Common/Result.cs
using System.Diagnostics.CodeAnalysis;

namespace DMPS.Shared.Core.Common
{
    /// <summary>
    /// Represents the outcome of an operation.
    /// </summary>
    public class Result
    {
        protected internal Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None)
            {
                throw new InvalidOperationException("A successful result cannot have an error.");
            }
            if (!isSuccess && error == Error.None)
            {
                throw new InvalidOperationException("A failed result must have an error.");
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error Error { get; }

        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error);
        public static Result<T> Success<T>(T value) => new(value, true, Error.None);
        public static Result<T> Failure<T>(Error error) => new(default, false, error);
    }

    /// <summary>
    /// Represents the outcome of an operation that returns a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value returned by the operation.</typeparam>
    public class Result<TValue> : Result
    {
        private readonly TValue? _value;

        protected internal Result(TValue? value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        [NotNull]
        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure result can not be accessed.");

        public static implicit operator Result<TValue>(TValue value) => Success(value);
    }

    /// <summary>
    /// Represents an error with a code and a descriptive message.
    /// </summary>
    public sealed record Error(string Code, string Description)
    {
        public static readonly Error None = new(string.Empty, string.Empty);
        public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");
    }
}