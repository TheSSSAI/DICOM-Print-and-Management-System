# Architecture Design Specification

# 1. Style
LayeredArchitecture


---

# 2. Patterns

## 2.1. Client-Server Architecture
The system is structured into two main components: a client and a server. The WPF Desktop Application acts as the client, handling all user interactions. The background Windows Service acts as the server, responsible for all asynchronous processing, DICOM services, and data management. This is explicitly required by REQ-1-001.

### 2.1.3. Benefits

- Separation of Concerns: Isolates the user interface from long-running background tasks, ensuring UI responsiveness.
- Centralized Logic: Consolidates all heavy processing and core services (DICOM SCP, database writes, printing) into a single, manageable service.
- Improved Reliability: The background service can run independently of the client application and can be configured for automatic recovery (REQ-1-086).

### 2.1.4. Tradeoffs

- Increased Complexity: Requires Inter-Process Communication (IPC) mechanisms (RabbitMQ, Named Pipes) to coordinate between the client and server.
- Deployment Overhead: Two separate processes must be installed, configured, and managed.

### 2.1.5. Applicability

- **Scenarios:**
  
  - Desktop applications that need to perform long-running tasks without blocking the user interface.
  - Systems requiring a persistent background process to listen for network events, such as a DICOM C-STORE SCP.
  - Applications where core business logic needs to be centralized and shared among potential future clients.
  
- **Constraints:**
  
  - The design must accommodate both synchronous (status checks) and asynchronous (task offloading) communication between the client and server.
  

## 2.2. Model-View-ViewModel (MVVM)
The WPF client application's UI will be structured using the MVVM pattern, as specified in REQ-1-013. The View (XAML) is responsible for the visual presentation, the ViewModel handles presentation logic and state, and the Model represents the application's data and business logic.

### 2.2.3. Benefits

- Testability: Decouples the UI from the application logic, allowing ViewModels to be unit tested without requiring a UI framework.
- Maintainability: Clear separation of concerns makes the codebase easier to understand, modify, and evolve.
- Data Binding: Leverages WPF's powerful data binding capabilities to reduce boilerplate code for UI updates.

### 2.2.4. Applicability

- **Scenarios:**
  
  - Developing user interfaces with Windows Presentation Foundation (WPF) or other XAML-based frameworks.
  

## 2.3. Event-Driven Messaging (via Message Queue)
Asynchronous communication from the WPF client to the Windows Service is achieved using a message queue (RabbitMQ), as per REQ-1-004. The client (Producer) publishes command messages (e.g., 'Process Print Job') to a queue, and the service (Consumer) subscribes to these messages to perform the work.

### 2.3.3. Benefits

- Decoupling: The client and service do not need to be aware of each other's immediate state. The client can submit jobs even if the service is temporarily busy.
- Resilience: With durable queues and persistent messages (REQ-1-005), tasks are not lost if the service or broker restarts.
- Scalability & Load Leveling: The queue acts as a buffer, smoothing out bursts of requests and allowing the service to process tasks at its own pace.

### 2.3.4. Tradeoffs

- Complexity: Introduces a message broker (RabbitMQ) as a system dependency that must be installed and managed (REQ-1-002).
- Asynchronous Complexity: Requires careful handling of eventual consistency and reporting task status back to the user.
- Monitoring: Requires monitoring of queue depths and the dead-letter queue (REQ-1-006, REQ-1-091).

### 2.3.5. Applicability

- **Scenarios:**
  
  - Offloading long-running tasks like PDF generation, printing, and database writes from a client application.
  - Decoupling high-throughput data ingestion (e.g., DICOM C-STORE SCP) from slower data persistence logic (REQ-1-010).
  



---

# 3. Layers

## 3.1. Presentation Layer (WPF Client)
This layer is responsible for the user interface and user experience. It consists of Views and ViewModels that interact with the user, based on the MVVM pattern.

### 3.1.4. Technologystack
WPF, XAML, Material Design in XAML Toolkit, .NET 8, Vortice.Windows

### 3.1.5. Language
C# 12

### 3.1.6. Type
Presentation

### 3.1.7. Responsibilities

- Render all user interface elements (windows, controls, dialogs). (REQ-1-001)
- Implement the MVVM pattern for UI logic and state management. (REQ-1-013)
- Handle user input (mouse, keyboard, drag-and-drop). (REQ-1-054, REQ-1-025)
- Display DICOM images using GPU-accelerated rendering. (REQ-1-052)
- Provide user feedback via progress indicators and non-blocking notifications. (REQ-1-072)
- Dynamically adjust UI based on user role (Technician/Administrator). (REQ-1-040)

### 3.1.8. Components

- Views (XAML): LoginView, MainWindow, StudyBrowserView, DicomViewerControl, PrintPreviewView, SettingsView, UserManagementView, AuditLogView, SystemHealthDashboard.
- ViewModels: LoginViewModel, MainViewModel, StudyBrowserViewModel, DicomViewerViewModel, PrintPreviewViewModel, SettingsViewModel, etc.
- UI Services: NavigationService, DialogService, NotificationService.

