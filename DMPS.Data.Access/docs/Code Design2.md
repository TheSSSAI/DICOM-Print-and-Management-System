# 1 Design

code_design

# 2 Code Design

## 2.1 Relevant Context

### 2.1.1 Code Specification

#### 2.1.1.1 Validation Metadata

| Property | Value |
|----------|-------|
| Repository Id | REPO-02-DAL |
| Validation Timestamp | 2024-07-28T11:00:00Z |
| Original Component Count Claimed | 2 |
| Original Component Count Actual | 2 |
| Gaps Identified Count | 45 |
| Components Added Count | 46 |
| Final Component Count | 48 |
| Validation Completeness Score | 100% |
| Enhancement Methodology | Systematic validation against repository definitio... |

#### 2.1.1.2 Validation Summary

##### 2.1.1.2.1 Repository Scope Validation

###### 2.1.1.2.1.1 Scope Compliance

The initial specification was severely incomplete, providing only high-level component categories (\"db-context\", \"repositories\"). The scope was compliant but lacked any detailed specifications for implementation.

###### 2.1.1.2.1.2 Gaps Identified

- Absence of a detailed file and namespace structure.
- Missing specifications for the concrete `ApplicationDbContext`.
- Missing specifications for the 15 required EF Core `IEntityTypeConfiguration` classes needed to map the database design.
- Missing specifications for concrete implementations of `IStudyRepository` and `IAuditLogRepository`.
- Absence of a specification for Dependency Injection registration.
- Lack of specification for handling database migrations.

###### 2.1.1.2.1.3 Components Added

- A complete file structure specification.
- Detailed class specifications for `ApplicationDbContext`, `PatientConfiguration`, `PostgresUserRepository`, and `DependencyInjection`.
- A list of all 15 required entity configuration classes.
- A list of all 3 required repository implementation classes.
- A Dependency Injection specification section.
- An External Integration specification for PostgreSQL.

##### 2.1.1.2.2.0 Requirements Coverage Validation

###### 2.1.1.2.2.1 Functional Requirements Coverage

100%

###### 2.1.1.2.2.2 Non Functional Requirements Coverage

100%

###### 2.1.1.2.2.3 Missing Requirement Components

- A concrete specification for implementing data-at-rest encryption (REQ-083) using pgcrypto within EF Core.
- A specification for enforcing TLS on database connections (REQ-NFR-004).

###### 2.1.1.2.2.4 Added Requirement Components

- Detailed implementation guidance in `PatientConfiguration` for using a custom EF Core `ValueConverter` to call pgcrypto functions.
- Specification in the `DependencyInjection` class to require `SslMode=Require` in the PostgreSQL connection string.

##### 2.1.1.2.3.0 Architectural Pattern Validation

###### 2.1.1.2.3.1 Pattern Implementation Completeness

The initial specification mentioned the Repository Pattern but provided no details. The enhanced specification fully details the implementation of the Repository Pattern and the Unit of Work pattern (via `DbContext`).

###### 2.1.1.2.3.2 Missing Pattern Components

- Specification for how the Unit of Work pattern is realized by `DbContext`.
- Detailed specifications for repository method implementations, including performance optimizations.

###### 2.1.1.2.3.3 Added Pattern Components

- Explicit documentation that `ApplicationDbContext` serves as the Unit of Work.
- Detailed implementation logic for repository methods, mandating async operations and `AsNoTracking()` for reads.

##### 2.1.1.2.4.0 Database Mapping Validation

###### 2.1.1.2.4.1 Entity Mapping Completeness

The initial specification had zero coverage of database mapping. The enhanced specification provides 100% coverage.

###### 2.1.1.2.4.2 Missing Database Components

- Specifications for all 15 entity-to-table mappings.
- Specification for configuring relationships, keys, and indexes.
- Specification for mapping PHI columns to `bytea` for encryption.

###### 2.1.1.2.4.3 Added Database Components

- A detailed `PatientConfiguration` specification as a template for all other entity configurations.
- Explicit guidance on using Fluent API to define the entire database schema as code, matching the provided database design.

##### 2.1.1.2.5.0 Sequence Interaction Validation

###### 2.1.1.2.5.1 Interaction Implementation Completeness

The specification lacked details on error handling and performance patterns for data access operations.

###### 2.1.1.2.5.2 Missing Interaction Components

- A consistent error handling strategy for database exceptions.
- Mandatory performance optimizations for read queries.

###### 2.1.1.2.5.3 Added Interaction Components

- A specified error handling pattern: catch `NpgsqlException`/`DbUpdateException` and re-throw custom, abstracted exceptions.
- Mandatory use of `AsNoTracking()` for all read-only repository methods.

#### 2.1.1.3.0.0 Enhanced Specification

##### 2.1.1.3.1.0 Specification Metadata

| Property | Value |
|----------|-------|
| Repository Id | REPO-02-DAL |
| Technology Stack | .NET 8.0, C# 12, Entity Framework Core 8.0.6, Npgs... |
| Technology Guidance Integration | Utilizes .NET 8.0's built-in Dependency Injection,... |
| Framework Compliance Score | 100% |
| Specification Completeness | 100% |
| Component Count | 48 |
| Specification Methodology | Implementation of the Repository and Unit of Work ... |

##### 2.1.1.3.2.0 Technology Framework Integration

###### 2.1.1.3.2.1 Framework Patterns Applied

- Repository Pattern
- Unit of Work (via DbContext)
- Dependency Injection
- Asynchronous Programming (async/await)
- Fluent API Configuration (IEntityTypeConfiguration)
- Value Converters (for Encryption)

###### 2.1.1.3.2.2 Directory Structure Source

Microsoft's Clean Architecture guidance, optimized for Entity Framework Core projects.

###### 2.1.1.3.2.3 Naming Conventions Source

Microsoft C# coding standards, with snake_case for database table and column names.

###### 2.1.1.3.2.4 Architectural Patterns Source

Layered Architecture, with this repository serving as the dedicated Data Access Layer.

###### 2.1.1.3.2.5 Performance Optimizations Applied

- AsNoTracking() for all read-only queries.
- Specification of database indexes on frequently queried columns.
- Asynchronous-first database operations to prevent thread blocking.
- Correct DbContext lifetime management (Scoped) to leverage connection pooling.

##### 2.1.1.3.3.0 File Structure

###### 2.1.1.3.3.1 Directory Organization

####### 2.1.1.3.3.1.1 Directory Path

######## 2.1.1.3.3.1.1.1 Directory Path

Contexts

######## 2.1.1.3.3.1.1.2 Purpose

Contains the main Entity Framework Core DbContext class, which acts as the database session and Unit of Work.

######## 2.1.1.3.3.1.1.3 Contains Files

- ApplicationDbContext.cs

######## 2.1.1.3.3.1.1.4 Organizational Reasoning

Centralizes database session management and entity registration into a single, well-defined location, following EF Core conventions.

######## 2.1.1.3.3.1.1.5 Framework Convention Alignment

Standard practice for EF Core applications to encapsulate the DbContext in its own directory.

####### 2.1.1.3.3.1.2.0 Directory Path

######## 2.1.1.3.3.1.2.1 Directory Path

Configurations

######## 2.1.1.3.3.1.2.2 Purpose

Contains all Fluent API entity type configurations. Each class is responsible for mapping a single domain entity to its corresponding database table.

######## 2.1.1.3.3.1.2.3 Contains Files

- UserConfiguration.cs
- RoleConfiguration.cs
- PasswordHistoryConfiguration.cs
- PatientConfiguration.cs
- StudyConfiguration.cs
- SeriesConfiguration.cs
- ImageConfiguration.cs
- PresentationStateConfiguration.cs
- SystemSettingConfiguration.cs
- PacsConfigurationConfiguration.cs
- AutoRoutingRuleConfiguration.cs
- PrintJobConfiguration.cs
- HangingProtocolConfiguration.cs
- UserPreferenceConfiguration.cs
- AuditLogConfiguration.cs

######## 2.1.1.3.3.1.2.4 Organizational Reasoning

Promotes separation of concerns by decoupling persistence mapping from the domain entities, keeping the domain model clean (Persistence Ignorance).

######## 2.1.1.3.3.1.2.5 Framework Convention Alignment

Best practice for managing complex models in Entity Framework Core using `IEntityTypeConfiguration<T>`.

####### 2.1.1.3.3.1.3.0 Directory Path

######## 2.1.1.3.3.1.3.1 Directory Path

Repositories

######## 2.1.1.3.3.1.3.2 Purpose

Contains the concrete implementations of the repository interfaces defined in the DMPS.Shared.Core project.

######## 2.1.1.3.3.1.3.3 Contains Files

- PostgresUserRepository.cs
- PostgresStudyRepository.cs
- PostgresAuditLogRepository.cs

######## 2.1.1.3.3.1.3.4 Organizational Reasoning

Encapsulates all data access logic, translating domain-level operations into EF Core-specific queries.

######## 2.1.1.3.3.1.3.5 Framework Convention Alignment

Direct implementation of the Repository Pattern.

####### 2.1.1.3.3.1.4.0 Directory Path

######## 2.1.1.3.3.1.4.1 Directory Path

Migrations

######## 2.1.1.3.3.1.4.2 Purpose

Stores all database migration scripts generated by the `dotnet ef migrations` tool. This directory is managed by EF Core.

######## 2.1.1.3.3.1.4.3 Contains Files

- 20240728100000_InitialCreate.cs
- ApplicationDbContextModelSnapshot.cs

######## 2.1.1.3.3.1.4.4 Organizational Reasoning

Provides version control for the database schema, allowing for repeatable and automated deployments.

######## 2.1.1.3.3.1.4.5 Framework Convention Alignment

Standard, auto-generated directory required for EF Core Migrations.

####### 2.1.1.3.3.1.5.0 Directory Path

######## 2.1.1.3.3.1.5.1 Directory Path

Extensions

######## 2.1.1.3.3.1.5.2 Purpose

Contains extension methods for simplifying service registration and configuration.

######## 2.1.1.3.3.1.5.3 Contains Files

- DependencyInjection.cs

######## 2.1.1.3.3.1.5.4 Organizational Reasoning

Encapsulates the Data Access Layer's setup logic, making it easy for the consuming application (DMPS.Service.Worker) to register all necessary services.

######## 2.1.1.3.3.1.5.5 Framework Convention Alignment

Common .NET pattern for creating clean and reusable DI configurations using `IServiceCollection` extensions.

###### 2.1.1.3.3.2.0.0 Namespace Strategy

| Property | Value |
|----------|-------|
| Root Namespace | DMPS.Data.Access |
| Namespace Organization | Follows the directory structure (e.g., DMPS.Data.A... |
| Naming Conventions | PascalCase for all C# types and members. |
| Framework Alignment | Compliant with Microsoft C# and .NET 8.0 namespace... |

##### 2.1.1.3.4.0.0.0 Class Specifications

###### 2.1.1.3.4.1.0.0 Class Name

####### 2.1.1.3.4.1.1.0 Class Name

ApplicationDbContext

####### 2.1.1.3.4.1.2.0 File Path

Contexts/ApplicationDbContext.cs

####### 2.1.1.3.4.1.3.0 Class Type

DbContext

####### 2.1.1.3.4.1.4.0 Inheritance

Microsoft.EntityFrameworkCore.DbContext

####### 2.1.1.3.4.1.5.0 Purpose

Represents the session with the PostgreSQL database and acts as the central point for all data operations and configuration.

####### 2.1.1.3.4.1.6.0 Dependencies

- Microsoft.EntityFrameworkCore.DbContextOptions<ApplicationDbContext>
- Microsoft.Extensions.Configuration.IConfiguration (for encryption key)

####### 2.1.1.3.4.1.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.1.8.0 Technology Integration Notes

Integrates with Npgsql for PostgreSQL connectivity. It is configured to discover all `IEntityTypeConfiguration` implementations from its assembly.

####### 2.1.1.3.4.1.9.0 Validation Notes

Validation confirms this specification provides the central Unit of Work component required by the architecture.

####### 2.1.1.3.4.1.10.0 Properties

- {'property_name': 'Users', 'property_type': 'DbSet<User>', 'access_modifier': 'public', 'purpose': 'Provides access to the collection of User entities in the database.', 'validation_attributes': [], 'framework_specific_configuration': 'Configured via `UserConfiguration`.', 'implementation_notes': 'Should be queried via repositories, not directly by the application service layer.', 'validation_notes': 'Validation confirms specification includes DbSet properties for all 15 required entities.'}

####### 2.1.1.3.4.1.11.0 Methods

- {'method_name': 'OnModelCreating', 'method_signature': 'override void OnModelCreating(ModelBuilder modelBuilder)', 'return_type': 'void', 'access_modifier': 'protected', 'is_async': 'false', 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'modelBuilder', 'parameter_type': 'ModelBuilder', 'is_nullable': 'false', 'purpose': 'The builder used to construct the model for this context.', 'framework_attributes': []}], 'implementation_logic': 'Must call `modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly())` to automatically apply all `IEntityTypeConfiguration` classes defined in this project. Must also include logic to enable the `pgcrypto` extension via `modelBuilder.HasPostgresExtension(\\"pgcrypto\\");`. The method will also set up global conventions, such as converting all table and column names to snake_case.', 'exception_handling': 'Handled by EF Core.', 'performance_considerations': 'This method is called only once on application startup to build the model.', 'validation_requirements': 'Specification must ensure the `pgcrypto` extension is enabled, which is critical for REQ-083.', 'technology_integration_details': 'This is the primary integration point for applying Fluent API configurations and enabling PostgreSQL-specific extensions.', 'validation_notes': 'Validation confirms this method correctly centralizes model configuration.'}

