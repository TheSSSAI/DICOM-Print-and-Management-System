# Architecture Design Specification

# 1. Components

- **Components:**
  
  ### .1. DMPS WPF Client Application
  The main user-facing desktop application responsible for all user interactions, DICOM viewing, and task initiation. It acts as the client in the client-server architecture.

  #### .1.4. Type
  Application

  #### .1.5. Dependencies
  
  - dmps.service.application
  - dmps.shared.core
  
  #### .1.6. Properties
  
  - **Framework:** WPF
  - **Pattern:** MVVM
  
  #### .1.7. Interfaces
  
  
  #### .1.8. Technology
  .NET 8, WPF, Material Design in XAML Toolkit, Vortice.Windows

  #### .1.9. Resources
  
  - **Cpu:** 1 core (min)
  - **Memory:** 2GB (min)
  - **Storage:** 500MB for application files
  - **Network:** 100Mbps
  
  #### .1.10. Configuration
  
  - **Rabbit Mq Connection String:** amqp://guest:guest@localhost:5672
  - **Named Pipe Name:** DicomServiceStatusPipe
  - **Odoo Api Endpoint:** https://odoo.example.com/api/license
  
  #### .1.11. Health Check
  
  - **Path:** N/A (Client-side check via Named Pipe)
  - **Interval:** 10
  - **Timeout:** 2
  
  #### .1.12. Responsible Features
  
  - User Interface and Interaction
  - DICOM Image Viewing and Manipulation
  - Print Preview and Configuration
  - DICOM C-FIND/C-MOVE SCU
  - System Configuration UI
  - User Management UI
  - Audit Trail Viewing
  
  #### .1.13. Security
  
  - **Requires Authentication:** True
  - **Requires Authorization:** True
  - **Allowed Roles:**
    
    - Technician
    - Administrator
    
  
  ### .2. DMPS Windows Service ('DICOM Service')
  A single, consolidated background Windows Service that hosts all asynchronous, long-running tasks, including DICOM services, database operations, and print job processing.

  #### .2.4. Type
  Service

  #### .2.5. Dependencies
  
  - PostgreSQL
  - RabbitMQ
  - dmps.shared.core
  
  #### .2.6. Properties
  
  - **Host:** Microsoft.Extensions.Hosting
  - **Service Name:** DICOM Service
  
  #### .2.7. Interfaces
  
  
  #### .2.8. Technology
  .NET 8, Microsoft.Extensions.Hosting, Entity Framework Core 8

  #### .2.9. Resources
  
  - **Cpu:** 2 cores (min)
  - **Memory:** 4GB (min)
  - **Storage:** 500MB for application files
  - **Network:** 1Gbps
  
  #### .2.10. Configuration
  
  - **Dicom Scp Port:** 11112
  - **Db Connection String:** Host=localhost;Database=dmps_db;Username=user;Password=...
  - **Rabbit Mq Connection String:** amqp://guest:guest@localhost:5672
  
  #### .2.11. Health Check
  
  - **Path:** N/A (Responds to Named Pipe requests)
  - **Interval:** 0
  - **Timeout:** 0
  
  #### .2.12. Responsible Features
  
  - DICOM C-STORE SCP Listener
  - Asynchronous Database Writing
  - PDF Generation Processing
  - Physical Print Job Spooling
  - Data Retention Policy Enforcement
  - Data Integrity Checks
  - System Health Monitoring
  
  #### .2.13. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .3. UI Components (Views & ViewModels)
  Implements the MVVM pattern. Views (XAML) define the UI structure, and ViewModels contain presentation logic, state, and commands, decoupled from the View.

  #### .3.4. Type
  UI Component Library

  #### .3.5. Dependencies
  
  - dmps.client.services
  - dmps.shared.core
  
  #### .3.6. Properties
  
  
  #### .3.7. Interfaces
  
  
  #### .3.8. Technology
  WPF, XAML, MVVM

  #### .3.9. Resources
  
  
  #### .3.10. Configuration
  
  
  #### .3.11. Responsible Features
  
  - Login Screen
  - Main Window Shell
  - Study Browser
  - DICOM Viewer
  - Print Preview
  - Settings Dashboards
  
  #### .3.12. Security
  
  - **Requires Authentication:** True
  - **Requires Authorization:** True
  
  ### .4. Client Application Services
  A collection of client-side services that orchestrate application logic, manage local state, and handle communication with the background service and external APIs.

  #### .4.4. Type
  Service

  #### .4.5. Dependencies
  
  - dmps.infrastructure.communication
  - dmps.infrastructure.apiclient
  
  #### .4.6. Properties
  
  
  #### .4.7. Interfaces
  
  - IAuthenticationService
  - ILicenseValidationService
  - ISessionLockService
  - ITaskPublisher
  
  #### .4.8. Technology
  .NET 8

  #### .4.9. Resources
  
  
  #### .4.10. Configuration
  
  
  #### .4.11. Responsible Features
  
  - User Authentication and Session Management
  - License Validation Workflow
  - Automatic Session Lock
  - Publishing tasks to RabbitMQ
  
  #### .4.12. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .5. Background Hosted Services
  A set of long-running services, implemented with IHostedService, that form the core of the Windows Service. Each service handles a specific background responsibility.

  #### .5.4. Type
  Service

  #### .5.5. Dependencies
  
  - dmps.data.repositories
  - dmps.infrastructure.communication
  - dmps.infrastructure.dicom
  - dmps.infrastructure.printing
  
  #### .5.6. Properties
  
  
  #### .5.7. Interfaces
  
  - IHostedService
  
  #### .5.8. Technology
  Microsoft.Extensions.Hosting

  #### .5.9. Resources
  
  
  #### .5.10. Configuration
  
  
  #### .5.11. Responsible Features
  
  - DICOM C-STORE SCP Listener
  - Consuming messages for DB writes
  - Consuming messages for print jobs
  - Responding to client status checks
  
  #### .5.12. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .6. Data Access Repositories
  Implements the Repository pattern to abstract database operations. Provides a clean API for the service layer to perform CRUD operations without knowing about EF Core.

  #### .6.4. Type
  Repository

  #### .6.5. Dependencies
  
  - dmps.shared.core
  
  #### .6.6. Properties
  
  - **Orm:** Entity Framework Core 8
  
  #### .6.7. Interfaces
  
  - IUserRepository
  - IStudyRepository
  - IAuditLogRepository
  - ISystemSettingRepository
  
  #### .6.8. Technology
  Entity Framework Core 8, Npgsql

  #### .6.9. Resources
  
  
  #### .6.10. Configuration
  
  - **Db Connection String:** Provided via DI
  
  #### .6.11. Responsible Features
  
  - Storing DICOM Metadata
  - Managing User Accounts
  - Persisting Audit Logs
  - Storing System Settings
  
  #### .6.12. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .7. Shared Core Library
  A shared class library containing code used by both the Client and Service. This includes domain models, repository interfaces, and DTOs for communication.

  #### .7.4. Type
  Library

  #### .7.5. Dependencies
  
  
  #### .7.6. Properties
  
  
  #### .7.7. Interfaces
  
  
  #### .7.8. Technology
  .NET 8

  #### .7.9. Resources
  
  
  #### .7.10. Configuration
  
  
  #### .7.11. Responsible Features
  
  - Domain Model Definitions (User, Study, etc.)
  - Data Transfer Objects (DTOs)
  - Repository Interface Definitions
  - Core Business Logic (e.g., PasswordPolicyValidator)
  
  #### .7.12. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .8. Communication Infrastructure
  Provides concrete implementations for inter-process communication (IPC) mechanisms, abstracting the details of RabbitMQ and Named Pipes from the application layer.

  #### .8.4. Type
  Infrastructure

  #### .8.5. Dependencies
  
  
  #### .8.6. Properties
  
  
  #### .8.7. Interfaces
  
  - IMessageProducer
  - IMessageConsumer
  - INamedPipeClient
  - INamedPipeServer
  
  #### .8.8. Technology
  RabbitMQ.Client, System.IO.Pipes

  #### .8.9. Resources
  
  
  #### .8.10. Configuration
  
  
  #### .8.11. Responsible Features
  
  - Asynchronous task initiation via RabbitMQ
  - Synchronous service status checks via Named Pipes
  
  #### .8.12. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .9. DICOM Functionality Infrastructure
  A wrapper around the 'fo-dicom' library, providing all DICOM-related functionalities like network services (SCU/SCP), metadata parsing, and image rendering.

  #### .9.4. Type
  Infrastructure

  #### .9.5. Dependencies
  
  - dmps.shared.core
  
  #### .9.6. Properties
  
  - **Library:** fo-dicom
  
  #### .9.7. Interfaces
  
  - IDicomScuService
  - IDicomAnonymizer
  - IDicomFileStorage
  
  #### .9.8. Technology
  fo-dicom

  #### .9.9. Resources
  
  
  #### .9.10. Configuration
  
  
  #### .9.11. Responsible Features
  
  - DICOM Network Communication (C-STORE, C-FIND, C-MOVE)
  - DICOM Metadata Anonymization
  - Saving and organizing DICOM files on disk
  
  #### .9.12. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .10. Cross-Cutting Infrastructure
  Provides implementations for concerns that span multiple layers, such as logging, configuration management, and credential storage.

  #### .10.4. Type
  Infrastructure

  #### .10.5. Dependencies
  
  
  #### .10.6. Properties
  
  
  #### .10.7. Interfaces
  
  - ILogger
  - ISecureCredentialStore
  
  #### .10.8. Technology
  Serilog, BCrypt.Net-Next, Windows Credential Manager

  #### .10.9. Resources
  
  
  #### .10.10. Configuration
  
  
  #### .10.11. Responsible Features
  
  - Structured logging with PHI redaction
  - Password Hashing and Verification
  - Secure storage of secrets
  
  #### .10.12. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  ### .11. I/O Infrastructure
  Components responsible for external input/output operations, such as PDF generation, printing, and communicating with external web APIs.

  #### .11.4. Type
  Infrastructure

  #### .11.5. Dependencies
  
  
  #### .11.6. Properties
  
  
  #### .11.7. Interfaces
  
  - IPdfGenerator
  - IPrintSpooler
  - ILicenseApiClient
  
  #### .11.8. Technology
  QuestPDF, System.Drawing.Printing, HttpClient

  #### .11.9. Resources
  
  
  #### .11.10. Configuration
  
  
  #### .11.11. Responsible Features
  
  - Exporting print layouts to PDF/A
  - Sending jobs to the Windows Print API
  - Communicating with the Odoo REST API
  
  #### .11.12. Security
  
  - **Requires Authentication:** False
  - **Requires Authorization:** False
  
  
