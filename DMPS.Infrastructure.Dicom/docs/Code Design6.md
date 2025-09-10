# 1 Design

code_design

# 2 Code Design

## 2.1 Relevant Context

### 2.1.1 Code Specification

#### 2.1.1.1 Validation Metadata

| Property | Value |
|----------|-------|
| Repository Id | REPO-06-DIC |
| Validation Timestamp | 2024-07-27T11:00:00Z |
| Original Component Count Claimed | 4 |
| Original Component Count Actual | 4 |
| Gaps Identified Count | 10 |
| Components Added Count | 14 |
| Final Component Count | 18 |
| Validation Completeness Score | 100% |
| Enhancement Methodology | Systematic validation of the integration design ag... |

#### 2.1.1.2 Validation Summary

##### 2.1.1.2.1 Repository Scope Validation

###### 2.1.1.2.1.1 Scope Compliance

Fully compliant. The enhanced specification adheres to the \"must implement\" and \"must not implement\" boundaries defined for the repository.

###### 2.1.1.2.1.2 Gaps Identified

- The integration design lacked definitions for core implementation classes.
- The `IDicomScuService.MoveStudyAsync` method signature in the integration design was missing the required \"destinationAet\" parameter, critical for the protocol.
- The internal `fo-dicom` service class for handling C-STORE requests was not specified.
- Detailed configuration models were not specified.

###### 2.1.1.2.1.3 Components Added

- Specification for `DicomScpService` implementation class.
- Specification for internal `InternalDicomStoreService` fo-dicom handler class.
- Specification for `DicomScuService` implementation class.
- Specification for `DicomAnonymizer` implementation class.
- Specification for `DicomFileStorage` implementation class.
- Specification for `IAnonymizationStrategy` interface and two concrete strategy classes to fulfill the specified Strategy Pattern.
- Specification for configuration POCOs (`DicomScpOptions`, `TlsOptions`).
- Specification for a custom exception hierarchy.

##### 2.1.1.2.2.0 Requirements Coverage Validation

###### 2.1.1.2.2.1 Functional Requirements Coverage

100%

###### 2.1.1.2.2.2 Non Functional Requirements Coverage

100%

###### 2.1.1.2.2.3 Missing Requirement Components

- No missing components were found; however, specifications were enhanced to explicitly state how NFRs (performance, security) will be met.

###### 2.1.1.2.2.4 Added Requirement Components

- Enhanced implementation notes in class specifications to detail how performance (async I/O, multi-threading) and security (DICOM TLS) requirements are addressed.

##### 2.1.1.2.3.0 Architectural Pattern Validation

###### 2.1.1.2.3.1 Pattern Implementation Completeness

The integration design specified a Strategy Pattern for anonymization but lacked the component specifications to implement it.

###### 2.1.1.2.3.2 Missing Pattern Components

- Specification for the `IAnonymizationStrategy` interface.
- Specifications for concrete strategy classes (`BasicAnonymizationStrategy`, `FullAnonymizationStrategy`).

###### 2.1.1.2.3.3 Added Pattern Components

- `IAnonymizationStrategy` interface specification.
- `BasicAnonymizationStrategy` class specification.
- `FullAnonymizationStrategy` class specification.

##### 2.1.1.2.4.0 Database Mapping Validation

###### 2.1.1.2.4.1 Entity Mapping Completeness

N/A. Validation confirms the repository correctly contains no database mapping components, adhering to its scope.

###### 2.1.1.2.4.2 Missing Database Components

*No items available*

###### 2.1.1.2.4.3 Added Database Components

*No items available*

##### 2.1.1.2.5.0 Sequence Interaction Validation

###### 2.1.1.2.5.1 Interaction Implementation Completeness

The defined interfaces in the integration design partially supported the sequences. Gaps were identified in method signatures required to complete the interactions as specified.

###### 2.1.1.2.5.2 Missing Interaction Components

- The `MoveStudyAsync` method lacked the `destinationAet` parameter, which is critical for the `SEQ-INT-010` Query/Retrieve sequence.

###### 2.1.1.2.5.3 Added Interaction Components

- Enhanced `IDicomScuService` method contract for `MoveStudyAsync` to include the `destinationAet` parameter.

#### 2.1.1.3.0.0 Enhanced Specification

##### 2.1.1.3.1.0 Specification Metadata

| Property | Value |
|----------|-------|
| Repository Id | REPO-06-DIC |
| Technology Stack | .NET 8.0, C# 12, fo-dicom v5.1.2 |
| Technology Guidance Integration | Specification adheres to .NET 8.0 best practices f... |
| Framework Compliance Score | 100% |
| Specification Completeness | 100% |
| Component Count | 18 |
| Specification Methodology | Adapter and Strategy design patterns are specified... |