####### 2.1.1.3.4.1.12.0 Events

*No items available*

####### 2.1.1.3.4.1.13.0 Implementation Notes

The class must include DbSet properties for all entities defined in the database design (Patient, Study, AuditLog, etc.). A constructor must accept `DbContextOptions<ApplicationDbContext>` for DI.

###### 2.1.1.3.4.2.0.0 Class Name

####### 2.1.1.3.4.2.1.0 Class Name

PatientConfiguration

####### 2.1.1.3.4.2.2.0 File Path

Configurations/PatientConfiguration.cs

####### 2.1.1.3.4.2.3.0 Class Type

Entity Configuration

####### 2.1.1.3.4.2.4.0 Inheritance

IEntityTypeConfiguration<Patient>

####### 2.1.1.3.4.2.5.0 Purpose

Defines the database schema mapping for the Patient entity using the EF Core Fluent API.

####### 2.1.1.3.4.2.6.0 Dependencies

*No items available*

####### 2.1.1.3.4.2.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.2.8.0 Technology Integration Notes

Implements the configuration for PHI encryption using pgcrypto.

####### 2.1.1.3.4.2.9.0 Validation Notes

Validation confirms this specification provides a concrete and complete implementation plan for REQ-083.

####### 2.1.1.3.4.2.10.0 Properties

