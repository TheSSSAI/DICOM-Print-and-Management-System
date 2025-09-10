# 1 Design

code_design

# 2 Code Design

## 2.1 Relevant Context

### 2.1.1 Code Specification

#### 2.1.1.1 Validation Metadata

| Property | Value |
|----------|-------|
| Repository Id | REPO-03-LOG |
| Validation Timestamp | 2024-07-20T10:00:01Z |
| Original Component Count Claimed | 1 |
| Original Component Count Actual | 1 |
| Gaps Identified Count | 4 |
| Components Added Count | 4 |
| Final Component Count | 5 |
| Validation Completeness Score | 100.0 |
| Enhancement Methodology | Systematic validation of the repository's responsi... |

#### 2.1.1.2 Validation Summary

##### 2.1.1.2.1 Repository Scope Validation

###### 2.1.1.2.1.1 Scope Compliance

Fully compliant. The enhanced specification details all components required by the repository's defined scope, including configuration, custom enrichers for PHI masking and correlation IDs, and context management.

###### 2.1.1.2.1.2 Gaps Identified

- Initial specification was a single high-level component, lacking detail.
- Missing explicit specification for the PHI Masking Enricher.
- Missing explicit specification for the Correlation ID Enricher and its context management.
- Missing specification for fluent configuration extension methods.

###### 2.1.1.2.1.3 Components Added

- PhiMaskingEnricher
- CorrelationIdEnricher
- CorrelationContext
- LoggerConfigurationExtensions

##### 2.1.1.2.2.0 Requirements Coverage Validation

###### 2.1.1.2.2.1 Functional Requirements Coverage

100.0%

###### 2.1.1.2.2.2 Non Functional Requirements Coverage

100.0%

###### 2.1.1.2.2.3 Missing Requirement Components

*No items available*

###### 2.1.1.2.2.4 Added Requirement Components

- Specification now explicitly maps REQ-1-039 to the PhiMaskingEnricher and sink configurations.
- Specification now explicitly maps REQ-1-090 to the CorrelationIdEnricher and CorrelationContext.

##### 2.1.1.2.3.0 Architectural Pattern Validation

###### 2.1.1.2.3.1 Pattern Implementation Completeness

The specification now fully details the implementation of the Enricher pattern, Static Configuration pattern, and DI integration, aligning perfectly with the Cross-Cutting Concerns layer.

###### 2.1.1.2.3.2 Missing Pattern Components

*No items available*

###### 2.1.1.2.3.3 Added Pattern Components

- Added detailed specifications for classes implementing the Serilog ILogEventEnricher interface.
- Added specification for a static extension class to provide a fluent configuration API.

##### 2.1.1.2.4.0 Database Mapping Validation

###### 2.1.1.2.4.1 Entity Mapping Completeness

Not applicable. This repository has no direct database interaction.

###### 2.1.1.2.4.2 Missing Database Components

*No items available*

###### 2.1.1.2.4.3 Added Database Components

*No items available*

##### 2.1.1.2.5.0 Sequence Interaction Validation

###### 2.1.1.2.5.1 Interaction Implementation Completeness

The specification details the primary interaction: the configuration sequence at application startup. It also defines the contract for runtime logging calls.

###### 2.1.1.2.5.2 Missing Interaction Components

*No items available*

###### 2.1.1.2.5.3 Added Interaction Components

- Added detailed specifications for method logic, error handling, and performance considerations for all public and internal methods.

#### 2.1.1.3.0.0 Enhanced Specification

##### 2.1.1.3.1.0 Specification Metadata

| Property | Value |
|----------|-------|
| Repository Id | REPO-03-LOG |
| Technology Stack | .NET 8.0, C# 12, Serilog v3.1.1 |
| Technology Guidance Integration | Specification fully aligns with Serilog best pract... |
| Framework Compliance Score | 100.0 |
| Specification Completeness | 100.0 |
| Component Count | 5 |
| Specification Methodology | Centralized static configuration with custom, inje... |

##### 2.1.1.3.2.0 Technology Framework Integration

###### 2.1.1.3.2.1 Framework Patterns Applied

- Static Configuration Class
- Enricher Pattern (Serilog)
- Dependency Injection Integration
- Options Pattern (for enricher configuration)

###### 2.1.1.3.2.2 Directory Structure Source

.NET 8.0-optimized structure for a cross-cutting concerns library, with specific folders for Serilog component types.

###### 2.1.1.3.2.3 Naming Conventions Source