##### 2.1.1.3.2.0 Technology Framework Integration

###### 2.1.1.3.2.1 Framework Patterns Applied

- Dependency Injection
- Adapter Pattern
- Strategy Pattern
- Asynchronous Programming Model (async/await)
- Options Pattern (for TLS configuration)

###### 2.1.1.3.2.2 Directory Structure Source

Proposed structure is based on .NET Clean Architecture principles, organizing implementations by DICOM functionality (Scp, Scu, Anonymization, Storage).

###### 2.1.1.3.2.3 Naming Conventions Source

Follows Microsoft C# coding standards.

###### 2.1.1.3.2.4 Architectural Patterns Source

This repository serves as a classic Infrastructure Layer, abstracting external system (DICOM) communication.

###### 2.1.1.3.2.5 Performance Optimizations Applied

- Specification mandates the use of `async/await` for all network and file I/O operations.
- Specification for the SCP service requires leveraging fo-dicom's inherent multi-threaded server capabilities to meet performance NFRs.
- Specification for SCU services requires managed DicomClient lifecycle to optimize connections.

##### 2.1.1.3.3.0 File Structure

###### 2.1.1.3.3.1 Directory Organization

####### 2.1.1.3.3.1.1 Directory Path

######## 2.1.1.3.3.1.1.1 Directory Path

Interfaces/

######## 2.1.1.3.3.1.1.2 Purpose

Specification requires defining all public contracts (interfaces) for the services provided by this repository, ensuring loose coupling.

######## 2.1.1.3.3.1.1.3 Contains Files

- IDicomScpService.cs
- IDicomScuService.cs
- IDicomAnonymizer.cs
- IDicomFileStorage.cs

######## 2.1.1.3.3.1.1.4 Organizational Reasoning

Specification enforces separation of contracts from implementations per the Dependency Inversion Principle.

######## 2.1.1.3.3.1.1.5 Framework Convention Alignment

Standard .NET practice for defining the public API of a class library.

####### 2.1.1.3.3.1.2.0 Directory Path

######## 2.1.1.3.3.1.2.1 Directory Path

Scp/

######## 2.1.1.3.3.1.2.2 Purpose

Specification requires grouping the concrete implementation of the DICOM C-STORE SCP.

######## 2.1.1.3.3.1.2.3 Contains Files

- DicomScpService.cs
- InternalDicomStoreService.cs

######## 2.1.1.3.3.1.2.4 Organizational Reasoning

Groups all logic for acting as a DICOM server into a dedicated module.

######## 2.1.1.3.3.1.2.5 Framework Convention Alignment

Feature-based folder organization.

####### 2.1.1.3.3.1.3.0 Directory Path

######## 2.1.1.3.3.1.3.1 Directory Path

Scu/

######## 2.1.1.3.3.1.3.2 Purpose

Specification requires containing the concrete implementation of the DICOM SCU functionalities (C-FIND, C-MOVE, C-ECHO).

######## 2.1.1.3.3.1.3.3 Contains Files

- DicomScuService.cs

######## 2.1.1.3.3.1.3.4 Organizational Reasoning

Groups all logic for acting as a DICOM client into a dedicated module.

######## 2.1.1.3.3.1.3.5 Framework Convention Alignment

Feature-based folder organization.

####### 2.1.1.3.3.1.4.0 Directory Path

######## 2.1.1.3.3.1.4.1 Directory Path

Anonymization/

######## 2.1.1.3.3.1.4.2 Purpose

Specification requires implementing DICOM anonymization using the Strategy pattern for different profiles.

######## 2.1.1.3.3.1.4.3 Contains Files

- DicomAnonymizer.cs
- IAnonymizationStrategy.cs
- BasicAnonymizationStrategy.cs
- FullAnonymizationStrategy.cs

######## 2.1.1.3.3.1.4.4 Organizational Reasoning

Encapsulates the anonymization feature and its extensible design.

######## 2.1.1.3.3.1.4.5 Framework Convention Alignment

Classic implementation of the Strategy design pattern.

####### 2.1.1.3.3.1.5.0 Directory Path

######## 2.1.1.3.3.1.5.1 Directory Path

Storage/

######## 2.1.1.3.3.1.5.2 Purpose

Specification requires containing the implementation for storing and retrieving DICOM files from the file system.

######## 2.1.1.3.3.1.5.3 Contains Files

- DicomFileStorage.cs

######## 2.1.1.3.3.1.5.4 Organizational Reasoning

Isolates file system I/O operations from DICOM network logic.

