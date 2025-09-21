# 1 Extraction Metadata

| Property | Value |
|----------|-------|
| Repository Id | DMPS.Client.Application |
| Extraction Timestamp | 2023-10-27T10:00:00Z |
| Mapping Validation Score | 100% |
| Context Completeness Score | 95% |
| Implementation Readiness Level | High |

# 2 Relevant Requirements

## 2.1 Requirement Id

### 2.1.1 Requirement Id

REQ-1-041

### 2.1.2 Requirement Text

The system must implement an automatic session lock after 15 minutes of user inactivity, requiring re-authentication to resume.

### 2.1.3 Validation Criteria

- The application service layer must contain logic to monitor user activity.
- After 15 minutes of inactivity, the service must trigger a 'locked' state.
- The service must provide a mechanism to validate credentials to unlock the session.

### 2.1.4 Implementation Implications

- An ISessionLockService will be implemented to manage an inactivity timer.
- This service will orchestrate with the IAuthenticationService to re-validate the user's password.
- The service will raise events to the Presentation Layer to show/hide a lock screen.

### 2.1.5 Extraction Reasoning

This requirement is explicitly mapped to REPO-08-APC. The repository description states a responsibility for 'managing user authentication and session state, including the automatic session lock timer'. Sequence Diagram SEQ-9 confirms the Application Services layer orchestrates this entire flow.

## 2.2.0 Requirement Id

### 2.2.1 Requirement Id

REQ-INT-003

### 2.2.2 Requirement Text

The application must validate its license key against an external API at startup and operate in different modes (e.g., Licensed, GracePeriod, ReadOnly) based on the validation result.

### 2.2.3 Validation Criteria

- The application must call the license validation API on startup.
- The application must correctly interpret the API response.
- The application must manage state for grace periods and apply operational restrictions if the license is invalid.

### 2.2.4 Implementation Implications

- A LicenseValidationService will be implemented to orchestrate the validation process.
- This service will use the ILicenseApiClient dependency to communicate with the external Odoo API.
- It will manage the application's operational state based on the LicenseStatusResult, including fallback logic for network failures as shown in SEQ-5.

### 2.2.5 Extraction Reasoning

This requirement is explicitly mapped to REPO-08-APC. The repository description assigns it the responsibility to 'orchestrate the license validation workflow...and handling the grace period logic'. Sequence Diagram SEQ-5 details this exact orchestration role for the 'DMPS.Client.Application'.

## 2.3.0 Requirement Id

### 2.3.1 Requirement Id

REQ-1-040

### 2.3.2 Requirement Text

The system must support role-based access control (RBAC) to differentiate features available to Administrators versus Technicians.

### 2.3.3 Validation Criteria

- The system must identify the role of the authenticated user.
- The application's security context must make the user's role available.

### 2.3.4 Implementation Implications

- The IAuthenticationService must, upon successful login, populate a user session object that includes the user's role.
- This user session information will be consumed by the Presentation Layer to dynamically adjust the UI.

### 2.3.5 Extraction Reasoning

While the UI is adjusted in the Presentation Layer, this repository is responsible for the foundational step of authenticating the user and determining their role. Sequence Diagram SEQ-1 shows the AuthenticationService returning a UserSession object, which is the source of the role information needed to fulfill this requirement.

## 2.4.0 Requirement Id

### 2.4.1 Requirement Id

REQ-FNC-003

### 2.4.2 Requirement Text

Users with a temporary password must be forced to change it upon their first successful login.

### 2.4.3 Validation Criteria

- The system must be able to flag a user account as requiring a password change.
- The authentication process must check for this flag.

### 2.4.4 Implementation Implications

- The IAuthenticationService's login logic will inspect the returned User domain object for a 'isTemporaryPassword' or similar flag.
- If the flag is true, the service will return a specific status or user session state that the Presentation Layer uses to trigger the password change workflow.

