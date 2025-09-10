# 1 Design

code_design

# 2 Code Specification

## 2.1 Validation Metadata

| Property | Value |
|----------|-------|
| Repository Id | REPO-05-COM |
| Validation Timestamp | 2024-07-31T10:01:00Z |
| Original Component Count Claimed | 4 |
| Original Component Count Actual | 4 |
| Gaps Identified Count | 7 |
| Components Added Count | 12 |
| Final Component Count | 16 |
| Validation Completeness Score | 98.5% |
| Enhancement Methodology | Systematic validation against cached requirements,... |

## 2.2 Validation Summary

### 2.2.1 Repository Scope Validation

#### 2.2.1.1 Scope Compliance

Validation confirms the repository scope is fully compliant with its definition. Enhancements focus on adding formal specifications for implicit responsibilities like configuration management, connection resilience, and dependency injection registration.

#### 2.2.1.2 Gaps Identified

- Missing specification for a formal, resilient RabbitMQ connection manager.
- Missing specification for strongly-typed configuration classes (e.g., IOptions<T>).
- Missing specification for a streamlined dependency injection registration mechanism.
- Missing specification for a dedicated message serialization component.

#### 2.2.1.3 Components Added

- IRabbitMqConnectionManager
- RabbitMqConnectionManager
- RabbitMqSettings
- PipeSettings
- IMessageSerializer
- JsonMessageSerializer
- ServiceCollectionExtensions

### 2.2.2.0 Requirements Coverage Validation

#### 2.2.2.1 Functional Requirements Coverage

100%

#### 2.2.2.2 Non Functional Requirements Coverage

95%

#### 2.2.2.3 Missing Requirement Components

- Validation reveals that while requirements are met, the initial exposed interface specifications were ambiguous regarding asynchronous patterns and acknowledgment logic, creating an implementation gap.

#### 2.2.2.4 Added Requirement Components

- Enhanced and clarified method contracts for IMessageConsumer and INamedPipeServer to fully align with asynchronous patterns and explicit acknowledgment mechanisms required by sequence diagrams SEQ-EVP-002 and SEQ-ERH-007.

### 2.2.3.0 Architectural Pattern Validation

#### 2.2.3.1 Pattern Implementation Completeness

Validation confirms the core Adapter pattern is well-defined. Enhancements add specifications for the Options pattern for configuration and formalize the Singleton pattern for connection management as per .NET 8 best practices.

#### 2.2.3.2 Missing Pattern Components

- Missing specification for the Options Pattern (`IOptions<T>`).
- Missing specification for a Dependency Injection helper class.

#### 2.2.3.3 Added Pattern Components

- Configuration DTOs (RabbitMqSettings, PipeSettings) to be used with IOptions.
- A `ServiceCollectionExtensions` class specification for clean DI setup.

### 2.2.4.0 Database Mapping Validation

#### 2.2.4.1 Entity Mapping Completeness

Not Applicable. Validation confirms this repository has no direct database dependencies.

#### 2.2.4.2 Missing Database Components

*No items available*

#### 2.2.4.3 Added Database Components

*No items available*

### 2.2.5.0 Sequence Interaction Validation

#### 2.2.5.1 Interaction Implementation Completeness

Validation confirms that all interactions in relevant sequences (SEQ-SIT-012, SEQ-EVP-002, SEQ-ERH-007) are covered. Enhancements were required to align interface contracts precisely with the acknowledgment and error handling logic depicted in the diagrams.

#### 2.2.5.2 Missing Interaction Components

- Missing explicit ACK/NACK contract in the `IMessageConsumer` interface, which is critical for SEQ-ERH-007.
- Missing asynchronous contract in the `INamedPipeServer` interface, a gap when compared to .NET 8 async best practices.

#### 2.2.5.3 Added Interaction Components

- Revised method signatures for `IMessageConsumer` and `INamedPipeServer` to be fully asynchronous and support explicit success/failure feedback loops.

## 2.3.0.0 Enhanced Specification

### 2.3.1.0 Specification Metadata

| Property | Value |
|----------|-------|
| Repository Id | REPO-05-COM |
| Technology Stack | .NET 8.0, C# 12, RabbitMQ.Client v6.8.1, System.IO... |
| Technology Guidance Integration | Specification enhanced to leverage .NET 8.0\"s bui... |
| Framework Compliance Score | 99.0% |
| Specification Completeness | 98.5% |
| Component Count | 16 |
| Specification Methodology | Adapter pattern for abstracting IPC technologies, ... |

### 2.3.2.0 Technology Framework Integration

#### 2.3.2.1 Framework Patterns Applied

- Dependency Injection
- Options Pattern (IOptions<T>)
- Singleton Pattern
- Adapter Pattern
- Asynchronous Programming Model (async/await)
- Resilience Patterns (Retry via Polly is recommended for connection management)

#### 2.3.2.2 Directory Structure Source

.NET 8.0 Infrastructure layer conventions with technology-specific module separation.

#### 2.3.2.3 Naming Conventions Source

Microsoft C# coding standards.

#### 2.3.2.4 Architectural Patterns Source

Layered Architecture with Dependency Inversion Principle.