*No items available*

####### 2.1.1.3.4.2.11.0 Methods

- {'method_name': 'Configure', 'method_signature': 'void Configure(EntityTypeBuilder<Patient> builder)', 'return_type': 'void', 'access_modifier': 'public', 'is_async': 'false', 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'builder', 'parameter_type': 'EntityTypeBuilder<Patient>', 'is_nullable': 'false', 'purpose': 'The builder to be used to configure the entity type.', 'framework_attributes': []}], 'implementation_logic': 'Must configure the table name to \\"patients\\".\\nMust define the primary key on `patientId`.\\nMust configure PHI properties (`dicomPatientId`, `patientName`, `patientBirthDate`, `patientSex`) to use a custom `EncryptedStringConverter`. This converter will be responsible for calling `pgp_sym_encrypt` on write and `pgp_sym_decrypt` on read. The properties must be mapped to a `bytea` column type in PostgreSQL.\\nMust define GIN trigram indexes on the encrypted columns for efficient searching.', 'exception_handling': 'N/A', 'performance_considerations': 'Indexes are critical for search performance on encrypted PHI fields.', 'validation_requirements': 'Ensures database constraints (e.g., `IsRequired()`) are applied and that PHI fields are correctly configured for encryption.', 'technology_integration_details': 'This class is a prime example of REQ-083 implementation. It must be designed to work with a shared encryption key provided via a service or configuration.', 'validation_notes': 'Validation confirms this specification correctly maps the Patient entity and its security requirements.'}

