# 1 Design

code_design

# 2 Code Specification

## 2.1 Validation Metadata

| Property | Value |
|----------|-------|
| Repository Id | REPO-04-SEC |
| Validation Timestamp | 2024-07-31T10:01:00Z |
| Original Component Count Claimed | 1 |
| Original Component Count Actual | 2 |
| Gaps Identified Count | 5 |
| Components Added Count | 5 |
| Final Component Count | 7 |
| Validation Completeness Score | 100.0 |
| Enhancement Methodology | Systematic validation of repository scope, require... |

## 2.2 Validation Summary

### 2.2.1 Repository Scope Validation

#### 2.2.1.1 Scope Compliance

Validation confirms the repository specification aligns with its defined scope of providing BCrypt hashing and secure credential storage. The `must_not_implement` constraints are correctly observed.

#### 2.2.1.2 Gaps Identified

- Missing specification for a strongly-typed configuration class for BCrypt settings (e.g., work factor).
- Missing specification for native P/Invoke helper classes required for Windows Credential Manager integration.
- Missing specification for custom exceptions to abstract native Win32 error codes.

#### 2.2.1.3 Components Added

- BCryptSettings configuration class specification.
- AdvApi32 native interop class specification.
- Custom exception class specifications (e.g., CredentialStoreException).

### 2.2.2.0 Requirements Coverage Validation

#### 2.2.2.1 Functional Requirements Coverage

100.0%

#### 2.2.2.2 Non Functional Requirements Coverage

100.0%

#### 2.2.2.3 Missing Requirement Components

- Validation reveals no missing components for requirement coverage, but existing specifications require enhancement.

#### 2.2.2.4 Added Requirement Components

- Enhanced specification details for exception handling to prevent secret leakage.
- Enhanced specification to mandate constant-time comparison for password verification.
- Specification for dependency injection lifetimes.

### 2.2.3.0 Architectural Pattern Validation

#### 2.2.3.1 Pattern Implementation Completeness

Validation confirms the planned use of the Adapter pattern is appropriate. A specification gap for the .NET Options pattern was identified.

#### 2.2.3.2 Missing Pattern Components

- Missing specification for applying the Options pattern for BCrypt configuration.
- Missing specification for a file structure that clearly separates abstractions from implementations.

#### 2.2.3.3 Added Pattern Components

- BCryptSettings class specification to enable the Options pattern.
- Enhanced file structure specification with `Abstractions` and `Implementations` directories.

### 2.2.4.0 Database Mapping Validation

#### 2.2.4.1 Entity Mapping Completeness

Not Applicable. Validation confirms this repository correctly has no database interaction responsibilities.

#### 2.2.4.2 Missing Database Components

*No items available*

#### 2.2.4.3 Added Database Components

*No items available*

### 2.2.5.0 Sequence Interaction Validation

#### 2.2.5.1 Interaction Implementation Completeness

Validation confirms all interactions in referenced sequence diagrams are covered by the exposed interfaces. However, the specifications for error handling and internal implementation logic are incomplete.

#### 2.2.5.2 Missing Interaction Components

- Missing specification for handling exceptions from the underlying BCrypt library during verification.
- Missing specification detailing the P/Invoke calls for Windows Credential Manager interaction.
- Missing specification for Dependency Injection registration of the provided services.

#### 2.2.5.3 Added Interaction Components

- Enhanced method specifications with detailed exception handling strategies.
- AdvApi32 class specification detailing required P/Invoke signatures.
- Dependency Injection specifications for all public services.

## 2.3.0.0 Enhanced Specification

### 2.3.1.0 Specification Metadata

| Property | Value |
|----------|-------|
| Repository Id | REPO-04-SEC |
| Technology Stack | .NET 8.0, C# 12, BCrypt.Net-Next v4.0.3, Windows C... |
| Technology Guidance Integration | Specification integrates .NET 8.0 best practices f... |
| Framework Compliance Score | 100.0 |
| Specification Completeness | 100.0 |
| Component Count | 7 |
| Specification Methodology | Interface-based service design following SOLID pri... |

### 2.3.2.0 Technology Framework Integration

#### 2.3.2.1 Framework Patterns Applied

- Dependency Injection
- Adapter Pattern
- Options Pattern

#### 2.3.2.2 Directory Structure Source

Clean Architecture principles, separating abstractions from concrete implementations to promote loose coupling and testability.

#### 2.3.2.3 Naming Conventions Source

Microsoft C# coding standards.

#### 2.3.2.4 Architectural Patterns Source

The repository is designed as a cross-cutting concerns library providing foundational, reusable security primitives.

#### 2.3.2.5 Performance Optimizations Applied

- Specification requires Singleton lifetime for stateless services to minimize object allocation.
- Specification mandates the use of BCrypt's constant-time comparison algorithm to prevent timing attacks.

### 2.3.3.0 File Structure

#### 2.3.3.1 Directory Organization

##### 2.3.3.1.1 Directory Path

###### 2.3.3.1.1.1 Directory Path

src/Abstractions

###### 2.3.3.1.1.2 Purpose

Specification requires this directory to define all public contracts (interfaces) for the services provided by this repository. This decouples consumers from the concrete implementations.

###### 2.3.3.1.1.3 Contains Files