Microsoft C# coding standards and Serilog community conventions.

###### 2.1.1.3.2.4 Architectural Patterns Source

Cross-cutting concern layer providing a foundational service to all other application layers.

###### 2.1.1.3.2.5 Performance Optimizations Applied

- Specification requires asynchronous sinks to offload I/O operations.
- Specification requires high-performance enricher design to minimize logging overhead.
- Specification requires use of structured logging message templates to avoid string interpolation costs.

##### 2.1.1.3.3.0 File Structure

###### 2.1.1.3.3.1 Directory Organization

####### 2.1.1.3.3.1.1 Directory Path

######## 2.1.1.3.3.1.1.1 Directory Path

Enrichers

######## 2.1.1.3.3.1.1.2 Purpose

Contains custom implementations of Serilog's ILogEventEnricher interface to add or modify contextual information on log events.

######## 2.1.1.3.3.1.1.3 Contains Files

- PhiMaskingEnricher.cs
- CorrelationIdEnricher.cs

######## 2.1.1.3.3.1.1.4 Organizational Reasoning

Follows Serilog conventions by isolating custom components into functionally-named directories, promoting modularity and maintainability.

######## 2.1.1.3.3.1.1.5 Framework Convention Alignment

Standard practice for extending Serilog with custom functionality.

####### 2.1.1.3.3.1.2.0 Directory Path

######## 2.1.1.3.3.1.2.1 Directory Path

Context

######## 2.1.1.3.3.1.2.2 Purpose

Provides mechanisms for managing and propagating contextual information, such as Correlation IDs, across asynchronous call stacks.

######## 2.1.1.3.3.1.2.3 Contains Files

- CorrelationContext.cs

######## 2.1.1.3.3.1.2.4 Organizational Reasoning

Encapsulates the management of cross-cutting context into a dedicated component, decoupling it from the enricher that consumes it.

######## 2.1.1.3.3.1.2.5 Framework Convention Alignment

Common pattern for managing ambient context in .NET applications using AsyncLocal<T>.

####### 2.1.1.3.3.1.3.0 Directory Path

######## 2.1.1.3.3.1.3.1 Directory Path

Extensions

######## 2.1.1.3.3.1.3.2 Purpose

Contains static extension methods to simplify the registration and configuration of the logging framework within the application's startup code.

######## 2.1.1.3.3.1.3.3 Contains Files

- LoggerConfigurationExtensions.cs

######## 2.1.1.3.3.1.3.4 Organizational Reasoning

Provides a clean, fluent API for consumers of this library, abstracting away the details of enricher registration.

######## 2.1.1.3.3.1.3.5 Framework Convention Alignment

Standard .NET pattern for creating discoverable and easy-to-use library configuration helpers.

####### 2.1.1.3.3.1.4.0 Directory Path

######## 2.1.1.3.3.1.4.1 Directory Path

/

######## 2.1.1.3.3.1.4.2 Purpose

Root directory for the main configuration entry point of the logging library.

######## 2.1.1.3.3.1.4.3 Contains Files

- LoggingConfiguration.cs

######## 2.1.1.3.3.1.4.4 Organizational Reasoning

The primary public-facing class of the library is placed at the root for easy discoverability.

######## 2.1.1.3.3.1.4.5 Framework Convention Alignment

Common practice for utility or configuration-focused libraries.

###### 2.1.1.3.3.2.0.0 Namespace Strategy

