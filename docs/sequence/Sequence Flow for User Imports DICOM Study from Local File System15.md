# Specification

# 1. Overview
## 1. User Imports DICOM Study from Local File System
A user imports DICOM files by dragging and dropping them from a local folder onto the application. The client validates the files, checks for duplicate Study UIDs, and then uses the same asynchronous message-based ingestion pipeline as the C-STORE SCP to process and store the study.

### 1.1. Diagram Id
SEQ-DTA-015

### 1.4. Type
DataFlow

### 1.5. Purpose
To allow users to import DICOM studies from local media or file shares into the system.

### 1.6. Complexity
Medium

### 1.7. Priority
High

### 1.8. Frequency
OnDemand

### 1.9. Participants

- REPO-09-PRE
- REPO-08-APC
- EXT-RABBITMQ
- REPO-02-SVC
- REPO-10-DAC
- EXT-PGSQL

### 1.10. Key Interactions

- User drags and drops DICOM files or a folder onto the client window.
- The client application validates the files are DICOM compliant.
- The client checks the database for existing Study Instance UIDs and prompts the user on conflict (Overwrite/Discard/Save as New).
- For valid new studies, the client publishes a ProcessDicomStoreCommand to RabbitMQ.
- The background service consumes the message and processes the study (identical to SEQ-EVP-002).

### 1.11. Triggers

- User performs a drag-and-drop import operation (FR-3.6.1).

### 1.12. Outcomes

- The DICOM study from the local file system is successfully imported, stored, and available in the application.
- The user receives feedback on the import progress and any file rejections.

### 1.13. Business Rules

- The system must handle import conflicts for existing Study UIDs (FR-3.6.1.1).
- Non-compliant files must be rejected with a report to the user (FR-3.6.2).

### 1.14. Error Scenarios

- User imports a non-DICOM file.
- Storage path is out of disk space.

### 1.15. Integration Points

- Local File System
- RabbitMQ Broker
- PostgreSQL Database


---

# 2. Details
## 2. User Imports DICOM Study from Local File System
A comprehensive sequence diagram detailing the technical implementation of importing DICOM files via drag-and-drop. It covers client-side validation, synchronous duplicate checking via Named Pipes, and asynchronous processing via a RabbitMQ message queue, culminating in transactional database persistence.

### 2.1. Diagram Id
SEQ-DTA-015

### 2.4. Participants

- **Repository Id:** REPO-09-PRE  
**Display Name:** Presentation Layer (Client)  
**Type:** UI Layer  
**Technology:** WPF, XAML, MVVM  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #3498DB
    - **Stereotype:** UI
    
- **Repository Id:** REPO-08-APC  
**Display Name:** Application Service (Client)  
**Type:** Application Service  
**Technology:** .NET 8, C# 12  
**Order:** 2  
**Style:**
    
    - **Shape:** participant
    - **Color:** #2ECC71
    - **Stereotype:** Client Logic
    
- **Repository Id:** REPO-02-SVC  
**Display Name:** Windows Service (Backend)  
**Type:** Background Service  
**Technology:** .NET 8, Microsoft.Extensions.Hosting  
**Order:** 3  
**Style:**
    
    - **Shape:** participant
    - **Color:** #F39C12
    - **Stereotype:** Service Host
    
- **Repository Id:** REPO-10-DAC  
**Display Name:** Data Access Layer (Service)  
**Type:** Repository Layer  
**Technology:** Entity Framework Core 8  
**Order:** 4  
**Style:**
    
    - **Shape:** participant
    - **Color:** #E74C3C
    - **Stereotype:** Repository
    
- **Repository Id:** EXT-PGSQL  
**Display Name:** PostgreSQL Database  
**Type:** Database  
**Technology:** PostgreSQL 16  
**Order:** 5  
**Style:**
    
    - **Shape:** database
    - **Color:** #9B59B6
    - **Stereotype:** External DB
    
- **Repository Id:** EXT-RABBITMQ  
**Display Name:** RabbitMQ Broker  
**Type:** Message Broker  
**Technology:** RabbitMQ  
**Order:** 6  
**Style:**
    
    - **Shape:** queue
    - **Color:** #E67E22
    - **Stereotype:** External Broker
    

### 2.5. Interactions

- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-08-APC  
**Message:** 1. User initiates Drag-and-Drop: Triggers `HandleFileDropAsync(string[] filePaths)`  
**Sequence Number:** 1  
**Type:** UI Event  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** WPF Event Aggregator
    - **Method:** HandleFileDropAsync
    - **Parameters:** Array of file system paths dropped by the user.
    - **Authentication:** User context established at login.
    - **Error Handling:** Logs errors if event payload is invalid.
    - **Performance:** Must execute asynchronously to avoid UI lock.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-08-APC  
