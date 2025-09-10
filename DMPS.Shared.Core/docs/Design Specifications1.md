# 1 Sds

## 1.1 Analysis Metadata

| Property | Value |
|----------|-------|
| Analysis Timestamp | 2024-07-27T10:00:00Z |
| Repository Component Id | DMPS.Shared.Core |
| Analysis Completeness Score | 100 |
| Critical Findings Count | 0 |
| Analysis Methodology | Systematic analysis of cached context, cross-refer... |

## 1.2 Repository Analysis

### 1.2.1 Repository Definition

#### 1.2.1.1 Scope Boundaries

- Defines all core domain entities (e.g., Patient, Study, User, Role) and Data Transfer Objects (DTOs) for the entire system.
- Specifies all data access contracts through repository interfaces (e.g., IUserRepository, IStudyRepository) but strictly forbids concrete implementations.
- Contains technology-agnostic core business rules and validation logic (e.g., PasswordPolicyValidator).
- Must not contain any UI, database, or external service-specific code or dependencies, acting as the most stable and central component of the architecture.

#### 1.2.1.2 Technology Stack

- .NET 8.0
- C# 12

#### 1.2.1.3 Architectural Constraints

- Must have zero dependencies on any other repository within the solution.
- All repository interface methods must be asynchronous, returning Task or Task<T>.
- Domain entities must be persistence-ignorant, containing no attributes or logic specific to an ORM like Entity Framework Core.

#### 1.2.1.4 Dependency Relationships

##### 1.2.1.4.1 Provider: REPO-02-DAL

###### 1.2.1.4.1.1 Dependency Type

Provider

###### 1.2.1.4.1.2 Target Component

REPO-02-DAL

###### 1.2.1.4.1.3 Integration Pattern

Interface Implementation

###### 1.2.1.4.1.4 Reasoning

DMPS.Shared.Core defines the data access interfaces (e.g., IUserRepository) that DMPS.Data.Access is required to implement, fulfilling the Dependency Inversion Principle.

##### 1.2.1.4.2.0 Provider: REPO-08-APC

###### 1.2.1.4.2.1 Dependency Type

Provider

###### 1.2.1.4.2.2 Target Component

REPO-08-APC

###### 1.2.1.4.2.3 Integration Pattern

Dependency Injection

###### 1.2.1.4.2.4 Reasoning

DMPS.Shared.Core provides the repository interfaces (e.g., IUserRepository) and domain entities that DMPS.Client.Application consumes to orchestrate business logic.

##### 1.2.1.4.3.0 Provider: REPO-10-BGW

###### 1.2.1.4.3.1 Dependency Type

Provider

###### 1.2.1.4.3.2 Target Component

REPO-10-BGW

###### 1.2.1.4.3.3 Integration Pattern

Dependency Injection

###### 1.2.1.4.3.4 Reasoning

DMPS.Shared.Core provides repository interfaces (e.g., IStudyRepository) and DTOs that DMPS.Service.Worker consumes to process background tasks like DICOM ingestion.

#### 1.2.1.5.0.0 Analysis Insights

This repository is the architectural linchpin of the system, embodying the Shared Kernel pattern. Its primary role is to enforce consistency and decoupling by serving as the single source of truth for domain models and data access contracts. Its stability is paramount, as any change here will ripple throughout the entire application stack. The design correctly emphasizes persistence ignorance and asynchronous operations, setting a strong architectural foundation.

## 1.3.0.0.0.0 Requirements Mapping

### 1.3.1.0.0.0 Functional Requirements

#### 1.3.1.1.0.0 Requirement Id

##### 1.3.1.1.1.0 Requirement Id

REQ-USR-001 (related to REQ-1-014)

##### 1.3.1.1.2.0 Requirement Description

The system must support distinct user roles, such as 'Technician' and 'Administrator'.

##### 1.3.1.1.3.0 Implementation Implications

- Requires the definition of 'User' and 'Role' domain entities.
- The 'User' entity must contain a property or foreign key reference to its associated 'Role'.

##### 1.3.1.1.4.0 Required Components

- User Entity
- Role Entity

##### 1.3.1.1.5.0 Analysis Reasoning

This repository is the designated location for all domain models, making it the correct place to define the 'User' and 'Role' entities that are fundamental to the system's role-based access control.