- **Configuration:**
  
  - **Environment:** production
  - **Logging Level:** INFO
  - **Database Url:** Secret managed by Windows Credential Manager
  - **Cache Ttl:** N/A
  - **Max Threads:** System-managed
  


---

# 2. Component_Relations

- **Architecture:**
  
  - **Components:**
    
    - **Id:** wpf-client-app-001  
**Name:** WpfClientApplication  
**Description:** The main WPF desktop client application responsible for all user interactions. It follows the MVVM pattern and communicates with the background service for asynchronous tasks.  
**Type:** Application  
**Dependencies:**
    
    - dicom-windows-service-002
    - rabbitmq-broker-003
    - postgresql-db-004
    
**Properties:**
    
    - **Framework:** .NET 8
    - **Architecture:** MVVM
    
**Interfaces:**
    
    
**Technology:** WPF, .NET 8, C# 12, Material Design in XAML Toolkit, Vortice.Windows  
**Resources:**
    
    - **Cpu:** 2 Cores (Recommended)
    - **Memory:** 8GB RAM (16GB Recommended)
    - **Storage:** 500MB for application files
    - **Gpu:** DirectX 11 compatible GPU required
    
**Configuration:**
    
    
**Health Check:**
    
    
**Responsible Features:**
    
    - User Interface and Interaction (REQ-1-001)
    - DICOM Viewer (REQ-1-051)
    - Print Preview and Configuration (REQ-1-022)
    - Administration Dashboard (REQ-1-016)
    - DICOM SCU Operations (C-FIND/C-MOVE) (REQ-1-036)
    