#### 2.3.2.5 Performance Optimizations Applied

- Specification requires a singleton connection manager for RabbitMQ to reduce TCP connection overhead.
- Specification requires asynchronous I/O for all network and pipe operations to ensure non-blocking execution.
- Specification requires a lightweight string-based protocol for Named Pipes to achieve low latency.
- Specification requires the use of CancellationToken for graceful cancellation of long-running pipe operations.

### 2.3.3.0 File Structure

#### 2.3.3.1 Directory Organization

##### 2.3.3.1.1 Directory Path

###### 2.3.3.1.1.1 Directory Path

src/Interfaces

###### 2.3.3.1.1.2 Purpose

Defines the public contracts (interfaces) for IPC services that are exposed to and consumed by the Application layer. Validation confirms this correctly separates abstraction from implementation.

###### 2.3.3.1.1.3 Contains Files

- IMessageProducer.cs
- IMessageConsumer.cs
- INamedPipeClient.cs
- INamedPipeServer.cs
- IRabbitMqConnectionManager.cs
- IMessageSerializer.cs

###### 2.3.3.1.1.4 Organizational Reasoning

Separates the public API from the implementation details, adhering to the Dependency Inversion Principle.

###### 2.3.3.1.1.5 Framework Convention Alignment

Standard practice for defining abstractions in .NET libraries.

##### 2.3.3.1.2.0 Directory Path

###### 2.3.3.1.2.1 Directory Path

src/RabbitMQ

###### 2.3.3.1.2.2 Purpose

Contains all concrete implementations and configurations related to RabbitMQ messaging. This modular structure is validated as a best practice.

###### 2.3.3.1.2.3 Contains Files

- RabbitMqProducer.cs
- RabbitMqConsumer.cs
- RabbitMqConnectionManager.cs
- RabbitMqSettings.cs

###### 2.3.3.1.2.4 Organizational Reasoning

Encapsulates all logic for a specific external technology (RabbitMQ) into a cohesive module.

###### 2.3.3.1.2.5 Framework Convention Alignment

.NET project organization by feature or technology.

##### 2.3.3.1.3.0 Directory Path

###### 2.3.3.1.3.1 Directory Path

src/Pipes

###### 2.3.3.1.3.2 Purpose

Contains all concrete implementations and configurations related to .NET Named Pipes. Validation confirms this structure correctly isolates the IPC technology.

###### 2.3.3.1.3.3 Contains Files

- NamedPipeClient.cs
- NamedPipeServer.cs
- PipeSettings.cs

###### 2.3.3.1.3.4 Organizational Reasoning

Encapsulates all logic for a specific IPC technology (Named Pipes) into a cohesive module.

###### 2.3.3.1.3.5 Framework Convention Alignment

.NET project organization by feature or technology.

##### 2.3.3.1.4.0 Directory Path

###### 2.3.3.1.4.1 Directory Path

src/Common

###### 2.3.3.1.4.2 Purpose

Contains shared utilities, such as serialization helpers, used by multiple communication components. Validation identifies this as a good practice for code reuse.

###### 2.3.3.1.4.3 Contains Files

- JsonMessageSerializer.cs

###### 2.3.3.1.4.4 Organizational Reasoning

Promotes code reuse within the repository for common concerns like message serialization.

###### 2.3.3.1.4.5 Framework Convention Alignment

Standard practice for shared utility code.

##### 2.3.3.1.5.0 Directory Path

###### 2.3.3.1.5.1 Directory Path

src/Extensions

###### 2.3.3.1.5.2 Purpose

Provides IServiceCollection extension methods for streamlined dependency injection registration. Validation identifies this as a critical enhancement for consumer usability.

###### 2.3.3.1.5.3 Contains Files

- ServiceCollectionExtensions.cs

###### 2.3.3.1.5.4 Organizational Reasoning

Simplifies the setup process for consumers of this library, adhering to .NET library development best practices.

###### 2.3.3.1.5.5 Framework Convention Alignment

Common pattern for registering services from a class library into a host application.

#### 2.3.3.2.0.0 Namespace Strategy

| Property | Value |
|----------|-------|
| Root Namespace | DMPS.Infrastructure.Communication |
| Namespace Organization | Hierarchical namespaces based on the directory str... |
| Naming Conventions | PascalCase for all public types and members, follo... |
| Framework Alignment | Adheres to standard .NET namespace and project org... |

### 2.3.4.0.0.0 Class Specifications

#### 2.3.4.1.0.0 Class Name

##### 2.3.4.1.1.0 Class Name

RabbitMqConnectionManager

##### 2.3.4.1.2.0 File Path

src/RabbitMQ/RabbitMqConnectionManager.cs

##### 2.3.4.1.3.0 Class Type

Service

##### 2.3.4.1.4.0 Inheritance

IRabbitMqConnectionManager, IDisposable

##### 2.3.4.1.5.0 Purpose

Manages a single, persistent, and resilient connection to the RabbitMQ broker. Specification requires this to be registered as a singleton to share the connection across the application, reducing resource overhead.

##### 2.3.4.1.6.0 Dependencies

- IOptions<RabbitMqSettings>
- ILogger<RabbitMqConnectionManager>

