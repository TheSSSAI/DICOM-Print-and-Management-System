# Specification

# 1. Data Transformation Analysis

- **System Overview:**
  
  - **Analysis Date:** 2025-06-13
  - **Technology Stack:**
    
    - .NET 8
    - C# 12
    - WPF
    - PostgreSQL 16
    - Entity Framework Core 8
    - RabbitMQ
    - fo-dicom
    
  - **Service Interfaces:**
    
    - DICOM C-STORE SCP -> RabbitMQ Message Bus
    - WPF Client -> RabbitMQ Message Bus
    - RabbitMQ Consumer -> Data Access Layer (EF Core)
    - WPF Client -> Odoo REST API (License Validation)
    - WPF Client -> Windows Service (Named Pipes for Status)
    
  - **Data Models:**
    
    - fo-dicom DICOM Dataset
    - PostgreSQL Relational Entities (Patient, Study, Series, Image)
    - RabbitMQ Message DTOs (e.g., SubmitPrintJobCommand)
    - WPF ViewModels
    
  
- **Data Mapping Strategy:**
  
  - **Essential Mappings:**
    
    - **Mapping Id:** MAP-001  
**Source:** fo-dicom DicomDataset  
**Target:** PostgreSQL Entities (Patient, Study, Series, Image)  
**Transformation:** splitting  
**Configuration:**
    
    
**Mapping Technique:** Object-to-Object Mapping  
**Justification:** Core data ingestion pathway for all DICOM studies received via C-STORE or file import, as required by REQ-1-010. A single DICOM file's metadata is split across multiple related database tables.  
**Complexity:** medium  
    - **Mapping Id:** MAP-002  
**Source:** WPF PrintPreviewViewModel  
**Target:** RabbitMQ SubmitPrintJobCommand DTO  
**Transformation:** direct  
**Configuration:**
    
    
**Mapping Technique:** Object-to-Object Mapping  
**Justification:** Required to serialize the user's print request from the UI into a message that can be processed asynchronously by the background service (REQ-1-004, REQ-1-021).  
**Complexity:** simple  
    - **Mapping Id:** MAP-003  
**Source:** Legacy System Database Schema  
**Target:** PostgreSQL Entities (Patient, Study, Series, Image)  
**Transformation:** custom  
**Configuration:**
    
    
**Mapping Technique:** ETLV Utility  
**Justification:** Required by the dedicated data migration utility to move data from the legacy system into the new application's schema (REQ-1-093).  
**Complexity:** complex  
    
  - **Object To Object Mappings:**
    
    - **Source Object:** DicomDataset  
**Target Object:** Patient, Study, Series, Image Entities  
**Field Mappings:**
    
    - **Source Field:** DicomTag.PatientName (0010,0010)  
**Target Field:** Patient.patientName  
**Transformation:** Direct  
**Data Type Conversion:** DICOM PN to String  
    - **Source Field:** DicomTag.StudyInstanceUID (0020,000D)  
**Target Field:** Study.studyInstanceUid  
**Transformation:** Direct  
**Data Type Conversion:** DICOM UI to String  
    - **Source Field:** DicomTag.StudyDate (0008,0020)  
**Target Field:** Study.studyDate  
**Transformation:** Parse DICOM Date  
**Data Type Conversion:** DICOM DA to DateTime  
    
    
  - **Data Type Conversions:**
    
    - **From:** DICOM DA (Date String)  
**To:** PostgreSQL DateTime  
**Conversion Method:** Custom parsing logic to handle YYYYMMDD format.  
**Validation Required:** True  
    - **From:** DICOM TM (Time String)  
**To:** C# TimeSpan or part of DateTime  
**Conversion Method:** Custom parsing logic to handle HHMMSS.FFFFFF format.  
**Validation Required:** True  
    - **From:** DICOM IS/DS (Numeric String)  
**To:** PostgreSQL INT/BIGINT  
**Conversion Method:** Standard integer/decimal parsing.  
**Validation Required:** True  
    
  - **Bidirectional Mappings:**
    
    - **Entity:** Study  