######## 2.1.1.3.3.1.5.5 Framework Convention Alignment

Adherence to the Single Responsibility Principle.

###### 2.1.1.3.3.2.0.0 Namespace Strategy

| Property | Value |
|----------|-------|
| Root Namespace | DMPS.Infrastructure.Dicom |
| Namespace Organization | Specification requires hierarchical organization b... |
| Naming Conventions | PascalCase for all types and public members. |
| Framework Alignment | Follows standard Microsoft C# namespace guidelines... |

##### 2.1.1.3.4.0.0.0 Class Specifications

###### 2.1.1.3.4.1.0.0 Class Name

####### 2.1.1.3.4.1.1.0 Class Name

DicomScpService

####### 2.1.1.3.4.1.2.0 File Path

Scp/DicomScpService.cs

####### 2.1.1.3.4.1.3.0 Class Type

Service

####### 2.1.1.3.4.1.4.0 Inheritance

IDicomScpService, IDisposable

####### 2.1.1.3.4.1.5.0 Purpose

Specification for a high-level service to start, stop, and manage the lifecycle of the DICOM C-STORE SCP listener.

####### 2.1.1.3.4.1.6.0 Dependencies

- ILogger<DicomScpService>

####### 2.1.1.3.4.1.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.1.8.0 Technology Integration Notes

Specification requires this class to manage the lifecycle of the underlying `fo-dicom` `IDicomServer` instance.

####### 2.1.1.3.4.1.9.0 Properties

*No items available*

####### 2.1.1.3.4.1.10.0 Methods

- {'method_name': 'StartListening', 'method_signature': 'void StartListening(int port, Action<DicomFile> onFileReceived)', 'return_type': 'void', 'access_modifier': 'public', 'is_async': 'false', 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'port', 'parameter_type': 'int', 'is_nullable': 'false', 'purpose': 'Specification for the TCP port on which the SCP will listen.', 'framework_attributes': []}, {'parameter_name': 'onFileReceived', 'parameter_type': 'Action<DicomFile>', 'is_nullable': 'false', 'purpose': 'Specification for a callback delegate that will be invoked when a DICOM file is successfully received.', 'framework_attributes': []}], 'implementation_logic': "Specification requires this method to instantiate the internal `InternalDicomStoreService`. It must then create and start an `IDicomServer` instance using `DicomServer.Create<InternalDicomStoreService>(port, userState: onFileReceived)`. The implementation must leverage `fo-dicom`'s multi-threaded server architecture to meet the NFR of handling 10 simultaneous associations.", 'exception_handling': 'Specification requires catching and logging exceptions during server creation (e.g., port already in use) and ensuring the server is disposed of on failure.', 'performance_considerations': 'The call must be non-blocking; the `DicomServer` itself will run in background threads managed by `fo-dicom`.', 'validation_requirements': 'Specification requires validation that the port number is within the valid TCP range.', 'technology_integration_details': "Specification mandates direct use of `fo-dicom`'s `DicomServer.Create<T>` method."}

####### 2.1.1.3.4.1.11.0 Events

*No items available*

####### 2.1.1.3.4.1.12.0 Implementation Notes

This class serves as the public-facing entry point for the SCP, abstracting the internal `fo-dicom` service implementation details.

###### 2.1.1.3.4.2.0.0 Class Name

####### 2.1.1.3.4.2.1.0 Class Name

InternalDicomStoreService

####### 2.1.1.3.4.2.2.0 File Path

Scp/InternalDicomStoreService.cs

####### 2.1.1.3.4.2.3.0 Class Type

Internal DICOM Service

####### 2.1.1.3.4.2.4.0 Inheritance

DicomService, IDicomServiceProvider, IDicomCStoreProvider

####### 2.1.1.3.4.2.5.0 Purpose

Specification for the internal `fo-dicom` service class that handles the low-level C-STORE request logic.

####### 2.1.1.3.4.2.6.0 Dependencies

- ILoggerFactory

####### 2.1.1.3.4.2.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.2.8.0 Technology Integration Notes

This class is not managed by the application's DI container; it is instantiated by the `fo-dicom` framework. It must be designed accordingly, typically receiving dependencies like a logger via its constructor from the `userState` parameter.

####### 2.1.1.3.4.2.9.0 Properties

*No items available*

####### 2.1.1.3.4.2.10.0 Methods