**Security:**
    
    - **Requires Authentication:** True
    - **Requires Authorization:** True
    - **Allowed Roles:**
      
      - Technician
      - Administrator
      
    
    - **Id:** dicom-windows-service-002  
**Name:** DicomWindowsService  
**Description:** The single, consolidated background Windows Service responsible for all asynchronous, long-running tasks, DICOM SCP, and data management.  
**Type:** Windows Service  
**Dependencies:**
    
    - rabbitmq-broker-003
    - postgresql-db-004
    - odoo-licensing-api-005
    
**Properties:**
    
    - **Framework:** .NET 8
    - **Hosting Model:** Microsoft.Extensions.Hosting
    
**Interfaces:**
    
    
**Technology:** .NET 8, C# 12, Microsoft.Extensions.Hosting, Serilog  
**Resources:**
    
    - **Cpu:** 2 Cores (sustained)
    - **Memory:** 4GB RAM
    - **Storage:** Depends on logging configuration
    - **Network:** Persistent network access required
    
**Configuration:**
    
    - **Startup Type:** Automatic
    - **Recovery:** Restart the Service on failure
    
**Health Check:**
    
    - **Path:** Via Named Pipe
    - **Interval:** 0
    - **Timeout:** 2
    
**Responsible Features:**
    
    - DICOM C-STORE SCP (REQ-1-035)
    - Asynchronous Database Writes (REQ-1-010)
    - PDF Generation Job Processing (REQ-1-029)
    - Physical Print Job Spooling (REQ-1-020)
    - Data Retention Policy Enforcement (REQ-1-018)
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** rabbitmq-broker-003  
**Name:** RabbitMQ Broker  
**Description:** External message broker used for all asynchronous inter-process communication between the client and the service. Manages queues for print jobs, DB writes, etc.  
**Type:** External Dependency  
**Dependencies:**
    
    
**Properties:**
    
    - **Queues:** durable
    - **Messages:** persistent
    - **Dlx/Dlq:** enabled
    