### 3.1.9. Dependencies

- **Layer Id:** application-services  
**Type:** Required  

## 3.2. Application Services Layer
This layer contains the application-specific logic, orchestrating interactions between the UI, domain objects, and infrastructure services. It exists in both the Client (client-side logic) and the Service (hosted background tasks).

### 3.2.4. Technologystack
.NET 8, Microsoft.Extensions.Hosting, Microsoft.Extensions.DependencyInjection

### 3.2.5. Language
C# 12

### 3.2.6. Type
ApplicationServices

### 3.2.7. Responsibilities

- **In Client:** Manage application state, orchestrate license validation (REQ-1-011), build and send messages to RabbitMQ (REQ-1-004), and make synchronous calls via Named Pipes (REQ-1-007).
- **In Service:** Host and manage the lifecycle of all background tasks using IHostedService. (REQ-1-003)
- **In Service:** Act as the DICOM C-STORE SCP listener. (REQ-1-035)
- **In Service:** Consume and dispatch messages from RabbitMQ for database writes, PDF generation, and printing. (REQ-1-003)
- **In Service:** Execute scheduled tasks like data retention policy enforcement (REQ-1-018) and data integrity checks (REQ-1-058).

### 3.2.8. Components

- Client Services: AuthenticationService, LicenseValidationService, SessionLockService, DicomQueryOrchestrator, PrintJobPublisher.
- Windows Service Hosted Services: DicomScpService, RabbitMqConsumerService (for DB Writes, Print Jobs, PDF Jobs), DataRetentionPolicyService, NamedPipeServerService.

### 3.2.9. Dependencies

- **Layer Id:** business-logic  
**Type:** Required  
- **Layer Id:** infrastructure  
**Type:** Required  

## 3.3. Business Logic / Domain Layer
Contains the core business logic, domain entities, and rules of the application. This code is independent of any specific UI or database technology and is shared between the Client and Service.

### 3.3.4. Technologystack
.NET 8

### 3.3.5. Language
C# 12

### 3.3.6. Type
BusinessLogic

### 3.3.7. Responsibilities

- Define domain entities like Patient, Study, Series, Image, User, Role. (Matches Database Design)
- Encapsulate business rules related to data validation and state transitions.
- Implement core DICOM logic for parsing, editing, and anonymization. (REQ-1-009, REQ-1-065)
- Define logic for creating and managing DICOM Presentation States (GSPS). (REQ-1-061)
- Contain logic for password policy enforcement. (REQ-1-042)
- Define data structures for print layouts and hanging protocols. (REQ-1-022, REQ-1-062)

### 3.3.8. Components

- Domain Entities: User, Role, Patient, Study, Series, Image, PrintJob, AuditLog, etc.
- Domain Services: DicomAnonymizer, PasswordPolicyValidator, WorkingCopyManager (REQ-1-045).
- Value Objects: DicomTag, WindowLevel, PrintLayout.
- Repositories Interfaces: IStudyRepository, IUserRepository, etc.

### 3.3.9. Dependencies


## 3.4. Data Access Layer
This layer is responsible for all data persistence and retrieval. It abstracts the underlying database from the rest of the application. It resides primarily within the Windows Service.

### 3.4.4. Technologystack
Entity Framework Core 8, Npgsql PostgreSQL Driver, PostgreSQL 16

### 3.4.5. Language
C# 12

### 3.4.6. Type
DataAccess

### 3.4.7. Responsibilities

- Implement repository patterns for all database entities.
- Perform all CRUD (Create, Read, Update, Delete) operations against the PostgreSQL database.
- Use Entity Framework Core to map domain objects to database tables.
- Handle database connections, transactions, and concurrency.
- Utilize pgcrypto for encrypting and decrypting PHI columns. (REQ-1-083)
- Manage database migrations.

### 3.4.8. Components

- DbContext: ApplicationDbContext.
- Repositories: UserRepository, StudyRepository, SeriesRepository, ImageRepository, AuditLogRepository, SystemSettingRepository, etc.

### 3.4.9. Dependencies

- **Layer Id:** business-logic  
**Type:** Required  

## 3.5. Infrastructure Layer
This layer contains implementations for interacting with external systems and frameworks. It provides concrete implementations for interfaces defined in the application or domain layers.

### 3.5.4. Technologystack
RabbitMQ.Client, fo-dicom, QuestPDF, Serilog, System.IO.Pipes

### 3.5.5. Language
C# 12

### 3.5.6. Type
Infrastructure

### 3.5.7. Responsibilities