- IPasswordHasher.cs
- ISecureCredentialStore.cs

###### 2.3.3.1.1.4 Organizational Reasoning

Follows the Dependency Inversion Principle, allowing higher-level modules to depend on these stable abstractions rather than volatile implementations.

###### 2.3.3.1.1.5 Framework Convention Alignment

Standard practice in .NET for defining contracts in a separate assembly or a distinct top-level folder to enforce clean architecture.

##### 2.3.3.1.2.0 Directory Path

###### 2.3.3.1.2.1 Directory Path

src/Implementations/Hashing

###### 2.3.3.1.2.2 Purpose

Specification requires this directory to contain the concrete implementation of the IPasswordHasher interface, encapsulating the BCrypt.Net-Next library.

###### 2.3.3.1.2.3 Contains Files

- BCryptPasswordHasher.cs

###### 2.3.3.1.2.4 Organizational Reasoning

Isolates the third-party BCrypt library dependency into a specific component, making it easy to replace or update without impacting consumers.

###### 2.3.3.1.2.5 Framework Convention Alignment

Clean Architecture principle: Infrastructure layer concerns are separated into their own cohesive implementation folders.

##### 2.3.3.1.3.0 Directory Path

###### 2.3.3.1.3.1 Directory Path

src/Implementations/CredentialManagement

###### 2.3.3.1.3.2 Purpose

Specification requires this directory to contain the concrete implementation for the ISecureCredentialStore, including all native Windows Credential Manager API interactions.

###### 2.3.3.1.3.3 Contains Files

- WindowsSecureCredentialStore.cs
- Native/AdvApi32.cs
- Exceptions/CredentialStoreException.cs
- Exceptions/CredentialNotFoundException.cs

###### 2.3.3.1.3.4 Organizational Reasoning

Encapsulates platform-specific P/Invoke logic and related concerns (e.g., custom exceptions), abstracting the complexity of native interop from the rest of the application.

###### 2.3.3.1.3.5 Framework Convention Alignment

Separates complex, platform-dependent infrastructure code into a dedicated, cohesive module.

##### 2.3.3.1.4.0 Directory Path

###### 2.3.3.1.4.1 Directory Path

src/Configuration

###### 2.3.3.1.4.2 Purpose

Specification requires this directory to define strongly-typed configuration objects for security-related settings.

###### 2.3.3.1.4.3 Contains Files

- BCryptSettings.cs

###### 2.3.3.1.4.4 Organizational Reasoning

Promotes type-safe access to configuration values using the .NET Options pattern, enhancing maintainability and testability.

###### 2.3.3.1.4.5 Framework Convention Alignment

Standard practice for managing configuration in modern .NET applications.

#### 2.3.3.2.0.0 Namespace Strategy

| Property | Value |
|----------|-------|
| Root Namespace | DMPS.CrossCutting.Security |
| Namespace Organization | Hierarchical by architectural concern: Abstraction... |
| Naming Conventions | PascalCase for all types and public members. |
| Framework Alignment | Follows Microsoft C# namespace guidelines and Clea... |

### 2.3.4.0.0.0 Class Specifications

#### 2.3.4.1.0.0 Class Name

##### 2.3.4.1.1.0 Class Name

BCryptPasswordHasher

##### 2.3.4.1.2.0 File Path

src/Implementations/Hashing/BCryptPasswordHasher.cs

##### 2.3.4.1.3.0 Class Type

Service Implementation

##### 2.3.4.1.4.0 Inheritance

IPasswordHasher

##### 2.3.4.1.5.0 Purpose

Specification requires this class to implement the IPasswordHasher interface using the BCrypt.Net-Next library to provide secure, salted password hashing and verification, fulfilling REQ-082.

##### 2.3.4.1.6.0 Dependencies

- Microsoft.Extensions.Options.IOptions<BCryptSettings>
- BCrypt.Net-Next (NuGet Package)

##### 2.3.4.1.7.0 Framework Specific Attributes

*No items available*

##### 2.3.4.1.8.0 Technology Integration Notes

Specification requires this class to act as an Adapter for the static `BCrypt.Net.BCrypt` class, making its functionality compatible with dependency injection and mockable for testing.

##### 2.3.4.1.9.0 Properties

- {'property_name': 'WorkFactor', 'property_type': 'int', 'access_modifier': 'private readonly', 'purpose': 'Specification requires this property to store the computational cost of the BCrypt algorithm. A higher value is more secure but slower.', 'validation_attributes': [], 'framework_specific_configuration': 'Specification requires injection via the constructor using `IOptions<BCryptSettings>` to allow for configuration-driven tuning without code changes.', 'implementation_notes': 'Specification requires a default value to be specified in the `BCryptSettings` class.'}

##### 2.3.4.1.10.0 Methods

###### 2.3.4.1.10.1 Method Name

####### 2.3.4.1.10.1.1 Method Name

HashPassword

####### 2.3.4.1.10.1.2 Method Signature

string HashPassword(string password)

####### 2.3.4.1.10.1.3 Return Type

string

####### 2.3.4.1.10.1.4 Access Modifier

public

####### 2.3.4.1.10.1.5 Is Async

false

####### 2.3.4.1.10.1.6 Framework Specific Attributes