**Interfaces:**
    
    - AMQP 0-9-1
    
**Technology:** RabbitMQ  
**Resources:**
    
    - **Cpu:** 1 Core
    - **Memory:** 2GB RAM+
    - **Storage:** Dependent on queue depth
    
**Configuration:**
    
    
**Health Check:**
    
    
**Responsible Features:**
    
    - Asynchronous Task Queuing (REQ-1-004)
    - Message Durability (REQ-1-005)
    - Dead-Lettering Failed Messages (REQ-1-006)
    
**Security:**
    
    - **Requires Authentication:** True
    - **Requires Authorization:** True
    
    - **Id:** postgresql-db-004  
**Name:** PostgreSQL Database  
**Description:** External PostgreSQL database server for storing all application metadata, including patient/study info, user accounts, and configuration.  
**Type:** External Dependency  
**Dependencies:**
    
    
**Properties:**
    
    - **Version:** 16
    - **Extensions:** pgcrypto
    
**Interfaces:**
    
    - SQL
    
**Technology:** PostgreSQL 16  
**Resources:**
    
    - **Cpu:** 2 Cores+
    - **Memory:** 4GB RAM+
    - **Storage:** Dependent on data volume and retention policy
    
**Configuration:**
    
    - **Encryption At Rest:** Full disk encryption required
    - **Encryption In Transit:** TLS required
    