**Forward Mapping:** Study Entity to StudyViewModel  
**Reverse Mapping:** Not required, UI updates are commands, not entity updates.  
**Consistency Strategy:** Read-only display. The database is the single source of truth.  
    
  
- **Schema Validation Requirements:**
  
  - **Field Level Validations:**
    
    - **Field:** User.username  
**Rules:**
    
    - required
    - unique
    
**Priority:** critical  
**Error Message:** Username is required and must be unique.  
    - **Field:** User.password  
**Rules:**
    
    - meets policy requirements (length, complexity)
    
**Priority:** critical  
**Error Message:** Password does not meet the security requirements.  
    - **Field:** DicomFile  
**Rules:**
    
    - is valid DICOM format
    - contains required tags (e.g., SOPInstanceUID)
    
**Priority:** high  
**Error Message:** File is not a valid or complete DICOM object.  
    
  - **Cross Field Validations:**
    
    
  - **Business Rule Validations:**
    
    - **Rule Id:** BR-001  
**Description:** Prevent import of studies with a Study Instance UID that already exists in the database.  
**Fields:**
    
    - Study.studyInstanceUid
    
**Logic:** On import, query the Study table for the incoming UID. If found, trigger user prompt for overwrite/discard/new options.  
**Priority:** critical  
    - **Rule Id:** BR-002  
**Description:** Enforce license status before enabling application features.  
**Fields:**
    
    - License.status
    
**Logic:** On startup, check license status. If invalid or grace period expired, switch application to read-only mode.  
**Priority:** critical  
    
  - **Conditional Validations:**
    
    - **Condition:** User.isTemporaryPassword is true  
**Applicable Fields:**
    
    - User.password
    
**Validation Rules:**
    
    - User must be forced to change password on next login.
    
    
  - **Validation Groups:**
    
    - **Group Name:** InstallerPrerequisiteCheck  
**Validations:**
    
    - PostgreSQLConnectivity
    - RabbitMQConnectivity
    - PgcryptoExtensionEnabled
    
**Execution Order:** 1  
**Stop On First Failure:** True  
    
  
- **Transformation Pattern Evaluation:**
  
  - **Selected Patterns:**
    
    - **Pattern:** pipeline  
**Use Case:** DICOM C-STORE Data Ingestion  
**Implementation:** A sequence of steps: 1. SCP receives file. 2. Metadata extracted. 3. Message published to RabbitMQ. 4. Consumer receives message. 5. DTO is mapped to DB entities. 6. Entities are saved to PostgreSQL.  
**Justification:** Decouples high-speed network reception from slower database operations, as required by REQ-1-010 to prevent backpressure.  
    - **Pattern:** adapter  
**Use Case:** License Validation  
**Implementation:** An 'OdooApiClient' class that adapts the application's internal license checking interface to the external Odoo REST API's HTTP requests and JSON responses.  
**Justification:** Isolates the application from the specifics of the external Odoo API, fulfilling REQ-1-011.  
    - **Pattern:** converter  
**Use Case:** DICOM Data Type Conversion  
**Implementation:** Dedicated utility functions to parse DICOM-specific string formats (DA, TM, DS, IS) into standard .NET types (DateTime, int, decimal).  
**Justification:** Required for correctly persisting DICOM metadata into the strongly-typed PostgreSQL database schema.  
    
  - **Pipeline Processing:**
    
    - **Required:** True
    - **Stages:**
      
      - **Stage:** Ingest  
**Transformation:** Receive DICOM file via C-STORE  
**Dependencies:**
    
    
      - **Stage:** Extract  
**Transformation:** Parse DicomDataset with fo-dicom  
**Dependencies:**
    
    - Ingest
    
      - **Stage:** Queue  
**Transformation:** Publish metadata DTO to RabbitMQ  
**Dependencies:**
    
    - Extract
    
      - **Stage:** Transform & Load  