- Handle all DICOM network communication (C-STORE, C-FIND, C-MOVE) using fo-dicom. (REQ-1-009)
- Manage publishing and consuming messages with RabbitMQ. (REQ-1-004)
- Implement client/server communication over Named Pipes. (REQ-1-007)
- Interact with the local file system for storing and retrieving DICOM files. (REQ-1-056)
- Generate PDF files using the QuestPDF library. (REQ-1-029)
- Interact with the Windows Print API for spooling print jobs. (REQ-1-020)
- Communicate with the Odoo REST API for license validation over HTTPS. (REQ-1-011)
- Securely store and retrieve secrets using the Windows Credential Manager. (REQ-1-084)

### 3.5.8. Components

- RabbitMqProducer/Consumer
- NamedPipeClient/Server
- DicomFileStorageService
- OdooApiClient
- PdfGeneratorService
- WindowsPrintService
- SecureCredentialStore

### 3.5.9. Dependencies

- **Layer Id:** cross-cutting  
**Type:** Required  

## 3.6. Cross-Cutting Concerns
This logical layer represents functionalities that are used across multiple other layers, such as logging, configuration, and security.

### 3.6.4. Technologystack
Serilog, Microsoft.Extensions.Configuration, BCrypt.Net-Next

### 3.6.5. Language
C# 12

### 3.6.6. Type
CrossCutting

### 3.6.7. Responsibilities

- Provide a consistent logging mechanism using Serilog, writing to a rolling file and the Windows Event Log. (REQ-1-039)
- Ensure all PHI is redacted from logs before writing. (REQ-1-039)
- Manage application configuration settings.
- Handle exception management and reporting.
- Implement security utilities, such as password hashing with BCrypt. (REQ-1-082)
- Generate and propagate correlation IDs for tracing asynchronous operations. (REQ-1-090)

### 3.6.8. Components

- Static Logger class or ILogger interface (via DI)
- Configuration Service
- Global Exception Handler
- PasswordHasher utility

### 3.6.9. Dependencies




---

# 4. Quality Attributes

## 4.1. Security
Protecting data and the system from unauthorized access and threats, with a strong focus on HIPAA compliance.

### 4.1.3. Tactics

- Authentication & Authorization: Mandatory login (REQ-1-040) with a Role-Based Access Control (RBAC) model (REQ-1-014).
- Data Encryption: TLS for all network traffic (PostgreSQL, Odoo API) (REQ-1-083, REQ-1-074). pgcrypto for PHI at rest (REQ-1-083).
- Auditing: Comprehensive audit trail for all significant events, especially PHI access and modifications (REQ-1-047, REQ-1-048, REQ-1-049).
- Secure Credential Management: Use of BCrypt for password hashing (REQ-1-082) and Windows Credential Manager for secrets (REQ-1-084).
- Session Management: Automatic session lock after 15 minutes of inactivity (REQ-1-041).
- Data Anonymization: Features for de-identifying DICOM metadata and pixel data (REQ-1-065, REQ-1-066).

## 4.2. Reliability
Ensuring the system operates correctly and consistently, especially for critical background tasks.

### 4.2.3. Tactics

- Asynchronous Messaging Guarantees: Use of durable RabbitMQ queues and persistent messages to prevent task loss on broker restart (REQ-1-005).
- Error Handling: Dead-letter queue (DLQ) mechanism to isolate and hold messages that fail processing for manual intervention (REQ-1-006).
- Service Recovery: The background Windows Service is configured for automatic restart on failure (REQ-1-086).
- Data Integrity: 'Working Copy' mechanism prevents modification of original data (REQ-1-080). Periodic background checks verify file system integrity against the database (REQ-1-058).
- Backup & Recovery: Defined RPO of 24h and RTO of 4h with documented procedures (REQ-1-088).

## 4.3. Performance
Ensuring the application is responsive and can handle the expected workload efficiently.

### 4.3.3. Tactics

- UI Responsiveness: All long-running operations are offloaded to the background service or executed asynchronously using async/await to prevent blocking the UI thread (REQ-1-079).
- Efficient Data Ingestion: Decoupling the DICOM C-STORE SCP from direct database writes via a message queue allows the SCP to handle high throughput (REQ-1-010, REQ-1-078).
- Optimized Rendering: GPU-accelerated rendering using DirectX (via Vortice.Windows) and progressive loading strategies for large image series (REQ-1-052).
- Database Performance: Use of appropriate indexes on database tables as defined in the data model to speed up queries.

## 4.4. Maintainability
The ease with which the system can be modified, corrected, and enhanced.

### 4.4.3. Tactics

- Separation of Concerns: The Client-Server, Layered, and MVVM architectures create clear boundaries between different parts of the system.
- Loose Coupling: Dependency Injection is used throughout to reduce hard dependencies. Message queues decouple the client from the service.
- Code Cohesion: Functionality is grouped into logical components and layers with single, well-defined responsibilities.
- Technology Standardization: A specific, modern technology stack (.NET 8) is enforced, ensuring consistency (REQ-1-008, REQ-1-013).



---