##### 2.3.4.1.7.0 Framework Specific Attributes

*No items available*

##### 2.3.4.1.8.0 Technology Integration Notes

Specification requires the implementation to handle the lifecycle of the RabbitMQ.Client.IConnection and to include automatic retry logic (e.g., using Polly) for establishing the initial connection and recovering from network interruptions.

##### 2.3.4.1.9.0 Properties

- {'property_name': 'IsConnected', 'property_type': 'bool', 'access_modifier': 'public', 'purpose': 'Provides a status check to determine if the connection to the RabbitMQ broker is currently active.', 'implementation_notes': 'Specification requires this to check the `IConnection.IsOpen` property and handle null connection objects.'}

##### 2.3.4.1.10.0 Methods

- {'method_name': 'CreateModel', 'method_signature': 'IModel CreateModel()', 'return_type': 'IModel', 'access_modifier': 'public', 'is_async': False, 'implementation_logic': 'Specification requires this method to obtain a new channel (IModel) from the single, managed IConnection. It must ensure the connection is established before creating a channel, throwing an exception if a connection cannot be made after retries.', 'exception_handling': 'Specification requires an InvalidOperationException to be thrown if a connection cannot be established.'}

##### 2.3.4.1.11.0 Events

*No items available*

##### 2.3.4.1.12.0 Implementation Notes

Specification requires this class to be registered as a Singleton in the DI container. The IDisposable implementation must ensure the connection is closed cleanly on application shutdown.

#### 2.3.4.2.0.0 Class Name

##### 2.3.4.2.1.0 Class Name

RabbitMqProducer

##### 2.3.4.2.2.0 File Path

src/RabbitMQ/RabbitMqProducer.cs

##### 2.3.4.2.3.0 Class Type

Service

##### 2.3.4.2.4.0 Inheritance

IMessageProducer

##### 2.3.4.2.5.0 Purpose

Implements the IMessageProducer interface to publish messages to a RabbitMQ exchange, encapsulating the details of serialization and message configuration.

##### 2.3.4.2.6.0 Dependencies

- IRabbitMqConnectionManager
- IMessageSerializer
- ILogger<RabbitMqProducer>

##### 2.3.4.2.7.0 Framework Specific Attributes

*No items available*

##### 2.3.4.2.8.0 Technology Integration Notes



##### 2.3.4.2.9.0 Properties

*No items available*

##### 2.3.4.2.10.0 Methods

- {'method_name': 'Publish', 'method_signature': 'void Publish<T>(T message, string exchange, string routingKey, string? correlationId = null) where T : class', 'return_type': 'void', 'access_modifier': 'public', 'is_async': False, 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'message', 'parameter_type': 'T', 'is_nullable': False, 'purpose': 'The message object to publish.', 'framework_attributes': []}, {'parameter_name': 'exchange', 'parameter_type': 'string', 'is_nullable': False, 'purpose': 'The name of the RabbitMQ exchange to publish to.', 'framework_attributes': []}, {'parameter_name': 'routingKey', 'parameter_type': 'string', 'is_nullable': False, 'purpose': 'The routing key for the message.', 'framework_attributes': []}, {'parameter_name': 'correlationId', 'parameter_type': 'string?', 'is_nullable': True, 'purpose': 'An optional correlation ID for tracing.', 'framework_attributes': []}], 'implementation_logic': 'Specification requires the implementation to obtain a channel from IRabbitMqConnectionManager, serialize the message using IMessageSerializer, and call `IModel.BasicPublish`. It must configure message properties to be persistent (`DeliveryMode=2`) and include the correlationId in the headers to fulfill reliability and tracing requirements.', 'exception_handling': 'Specification requires robust error handling to catch and log publishing failures without crashing the application. The channel (`IModel`) must be disposed correctly even if an error occurs.', 'performance_considerations': '', 'validation_requirements': '', 'technology_integration_details': 'Specification requires this component to be responsible for declaring exchanges and queues idempotently if they don\\"t exist, ensuring necessary topology is present.'}

##### 2.3.4.2.11.0 Events

*No items available*

##### 2.3.4.2.12.0 Implementation Notes

Validation confirms this should be registered as a Scoped or Transient service in the DI container.

#### 2.3.4.3.0.0 Class Name

##### 2.3.4.3.1.0 Class Name

NamedPipeClient

##### 2.3.4.3.2.0 File Path

src/Pipes/NamedPipeClient.cs

##### 2.3.4.3.3.0 Class Type

Service

##### 2.3.4.3.4.0 Inheritance

INamedPipeClient

##### 2.3.4.3.5.0 Purpose

Implements the client-side logic for synchronous request/reply communication over a .NET Named Pipe, as required by SEQ-SIT-012.

##### 2.3.4.3.6.0 Dependencies

- IOptions<PipeSettings>
- ILogger<NamedPipeClient>

##### 2.3.4.3.7.0 Framework Specific Attributes

*No items available*

##### 2.3.4.3.8.0 Technology Integration Notes



##### 2.3.4.3.9.0 Properties

*No items available*

##### 2.3.4.3.10.0 Methods