**Transformation:** Map DTO to EF Core Entities and persist  
**Dependencies:**
    
    - Queue
    
      
    - **Parallelization:** False
    
  - **Processing Mode:**
    
    - **Real Time:**
      
      - **Required:** True
      - **Scenarios:**
        
        - DICOM C-STORE reception
        - Client-Service status checks via Named Pipes
        
      - **Latency Requirements:** < 200ms for acknowledgment
      
    - **Batch:**
      
      - **Required:** True
      - **Batch Size:** 1000
      - **Frequency:** On-demand
      
    - **Streaming:**
      
      - **Required:** False
      - **Streaming Framework:** N/A
      - **Windowing Strategy:** N/A
      
    
  - **Canonical Data Model:**
    
    - **Applicable:** False
    - **Scope:**
      
      
    - **Benefits:**
      
      
    
  
- **Version Handling Strategy:**
  
  - **Schema Evolution:**
    
    - **Strategy:** Tolerant Reader
    - **Versioning Scheme:** Semantic Versioning on shared DTO library
    - **Compatibility:**
      
      - **Backward:** True
      - **Forward:** False
      - **Reasoning:** The consumer (service) must be able to process messages from older clients. As client and service are deployed in a controlled manner, forward compatibility is not a strict requirement.
      
    
  - **Transformation Versioning:**
    
    - **Mechanism:** Source control and versioning of the shared library containing DTOs and mapping logic.
    - **Version Identification:** DLL version
    - **Migration Strategy:** Deploy updated service first, then client.
    
  - **Data Model Changes:**
    
    - **Migration Path:** Entity Framework Core Migrations
    - **Rollback Strategy:** EF Core CLI `database update <previous_migration>`
    - **Validation Strategy:** Automated tests and manual verification in a staging environment.
    
  - **Schema Registry:**
    
    - **Required:** False
    - **Technology:** N/A
    - **Governance:** Schema is managed via shared C# DTO classes in a common library.
    
  
- **Performance Optimization:**
  
  - **Critical Requirements:**
    
    - **Operation:** DICOM Data Ingestion (C-STORE to DB)  
**Max Latency:** N/A (asynchronous)  
**Throughput Target:** Handle 10 concurrent C-STORE associations (REQ-1-078)  
**Justification:** Must not create backpressure on sending modalities.  
    - **Operation:** Large Study Viewer Load  
**Max Latency:** < 3 seconds for 500MB study (REQ-1-077)  
**Throughput Target:** N/A  
**Justification:** Core user experience requirement for clinical workflow.  
    
  - **Parallelization Opportunities:**
    
    
  - **Caching Strategies:**
    
    - **Cache Type:** In-Memory  
**Cache Scope:** Application-level (Singleton service)  
**Eviction Policy:** Never (reload on application start)  
**Applicable Transformations:**
    
    - SystemSetting lookup
    - Role lookup
    - PacsConfiguration lookup
    
    
  - **Memory Optimization:**
    
    - **Techniques:**
      
      - Progressive loading for large DICOM series (REQ-1-052)
      - Stream processing for file I/O where possible
      
    - **Thresholds:** N/A
    - **Monitoring Required:** False
    
  - **Lazy Evaluation:**
    
    - **Applicable:** True
    - **Scenarios:**
      
      - Loading full-resolution DICOM pixel data only when the user zooms in (REQ-1-052).
      
    - **Implementation:** Initially load downsampled previews, fetch full-resolution data on-demand.
    
  - **Bulk Processing:**
    
    - **Required:** True
    - **Batch Sizes:**
      
      - **Optimal:** 1000
      - **Maximum:** 5000
      
    - **Parallelism:** 1
    
  
- **Error Handling And Recovery:**
  
  - **Error Handling Strategies:**
    
    - **Error Type:** Asynchronous Message Processing Failure  
**Strategy:** After configurable retries, move message to a Dead Letter Queue (DLQ).  
**Fallback Action:** Log error with correlation ID, send alert to administrator.  
**Escalation Path:**
    
    - System Administrator
    
    - **Error Type:** DICOM File Validation Failure  