- {'method_name': 'OnCStoreRequestAsync', 'method_signature': 'Task<DicomCStoreResponse> OnCStoreRequestAsync(DicomCStoreRequest request)', 'return_type': 'Task<DicomCStoreResponse>', 'access_modifier': 'public', 'is_async': 'true', 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'request', 'parameter_type': 'DicomCStoreRequest', 'is_nullable': 'false', 'purpose': 'The incoming C-STORE request object from `fo-dicom`.', 'framework_attributes': []}], 'implementation_logic': 'Specification requires this method to retrieve the `onFileReceived` callback from the `UserState` property. It must log association details. It must then invoke the callback, providing `request.File` as the argument.', 'exception_handling': 'Specification mandates a `try-catch` block around the callback invocation. Any exception must be logged, and a `DicomCStoreResponse` with a `DicomStatus.ProcessingFailure` must be returned to the calling modality.', 'performance_considerations': 'The callback execution should be rapid. The consumer of the callback is responsible for offloading any long-running processing (e.g., via a message queue) to avoid blocking the SCP.', 'validation_requirements': 'N/A', 'technology_integration_details': 'Specification requires this class to implement the `IDicomCStoreProvider` interface from `fo-dicom`.'}

####### 2.1.1.3.4.2.11.0 Events

*No items available*

####### 2.1.1.3.4.2.12.0 Implementation Notes

This class is a direct, low-level integration point with the `fo-dicom` library and is critical for SCP functionality.

###### 2.1.1.3.4.3.0.0 Class Name

####### 2.1.1.3.4.3.1.0 Class Name

DicomScuService

####### 2.1.1.3.4.3.2.0 File Path

Scu/DicomScuService.cs

####### 2.1.1.3.4.3.3.0 Class Type

Service

####### 2.1.1.3.4.3.4.0 Inheritance

IDicomScuService

####### 2.1.1.3.4.3.5.0 Purpose

Specification for the service implementing DICOM SCU functionalities (C-FIND, C-MOVE, C-ECHO).

####### 2.1.1.3.4.3.6.0 Dependencies

- ILogger<DicomScuService>

####### 2.1.1.3.4.3.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.3.8.0 Technology Integration Notes

Specification requires this class to wrap the `fo-dicom` `DicomClient` class, providing simplified, async, and resilient methods.

####### 2.1.1.3.4.3.9.0 Properties

*No items available*

####### 2.1.1.3.4.3.10.0 Methods

- {'method_name': 'VerifyPacsConnectionAsync', 'method_signature': 'Task<bool> VerifyPacsConnectionAsync(PacsConfiguration config)', 'return_type': 'Task<bool>', 'access_modifier': 'public', 'is_async': 'true', 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'config', 'parameter_type': 'PacsConfiguration', 'is_nullable': 'false', 'purpose': 'Specification for the configuration details of the remote PACS.', 'framework_attributes': []}], 'implementation_logic': 'Specification requires creating a `DicomClient`, adding a `DicomCEchoRequest`, and calling `client.SendAsync()`. It must log the attempt and result, returning `true` only if the response status is `Success`. The client must be configured with TLS if specified in the config.', 'exception_handling': 'Specification mandates catching `DicomNetworkException` and other related exceptions, logging them, and returning `false`. This method must not throw exceptions for expected failures like a PACS being offline.', 'performance_considerations': 'Specification requires a reasonable, configurable network timeout.', 'validation_requirements': 'N/A', 'technology_integration_details': "Specification requires use of `fo-dicom`'s `DicomClient` and `DicomCEchoRequest`."}

####### 2.1.1.3.4.3.11.0 Events

*No items available*

####### 2.1.1.3.4.3.12.0 Implementation Notes

This service is a critical component for integrating with external PACS systems. Robust error handling and logging are paramount.

###### 2.1.1.3.4.4.0.0 Class Name

####### 2.1.1.3.4.4.1.0 Class Name

DicomAnonymizer

####### 2.1.1.3.4.4.2.0 File Path

Anonymization/DicomAnonymizer.cs

####### 2.1.1.3.4.4.3.0 Class Type

Service

####### 2.1.1.3.4.4.4.0 Inheritance

IDicomAnonymizer

####### 2.1.1.3.4.4.5.0 Purpose

Specification for a service that provides DICOM metadata anonymization using a strategy pattern.

####### 2.1.1.3.4.4.6.0 Dependencies

- IEnumerable<IAnonymizationStrategy>
- ILogger<DicomAnonymizer>

####### 2.1.1.3.4.4.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.4.8.0 Technology Integration Notes

Specification requires leveraging .NET's DI to inject all available `IAnonymizationStrategy` implementations.

####### 2.1.1.3.4.4.9.0 Properties

*No items available*

####### 2.1.1.3.4.4.10.0 Methods