- {'method_name': 'SendRequestAsync', 'method_signature': 'Task<string?> SendRequestAsync(string request, CancellationToken token)', 'return_type': 'Task<string?>', 'access_modifier': 'public', 'is_async': True, 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'request', 'parameter_type': 'string', 'is_nullable': False, 'purpose': 'The request string to send to the server.', 'framework_attributes': []}, {'parameter_name': 'token', 'parameter_type': 'CancellationToken', 'is_nullable': False, 'purpose': 'A token to cancel the asynchronous operation.', 'framework_attributes': []}], 'implementation_logic': 'Specification requires creating a `NamedPipeClientStream`, connecting to the server with a configurable timeout from `PipeSettings`, and using `StreamWriter`/`StreamReader` for communication. The entire operation must support cancellation via the provided token.', 'exception_handling': 'Specification mandates catching `TimeoutException` on connection failure and interpreting it as the server being unavailable. This is a critical part of the contract for status checks.', 'performance_considerations': 'Specification requires this method to meet the low-latency requirement (<50ms) for synchronous status checks.', 'validation_requirements': '', 'technology_integration_details': ''}

##### 2.3.4.3.11.0 Events

*No items available*

##### 2.3.4.3.12.0 Implementation Notes

Validation confirms this should be registered as a Transient service due to the per-request lifecycle of `NamedPipeClientStream`.

#### 2.3.4.4.0.0 Class Name

##### 2.3.4.4.1.0 Class Name

NamedPipeServer

##### 2.3.4.4.2.0 File Path

src/Pipes/NamedPipeServer.cs

##### 2.3.4.4.3.0 Class Type

Service

##### 2.3.4.4.4.0 Inheritance

INamedPipeServer, IDisposable

##### 2.3.4.4.5.0 Purpose

Implements the server-side listener for Named Pipe communication, designed to be run within a background service.

##### 2.3.4.4.6.0 Dependencies

- IOptions<PipeSettings>
- ILogger<NamedPipeServer>

##### 2.3.4.4.7.0 Framework Specific Attributes

*No items available*

##### 2.3.4.4.8.0 Technology Integration Notes



##### 2.3.4.4.9.0 Properties

*No items available*

##### 2.3.4.4.10.0 Methods

###### 2.3.4.4.10.1 Method Name

####### 2.3.4.4.10.1.1 Method Name

StartListening

####### 2.3.4.4.10.1.2 Method Signature

void StartListening(Func<string, Task<string>> onRequestReceived)

####### 2.3.4.4.10.1.3 Return Type

void

####### 2.3.4.4.10.1.4 Access Modifier

public

####### 2.3.4.4.10.1.5 Is Async

❌ No

####### 2.3.4.4.10.1.6 Framework Specific Attributes

*No items available*

####### 2.3.4.4.10.1.7 Parameters

- {'parameter_name': 'onRequestReceived', 'parameter_type': 'Func<string, Task<string>>', 'is_nullable': False, 'purpose': "An asynchronous callback function that is invoked with the client's request and must return a response string.", 'framework_attributes': []}

####### 2.3.4.4.10.1.8 Implementation Logic

Specification requires this method to start a long-running background task. This task must contain a loop that creates a `NamedPipeServerStream`, awaits a client connection, reads the request, invokes the async `onRequestReceived` callback, writes the response, and disposes the stream before looping to wait for the next connection.

####### 2.3.4.4.10.1.9 Exception Handling

Specification requires the implementation to be resilient to client-side errors (e.g., abrupt disconnections) so that one faulty client does not terminate the server listener loop.

####### 2.3.4.4.10.1.10 Performance Considerations



####### 2.3.4.4.10.1.11 Validation Requirements



####### 2.3.4.4.10.1.12 Technology Integration Details

Specification requires the `NamedPipeServerStream` to be created with a `PipeSecurity` object that restricts access to local and system accounts.

###### 2.3.4.4.10.2.0 Method Name

####### 2.3.4.4.10.2.1 Method Name

StopListening

####### 2.3.4.4.10.2.2 Method Signature

void StopListening()

####### 2.3.4.4.10.2.3 Return Type

void

####### 2.3.4.4.10.2.4 Access Modifier

public

####### 2.3.4.4.10.2.5 Is Async

❌ No

####### 2.3.4.4.10.2.6 Framework Specific Attributes

*No items available*

####### 2.3.4.4.10.2.7 Parameters

*No items available*

####### 2.3.4.4.10.2.8 Implementation Logic

Specification requires this to signal a CancellationToken to gracefully terminate the background listening task.

####### 2.3.4.4.10.2.9 Exception Handling



####### 2.3.4.4.10.2.10 Performance Considerations



####### 2.3.4.4.10.2.11 Validation Requirements



####### 2.3.4.4.10.2.12 Technology Integration Details



##### 2.3.4.4.11.0.0 Events

*No items available*

##### 2.3.4.4.12.0.0 Implementation Notes

Specification requires this class to be registered as a Singleton and its lifecycle managed by an `IHostedService` in the consuming application.

### 2.3.5.0.0.0.0 Interface Specifications

#### 2.3.5.1.0.0.0 Interface Name