**Message:** 2. [Loop] Validate files and extract Study UIDs  
**Sequence Number:** 2  
**Type:** Internal Processing  
**Is Synchronous:** True  
**Return Message:** List of valid DicomObjects and rejected file reports.  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** File I/O & In-Memory
    - **Method:** DICOM validation using fo-dicom library.
    - **Parameters:** File paths.
    - **Authentication:** N/A
    - **Error Handling:** Catches exceptions for non-DICOM or corrupt files; adds them to a rejection report per FR-3.6.2.
    - **Performance:** Reads file headers only for initial validation to improve speed.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-02-SVC  
**Message:** 3. [Loop] Check for duplicate Study Instance UID  
**Sequence Number:** 3  
**Type:** Synchronous Request  
**Is Synchronous:** True  
**Return Message:** 4. Returns `{"exists": true/false}`  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** Named Pipes
    - **Method:** SendRequestAsync('CHECK_UID:{"uid":"..."}')
    - **Parameters:** JSON serialized request containing the StudyInstanceUID to check.
    - **Authentication:** Local machine communication, implicitly trusted.
    - **Error Handling:** Handles PipeBrokenException or TimeoutException if service is down or unresponsive.
    - **Performance:** Low latency (<50ms) required for responsive UI.
    
**Nested Interactions:**
    
    - **Source Id:** REPO-02-SVC  
**Target Id:** REPO-10-DAC  
**Message:** 3.1. `StudyExistsAsync(studyUid)`  
**Sequence Number:** 3.1  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** Returns boolean.  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** Direct method call (DI)
    - **Method:** StudyExistsAsync
    - **Parameters:** string studyUid
    - **Authentication:** N/A
    - **Error Handling:** Propagates database exceptions.
    - **Performance:** Sub-millisecond.
    
    - **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 3.2. `SELECT 1 FROM "Studies" WHERE "StudyInstanceUid" = @p0 LIMIT 1`  
**Sequence Number:** 3.2  
**Type:** Database Query  
**Is Synchronous:** True  
**Return Message:** Returns 1 row or 0 rows.  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** SQL/Npgsql
    - **Method:** EF Core `AnyAsync` translation
    - **Parameters:** StudyInstanceUid
    - **Authentication:** Via connection string credentials.
    - **Error Handling:** Handled by EF Core; throws NpgsqlException on failure.
    - **Performance:** Query must use the index on 'StudyInstanceUid' for high performance.
    
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-09-PRE  
**Message:** 5. If duplicate found, show conflict resolution dialog (FR-3.6.1.1)  
**Sequence Number:** 5  
**Type:** UI Update  
**Is Synchronous:** True  
**Return Message:** 6. Returns user's choice (Overwrite, Discard, Save as New).  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** MVVM / Data Binding
    - **Method:** DisplayModalView
    - **Parameters:** DialogViewModel with conflict details.
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** Modal dialog is blocking for the user as intended.
    
- **Source Id:** REPO-08-APC  
**Target Id:** EXT-RABBITMQ  
**Message:** 7. For valid studies, publish `ProcessDicomStoreCommand`  
**Sequence Number:** 7  
**Type:** Message Publish  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** AMQP
    - **Method:** BasicPublish
    - **Parameters:** Exchange: `dicom_system_exchange`, RoutingKey: `dicom.store.process`, Message: Serialized DTO with staged file paths, metadata, and a new CorrelationId. Message is marked as persistent.
    - **Authentication:** Via connection string credentials.
    - **Error Handling:** Handles exceptions if RabbitMQ broker is unreachable. Notifies user of failure.
    - **Performance:** Publishing is a fast, non-blocking operation.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-09-PRE  
**Message:** 8. Display non-blocking notification: 'Import Queued'  
**Sequence Number:** 8  
**Type:** UI Update  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** MVVM / Notification Service
    - **Method:** ShowToastNotification
    - **Parameters:** Message, Status=Info
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** Sub-millisecond.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** EXT-RABBITMQ  
**Message:** 9. Consume `ProcessDicomStoreCommand` message  
**Sequence Number:** 9  
**Type:** Message Consume  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** AMQP
    - **Method:** Event handler for queue `dicom_store_queue`
    - **Parameters:** Deserialized DTO.
    - **Authentication:** Via connection string credentials.
    - **Error Handling:** Message ACK is withheld until processing is complete. On failure, message is NACK'd for retry or DLQ.
    - **Performance:** Consumer runs in a long-lived background thread.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-02-SVC  