### 2.4.5 Extraction Reasoning

This requirement is mapped to REPO-08-APC. User management orchestration, including the logic following an initial login with a temporary password, is a core responsibility of the client application service layer. Sequence Diagram SEQ-14 and SEQ-16 reference setting a 'ForcePasswordChange' flag, which this service would need to interpret during login.

# 3.0.0 Relevant Components

- {'component_name': 'Client Services', 'component_specification': "A collection of use-case-oriented services that orchestrate interactions between the Presentation Layer and the application's core logic and infrastructure. This includes services like AuthenticationService, LicenseValidationService, SessionLockService, and PrintJobPublisher.", 'implementation_requirements': ['Implement service classes like AuthenticationService and PrintJobService that expose the public interfaces defined in exposed_contracts.', 'Use constructor dependency injection to acquire dependencies on infrastructure interfaces (e.g., IMessageProducer, ILicenseApiClient).', 'Services must be stateless or manage state related only to the client session (e.g., current user, license status).', 'All methods exposed to the Presentation Layer that involve I/O must be asynchronous (async Task).'], 'architectural_context': "This component group is the primary implementation of the client-side responsibilities of the 'Application Services Layer'. It acts as a mediator, decoupling the UI from the backend and infrastructure concerns.", 'extraction_reasoning': "The repository is explicitly defined as the 'Application Services Layer for the client-side WPF application' and its components_map points directly to 'client-services'. The architecture document confirms these components belong to this layer."}

# 4.0.0 Architectural Layers

- {'layer_name': 'Application Services Layer', 'layer_responsibilities': 'Orchestrates application-specific logic. For the client, this involves managing application state, handling user authentication and session, orchestrating calls to infrastructure for external communication (APIs, message queues, IPC), and providing a clean API for the Presentation Layer.', 'layer_constraints': ['Must not contain any UI-specific code or references to UI frameworks like WPF.', 'Must not perform direct data access or file system operations; must delegate these tasks to infrastructure services through interfaces.', 'Should be designed to be testable, with dependencies abstracted via interfaces.'], 'implementation_patterns': ['Service Layer', 'Facade', 'Dependency Injection'], 'extraction_reasoning': "The repository definition explicitly maps to the 'application-services-layer' and its description perfectly matches the responsibilities of this layer within the specified Layered Architecture."}

# 5.0.0 Dependency Interfaces

## 5.1.0 Interface Name

### 5.1.1 Interface Name

IMessageProducer

### 5.1.2 Source Repository

REPO-05-COM

### 5.1.3 Method Contracts

- {'method_name': 'Publish', 'method_signature': 'void Publish<T>(T message, string routingKey, string correlationId)', 'method_purpose': 'To publish a command or event message to the message broker (RabbitMQ) for asynchronous processing by the background service.', 'integration_context': 'Called when the user initiates a long-running task that should not block the UI, such as submitting a print job (SEQ-3) or exporting to PDF (SEQ-18).'}

### 5.1.4 Integration Pattern

Dependency Injection

### 5.1.5 Communication Protocol

Direct method calls (in-process)

### 5.1.6 Extraction Reasoning

The repository's responsibility to 'construct and publish command messages to the RabbitMQ message queue' necessitates this dependency, which is listed in its dependency_contracts.

## 5.2.0 Interface Name

### 5.2.1 Interface Name

INamedPipeClient

### 5.2.2 Source Repository

REPO-05-COM

### 5.2.3 Method Contracts

- {'method_name': 'SendRequestAsync', 'method_signature': 'Task<string> SendRequestAsync(string request, CancellationToken token)', 'method_purpose': 'To send a synchronous, low-latency request to the background Windows Service and await a reply.', 'integration_context': "Used to perform health or status checks, such as verifying the background service is running before enabling the 'Print' button in the UI, as detailed in Sequence Diagram SEQ-12."}

### 5.2.4 Integration Pattern