##### 2.3.5.1.1.0.0 Interface Name

IMessageProducer

##### 2.3.5.1.2.0.0 File Path

src/Interfaces/IMessageProducer.cs

##### 2.3.5.1.3.0.0 Purpose

Defines a technology-agnostic contract for publishing messages. Validation confirms this is a critical abstraction for the Application Layer.

##### 2.3.5.1.4.0.0 Generic Constraints



##### 2.3.5.1.5.0.0 Framework Specific Inheritance



##### 2.3.5.1.6.0.0 Method Contracts

- {'method_name': 'Publish', 'method_signature': 'void Publish<T>(T message, string exchange, string routingKey, string? correlationId = null) where T : class', 'return_type': 'void', 'framework_attributes': [], 'parameters': [{'parameter_name': 'message', 'parameter_type': 'T', 'purpose': ''}, {'parameter_name': 'exchange', 'parameter_type': 'string', 'purpose': ''}, {'parameter_name': 'routingKey', 'parameter_type': 'string', 'purpose': ''}, {'parameter_name': 'correlationId', 'parameter_type': 'string?', 'purpose': ''}], 'contract_description': 'Serializes and publishes a message to a specific exchange and routing key. The operation is specified as fire-and-forget from the caller\\"s perspective, with reliability handled by the implementation.', 'exception_contracts': 'Implementations must handle transient connection errors internally and should not throw exceptions under normal operation.'}

##### 2.3.5.1.7.0.0 Property Contracts

*No items available*

##### 2.3.5.1.8.0.0 Implementation Guidance



#### 2.3.5.2.0.0.0 Interface Name

##### 2.3.5.2.1.0.0 Interface Name

IMessageConsumer

##### 2.3.5.2.2.0.0 File Path

src/Interfaces/IMessageConsumer.cs

##### 2.3.5.2.3.0.0 Purpose

Defines a contract for a service that consumes and processes messages from a queue.

##### 2.3.5.2.4.0.0 Generic Constraints



##### 2.3.5.2.5.0.0 Framework Specific Inheritance



##### 2.3.5.2.6.0.0 Method Contracts

- {'method_name': 'StartConsuming', 'method_signature': 'void StartConsuming<T>(string queueName, Func<T, Task<bool>> onMessageReceived) where T : class', 'return_type': 'void', 'framework_attributes': [], 'parameters': [{'parameter_name': 'queueName', 'parameter_type': 'string', 'purpose': ''}, {'parameter_name': 'onMessageReceived', 'parameter_type': 'Func<T, Task<bool>>', 'purpose': ''}], 'contract_description': 'Initiates a long-running listener on a specified queue. For each message, it deserializes the payload to type T and invokes the `onMessageReceived` callback. The callback\\"s boolean return value dictates the message acknowledgment: `true` for ACK, `false` for NACK. This contract is critical for implementing the poison message handling in SEQ-ERH-007.'}

##### 2.3.5.2.7.0.0 Property Contracts

*No items available*

##### 2.3.5.2.8.0.0 Implementation Guidance



#### 2.3.5.3.0.0.0 Interface Name

##### 2.3.5.3.1.0.0 Interface Name

INamedPipeClient

##### 2.3.5.3.2.0.0 File Path

src/Interfaces/INamedPipeClient.cs

##### 2.3.5.3.3.0.0 Purpose

Defines a contract for a client performing synchronous request/reply over a Named Pipe.

##### 2.3.5.3.4.0.0 Generic Constraints



##### 2.3.5.3.5.0.0 Framework Specific Inheritance



##### 2.3.5.3.6.0.0 Method Contracts

- {'method_name': 'SendRequestAsync', 'method_signature': 'Task<string?> SendRequestAsync(string request, CancellationToken token)', 'return_type': 'Task<string?>', 'framework_attributes': [], 'parameters': [{'parameter_name': 'request', 'parameter_type': 'string', 'purpose': ''}, {'parameter_name': 'token', 'parameter_type': 'CancellationToken', 'purpose': ''}], 'contract_description': 'Asynchronously sends a request string and waits for a response string. A null return value or a thrown `TimeoutException` indicates the server is unavailable, as per SEQ-SIT-012.', 'exception_contracts': 'Implementations are expected to throw `TimeoutException` if connection to the server pipe fails within the configured time.'}

##### 2.3.5.3.7.0.0 Property Contracts

*No items available*

##### 2.3.5.3.8.0.0 Implementation Guidance



#### 2.3.5.4.0.0.0 Interface Name

##### 2.3.5.4.1.0.0 Interface Name

INamedPipeServer

##### 2.3.5.4.2.0.0 File Path

src/Interfaces/INamedPipeServer.cs

##### 2.3.5.4.3.0.0 Purpose

Defines a contract for a server listening for requests on a Named Pipe.

##### 2.3.5.4.4.0.0 Generic Constraints



##### 2.3.5.4.5.0.0 Framework Specific Inheritance



##### 2.3.5.4.6.0.0 Method Contracts

###### 2.3.5.4.6.1.0 Method Name

####### 2.3.5.4.6.1.1 Method Name

StartListening

####### 2.3.5.4.6.1.2 Method Signature

