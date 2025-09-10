# Specification

# 1. Overview
## 1. Asynchronous Print Job Submission and Processing
A user finalizes a print layout in the WPF client and initiates printing. The client service serializes the print job into a SubmitPrintJobCommand and publishes it to a RabbitMQ queue. This provides immediate feedback to the user that the job is 'Queued'. The background Windows Service consumes this message, generates the print document, and spools it to the selected Windows printer.

### 1.1. Diagram Id
SEQ-EVP-003

### 1.4. Type
EventProcessing

### 1.5. Purpose
To offload the potentially slow process of print document generation and spooling from the UI thread, ensuring the client application remains responsive.

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
- REPO-11-INF
- EXT-OS

### 1.10. Key Interactions

- User clicks 'Print' in the Print Preview UI.
- Client application publishes a persistent message to a RabbitMQ queue.
- Background service's Print Job Consumer receives the message.
- Service retrieves DICOM images from storage.
- Service generates the print output using the specified layout and overlays.
- Service submits the job to the Windows Print API.
- Consumer acknowledges the message on successful spooling.

### 1.11. Triggers

- User clicks the 'Print' button (FR-3.1.1.1).

### 1.12. Outcomes

- A print job is successfully sent to the target printer.
- The user is immediately notified that the job has been queued.
- The job appears in the 'Print Queue' UI view with a 'Queued' status.

### 1.13. Business Rules

- Communication for asynchronous tasks must be handled via a message queue (REQ-TEC-002).
- The 'Print' button must be disabled if the service is unavailable (FR-3.1.1.1).

### 1.14. Error Scenarios

- The selected printer is offline or out of paper.
- The DICOM files for the print job are missing.
- The message fails processing and is sent to the DLQ.

### 1.15. Integration Points

- RabbitMQ Broker
- Local File System
- Windows Print API


---

# 2. Details
## 2. Technical Sequence: Asynchronous Print Job Submission and Processing via RabbitMQ
This sequence diagram provides a detailed technical specification for the asynchronous submission and processing of a DICOM print job. The process is initiated from the WPF client, which offloads the task to a background Windows Service using RabbitMQ as a message broker. This architecture ensures UI responsiveness, reliability through persistent messaging, and robust error handling via a Dead Letter Queue (DLQ) mechanism.

### 2.1. Diagram Id
SEQ-EVP-003

### 2.4. Participants

- **Repository Id:** REPO-09-PRE  
**Display Name:** Presentation Layer  
**Type:** WPF UI (MVVM)  
**Technology:** WPF, .NET 8  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #1E90FF
    - **Stereotype:** User Interface
    
- **Repository Id:** REPO-08-APC  
**Display Name:** Client Application Service  
**Type:** Application Service  
**Technology:** .NET 8  
**Order:** 2  
**Style:**
    
    - **Shape:** participant
    - **Color:** #4682B4
    - **Stereotype:** Client Logic
    
- **Repository Id:** EXT-RABBITMQ  
**Display Name:** RabbitMQ Broker  
**Type:** Message Broker  
**Technology:** RabbitMQ  
**Order:** 3  
**Style:**
    
    - **Shape:** queue
    - **Color:** #FF4500
    - **Stereotype:** External Dependency
    
- **Repository Id:** REPO-02-SVC  
**Display Name:** Background Service  
**Type:** Windows Service  
**Technology:** .NET 8 Hosted Service  
**Order:** 4  
**Style:**
    
    - **Shape:** participant
    - **Color:** #32CD32
    - **Stereotype:** Backend Process
    
- **Repository Id:** REPO-11-INF  
**Display Name:** Infrastructure Services  
**Type:** Infrastructure Layer  
**Technology:** fo-dicom, QuestPDF, System.IO  
**Order:** 5  
**Style:**
    
    - **Shape:** participant
    - **Color:** #DAA520
    - **Stereotype:** Infrastructure
    
- **Repository Id:** REPO-01-SHK  
**Display Name:** Data Access Layer  
**Type:** Repository  
**Technology:** Entity Framework Core 8  
**Order:** 6  
**Style:**
    
    - **Shape:** database
    - **Color:** #9370DB
    - **Stereotype:** Persistence
    
- **Repository Id:** EXT-OS  
**Display Name:** Windows Print Spooler  
**Type:** Operating System API  
**Technology:** Windows Print API (GDI+)  
**Order:** 7  
**Style:**
    
    - **Shape:** boundary
    - **Color:** #778899
    - **Stereotype:** External System
    