**Strategy:** Reject the non-compliant file and continue with the rest of the import batch.  
**Fallback Action:** Provide a summary report to the user detailing rejected files and reasons (REQ-1-057).  
**Escalation Path:**
    
    - User
    
    
  - **Logging Requirements:**
    
    - **Log Level:** info
    - **Included Data:**
      
      - Timestamp
      - LogLevel
      - Message
      - ExceptionDetails
      - CorrelationID (REQ-1-090)
      
    - **Retention Period:** 30 days
    - **Alerting:** True
    
  - **Partial Success Handling:**
    
    - **Strategy:** For batch imports or migrations, process valid records and report on failed records without rolling back the entire transaction.
    - **Reporting Mechanism:** End-of-process summary log and UI message.
    - **Recovery Actions:**
      
      - Manual correction and re-import of failed records.
      
    
  - **Circuit Breaking:**
    
    
  - **Retry Strategies:**
    
    - **Operation:** RabbitMQ Message Consumption  
**Max Retries:** 3  
**Backoff Strategy:** exponential  
**Retry Conditions:**
    
    - Transient database connection error
    - Temporary network failure
    
    
  - **Error Notifications:**
    
    - **Condition:** Message is sent to the Dead Letter Queue.  
**Recipients:**
    
    - Administrator Email
    
**Severity:** critical  
**Channel:** Email  
    - **Condition:** A critical error is logged to the Windows Event Log.  
**Recipients:**
    
    - Administrator Email
    
**Severity:** critical  
**Channel:** Email  
    
  
- **Project Specific Transformations:**
  
  ### .1. DICOM Metadata Ingestion
  Extracts metadata from a received DICOM file and maps it to the normalized PostgreSQL schema. This is the primary data ingestion transformation.

  #### .1.1. Transformation Id
  T-001

  #### .1.4. Source
  
  - **Service:** DICOM C-STORE SCP
  - **Model:** DicomDataset (fo-dicom)
  - **Fields:**
    
    - PatientName
    - PatientID
    - StudyInstanceUID
    - SeriesInstanceUID
    - SOPInstanceUID
    - Modality
    - StudyDate
    
  
  #### .1.5. Target
  
  - **Service:** Data Access Layer
  - **Model:** Patient, Study, Series, Image Entities
  - **Fields:**
    
    - patientName
    - dicomPatientId
    - studyInstanceUid
    - seriesInstanceUid
    - sopInstanceUid
    - modality
    - studyDate
    
  
  #### .1.6. Transformation
  
  - **Type:** splitting
  - **Logic:** A single DICOM dataset is de-normalized into multiple related entities. Logic must handle creating a new Patient/Study if it doesn't exist, or linking to an existing one.
  - **Configuration:**
    
    
  
  #### .1.7. Frequency
  real-time

  #### .1.8. Criticality
  critical

  #### .1.9. Dependencies
  
  - REQ-1-010
  
  #### .1.10. Validation
  
  - **Pre Transformation:**
    
    - Verify file is valid DICOM.
    
  - **Post Transformation:**
    
    - Verify all required entity fields are populated.
    
  
  #### .1.11. Performance
  
  - **Expected Volume:** High burst, low average
  - **Latency Requirement:** N/A (async)
  - **Optimization Strategy:** Decoupling via message queue.
  
  ### .2. Print Job Submission
  Packages UI state from the print preview screen into a serializable command DTO for asynchronous processing.

  #### .2.1. Transformation Id
  T-002

  #### .2.4. Source
  
  - **Service:** WPF Client
  - **Model:** PrintPreviewViewModel
  - **Fields:**
    
    - SelectedPrinter
    - SelectedLayout
    - ImageSopUids
    - PrintSettings
    
  
  #### .2.5. Target
  
  - **Service:** RabbitMQ Message Bus
  - **Model:** SubmitPrintJobCommand DTO
  - **Fields:**
    
    - PrinterName
    - LayoutDefinition
    - ImageReferences
    - PrintSettings
    
  
  #### .2.6. Transformation
  
  - **Type:** direct
  - **Logic:** Direct mapping of ViewModel properties to DTO properties.
  - **Configuration:**
    
    
  
  #### .2.7. Frequency
  on-demand

  #### .2.8. Criticality
  high

  #### .2.9. Dependencies
  
  - REQ-1-004
  - REQ-1-021
  
  #### .2.10. Validation
  
  - **Pre Transformation:**
    
    - Verify a printer is selected.
    
  - **Post Transformation:**
    
    
  
  #### .2.11. Performance
  
  - **Expected Volume:** Low
  - **Latency Requirement:** < 100ms to publish
  - **Optimization Strategy:** Lightweight DTO serialization.
  
  