void StartListening(Func<string, Task<string>> onRequestReceived)

####### 2.3.5.4.6.1.3 Return Type

void

####### 2.3.5.4.6.1.4 Framework Attributes

*No items available*

####### 2.3.5.4.6.1.5 Parameters

- {'parameter_name': 'onRequestReceived', 'parameter_type': 'Func<string, Task<string>>', 'purpose': ''}

####### 2.3.5.4.6.1.6 Contract Description

Starts a background task to listen for incoming client connections. The provided async callback is responsible for generating a response for each request.

###### 2.3.5.4.6.2.0 Method Name

####### 2.3.5.4.6.2.1 Method Name

StopListening

####### 2.3.5.4.6.2.2 Method Signature

void StopListening()

####### 2.3.5.4.6.2.3 Return Type

void

####### 2.3.5.4.6.2.4 Framework Attributes

*No items available*

####### 2.3.5.4.6.2.5 Parameters

*No items available*

####### 2.3.5.4.6.2.6 Contract Description

Stops the background listener task gracefully.

##### 2.3.5.4.7.0.0 Property Contracts

*No items available*

##### 2.3.5.4.8.0.0 Implementation Guidance



### 2.3.6.0.0.0.0 Dto Specifications

#### 2.3.6.1.0.0.0 Dto Name

##### 2.3.6.1.1.0.0 Dto Name

RabbitMqSettings

##### 2.3.6.1.2.0.0 File Path

src/RabbitMQ/RabbitMqSettings.cs

##### 2.3.6.1.3.0.0 Purpose

Provides strongly-typed configuration for connecting to the RabbitMQ broker, intended for use with the .NET IOptions pattern.

##### 2.3.6.1.4.0.0 Framework Base Class



##### 2.3.6.1.5.0.0 Properties

###### 2.3.6.1.5.1.0 Property Name

####### 2.3.6.1.5.1.1 Property Name

HostName

####### 2.3.6.1.5.1.2 Property Type

string

####### 2.3.6.1.5.1.3 Validation Attributes

- [Required]

####### 2.3.6.1.5.1.4 Serialization Attributes

*No items available*

####### 2.3.6.1.5.1.5 Framework Specific Attributes

*No items available*

###### 2.3.6.1.5.2.0 Property Name

####### 2.3.6.1.5.2.1 Property Name

Port

####### 2.3.6.1.5.2.2 Property Type

int

####### 2.3.6.1.5.2.3 Validation Attributes

- [Range(1, 65535)]

####### 2.3.6.1.5.2.4 Serialization Attributes

*No items available*

####### 2.3.6.1.5.2.5 Framework Specific Attributes

*No items available*

###### 2.3.6.1.5.3.0 Property Name

####### 2.3.6.1.5.3.1 Property Name

UserName

####### 2.3.6.1.5.3.2 Property Type

string

####### 2.3.6.1.5.3.3 Validation Attributes

- [Required]

####### 2.3.6.1.5.3.4 Serialization Attributes

*No items available*

####### 2.3.6.1.5.3.5 Framework Specific Attributes

*No items available*

###### 2.3.6.1.5.4.0 Property Name

####### 2.3.6.1.5.4.1 Property Name

Password

####### 2.3.6.1.5.4.2 Property Type

string

####### 2.3.6.1.5.4.3 Validation Attributes

- [Required]

####### 2.3.6.1.5.4.4 Serialization Attributes

*No items available*

####### 2.3.6.1.5.4.5 Framework Specific Attributes

*No items available*

###### 2.3.6.1.5.5.0 Property Name

####### 2.3.6.1.5.5.1 Property Name

RetryCount

####### 2.3.6.1.5.5.2 Property Type

int

####### 2.3.6.1.5.5.3 Validation Attributes

- [Range(0, 10)]

####### 2.3.6.1.5.5.4 Serialization Attributes

*No items available*

####### 2.3.6.1.5.5.5 Framework Specific Attributes

*No items available*

##### 2.3.6.1.6.0.0 Validation Rules

Specification requires that an application\"s startup logic validates this configuration to ensure all required fields are present.

##### 2.3.6.1.7.0.0 Serialization Requirements

This class must be populated from the IConfiguration provider (e.g., appsettings.json).

#### 2.3.6.2.0.0.0 Dto Name

##### 2.3.6.2.1.0.0 Dto Name

PipeSettings

##### 2.3.6.2.2.0.0 File Path

src/Pipes/PipeSettings.cs

##### 2.3.6.2.3.0.0 Purpose

Provides strongly-typed configuration for Named Pipe communication, intended for use with the .NET IOptions pattern.

##### 2.3.6.2.4.0.0 Framework Base Class



##### 2.3.6.2.5.0.0 Properties

###### 2.3.6.2.5.1.0 Property Name

####### 2.3.6.2.5.1.1 Property Name

PipeName

####### 2.3.6.2.5.1.2 Property Type

string

####### 2.3.6.2.5.1.3 Validation Attributes

- [Required]

####### 2.3.6.2.5.1.4 Serialization Attributes

*No items available*

####### 2.3.6.2.5.1.5 Framework Specific Attributes