| Property | Value |
|----------|-------|
| Root Namespace | DMPS.CrossCutting.Logging |
| Namespace Organization | Hierarchical by feature area (e.g., DMPS.CrossCutt... |
| Naming Conventions | PascalCase following Microsoft C# standards. |
| Framework Alignment | Aligns with .NET library design guidelines. |

##### 2.1.1.3.4.0.0.0 Class Specifications

###### 2.1.1.3.4.1.0.0 Class Name

####### 2.1.1.3.4.1.1.0 Class Name

LoggingConfiguration

####### 2.1.1.3.4.1.2.0 File Path

LoggingConfiguration.cs

####### 2.1.1.3.4.1.3.0 Class Type

Static Configuration Class

####### 2.1.1.3.4.1.4.0 Inheritance

None

####### 2.1.1.3.4.1.5.0 Purpose

Provides the central, static method for configuring the Serilog logger for the entire application. This is the main entry point for this repository.

####### 2.1.1.3.4.1.6.0 Dependencies

- Microsoft.Extensions.Configuration.IConfiguration
- Serilog.LoggerConfiguration
- DMPS.CrossCutting.Logging.Extensions.LoggerConfigurationExtensions

####### 2.1.1.3.4.1.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.1.8.0 Technology Integration Notes

Integrates with Microsoft.Extensions.Configuration to read settings from appsettings.json, enabling external configuration of logging levels, file paths, and retention policies.

####### 2.1.1.3.4.1.9.0 Validation Notes

Validation complete. Specification is aligned with repository scope and architectural patterns.

####### 2.1.1.3.4.1.10.0 Properties

*No items available*

####### 2.1.1.3.4.1.11.0 Methods

- {'method_name': 'ConfigureLogger', 'method_signature': 'ConfigureLogger(IConfiguration configuration)', 'return_type': 'Serilog.ILogger', 'access_modifier': 'public static', 'is_async': 'false', 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'configuration', 'parameter_type': 'IConfiguration', 'is_nullable': 'false', 'purpose': "The application's configuration provider, used to read logging settings from appsettings.json.", 'framework_attributes': []}], 'implementation_logic': 'Specification requires this method to perform the following actions:\\n1. Create a new Serilog.LoggerConfiguration instance.\\n2. Read configuration from the provided IConfiguration object using `.ReadFrom.Configuration(configuration)`.\\n3. Apply custom enrichers using the extension methods from LoggerConfigurationExtensions (e.g., `.WithCorrelationId()` and `.WithPhiMasking()`).\\n4. Configure the File sink to be asynchronous (`WriteTo.Async`) with properties for path, rolling interval, and retention count read from configuration.\\n5. Configure the Windows Event Log sink, specifying the source name.\\n6. Create the logger using `.CreateLogger()`.\\n7. Set the static Serilog.Log.Logger to this instance to capture any logs before DI is fully configured.\\n8. Return the created logger instance so it can be registered with the DI container.', 'exception_handling': 'Specification requires wrapping the configuration logic in a try-catch block. On failure, it must log the error to the Console and throw a specific `InvalidOperationException` to prevent the application from starting with misconfigured logging.', 'performance_considerations': 'This method is called only once at application startup, so performance is not critical. However, it is responsible for configuring sinks asynchronously to ensure high performance during application runtime.', 'validation_requirements': 'The method assumes the `IConfiguration` object contains a \\"Serilog\\" section with necessary settings. Absence of these settings must be handled gracefully.', 'technology_integration_details': "Directly uses Serilog's fluent configuration API and the `Serilog.Settings.Configuration` package."}

####### 2.1.1.3.4.1.12.0 Events

*No items available*

####### 2.1.1.3.4.1.13.0 Implementation Notes

This class serves as the sole public entry point for configuring logging, ensuring a consistent setup across both the client and service applications.

###### 2.1.1.3.4.2.0.0 Class Name

####### 2.1.1.3.4.2.1.0 Class Name

PhiMaskingEnricher

####### 2.1.1.3.4.2.2.0 File Path

Enrichers/PhiMaskingEnricher.cs

####### 2.1.1.3.4.2.3.0 Class Type

Serilog Enricher

####### 2.1.1.3.4.2.4.0 Inheritance

Serilog.Core.ILogEventEnricher

####### 2.1.1.3.4.2.5.0 Purpose

Implements the core security requirement to redact Protected Health Information (PHI) from all log events before they are written to any sink, ensuring HIPAA compliance (REQ-1-039).

####### 2.1.1.3.4.2.6.0 Dependencies

- Serilog.Core.ILogEventEnricher
- System.Text.RegularExpressions.Regex

####### 2.1.1.3.4.2.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.2.8.0 Technology Integration Notes

Leverages Serilog's extensibility model by implementing a core interface. Must be highly performant to avoid impacting application logging speed.

####### 2.1.1.3.4.2.9.0 Validation Notes

Validation complete. This component directly fulfills the PHI redaction requirement REQ-1-039.

####### 2.1.1.3.4.2.10.0 Properties

- {'property_name': 'PhiRedactionRegex', 'property_type': 'Regex', 'access_modifier': 'private static readonly', 'purpose': 'A pre-compiled regular expression used to efficiently identify and mask PHI patterns in log messages and properties.', 'validation_attributes': [], 'framework_specific_configuration': 'Specification requires the regex pattern to be defined internally.', 'implementation_notes': 'Specification requires the regex to be compiled for performance. It must target known PHI formats, such as patient names, MRNs, and accession numbers based on expected log message structures.'}