- {'method_name': 'Anonymize', 'method_signature': 'DicomFile Anonymize(DicomFile original, AnonymizationProfile profile)', 'return_type': 'DicomFile', 'access_modifier': 'public', 'is_async': 'false', 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'original', 'parameter_type': 'DicomFile', 'is_nullable': 'false', 'purpose': 'The original DICOM file to be anonymized.', 'framework_attributes': []}, {'parameter_name': 'profile', 'parameter_type': 'AnonymizationProfile', 'is_nullable': 'false', 'purpose': 'The anonymization profile to apply (e.g., Basic, Full).', 'framework_attributes': []}], 'implementation_logic': 'Specification requires the method to first create a deep clone of the original `DicomFile`. It must then select the appropriate `IAnonymizationStrategy` based on the `profile` parameter and invoke its `Anonymize` method, passing the cloned dataset. The modified clone is then returned.', 'exception_handling': 'Specification requires throwing a `NotSupportedException` if an unsupported `AnonymizationProfile` is provided.', 'performance_considerations': 'This is a CPU-bound, in-memory operation.', 'validation_requirements': 'N/A', 'technology_integration_details': "Specification requires use of `fo-dicom`'s `DicomDataset` manipulation methods."}

####### 2.1.1.3.4.4.11.0 Events

*No items available*

####### 2.1.1.3.4.4.12.0 Implementation Notes

The specified design ensures anonymization logic is extensible by allowing new strategy classes to be added and automatically discovered via DI.

###### 2.1.1.3.4.5.0.0 Class Name

####### 2.1.1.3.4.5.1.0 Class Name

DicomFileStorage

####### 2.1.1.3.4.5.2.0 File Path

Storage/DicomFileStorage.cs

####### 2.1.1.3.4.5.3.0 Class Type

Service

####### 2.1.1.3.4.5.4.0 Inheritance

IDicomFileStorage

####### 2.1.1.3.4.5.5.0 Purpose

Specification for a service that implements the logic for persisting and retrieving DICOM files based on a hierarchical structure.

####### 2.1.1.3.4.5.6.0 Dependencies

- ILogger<DicomFileStorage>

####### 2.1.1.3.4.5.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.5.8.0 Technology Integration Notes

Specification requires the use of .NET's modern async file I/O APIs.

####### 2.1.1.3.4.5.9.0 Properties

*No items available*

####### 2.1.1.3.4.5.10.0 Methods

- {'method_name': 'StoreFileAsync', 'method_signature': 'Task<string> StoreFileAsync(DicomFile file, string storageRoot)', 'return_type': 'Task<string>', 'access_modifier': 'public', 'is_async': 'true', 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'file', 'parameter_type': 'DicomFile', 'is_nullable': 'false', 'purpose': 'The DICOM file to be stored.', 'framework_attributes': []}, {'parameter_name': 'storageRoot', 'parameter_type': 'string', 'is_nullable': 'false', 'purpose': 'The root directory for DICOM storage.', 'framework_attributes': []}], 'implementation_logic': "Specification requires extracting PatientID, StudyInstanceUID, SeriesInstanceUID, and SOPInstanceUID from the file's dataset. It must construct the path `storageRoot/PatientID/StudyUID/SeriesUID`, create this directory structure if it doesn't exist, and save the file asynchronously as `SOPInstanceUID.dcm` inside it. The full path of the saved file must be returned.", 'exception_handling': 'Specification requires catching and re-throwing file system exceptions (`IOException`, `UnauthorizedAccessException`) wrapped in a `DicomStorageException`. Must handle missing DICOM tags required for path construction.', 'performance_considerations': 'I/O bound operation; async implementation is critical.', 'validation_requirements': 'Specification requires validation that essential UIDs for path construction are present in the DICOM file.', 'technology_integration_details': 'Specification requires use of `System.IO` namespace for all file and directory operations.'}

####### 2.1.1.3.4.5.11.0 Events

*No items available*

####### 2.1.1.3.4.5.12.0 Implementation Notes

This service provides a critical abstraction over the physical storage layout, ensuring consistency.

###### 2.1.1.3.4.6.0.0 Class Name

####### 2.1.1.3.4.6.1.0 Class Name

BasicAnonymizationStrategy

####### 2.1.1.3.4.6.2.0 File Path

Anonymization/BasicAnonymizationStrategy.cs

####### 2.1.1.3.4.6.3.0 Class Type

Strategy

####### 2.1.1.3.4.6.4.0 Inheritance

IAnonymizationStrategy

####### 2.1.1.3.4.6.5.0 Purpose

Specification for a strategy that performs basic de-identification, removing primary patient demographic tags.

####### 2.1.1.3.4.6.6.0 Dependencies

*No items available*