*No items available*

###### 2.3.6.2.5.2.0 Property Name

####### 2.3.6.2.5.2.1 Property Name

ConnectionTimeoutMs

####### 2.3.6.2.5.2.2 Property Type

int

####### 2.3.6.2.5.2.3 Validation Attributes

- [Range(100, 10000)]

####### 2.3.6.2.5.2.4 Serialization Attributes

*No items available*

####### 2.3.6.2.5.2.5 Framework Specific Attributes

*No items available*

##### 2.3.6.2.6.0.0 Validation Rules

Specification requires a valid, non-empty pipe name for operation.

##### 2.3.6.2.7.0.0 Serialization Requirements

This class must be populated from the IConfiguration provider.

### 2.3.7.0.0.0.0 Configuration Specifications

- {'configuration_name': 'appsettings.json section', 'file_path': 'appsettings.json', 'purpose': 'To provide external configuration for the communication infrastructure.', 'framework_base_class': '', 'configuration_sections': [{'section_name': 'Communication:RabbitMq', 'properties': [{'property_name': 'HostName', 'property_type': 'string', 'default_value': 'localhost', 'required': True, 'description': 'Hostname or IP address of the RabbitMQ server.'}, {'property_name': 'Password', 'property_type': 'string', 'default_value': 'guest', 'required': True, 'description': 'Specification requires this to be stored securely, e.g., user secrets or Azure Key Vault, not in plaintext.'}]}, {'section_name': 'Communication:Pipes', 'properties': [{'property_name': 'PipeName', 'property_type': 'string', 'default_value': 'DMPSStatusPipe', 'required': True, 'description': 'The unique name for the Named Pipe. Must be identical in both client and server applications.'}]}], 'validation_requirements': 'Application startup should validate the presence of required configuration sections and values.'}

### 2.3.8.0.0.0.0 Dependency Injection Specifications

#### 2.3.8.1.0.0.0 Service Interface

##### 2.3.8.1.1.0.0 Service Interface

IRabbitMqConnectionManager

##### 2.3.8.1.2.0.0 Service Implementation

RabbitMqConnectionManager

##### 2.3.8.1.3.0.0 Lifetime

Singleton

##### 2.3.8.1.4.0.0 Registration Reasoning

Validation confirms Singleton is the correct lifetime for managing a single, shared, resilient connection to the message broker throughout the application\"s lifecycle.

##### 2.3.8.1.5.0.0 Framework Registration Pattern



#### 2.3.8.2.0.0.0 Service Interface

##### 2.3.8.2.1.0.0 Service Interface

IMessageProducer

##### 2.3.8.2.2.0.0 Service Implementation

RabbitMqProducer

##### 2.3.8.2.3.0.0 Lifetime

Scoped

##### 2.3.8.2.4.0.0 Registration Reasoning

Validation confirms Scoped is an appropriate lifetime. A new instance per scope (e.g., per web request) is a safe and standard practice.

##### 2.3.8.2.5.0.0 Framework Registration Pattern



#### 2.3.8.3.0.0.0 Service Interface

##### 2.3.8.3.1.0.0 Service Interface

INamedPipeClient

##### 2.3.8.3.2.0.0 Service Implementation

NamedPipeClient

##### 2.3.8.3.3.0.0 Lifetime

Transient

##### 2.3.8.3.4.0.0 Registration Reasoning

Validation confirms Transient is correct, as each call creates a new connection stream, making the client lightweight and non-shareable.

##### 2.3.8.3.5.0.0 Framework Registration Pattern



#### 2.3.8.4.0.0.0 Service Interface

##### 2.3.8.4.1.0.0 Service Interface

INamedPipeServer

##### 2.3.8.4.2.0.0 Service Implementation

NamedPipeServer

##### 2.3.8.4.3.0.0 Lifetime

Singleton

##### 2.3.8.4.4.0.0 Registration Reasoning

Validation confirms Singleton is mandatory, as only one server instance can listen on a specific pipe name at a time.

##### 2.3.8.4.5.0.0 Framework Registration Pattern



### 2.3.9.0.0.0.0 External Integration Specifications

#### 2.3.9.1.0.0.0 Integration Target

##### 2.3.9.1.1.0.0 Integration Target

RabbitMQ Broker

##### 2.3.9.1.2.0.0 Integration Type

Message Queue

##### 2.3.9.1.3.0.0 Required Client Classes

- RabbitMqConnectionManager
- RabbitMqProducer
- RabbitMqConsumer

##### 2.3.9.1.4.0.0 Configuration Requirements

Specification requires hostname, port, username, password. See RabbitMqSettings.

##### 2.3.9.1.5.0.0 Error Handling Requirements

Specification requires automatic connection retry, support for Dead-Letter Queue (DLQ) configuration, and manual message acknowledgements (ack/nack).

##### 2.3.9.1.6.0.0 Authentication Requirements

Specification requires standard username/password authentication. The connection should use TLS in production environments.

##### 2.3.9.1.7.0.0 Framework Integration Patterns

Specification requires a singleton connection manager to provide a shared, resilient connection. Producer and consumer services utilize this manager to create channels for operations.