####### 2.1.1.3.4.2.11.0 Methods

- {'method_name': 'Enrich', 'method_signature': 'Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)', 'return_type': 'void', 'access_modifier': 'public', 'is_async': 'false', 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'logEvent', 'parameter_type': 'LogEvent', 'is_nullable': 'false', 'purpose': 'The log event to be enriched (i.e., have its data masked).', 'framework_attributes': []}, {'parameter_name': 'propertyFactory', 'parameter_type': 'ILogEventPropertyFactory', 'is_nullable': 'false', 'purpose': 'A factory for creating new or modified log event properties.', 'framework_attributes': []}], 'implementation_logic': 'Specification requires this method to:\\n1. Iterate through each property of the `logEvent`.\\n2. For each property that is a string, apply the `PhiRedactionRegex` to replace any matching PHI with a static string like \\"[REDACTED]\\".\\n3. Create a new, modified property using the `propertyFactory` and replace the original property on the `logEvent` using `logEvent.AddOrUpdateProperty()`.\\n4. The logic must handle both scalar string properties and potentially structured objects by recursively checking their string properties.', 'exception_handling': "Specification requires the enricher to be robust and never throw an exception, as this could crash the application's logging pipeline. Any internal errors must be caught and ignored, prioritizing application stability over perfect masking in edge cases.", 'performance_considerations': 'This is a critical path for every log message. Specification requires the use of a compiled, static Regex. The logic must avoid unnecessary allocations and complex operations.', 'validation_requirements': 'Requires extensive unit testing with various log messages containing and not containing PHI to ensure accurate masking without corrupting valid data.', 'technology_integration_details': 'Directly implements a core Serilog interface to plug into the logging pipeline.'}

####### 2.1.1.3.4.2.12.0 Events

*No items available*

####### 2.1.1.3.4.2.13.0 Implementation Notes

Specification requires the exact keywords and patterns for PHI to be defined and maintained within this class. The redaction must be non-reversible.

###### 2.1.1.3.4.3.0.0 Class Name

####### 2.1.1.3.4.3.1.0 Class Name

CorrelationIdEnricher

####### 2.1.1.3.4.3.2.0 File Path

Enrichers/CorrelationIdEnricher.cs

####### 2.1.1.3.4.3.3.0 Class Type

Serilog Enricher

####### 2.1.1.3.4.3.4.0 Inheritance

Serilog.Core.ILogEventEnricher

####### 2.1.1.3.4.3.5.0 Purpose

Enriches all log events with a \"CorrelationId\" property, enabling end-to-end tracing of operations across asynchronous boundaries and distributed components (REQ-1-090).

####### 2.1.1.3.4.3.6.0 Dependencies

- Serilog.Core.ILogEventEnricher
- DMPS.CrossCutting.Logging.Context.CorrelationContext

####### 2.1.1.3.4.3.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.3.8.0 Technology Integration Notes

Relies on an ambient context provider (`CorrelationContext`) to retrieve the ID, decoupling the logging mechanism from the context management mechanism.

####### 2.1.1.3.4.3.9.0 Validation Notes

Validation complete. This component directly fulfills the correlation ID requirement REQ-1-090.

####### 2.1.1.3.4.3.10.0 Properties

*No items available*

####### 2.1.1.3.4.3.11.0 Methods

- {'method_name': 'Enrich', 'method_signature': 'Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)', 'return_type': 'void', 'access_modifier': 'public', 'is_async': 'false', 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'logEvent', 'parameter_type': 'LogEvent', 'is_nullable': 'false', 'purpose': 'The log event to be enriched.', 'framework_attributes': []}, {'parameter_name': 'propertyFactory', 'parameter_type': 'ILogEventPropertyFactory', 'is_nullable': 'false', 'purpose': 'A factory for creating log event properties.', 'framework_attributes': []}], 'implementation_logic': 'Specification requires this method to:\\n1. Call `CorrelationContext.GetCorrelationId()` to retrieve the current correlation ID.\\n2. If the retrieved ID is not null or empty, create a new `LogEventProperty` named \\"CorrelationId\\" with the ID\'s value using the `propertyFactory`.\\n3. Add this new property to the `logEvent` using `logEvent.AddPropertyIfAbsent()`.', 'exception_handling': 'Specification requires this method to not throw exceptions. If the context retrieval fails for any reason, it should simply not add the property.', 'performance_considerations': 'The call to retrieve the correlation ID must be extremely fast. `AsyncLocal<T>` provides this performance characteristic.', 'validation_requirements': 'N/A', 'technology_integration_details': 'Integrates with the `CorrelationContext` to read the ambient correlation ID.'}