Dependency Injection

### 5.2.5 Communication Protocol

Direct method calls (in-process), which then wrap Inter-Process Communication (IPC) via Named Pipes.

### 5.2.6 Extraction Reasoning

The repository's responsibility for 'making synchronous status requests to the background service via Named Pipes' requires this dependency, which is listed in its dependency_contracts.

## 5.3.0 Interface Name

### 5.3.1 Interface Name

ILicenseApiClient

### 5.3.2 Source Repository

REPO-07-IOI

### 5.3.3 Method Contracts

- {'method_name': 'ValidateLicenseAsync', 'method_signature': 'Task<LicenseStatus> ValidateLicenseAsync(string licenseKey)', 'method_purpose': "To communicate with the external Odoo Licensing API over HTTPS to validate the application's license key.", 'integration_context': 'Called during application startup as part of the license validation workflow orchestrated by the LicenseValidationService, as shown in Sequence Diagram SEQ-5.'}

### 5.3.4 Integration Pattern

Dependency Injection

### 5.3.5 Communication Protocol

Direct method calls (in-process)

### 5.3.6 Extraction Reasoning

The repository's responsibility to 'orchestrate the license validation workflow' depends on this interface for the actual external communication. It is explicitly defined in dependency_contracts.

## 5.4.0 Interface Name

### 5.4.1 Interface Name

IUserRepository

### 5.4.2 Source Repository

REPO-01-SHK

### 5.4.3 Method Contracts

- {'method_name': 'GetUserByUsernameAsync', 'method_signature': 'Task<User> GetUserByUsernameAsync(string username)', 'method_purpose': "To retrieve a user's domain object from the persistence store based on their username.", 'integration_context': "Called by the AuthenticationService during the login process (SEQ-1) to fetch the user's data, including their password hash."}

### 5.4.4 Integration Pattern

Dependency Injection

### 5.4.5 Communication Protocol

Direct method calls (in-process)

### 5.4.6 Extraction Reasoning

User authentication, a core responsibility, requires fetching user data. This dependency contract is specified for that purpose. Note: The repository definition implies this dependency through REPO-01-SHK, this detail is from the provided dependency_contracts section.

## 5.5.0 Interface Name

### 5.5.1 Interface Name

IPasswordHasher

### 5.5.2 Source Repository

REPO-04-SEC

### 5.5.3 Method Contracts

- {'method_name': 'VerifyPassword', 'method_signature': 'bool VerifyPassword(string password, string hashedPassword)', 'method_purpose': 'To securely compare a plaintext password provided by the user against the stored hash.', 'integration_context': 'Called by the AuthenticationService immediately after fetching the user record to verify the provided credentials, as shown in Sequence Diagram SEQ-1.'}

### 5.5.4 Integration Pattern

Dependency Injection

### 5.5.5 Communication Protocol

Direct method calls (in-process)

### 5.5.6 Extraction Reasoning

The repository is responsible for authentication, which must be done securely. This dependency provides the necessary cryptographic utility as defined in dependency_contracts.

# 6.0.0 Exposed Interfaces

## 6.1.0 Interface Name

### 6.1.1 Interface Name

IAuthenticationService

### 6.1.2 Consumer Repositories

- REPO-09-PRE

### 6.1.3 Method Contracts

#### 6.1.3.1 Method Name

##### 6.1.3.1.1 Method Name

LoginAsync

##### 6.1.3.1.2 Method Signature

Task<bool> LoginAsync(string username, string password)

##### 6.1.3.1.3 Method Purpose

Orchestrates the user login process by validating credentials against the backend and establishing a user session.

##### 6.1.3.1.4 Implementation Requirements

Must use IUserRepository to fetch the user and IPasswordHasher to verify the password. Must manage the CurrentUser session state.

#### 6.1.3.2.0 Method Name

##### 6.1.3.2.1 Method Name

Logout

##### 6.1.3.2.2 Method Signature