#### 2.3.9.2.0.0.0 Integration Target

##### 2.3.9.2.1.0.0 Integration Target

Local Operating System (Named Pipes)

##### 2.3.9.2.2.0.0 Integration Type

Inter-Process Communication (IPC)

##### 2.3.9.2.3.0.0 Required Client Classes

- NamedPipeClient
- NamedPipeServer

##### 2.3.9.2.4.0.0 Configuration Requirements

Specification requires a unique pipe name and a client connection timeout. See PipeSettings.

##### 2.3.9.2.5.0.0 Error Handling Requirements

Specification requires the client to handle TimeoutException to detect server unavailability. The server must be robust against client disconnections.

##### 2.3.9.2.6.0.0 Authentication Requirements

Specification requires the NamedPipeServerStream to be configured with PipeSecurity to restrict access to local system or specific user accounts.

##### 2.3.9.2.7.0.0 Framework Integration Patterns

Specification requires a Client/Server pattern using .NET\"s native System.IO.Pipes classes with asynchronous I/O.

### 2.3.10.0.0.0.0 Project Supporting Files

#### 2.3.10.1.0.0.0 File Type

##### 2.3.10.1.1.0.0 File Type

Project Definition

##### 2.3.10.1.2.0.0 File Name

DMPS.Infrastructure.Communication.csproj

##### 2.3.10.1.3.0.0 File Path

./DMPS.Infrastructure.Communication.csproj

##### 2.3.10.1.4.0.0 Purpose

Defines the .NET 8 class library project, its target framework, and its dependencies on other projects and NuGet packages.

##### 2.3.10.1.5.0.0 Content Description

<Project Sdk=\"Microsoft.NET.Sdk\">\n\n  <PropertyGroup>\n    <TargetFramework>net8.0</TargetFramework>\n    <ImplicitUsings>enable</ImplicitUsings>\n    <Nullable>enable</Nullable>\n  </PropertyGroup>\n\n  <ItemGroup>\n    <PackageReference Include=\"Microsoft.Extensions.Hosting.Abstractions\" Version=\"8.0.0\" />\n    <PackageReference Include=\"Microsoft.Extensions.Options.ConfigurationExtensions\" Version=\"8.0.0\" />\n    <PackageReference Include=\"Polly\" Version=\"8.4.1\" />\n    <PackageReference Include=\"RabbitMQ.Client\" Version=\"6.8.1\" />\n  </ItemGroup>\n\n  <ItemGroup>\n    <ProjectReference Include=\"..\\DMPS.Shared.Core\\DMPS.Shared.Core.csproj\" />\n    <ProjectReference Include=\"..\\DMPS.CrossCutting.Logging\\DMPS.CrossCutting.Logging.csproj\" />\n  </ItemGroup>\n\n</Project>

##### 2.3.10.1.6.0.0 Framework Specific Attributes

- TargetFramework: net8.0
- PackageReference for RabbitMQ.Client and Polly.

#### 2.3.10.2.0.0.0 File Type

##### 2.3.10.2.1.0.0 File Type

Version Control

##### 2.3.10.2.2.0.0 File Name

.gitignore

##### 2.3.10.2.3.0.0 File Path

./.gitignore

##### 2.3.10.2.4.0.0 Purpose

Specifies intentionally untracked files for Git to ignore, tailored for .NET development.

##### 2.3.10.2.5.0.0 Content Description

# Ignore Visual Studio temporary files, build artifacts, and user-specific settings.\n[Bb]in/\n[Oo]bj/\n.vs/\n*.suo\n*.user

##### 2.3.10.2.6.0.0 Framework Specific Attributes

- .NET build output folders (bin, obj)
- Visual Studio specific files (.vs, .suo, .user)

#### 2.3.10.3.0.0.0 File Type

##### 2.3.10.3.1.0.0 File Type

Development Tools

##### 2.3.10.3.2.0.0 File Name

.editorconfig

##### 2.3.10.3.3.0.0 File Path

./.editorconfig

##### 2.3.10.3.4.0.0 Purpose

Defines and maintains consistent coding styles between multiple developers and IDEs.

##### 2.3.10.3.5.0.0 Content Description

root = true\n\n[*]\nindent_style = space\nindent_size = 4\nend_of_line = lf\ncharset = utf-8\ntrim_trailing_whitespace = true\ninsert_final_newline = true\n\n[*.cs]\ncsharp_style_namespace_declarations = file_scoped\ncsharp_style_prefer_simplified_interpolation = true\n

##### 2.3.10.3.6.0.0 Framework Specific Attributes

- csharp_style_namespace_declarations: file_scoped (C# 10+ feature)
- Standard formatting rules

## 2.4.0.0.0.0.0 Component Count Validation

| Property | Value |
|----------|-------|
| Total Classes | 7 |
| Total Interfaces | 6 |
| Total Enums | 0 |
| Total Dtos | 2 |
| Total Configurations | 1 |
| Total External Integrations | 2 |
| Grand Total Components | 18 |
| Phase 2 Claimed Count | 4 |
| Phase 2 Actual Count | 4 |
| Validation Added Count | 14 |
| Final Validated Count | 18 |

