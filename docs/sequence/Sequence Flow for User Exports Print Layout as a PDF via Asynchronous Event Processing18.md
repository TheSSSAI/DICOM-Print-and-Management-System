# Specification

# 1. Overview
## 1. User Exports Print Layout as a PDF
A user in the Print Preview UI chooses to export the current layout as a PDF. The client application gathers the layout details, serializes them into a GeneratePdfCommand, and publishes it to a RabbitMQ queue. The background service consumes this message, uses the QuestPDF library to generate a PDF/A-compliant file, and saves it to a location specified by the user.

### 1.1. Diagram Id
SEQ-EVP-018

### 1.4. Type
EventProcessing

### 1.5. Purpose
To provide a non-blocking way for users to create and save a digital, portable version of their print output.

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

### 1.10. Key Interactions

- User clicks 'Export to PDF' in the Print Preview UI and selects a file path.
- Client application publishes a GeneratePdfCommand to RabbitMQ.
- Background service's PDF Job Consumer receives the message.
- Service retrieves DICOM images from storage.
- Service uses QuestPDF to generate the PDF document in memory.
- Service saves the generated PDF to the specified file path.

### 1.11. Triggers

- User initiates the 'Export to PDF' action (FR-3.1.4).

### 1.12. Outcomes

- A PDF/A-3 compliant file matching the print preview is created and saved.
- The user is notified that the export job has started and when it is complete.

### 1.13. Business Rules

- The generated PDF shall be PDF/A-3 compliant (FR-3.1.4).
- PDF generation must be processed asynchronously by the background service (REQ-TEC-002).

### 1.14. Error Scenarios

- The target save location is not writable.
- An error occurs during PDF generation.
- The message is sent to the DLQ after failed retries.

### 1.15. Integration Points

- RabbitMQ Broker
- Local File System
- QuestPDF Library


---

# 2. Details
## 2. User Exports Print Layout as a PDF via Asynchronous Event Processing
This sequence details the asynchronous, event-driven process for exporting a DICOM print layout to a PDF. The WPF client initiates the process by publishing a 'GeneratePdfCommand' to a RabbitMQ message queue. A background service consumer processes this command, utilizing the infrastructure layer to retrieve DICOM data, generate a PDF/A-compliant document using QuestPDF, and save it to the user-specified file path. The design emphasizes reliability through persistent messaging and robust error handling via a Dead Letter Queue.

### 2.1. Diagram Id
SEQ-EVP-018

### 2.4. Participants

- **Repository Id:** USER  
**Display Name:** User (Admin/Technician)  
**Type:** Actor  
**Technology:** Human  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #E6E6E6
    - **Stereotype:** Human Actor
    
- **Repository Id:** REPO-09-PRE  
**Display Name:** Presentation Layer  
**Type:** UI Layer  
**Technology:** WPF, MVVM  
**Order:** 2  
**Style:**
    
    - **Shape:** boundary
    - **Color:** #B3DAFF
    - **Stereotype:** DMPS.Client.Presentation
    
- **Repository Id:** REPO-08-APC  
**Display Name:** Client Application Service  
**Type:** Application Service  
**Technology:** .NET 8  
**Order:** 3  
**Style:**
    
    - **Shape:** control
    - **Color:** #8AC9FF
    - **Stereotype:** DMPS.Client.Application
    
- **Repository Id:** EXT-RABBITMQ  
**Display Name:** RabbitMQ Broker  
**Type:** Message Broker  
**Technology:** RabbitMQ  
**Order:** 4  
**Style:**
    
    - **Shape:** queue
    - **Color:** #FFC300
    - **Stereotype:** External Dependency
    
- **Repository Id:** REPO-02-SVC  
**Display Name:** Background Service (PDF Consumer)  
**Type:** Background Service  
**Technology:** .NET 8 IHostedService  
**Order:** 5  
**Style:**
    
    - **Shape:** control
    - **Color:** #44A2FF
    - **Stereotype:** DMPS.Service.Application
    
- **Repository Id:** REPO-11-INF  
**Display Name:** Infrastructure Layer  
**Type:** Infrastructure  
**Technology:** QuestPDF, .NET File I/O  
**Order:** 6  
**Style:**
    
    - **Shape:** component
    - **Color:** #65B4FF
    - **Stereotype:** DMPS.Infrastructure
    