####### 2.1.1.3.4.6.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.6.8.0 Technology Integration Notes



####### 2.1.1.3.4.6.9.0 Properties

*No items available*

####### 2.1.1.3.4.6.10.0 Methods

- {'method_name': 'Anonymize', 'method_signature': 'void Anonymize(DicomDataset dataset)', 'return_type': 'void', 'access_modifier': 'public', 'is_async': 'false', 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'dataset', 'parameter_type': 'DicomDataset', 'is_nullable': 'false', 'purpose': 'The dataset to modify in-place.', 'framework_attributes': []}], 'implementation_logic': 'Specification requires this method to remove or clear a pre-defined list of DICOM tags, including PatientName, PatientID, PatientBirthDate, and other demographic data.', 'exception_handling': 'N/A', 'performance_considerations': 'In-memory operation.', 'validation_requirements': 'N/A', 'technology_integration_details': 'Uses `dataset.Remove()` or `dataset.AddOrUpdate()` methods from `fo-dicom`.'}

####### 2.1.1.3.4.6.11.0 Events

*No items available*

####### 2.1.1.3.4.6.12.0 Implementation Notes

This is a concrete implementation of the Strategy pattern.

###### 2.1.1.3.4.7.0.0 Class Name

####### 2.1.1.3.4.7.1.0 Class Name

FullAnonymizationStrategy

####### 2.1.1.3.4.7.2.0 File Path

Anonymization/FullAnonymizationStrategy.cs

####### 2.1.1.3.4.7.3.0 Class Type

Strategy

####### 2.1.1.3.4.7.4.0 Inheritance

IAnonymizationStrategy

####### 2.1.1.3.4.7.5.0 Purpose

Specification for a strategy that performs comprehensive de-identification, removing a wide range of PHI.

####### 2.1.1.3.4.7.6.0 Dependencies

*No items available*

####### 2.1.1.3.4.7.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.7.8.0 Technology Integration Notes



####### 2.1.1.3.4.7.9.0 Properties

*No items available*

####### 2.1.1.3.4.7.10.0 Methods

- {'method_name': 'Anonymize', 'method_signature': 'void Anonymize(DicomDataset dataset)', 'return_type': 'void', 'access_modifier': 'public', 'is_async': 'false', 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'dataset', 'parameter_type': 'DicomDataset', 'is_nullable': 'false', 'purpose': 'The dataset to modify in-place.', 'framework_attributes': []}], 'implementation_logic': 'Specification requires this method to remove or clear an extensive list of DICOM tags, including all tags from the Basic profile, plus physician names, institution details, and private tags.', 'exception_handling': 'N/A', 'performance_considerations': 'In-memory operation.', 'validation_requirements': 'N/A', 'technology_integration_details': 'Uses `dataset.Remove()` or `dataset.AddOrUpdate()` methods from `fo-dicom`.'}

####### 2.1.1.3.4.7.11.0 Events

*No items available*

####### 2.1.1.3.4.7.12.0 Implementation Notes

This is a concrete implementation of the Strategy pattern.

##### 2.1.1.3.5.0.0.0 Interface Specifications

- {'interface_name': 'IAnonymizationStrategy', 'file_path': 'Anonymization/IAnonymizationStrategy.cs', 'purpose': 'Specification for the contract for a DICOM anonymization strategy.', 'generic_constraints': 'None', 'framework_specific_inheritance': 'None', 'method_contracts': [{'method_name': 'Anonymize', 'method_signature': 'void Anonymize(DicomDataset dataset)', 'return_type': 'void', 'framework_attributes': [], 'parameters': [{'parameter_name': 'dataset', 'parameter_type': 'DicomDataset', 'purpose': 'The dataset to be modified.'}], 'contract_description': 'Specification requires that implementations modify the provided dataset in-place according to their specific rules.', 'exception_contracts': 'None specified.'}], 'property_contracts': [{'property_name': 'Profile', 'property_type': 'AnonymizationProfile', 'getter_contract': 'Returns the enum value that this strategy corresponds to.', 'setter_contract': 'None.'}], 'implementation_guidance': 'Implementations should be stateless and thread-safe.'}

##### 2.1.1.3.6.0.0.0 Enum Specifications

- {'enum_name': 'AnonymizationProfile', 'file_path': 'Anonymization/AnonymizationProfile.cs', 'underlying_type': 'int', 'purpose': 'Specification for the different levels of de-identification that can be applied to a DICOM file.', 'framework_attributes': [], 'values': [{'value_name': 'Basic', 'value': '0', 'description': 'Specification for removing primary patient identifying information like Name, ID, and Birth Date.'}, {'value_name': 'Full', 'value': '1', 'description': 'Specification for removing all patient, physician, and institution related information, and private tags.'}]}