### 2.5. Interactions

- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-08-APC  
**Message:** Invoke IPrintJobService.SubmitPrintJobAsync(printJobData)  
**Sequence Number:** 1  
**Type:** Method Call  
**Is Synchronous:** True  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** SubmitPrintJobAsync
    - **Parameters:** printJobData: Contains selected printer, layout, image UIDs, overlays, etc.
    - **Authentication:** N/A (internal call)
    - **Error Handling:** Exceptions are caught and displayed as a UI notification.
    - **Performance:** Should complete in < 200ms.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-01-SHK  
**Message:** CreatePrintJobRecordAsync(jobDetails)  
**Sequence Number:** 2  
**Type:** Database Call  
**Is Synchronous:** True  
**Has Return:** True  
**Is Activation:** True  
**Return Message:** printJobId  
**Technical Details:**
    
    - **Protocol:** EF Core
    - **Method:** AddAsync, SaveChangesAsync
    - **Parameters:** Creates a new PrintJob entity with initial 'Queued' status.
    - **Authentication:** Database connection string credentials.
    - **Error Handling:** Database exceptions propagated to the service layer.
    - **Performance:** DB write should complete in < 50ms.
    
- **Source Id:** REPO-08-APC  
**Target Id:** EXT-RABBITMQ  
**Message:** Publish(SubmitPrintJobCommand)  
**Sequence Number:** 3  
**Type:** Message Publication  
**Is Synchronous:** False  
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** AMQP
    - **Method:** BasicPublish
    - **Parameters:** Payload: Serialized SubmitPrintJobCommand DTO. Headers: X-Correlation-ID. Properties: Persistent=true.
    - **Authentication:** RabbitMQ user credentials.
    - **Error Handling:** Handles connection errors to RabbitMQ broker with retry logic.
    - **Performance:** Publishing should be fire-and-forget, completing in < 20ms.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-09-PRE  
**Message:** Return 'Queued' status acknowledgement  
**Sequence Number:** 4  
**Type:** Return Value  
**Is Synchronous:** True  
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** Task<SubmitStatus>
    - **Parameters:** Returns an enum indicating the job was successfully queued.
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** Immediate return after message publication.
    
- **Source Id:** EXT-RABBITMQ  
**Target Id:** REPO-02-SVC  
**Message:** Deliver Message: SubmitPrintJobCommand  
**Sequence Number:** 5  
**Type:** Message Delivery  
**Is Synchronous:** False  
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** AMQP
    - **Method:** Basic.Deliver
    - **Parameters:** Consumer receives the serialized command from its subscribed queue.
    - **Authentication:** N/A (occurs on connection)
    - **Error Handling:** Consumer host handles exceptions during message reception.
    - **Performance:** Dependent on message broker latency.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-01-SHK  
**Message:** UpdatePrintJobStatusAsync(jobId, 'Processing')  
**Sequence Number:** 6  
**Type:** Database Call  
**Is Synchronous:** True  
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** EF Core
    - **Method:** FindAsync, SaveChangesAsync
    - **Parameters:** Updates the PrintJob entity status to 'Processing'.
    - **Authentication:** Database connection string credentials.
    - **Error Handling:** Transient DB errors will trigger a message requeue (nack).
    - **Performance:** DB update should complete in < 50ms.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-11-INF  
**Message:** GetDicomFilesForPrinting(imageUids)  
**Sequence Number:** 7  
**Type:** Method Call  
**Is Synchronous:** True  
**Has Return:** True  
**Is Activation:** True  
**Return Message:** List<DicomFile>  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** GetDicomFilesForPrinting
    - **Parameters:** Array of SOP Instance UIDs to be printed.
    - **Authentication:** N/A
    - **Error Handling:** Throws FileNotFoundException if files are missing, triggering DLQ process.
    - **Performance:** Depends on storage speed and number of images.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-11-INF  
**Message:** GeneratePrintDocument(dicomFiles, layout)  
**Sequence Number:** 8  
**Type:** Method Call  
**Is Synchronous:** True  
**Has Return:** True  
**Is Activation:** True  
**Return Message:** PrintDocument object  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** GeneratePrintDocument
    - **Parameters:** DICOM data and layout configuration.
    - **Authentication:** N/A
    - **Error Handling:** Exceptions during PDF/GDI+ rendering will trigger DLQ process.
    - **Performance:** Can be long-running (>5s) depending on complexity and image count.
    