**Health Check:**
    
    
**Responsible Features:**
    
    - Study Metadata Storage (REQ-1-010)
    - User and Role Management Storage (REQ-1-014)
    - Audit Trail Storage (REQ-1-047)
    
**Security:**
    
    - **Requires Authentication:** True
    - **Requires Authorization:** True
    
    - **Id:** odoo-licensing-api-005  
**Name:** Odoo Licensing API  
**Description:** External REST API provided by Odoo for application license validation.  
**Type:** External Dependency  
**Dependencies:**
    
    
**Properties:**
    
    
**Interfaces:**
    
    - REST/HTTPS
    
**Technology:** Odoo Web Portal  
**Resources:**
    
    
**Configuration:**
    
    - **Tls Version:** 1.2 minimum, 1.3 preferred
    
**Health Check:**
    
    
**Responsible Features:**
    
    - License Validation (REQ-1-011)
    - Secure Communication for Licensing (REQ-1-074)
    
**Security:**
    
    - **Requires Authentication:** True
    - **Requires Authorization:** False
    
    - **Id:** viewmodel-layer-006  
**Name:** ViewModel Layer  
**Description:** A collection of ViewModel classes within the WPF client. They contain the presentation logic, state management, and command handling for their corresponding Views, decoupling the UI from the application logic.  
**Type:** Component Group  
**Dependencies:**
    
    - application-services-client-007
    
**Properties:**
    
    
**Interfaces:**
    
    - INotifyPropertyChanged
    
**Technology:** .NET 8, C# 12  
**Resources:**
    
    
**Configuration:**
    
    
**Health Check:**
    
    
**Responsible Features:**
    
    - MVVM Pattern Implementation (REQ-1-013)
    - UI State Management
    - Role-Based UI Adjustments (REQ-1-040)
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** application-services-client-007  
**Name:** Client Application Services  
**Description:** Services running within the WPF client process that orchestrate application logic, manage client-side state, and communicate with external systems (Windows Service, Odoo API).  
**Type:** Service Layer  
**Dependencies:**
    
    - infrastructure-wrappers-011
    - domain-logic-009
    
**Properties:**
    
    
**Interfaces:**
    
    - IAuthenticationService
    - ILicenseValidationService
    - ISessionStateService
    
**Technology:** .NET 8, C# 12  
**Resources:**
    
    
**Configuration:**
    
    
**Health Check:**
    
    
**Responsible Features:**
    
    - User Authentication (REQ-1-040)
    - Odoo License Check Orchestration (REQ-1-073)
    - Automatic Session Lock Management (REQ-1-041)
    - Publishing tasks to RabbitMQ (REQ-1-004)
    - Checking service status via Named Pipes (REQ-1-007)
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** background-hosted-services-008  
**Name:** Background Hosted Services  
**Description:** The set of IHostedService implementations within the Windows Service. Each service is a long-running background task responsible for a specific function, such as listening for DICOM connections or processing a message queue.  
**Type:** HostedService  
**Dependencies:**
    
    - domain-logic-009
    - data-access-layer-010
    - infrastructure-wrappers-011
    
**Properties:**
    
    
**Interfaces:**
    
    - IHostedService
    
