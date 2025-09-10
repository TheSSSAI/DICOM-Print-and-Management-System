using DMPS.Infrastructure.IO.Interfaces;
using DMPS.Infrastructure.IO.License;
using DMPS.Infrastructure.IO.License.Models;
using DMPS.Infrastructure.IO.Pdf;
using DMPS.Infrastructure.IO.Printing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using System.Runtime.Versioning;

namespace DMPS.Infrastructure.IO.Extensions;

/// <summary>
/// Provides extension methods for registering I/O infrastructure services with the IServiceCollection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all necessary services for the DMPS.Infrastructure.IO layer, including
    /// PDF generation, Windows print spooling, and the resilient Odoo license API client.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <param name="configuration">The application's configuration, used to bind settings.</param>
    /// <returns>The same IServiceCollection for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown if services or configuration is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if required configuration sections are missing.</exception>
    public static IServiceCollection AddInfrastructureIOServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Register PDF generation service (QuestPDF wrapper)
        services.AddScoped<IPdfGenerator, PdfGeneratorService>();

        // Register Windows-specific print spooler service
        if (OperatingSystem.IsWindows())
        {
            RegisterWindowsPrintServices(services);
        }

        // Configure strongly-typed settings for the Odoo API
        var odooApiConfigSection = configuration.GetSection(nameof(OdooApiSettings));
        if (!odooApiConfigSection.Exists())
        {
            throw new InvalidOperationException($"Required configuration section '{nameof(OdooApiSettings)}' is missing.");
        }
        services.Configure<OdooApiSettings>(odooApiConfigSection);

        // Configure resilient typed HttpClient for the Odoo License API Client
        services.AddHttpClient<ILicenseApiClient, OdooApiClient>((serviceProvider, client) =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<OdooApiSettings>>().Value;
                if (string.IsNullOrWhiteSpace(settings.BaseUrl) || !Uri.TryCreate(settings.BaseUrl, UriKind.Absolute, out _))
                {
                    throw new InvalidOperationException($"The '{nameof(OdooApiSettings.BaseUrl)}' configuration is missing or invalid.");
                }

                client.BaseAddress = new Uri(settings.BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetTimeoutPolicy());

        return services;
    }

    /// <summary>
    /// Encapsulates the registration of Windows-specific services.
    /// This method is annotated to inform the build analyzer that it contains platform-specific code.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    [SupportedOSPlatform("windows")]
    private static void RegisterWindowsPrintServices(IServiceCollection services)
    {
        services.AddScoped<IPrintSpooler, WindowsPrintService>();
    }

    /// <summary>
    /// Defines the retry policy for transient HTTP errors when communicating with the Odoo API.
    /// It retries up to 3 times with an exponential backoff.
    /// </summary>
    /// <returns>An IAsyncPolicy for HttpResponseMessage.</returns>
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError() // Handles HttpRequestException, 5xx, and 408 status codes
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests) // Handle 429
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff: 2, 4, 8 seconds
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    // Optionally log the retry attempt.
                    // A logger would need to be passed in or resolved if logging is desired here.
                });
    }

    /// <summary>
    /// Defines a timeout policy to prevent requests from hanging indefinitely.
    /// </summary>
    /// <returns>An IAsyncPolicy for HttpResponseMessage.</returns>
    private static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
    {
        // Timeout for each individual try. The retry policy is outside this, so each retry gets its own timeout.
        return Policy.TimeoutAsync<HttpResponseMessage>(15); // 15 seconds timeout
    }
}