void Logout()

##### 6.1.3.2.3 Method Purpose

Clears the current user session and related authentication state.

##### 6.1.3.2.4 Implementation Requirements

Must set CurrentUser to null and raise the UserLoggedOut event.

### 6.1.4.0.0 Service Level Requirements

- Login response time should be under 500ms under normal conditions.

### 6.1.5.0.0 Implementation Constraints

- Must never store the plaintext password in memory after the verification step is complete.

### 6.1.6.0.0 Extraction Reasoning

This is a primary public interface defined in the repository's exposed_contracts. It serves the core use case of user authentication for the Presentation Layer.

## 6.2.0.0.0 Interface Name

### 6.2.1.0.0 Interface Name

IPrintJobService

### 6.2.2.0.0 Consumer Repositories

- REPO-09-PRE

### 6.2.3.0.0 Method Contracts

- {'method_name': 'SubmitPrintJobAsync', 'method_signature': 'Task SubmitPrintJobAsync(PrintJobData jobData)', 'method_purpose': 'Provides a simple entry point for the UI to request a print job. It abstracts away the details of message queueing.', 'implementation_requirements': 'Must construct a SubmitPrintJobCommand message from the jobData and publish it using the IMessageProducer dependency.'}

### 6.2.4.0.0 Service Level Requirements

- The method must return quickly to the caller, acknowledging that the job has been queued, not that it has been completed.

### 6.2.5.0.0 Implementation Constraints

*No items available*

### 6.2.6.0.0 Extraction Reasoning

This interface is explicitly defined in exposed_contracts and is required to decouple the Presentation Layer from the asynchronous communication mechanism used for printing.

## 6.3.0.0.0 Interface Name

### 6.3.1.0.0 Interface Name

ISystemStatusService

### 6.3.2.0.0 Consumer Repositories

- REPO-09-PRE

### 6.3.3.0.0 Method Contracts

- {'method_name': 'IsBackgroundServiceRunningAsync', 'method_signature': 'Task<bool> IsBackgroundServiceRunningAsync()', 'method_purpose': 'Allows the UI to perform a quick, synchronous check on the availability of the background Windows Service.', 'implementation_requirements': "Must use the INamedPipeClient dependency to send a 'PING' request and interpret the 'PONG' response, as shown in SEQ-12."}

### 6.3.4.0.0 Service Level Requirements

- Response time must be very low (e.g., <50ms) to avoid delaying UI updates.

### 6.3.5.0.0 Implementation Constraints

*No items available*

### 6.3.6.0.0 Extraction Reasoning

This interface is defined in exposed_contracts to enable the UI to provide immediate feedback to the user about system readiness, preventing them from initiating tasks destined to fail.

# 7.0.0.0.0 Technology Context

## 7.1.0.0.0 Framework Requirements

.NET 8.0, C# 12. Must use Microsoft.Extensions.DependencyInjection for dependency resolution.

## 7.2.0.0.0 Integration Technologies

- RabbitMQ (via IMessageProducer abstraction)
- Named Pipes (via INamedPipeClient abstraction)
- HTTPS/REST (via ILicenseApiClient abstraction)

## 7.3.0.0.0 Performance Constraints

All I/O-bound operations or potentially long-running orchestrations must be implemented using async/await patterns to ensure the client UI remains responsive.

## 7.4.0.0.0 Security Requirements

Must not store sensitive data like passwords in memory. Must handle user session data securely. Orchestrates with security components but does not implement cryptographic functions itself.

# 8.0.0.0.0 Extraction Validation

| Property | Value |
|----------|-------|
| Mapping Completeness Check | All specified mappings in the repository definitio... |
| Cross Reference Validation | Responsibilities listed in the repository descript... |
| Implementation Readiness Assessment | The context is highly sufficient for implementatio... |
| Quality Assurance Confirmation | A systematic review confirmed that the repository'... |