#### 1.3.1.2.0.0 Requirement Id

##### 1.3.1.2.1.0 Requirement Id

REQ-FNC-001 (related to REQ-1-033)

##### 1.3.1.2.2.0 Requirement Description

Users must be able to submit print jobs for asynchronous processing.

##### 1.3.1.2.3.0 Implementation Implications

- Requires a Data Transfer Object (DTO) to encapsulate all information for a print job command (e.g., 'SubmitPrintJobCommand').
- A 'PrintJob' domain entity is required to model the state and details of a print job as it is tracked in the system.

##### 1.3.1.2.4.0 Required Components

- PrintJob Entity
- SubmitPrintJobCommand DTO

##### 1.3.1.2.5.0 Analysis Reasoning

As per its scope, this repository defines the contracts for inter-process communication. The 'SubmitPrintJobCommand' DTO is the contract for messages sent from the client to the service via RabbitMQ.

#### 1.3.1.3.0.0 Requirement Id

##### 1.3.1.3.1.0 Requirement Id

REQ-FNC-004 (related to REQ-1-047)

##### 1.3.1.3.2.0 Requirement Description

The system must maintain a comprehensive audit trail of significant user actions.

##### 1.3.1.3.3.0 Implementation Implications

- Requires an 'AuditLog' or 'AuditLogEntry' domain entity to model the structure of an audit record.
- Requires an 'IAuditLogRepository' interface to define the contract for persisting audit entries.

##### 1.3.1.3.4.0 Required Components

- AuditLog Entity
- IAuditLogRepository Interface

##### 1.3.1.3.5.0 Analysis Reasoning

The definition of the audit log's data structure and its persistence contract is a core, cross-cutting domain concern, placing it squarely within the responsibility of the Shared Kernel.

### 1.3.2.0.0.0 Non Functional Requirements

#### 1.3.2.1.0.0 Requirement Type

##### 1.3.2.1.1.0 Requirement Type

Maintainability

##### 1.3.2.1.2.0 Requirement Specification

The system must be designed with a clear separation of concerns to be easily modified and enhanced.

##### 1.3.2.1.3.0 Implementation Impact

This repository is the primary enabler of maintainability. By centralizing domain models and interfaces, it creates a single point of definition and decouples business logic from implementation details.

##### 1.3.2.1.4.0 Design Constraints

- Must remain free of dependencies on concrete implementation libraries (e.g., EF Core, RabbitMQ.Client).
- Interface contracts must be stable.

##### 1.3.2.1.5.0 Analysis Reasoning

The Shared Kernel pattern is a direct, strategic response to the requirement for high maintainability in a complex system.

#### 1.3.2.2.0.0 Requirement Type

##### 1.3.2.2.1.0 Requirement Type

Testability

##### 1.3.2.2.2.0 Requirement Specification

All application and business logic must be unit-testable without requiring a live database or UI framework.

##### 1.3.2.2.3.0 Implementation Impact

This repository enables testability by defining interfaces for all data access operations. Other layers can then use dependency injection to consume these interfaces, allowing them to be easily mocked or stubbed in unit tests.

##### 1.3.2.2.4.0 Design Constraints

- Interfaces must fully define the required data operations.
- Entities must be simple POCOs or records, easily instantiated in test setups.

##### 1.3.2.2.5.0 Analysis Reasoning

The interface-based contracts are a deliberate architectural choice to facilitate the testability of dependent components like the Application Services layer.

### 1.3.3.0.0.0 Requirements Analysis Summary

This repository is the cornerstone for fulfilling the system's core requirements. It translates business needs like user roles, printing, and auditing into concrete, technology-agnostic C# types (entities, DTOs, interfaces). It directly enables critical non-functional requirements like maintainability and testability by establishing a decoupled, contract-based architecture. All foundational data structures and persistence contracts required by other repositories originate here.

## 1.4.0.0.0.0 Architecture Analysis

### 1.4.1.0.0.0 Architectural Patterns

#### 1.4.1.1.0.0 Pattern Name

##### 1.4.1.1.1.0 Pattern Name

Shared Kernel

##### 1.4.1.1.2.0 Pattern Application