### 2.5. Interactions

- **Source Id:** USER  
**Target Id:** REPO-09-PRE  
**Message:** 1. Click 'Export to PDF' and select file path via Save File Dialog  
**Sequence Number:** 1  
**Type:** User Interaction  
**Is Synchronous:** True  
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** WPF Events
    - **Method:** Button_Click
    - **Parameters:**
      
      - Event Arguments
      
    - **Authentication:** N/A
    - **Error Handling:** UI will handle cases where no path is selected.
    
- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-08-APC  
**Message:** 2. Invoke ExportToPdfAsync(viewModelData, outputPath)  
**Sequence Number:** 2  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** 3. Return Task<Result>  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Method Call
    - **Method:** IPrintJobService.ExportToPdfAsync
    - **Parameters:**
      
      - PrintLayoutViewModel viewModelData
      - string outputPath
      
    - **Authentication:** User session context is passed implicitly.
    - **Error Handling:** Exceptions are caught and translated into a Failure Result for the UI.
    
- **Source Id:** REPO-08-APC  
**Target Id:** EXT-RABBITMQ  
**Message:** 4. Publish GeneratePdfCommand message  
**Sequence Number:** 3  
**Type:** Message Publication  
**Is Synchronous:** True  
**Return Message:** 5. Acknowledge publish  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** AMQP
    - **Method:** basic.publish
    - **Parameters:**
      
      - Exchange: 'dicom_system_exchange'
      - Routing Key: 'pdf.generation.request'
      - Message Properties: Persistent=true, ContentType='application/json', CorrelationId=<new_guid>
      - Payload: Serialized GeneratePdfCommand DTO
      
    - **Authentication:** Authenticated connection to RabbitMQ.
    - **Error Handling:** Handle RabbitMQ connection failures with retry logic and user notification.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-09-PRE  
**Message:** 6. Display 'PDF export has started' notification  
**Sequence Number:** 4  
**Type:** UI Update  
**Is Synchronous:** False  
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Process Event/Callback
    - **Method:** NotificationService.ShowToast
    - **Parameters:**
      
      - Message: 'PDF export has started. You will be notified upon completion.'
      - Severity: 'Info'
      
    - **Authentication:** N/A
    - **Error Handling:** N/A
    
- **Source Id:** EXT-RABBITMQ  
**Target Id:** REPO-02-SVC  
**Message:** 7. Deliver GeneratePdfCommand message  
**Sequence Number:** 5  
**Type:** Message Delivery  
**Is Synchronous:** False  
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** AMQP
    - **Method:** basic.deliver
    - **Parameters:**
      
      - Queue: 'pdf_generation_queue'
      
    - **Authentication:** N/A
    - **Error Handling:** Consumer is subscribed to the queue; broker manages delivery.
    
**Nested Interactions:**
    
    - **Source Id:** REPO-02-SVC  
**Target Id:** REPO-11-INF  
**Message:** 8. Request DICOM files: RetrieveDicomFilesAsync(imageReferences)  
**Sequence Number:** 6  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** 9. Return DICOM data streams  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Method Call
    - **Method:** IDicomFileStorage.RetrieveDicomFilesAsync
    - **Parameters:**
      
      - List<string> imageReferences
      
    - **Authentication:** N/A
    - **Error Handling:** Throw FileNotFoundException if a DICOM file is missing from storage.
    
    - **Source Id:** REPO-02-SVC  
**Target Id:** REPO-11-INF  
**Message:** 10. Generate PDF: GeneratePdfAsync(layout, dicomData)  
**Sequence Number:** 7  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** 11. Return PDF byte array  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Method Call
    - **Method:** IPdfGenerator.GeneratePdfAsync
    - **Parameters:**
      
      - PrintLayoutDefinition layout
      - List<DicomData> dicomData
      
    - **Authentication:** N/A
    - **Error Handling:** Wraps and throws exceptions from the QuestPDF library on generation failure.
    
    - **Source Id:** REPO-02-SVC  
