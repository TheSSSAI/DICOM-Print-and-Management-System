using System.Text.Json.Serialization;

namespace DMPS.Infrastructure.IO.License.Models;

/// <summary>
/// Represents the data transfer object (DTO) for a license validation response received from the Odoo API.
/// </summary>
public class OdooLicenseResponse
{
    /// <summary>
    /// The status of the license validation.
    /// Expected values might include "valid", "invalid", "expired", etc.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// An optional message from the server providing more context about the validation result.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Checks if the response status indicates a valid license.
    /// </summary>
    [JsonIgnore]
    public bool IsValid => "valid".Equals(Status, StringComparison.OrdinalIgnoreCase);
}