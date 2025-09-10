using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace DMPS.Data.Access.ValueConverters;

/// <summary>
/// A custom EF Core ValueConverter designed to facilitate database-side encryption and decryption.
/// </summary>
/// <remarks>
/// This converter itself does not perform encryption. Instead, it serves as a strongly-typed container for
/// LINQ expression trees that are provided by a higher-level component (e.g., an IEntityTypeConfiguration).
/// These expressions are designed to be translated by EF Core into calls to database functions,
/// such as PostgreSQL's `pgp_sym_encrypt` and `pgp_sym_decrypt`.
/// This design respects the dependency hierarchy by allowing the low-level converter to remain
/// ignorant of the specific database functions, which are defined and registered at a higher level (in the DbContext).
/// This is a key component for implementing REQ-1-083 (PHI Encryption).
/// </remarks>
public sealed class EncryptedStringConverter : ValueConverter<string, byte[]>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptedStringConverter"/> class.
    /// </summary>
    /// <param name="convertToProviderExpression">
    /// The expression tree to convert a string value from the model to a byte array for the database.
    /// This expression should translate to a call to the database's encryption function (e.g., `pgp_sym_encrypt`).
    /// </param>
    /// <param name="convertFromProviderExpression">
    /// The expression tree to convert a byte array from the database back to a string value for the model.
    /// This expression should translate to a call to the database's decryption function (e.g., `pgp_sym_decrypt`).
    /// </param>
    /// <param name="mappingHints">
    /// Optional hints that can be used by the type mapping infrastructure to create data types with appropriate facets
    /// for the converted data.
    /// </param>
    public EncryptedStringConverter(
        Expression<Func<string, byte[]>> convertToProviderExpression,
        Expression<Func<byte[], string>> convertFromProviderExpression,
        ConverterMappingHints? mappingHints = null)
        : base(convertToProviderExpression, convertFromProviderExpression, mappingHints)
    {
    }
}