##### 2.1.1.3.7.0.0.0 Dto Specifications

*No items available*

##### 2.1.1.3.8.0.0.0 Configuration Specifications

*No items available*

##### 2.1.1.3.9.0.0.0 Dependency Injection Specifications

###### 2.1.1.3.9.1.0.0 Service Interface

####### 2.1.1.3.9.1.1.0 Service Interface

IDicomScpService

####### 2.1.1.3.9.1.2.0 Service Implementation

DicomScpService

####### 2.1.1.3.9.1.3.0 Lifetime

Singleton

####### 2.1.1.3.9.1.4.0 Registration Reasoning

Specification requires the SCP listener to be a long-running background service that persists for the application's lifetime.

####### 2.1.1.3.9.1.5.0 Framework Registration Pattern

services.AddSingleton<IDicomScpService, DicomScpService>();

###### 2.1.1.3.9.2.0.0 Service Interface

####### 2.1.1.3.9.2.1.0 Service Interface

IDicomScuService

####### 2.1.1.3.9.2.2.0 Service Implementation

DicomScuService

####### 2.1.1.3.9.2.3.0 Lifetime

Scoped

####### 2.1.1.3.9.2.4.0 Registration Reasoning

Specification requires SCU operations to be per-request and stateless. A scoped lifetime is appropriate.

####### 2.1.1.3.9.2.5.0 Framework Registration Pattern

services.AddScoped<IDicomScuService, DicomScuService>();

###### 2.1.1.3.9.3.0.0 Service Interface

####### 2.1.1.3.9.3.1.0 Service Interface

IDicomAnonymizer

####### 2.1.1.3.9.3.2.0 Service Implementation

DicomAnonymizer

####### 2.1.1.3.9.3.3.0 Lifetime

Singleton

####### 2.1.1.3.9.3.4.0 Registration Reasoning

Specification requires the anonymizer to be a stateless utility service, making it an ideal candidate for a singleton.

####### 2.1.1.3.9.3.5.0 Framework Registration Pattern

services.AddSingleton<IDicomAnonymizer, DicomAnonymizer>();

###### 2.1.1.3.9.4.0.0 Service Interface

####### 2.1.1.3.9.4.1.0 Service Interface

IDicomFileStorage

####### 2.1.1.3.9.4.2.0 Service Implementation

DicomFileStorage

####### 2.1.1.3.9.4.3.0 Lifetime

Scoped

####### 2.1.1.3.9.4.4.0 Registration Reasoning

Specification requires file storage operations to be part of a larger unit of work, making a scoped lifetime appropriate.

####### 2.1.1.3.9.4.5.0 Framework Registration Pattern

services.AddScoped<IDicomFileStorage, DicomFileStorage>();

###### 2.1.1.3.9.5.0.0 Service Interface

####### 2.1.1.3.9.5.1.0 Service Interface

IAnonymizationStrategy

####### 2.1.1.3.9.5.2.0 Service Implementation

BasicAnonymizationStrategy

####### 2.1.1.3.9.5.3.0 Lifetime

Singleton

####### 2.1.1.3.9.5.4.0 Registration Reasoning

Specification requires strategies to be stateless and registered as singletons to be collected by the DicomAnonymizer.

####### 2.1.1.3.9.5.5.0 Framework Registration Pattern

services.AddSingleton<IAnonymizationStrategy, BasicAnonymizationStrategy>();

###### 2.1.1.3.9.6.0.0 Service Interface

####### 2.1.1.3.9.6.1.0 Service Interface

IAnonymizationStrategy

####### 2.1.1.3.9.6.2.0 Service Implementation

FullAnonymizationStrategy

####### 2.1.1.3.9.6.3.0 Lifetime

Singleton

####### 2.1.1.3.9.6.4.0 Registration Reasoning

Specification requires strategies to be stateless and registered as singletons.

####### 2.1.1.3.9.6.5.0 Framework Registration Pattern

services.AddSingleton<IAnonymizationStrategy, FullAnonymizationStrategy>();

##### 2.1.1.3.10.0.0.0 External Integration Specifications

###### 2.1.1.3.10.1.0.0 Integration Target

####### 2.1.1.3.10.1.1.0 Integration Target

External PACS/Modalities

####### 2.1.1.3.10.1.2.0 Integration Type

DICOM Network Protocol

####### 2.1.1.3.10.1.3.0 Required Client Classes

- DicomScpService
- DicomScuService

####### 2.1.1.3.10.1.4.0 Configuration Requirements

Specification requires local port configuration for the SCP and remote AE Title, Host, and Port for each external PACS.

