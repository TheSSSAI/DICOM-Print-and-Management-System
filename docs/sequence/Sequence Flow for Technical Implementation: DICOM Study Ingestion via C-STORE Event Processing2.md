# Specification

# 1. Overview
## 1. DICOM Study Ingestion via C-STORE
An external medical imaging modality sends a DICOM study. The background service's C-STORE SCP listener receives the files, extracts metadata using the fo-dicom library, and publishes a ProcessDicomStoreCommand to a durable RabbitMQ queue. A separate DB Writer consumer service processes this message, writing study metadata to PostgreSQL and moving DICOM files to permanent storage. Finally, the consumer acknowledges the message, removing it from the queue.

### 1.1. Diagram Id
SEQ-EVP-002

### 1.4. Type
EventProcessing

### 1.5. Purpose
To reliably and efficiently ingest DICOM studies from external modalities without blocking the SCP listener, ensuring high throughput by decoupling reception from persistence.

### 1.6. Complexity
High

### 1.7. Priority
Critical

### 1.8. Frequency
OnDemand

### 1.9. Participants

- EXT-MODALITY
- REPO-02-SVC
- REPO-11-INF
- EXT-RABBITMQ
- REPO-10-DAC
- EXT-PGSQL

### 1.10. Key Interactions

- C-STORE SCP receives DICOM association and files.
- Metadata is extracted using fo-dicom library.
- A command message is published to a persistent RabbitMQ queue.
- DB Writer consumer receives the message.
- Data is written to Patient, Study, and Series tables in PostgreSQL within a transaction.
- DICOM files are moved to the configured storage path.
- Consumer sends an acknowledgment (ack) to RabbitMQ upon successful completion.

### 1.11. Triggers

- A DICOM C-STORE request is received from a modality.

### 1.12. Outcomes

- The DICOM study metadata is stored in the database.
- The DICOM files are stored on the local file system.
- The study becomes available for searching and viewing in the client application.

### 1.13. Business Rules

- Incoming data must be published to a message queue to decouple ingestion from persistence (REQ-TEC-001).
- Messages must be marked as persistent to survive broker restarts (REQ-TEC-002).

### 1.14. Error Scenarios

- RabbitMQ is unavailable.
- Database write fails, causing the message to be re-queued or dead-lettered.
- Received file is not a valid DICOM object.

### 1.15. Integration Points

- DICOM Network Protocol
- RabbitMQ Broker
- PostgreSQL Database
- Local File System


---

# 2. Details
## 2. Technical Implementation: DICOM Study Ingestion via C-STORE Event Processing
A comprehensive technical sequence for the asynchronous ingestion of DICOM studies. An external modality initiates a C-STORE operation. The service's DICOM SCP listener component, implemented via the infrastructure layer, receives and stages the files. It immediately publishes a 'ProcessDicomStoreCommand' to a durable, persistent RabbitMQ queue to decouple high-throughput reception from slower persistence logic, as per REQ-TEC-001. A separate consumer within the service processes the message transactionally, persisting metadata to PostgreSQL and moving files to permanent storage before acknowledging the message. This design ensures high availability, reliability, and handles backpressure gracefully.

### 2.1. Diagram Id
SEQ-EVP-002-IMPL

### 2.4. Participants

- **Repository Id:** EXT-MODALITY  
**Display Name:** External DICOM Modality  
**Type:** Actor  
**Technology:** DICOM-compliant Imaging Equipment  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #999999
    - **Stereotype:** External
    
- **Repository Id:** REPO-02-SVC  
**Display Name:** DMPS Windows Service  
**Type:** Service  
**Technology:** .NET 8 IHostedService (DicomScpService, RabbitMqConsumerService)  
**Order:** 2  
**Style:**
    
    - **Shape:** component
    - **Color:** #1168BD
    - **Stereotype:** Application Service
    
