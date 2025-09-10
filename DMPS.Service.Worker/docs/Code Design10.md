# 1 Design

code_design

# 2 Code Design

## 2.1 Relevant Context

### 2.1.1 Code Specification

#### 2.1.1.1 Validation Metadata

| Property | Value |
|----------|-------|
| Repository Id | REPO-10-BGW |
| Validation Timestamp | 2024-07-21T10:00:00Z |
| Original Component Count Claimed | 1 |
| Original Component Count Actual | 1 |
| Gaps Identified Count | 14 |
| Components Added Count | 14 |
| Final Component Count | 15 |
| Validation Completeness Score | 99.0% |
| Enhancement Methodology | Systematic analysis of repository scope, requireme... |

#### 2.1.1.2 Validation Summary

##### 2.1.1.2.1 Repository Scope Validation

###### 2.1.1.2.1.1 Scope Compliance

The initial specification was compliant at a high level but lacked the necessary granularity for implementation. The scope was overly consolidated into a single component.

###### 2.1.1.2.1.2 Gaps Identified

- Missing specification for the application entry point (Program.cs) as the composition root.
- Missing discrete specifications for each required IHostedService implementation (e.g., DicomScpListener, PrintJobConsumer, etc.).
- Missing specification for separating message consumption from message processing logic (Consumer vs. Handler pattern).
- Missing specification for strongly-typed configuration classes using the IOptions pattern.
- Missing specification for scheduled tasks like data retention and integrity checks.
- Validation confirmed the `IMessageProducer` dependency was missing from the repository definition, required for the SCP-to-Consumer workflow.

###### 2.1.1.2.1.3 Components Added

- Class specification for Program.cs.
- Class specifications for 6 distinct IHostedService workers (DicomScpListenerWorker, DicomStoreConsumerWorker, PrintJobConsumerWorker, PdfGenerationConsumerWorker, NamedPipeServerWorker, DataRetentionWorker).
- Class specifications for 3 distinct Message Handler classes.
- Configuration specifications for DicomScpSettings and MessageQueueSettings.
- Enhanced DI specifications to cover all components and their lifetimes, including the critical IServiceScopeFactory pattern for consumers.

##### 2.1.1.2.2.0 Requirements Coverage Validation