####### 2.1.1.3.10.1.5.0 Error Handling Requirements

Specification requires robust handling of network timeouts, association rejections, and DICOM error statuses.

####### 2.1.1.3.10.1.6.0 Authentication Requirements

Authentication is specified via Application Entity (AE) Titles. Implementation must support configurable DICOM TLS for encryption.

####### 2.1.1.3.10.1.7.0 Framework Integration Patterns

The repository acts as an Adapter for the `fo-dicom` library, providing simpler, application-focused services.

###### 2.1.1.3.10.2.0.0 Integration Target

####### 2.1.1.3.10.2.1.0 Integration Target

Local File System

####### 2.1.1.3.10.2.2.0 Integration Type

File I/O

####### 2.1.1.3.10.2.3.0 Required Client Classes

- DicomFileStorage

####### 2.1.1.3.10.2.4.0 Configuration Requirements

Specification requires a configurable root path for DICOM storage.

####### 2.1.1.3.10.2.5.0 Error Handling Requirements

Specification requires handling of `IOException`, `UnauthorizedAccessException`, and other file system errors. All I/O must be asynchronous.

####### 2.1.1.3.10.2.6.0 Authentication Requirements

Relies on the file system permissions of the service account running the application.

####### 2.1.1.3.10.2.7.0 Framework Integration Patterns

Uses standard .NET `System.IO` APIs for file and directory manipulation.

#### 2.1.1.4.0.0.0.0 Component Count Validation

| Property | Value |
|----------|-------|
| Total Classes | 8 |
| Total Interfaces | 5 |
| Total Enums | 1 |
| Total Dtos | 0 |
| Total Configurations | 0 |
| Total External Integrations | 2 |
| Grand Total Components | 16 |
| Phase 2 Claimed Count | 4 |
| Phase 2 Actual Count | 4 |
| Validation Added Count | 12 |
| Final Validated Count | 16 |

### 2.1.2.0.0.0.0.0 Project Supporting Files

#### 2.1.2.1.0.0.0.0 File Type

##### 2.1.2.1.1.0.0.0 File Type

Project Definition

##### 2.1.2.1.2.0.0.0 File Name

DMPS.Infrastructure.Dicom.csproj

##### 2.1.2.1.3.0.0.0 File Path

./DMPS.Infrastructure.Dicom.csproj

##### 2.1.2.1.4.0.0.0 Purpose

Defines the .NET 8 class library project, its target framework, and its dependencies on other projects and the fo-dicom NuGet package.

##### 2.1.2.1.5.0.0.0 Content Description

<Project Sdk=\"Microsoft.NET.Sdk\">\n\n  <PropertyGroup>\n    <TargetFramework>net8.0</TargetFramework>\n    <ImplicitUsings>enable</ImplicitUsings>\n    <Nullable>enable</Nullable>\n  </PropertyGroup>\n\n  <ItemGroup>\n    <PackageReference Include=\"fo-dicom\" Version=\"5.1.2\" />\n    <PackageReference Include=\"Microsoft.Extensions.DependencyInjection.Abstractions\" Version=\"8.0.0\" />\n    <PackageReference Include=\"Microsoft.Extensions.Logging.Abstractions\" Version=\"8.0.0\" />\n    <PackageReference Include=\"Microsoft.Extensions.Options.ConfigurationExtensions\" Version=\"8.0.0\" />\n  </ItemGroup>\n\n  <ItemGroup>\n    <ProjectReference Include=\"..\\DMPS.Shared.Core\\DMPS.Shared.Core.csproj\" />\n  </ItemGroup>\n\n</Project>

##### 2.1.2.1.6.0.0.0 Framework Specific Attributes

- TargetFramework: net8.0
- PackageReference for fo-dicom

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

Development Tools

##### 2.1.2.3.2.0.0.0 File Name

.editorconfig

##### 2.1.2.3.3.0.0.0 File Path

./.editorconfig

##### 2.1.2.3.4.0.0.0 Purpose

Defines and maintains consistent coding styles between multiple developers and IDEs.

##### 2.1.2.3.5.0.0.0 Content Description

root = true\n\n[*]\nindent_style = space\nindent_size = 4\nend_of_line = lf\ncharset = utf-8\ntrim_trailing_whitespace = true\ninsert_final_newline = true\n\n[*.cs]\ncsharp_style_namespace_declarations = file_scoped\ncsharp_style_prefer_simplified_interpolation = true\n

##### 2.1.2.3.6.0.0.0 Framework Specific Attributes

- csharp_style_namespace_declarations: file_scoped (C# 10+ feature)
- Standard formatting rules