####### 2.1.1.3.4.3.12.0 Events

*No items available*

####### 2.1.1.3.4.3.13.0 Implementation Notes

This enricher is the \"read\" side of the correlation ID pattern. The \"write\" side will be implemented in middleware or service entry points in other repositories.

###### 2.1.1.3.4.4.0.0 Class Name

####### 2.1.1.3.4.4.1.0 Class Name

CorrelationContext

####### 2.1.1.3.4.4.2.0 File Path

Context/CorrelationContext.cs

####### 2.1.1.3.4.4.3.0 Class Type

Static Context Manager

####### 2.1.1.3.4.4.4.0 Inheritance

None

####### 2.1.1.3.4.4.5.0 Purpose

Provides a static, thread-safe, and async-aware mechanism for storing and retrieving the current operation's Correlation ID.

####### 2.1.1.3.4.4.6.0 Dependencies

- System.Threading.AsyncLocal<T>

####### 2.1.1.3.4.4.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.4.8.0 Technology Integration Notes

Utilizes `System.Threading.AsyncLocal<string>` which is the standard .NET mechanism for maintaining ambient context across `async/await` state machine transitions.

####### 2.1.1.3.4.4.9.0 Validation Notes

Validation complete. This component is a necessary dependency for the CorrelationIdEnricher.

####### 2.1.1.3.4.4.10.0 Properties

- {'property_name': '_correlationId', 'property_type': 'AsyncLocal<string>', 'access_modifier': 'private static readonly', 'purpose': 'The underlying storage for the correlation ID that flows with the asynchronous execution context.', 'validation_attributes': [], 'framework_specific_configuration': 'Specification requires this to be instantiated internally.', 'implementation_notes': 'This is the core of the context propagation mechanism.'}

####### 2.1.1.3.4.4.11.0 Methods

######## 2.1.1.3.4.4.11.1 Method Name

######### 2.1.1.3.4.4.11.1.1 Method Name

SetCorrelationId

######### 2.1.1.3.4.4.11.1.2 Method Signature

SetCorrelationId(string correlationId)

######### 2.1.1.3.4.4.11.1.3 Return Type

void

######### 2.1.1.3.4.4.11.1.4 Access Modifier

public static

######### 2.1.1.3.4.4.11.1.5 Is Async

false

######### 2.1.1.3.4.4.11.1.6 Framework Specific Attributes

*No items available*

######### 2.1.1.3.4.4.11.1.7 Parameters

- {'parameter_name': 'correlationId', 'parameter_type': 'string', 'is_nullable': 'false', 'purpose': 'The correlation ID to set for the current execution context.', 'framework_attributes': []}

######### 2.1.1.3.4.4.11.1.8 Implementation Logic

Specification requires this method to set the `Value` of the static `_correlationId` `AsyncLocal` field to the provided string.

######### 2.1.1.3.4.4.11.1.9 Exception Handling

N/A

######### 2.1.1.3.4.4.11.1.10 Performance Considerations

This is a very low-overhead operation.

######### 2.1.1.3.4.4.11.1.11 Validation Requirements

N/A

######### 2.1.1.3.4.4.11.1.12 Technology Integration Details

Directly uses the .NET `AsyncLocal<T>` API.

######## 2.1.1.3.4.4.11.2.0 Method Name

######### 2.1.1.3.4.4.11.2.1 Method Name

GetCorrelationId

######### 2.1.1.3.4.4.11.2.2 Method Signature

GetCorrelationId()

######### 2.1.1.3.4.4.11.2.3 Return Type

string

######### 2.1.1.3.4.4.11.2.4 Access Modifier

public static

######### 2.1.1.3.4.4.11.2.5 Is Async

false

######### 2.1.1.3.4.4.11.2.6 Framework Specific Attributes

*No items available*

######### 2.1.1.3.4.4.11.2.7 Parameters

*No items available*

######### 2.1.1.3.4.4.11.2.8 Implementation Logic

Specification requires this method to return the `Value` of the static `_correlationId` `AsyncLocal` field.

######### 2.1.1.3.4.4.11.2.9 Exception Handling

N/A

######### 2.1.1.3.4.4.11.2.10 Performance Considerations

This is a very low-overhead operation.

######### 2.1.1.3.4.4.11.2.11 Validation Requirements

