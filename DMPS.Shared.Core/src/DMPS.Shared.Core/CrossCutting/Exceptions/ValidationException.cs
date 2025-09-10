using System.Text;

namespace DMPS.Shared.Core.CrossCutting.Exceptions;

/// <summary>
/// Represents an exception that occurs during application-level validation.
/// This exception holds a collection of validation errors.
/// </summary>
public sealed class ValidationException : Exception
{
    /// <summary>
    /// Gets the collection of validation errors, where the key is the property name and the value is an array of error messages for that property.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a default message and an empty error collection.
    /// </summary>
    public ValidationException()
        : base("One or more validation errors occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a collection of validation failures.
    /// </summary>
    /// <param name="failures">A dictionary containing validation failures.</param>
    public ValidationException(IReadOnlyDictionary<string, string[]> failures)
        : this()
    {
        Errors = failures;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a single validation failure.
    /// </summary>
    /// <param name="propertyName">The name of the property that failed validation.</param>
    /// <param name="errorMessage">The error message for the validation failure.</param>
    public ValidationException(string propertyName, string errorMessage) : this()
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, [errorMessage] }
        };
    }
    
    /// <summary>
    /// Creates and returns a string representation of the current exception.
    /// </summary>
    /// <returns>A string representation of the exception.</returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(base.ToString());
        sb.AppendLine("Validation Errors:");

        if (Errors.Count == 0)
        {
            sb.AppendLine("  (No specific errors provided)");
            return sb.ToString();
        }

        foreach (var error in Errors)
        {
            sb.AppendLine($"  - Property: {error.Key}");
            foreach (var message in error.Value)
            {
                sb.AppendLine($"    - {message}");
            }
        }

        return sb.ToString();
    }
}