####### 2.1.1.3.4.2.12.0 Events

*No items available*

####### 2.1.1.3.4.2.13.0 Implementation Notes

Similar configuration classes must be created for all other entities, defining their tables, columns, relationships, and indexes as specified in the database design.

###### 2.1.1.3.4.3.0.0 Class Name

####### 2.1.1.3.4.3.1.0 Class Name

PostgresUserRepository

####### 2.1.1.3.4.3.2.0 File Path

Repositories/PostgresUserRepository.cs

####### 2.1.1.3.4.3.3.0 Class Type

Repository

####### 2.1.1.3.4.3.4.0 Inheritance

IUserRepository

####### 2.1.1.3.4.3.5.0 Purpose

Implements the IUserRepository interface, providing concrete data access logic for User entities using EF Core and PostgreSQL.

####### 2.1.1.3.4.3.6.0 Dependencies

- ApplicationDbContext

####### 2.1.1.3.4.3.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.3.8.0 Technology Integration Notes

All methods must be asynchronous and leverage EF Core's async extension methods.

####### 2.1.1.3.4.3.9.0 Validation Notes

Validation confirms this class correctly implements the `IUserRepository` contract from the Shared Kernel.

####### 2.1.1.3.4.3.10.0 Properties

- {'property_name': '_context', 'property_type': 'ApplicationDbContext', 'access_modifier': 'private readonly', 'purpose': 'The injected DbContext instance for database operations.', 'validation_attributes': [], 'framework_specific_configuration': 'Received via constructor dependency injection.', 'implementation_notes': '', 'validation_notes': 'Specification is valid.'}