This repository acts as the Shared Kernel for the entire solution, containing the common domain model and interfaces shared by both the client and service applications.

##### 1.4.1.1.3.0 Required Components

- Domain Entities
- Repository Interfaces
- DTOs

##### 1.4.1.1.4.0 Implementation Strategy

The repository will be built as a standalone .NET 8 class library with zero external dependencies, distributed as a NuGet package to all other projects in the solution. It will be the most stable and central component.

##### 1.4.1.1.5.0 Analysis Reasoning

The choice of a Shared Kernel is explicitly stated and is ideal for ensuring consistency and reducing code duplication between the separate client (REPO-09-PRE) and server (REPO-10-BGW) processes.

#### 1.4.1.2.0.0 Pattern Name

##### 1.4.1.2.1.0 Pattern Name

Domain-Driven Design (DDD)

##### 1.4.1.2.2.0 Pattern Application

The repository contains the building blocks of the domain model, including Entities (Patient, Study, User), and defines contracts for data access using the Repository pattern.

##### 1.4.1.2.3.0 Required Components

- User Entity
- Study Entity
- IUserRepository
- IStudyRepository

##### 1.4.1.2.4.0 Implementation Strategy

Entities will be implemented as plain C# classes (POCOs) or records, focusing on encapsulating data and behavior without any persistence-related concerns. Interfaces will define the data access methods required by the domain.

##### 1.4.1.2.5.0 Analysis Reasoning

DDD is appropriate for a system with complex business rules like a medical application. This repository correctly isolates the domain model from other technical concerns, which is a core tenet of DDD.

### 1.4.2.0.0.0 Integration Points

- {'integration_type': '.NET Assembly Reference', 'target_components': ['REPO-02-DAL', 'REPO-08-APC', 'REPO-10-BGW'], 'communication_pattern': 'In-Process/Direct Method Calls', 'interface_requirements': ['All public types (classes, interfaces, records, enums) in this repository constitute its API.', 'Consuming projects will reference the compiled DLL or the project directly.'], 'analysis_reasoning': 'As the foundational library, all other C# projects in the solution will reference it to gain access to the shared domain models and service contracts. This is the standard integration pattern for a Shared Kernel in a .NET solution.'}

### 1.4.3.0.0.0 Layering Strategy

| Property | Value |
|----------|-------|
| Layer Organization | This repository constitutes the core of the 'Busin... |
| Component Placement | It contains only domain entities, value objects, d... |
| Analysis Reasoning | This strict placement enforces the Dependency Rule... |

## 1.5.0.0.0.0 Database Analysis

### 1.5.1.0.0.0 Entity Mappings

#### 1.5.1.1.0.0 Entity Name

##### 1.5.1.1.1.0 Entity Name

User

##### 1.5.1.1.2.0 Database Table

User

##### 1.5.1.1.3.0 Required Properties

- userId (Guid), username (string), passwordHash (string), roleId (Guid), isActive (bool)
- The 'User' entity must model the data structure required by the 'User' table in the database design.

##### 1.5.1.1.4.0 Relationship Mappings

- A User has one Role.

##### 1.5.1.1.5.0 Access Patterns

- Retrieval by username (GetUserByUsernameAsync)
- Standard CRUD operations (AddUserAsync, UpdateUserAsync, DeleteUserAsync)

##### 1.5.1.1.6.0 Analysis Reasoning

The User entity is defined here as a clean POCO, with the responsibility of mapping it to the database table delegated to the Data Access Layer, which will implement the IUserRepository interface.

#### 1.5.1.2.0.0 Entity Name

##### 1.5.1.2.1.0 Entity Name

Study

##### 1.5.1.2.2.0 Database Table

Study

##### 1.5.1.2.3.0 Required Properties

- studyId (Guid), patientId (Guid), studyInstanceUid (string), studyDate (DateTime)
- The entity must contain all fields specified in the 'Study' table design that are relevant to the domain.

##### 1.5.1.2.4.0 Relationship Mappings

- A Study belongs to one Patient.
- A Study contains many Series.

##### 1.5.1.2.5.0 Access Patterns

- Retrieval by Study Instance UID (GetStudyByUidAsync)
- Complex searching based on criteria (FindStudiesAsync)
- Creation of new studies (AddStudyAsync)