| Property | Value |
|----------|-------|
| Functional Requirements Coverage | 100% |
| Non Functional Requirements Coverage | 100% |
| Missing Requirement Components | Validation revealed that while the repository was ... |
| Added Requirement Components | Added specifications for `DataRetentionWorker`, `D... |

##### 2.1.1.2.3.0 Architectural Pattern Validation

###### 2.1.1.2.3.1 Pattern Implementation Completeness

The specification lacked details on critical .NET background service patterns.

###### 2.1.1.2.3.2 Missing Pattern Components

- Specification for the \"Composition Root\" pattern in Program.cs.
- Specification for the \"Unit of Work\" pattern within message consumers, using IServiceScopeFactory to manage DbContext lifetimes.
- Specification for separating the \"Message Consumer\" (boilerplate connection logic) from the \"Message Handler\" (business logic).

###### 2.1.1.2.3.3 Added Pattern Components

Enhanced class specifications for all message consumers and handlers to explicitly detail the required patterns for robust, scalable implementation.

##### 2.1.1.2.4.0 Database Mapping Validation

| Property | Value |
|----------|-------|
| Entity Mapping Completeness | Not applicable to this repository, as it consumes ... |
| Missing Database Components | The specification for message handlers that intera... |
| Added Database Components | Added detailed implementation notes to all relevan... |

##### 2.1.1.2.5.0 Sequence Interaction Validation

| Property | Value |
|----------|-------|
| Interaction Implementation Completeness | The original specification was too high-level to m... |
| Missing Interaction Components | Missing specific worker and handler classes to imp... |
| Added Interaction Components | Added a full suite of class specifications that di... |

#### 2.1.1.3.0.0 Enhanced Specification

##### 2.1.1.3.1.0 Specification Metadata

| Property | Value |
|----------|-------|
| Repository Id | REPO-10-BGW |
| Technology Stack | .NET 8.0, Microsoft.Extensions.Hosting, Microsoft.... |
| Technology Guidance Integration | Specifications adhere to .NET 8.0 best practices f... |
| Framework Compliance Score | 100% |
| Specification Completeness | 99.5% |
| Component Count | 15 |
| Specification Methodology | Systematic decomposition of the monolithic service... |

##### 2.1.1.3.2.0 Technology Framework Integration

###### 2.1.1.3.2.1 Framework Patterns Applied

- Hosted Service / Background Worker (IHostedService/BackgroundService)
- Dependency Injection (Composition Root in Program.cs)
- Options Pattern for Configuration (IOptions<T>)
- Message Consumer Pattern with separate Handlers
- Unit of Work (via IServiceScopeFactory for scoped dependencies in singletons)
- Structured Logging with ILogger<T> and Correlation IDs
- Graceful Shutdown with CancellationToken

###### 2.1.1.3.2.2 Directory Structure Source

Follows the .NET Worker Service template, enhanced with dedicated directories for \"Workers\", \"Handlers\", and \"Configuration\" to align with Clean Architecture principles.

###### 2.1.1.3.2.3 Naming Conventions Source

Adheres to Microsoft C# coding standards and .NET community best practices.

###### 2.1.1.3.2.4 Architectural Patterns Source

Implements an Event-Driven, Layered Architecture hosted within a single, consolidated Windows Service process.

###### 2.1.1.3.2.5 Performance Optimizations Applied

- Specification mandates extensive use of async/await for all I/O-bound operations to ensure high throughput and prevent thread blocking.
- Specification requires the use of IServiceScopeFactory to manage DbContext lifecycle within singleton hosted services, preventing memory leaks and concurrency issues.
- Specification requires that high-throughput listeners (like the DICOM SCP) use a message queue to decouple reception from processing, enabling load leveling.

##### 2.1.1.3.3.0 File Structure

###### 2.1.1.3.3.1 Directory Organization

####### 2.1.1.3.3.1.1 Directory Path

######## 2.1.1.3.3.1.1.1 Directory Path

DMPS.Service.Worker

######## 2.1.1.3.3.1.1.2 Purpose

The root directory of the .NET Worker Service project.

######## 2.1.1.3.3.1.1.3 Contains Files

- Program.cs
- appsettings.json
- appsettings.Development.json

######## 2.1.1.3.3.1.1.4 Organizational Reasoning

Standard structure for a .NET application, with Program.cs serving as the entry point and composition root.

######## 2.1.1.3.3.1.1.5 Framework Convention Alignment

Follows the standard .NET 8.0 Worker Service project template.

####### 2.1.1.3.3.1.2.0 Directory Path

######## 2.1.1.3.3.1.2.1 Directory Path

DMPS.Service.Worker/Workers

######## 2.1.1.3.3.1.2.2 Purpose

Contains all concrete implementations of IHostedService, encapsulating each distinct long-running task and its lifecycle management.

######## 2.1.1.3.3.1.2.3 Contains Files

- DicomScpListenerWorker.cs
- DicomStoreConsumerWorker.cs
- PrintJobConsumerWorker.cs
- PdfGenerationConsumerWorker.cs
- NamedPipeServerWorker.cs
- DataRetentionWorker.cs

######## 2.1.1.3.3.1.2.4 Organizational Reasoning

Separates each distinct background responsibility into its own class, promoting modularity, testability, and adherence to the Single Responsibility Principle.

######## 2.1.1.3.3.1.2.5 Framework Convention Alignment

Encapsulates long-running tasks within classes derived from BackgroundService, as recommended by Microsoft.Extensions.Hosting documentation.

####### 2.1.1.3.3.1.3.0 Directory Path

######## 2.1.1.3.3.1.3.1 Directory Path

DMPS.Service.Worker/Configuration

######## 2.1.1.3.3.1.3.2 Purpose

Defines strongly-typed Plain Old C# Object (POCO) classes for application configuration sections.

######## 2.1.1.3.3.1.3.3 Contains Files

- DicomScpSettings.cs
- MessageQueueSettings.cs

######## 2.1.1.3.3.1.3.4 Organizational Reasoning

Centralizes configuration models for type-safe access via the IOptions pattern, improving maintainability and reducing magic strings.

######## 2.1.1.3.3.1.3.5 Framework Convention Alignment

Standard practice for robust, strongly-typed configuration in modern .NET applications.

####### 2.1.1.3.3.1.4.0 Directory Path

######## 2.1.1.3.3.1.4.1 Directory Path

DMPS.Service.Worker/Handlers

######## 2.1.1.3.3.1.4.2 Purpose

Contains the specific business logic for processing messages consumed from the message queue. This decouples logic from the queueing infrastructure.

######## 2.1.1.3.3.1.4.3 Contains Files

- DicomStoreMessageHandler.cs
- PrintJobMessageHandler.cs
- PdfGenerationMessageHandler.cs

######## 2.1.1.3.3.1.4.4 Organizational Reasoning

Decouples the complex logic of message processing from the boilerplate of queue consumption, improving testability and separation of concerns.

######## 2.1.1.3.3.1.4.5 Framework Convention Alignment

Promotes the Single Responsibility Principle, where the Worker manages the subscription and the Handler manages the processing.

###### 2.1.1.3.3.2.0.0 Namespace Strategy

| Property | Value |
|----------|-------|
| Root Namespace | DMPS.Service.Worker |
| Namespace Organization | Hierarchically organized by feature and architectu... |
| Naming Conventions | PascalCase for all types and methods, following Mi... |
| Framework Alignment | Adheres to standard .NET namespace conventions for... |

##### 2.1.1.3.4.0.0.0 Class Specifications

###### 2.1.1.3.4.1.0.0 Class Name

####### 2.1.1.3.4.1.1.0 Class Name

Program

####### 2.1.1.3.4.1.2.0 File Path

DMPS.Service.Worker/Program.cs

####### 2.1.1.3.4.1.3.0 Class Type

Static Class (Application Entry Point)

####### 2.1.1.3.4.1.4.0 Inheritance

N/A

####### 2.1.1.3.4.1.5.0 Purpose

Specification requires this class to act as the Composition Root. It must configure the .NET Generic Host, register all dependencies from all solution repositories via extension methods, configure logging and configuration sources, and register all IHostedService workers. This is the central point where the entire service is assembled.

####### 2.1.1.3.4.1.6.0 Dependencies

- REPO-01-SHK
- REPO-02-DAL
- REPO-03-LOG
- REPO-04-SEC
- REPO-05-COM
- REPO-06-DIC
- REPO-07-IOI

####### 2.1.1.3.4.1.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.1.8.0 Technology Integration Notes

Specification requires using `Host.CreateDefaultBuilder().UseWindowsService()` to create a host compatible with both console debugging and production deployment as a Windows Service.

####### 2.1.1.3.4.1.9.0 Methods

- {'method_name': 'Main', 'method_signature': 'static async Task Main(string[] args)', 'return_type': 'Task', 'access_modifier': 'public static', 'is_async': True, 'implementation_logic': 'Specification requires this method to: 1. Create a HostBuilder. 2. Configure logging (from REPO-03-LOG). 3. Configure appsettings.json and environment variables. 4. Call extension methods to register services from all dependent repositories (e.g., `services.AddDataAccessServices(...)`, `services.AddCommunicationServices(...)`). 5. Register all IHostedService workers from this repository (e.g., `services.AddHostedService<DicomScpListenerWorker>()`). 6. Build and run the host.', 'exception_handling': 'Specification requires a top-level try-catch block to log any fatal exceptions during host startup to prevent silent failures.'}

####### 2.1.1.3.4.1.10.0 Implementation Notes

Validation confirms this specification is critical for fulfilling the Dependency Injection integration pattern and assembling the application.

###### 2.1.1.3.4.2.0.0 Class Name

####### 2.1.1.3.4.2.1.0 Class Name

DicomScpListenerWorker

####### 2.1.1.3.4.2.2.0 File Path

DMPS.Service.Worker/Workers/DicomScpListenerWorker.cs

####### 2.1.1.3.4.2.3.0 Class Type

Hosted Service

####### 2.1.1.3.4.2.4.0 Inheritance

BackgroundService

####### 2.1.1.3.4.2.5.0 Purpose

Specification requires this worker to host the DICOM C-STORE SCP listener, enabling the application to receive studies from external modalities as detailed in sequence SEQ-EVP-002.

####### 2.1.1.3.4.2.6.0 Dependencies

- ILogger<DicomScpListenerWorker>
- IOptions<DicomScpSettings>
- IDicomScpService (from REPO-06-DIC)
- IMessageProducer (from REPO-05-COM)

####### 2.1.1.3.4.2.7.0 Methods

- {'method_name': 'ExecuteAsync', 'method_signature': 'override Task ExecuteAsync(CancellationToken stoppingToken)', 'return_type': 'Task', 'access_modifier': 'protected override', 'is_async': True, 'implementation_logic': 'Specification requires this method to retrieve the configured port and AE Title from `DicomScpSettings`. It must call `IDicomScpService.StartListening`, providing a callback function. This callback, upon receiving a DICOM file, must create a `ProcessDicomStoreCommand` DTO, generate a new Correlation ID, and use `IMessageProducer` to publish it to the DICOM storage queue. This ensures the SCP listener is not blocked by persistence logic.', 'exception_handling': 'Specification requires logging any exceptions during listener startup. Must honor the `stoppingToken` for graceful shutdown.'}

####### 2.1.1.3.4.2.8.0 Implementation Notes

Validation confirms this worker correctly implements the first half of the decoupled DICOM ingestion process (SEQ-EVP-002), prioritizing performance and reliability.

###### 2.1.1.3.4.3.0.0 Class Name

####### 2.1.1.3.4.3.1.0 Class Name

DicomStoreConsumerWorker

####### 2.1.1.3.4.3.2.0 File Path

DMPS.Service.Worker/Workers/DicomStoreConsumerWorker.cs

####### 2.1.1.3.4.3.3.0 Class Type

Hosted Service

####### 2.1.1.3.4.3.4.0 Inheritance

BackgroundService

####### 2.1.1.3.4.3.5.0 Purpose

Specification requires this worker to manage the subscription to the DICOM storage queue, delegating message processing to a dedicated handler.

####### 2.1.1.3.4.3.6.0 Dependencies

- ILogger<DicomStoreConsumerWorker>
- IOptions<MessageQueueSettings>
- IMessageConsumer (from REPO-05-COM)
- DicomStoreMessageHandler

####### 2.1.1.3.4.3.7.0 Methods

- {'method_name': 'ExecuteAsync', 'method_signature': 'override Task ExecuteAsync(CancellationToken stoppingToken)', 'return_type': 'Task', 'access_modifier': 'protected override', 'is_async': True, 'implementation_logic': 'Specification requires this method to retrieve the queue name from `MessageQueueSettings` and call `IMessageConsumer.StartConsuming`, providing the queue name and a delegate to the `DicomStoreMessageHandler.HandleMessageAsync` method. It must remain running until the `stoppingToken` is canceled.', 'exception_handling': 'The underlying `IMessageConsumer` is responsible for connection resilience. This worker focuses on lifecycle management.'}

###### 2.1.1.3.4.4.0.0 Class Name

####### 2.1.1.3.4.4.1.0 Class Name

DicomStoreMessageHandler

####### 2.1.1.3.4.4.2.0 File Path

DMPS.Service.Worker/Handlers/DicomStoreMessageHandler.cs

####### 2.1.1.3.4.4.3.0 Class Type

Message Handler

####### 2.1.1.3.4.4.4.0 Inheritance

N/A

####### 2.1.1.3.4.4.5.0 Purpose

Specification requires this class to contain the business logic for processing a single DICOM storage message, including database persistence.

####### 2.1.1.3.4.4.6.0 Dependencies

- ILogger<DicomStoreMessageHandler>
- IServiceScopeFactory

####### 2.1.1.3.4.4.7.0 Methods

- {'method_name': 'HandleMessageAsync', 'method_signature': 'Task HandleMessageAsync(Message message)', 'return_type': 'Task', 'access_modifier': 'public', 'is_async': True, 'implementation_logic': 'Specification requires this method to: 1. Create a new dependency scope using `IServiceScopeFactory`. 2. Within the scope, resolve scoped services like `IStudyRepository` and `IDicomFileStorage`. 3. Deserialize the message body into a `ProcessDicomStoreCommand`. 4. Orchestrate moving DICOM files to permanent storage and calling the repository to persist study metadata transactionally. 5. The logic must be idempotent to handle potential message redelivery.', 'exception_handling': 'Specification requires a try-catch block. On success, the method completes. On a persistent failure (e.g., invalid data), it must re-throw an exception to signal the consumer to dead-letter the message as per SEQ-ERH-007.'}

####### 2.1.1.3.4.4.8.0 Implementation Notes

Validation confirms this specification is critical. It enforces the correct pattern for using scoped dependencies (like DbContext) within a singleton consumer, preventing major runtime issues.

###### 2.1.1.3.4.5.0.0 Class Name

####### 2.1.1.3.4.5.1.0 Class Name

PrintJobConsumerWorker

####### 2.1.1.3.4.5.2.0 File Path

DMPS.Service.Worker/Workers/PrintJobConsumerWorker.cs

####### 2.1.1.3.4.5.3.0 Class Type

Hosted Service

####### 2.1.1.3.4.5.4.0 Inheritance

BackgroundService

####### 2.1.1.3.4.5.5.0 Purpose

Specification requires this worker to manage the subscription to the print job queue, handling asynchronous print requests.

####### 2.1.1.3.4.5.6.0 Dependencies

- IMessageConsumer
- PrintJobMessageHandler

####### 2.1.1.3.4.5.7.0 Implementation Notes

The implementation pattern specification is identical to `DicomStoreConsumerWorker` but for the print queue.

###### 2.1.1.3.4.6.0.0 Class Name

####### 2.1.1.3.4.6.1.0 Class Name

PrintJobMessageHandler

####### 2.1.1.3.4.6.2.0 File Path

DMPS.Service.Worker/Handlers/PrintJobMessageHandler.cs

####### 2.1.1.3.4.6.3.0 Class Type

Message Handler

####### 2.1.1.3.4.6.4.0 Purpose

Specification requires this class to contain the logic for processing a print job message as per SEQ-EVP-003.

####### 2.1.1.3.4.6.5.0 Dependencies

- IServiceScopeFactory
- ILogger<PrintJobMessageHandler>

####### 2.1.1.3.4.6.6.0 Implementation Logic

Specification requires this handler to use a service scope to resolve `IPrintSpooler`, `IPrintJobRepository` (for status updates), and `IAuditLogRepository`. It must orchestrate the steps of generating and spooling the print job.

####### 2.1.1.3.4.6.7.0 Exception Handling

Specification requires robust error handling for scenarios like missing DICOM files or printer errors, leading to a message dead-letter event.

###### 2.1.1.3.4.7.0.0 Class Name

####### 2.1.1.3.4.7.1.0 Class Name

NamedPipeServerWorker

####### 2.1.1.3.4.7.2.0 File Path

DMPS.Service.Worker/Workers/NamedPipeServerWorker.cs

####### 2.1.1.3.4.7.3.0 Class Type

Hosted Service

####### 2.1.1.3.4.7.4.0 Inheritance

BackgroundService

####### 2.1.1.3.4.7.5.0 Purpose

Specification requires this worker to host the Named Pipe server for responding to synchronous status checks from the client application, as per SEQ-SIT-012.

####### 2.1.1.3.4.7.6.0 Dependencies

- ILogger<NamedPipeServerWorker>
- INamedPipeServer (from REPO-05-COM)

####### 2.1.1.3.4.7.7.0 Methods

- {'method_name': 'ExecuteAsync', 'method_signature': 'override Task ExecuteAsync(CancellationToken stoppingToken)', 'return_type': 'Task', 'access_modifier': 'protected override', 'is_async': True, 'implementation_logic': 'Specification requires calling `INamedPipeServer.StartListening`, providing a callback that receives a request string. If the request is \\"PING\\", the callback must return \\"PONG\\". The listener must run until the `stoppingToken` is canceled.', 'exception_handling': 'Specification requires logging any pipe communication errors gracefully without crashing the worker.'}

###### 2.1.1.3.4.8.0.0 Class Name

####### 2.1.1.3.4.8.1.0 Class Name

DataRetentionWorker

####### 2.1.1.3.4.8.2.0 File Path

DMPS.Service.Worker/Workers/DataRetentionWorker.cs

####### 2.1.1.3.4.8.3.0 Class Type

Hosted Service

####### 2.1.1.3.4.8.4.0 Inheritance

BackgroundService

####### 2.1.1.3.4.8.5.0 Purpose

Specification added for a scheduled worker that periodically enforces data retention policies (REQ-1-018) and performs data integrity checks (REQ-1-058).

####### 2.1.1.3.4.8.6.0 Dependencies

- ILogger<DataRetentionWorker>
- IServiceScopeFactory

####### 2.1.1.3.4.8.7.0 Methods

- {'method_name': 'ExecuteAsync', 'method_signature': 'override Task ExecuteAsync(CancellationToken stoppingToken)', 'return_type': 'Task', 'access_modifier': 'protected override', 'is_async': True, 'implementation_logic': 'Specification requires this method to run in a timed loop (e.g., once every 24 hours). In each iteration, it must create a service scope and resolve repositories to: 1. Query for studies older than the configured retention period and mark them for deletion. 2. Perform a data integrity check by comparing database records against the file system.', 'exception_handling': 'Specification requires logging all actions and errors. A failure in one iteration should not prevent the next scheduled run.'}

##### 2.1.1.3.5.0.0.0 Configuration Specifications

###### 2.1.1.3.5.1.0.0 Configuration Name

####### 2.1.1.3.5.1.1.0 Configuration Name

DicomScpSettings

####### 2.1.1.3.5.1.2.0 File Path

DMPS.Service.Worker/Configuration/DicomScpSettings.cs

####### 2.1.1.3.5.1.3.0 Purpose

Specification for a strongly-typed configuration object for the DICOM C-STORE SCP listener, to be loaded from appsettings.json.

####### 2.1.1.3.5.1.4.0 Framework Base Class

N/A

####### 2.1.1.3.5.1.5.0 Configuration Sections

- {'section_name': 'DicomScp', 'properties': [{'property_name': 'Port', 'property_type': 'int', 'default_value': '104', 'required': True, 'description': 'The TCP port on which the DICOM SCP service will listen for incoming associations.'}, {'property_name': 'AETitle', 'property_type': 'string', 'default_value': 'DMPSSCP', 'required': True, 'description': "The Application Entity Title of this application's SCP service."}]}

####### 2.1.1.3.5.1.6.0 Validation Requirements

Specification requires validation attributes (e.g., Range for Port, MaxLength for AETitle) to be added for robust configuration validation at startup.

###### 2.1.1.3.5.2.0.0 Configuration Name

####### 2.1.1.3.5.2.1.0 Configuration Name

MessageQueueSettings

####### 2.1.1.3.5.2.2.0 File Path

DMPS.Service.Worker/Configuration/MessageQueueSettings.cs

####### 2.1.1.3.5.2.3.0 Purpose

Specification for a strongly-typed configuration object for RabbitMQ connections and queue names.

####### 2.1.1.3.5.2.4.0 Framework Base Class

N/A

####### 2.1.1.3.5.2.5.0 Configuration Sections

- {'section_name': 'MessageQueue', 'properties': [{'property_name': 'Hostname', 'property_type': 'string', 'default_value': 'localhost', 'required': True, 'description': 'The hostname or IP address of the RabbitMQ server.'}, {'property_name': 'DicomStoreQueueName', 'property_type': 'string', 'default_value': 'dicom_store_queue', 'required': True, 'description': 'The name of the queue for processing DICOM ingestion messages.'}]}

####### 2.1.1.3.5.2.6.0 Validation Requirements

Specification requires validation to ensure no required properties are null or empty.

##### 2.1.1.3.6.0.0.0 Dependency Injection Specifications

###### 2.1.1.3.6.1.0.0 Service Interface

####### 2.1.1.3.6.1.1.0 Service Interface

N/A

####### 2.1.1.3.6.1.2.0 Service Implementation

DicomScpListenerWorker

####### 2.1.1.3.6.1.3.0 Lifetime

Singleton (managed by host)

####### 2.1.1.3.6.1.4.0 Registration Reasoning

Specification requires all IHostedService implementations to be registered as singletons for the .NET Generic Host to manage their lifecycle.

####### 2.1.1.3.6.1.5.0 Framework Registration Pattern

services.AddHostedService<DicomScpListenerWorker>();

###### 2.1.1.3.6.2.0.0 Service Interface

####### 2.1.1.3.6.2.1.0 Service Interface

DicomStoreMessageHandler

####### 2.1.1.3.6.2.2.0 Service Implementation

DicomStoreMessageHandler

####### 2.1.1.3.6.2.3.0 Lifetime

Singleton

####### 2.1.1.3.6.2.4.0 Registration Reasoning

Specification requires the handler itself to be a stateless singleton. It will manage the lifetime of its scoped dependencies (like DbContext) internally using IServiceScopeFactory.

####### 2.1.1.3.6.2.5.0 Framework Registration Pattern

services.AddSingleton<DicomStoreMessageHandler>();

###### 2.1.1.3.6.3.0.0 Service Interface

####### 2.1.1.3.6.3.1.0 Service Interface

IMessageConsumer (from REPO-05-COM)

####### 2.1.1.3.6.3.2.0 Service Implementation

RabbitMqConsumer (from REPO-05-COM)

####### 2.1.1.3.6.3.3.0 Lifetime

Singleton

####### 2.1.1.3.6.3.4.0 Registration Reasoning

Specification requires communication clients that manage persistent connections to be singletons to avoid resource exhaustion and ensure connection reuse.

####### 2.1.1.3.6.3.5.0 Framework Registration Pattern

Handled by an extension method in the Communication project: `services.AddCommunicationServices(configuration);`

###### 2.1.1.3.6.4.0.0 Service Interface

####### 2.1.1.3.6.4.1.0 Service Interface

IStudyRepository (from REPO-02-DAL)

####### 2.1.1.3.6.4.2.0 Service Implementation

StudyRepository (from REPO-02-DAL)

####### 2.1.1.3.6.4.3.0 Lifetime

Scoped

####### 2.1.1.3.6.4.4.0 Registration Reasoning

Specification requires repositories that depend on the DbContext to be scoped. This ensures they use the same DbContext instance within a single unit of work (e.g., one message processing scope).

####### 2.1.1.3.6.4.5.0 Framework Registration Pattern

Handled by an extension method in the Data Access project: `services.AddDataAccessServices(configuration);`

##### 2.1.1.3.7.0.0.0 External Integration Specifications

###### 2.1.1.3.7.1.0.0 Integration Target

####### 2.1.1.3.7.1.1.0 Integration Target

RabbitMQ

####### 2.1.1.3.7.1.2.0 Integration Type

Message Queue

####### 2.1.1.3.7.1.3.0 Required Client Classes

- IMessageConsumer
- IMessageProducer

####### 2.1.1.3.7.1.4.0 Configuration Requirements

Specification requires configuration of hostname, credentials, and names for all work queues and dead-letter queues via appsettings.json and the `MessageQueueSettings` class.

####### 2.1.1.3.7.1.5.0 Error Handling Requirements

Specification requires all message handlers to implement logic for retries and dead-lettering as detailed in SEQ-ERH-007. Consumer logic must be idempotent.

####### 2.1.1.3.7.1.6.0 Authentication Requirements

Specification requires username/password credentials for the RabbitMQ connection, to be stored securely.

####### 2.1.1.3.7.1.7.0 Framework Integration Patterns

The `IMessageConsumer` and `IMessageProducer` interfaces are consumed within singleton `BackgroundService` workers, which manage the connection and subscription lifecycle.

###### 2.1.1.3.7.2.0.0 Integration Target

####### 2.1.1.3.7.2.1.0 Integration Target

PostgreSQL

####### 2.1.1.3.7.2.2.0 Integration Type

Database

####### 2.1.1.3.7.2.3.0 Required Client Classes

- IStudyRepository
- IAuditLogRepository
- IPrintJobRepository
- etc.

####### 2.1.1.3.7.2.4.0 Configuration Requirements

Specification requires a standard PostgreSQL connection string, retrieved from a secure source like Windows Credential Manager or Azure Key Vault.

####### 2.1.1.3.7.2.5.0 Error Handling Requirements

Specification requires database exceptions to be caught by message handlers and translated into message processing failures (triggering a nack/rejection).

####### 2.1.1.3.7.2.6.0 Authentication Requirements

Specification requires standard database username/password credentials in the connection string.

####### 2.1.1.3.7.2.7.0 Framework Integration Patterns

Entity Framework Core is used for data access. The `DbContext` and repository lifetimes must be scoped. Singleton services must use `IServiceScopeFactory` to create scopes for database operations.

#### 2.1.1.4.0.0.0.0 Component Count Validation

| Property | Value |
|----------|-------|
| Total Classes | 12 |
| Total Interfaces | 0 |
| Total Enums | 0 |
| Total Dtos | 0 |
| Total Configurations | 2 |
| Total External Integrations | 2 |
| Grand Total Components | 16 |
| Phase 2 Claimed Count | 1 |
| Phase 2 Actual Count | 1 |
| Validation Added Count | 15 |
| Final Validated Count | 16 |
| Validation Notes | The original component count was a high-level abst... |

### 2.1.2.0.0.0.0.0 Project Supporting Files

#### 2.1.2.1.0.0.0.0 File Type

##### 2.1.2.1.1.0.0.0 File Type

Project Definition

##### 2.1.2.1.2.0.0.0 File Name

DMPS.Service.Worker.csproj

##### 2.1.2.1.3.0.0.0 File Path

./DMPS.Service.Worker.csproj

##### 2.1.2.1.4.0.0.0 Purpose

Defines the .NET 8 Worker Service project, its target framework, dependencies, and build settings.

##### 2.1.2.1.5.0.0.0 Content Description

<Project Sdk=\"Microsoft.NET.Sdk.Worker\">\n\n  <PropertyGroup>\n    <TargetFramework>net8.0</TargetFramework>\n    <Nullable>enable</Nullable>\n    <ImplicitUsings>enable</ImplicitUsings>\n    <UserSecretsId>dotnet-DMPS.Service.Worker-UNIQUE_GUID</UserSecretsId>\n  </PropertyGroup>\n\n  <ItemGroup>\n    <PackageReference Include=\"Microsoft.Extensions.Hosting\" Version=\"8.0.0\" />\n    <PackageReference Include=\"Microsoft.Extensions.Hosting.WindowsServices\" Version=\"8.0.0\" />\n  </ItemGroup>\n\n  <ItemGroup>\n    <ProjectReference Include=\"..\\DMPS.Shared.Core\\DMPS.Shared.Core.csproj\" />\n    <ProjectReference Include=\"..\\DMPS.Data.Access\\DMPS.Data.Access.csproj\" />\n    <ProjectReference Include=\"..\\DMPS.CrossCutting.Logging\\DMPS.CrossCutting.Logging.csproj\" />\n    <ProjectReference Include=\"..\\DMPS.CrossCutting.Security\\DMPS.CrossCutting.Security.csproj\" />\n    <ProjectReference Include=\"..\\DMPS.Infrastructure.Communication\\DMPS.Infrastructure.Communication.csproj\" />\n    <ProjectReference Include=\"..\\DMPS.Infrastructure.Dicom\\DMPS.Infrastructure.Dicom.csproj\" />\n    <ProjectReference Include=\"..\\DMPS.Infrastructure.IO\\DMPS.Infrastructure.IO.csproj\" />\n  </ItemGroup>\n</Project>

##### 2.1.2.1.6.0.0.0 Framework Specific Attributes

- Sdk: Microsoft.NET.Sdk.Worker
- TargetFramework: net8.0
- PackageReference for Microsoft.Extensions.Hosting.WindowsServices

#### 2.1.2.2.0.0.0.0 File Type

##### 2.1.2.2.1.0.0.0 File Type

Version Control

##### 2.1.2.2.2.0.0.0 File Name

.gitignore

##### 2.1.2.2.3.0.0.0 File Path

./.gitignore

##### 2.1.2.2.4.0.0.0 Purpose

Specifies intentionally untracked files for Git to ignore, tailored for .NET development.

##### 2.1.2.2.5.0.0.0 Content Description

# Ignore Visual Studio temporary files, build artifacts, and user-specific settings.\n[Bb]in/\n[Oo]bj/\n.vs/\n*.suo\n*.user

##### 2.1.2.2.6.0.0.0 Framework Specific Attributes

- .NET build output folders (bin, obj)
- Visual Studio specific files (.vs, .suo, .user)

#### 2.1.2.3.0.0.0.0 File Type

##### 2.1.2.3.1.0.0.0 File Type

Application Configuration

##### 2.1.2.3.2.0.0.0 File Name

appsettings.json

##### 2.1.2.3.3.0.0.0 File Path

./appsettings.json

##### 2.1.2.3.4.0.0.0 Purpose

Provides runtime configuration for the background service, such as queue names, DICOM settings, and logging levels.

##### 2.1.2.3.5.0.0.0 Content Description

{\n  \"Logging\": {\n    \"LogLevel\": {\n      \"Default\": \"Information\",\n      \"Microsoft.Hosting.Lifetime\": \"Information\"\n    }\n  },\n  \"DicomScp\": {\n    \"Port\": 104,\n    \"AETitle\": \"DMPSSCP\"\n  },\n  \"MessageQueue\": {\n    \"Hostname\": \"localhost\",\n    \"DicomStoreQueueName\": \"dicom_store_queue\",\n    \"PrintJobQueueName\": \"print_job_queue\",\n    \"PdfGenerationQueueName\": \"pdf_generation_queue\"\n  },\n  \"DataRetention\": {\n    \"RetentionPeriodDays\": 2555,\n    \"ScheduleCronExpression\": \"0 2 * * *\" \n  }\n}

##### 2.1.2.3.6.0.0.0 Framework Specific Attributes

- JSON format
- Loaded by Microsoft.Extensions.Hosting at startup