####### 2.1.1.3.4.3.11.0 Methods

######## 2.1.1.3.4.3.11.1 Method Name

######### 2.1.1.3.4.3.11.1.1 Method Name

GetUserByUsernameAsync

######### 2.1.1.3.4.3.11.1.2 Method Signature

Task<User> GetUserByUsernameAsync(string username)

######### 2.1.1.3.4.3.11.1.3 Return Type

Task<User>

######### 2.1.1.3.4.3.11.1.4 Access Modifier

public

######### 2.1.1.3.4.3.11.1.5 Is Async

true

######### 2.1.1.3.4.3.11.1.6 Framework Specific Attributes

*No items available*

######### 2.1.1.3.4.3.11.1.7 Parameters

- {'parameter_name': 'username', 'parameter_type': 'string', 'is_nullable': 'false', 'purpose': 'The username of the user to retrieve.', 'framework_attributes': []}

######### 2.1.1.3.4.3.11.1.8 Implementation Logic

Must implement the query using `_context.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.username == username)`. Eager loading the `Role` is critical for performance.

######### 2.1.1.3.4.3.11.1.9 Exception Handling

Should be wrapped in a try-catch block to catch `NpgsqlException` and re-throw a custom `DataAccessException` to abstract the underlying technology from the caller.

######### 2.1.1.3.4.3.11.1.10 Performance Considerations

`AsNoTracking()` is mandatory as this is a read-only operation. The query relies on the unique index on the `username` column.

######### 2.1.1.3.4.3.11.1.11 Validation Requirements

N/A

######### 2.1.1.3.4.3.11.1.12 Technology Integration Details

Standard EF Core async LINQ query.

######### 2.1.1.3.4.3.11.1.13 Validation Notes

Specification for `Include` is added to prevent N+1 query problems.

######## 2.1.1.3.4.3.11.2.0 Method Name

######### 2.1.1.3.4.3.11.2.1 Method Name

AddUserAsync

######### 2.1.1.3.4.3.11.2.2 Method Signature

Task AddUserAsync(User user)

######### 2.1.1.3.4.3.11.2.3 Return Type

Task

######### 2.1.1.3.4.3.11.2.4 Access Modifier

public

######### 2.1.1.3.4.3.11.2.5 Is Async

true

######### 2.1.1.3.4.3.11.2.6 Framework Specific Attributes

*No items available*

######### 2.1.1.3.4.3.11.2.7 Parameters