- **Source Id:** REPO-11-INF  
**Target Id:** EXT-OS  
**Message:** SpoolJobToPrinter(printDocument)  
**Sequence Number:** 9  
**Type:** API Call  
**Is Synchronous:** True  
**Has Return:** True  
**Is Activation:** False  
**Return Message:** Success/Failure  
**Technical Details:**
    
    - **Protocol:** Windows Print API
    - **Method:** PrintDocument.Print()
    - **Parameters:** The generated GDI+ document object.
    - **Authentication:** Operating system user context.
    - **Error Handling:** Catches printer errors (e.g., Offline, Out of Paper) and throws specific exceptions.
    - **Performance:** Variable, depends on driver and printer hardware.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-01-SHK  
**Message:** UpdatePrintJobStatusAsync(jobId, 'Completed')  
**Sequence Number:** 10  
**Type:** Database Call  
**Is Synchronous:** True  
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** EF Core
    - **Method:** FindAsync, SaveChangesAsync
    - **Parameters:** Updates the PrintJob entity status to 'Completed'.
    - **Authentication:** Database connection string credentials.
    - **Error Handling:** Failure here is logged but the message is still ack'd as the print job was sent.
    - **Performance:** DB update should complete in < 50ms.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** EXT-RABBITMQ  
**Message:** Acknowledge Message (BasicAck)  
**Sequence Number:** 11  
**Type:** Message Ack  
**Is Synchronous:** False  
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** AMQP
    - **Method:** BasicAck
    - **Parameters:** deliveryTag
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** Immediate.
    

### 2.6. Notes

- **Content:** Message Queue Configuration (REQ-TEC-002):
- Exchange Type: Direct
- Queue: Durable
- Messages: Persistent
- DLX/DLQ: Enabled for error handling.  
**Position:** top-right  
**Participant Id:** EXT-RABBITMQ  
**Sequence Number:** 3  
- **Content:** Error Handling Flow:
If any step from 6-9 fails after retries, the service sends a Negative Acknowledgment (BasicNack) with requeue=false. The message is then routed to the Dead Letter Queue for manual inspection and an alert is triggered. The job status in the DB is updated to 'Failed'.  
**Position:** bottom-left  
**Participant Id:** REPO-02-SVC  
**Sequence Number:** 5  
- **Content:** Idempotency:
The consumer should be idempotent. Before starting processing (step 6), it could check the job status in the database. If the status is not 'Queued', it assumes the job is a duplicate or already processed and acknowledges the message without taking further action.  
**Position:** middle-right  
**Participant Id:** REPO-02-SVC  
**Sequence Number:** 6  
- **Content:** Correlation & Tracing (REQ-REP-002):
A unique Correlation ID is generated in step 1 by REPO-08-APC. It is stored with the PrintJob record in the DB (step 2), passed as a header in the RabbitMQ message (step 3), and used in all structured logs by REPO-02-SVC and its dependencies for end-to-end tracing.  
**Position:** top-left  
**Participant Id:** REPO-08-APC  
**Sequence Number:** 1  

### 2.7. Implementation Guidance

- **Security Requirements:** RabbitMQ and PostgreSQL connections must use credentials stored securely via the Windows Credential Manager (REQ-NFR-004). All network connections must be over TLS. The client has no direct access to the database; all actions are mediated by the service via the message queue.
- **Performance Targets:** The client-side submission (steps 1-4) must complete in under 500ms to ensure UI responsiveness. The background service's processing time is variable but should be monitored. High queue depth should trigger a warning alert.
- **Error Handling Strategy:** The primary strategy is retry for transient errors followed by dead-lettering for persistent failures. A robust in-consumer retry policy (e.g., using Polly) should be implemented before a message is nack'd. The DLQ must be monitored, and any message landing there requires immediate administrative attention.
- **Testing Considerations:** Unit test the message serialization/deserialization. Integration test the full flow using a test RabbitMQ instance, mocking the Windows Print API. Test failure scenarios by simulating DB connection loss, missing files, and printer-offline errors to ensure messages are correctly routed to the DLQ.
- **Monitoring Requirements:** The System Health Dashboard must monitor the RabbitMQ connection status and the depth of the 'print_job_queue' and its corresponding DLQ. Log entries must include the Correlation ID. The duration of the print job processing (from message receipt to ack) should be measured and logged as a performance metric.
- **Deployment Considerations:** RabbitMQ must be deployed as a prerequisite service. The queue, exchange, and DLX/DLQ bindings should be declared and configured on service startup to ensure they exist and are correctly configured. The Windows Service must be configured for automatic restart on failure.


---