##### 1.5.1.2.6.0 Analysis Reasoning

The Study entity serves as the aggregate root for the DICOM study hierarchy. It is defined here to be used by both the application services for business logic and the data access layer for persistence.

### 1.5.2.0.0.0 Data Access Requirements

#### 1.5.2.1.0.0 Operation Type

##### 1.5.2.1.1.0 Operation Type

User Management

##### 1.5.2.1.2.0 Required Methods

- Task<User> GetUserByUsernameAsync(string username)
- Task AddUserAsync(User user)
- Task UpdateUserAsync(User user)
- Task DeleteUserAsync(Guid userId)

##### 1.5.2.1.3.0 Performance Constraints

GetUserByUsernameAsync must be highly performant, as it is called during every login attempt.

##### 1.5.2.1.4.0 Analysis Reasoning

The 'IUserRepository' interface defines the complete set of data operations required for user management features, as seen in sequences like SEQ-AFL-001 and SEQ-BUP-004.

#### 1.5.2.2.0.0 Operation Type

##### 1.5.2.2.1.0 Operation Type

DICOM Data Management

##### 1.5.2.2.2.0 Required Methods

- Task<Study> GetStudyByUidAsync(string studyInstanceUid)
- Task<IEnumerable<Study>> FindStudiesAsync(SearchCriteria criteria)
- Task AddStudyAsync(Study study)

##### 1.5.2.2.3.0 Performance Constraints

FindStudiesAsync must be implemented efficiently to support responsive browsing of the study list.

##### 1.5.2.2.4.0 Analysis Reasoning

The 'IStudyRepository' interface defines the core operations needed to ingest and retrieve DICOM study data, as required by sequences SEQ-EVP-002 and SEQ-DTA-015.

### 1.5.3.0.0.0 Persistence Strategy

| Property | Value |
|----------|-------|
| Orm Configuration | This repository requires a persistence-ignorant ap... |
| Migration Requirements | This repository drives the need for database migra... |
| Analysis Reasoning | This strategy enforces a clean separation between ... |

## 1.6.0.0.0.0 Sequence Analysis

### 1.6.1.0.0.0 Interaction Patterns

#### 1.6.1.1.0.0 Sequence Name

##### 1.6.1.1.1.0 Sequence Name

SEQ-AFL-001: User Login - Successful Authentication

##### 1.6.1.1.2.0 Repository Role

Provides the contract ('IUserRepository') and the data model ('User' entity).

##### 1.6.1.1.3.0 Required Interfaces

- IUserRepository

##### 1.6.1.1.4.0 Method Specifications

- {'method_name': 'GetUserByUsernameAsync', 'interaction_context': "Called by the AuthenticationService to retrieve a user's record from the database based on their provided username.", 'parameter_analysis': "Accepts a single 'string username'.", 'return_type_analysis': "Returns a 'Task<User>', which will resolve to the 'User' domain entity if found, or null otherwise.", 'analysis_reasoning': 'This method is the primary data retrieval operation for the authentication process. Its definition in the Shared Kernel allows the application service to be completely decoupled from the database implementation.'}

##### 1.6.1.1.5.0 Analysis Reasoning

The sequence demonstrates the core value of this repository: the application service ('REPO-08-APC') depends only on the 'IUserRepository' interface and 'User' entity, not on how or where the user data is stored.

#### 1.6.1.2.0.0 Sequence Name

##### 1.6.1.2.1.0 Sequence Name

SEQ-EVP-003: Asynchronous Print Job Submission

##### 1.6.1.2.2.0 Repository Role

Provides the DTO ('SubmitPrintJobCommand') used as the message payload.

##### 1.6.1.2.3.0 Required Interfaces

*No items available*

##### 1.6.1.2.4.0 Method Specifications

*No items available*

##### 1.6.1.2.5.0 Analysis Reasoning

This sequence shows the repository's role in defining communication contracts. The 'SubmitPrintJobCommand' DTO is a shared, technology-agnostic data structure defined here, ensuring the client and service agree on the message format for print jobs.

### 1.6.2.0.0.0 Communication Protocols

- {'protocol_type': 'In-Process (.NET Method Calls)', 'implementation_requirements': 'Consumers will add a project reference to this repository. All interactions will be through standard C# interface method calls and object instantiation.', 'analysis_reasoning': 'This is the standard and most efficient communication protocol for a shared library within a single-language ecosystem like .NET.'}