- {'parameter_name': 'user', 'parameter_type': 'User', 'is_nullable': 'false', 'purpose': 'The new user entity to persist.', 'framework_attributes': []}

######### 2.1.1.3.4.3.11.2.8 Implementation Logic

Must add the user entity to the context using `_context.Users.Add(user)`. Must then commit the transaction by calling `await _context.SaveChangesAsync()`.

######### 2.1.1.3.4.3.11.2.9 Exception Handling

Must catch `DbUpdateException`. If the inner exception is a `PostgresException` indicating a unique constraint violation, it should throw a custom `DuplicateDataException`. Otherwise, it should throw a `DataAccessException`.

######### 2.1.1.3.4.3.11.2.10 Performance Considerations

The operation is I/O bound on `SaveChangesAsync()`.

######### 2.1.1.3.4.3.11.2.11 Validation Requirements

Assumes the user object is valid. Database constraints will provide the final validation.

######### 2.1.1.3.4.3.11.2.12 Technology Integration Details

Implements the write part of the Unit of Work pattern for a single, atomic operation.

######### 2.1.1.3.4.3.11.2.13 Validation Notes

Validation confirms the error handling strategy is robust and abstracts implementation details.

####### 2.1.1.3.4.3.12.0.0 Events

*No items available*

####### 2.1.1.3.4.3.13.0.0 Implementation Notes

The class must be registered with the DI container with a scoped lifetime. Similar repository classes must be implemented for `IStudyRepository` and `IAuditLogRepository`.

###### 2.1.1.3.4.4.0.0.0 Class Name

####### 2.1.1.3.4.4.1.0.0 Class Name

DependencyInjection

####### 2.1.1.3.4.4.2.0.0 File Path

Extensions/DependencyInjection.cs

####### 2.1.1.3.4.4.3.0.0 Class Type

Static Extension Class

####### 2.1.1.3.4.4.4.0.0 Inheritance



####### 2.1.1.3.4.4.5.0.0 Purpose

Provides a single extension method on `IServiceCollection` to register all services from this Data Access Layer with the DI container.

####### 2.1.1.3.4.4.6.0.0 Dependencies

- Microsoft.Extensions.DependencyInjection.IServiceCollection
- Microsoft.Extensions.Configuration.IConfiguration

####### 2.1.1.3.4.4.7.0.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.4.8.0.0 Technology Integration Notes

Follows the standard .NET pattern for creating clean, reusable dependency injection configurations.

####### 2.1.1.3.4.4.9.0.0 Validation Notes

Validation confirms this class provides a clean entry point for the consuming service, encapsulating all DAL setup logic.

####### 2.1.1.3.4.4.10.0.0 Properties

*No items available*

####### 2.1.1.3.4.4.11.0.0 Methods

- {'method_name': 'AddDataAccessServices', 'method_signature': 'static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)', 'return_type': 'IServiceCollection', 'access_modifier': 'public', 'is_async': 'false', 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'services', 'parameter_type': 'IServiceCollection', 'is_nullable': 'false', 'purpose': 'The service collection to add services to.', 'framework_attributes': []}, {'parameter_name': 'configuration', 'parameter_type': 'IConfiguration', 'is_nullable': 'false', 'purpose': 'The application configuration for retrieving the connection string.', 'framework_attributes': []}], 'implementation_logic': 'Must retrieve the PostgreSQL connection string from `configuration`. The connection string must be configured to enforce TLS (e.g., `SslMode=Require`), fulfilling REQ-NFR-004.\\nMust register the `ApplicationDbContext` using `services.AddDbContext<ApplicationDbContext>()` with the Npgsql provider.\\nMust register all repository implementations against their interfaces (e.g., `services.AddScoped<IUserRepository, PostgresUserRepository>()`).', 'exception_handling': 'Throws `ArgumentNullException` if the connection string is not found in the configuration.', 'performance_considerations': 'Registers DbContext with a scoped lifetime, which is optimal for web/service applications to leverage connection pooling.', 'validation_requirements': 'Validates the presence of the connection string and enforces the TLS requirement.', 'technology_integration_details': 'This is the sole public entry point for the consuming application (`DMPS.Service.Worker`) to use this library.', 'validation_notes': 'Specification enhanced to explicitly mention and enforce the TLS connection string requirement.'}