- **Implementation Priority:**
  
  - **Component:** DICOM Metadata Ingestion Transformation (T-001)  
**Priority:** high  
**Dependencies:**
    
    - Database Schema
    - RabbitMQ Infrastructure
    
**Estimated Effort:** Medium  
**Risk Level:** medium  
  - **Component:** Print Job Submission Transformation (T-002)  
**Priority:** high  
**Dependencies:**
    
    - RabbitMQ Infrastructure
    
**Estimated Effort:** Low  
**Risk Level:** low  
  - **Component:** Error Handling and DLQ Strategy  
**Priority:** high  
**Dependencies:**
    
    - RabbitMQ Infrastructure
    
**Estimated Effort:** Low  
**Risk Level:** low  
  - **Component:** Legacy Data Migration Utility (MAP-003)  
**Priority:** medium  
**Dependencies:**
    
    - Database Schema
    - Access to legacy system
    
**Estimated Effort:** High  
**Risk Level:** high  
  
- **Risk Assessment:**
  
  - **Risk:** Incorrect mapping of DICOM tags to database fields leads to corrupt or unusable metadata.  
**Impact:** high  
**Probability:** medium  
**Mitigation:** Create a detailed DICOM-to-Schema mapping document. Implement comprehensive unit and integration tests using a diverse set of real-world DICOM files.  
**Contingency Plan:** Develop a data correction script to fix incorrectly mapped data post-migration or post-release.  
  - **Risk:** Transformation logic in the data ingestion pipeline becomes a performance bottleneck, causing the RabbitMQ queue to grow indefinitely.  
**Impact:** high  
**Probability:** low  
**Mitigation:** Performance test the consumer under heavy load. Ensure database queries within the transformation logic are optimized with appropriate indexes. Monitor queue depth as per REQ-1-091.  
**Contingency Plan:** Temporarily increase the number of consumer instances (if architecture is adapted) or scale up the database server.  
  - **Risk:** A change in the message DTO schema between the client and service is not backward compatible, causing messages to fail processing and fill the DLQ.  
**Impact:** medium  
**Probability:** medium  
**Mitigation:** Strictly enforce backward compatibility for all DTO changes (additive changes only). Use a shared library for DTO contracts to ensure compile-time consistency.  
**Contingency Plan:** Roll back the client or service deployment. Manually re-process DLQ'd messages after deploying a compatible service version.  
  
- **Recommendations:**
  
  - **Category:** Implementation  
**Recommendation:** Utilize a lightweight object-to-object mapping library (e.g., Mapster) to automate and standardize the mapping between DTOs, ViewModels, and Entities.  
**Justification:** Reduces boilerplate code, enforces consistency, and makes mappings easier to manage and test compared to manual property assignments.  
**Priority:** high  
**Implementation Notes:** Define mapping configurations in a centralized location for maintainability.  
  - **Category:** Reliability  
**Recommendation:** Ensure all message consumers that perform database writes are idempotent.  
**Justification:** RabbitMQ's 'at-least-once' delivery guarantee means a message could be delivered more than once if an ACK is lost. Idempotent consumers (e.g., using 'INSERT ON CONFLICT' or checking for existence before insert) prevent data duplication.  
**Priority:** high  
**Implementation Notes:** Use the unique SOPInstanceUID or SeriesInstanceUID as the key for idempotency checks.  
  - **Category:** Security  
**Recommendation:** Implement a dedicated transformation step to mask or redact all PHI from data before it is logged.  
**Justification:** Fulfills the critical security requirement REQ-1-039. This should be a cross-cutting concern applied to all logging operations.  
**Priority:** critical  
**Implementation Notes:** Can be implemented as a custom sink or enricher in the Serilog logging pipeline.  
  


---