## 1.7.0.0.0.0 Critical Analysis Findings

*No items available*

## 1.8.0.0.0.0 Analysis Traceability

### 1.8.1.0.0.0 Cached Context Utilization

Analysis was performed by systematically processing all provided cached context elements: the target repository definition (REPO-01-SHK), the overall architecture document, the database design, and all relevant sequence diagrams (SEQ-AFL-001, SEQ-EVP-003, etc.).

### 1.8.2.0.0.0 Analysis Decision Trail

- Determined the repository's primary function as a Shared Kernel based on its description and architectural constraints.
- Mapped explicit requirements (REQ-USR-001, etc.) to specific components (User entity, PrintJob DTO).
- Validated the repository's role in the layered architecture as the central domain layer.
- Confirmed that entity definitions align with the database schema, establishing the link between the domain model and persistence.
- Verified that sequence diagrams correctly utilize the interfaces and DTOs defined by this repository.

### 1.8.3.0.0.0 Assumption Validations

- Verified that the repository's 'must_not_implement' constraints are strict and central to its design.
- Confirmed that the 'async' nature of repository interfaces is a deliberate design choice to enforce non-blocking I/O operations throughout the system.

### 1.8.4.0.0.0 Cross Reference Checks

- Cross-referenced 'exposed_contracts' with 'consumers' in other repository definitions to validate dependency relationships.
- Cross-referenced entity definitions with the 'DATABASE DESIGN' to ensure alignment.
- Cross-referenced sequence diagram method calls with the method signatures defined in the repository interfaces.

# 2.0.0.0.0.0 Code Instructions

