using DMPS.CrossCutting.Logging;
using DMPS.CrossCutting.Security;
using DMPS.Data.Access;
using DMPS.Infrastructure.Communication;
using DMPS.Infrastructure.Dicom;
using DMPS.Infrastructure.IO;
using DMPS.Service.Worker.Configuration;
using DMPS.Service.Worker.Handlers;
using DMPS.Service.Worker.Workers;
using DMPS.Shared.Core;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;

// REQ-80: The system shall be designed, developed, and implemented in a manner that complies with HIPAA.
// REQ-81: The entire system shall be developed using the .NET 8 framework and C# 12.
// REQ-69: The system shall be implemented using a Client-Server architecture, consisting of a single, consolidated background Windows Service.
// REQ-71: The background Windows Service shall be the sole host for all asynchronous, long-running tasks.
// REQ-88: The installer must register and start the background Windows Service, which shall be named "DICOM Service".

// Using a static Serilog logger for bootstrap logging in case the host fails to build.
// The fully configured logger will be injected later by the host.
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Debug()
    .CreateBootstrapLogger();

try
{
    var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
    {
        Args = args,
        ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
    });
    
    // === 1. Configure Host Environment and Windows Service integration ===
    // REQ-81: Utilizes Microsoft.Extensions.Hosting for Windows Services
    // REQ-88: Sets the service name to "DICOM Service"
    builder.Services.AddWindowsService(options =>
    {
        options.ServiceName = "DICOM Service";
    });

    // === 2. Configure Logging ===
    // This calls the extension method from the DMPS.CrossCutting.Logging project.
    // REQ-107: The system's logging mechanism, implemented with Serilog, shall write to two destinations: a local rolling text file and the Windows Event Log.
    builder.Services.AddLoggingServices(builder.Configuration);
    Log.Information("Logging services configured.");

    // === 3. Configure Strongly-Typed Options ===
    // Binds sections from appsettings.json to POCO classes for type-safe access.
    // REQ-103: SCP must listen on a configurable port.
    builder.Services.Configure<DicomScpSettings>(builder.Configuration.GetSection(DicomScpSettings.SectionName));
    builder.Services.Configure<MessageQueueSettings>(builder.Configuration.GetSection(MessageQueueSettings.SectionName));
    builder.Services.Configure<DataRetentionSettings>(builder.Configuration.GetSection(DataRetentionSettings.SectionName));
    Log.Information("Configuration options bound.");

    // === 4. Register Services from External Projects (Clean Architecture Layers) ===
    // Each extension method is responsible for registering all public services
    // within its respective project, promoting modularity and separation of concerns.
    builder.Services.AddCoreServices();
    builder.Services.AddDataAccessServices(builder.Configuration);
    builder.Services.AddSecurityServices();
    builder.Services.AddCommunicationServices(builder.Configuration);
    builder.Services.AddDicomServices();
    builder.Services.AddIoServices();
    Log.Information("Services from all external projects registered.");
    
    // === 5. Register Local Message Handlers ===
    // Handlers contain the core business logic for processing a consumed message.
    // They are registered as singletons because they are stateless, and they use
    // IServiceScopeFactory to create scopes for their dependencies (like DbContext).
    builder.Services.AddSingleton<DicomStoreMessageHandler>();
    builder.Services.AddSingleton<PrintJobMessageHandler>();
    builder.Services.AddSingleton<PdfGenerationMessageHandler>();
    Log.Information("Local message handlers registered.");

    // === 6. Register Hosted Services (Workers) ===
    // Each worker is a long-running background task managed by the .NET Generic Host.
    // REQ-71: The background service hosts all asynchronous tasks.
    // REQ-103: The system's background service shall function as a DICOM C-STORE SCP.
    builder.Services.AddHostedService<DicomScpListenerWorker>();

    // Consumers for RabbitMQ queues
    builder.Services.AddHostedService<DicomStoreConsumerWorker>();
    builder.Services.AddHostedService<PrintJobConsumerWorker>();
    builder.Services.AddHostedService<PdfGenerationConsumerWorker>();

    // REQ-75: The system shall use Named Pipes for direct, synchronous, inter-process communication.
    builder.Services.AddHostedService<NamedPipeServerWorker>();
    
    // REQ-86: The system shall implement a configurable data retention policy.
    // REQ-126: The system shall implement a scheduled background task that performs periodic data integrity checks.
    builder.Services.AddHostedService<DataRetentionWorker>();
    Log.Information("All IHostedService workers registered.");

    // === 7. Build and Run the Host ===
    var host = builder.Build();
    
    Log.Information("Host built successfully. Starting DMPS Service Worker...");
    await host.RunAsync();
}
catch (Exception ex)
{
    // This catch block is critical for diagnosing startup failures.
    // If the host fails to build (e.g., due to a DI validation error or configuration issue),
    // this will log the fatal exception.
    Log.Fatal(ex, "DMPS Service Worker failed to start.");
}
finally
{
    // Ensure all logs are flushed before the application exits.
    await Log.CloseAndFlushAsync();
}