N/A

######### 2.1.1.3.4.4.11.2.12 Technology Integration Details

Directly uses the .NET `AsyncLocal<T>` API.

####### 2.1.1.3.4.4.12.0.0 Events

*No items available*

####### 2.1.1.3.4.4.13.0.0 Implementation Notes

This class provides the shared context that the `CorrelationIdEnricher` reads from. It is designed to be used by high-level components like message consumers or API middleware to initiate the context.

###### 2.1.1.3.4.5.0.0.0 Class Name

####### 2.1.1.3.4.5.1.0.0 Class Name

LoggerConfigurationExtensions

####### 2.1.1.3.4.5.2.0.0 File Path

Extensions/LoggerConfigurationExtensions.cs

####### 2.1.1.3.4.5.3.0.0 Class Type

Static Extension Class

####### 2.1.1.3.4.5.4.0.0 Inheritance

None

####### 2.1.1.3.4.5.5.0.0 Purpose

Provides fluent extension methods for `Serilog.LoggerConfiguration` to simplify the addition of custom enrichers from this library.

####### 2.1.1.3.4.5.6.0.0 Dependencies

- Serilog.LoggerConfiguration
- DMPS.CrossCutting.Logging.Enrichers.PhiMaskingEnricher
- DMPS.CrossCutting.Logging.Enrichers.CorrelationIdEnricher

####### 2.1.1.3.4.5.7.0.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.5.8.0.0 Technology Integration Notes

Follows the standard .NET extension method pattern to enhance an external library's (Serilog) configuration API.

####### 2.1.1.3.4.5.9.0.0 Validation Notes

Validation complete. This component improves the usability and maintainability of the library's configuration.

####### 2.1.1.3.4.5.10.0.0 Properties

*No items available*

####### 2.1.1.3.4.5.11.0.0 Methods

######## 2.1.1.3.4.5.11.1.0 Method Name

######### 2.1.1.3.4.5.11.1.1 Method Name

WithPhiMasking

######### 2.1.1.3.4.5.11.1.2 Method Signature

WithPhiMasking(this LoggerConfiguration loggerConfiguration)

######### 2.1.1.3.4.5.11.1.3 Return Type

LoggerConfiguration

######### 2.1.1.3.4.5.11.1.4 Access Modifier

public static

######### 2.1.1.3.4.5.11.1.5 Is Async

false

######### 2.1.1.3.4.5.11.1.6 Framework Specific Attributes

*No items available*

######### 2.1.1.3.4.5.11.1.7 Parameters

- {'parameter_name': 'loggerConfiguration', 'parameter_type': 'LoggerConfiguration', 'is_nullable': 'false', 'purpose': 'The Serilog logger configuration instance to extend.', 'framework_attributes': []}

######### 2.1.1.3.4.5.11.1.8 Implementation Logic

Specification requires this method to call `loggerConfiguration.Enrich.With<PhiMaskingEnricher>()` and return the modified `loggerConfiguration` instance to allow for fluent chaining.

######### 2.1.1.3.4.5.11.1.9 Exception Handling

Specification requires a null check for the `loggerConfiguration` parameter.

######### 2.1.1.3.4.5.11.1.10 Performance Considerations

Called only at startup; no runtime performance impact.

######### 2.1.1.3.4.5.11.1.11 Validation Requirements

N/A

######### 2.1.1.3.4.5.11.1.12 Technology Integration Details

Uses Serilog's generic `With<T>()` method for enricher registration.

######## 2.1.1.3.4.5.11.2.0 Method Name

######### 2.1.1.3.4.5.11.2.1 Method Name

WithCorrelationId

######### 2.1.1.3.4.5.11.2.2 Method Signature

WithCorrelationId(this LoggerConfiguration loggerConfiguration)

######### 2.1.1.3.4.5.11.2.3 Return Type

LoggerConfiguration

######### 2.1.1.3.4.5.11.2.4 Access Modifier

public static

######### 2.1.1.3.4.5.11.2.5 Is Async

false

######### 2.1.1.3.4.5.11.2.6 Framework Specific Attributes

*No items available*

######### 2.1.1.3.4.5.11.2.7 Parameters

- {'parameter_name': 'loggerConfiguration', 'parameter_type': 'LoggerConfiguration', 'is_nullable': 'false', 'purpose': 'The Serilog logger configuration instance to extend.', 'framework_attributes': []}

######### 2.1.1.3.4.5.11.2.8 Implementation Logic