**Target Id:** REPO-11-INF  
**Message:** 12. Save PDF file: SaveFileAsync(pdfBytes, outputPath)  
**Sequence Number:** 8  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** 13. Return success  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Method Call
    - **Method:** IFileStore.SaveFileAsync
    - **Parameters:**
      
      - byte[] pdfBytes
      - string outputPath
      
    - **Authentication:** Service account must have write permissions to the target location.
    - **Error Handling:** Throw UnauthorizedAccessException or IOException on file system errors.
    
    - **Source Id:** REPO-02-SVC  
**Target Id:** EXT-RABBITMQ  
**Message:** 14. Acknowledge message (basic.ack)  
**Sequence Number:** 9  
**Type:** Message Acknowledgement  
**Is Synchronous:** False  
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** AMQP
    - **Method:** basic.ack
    - **Parameters:**
      
      - deliveryTag
      
    - **Authentication:** N/A
    - **Error Handling:** If ack fails, message may be redelivered. Consumer logic must be idempotent.
    
    

### 2.6. Notes

- **Content:** Error Handling: If any step within the consumer (8-13) fails, the service will catch the exception. After exhausting a configured retry policy for transient errors, it will send a negative acknowledgement (basic.nack with requeue=false) to RabbitMQ, which will route the message to the Dead Letter Queue (DLQ) for manual inspection and intervention.  
**Position:** bottom  
**Participant Id:** REPO-02-SVC  
**Sequence Number:** 5  
- **Content:** Idempotency: The PDF generation consumer should be designed to be idempotent. If a message is redelivered after successful processing but before the ACK is confirmed, the consumer should handle it gracefully (e.g., by overwriting the previously generated file or checking for its existence).  
**Position:** bottom  
**Participant Id:** REPO-02-SVC  
**Sequence Number:** 5  
- **Content:** User Notification on Completion/Failure: The sequence shows a 'Job Started' notification. A robust implementation requires a separate mechanism to notify the user of final completion or failure. This could be achieved by the service writing the job status to a database table that the client polls, or by using a real-time communication channel like SignalR.  
**Position:** bottom  
**Participant Id:** REPO-09-PRE  
**Sequence Number:** 4  
- **Content:** Security: The background service runs under a specific service account. This account MUST have appropriate read permissions for the DICOM storage location and write permissions for common user-accessible locations (e.g., 'My Documents'). The service MUST validate the 'outputPath' to prevent path traversal attacks and writing to sensitive system directories.  
**Position:** bottom  
**Participant Id:** REPO-02-SVC  
**Sequence Number:** 8  

### 2.7. Implementation Guidance

- **Security Requirements:** The service must sanitize and validate the user-provided 'outputPath' to prevent directory traversal vulnerabilities. The Windows Service account's file system permissions must be carefully configured to be as restrictive as possible while still allowing functionality.
- **Performance Targets:** The client-side message publication (steps 2-3) must complete in under 2 seconds to maintain UI responsiveness as per REQ-NFR-002. The PDF generation itself is handled asynchronously and its duration will depend on the complexity and size of the layout.
- **Error Handling Strategy:** A global Dead Letter Queue (DLQ) will be configured. When the PDF consumer fails after 3 retry attempts, the message is sent to the DLQ. The system health dashboard will monitor the DLQ depth, and an alert will be sent to an administrator if the count is > 0.
- **Testing Considerations:** Unit test the PDF consumer logic by mocking the file system and QuestPDF dependencies. Integration tests should verify the end-to-end flow using a real RabbitMQ instance, checking that messages are correctly queued, consumed, and dead-lettered on failure. Test with invalid/unwritable file paths.
- **Monitoring Requirements:** The depth of the 'pdf_generation_queue' and the 'dead_letter_queue' must be monitored on the System Health Dashboard as per REQ-REP-002. All processing steps in the consumer should be logged with the Correlation ID from the message header for end-to-end tracing.
- **Deployment Considerations:** RabbitMQ must be configured with a durable 'dicom_system_exchange' (direct type) and a durable 'pdf_generation_queue'. The queue must be bound to the exchange with the 'pdf.generation.request' routing key. DLX/DLQ settings must be applied to the main queue.


---

