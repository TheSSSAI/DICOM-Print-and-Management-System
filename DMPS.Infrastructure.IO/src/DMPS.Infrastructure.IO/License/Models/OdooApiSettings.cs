using System.ComponentModel.DataAnnotations;

namespace DMPS.Infrastructure.IO.License.Models;

/// <summary>
/// Represents the strongly-typed configuration model for connecting to the Odoo Licensing API.
/// This class is designed to be used with the .NET Options Pattern.
/// </summary>
public class OdooApiSettings
{
    /// <summary>
    /// The configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "OdooApi";

    /// <summary>
    /// The base URL of the Odoo Licensing Portal API.
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [Url]
    public required string BaseUrl { get; set; }

    /// <summary>
    /// The key name used to look up the Odoo API bearer token in the secure credential store (e.g., Windows Credential Manager).
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    public required string ApiKeySecretName { get; set; }
}