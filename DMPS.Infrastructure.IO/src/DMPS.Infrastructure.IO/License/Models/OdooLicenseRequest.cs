using System.Text.Json.Serialization;

namespace DMPS.Infrastructure.IO.License.Models;

/// <summary>
/// Represents the data transfer object (DTO) for a license validation request sent to the Odoo API.
/// </summary>
/// <param name="LicenseKey">The license key being validated.</param>
public record OdooLicenseRequest(
    [property: JsonPropertyName("license_key")]
    string LicenseKey
);