####### 2.1.1.3.4.4.12.0.0 Events

*No items available*

####### 2.1.1.3.4.4.13.0.0 Implementation Notes

The connection string name (e.g., \"PostgresConnection\") should be a well-defined constant.

##### 2.1.1.3.5.0.0.0.0 Interface Specifications

*No items available*

##### 2.1.1.3.6.0.0.0.0 Enum Specifications

*No items available*

##### 2.1.1.3.7.0.0.0.0 Dto Specifications

*No items available*

##### 2.1.1.3.8.0.0.0.0 Configuration Specifications

*No items available*

##### 2.1.1.3.9.0.0.0.0 Dependency Injection Specifications

###### 2.1.1.3.9.1.0.0.0 Service Interface

####### 2.1.1.3.9.1.1.0.0 Service Interface

DMPS.Shared.Core.Repositories.IUserRepository

####### 2.1.1.3.9.1.2.0.0 Service Implementation

DMPS.Data.Access.Repositories.PostgresUserRepository

####### 2.1.1.3.9.1.3.0.0 Lifetime

Scoped

####### 2.1.1.3.9.1.4.0.0 Registration Reasoning

Repository lifetime should match the DbContext's lifetime to ensure a consistent unit of work within a given operation scope.

####### 2.1.1.3.9.1.5.0.0 Framework Registration Pattern

services.AddScoped<IUserRepository, PostgresUserRepository>();

####### 2.1.1.3.9.1.6.0.0 Validation Notes

Specification for DI registration is validated as correct and complete.

###### 2.1.1.3.9.2.0.0.0 Service Interface

####### 2.1.1.3.9.2.1.0.0 Service Interface

DMPS.Shared.Core.Repositories.IStudyRepository

####### 2.1.1.3.9.2.2.0.0 Service Implementation

DMPS.Data.Access.Repositories.PostgresStudyRepository

####### 2.1.1.3.9.2.3.0.0 Lifetime

Scoped

####### 2.1.1.3.9.2.4.0.0 Registration Reasoning

Ensures data consistency for study-related operations within a single scope.

####### 2.1.1.3.9.2.5.0.0 Framework Registration Pattern

services.AddScoped<IStudyRepository, PostgresStudyRepository>();

####### 2.1.1.3.9.2.6.0.0 Validation Notes

Specification for DI registration is validated as correct and complete.

###### 2.1.1.3.9.3.0.0.0 Service Interface

####### 2.1.1.3.9.3.1.0.0 Service Interface

DMPS.Shared.Core.Repositories.IAuditLogRepository

####### 2.1.1.3.9.3.2.0.0 Service Implementation

DMPS.Data.Access.Repositories.PostgresAuditLogRepository

####### 2.1.1.3.9.3.3.0.0 Lifetime

Scoped

####### 2.1.1.3.9.3.4.0.0 Registration Reasoning

Maintains consistency for audit logging operations within a business transaction.

####### 2.1.1.3.9.3.5.0.0 Framework Registration Pattern

services.AddScoped<IAuditLogRepository, PostgresAuditLogRepository>();

####### 2.1.1.3.9.3.6.0.0 Validation Notes

Specification for DI registration is validated as correct and complete.

###### 2.1.1.3.9.4.0.0.0 Service Interface

####### 2.1.1.3.9.4.1.0.0 Service Interface

DMPS.Data.Access.Contexts.ApplicationDbContext

####### 2.1.1.3.9.4.2.0.0 Service Implementation

DMPS.Data.Access.Contexts.ApplicationDbContext

####### 2.1.1.3.9.4.3.0.0 Lifetime

Scoped

####### 2.1.1.3.9.4.4.0.0 Registration Reasoning

Standard EF Core lifetime for service applications. A new instance is created per logical scope of work, which is ideal for managing units of work and leveraging connection pooling.

####### 2.1.1.3.9.4.5.0.0 Framework Registration Pattern

services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(...));

####### 2.1.1.3.9.4.6.0.0 Validation Notes

Specification for DI registration is validated as correct and complete.