- **Repository Id:** REPO-11-INF  
**Display Name:** Infrastructure Layer  
**Type:** Infrastructure  
**Technology:** fo-dicom, RabbitMQ.Client, System.IO  
**Order:** 3  
**Style:**
    
    - **Shape:** component
    - **Color:** #6B439C
    - **Stereotype:** Infrastructure
    
- **Repository Id:** EXT-RABBITMQ  
**Display Name:** RabbitMQ Broker  
**Type:** Message Broker  
**Technology:** RabbitMQ Server  
**Order:** 4  
**Style:**
    
    - **Shape:** queue
    - **Color:** #FF6600
    - **Stereotype:** External
    
- **Repository Id:** REPO-10-DAC  
**Display Name:** Data Access Layer  
**Type:** Repository  
**Technology:** Entity Framework Core 8, Npgsql  
**Order:** 5  
**Style:**
    
    - **Shape:** component
    - **Color:** #1E8449
    - **Stereotype:** Data Access
    
- **Repository Id:** EXT-PGSQL  
**Display Name:** PostgreSQL Database  
**Type:** Database  
**Technology:** PostgreSQL 16  
**Order:** 6  
**Style:**
    
    - **Shape:** database
    - **Color:** #336791
    - **Stereotype:** External
    

### 2.5. Interactions

- **Source Id:** EXT-MODALITY  
**Target Id:** REPO-02-SVC  
**Message:** 1. [C-STORE-RQ] Initiate DICOM Association & send DICOM objects.  
**Sequence Number:** 1  
**Type:** Asynchronous Request  
**Is Synchronous:** False  
**Has Return:** True  
**Return Message:** 20. [C-STORE-RSP] DICOM Success Response (0x0000)  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** DICOM
    - **Method:** C-STORE
    - **Parameters:** Called AE Title, DICOM SOP Instances (binary stream)
    - **Authentication:** AE Title validation against configured list of allowed sources.
    - **Error Handling:** Returns DICOM error code on failure (e.g., unrecognized AE Title).
    
- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-11-INF  
**Message:** 2. OnCStoreRequest(request): Delegate file handling and parsing.  
**Sequence Number:** 2  
**Type:** Method Call  
**Is Synchronous:** True  
**Has Return:** True  
**Return Message:** 4. return DicomMetadataDto  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** IDicomScpHandler.HandleCStoreRequestAsync
    - **Parameters:** DicomCStoreRequest request
    - **Authentication:** N/A
    - **Error Handling:** Exceptions are caught by the service and translated into a DICOM error response.
    
**Nested Interactions:**
    
    - **Source Id:** REPO-11-INF  
**Target Id:** REPO-11-INF  
**Message:** 3. ParseDicomFiles(fileStreams): Extract metadata using fo-dicom and validate.  
**Sequence Number:** 3  
**Type:** Self-Call  
**Is Synchronous:** True  
**Has Return:** False  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** DicomParser.ExtractMetadata
    - **Parameters:** List<Stream> dicomFileStreams
    - **Authentication:** N/A
    - **Error Handling:** Throws InvalidDicomFormatException if files are non-compliant, leading to C-STORE failure.
    
    
- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-11-INF  
**Message:** 5. PublishAsync<ProcessDicomStoreCommand>(command): Queue metadata for persistence.  
**Sequence Number:** 5  
**Type:** Method Call  
**Is Synchronous:** True  
**Has Return:** True  
**Return Message:** Acknowledges successful publication.  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** IMessageProducer.PublishAsync
    - **Parameters:** ProcessDicomStoreCommand command (contains DTO, temp file paths, CorrelationId)
    - **Authentication:** N/A
    - **Error Handling:** Throws MessageBrokerUnavailableException if RabbitMQ cannot be reached.
    