Specification requires this method to call `loggerConfiguration.Enrich.With<CorrelationIdEnricher>()` and return the modified `loggerConfiguration` instance.

######### 2.1.1.3.4.5.11.2.9 Exception Handling

Specification requires a null check for the `loggerConfiguration` parameter.

######### 2.1.1.3.4.5.11.2.10 Performance Considerations

Called only at startup; no runtime performance impact.

######### 2.1.1.3.4.5.11.2.11 Validation Requirements

N/A

######### 2.1.1.3.4.5.11.2.12 Technology Integration Details

Uses Serilog's generic `With<T>()` method for enricher registration.

####### 2.1.1.3.4.5.12.0.0 Events

*No items available*

####### 2.1.1.3.4.5.13.0.0 Implementation Notes

These extensions make the configuration code in the main application's `Program.cs` cleaner and more readable.

##### 2.1.1.3.5.0.0.0.0 Interface Specifications

*No items available*

##### 2.1.1.3.6.0.0.0.0 Enum Specifications

*No items available*

##### 2.1.1.3.7.0.0.0.0 Dto Specifications

*No items available*

##### 2.1.1.3.8.0.0.0.0 Configuration Specifications

- {'configuration_name': 'Serilog Settings in appsettings.json', 'file_path': 'appsettings.json (in consumer projects)', 'purpose': 'To provide external, environment-specific configuration for Serilog sinks, logging levels, and properties.', 'framework_base_class': 'Microsoft.Extensions.Configuration.IConfiguration', 'configuration_sections': [{'section_name': 'Serilog', 'properties': [{'property_name': 'MinimumLevel', 'property_type': 'object', 'default_value': '{\\"Default\\": \\"Information\\", \\"Override\\": {\\"Microsoft\\": \\"Warning\\", \\"System\\": \\"Warning\\"}}', 'required': 'true', 'description': 'Specifies the minimum log level for different sources. The \\"Default\\" is used unless a more specific \\"Override\\" matches.'}, {'property_name': 'WriteTo', 'property_type': 'array', 'default_value': 'See description', 'required': 'true', 'description': 'An array defining the sinks. Specification requires this to contain configurations for \\"File\\" and \\"EventLog\\".\\nExample for File sink: `{\\"Name\\": \\"File\\", \\"Args\\": {\\"path\\": \\"logs\\\\\\\\log-.txt\\", \\"rollingInterval\\": \\"Day\\", \\"retainedFileCountLimit\\": 7, \\"outputTemplate\\": \\"{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}\\"}}`.\\nExample for EventLog sink: `{\\"Name\\": \\"EventLog\\", \\"Args\\": {\\"source\\": \\"DMPS Application\\"}}`.'}, {'property_name': 'Enrich', 'property_type': 'array', 'default_value': '[\\"FromLogContext\\", \\"WithMachineName\\", \\"WithThreadId\\"]', 'required': 'true', 'description': 'Specifies standard Serilog enrichers to be applied. Custom enrichers from this repository must be added programmatically.'}]}], 'validation_requirements': 'The consuming application must ensure that the `Serilog` section is present in `appsettings.json` and contains valid settings for the required sinks.'}

##### 2.1.1.3.9.0.0.0.0 Dependency Injection Specifications

- {'service_interface': 'Serilog.ILogger', 'service_implementation': 'Serilog.Core.Logger', 'lifetime': 'Singleton', 'registration_reasoning': 'The logger is thread-safe and expensive to create. A single, global instance should be shared across the entire application for performance and consistency. This is the standard pattern for logging frameworks.', 'framework_registration_pattern': 'Specification requires the `LoggingConfiguration.ConfigureLogger` method to return a configured ILogger instance. This instance must be registered in the DI container of the consuming application (e.g., in `Program.cs`) using `builder.Services.AddSingleton(logger);`.'}

##### 2.1.1.3.10.0.0.0.0 External Integration Specifications

- {'integration_target': 'Serilog v3.1.1', 'integration_type': 'Library', 'required_client_classes': ['Serilog.LoggerConfiguration', 'Serilog.Core.ILogEventEnricher', 'Serilog.Sinks.File', 'Serilog.Sinks.EventLog'], 'configuration_requirements': 'Configuration is managed via the static `LoggingConfiguration` class and `appsettings.json`. This includes setting up sinks, enrichers, and log levels.', 'error_handling_requirements': 'The library is configured to be robust. Specification requires sink failures to be handled internally by Serilog and should not crash the main application. Enrichers must be implemented defensively to prevent exceptions.', 'authentication_requirements': 'N/A', 'framework_integration_patterns': "The repository acts as a configuration wrapper for the Serilog library, integrating it into the .NET host application's DI container and configuration system."}