*No items available*

####### 2.3.4.1.10.1.7 Parameters

- {'parameter_name': 'password', 'parameter_type': 'string', 'is_nullable': 'false', 'purpose': 'The plaintext password to be hashed.', 'framework_attributes': []}

####### 2.3.4.1.10.1.8 Implementation Logic

Specification requires this method to validate that the input password is not null or empty. It must then call `BCrypt.Net.BCrypt.HashPassword` with the provided password and the configured WorkFactor to generate the hash.

####### 2.3.4.1.10.1.9 Exception Handling

Specification requires throwing an `ArgumentNullException` for null or empty password input. It should not catch exceptions from the underlying library, allowing them to propagate.

####### 2.3.4.1.10.1.10 Performance Considerations

This operation is intentionally CPU-intensive. Its performance is directly tied to the configured work factor. Specification acknowledges this is a security feature to mitigate brute-force attacks.

####### 2.3.4.1.10.1.11 Validation Requirements

Specification requires a non-null, non-empty string for the \"password\" parameter.

####### 2.3.4.1.10.1.12 Technology Integration Details

Specification requires a direct invocation of the static `BCrypt.Net.BCrypt.HashPassword` method.

###### 2.3.4.1.10.2.0 Method Name

####### 2.3.4.1.10.2.1 Method Name

VerifyPassword

####### 2.3.4.1.10.2.2 Method Signature

bool VerifyPassword(string password, string hashedPassword)

####### 2.3.4.1.10.2.3 Return Type

bool

####### 2.3.4.1.10.2.4 Access Modifier

public

####### 2.3.4.1.10.2.5 Is Async

false

####### 2.3.4.1.10.2.6 Framework Specific Attributes

*No items available*

####### 2.3.4.1.10.2.7 Parameters

######## 2.3.4.1.10.2.7.1 Parameter Name

######### 2.3.4.1.10.2.7.1.1 Parameter Name

password

######### 2.3.4.1.10.2.7.1.2 Parameter Type

string

######### 2.3.4.1.10.2.7.1.3 Is Nullable

false

######### 2.3.4.1.10.2.7.1.4 Purpose

The plaintext password to compare.

######### 2.3.4.1.10.2.7.1.5 Framework Attributes

*No items available*

######## 2.3.4.1.10.2.7.2.0 Parameter Name

######### 2.3.4.1.10.2.7.2.1 Parameter Name

hashedPassword

######### 2.3.4.1.10.2.7.2.2 Parameter Type

string

######### 2.3.4.1.10.2.7.2.3 Is Nullable

false

######### 2.3.4.1.10.2.7.2.4 Purpose

The stored BCrypt hash to compare against.

######### 2.3.4.1.10.2.7.2.5 Framework Attributes

*No items available*

####### 2.3.4.1.10.2.8.0.0 Implementation Logic

Specification requires this method to validate that inputs are not null or empty. It must call `BCrypt.Net.BCrypt.Verify` with the provided password and hash. The method must wrap the call in a try-catch block to handle potential hashing exceptions from the library (e.g., for a malformed hash).

####### 2.3.4.1.10.2.9.0.0 Exception Handling

Specification requires returning `false` for null/empty inputs. In case of an exception from `BCrypt.Verify` (e.g., `SaltParseException`), the implementation must log the error for diagnostics but return `false` to the caller. It must not re-throw the exception, as this could be used for hash format enumeration attacks.

####### 2.3.4.1.10.2.10.0.0 Performance Considerations

Specification requires use of a constant-time comparison algorithm provided by the BCrypt library to prevent timing attacks.

####### 2.3.4.1.10.2.11.0.0 Validation Requirements

Specification requires non-null, non-empty strings for both parameters.

####### 2.3.4.1.10.2.12.0.0 Technology Integration Details

Specification requires a direct invocation of the static `BCrypt.Net.BCrypt.Verify` method.

##### 2.3.4.1.11.0.0.0.0 Events

*No items available*

##### 2.3.4.1.12.0.0.0.0 Implementation Notes

Specification requires this service to be registered with a Singleton lifetime in the DI container as it is stateless and thread-safe.

#### 2.3.4.2.0.0.0.0.0 Class Name

##### 2.3.4.2.1.0.0.0.0 Class Name

WindowsSecureCredentialStore

##### 2.3.4.2.2.0.0.0.0 File Path

src/Implementations/CredentialManagement/WindowsSecureCredentialStore.cs

##### 2.3.4.2.3.0.0.0.0 Class Type

Service Implementation

##### 2.3.4.2.4.0.0.0.0 Inheritance

ISecureCredentialStore

##### 2.3.4.2.5.0.0.0.0 Purpose

Specification requires this class to implement the ISecureCredentialStore interface using the native Windows Credential Manager API via P/Invoke to securely store and retrieve secrets, fulfilling REQ-084.

##### 2.3.4.2.6.0.0.0.0 Dependencies

- Microsoft.Extensions.Logging.ILogger<WindowsSecureCredentialStore>

##### 2.3.4.2.7.0.0.0.0 Framework Specific Attributes