- **Source Id:** REPO-11-INF  
**Target Id:** EXT-RABBITMQ  
**Message:** 6. [AMQP] basic.publish to 'dicom_system_exchange' with routing key 'dicom.store.process'.  
**Sequence Number:** 6  
**Type:** Event Publication  
**Is Synchronous:** False  
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** AMQP 0-9-1
    - **Method:** basic.publish
    - **Parameters:** Body: JSON serialized command. Properties: delivery_mode=2 (persistent), correlation_id=<guid>.
    - **Authentication:** TLS and credentials for RabbitMQ connection.
    - **Error Handling:** Broker returns error if exchange is not found. Publisher confirms handle delivery failures.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** EXT-MODALITY  
**Message:** 7. [C-STORE-RSP] Send DICOM success response (0x0000).  
**Sequence Number:** 7  
**Type:** Asynchronous Response  
**Is Synchronous:** False  
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** DICOM
    - **Method:** C-STORE Response
    - **Parameters:** Status Code: Success (0x0000)
    - **Authentication:** N/A
    - **Error Handling:** N/A
    
- **Source Id:** EXT-RABBITMQ  
**Target Id:** REPO-11-INF  
**Message:** 8. [AMQP] Delivers message from 'dicom_store_queue' to subscribed consumer.  
**Sequence Number:** 8  
**Type:** Message Delivery  
**Is Synchronous:** False  
**Has Return:** True  
**Return Message:** Invokes the message handler logic in the service.  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** AMQP 0-9-1
    - **Method:** basic.deliver
    - **Parameters:** Message payload and properties.
    - **Authentication:** N/A (part of established consumer channel)
    - **Error Handling:** N/A
    
- **Source Id:** REPO-11-INF  
**Target Id:** REPO-02-SVC  
**Message:** 9. HandleMessageAsync(ProcessDicomStoreCommand): Invoke consumer logic.  
**Sequence Number:** 9  
**Type:** Method Call  
**Is Synchronous:** True  
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** DicomStoreConsumer.ProcessMessageAsync
    - **Parameters:** ProcessDicomStoreCommand command
    - **Authentication:** N/A
    - **Error Handling:** Catches exceptions from downstream services to determine if message should be Ack'd or Nack'd.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-10-DAC  
**Message:** 10. PersistStudyTransactionAsync(command.Metadata): Persist metadata within a transaction.  
**Sequence Number:** 10  
**Type:** Method Call  
**Is Synchronous:** True  
**Has Return:** True  
**Return Message:** 14. return persistence result (success/failure)  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** IStudyRepository.PersistStudyTransactionAsync
    - **Parameters:** DicomMetadataDto metadata
    - **Authentication:** N/A
    - **Error Handling:** Manages EF Core DbContext transaction. Throws exceptions on database errors.
    
**Nested Interactions:**
    
    - **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 11-13. [SQL] Execute transactional INSERT/UPDATE for Patient, Study, Series, Image.  
**Sequence Number:** 11  
**Type:** Database Query  
**Is Synchronous:** True  
**Has Return:** False  
**Technical Details:**
    
    - **Protocol:** PostgreSQL Wire Protocol
    - **Method:** BEGIN; INSERT ... ON CONFLICT ...; COMMIT/ROLLBACK;
    - **Parameters:** Structured entity data.
    - **Authentication:** Database user credentials (via connection string).
    - **Error Handling:** PostgreSQL returns error codes for constraint violations or other failures.
    
    
- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-11-INF  
**Message:** 15. MoveDicomFilesToPermanentStorage(tempPaths, permanentPaths).  
**Sequence Number:** 15  
**Type:** Method Call  
**Is Synchronous:** True  
**Has Return:** True  
**Return Message:** 16. return file move result (success/failure)  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** IFileStorage.MoveToPermanentLocationAsync
    - **Parameters:** List<string> sourcePaths, string destinationPath
    - **Authentication:** N/A
    - **Error Handling:** Throws IOException on file system errors (e.g., permissions, disk full). This will trigger a database transaction rollback.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-11-INF  