**Message:** 10. Move files from staging to permanent storage path  
**Sequence Number:** 10  
**Type:** Internal Processing  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** File I/O
    - **Method:** File.Move
    - **Parameters:** Source path from message, destination path derived from DICOM metadata per FR-3.6.2.
    - **Authentication:** Service account must have write permissions to the storage path.
    - **Error Handling:** Handles IOException (e.g., disk full, path not found). Triggers a message processing failure.
    - **Performance:** I/O bound operation; performance depends on storage subsystem.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-10-DAC  
**Message:** 11. Call `AddStudyAsync(study)` within a transaction  
**Sequence Number:** 11  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** Direct method call (DI)
    - **Method:** AddStudyAsync
    - **Parameters:** Populated Patient, Study, Series, and Image entities.
    - **Authentication:** N/A
    - **Error Handling:** The data access method must implement a Unit of Work pattern, wrapping all database operations in a single EF Core transaction.
    - **Performance:** Sub-millisecond call overhead.
    
- **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 12. Execute Transaction: `BEGIN; INSERT...; COMMIT;`  
**Sequence Number:** 12  
**Type:** Database Transaction  
**Is Synchronous:** True  
**Return Message:** Transaction success/failure.  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** SQL/Npgsql
    - **Method:** EF Core `SaveChangesAsync` within a transaction scope.
    - **Parameters:** Mapped entity objects.
    - **Authentication:** Via connection string credentials.
    - **Error Handling:** On any SQL error (e.g., constraint violation), the transaction is rolled back and a DbUpdateException is thrown.
    - **Performance:** Performance is dependent on database indexing and hardware.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** EXT-RABBITMQ  
**Message:** 13. Acknowledge message (`BasicAck`) on success  
**Sequence Number:** 13  
**Type:** Message Acknowledge  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** AMQP
    - **Method:** BasicAck
    - **Parameters:** Delivery tag of the consumed message.
    - **Authentication:** N/A
    - **Error Handling:** If this fails, broker may redeliver the message. Consumer logic must be idempotent to handle this.
    - **Performance:** Sub-millisecond.
    

### 2.6. Notes

- **Content:** CRITICAL: The duplicate check in step 3 must be synchronous to provide immediate feedback to the user before queuing the import. This necessitates a synchronous IPC mechanism like Named Pipes, as the client does not have direct DB access.  
**Position:** top  
**Participant Id:** REPO-08-APC  
**Sequence Number:** 3  
- **Content:** The processing logic from step 9 onwards is identical to the C-STORE SCP ingestion path (SEQ-EVP-002), reusing the same command and consumer for consistency and maintainability.  
**Position:** bottom  
**Participant Id:** REPO-02-SVC  
**Sequence Number:** 9  
- **Content:** On processing failure (e.g., disk full, DB error), the message will be NACK'd. After configured retries, it will be sent to the Dead-Letter Queue (DLQ) for manual admin intervention.  
**Position:** bottom  
**Participant Id:** REPO-02-SVC  
**Sequence Number:** 13  

### 2.7. Implementation Guidance

- **Security Requirements:** The Windows Service account requires read/write/delete permissions on the configured DICOM storage and staging paths. Database credentials must be stored securely using the Windows Credential Manager (REQ-1-084). The import action must be logged in the audit trail.
- **Performance Targets:** Client-side validation of 100 files should complete in under 10 seconds. UI must remain responsive throughout. The backend ingestion throughput should match or exceed that of the C-STORE SCP.
- **Error Handling Strategy:** Client-side: Gracefully handle non-DICOM files and I/O errors, reporting them to the user without halting the entire import batch. Handle service unavailability during the synchronous duplicate check. Service-side: Utilize a retry-then-DLQ pattern for message processing. Ensure database transactions are atomic. The consumer must be idempotent to safely handle message redeliveries.
- **Testing Considerations:** Test with various DICOM files (multi-frame, different modalities). Test the duplicate conflict resolution UI flow for all user choices. Test error conditions: non-DICOM files, read-only files, service being down during duplicate check, RabbitMQ being down during publish, service consumer failing due to full disk or DB connection loss.
- **Monitoring Requirements:** Monitor the `dicom_store_queue` depth. An alert should be triggered if the queue depth grows unexpectedly, indicating a processing bottleneck or failure. The DLQ depth must be monitored, with any message triggering a CRITICAL alert.
- **Deployment Considerations:** The client application and the service must agree on the format of the Named Pipe requests and the schema of the `ProcessDicomStoreCommand` message. A shared library should be used for these contracts.


---