#### 2.1.1.4.0.0.0.0.0 Component Count Validation

| Property | Value |
|----------|-------|
| Total Classes | 5 |
| Total Interfaces | 0 |
| Total Enums | 0 |
| Total Dtos | 0 |
| Total Configurations | 1 |
| Total External Integrations | 1 |
| Grand Total Components | 7 |
| Phase 2 Claimed Count | 1 |
| Phase 2 Actual Count | 1 |
| Validation Added Count | 6 |
| Final Validated Count | 7 |

## 2.2.0.0.0.0.0.0.0 Project Supporting Files

### 2.2.1.0.0.0.0.0.0 File Type

#### 2.2.1.1.0.0.0.0.0 File Type

Project Definition

#### 2.2.1.2.0.0.0.0.0 File Name

DMPS.CrossCutting.Logging.csproj

#### 2.2.1.3.0.0.0.0.0 File Path

./DMPS.CrossCutting.Logging.csproj

#### 2.2.1.4.0.0.0.0.0 Purpose

Defines the .NET 8 class library project, its target framework, and its NuGet package dependencies on Serilog and related libraries.

#### 2.2.1.5.0.0.0.0.0 Content Description

<Project Sdk=\"Microsoft.NET.Sdk\">\n\n  <PropertyGroup>\n    <TargetFramework>net8.0</TargetFramework>\n    <ImplicitUsings>enable</ImplicitUsings>\n    <Nullable>enable</Nullable>\n  </PropertyGroup>\n\n  <ItemGroup>\n    <PackageReference Include=\"Microsoft.Extensions.Configuration.Abstractions\" Version=\"8.0.0\" />\n    <PackageReference Include=\"Serilog\" Version=\"3.1.1\" />\n    <PackageReference Include=\"Serilog.Settings.Configuration\" Version=\"8.0.0\" />\n    <PackageReference Include=\"Serilog.Sinks.Async\" Version=\"1.5.0\" />\n    <PackageReference Include=\"Serilog.Sinks.EventLog\" Version=\"3.1.0\" />\n    <PackageReference Include=\"Serilog.Sinks.File\" Version=\"5.0.0\" />\n  </ItemGroup>\n\n</Project>

#### 2.2.1.6.0.0.0.0.0 Framework Specific Attributes

- TargetFramework: net8.0
- PackageReference for Serilog and its required sinks.

### 2.2.2.0.0.0.0.0.0 File Type

#### 2.2.2.1.0.0.0.0.0 File Type

Version Control

#### 2.2.2.2.0.0.0.0.0 File Name

.gitignore

#### 2.2.2.3.0.0.0.0.0 File Path

./.gitignore

#### 2.2.2.4.0.0.0.0.0 Purpose

Specifies intentionally untracked files for Git to ignore, tailored for .NET development.

#### 2.2.2.5.0.0.0.0.0 Content Description

# Ignore Visual Studio temporary files, build artifacts, and user-specific settings.\n[Bb]in/\n[Oo]bj/\n.vs/\n*.suo\n*.user

#### 2.2.2.6.0.0.0.0.0 Framework Specific Attributes

- .NET build output folders (bin, obj)
- Visual Studio specific files (.vs, .suo, .user)

### 2.2.3.0.0.0.0.0.0 File Type

#### 2.2.3.1.0.0.0.0.0 File Type

Development Tools

#### 2.2.3.2.0.0.0.0.0 File Name

.editorconfig

#### 2.2.3.3.0.0.0.0.0 File Path

./.editorconfig

#### 2.2.3.4.0.0.0.0.0 Purpose

Defines and maintains consistent coding styles between multiple developers and IDEs.

#### 2.2.3.5.0.0.0.0.0 Content Description

root = true\n\n[*]\nindent_style = space\nindent_size = 4\nend_of_line = lf\ncharset = utf-8\ntrim_trailing_whitespace = true\ninsert_final_newline = true\n\n[*.cs]\ncsharp_style_namespace_declarations = file_scoped\ncsharp_style_prefer_simplified_interpolation = true\n

#### 2.2.3.6.0.0.0.0.0 Framework Specific Attributes

- csharp_style_namespace_declarations: file_scoped (C# 10+ feature)
- Standard formatting rules