**Message:** 17. AcknowledgeMessage(deliveryTag): Confirm successful processing.  
**Sequence Number:** 17  
**Type:** Method Call  
**Is Synchronous:** True  
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** IMessageConsumer.Ack
    - **Parameters:** ulong deliveryTag
    - **Authentication:** N/A
    - **Error Handling:** N/A
    
- **Source Id:** REPO-11-INF  
**Target Id:** EXT-RABBITMQ  
**Message:** 18. [AMQP] basic.ack to remove message from 'dicom_store_queue'.  
**Sequence Number:** 18  
**Type:** Acknowledgment  
**Is Synchronous:** False  
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** AMQP 0-9-1
    - **Method:** basic.ack
    - **Parameters:** delivery_tag, multiple=false
    - **Authentication:** N/A
    - **Error Handling:** If the broker connection is lost, the un-acked message will be redelivered upon consumer reconnection.
    

### 2.6. Notes

- **Content:** Decoupling Point (REQ-TEC-001): The C-STORE Success response is sent at Step 7, immediately after the job is queued. The modality is not blocked waiting for database persistence, enabling high-throughput ingestion.  
**Position:** top  
**Participant Id:** REPO-02-SVC  
**Sequence Number:** 7  
- **Content:** Transactional Boundary: The database write (Step 10) and file move (Step 15) must be atomic. If the file move fails, the database transaction is rolled back and the message is rejected (Nack'd) to be retried or dead-lettered.  
**Position:** right  
**Participant Id:** REPO-02-SVC  
**Sequence Number:** 15  
- **Content:** Idempotency: The consumer logic at Step 10 must be idempotent. The Data Access Layer should use 'INSERT ON CONFLICT' or similar checks to prevent creating duplicate records if a message is redelivered by RabbitMQ.  
**Position:** right  
**Participant Id:** REPO-10-DAC  
**Sequence Number:** 10  
- **Content:** Error Handling & DLQ (REQ-TEC-002): If any step after 9 fails permanently, the consumer sends a 'basic.nack' with requeue=false. RabbitMQ then routes the poison message to the configured Dead-Letter Queue for manual inspection and administrator alerting.  
**Position:** bottom  
**Participant Id:** EXT-RABBITMQ  
**Sequence Number:** 18  

### 2.7. Implementation Guidance

- **Security Requirements:** DICOM SCP listener must be configured to only accept associations from known AE Titles. All communication with PostgreSQL (TLS) and RabbitMQ (TLS) must be encrypted. Credentials for external services must be stored securely using Windows Credential Manager (REQ-NFR-004).
- **Performance Targets:** The C-STORE SCP listener must handle at least 10 simultaneous operations without significant degradation (REQ-NFR-002). The time from receiving a C-STORE request to sending the success response (Steps 1-7) should be under 500ms to avoid timing out the modality.
- **Error Handling Strategy:** The message consumer in the service will implement an in-process retry policy (e.g., using Polly) for transient errors (DB connection timeout, temporary file lock). After 3 failed retries, it will issue a 'basic.nack' with requeue=false to route the message to the DLQ. Invalid DICOM files at Step 3 should be rejected immediately with a C-STORE failure response.
- **Testing Considerations:** Integration tests must cover: 1) RabbitMQ or DB being unavailable during processing. 2) Invalid/corrupt DICOM files being sent. 3) File system permission errors during the file move operation. 4) Race conditions and idempotency by sending the same message twice.
- **Monitoring Requirements:** The depth of the 'dicom_store_queue' and the message count in the associated DLQ are critical metrics for the System Health Dashboard (REQ-REP-002). All log entries for this entire flow must be tagged with the same Correlation ID for end-to-end tracing.
- **Deployment Considerations:** The RabbitMQ exchange ('dicom_system_exchange'), queue ('dicom_store_queue'), and the DLQ topology must be declared and configured as part of the application deployment or first-run setup. The permanent DICOM storage path must be configurable and have appropriate write permissions for the service account.


---