- System.Runtime.Versioning.SupportedOSPlatform(\"windows\")

##### 2.3.4.2.8.0.0.0.0 Technology Integration Notes

Specification defines this class as platform-specific to Windows. It must encapsulate all P/Invoke logic required to communicate with `advapi32.dll`.

##### 2.3.4.2.9.0.0.0.0 Properties

*No items available*

##### 2.3.4.2.10.0.0.0.0 Methods

###### 2.3.4.2.10.1.0.0.0 Method Name

####### 2.3.4.2.10.1.1.0.0 Method Name

GetSecret

####### 2.3.4.2.10.1.2.0.0 Method Signature

string GetSecret(string key)

####### 2.3.4.2.10.1.3.0.0 Return Type

string

####### 2.3.4.2.10.1.4.0.0 Access Modifier

public

####### 2.3.4.2.10.1.5.0.0 Is Async

false

####### 2.3.4.2.10.1.6.0.0 Framework Specific Attributes

*No items available*

####### 2.3.4.2.10.1.7.0.0 Parameters

- {'parameter_name': 'key', 'parameter_type': 'string', 'is_nullable': 'false', 'purpose': 'The unique target name of the secret to retrieve from the Credential Manager.', 'framework_attributes': []}

####### 2.3.4.2.10.1.8.0.0 Implementation Logic

Specification requires this method to call the `AdvApi32.CredRead` P/Invoke function. It must handle the returned pointer, marshal the native `CREDENTIAL` structure, extract the password, and ensure `AdvApi32.CredFree` is called to release the native memory, even in case of exceptions.

####### 2.3.4.2.10.1.9.0.0 Exception Handling

Specification requires that if `CredRead` returns `false` and the last Win32 error is `ERROR_NOT_FOUND`, a `CredentialNotFoundException` must be thrown. For any other Win32 error, a `CredentialStoreException` must be thrown that includes the error code but NOT the secret key. A `finally` block must be used to guarantee `CredFree` is called if the initial read was successful.

####### 2.3.4.2.10.1.10.0.0 Performance Considerations

Performance should be very fast due to being a local OS call. Specification requires the implementation to be thread-safe.

####### 2.3.4.2.10.1.11.0.0 Validation Requirements

Specification requires a non-null, non-empty string for the \"key\" parameter.

####### 2.3.4.2.10.1.12.0.0 Technology Integration Details

Specification requires interaction with P/Invoke methods defined in the `Native/AdvApi32.cs` class.

###### 2.3.4.2.10.2.0.0.0 Method Name

####### 2.3.4.2.10.2.1.0.0 Method Name

SetSecret

####### 2.3.4.2.10.2.2.0.0 Method Signature

void SetSecret(string key, string secret)

####### 2.3.4.2.10.2.3.0.0 Return Type

void

####### 2.3.4.2.10.2.4.0.0 Access Modifier

public

####### 2.3.4.2.10.2.5.0.0 Is Async

false

####### 2.3.4.2.10.2.6.0.0 Framework Specific Attributes

*No items available*

####### 2.3.4.2.10.2.7.0.0 Parameters

######## 2.3.4.2.10.2.7.1.0 Parameter Name

######### 2.3.4.2.10.2.7.1.1 Parameter Name

key

######### 2.3.4.2.10.2.7.1.2 Parameter Type

string

######### 2.3.4.2.10.2.7.1.3 Is Nullable

false

######### 2.3.4.2.10.2.7.1.4 Purpose

The unique target name under which to store the secret.

######### 2.3.4.2.10.2.7.1.5 Framework Attributes

*No items available*

######## 2.3.4.2.10.2.7.2.0 Parameter Name

######### 2.3.4.2.10.2.7.2.1 Parameter Name

secret

######### 2.3.4.2.10.2.7.2.2 Parameter Type

string

######### 2.3.4.2.10.2.7.2.3 Is Nullable

false

######### 2.3.4.2.10.2.7.2.4 Purpose

The secret value to store.

######### 2.3.4.2.10.2.7.2.5 Framework Attributes

*No items available*

####### 2.3.4.2.10.2.8.0.0 Implementation Logic

Specification requires this method to construct a native `CREDENTIAL` structure, populating it with the key, secret, and other required fields (e.g., `Type = CRED_TYPE_GENERIC`, `Persist = CRED_PERSIST_LOCAL_MACHINE`). It must then call the `AdvApi32.CredWrite` P/Invoke function.

####### 2.3.4.2.10.2.9.0.0 Exception Handling

Specification requires that if `CredWrite` returns `false`, the implementation must get the last Win32 error and throw a `CredentialStoreException` containing the error code. The exception message must never contain the secret key or value.

####### 2.3.4.2.10.2.10.0.0 Performance Considerations

Performance should be very fast.

####### 2.3.4.2.10.2.11.0.0 Validation Requirements

Specification requires non-null, non-empty strings for both parameters.

####### 2.3.4.2.10.2.12.0.0 Technology Integration Details

Specification requires interaction with P/Invoke methods defined in the `Native/AdvApi32.cs` class.

##### 2.3.4.2.11.0.0.0.0 Events

*No items available*

##### 2.3.4.2.12.0.0.0.0 Implementation Notes

Specification requires this service to be registered with a Singleton lifetime in the DI container as it is stateless and thread-safe.

#### 2.3.4.3.0.0.0.0.0 Class Name

##### 2.3.4.3.1.0.0.0.0 Class Name

AdvApi32

##### 2.3.4.3.2.0.0.0.0 File Path

src/Implementations/CredentialManagement/Native/AdvApi32.cs

##### 2.3.4.3.3.0.0.0.0 Class Type

Static P/Invoke Helper

##### 2.3.4.3.4.0.0.0.0 Inheritance

None

##### 2.3.4.3.5.0.0.0.0 Purpose

Specification requires this class to contain all the P/Invoke method signatures, constants, and structures required to interact with the Windows Credential Manager functions in `advapi32.dll`.

##### 2.3.4.3.6.0.0.0.0 Dependencies

*No items available*

##### 2.3.4.3.7.0.0.0.0 Framework Specific Attributes

*No items available*

##### 2.3.4.3.8.0.0.0.0 Technology Integration Notes

Specification designates this class as the bridge between managed C# code and the unmanaged native Windows API.

##### 2.3.4.3.9.0.0.0.0 Properties

*No items available*

##### 2.3.4.3.10.0.0.0.0 Methods

###### 2.3.4.3.10.1.0.0.0 Method Name

####### 2.3.4.3.10.1.1.0.0 Method Name

CredRead

####### 2.3.4.3.10.1.2.0.0 Method Signature

[DllImport(\"advapi32.dll\", CharSet = CharSet.Unicode, SetLastError = true)] public static extern bool CredRead(string target, CredentialType type, int flags, out IntPtr credential);

####### 2.3.4.3.10.1.3.0.0 Return Type

bool

####### 2.3.4.3.10.1.4.0.0 Access Modifier

internal static extern

####### 2.3.4.3.10.1.5.0.0 Is Async

false

####### 2.3.4.3.10.1.6.0.0 Framework Specific Attributes

*No items available*

####### 2.3.4.3.10.1.7.0.0 Parameters

*No items available*

####### 2.3.4.3.10.1.8.0.0 Implementation Logic

This is a P/Invoke declaration for the native `CredReadW` function. Specification requires `CharSet.Unicode` and `SetLastError = true`.

####### 2.3.4.3.10.1.9.0.0 Exception Handling

N/A (The wrapper class, `WindowsSecureCredentialStore`, is responsible for handling exceptions based on the return value and Marshal.GetLastWin32Error()).

####### 2.3.4.3.10.1.10.0.0 Performance Considerations

N/A

####### 2.3.4.3.10.1.11.0.0 Validation Requirements

N/A

####### 2.3.4.3.10.1.12.0.0 Technology Integration Details

Maps directly to the native `advapi32.dll` export.

###### 2.3.4.3.10.2.0.0.0 Method Name

####### 2.3.4.3.10.2.1.0.0 Method Name

CredWrite

####### 2.3.4.3.10.2.2.0.0 Method Signature

[DllImport(\"advapi32.dll\", CharSet = CharSet.Unicode, SetLastError = true)] public static extern bool CredWrite([In] ref Credential credential, int flags);

####### 2.3.4.3.10.2.3.0.0 Return Type

bool

####### 2.3.4.3.10.2.4.0.0 Access Modifier

internal static extern

####### 2.3.4.3.10.2.5.0.0 Is Async

false

####### 2.3.4.3.10.2.6.0.0 Framework Specific Attributes

*No items available*

####### 2.3.4.3.10.2.7.0.0 Parameters

*No items available*

####### 2.3.4.3.10.2.8.0.0 Implementation Logic

This is a P/Invoke declaration for the native `CredWriteW` function. Specification requires `CharSet.Unicode` and `SetLastError = true`.

####### 2.3.4.3.10.2.9.0.0 Exception Handling

N/A

####### 2.3.4.3.10.2.10.0.0 Performance Considerations

N/A

####### 2.3.4.3.10.2.11.0.0 Validation Requirements

N/A

####### 2.3.4.3.10.2.12.0.0 Technology Integration Details

Maps directly to the native `advapi32.dll` export.

###### 2.3.4.3.10.3.0.0.0 Method Name

####### 2.3.4.3.10.3.1.0.0 Method Name

CredFree

####### 2.3.4.3.10.3.2.0.0 Method Signature

[DllImport(\"advapi32.dll\")] public static extern void CredFree(IntPtr buffer);

####### 2.3.4.3.10.3.3.0.0 Return Type

void

####### 2.3.4.3.10.3.4.0.0 Access Modifier

internal static extern

####### 2.3.4.3.10.3.5.0.0 Is Async

false

####### 2.3.4.3.10.3.6.0.0 Framework Specific Attributes

*No items available*

####### 2.3.4.3.10.3.7.0.0 Parameters

*No items available*

####### 2.3.4.3.10.3.8.0.0 Implementation Logic

This is a P/Invoke declaration for the native `CredFree` function.

####### 2.3.4.3.10.3.9.0.0 Exception Handling

N/A

####### 2.3.4.3.10.3.10.0.0 Performance Considerations

N/A

####### 2.3.4.3.10.3.11.0.0 Validation Requirements

N/A

####### 2.3.4.3.10.3.12.0.0 Technology Integration Details

Maps directly to the native `advapi32.dll` export.

##### 2.3.4.3.11.0.0.0.0 Events

*No items available*

##### 2.3.4.3.12.0.0.0.0 Implementation Notes

Specification requires this class to be internal to the assembly to hide the implementation details of the P/Invoke calls from consumers of the library.

### 2.3.5.0.0.0.0.0.0 Interface Specifications

#### 2.3.5.1.0.0.0.0.0 Interface Name

##### 2.3.5.1.1.0.0.0.0 Interface Name

IPasswordHasher

##### 2.3.5.1.2.0.0.0.0 File Path

src/Abstractions/IPasswordHasher.cs

##### 2.3.5.1.3.0.0.0.0 Purpose

Specification requires this interface to define the contract for a service that provides one-way password hashing and verification, abstracting the specific algorithm and implementation.

##### 2.3.5.1.4.0.0.0.0 Generic Constraints

None

##### 2.3.5.1.5.0.0.0.0 Framework Specific Inheritance

None

##### 2.3.5.1.6.0.0.0.0 Method Contracts

###### 2.3.5.1.6.1.0.0.0 Method Name

####### 2.3.5.1.6.1.1.0.0 Method Name

HashPassword

####### 2.3.5.1.6.1.2.0.0 Method Signature

string HashPassword(string password)

####### 2.3.5.1.6.1.3.0.0 Return Type

string

####### 2.3.5.1.6.1.4.0.0 Framework Attributes

*No items available*

####### 2.3.5.1.6.1.5.0.0 Parameters

- {'parameter_name': 'password', 'parameter_type': 'string', 'purpose': 'The plaintext password to hash.'}

####### 2.3.5.1.6.1.6.0.0 Contract Description

Specification requires this method to generate a secure, salted hash of the provided password. The hash format will be compliant with the underlying implementation (BCrypt).

####### 2.3.5.1.6.1.7.0.0 Exception Contracts

Specification requires implementations to throw `System.ArgumentNullException` if the password is null or empty.

###### 2.3.5.1.6.2.0.0.0 Method Name

####### 2.3.5.1.6.2.1.0.0 Method Name

VerifyPassword

####### 2.3.5.1.6.2.2.0.0 Method Signature

bool VerifyPassword(string password, string hashedPassword)

####### 2.3.5.1.6.2.3.0.0 Return Type

bool

####### 2.3.5.1.6.2.4.0.0 Framework Attributes

*No items available*

####### 2.3.5.1.6.2.5.0.0 Parameters

######## 2.3.5.1.6.2.5.1.0 Parameter Name

######### 2.3.5.1.6.2.5.1.1 Parameter Name

password

######### 2.3.5.1.6.2.5.1.2 Parameter Type

string

######### 2.3.5.1.6.2.5.1.3 Purpose

The plaintext password to verify.

######## 2.3.5.1.6.2.5.2.0 Parameter Name

######### 2.3.5.1.6.2.5.2.1 Parameter Name

hashedPassword

######### 2.3.5.1.6.2.5.2.2 Parameter Type

string

######### 2.3.5.1.6.2.5.2.3 Purpose

The previously generated hash to compare against.

####### 2.3.5.1.6.2.6.0.0 Contract Description

Specification requires this method to compare a plaintext password with a stored hash. It must return `true` if they match, and `false` otherwise. This comparison must be performed in constant time to prevent timing attacks.

####### 2.3.5.1.6.2.7.0.0 Exception Contracts

Specification requires implementations not to throw exceptions for invalid hash formats; they should return `false` instead to prevent information leakage.

##### 2.3.5.1.7.0.0.0.0 Property Contracts

*No items available*

##### 2.3.5.1.8.0.0.0.0 Implementation Guidance

Specification requires implementations to be thread-safe and stateless, suitable for registration as Singleton services in a DI container.

#### 2.3.5.2.0.0.0.0.0 Interface Name

##### 2.3.5.2.1.0.0.0.0 Interface Name

ISecureCredentialStore

##### 2.3.5.2.2.0.0.0.0 File Path

src/Abstractions/ISecureCredentialStore.cs

##### 2.3.5.2.3.0.0.0.0 Purpose

Specification requires this interface to define the contract for a key-value store that provides secure storage and retrieval of sensitive secrets, such as connection strings or API keys.

##### 2.3.5.2.4.0.0.0.0 Generic Constraints

None

##### 2.3.5.2.5.0.0.0.0 Framework Specific Inheritance

None

##### 2.3.5.2.6.0.0.0.0 Method Contracts

###### 2.3.5.2.6.1.0.0.0 Method Name

####### 2.3.5.2.6.1.1.0.0 Method Name

GetSecret

####### 2.3.5.2.6.1.2.0.0 Method Signature

string GetSecret(string key)

####### 2.3.5.2.6.1.3.0.0 Return Type

string

####### 2.3.5.2.6.1.4.0.0 Framework Attributes

*No items available*

####### 2.3.5.2.6.1.5.0.0 Parameters

- {'parameter_name': 'key', 'parameter_type': 'string', 'purpose': 'The unique key (target name) identifying the secret to retrieve.'}

####### 2.3.5.2.6.1.6.0.0 Contract Description

Specification requires this method to retrieve a secret from the secure store. The implementation must handle the case where the secret does not exist.

####### 2.3.5.2.6.1.7.0.0 Exception Contracts

Specification requires implementations to throw a custom `CredentialNotFoundException` if no secret is found for the given key, and `CredentialStoreException` for other underlying storage errors.

###### 2.3.5.2.6.2.0.0.0 Method Name

####### 2.3.5.2.6.2.1.0.0 Method Name

SetSecret

####### 2.3.5.2.6.2.2.0.0 Method Signature

void SetSecret(string key, string secret)

####### 2.3.5.2.6.2.3.0.0 Return Type

void

####### 2.3.5.2.6.2.4.0.0 Framework Attributes

*No items available*

####### 2.3.5.2.6.2.5.0.0 Parameters

######## 2.3.5.2.6.2.5.1.0 Parameter Name

######### 2.3.5.2.6.2.5.1.1 Parameter Name

key

######### 2.3.5.2.6.2.5.1.2 Parameter Type

string

######### 2.3.5.2.6.2.5.1.3 Purpose

The unique key (target name) under which to store the secret.

######## 2.3.5.2.6.2.5.2.0 Parameter Name

######### 2.3.5.2.6.2.5.2.1 Parameter Name

secret

######### 2.3.5.2.6.2.5.2.2 Parameter Type

string

######### 2.3.5.2.6.2.5.2.3 Purpose

The sensitive value to be stored.

####### 2.3.5.2.6.2.6.0.0 Contract Description

Specification requires this method to store or update a secret in the secure store. The operation should be idempotent.

####### 2.3.5.2.6.2.7.0.0 Exception Contracts

Specification requires implementations to throw a `CredentialStoreException` if the operation fails for any reason. Exception messages must never contain the secret key or value.

##### 2.3.5.2.7.0.0.0.0 Property Contracts

*No items available*

##### 2.3.5.2.8.0.0.0.0 Implementation Guidance

Specification requires implementations to be platform-specific (Windows) and documented as such. They must be thread-safe and suitable for Singleton registration.

### 2.3.6.0.0.0.0.0.0 Enum Specifications

*No items available*

### 2.3.7.0.0.0.0.0.0 Dto Specifications

*No items available*

### 2.3.8.0.0.0.0.0.0 Configuration Specifications

- {'configuration_name': 'BCryptSettings', 'file_path': 'src/Configuration/BCryptSettings.cs', 'purpose': 'Specification requires this class to provide a strongly-typed representation of BCrypt configuration settings, intended to be used with the .NET Options pattern.', 'framework_base_class': 'None', 'configuration_sections': [{'section_name': 'Security:BCrypt', 'properties': [{'property_name': 'WorkFactor', 'property_type': 'int', 'default_value': '12', 'required': 'false', 'description': 'The cost factor for the BCrypt hashing algorithm. Higher values are more secure but slower. Must be between 4 and 31.'}]}], 'validation_requirements': 'Specification requires the `WorkFactor` to be a value supported by the BCrypt library. Data annotation attributes (e.g., `[Range(4, 31)]`) should be used for validation.'}

### 2.3.9.0.0.0.0.0.0 Dependency Injection Specifications

#### 2.3.9.1.0.0.0.0.0 Service Interface

##### 2.3.9.1.1.0.0.0.0 Service Interface

IPasswordHasher

##### 2.3.9.1.2.0.0.0.0 Service Implementation

BCryptPasswordHasher

##### 2.3.9.1.3.0.0.0.0 Lifetime

Singleton

##### 2.3.9.1.4.0.0.0.0 Registration Reasoning

Validation confirms this service is stateless and thread-safe. A singleton instance is specified to reduce object creation overhead.

##### 2.3.9.1.5.0.0.0.0 Framework Registration Pattern

services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

#### 2.3.9.2.0.0.0.0.0 Service Interface

##### 2.3.9.2.1.0.0.0.0 Service Interface

ISecureCredentialStore

##### 2.3.9.2.2.0.0.0.0 Service Implementation

WindowsSecureCredentialStore

##### 2.3.9.2.3.0.0.0.0 Lifetime

Singleton

##### 2.3.9.2.4.0.0.0.0 Registration Reasoning

Validation confirms this service is stateless, thread-safe, and interacts with a system-level resource. A singleton instance is specified as the most efficient and appropriate lifetime.

##### 2.3.9.2.5.0.0.0.0 Framework Registration Pattern

services.AddSingleton<ISecureCredentialStore, WindowsSecureCredentialStore>();

### 2.3.10.0.0.0.0.0.0 External Integration Specifications

#### 2.3.10.1.0.0.0.0.0 Integration Target

##### 2.3.10.1.1.0.0.0.0 Integration Target

BCrypt.Net-Next v4.0.3

##### 2.3.10.1.2.0.0.0.0 Integration Type

NuGet Library

##### 2.3.10.1.3.0.0.0.0 Required Client Classes

- BCryptPasswordHasher

##### 2.3.10.1.4.0.0.0.0 Configuration Requirements

Specification requires a configurable \"WorkFactor\" to control hashing complexity. This must be managed via `appsettings.json` and the `BCryptSettings` options class.

##### 2.3.10.1.5.0.0.0.0 Error Handling Requirements

Specification requires the implementation to gracefully handle exceptions thrown by the library for malformed hashes during verification, returning `false` instead of re-throwing to prevent security vulnerabilities.

##### 2.3.10.1.6.0.0.0.0 Authentication Requirements

N/A

##### 2.3.10.1.7.0.0.0.0 Framework Integration Patterns

Specification mandates that the library's static methods be wrapped in a non-static service class (Adapter pattern) to facilitate dependency injection and testing.

#### 2.3.10.2.0.0.0.0.0 Integration Target

##### 2.3.10.2.1.0.0.0.0 Integration Target

Windows Credential Manager API

##### 2.3.10.2.2.0.0.0.0 Integration Type

Native OS API

##### 2.3.10.2.3.0.0.0.0 Required Client Classes

- WindowsSecureCredentialStore
- Native.AdvApi32

##### 2.3.10.2.4.0.0.0.0 Configuration Requirements

Specification requires the `key` used to store/retrieve secrets to be unique and well-known to the application components that need it.

##### 2.3.10.2.5.0.0.0.0 Error Handling Requirements

Specification requires the implementation to translate native Win32 error codes into specific, documented .NET exceptions (`CredentialNotFoundException`, `CredentialStoreException`). Error messages must not contain sensitive information.

##### 2.3.10.2.6.0.0.0.0 Authentication Requirements

Specification requires the application process to run with sufficient permissions to access the Local Machine credential store (`CRED_PERSIST_LOCAL_MACHINE`).

##### 2.3.10.2.7.0.0.0.0 Framework Integration Patterns

Specification requires P/Invoke to be used to call functions from `advapi32.dll`. All native interop logic must be encapsulated within the `WindowsSecureCredentialStore` and its internal native helper classes.

## 2.4.0.0.0.0.0.0.0 Project Supporting Files

### 2.4.1.0.0.0.0.0.0 File Type

#### 2.4.1.1.0.0.0.0.0 File Type

Project Definition

#### 2.4.1.2.0.0.0.0.0 File Name

DMPS.CrossCutting.Security.csproj

#### 2.4.1.3.0.0.0.0.0 File Path

./DMPS.CrossCutting.Security.csproj

#### 2.4.1.4.0.0.0.0.0 Purpose

Defines the .NET 8 class library project, its target framework, and its dependencies on NuGet packages.

#### 2.4.1.5.0.0.0.0.0 Content Description

<Project Sdk=\"Microsoft.NET.Sdk\">\n\n  <PropertyGroup>\n    <TargetFramework>net8.0</TargetFramework>\n    <ImplicitUsings>enable</ImplicitUsings>\n    <Nullable>enable</Nullable>\n  </PropertyGroup>\n\n  <ItemGroup>\n    <PackageReference Include=\"BCrypt.Net-Next\" Version=\"4.0.3\" />\n    <PackageReference Include=\"Microsoft.Extensions.DependencyInjection.Abstractions\" Version=\"8.0.0\" />\n    <PackageReference Include=\"Microsoft.Extensions.Logging.Abstractions\" Version=\"8.0.0\" />\n    <PackageReference Include=\"Microsoft.Extensions.Options\" Version=\"8.0.0\" />\n  </ItemGroup>\n\n</Project>

#### 2.4.1.6.0.0.0.0.0 Framework Specific Attributes

- TargetFramework: net8.0
- PackageReference for BCrypt.Net-Next and Microsoft.Extensions libraries

### 2.4.2.0.0.0.0.0.0 File Type

#### 2.4.2.1.0.0.0.0.0 File Type

Version Control

#### 2.4.2.2.0.0.0.0.0 File Name

.gitignore

#### 2.4.2.3.0.0.0.0.0 File Path

./.gitignore

#### 2.4.2.4.0.0.0.0.0 Purpose

Specifies intentionally untracked files for Git to ignore, tailored for .NET development.

#### 2.4.2.5.0.0.0.0.0 Content Description

# Ignore Visual Studio temporary files, build artifacts, and user-specific settings.\n[Bb]in/\n[Oo]bj/\n.vs/\n*.suo\n*.user

#### 2.4.2.6.0.0.0.0.0 Framework Specific Attributes

- .NET build output folders (bin, obj)
- Visual Studio specific files (.vs, .suo, .user)

### 2.4.3.0.0.0.0.0.0 File Type

#### 2.4.3.1.0.0.0.0.0 File Type

Development Tools

#### 2.4.3.2.0.0.0.0.0 File Name

.editorconfig

#### 2.4.3.3.0.0.0.0.0 File Path

./.editorconfig

#### 2.4.3.4.0.0.0.0.0 Purpose

Defines and maintains consistent coding styles between multiple developers and IDEs.

#### 2.4.3.5.0.0.0.0.0 Content Description

root = true\n\n[*]\nindent_style = space\nindent_size = 4\nend_of_line = lf\ncharset = utf-8\ntrim_trailing_whitespace = true\ninsert_final_newline = true\n\n[*.cs]\ncsharp_style_namespace_declarations = file_scoped\ncsharp_style_prefer_simplified_interpolation = true\n

#### 2.4.3.6.0.0.0.0.0 Framework Specific Attributes

- csharp_style_namespace_declarations: file_scoped (C# 10+ feature)
- Standard formatting rules