#TECH and REPO STRUCTURE INSTRUCTIONS\n# SharedKernel REPOSITORY GUIDELINES FOR .NET 8.0\nAct as an expert SharedKernel architect with deep expertise in .NET 8.0 development, focusing on designing robust, maintainable, and highly performant foundational libraries and shared domain models using modern C# features and .NET 8.0's native capabilities. Ensure all outputs maintain high type safety, strong domain purity, and optimal runtime performance while optimizing for Dependency Injection abstractions, record types for immutability, and asynchronous programming best practices.\n\n- Technology-Informed Thinking Steps\n{\n  \"thinking_steps\": [\n    {\n      \"step\": \"Understand SharedKernel's Core Domain Responsibilities\",\n      \"details\": \"Identified the primary domain concerns, functional patterns, and architectural requirements specific to SharedKernel repositories, including defining core DDD building blocks (Entities, Value Objects, Aggregates, Domain Events), common interfaces for application and infrastructure layers, and generic utilities to promote consistency and reduce duplication across the solution.\"\n    },\n    {\n      \"step\": \"Analyze .NET 8.0 Framework Capabilities and Patterns\",\n      \"details\": \"Assessed .NET 8.0-native architectural patterns, framework conventions, version-specific features (e.g., Records, Primary Constructors, Nullable Reference Types, Collection Expressions), and optimization opportunities (e.g., improved 'System.Text.Json' performance, AOT compatibility) that align with SharedKernel's requirements for type safety, immutability, and performance.\"\n    },\n    {\n      \"step\": \"Synthesize Technology-Repository Alignment\",\n      \"details\": \"Determined optimal integration points between SharedKernel domain requirements and .NET 8.0 framework capabilities, identifying native patterns like using 'record' types for Value Objects, primary constructors for concise types, 'Task' and 'ValueTask' for asynchronous operations, and leveraging nullable reference types for robust API contracts, to ensure strong type safety and maintainability.\"\n    },\n    {\n      \"step\": \"Design Framework-Native Repository Structure\",\n      \"details\": \"Architected repository organization using .NET 8.0-specific conventions, including a clear namespace and folder structure (e.g., 'Domain', 'Application.Abstractions', 'Infrastructure.Abstractions', 'Common'), leveraging '.csproj' for dependency management and ensuring adherence to Clean/Onion Architecture principles via interface-based design.\"\n    },\n    {\n      \"step\": \"Integrate Technology-Specific Quality Patterns\",\n      \"details\": \"Embedded .NET 8.0-native patterns for quality, including argument validation using custom guard clauses, strong typing with nullable reference types, promoting immutability with records, defining a structured exception hierarchy, and providing interfaces for logging and time abstraction suitable for testability and maintainability within SharedKernel implementations.\"\n    }\n  ]\n}\n\n- .NET 8.0-Optimized Structure for defining core domain and cross-cutting abstractions including:\n  *   **Domain Model Abstractions**: Base classes and interfaces for Entities, Value Objects, Aggregates, and Domain Events, leveraging C# 'record' types and primary constructors for concise, immutable definitions, coupled with Nullable Reference Types for strong compile-time null safety.\n  *   **Application Layer Contracts**: Generic interfaces for Commands, Queries, and Handlers ('ICommand', 'IQuery', 'ICommandHandler<TCommand>', 'IQueryHandler<TQuery, TResult>'), along with shared Data Transfer Objects (DTOs), designed for interoperability and efficient serialization with 'System.Text.Json'.\n  *   **Infrastructure Abstraction Interfaces**: Common interfaces for data repositories ('IRepository<TEntity, TId>'), Unit of Work ('IUnitOfWork'), and external service contracts (e.g., 'IEmailSender', 'IDateTimeService'), promoting loose coupling and testability through Dependency Injection.\n  *   **Cross-Cutting Utilities**: Essential helper classes, extension methods (e.g., 'Argument' validation via guard clauses, string manipulation), and generic types ('Result<T>' or 'Maybe<T>'), optimized for performance and type safety with Nullable Reference Types.\n  *   **Custom Exception Types**: A hierarchy of domain-specific exceptions (e.g., 'DomainException', 'NotFoundException', 'ValidationException') that inherit from 'System.Exception', providing structured error reporting and clear semantic meaning across the system.\n  *   **Specification Pattern Implementations**: Base classes and interfaces for defining query specifications ('ISpecification<TEntity>'), enabling reusable and composable filtering logic, leveraging LINQ expressions and compiled delegates for efficient data access.\n  *   **Time Abstraction**: An 'ITimeProvider' interface for abstracting system time, allowing for deterministic testing and dependency injection of time, directly aligning with and extending .NET 8.0's 'TimeProvider' concept where applicable.\n  *   **Logging Abstractions**: Minimal interfaces for logging ('ILoggerAdapter') or definitions for source-generated log messages via 'LoggerMessage' attributes, integrating seamlessly with 'Microsoft.Extensions.Logging.Abstractions' for high-performance and structured logging.\n\n- **Technology-Informed Architectural Principle 1**: **Purity of Domain Models with C# Records and Primary Constructors**: All core domain entities and value objects are defined using C# 'record' types (classes or structs) to enforce immutability and value equality semantics where appropriate. Primary constructors are leveraged to reduce boilerplate, enhance readability, and ensure all required dependencies or properties are initialized at object creation, aligning with functional programming principles within an OOP context.\n- **Framework-Native Architectural Principle 2**: **Interface-Driven Design for DI and Testability**: The SharedKernel exclusively defines interfaces for application services, infrastructure services (like 'IRepository<T>'), and cross-cutting concerns (e.g., 'ITimeProvider'). This maximizes adherence to the Dependency Inversion Principle, facilitating seamless integration with .NET's built-in Dependency Injection container and enabling robust unit and integration testing by allowing mock or stub implementations.\n- **Version-Optimized Architectural Principle 3**: **Robust Type Safety with Nullable Reference Types and Collection Expressions**: Nullable Reference Types (NRT) are fully enabled and enforced ('<Nullable>enable</Nullable>' in '.csproj') across all SharedKernel types to eliminate common 'NullReferenceException' errors at compile time, creating safer and more predictable API contracts. Additionally, C# 12's collection expressions are utilized for concise and readable initialization of collection properties within domain models and DTOs.\n- **Technology-Specific Quality Principle**: **Structured Error Handling with Custom Exceptions and Argument Guards**: The SharedKernel defines a clear hierarchy of custom domain-specific exceptions to provide granular and semantically rich error reporting. Furthermore, a 'Guard' clause utility (or extension methods on 'Argument') is extensively used for method argument validation, ensuring that domain invariants and preconditions are met early in the execution flow, thereby improving robustness and maintainability, and providing clear error messages aligned with .NET's 'ArgumentException' patterns.\n\n\n\n# Layer enhancement Instructions\n## SharedKernel REPOSITORY CONSIDERATIONS FOR .NET 8.0\n\n{\n  \"thinking_steps\": [\n    {\n      \"step\": \"Understand SharedKernel's Core Structural Requirements\",\n      \"details\": \"Identified the primary organizational patterns, directory structures, and file organization conventions specific to SharedKernel repositories, including **stability, domain purity, ubiquitous language adherence, minimal dependencies, and clear boundaries for shared domain concepts (Value Objects, Entities, Domain Events, common Interfaces/Abstractions)**\"\n    },\n    {\n      \"step\": \"Analyze .NET 8.0 Framework-Native Organization Patterns\",\n      \"details\": \"Assessed .NET 8.0-specific directory conventions, configuration file patterns, and framework-native organizational approaches that optimize repository structure, focusing on **'.csproj' for class libraries, NuGet for packaging, consistent namespace conventions, folder-per-concept, and C# 10+ features like 'global using'**\"\n    },\n    {\n      \"step\": \"Synthesize Technology-Repository Structural Alignment\",\n      \"details\": \"Determined optimal integration between SharedKernel organizational requirements and .NET 8.0 framework conventions, identifying native structural patterns such as **mapping shared domain concepts to a single, stable '.NET' class library project ('.csproj') designed for NuGet distribution, with logical grouping of domain concepts into namespaces and corresponding folders**\"\n    },\n    {\n      \"step\": \"Design Framework-Native Repository Organization\",\n      \"details\": \"Architected repository structure using .NET 8.0-specific conventions, configuration patterns, and framework-native separation of concerns by creating a **single '.csproj' (e.g., 'YourCompany.SharedKernel.Domain.csproj') with distinct subfolders for specific domain primitives (e.g., 'ValueObjects', 'Events'), explicitly configured for easy NuGet package generation**\"\n    },\n    {\n      \"step\": \"Optimize for Technology Ecosystem Integration\",\n      \"details\": \"Structured repository for optimal integration with .NET 8.0 tooling, build processes, and ecosystem conventions while maintaining SharedKernel domain clarity by **ensuring the '.csproj' is correctly configured for standard 'dotnet build' and 'dotnet pack' commands, adhering to public API design guidelines, and minimizing external dependencies to maintain stability and compatibility across consuming contexts**\"\n    }\n  ]\n}\n\nWhen building the .NET 8.0-optimized structure for this SharedKernel repository type, prioritize:\n\n-   **NuGet Packageability**: The SharedKernel's primary purpose is to be a stable, reusable library distributed as a NuGet package; therefore, the '.csproj' must be meticulously configured for seamless packaging and versioning.\n-   **Namespace-to-Folder Alignment**: Adhere strictly to .NET's convention where the folder hierarchy directly corresponds to the namespace structure, enhancing code discoverability, maintainability, and tooling integration.\n-   **Minimal Project References with 'net8.0'**: Leverage the 'net8.0' target framework to utilize the latest BCL features directly, thereby minimizing external project dependencies and ensuring a lean, stable dependency graph crucial for a SharedKernel.\n-   **'global using' Directives for Common Types**: Utilize C# 10+ 'global using' functionality to centralize common namespace imports, reducing boilerplate in individual files and simplifying the codebase for both developers within the SharedKernel and its consumers.\n\nEssential .NET 8.0-native directories and files should include:\n*   **'src/YourCompany.SharedKernel.Domain/YourCompany.SharedKernel.Domain.csproj'**: The core .NET 8.0 Class Library project file, meticulously configured with target framework ('net8.0'), essential NuGet package metadata (e.g., 'PackageId', 'Version', 'Authors', 'Description', 'PackageReadmeFile'), and release build settings.\n*   **'src/YourCompany.SharedKernel.Domain/ValueObjects/'**: A dedicated directory for all immutable, comparable Value Objects (e.g., 'Money.cs', 'Address.cs', 'Email.cs'), leveraging C# 9+ 'record' types for concise definition and inherent immutability.\n*   **'src/YourCompany.SharedKernel.Domain/Entities/'**: Contains abstract base classes or interfaces that define common characteristics for Entities and Aggregate Roots (e.g., 'EntityBase.cs', 'IAggregateRoot.cs'), ensuring a consistent foundation across all Bounded Contexts that use these core types.\n*   **'src/YourCompany.SharedKernel.Domain/Events/'**: Stores definitions for common Domain Events (e.g., 'IDomainEvent.cs', 'DomainEventBase.cs', 'EntityCreatedEvent.cs') that represent significant occurrences within the shared domain, facilitating inter-context communication.\n*   **'src/YourCompany.SharedKernel.Domain/Interfaces/'**: Holds generic interfaces and abstractions that define contracts for domain services, repositories, or other infrastructural components expected to be implemented by consuming Bounded Contexts (e.g., 'IGenericRepository<T>.cs', 'IUnitOfWork.cs').\n*   **'src/YourCompany.SharedKernel.Domain/Exceptions/'**: A structured directory for custom, domain-specific exception types (e.g., 'DomainException.cs', 'InvalidOperationException.cs' if a more specific base is needed) that convey meaning in the ubiquitous language.\n*   **'src/YourCompany.SharedKernel.Domain/Primitives/'**: For fundamental, basic types, enums, or simple struct definitions that are universally shared and do not fit neatly into Value Objects or Entities (e.g., 'Guard.cs' for common validation, 'Result<T>.cs' for functional error handling).\n*   **'src/YourCompany.SharedKernel.Domain/Usings.cs' (or defined in '.csproj')**: A centralized file (or configuration block) leveraging C# 'global using' directives to include frequently used namespaces within the SharedKernel and for its consumers, reducing verbosity and improving code clarity.\n*   **'Directory.Build.props' (at solution root or higher)**: A build configuration file leveraging MSBuild properties to define common project settings (e.g., 'LangVersion', 'ImplicitUsings', 'Nullable', 'TreatWarningsAsErrors') that apply uniformly to all projects within the solution, including the SharedKernel, promoting consistency.\n\nCritical .NET 8.0-optimized interfaces with other components:\n*   **Public NuGet API Surface**: The entire public API (classes, interfaces, enums, records, methods) exposed by the 'YourCompany.SharedKernel.Domain' NuGet package serves as the primary, versioned interface for all consuming Bounded Contexts, strictly adhering to Semantic Versioning.\n*   **Shared .NET Types and Interfaces**: Exposes highly stable C# interfaces (e.g., 'IAggregateRoot', 'IDomainEvent') and base types (e.g., 'EntityBase') that Bounded Contexts implement or inherit from, ensuring type compatibility and a common language across the system.\n*   **Immutable Record Types for Value Objects**: Leverages C# 9+ 'record' types for Value Objects, providing inherent immutability, value-based equality, and concise syntax, which streamlines their consumption and guarantees predictable behavior in contexts that rely on these shared types.\n\nFor this SharedKernel repository type with .NET 8.0, the JSON structure should particularly emphasize:\n-   **Technology-Informed File Organization Pattern 1**: **Project-Centric Domain Grouping**: The SharedKernel is organized as a distinct '.csproj' specifically designed to be a self-contained, stable domain library, facilitating its independent development, testing, and NuGet distribution.\n-   **Framework-Specific File Organization Pattern 2**: **Explicit Namespace-Folder Mapping**: Directories are meticulously aligned with .NET's conventional namespace hierarchy (e.g., 'ValueObjects' folder maps to 'YourCompany.SharedKernel.Domain.ValueObjects' namespace), ensuring natural discoverability and framework tooling compatibility.\n-   **Version-Optimized File Organization Pattern 3**: **'global using' Centralization**: Modern C# 10+ 'global using' directives are strategically employed (either in 'Usings.cs' or via '.csproj') to consolidate common namespace imports, streamlining code and enhancing developer productivity within and for consumers of the SharedKernel.\n-   **Technology-Integrated File Organization Pattern 4**: **NuGet Packaging Metadata**: The '.csproj' is enriched with comprehensive NuGet-specific metadata ('<PackageId>', '<Version>', '<Authors>', '<Description>', '<RepositoryType>', '<RepositoryUrl>') to ensure the SharedKernel is a fully compliant and easily consumable package within the .NET ecosystem.\n