**Technology:** .NET 8, Microsoft.Extensions.Hosting  
**Resources:**
    
    
**Configuration:**
    
    
**Health Check:**
    
    
**Responsible Features:**
    
    - DICOM SCP Listener (REQ-1-003)
    - RabbitMQ Message Consumption (REQ-1-003)
    - Print Job Processing (REQ-1-003)
    - Scheduled Data Retention (REQ-1-018)
    - Named Pipe Server for Status Checks (REQ-1-007)
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** domain-logic-009  
**Name:** Domain Logic & Entities  
**Description:** A shared core library containing technology-agnostic business logic, domain entities (User, Study), business rules (Password Policy), and repository interfaces. This component has no external dependencies on UI, database, or infrastructure.  
**Type:** Domain Layer  
**Dependencies:**
    
    
**Properties:**
    
    
**Interfaces:**
    
    - IUserRepository
    - IStudyRepository
    
**Technology:** .NET 8, C# 12  
**Resources:**
    
    
**Configuration:**
    
    
**Health Check:**
    
    
**Responsible Features:**
    
    - DICOM Anonymization Rules (REQ-1-065)
    - Password Policy Enforcement Logic (REQ-1-042)
    - Working Copy Management for Edits (REQ-1-045)
    - Definition of Core Data Structures (e.g., Patient, Study, Series)
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** data-access-layer-010  
**Name:** Data Access Layer  
**Description:** Resides within the Windows Service and handles all database interactions. It implements the repository interfaces defined in the domain layer using Entity Framework Core.  
**Type:** Repository Layer  
**Dependencies:**
    
    - domain-logic-009
    - postgresql-db-004
    
**Properties:**
    
    
**Interfaces:**
    
    
**Technology:** Entity Framework Core 8, Npgsql  
**Resources:**
    
    
**Configuration:**
    
    
**Health Check:**
    
    
**Responsible Features:**
    
    - CRUD operations for all entities
    - Use of pgcrypto for PHI encryption/decryption (REQ-1-083)
    - Database Migrations
    
**Security:**
    
    - **Requires Authentication:** True
    - **Requires Authorization:** True
    
    - **Id:** infrastructure-wrappers-011  
**Name:** Infrastructure Wrappers  
**Description:** A collection of classes that encapsulate and abstract interactions with external libraries and systems like fo-dicom, RabbitMQ, QuestPDF, and the file system. Provides a clean, application-specific API over third-party tools.  
**Type:** Infrastructure Layer  
**Dependencies:**
    
    - cross-cutting-concerns-012
    
**Properties:**
    
    
**Interfaces:**
    
    - IMessageQueueProducer
    - IDicomService
    - IPdfGenerator
    - ISecureCredentialStore
    
**Technology:** fo-dicom, RabbitMQ.Client, QuestPDF, System.IO.Pipes  
**Resources:**
    
    
**Configuration:**
    
    
**Health Check:**
    
    
**Responsible Features:**
    
    - All DICOM Functionality (REQ-1-009)
    - RabbitMQ Communication (REQ-1-004)
    - PDF Generation (REQ-1-029)
    - Secure Credential Storage (REQ-1-084)
    - Named Pipe Communication (REQ-1-007)
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    - **Id:** cross-cutting-concerns-012  
**Name:** Cross-Cutting Concerns  
**Description:** Shared library providing services used across all layers, such as logging, configuration management, exception handling, and security utilities.  
**Type:** Utility  
**Dependencies:**
    
    
**Properties:**
    
    
**Interfaces:**
    
    - ILogger
    
**Technology:** Serilog, Microsoft.Extensions.Configuration, BCrypt.Net-Next  
**Resources:**
    
    
**Configuration:**
    
    
**Health Check:**
    
    
**Responsible Features:**
    
    - PHI-Redacted Logging to File and Event Log (REQ-1-039)
    - BCrypt Password Hashing (REQ-1-082)
    - Correlation ID Generation and Propagation (REQ-1-090)
    
**Security:**
    
    - **Requires Authentication:** False
    - **Requires Authorization:** False
    
    
  - **Configuration:**
    
    - **Environment:** production
    - **Logging Level:** INFO
    - **Database Url:** Stored in Windows Credential Manager
    - **Cache Ttl:** 3600
    - **Max Threads:** 50
    
  


---