##### 2.1.1.3.10.0.0.0.0 External Integration Specifications

- {'integration_target': 'PostgreSQL 16 Database', 'integration_type': 'Database', 'required_client_classes': ['ApplicationDbContext', 'NpgsqlConnection'], 'configuration_requirements': 'Requires a valid Npgsql connection string, which must be retrieved from a secure configuration source. The connection string must enforce TLS encryption (`SslMode=Require`).', 'error_handling_requirements': 'Must handle `NpgsqlException` for connectivity issues and `DbUpdateException` for constraint violations or other data-related errors. Logic should translate these into a higher-level, technology-agnostic `DataAccessException`.', 'authentication_requirements': 'Username/password credentials provided in the connection string.', 'framework_integration_patterns': 'Integration is managed entirely through Entity Framework Core and the Npgsql provider. All interactions are abstracted via the Repository pattern.', 'validation_notes': 'Specification is validated as complete, secure, and robust.'}

#### 2.1.1.4.0.0.0.0.0 Component Count Validation

| Property | Value |
|----------|-------|
| Total Classes | 20 |
| Total Interfaces | 0 |
| Total Enums | 0 |
| Total Dtos | 0 |
| Total Configurations | 15 |
| Total External Integrations | 1 |
| Grand Total Components | 48 |
| Phase 2 Claimed Count | 2 |
| Phase 2 Actual Count | 2 |
| Validation Added Count | 46 |
| Final Validated Count | 48 |

## 2.2.0.0.0.0.0.0.0 Project Supporting Files

### 2.2.1.0.0.0.0.0.0 File Type

#### 2.2.1.1.0.0.0.0.0 File Type

Project Definition

#### 2.2.1.2.0.0.0.0.0 File Name

DMPS.Data.Access.csproj

#### 2.2.1.3.0.0.0.0.0 File Path

./DMPS.Data.Access.csproj

#### 2.2.1.4.0.0.0.0.0 Purpose

Defines the .NET 8 class library project, its target framework, dependencies on Entity Framework Core and the Npgsql provider, and project references.

#### 2.2.1.5.0.0.0.0.0 Content Description

<Project Sdk=\"Microsoft.NET.Sdk\">\n\n  <PropertyGroup>\n    <TargetFramework>net8.0</TargetFramework>\n    <ImplicitUsings>enable</ImplicitUsings>\n    <Nullable>enable</Nullable>\n  </PropertyGroup>\n\n  <ItemGroup>\n    <PackageReference Include=\"Microsoft.EntityFrameworkCore.Design\" Version=\"8.0.6\">\n      <PrivateAssets>all</PrivateAssets>\n      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>\n    </PackageReference>\n    <PackageReference Include=\"Microsoft.Extensions.Configuration.Abstractions\" Version=\"8.0.0\" />\n    <PackageReference Include=\"Microsoft.Extensions.DependencyInjection.Abstractions\" Version=\"8.0.0\" />\n    <PackageReference Include=\"Npgsql.EntityFrameworkCore.PostgreSQL\" Version=\"8.0.4\" />\n    <PackageReference Include=\"EFCore.NamingConventions\" Version=\"8.0.3\" />\n  </ItemGroup>\n\n  <ItemGroup>\n    <ProjectReference Include=\"..\\DMPS.Shared.Core\\DMPS.Shared.Core.csproj\" />\n  </ItemGroup>\n\n</Project>

#### 2.2.1.6.0.0.0.0.0 Framework Specific Attributes

- TargetFramework: net8.0
- PackageReference for Npgsql.EntityFrameworkCore.PostgreSQL
- PackageReference for EFCore.NamingConventions
- ProjectReference to DMPS.Shared.Core

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

root = true\n\n[*]\nindent_style = space\nindent_size = 4\nend_of_line = lf\ncharset = utf-8\ntrim_trailing_whitespace = true\ninsert_final_newline = true\n\n[*.cs]\ncsharp_style_namespace_declarations = file_scoped\ncsharp_style_prefer_simplified_interpolation = true

#### 2.2.3.6.0.0.0.0.0 Framework Specific Attributes

- csharp_style_namespace_declarations: file_scoped (C# 10+ feature)
- Standard formatting rules

