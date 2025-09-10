# Specification

# 1. Event Driven Architecture Analysis

- **System Overview:**
  
  - **Analysis Date:** 2025-06-13
  - **Architecture Type:** Decoupled Client-Server with Message Queue
  - **Technology Stack:**
    
    - RabbitMQ
    - .NET 8
    - C# 12
    
  - **Bounded Contexts:**
    
    - DICOM Management and Printing
    
  
- **Project Specific Events:**
  
  - **Event Id:** CMD-001  
**Event Name:** SubmitPrintJobCommand  
**Event Type:** command  
**Category:** Printing  
**Description:** A command initiated by the WPF client to request the background service to process and spool a print job. Carries all necessary data like selected images, layout, printer settings, and overlays.  
**Trigger Condition:** User clicks the 'Print' button in the Print Preview UI.  
**Source Context:** Presentation Layer (WPF Client)  
**Target Contexts:**
    
    - Application Services Layer (Windows Service)
    
**Payload:**
    
    - **Schema:**
      
      - **Print Job Id:** Guid
      - **Submitted By User Id:** Guid
      - **Printer Name:** string
      - **Layout Definition:** object
      - **Image References:** array
      - **Print Settings:** object
      
    - **Required Fields:**
      
      - PrintJobId
      - SubmittedByUserId
      - PrinterName
      - ImageReferences
      
    - **Optional Fields:**
      
      - LayoutDefinition
      - PrintSettings
      
    
**Frequency:** medium  
**Business Criticality:** critical  
**Data Source:**
    
    - **Database:** PostgreSQL
    - **Table:** PrintJob
    - **Operation:** create
    
**Routing:**
    
    - **Routing Key:** print.job.submit
    - **Exchange:** dicom_system_exchange
    - **Queue:** print_job_queue
    
**Consumers:**
    
    - **Service:** Windows Service  
**Handler:** PrintJobConsumerService  
**Processing Type:** async  
    
**Dependencies:**
    
    - REQ-1-003
    - REQ-1-004
    - REQ-1-021
    
**Error Handling:**
    
    - **Retry Strategy:** Exponential backoff for transient errors.
    - **Dead Letter Queue:** dicom_system_dlq
    - **Timeout Ms:** 300000
    
  - **Event Id:** CMD-002  
**Event Name:** ProcessDicomStoreCommand  
**Event Type:** command  
**Category:** Data Ingestion  
**Description:** A command initiated internally by the C-STORE SCP component within the background service to decouple high-throughput DICOM reception from slower database write operations.  
**Trigger Condition:** A DICOM study is successfully received by the C-STORE SCP listener.  
**Source Context:** Application Services Layer (Windows Service - SCP)  
**Target Contexts:**
    
    - Application Services Layer (Windows Service - DB Writer)
    
**Payload:**
    
    - **Schema:**
      
      - **Staged File Paths:** array
      - **Extracted Metadata:** object
      - **Source Aetitle:** string
      
    - **Required Fields:**
      
      - StagedFilePaths
      - ExtractedMetadata
      
    - **Optional Fields:**
      
      - SourceAETitle
      
    
**Frequency:** high  
**Business Criticality:** critical  
**Data Source:**
    
    - **Database:** PostgreSQL
    - **Table:** Patient, Study, Series, Image
    - **Operation:** create
    
**Routing:**
    
    - **Routing Key:** dicom.store.process
    - **Exchange:** dicom_system_exchange
    - **Queue:** dicom_store_queue
    
**Consumers:**
    
    - **Service:** Windows Service  
**Handler:** DicomStoreConsumerService  
**Processing Type:** async  
    
**Dependencies:**
    
    - REQ-1-010
    - REQ-1-003
    
**Error Handling:**
    
    - **Retry Strategy:** Exponential backoff for transient DB connection issues.
    - **Dead Letter Queue:** dicom_system_dlq
    - **Timeout Ms:** 60000
    
  
- **Event Types And Schema Design:**
  
  - **Essential Event Types:**
    
    - **Event Name:** SubmitPrintJobCommand  
**Category:** integration  
**Description:** Represents a user's request to print DICOM images.  
**Priority:** high  
    - **Event Name:** ProcessDicomStoreCommand  
**Category:** integration  
**Description:** Represents a received DICOM study that needs to be persisted to the database.  
**Priority:** high  
    - **Event Name:** GeneratePdfCommand  
**Category:** integration  
**Description:** Represents a user's request to export a view to a PDF document.  
**Priority:** medium  
    
  - **Schema Design:**
    
    - **Format:** JSON
    - **Reasoning:** Standard for .NET applications, easy serialization/deserialization with System.Text.Json, and human-readable for debugging in RabbitMQ Management UI. Avoids adding external dependencies like Protobuf or Avro which are not required by the project's scale.
    - **Consistency Approach:** Use shared Data Transfer Object (DTO) classes in a common .NET library referenced by both the WPF client and the Windows Service projects.
    
  - **Schema Evolution:**
    
    - **Backward Compatibility:** True
    - **Forward Compatibility:** False
    - **Strategy:** Tolerant Reader pattern. Consumers must ignore any unrecognized fields. New fields must be optional and non-breaking. This avoids the need for a complex versioning strategy or schema registry.
    
  - **Event Structure:**
    
    - **Standard Fields:**
      
      - CorrelationId
      - Timestamp
      - MessageVersion
      
    - **Metadata Requirements:** The CorrelationId (REQ-1-090) must be passed as a message header/property to facilitate tracing without polluting the payload.
    
  
- **Event Routing And Processing:**
  
  - **Routing Mechanisms:**
    
    - **Type:** Direct Exchange with dedicated Queues  
**Description:** A single direct exchange is used. Each command type is routed to its own dedicated, durable queue via a unique routing key. This ensures isolation between different types of tasks.  
**Use Case:** Simple and reliable command routing where each message has a single, specific destination. Ideal for the defined print and data ingestion workflows.  
    
  - **Processing Patterns:**
    
    - **Pattern:** sequential  
**Applicable Scenarios:**
    
    - Processing database writes from C-STORE to maintain data consistency.
    - Processing print jobs to maintain a predictable order.
    
**Implementation:** A single consumer instance per queue within the Windows Service. This naturally enforces sequential processing of messages from that queue.  
    
  - **Filtering And Subscription:**
    
    - **Filtering Mechanism:** Routing Key Matching
    - **Subscription Model:** Competing Consumer (with a single consumer)
    - **Routing Keys:**
      
      - print.job.submit
      - dicom.store.process
      - pdf.generation.request
      
    
  - **Handler Isolation:**
    
    - **Required:** False
    - **Approach:** Handlers are implemented as separate IHostedService instances within the single, consolidated Windows Service process. This provides logical isolation without the overhead of multiple processes.
    - **Reasoning:** REQ-1-001 specifies a single, consolidated background service. Physical process isolation is not required and would add unnecessary complexity.
    
  - **Delivery Guarantees:**
    
    - **Level:** at-least-once
    - **Justification:** This is critical to ensure that print jobs and database writes are not lost in case of a service crash or broker restart, as mandated by REQ-1-005.
    - **Implementation:** Use of durable queues and persistent messages (REQ-1-005). The consumer must use manual message acknowledgment, only sending the ACK after the work is successfully completed and persisted.
    
  
- **Event Storage And Replay:**
  
  - **Persistence Requirements:**
    
    - **Required:** True
    - **Duration:** Until message is successfully processed or dead-lettered
    - **Reasoning:** REQ-1-005 explicitly requires that messages for critical data processing survive a message broker restart.
    
  - **Event Sourcing:**
    
    - **Necessary:** False
    - **Justification:** The system architecture is based on storing the current state in a PostgreSQL database. The audit log (REQ-1-047) fulfills the need for historical tracking of changes. Event sourcing would be an unnecessary complexity.
    - **Scope:**
      
      
    
  - **Technology Options:**
    
    
  - **Replay Capabilities:**
    
    - **Required:** False
    - **Scenarios:**
      
      - The only 'replay' required is the manual re-queueing of a failed message from the Dead Letter Queue after the root cause of the failure has been addressed.
      
    - **Implementation:** Manual intervention via the RabbitMQ Management UI.
    
  - **Retention Policy:**
    
    - **Strategy:** No retention after acknowledgment
    - **Duration:** N/A
    - **Archiving Approach:** No event archiving is required. The PostgreSQL database is the system of record.
    
  
- **Dead Letter Queue And Error Handling:**
  
  - **Dead Letter Strategy:**
    
    - **Approach:** Use RabbitMQ's built-in Dead Letter Exchange (DLX) mechanism as explicitly required by REQ-1-006.
    - **Queue Configuration:** Each primary work queue will be configured with arguments to route expired or negatively-acknowledged messages to a single, shared dead-letter exchange, which in turn routes them to a unified dead-letter queue (DLQ).
    - **Processing Logic:** Messages in the DLQ are for manual inspection only. An administrator will be alerted (REQ-1-092) and must use the RabbitMQ Management UI to inspect, purge, or republish messages.
    
  - **Retry Policies:**
    
    - **Error Type:** Transient errors (e.g., temporary database unavailability, network glitches)  
**Max Retries:** 3  
**Backoff Strategy:** exponential  
**Delay Configuration:** An in-memory retry mechanism within the consumer (e.g., using Polly) will attempt reprocessing before rejecting the message. This avoids complex delayed-message exchanges in RabbitMQ.  
    
  - **Poison Message Handling:**
    
    - **Detection Mechanism:** A message is considered a poison message after the configured number of retries fails due to a persistent error (e.g., malformed JSON, invalid data).
    - **Handling Strategy:** The consumer rejects the message with `requeue=false`, which triggers routing to the configured DLQ.
    - **Alerting Required:** True
    
  - **Error Notification:**
    
    - **Channels:**
      
      - Email
      - System Health Dashboard
      
    - **Severity:** critical
    - **Recipients:**
      
      - System Administrator
      
    
  - **Recovery Procedures:**
    
    - **Scenario:** A message is present in the Dead Letter Queue.  
**Procedure:** 1. Admin is alerted. 2. Admin uses RabbitMQ Management UI to inspect message payload and headers. 3. Admin investigates the root cause of the failure using logs (traced via Correlation ID). 4. After fixing the underlying issue, Admin manually republishes the message to the original exchange.  
**Automation Level:** manual  
    
  
- **Event Versioning Strategy:**
  
  - **Schema Evolution Approach:**
    
    - **Strategy:** Additive changes and Tolerant Reader. Consumers must not fail if they encounter unknown fields in a message payload.
    - **Versioning Scheme:** A simple integer or semantic version number in the message payload (e.g., `"messageVersion": "1.1"`).
    - **Migration Strategy:** Not applicable due to the simplicity of the approach. Consumers will be updated to handle new fields as needed.
    
  - **Compatibility Requirements:**
    
    - **Backward Compatible:** True
    - **Forward Compatible:** False
    - **Reasoning:** The consumer (service) must always be able to process messages created by an older client. Forward compatibility is not a strict requirement for this tightly coupled client-server system.
    
  - **Version Identification:**
    
    - **Mechanism:** Property in message payload
    - **Location:** payload
    - **Format:** string (e.g., '1.0')
    
  - **Consumer Upgrade Strategy:**
    
    - **Approach:** Deploy the updated consumer (Windows Service) first, then the producer (WPF Client). The durable queues will buffer any messages sent during the brief service restart.
    - **Rollout Strategy:** Blue-Green or Canary deployments are not applicable for this desktop/single-service architecture. A simple stop-and-replace deployment is sufficient.
    - **Rollback Procedure:** Redeploy the previous version of the Windows Service.
    
  - **Schema Registry:**
    
    - **Required:** False
    - **Technology:** N/A
    - **Governance:** Message contracts (DTOs) are managed in a shared .NET library, providing compile-time checking and avoiding the need for a runtime schema registry.
    
  
- **Event Monitoring And Observability:**
  
  - **Monitoring Capabilities:**
    
    - **Capability:** Queue Depth Monitoring  
**Justification:** Required by REQ-1-091 to be displayed on the System Health Dashboard. It indicates if the system is keeping up with the workload.  
**Implementation:** Periodically query the RabbitMQ Management API from the background service.  
    - **Capability:** Dead Letter Queue Message Count  
**Justification:** Required by REQ-1-091. A non-zero count is a critical indicator of processing failures.  
**Implementation:** Periodically query the RabbitMQ Management API from the background service.  
    
  - **Tracing And Correlation:**
    
    - **Tracing Required:** True
    - **Correlation Strategy:** A unique Correlation ID is generated by the producer (WPF Client or SCP Service) for each operation.
    - **Trace Id Propagation:** The Correlation ID is passed as a message property/header and included in all structured log entries related to that operation, as mandated by REQ-1-090.
    
  - **Performance Metrics:**
    
    - **Metric:** Message Processing Latency (time from enqueue to ACK)  
**Threshold:** For internal monitoring and performance tuning, not for alerting.  
**Alerting:** False  
    - **Metric:** Consumer Throughput (messages/sec)  
**Threshold:** For internal monitoring and performance tuning, not for alerting.  
**Alerting:** False  
    
  - **Event Flow Visualization:**
    
    - **Required:** False
    - **Tooling:** The built-in RabbitMQ Management UI is sufficient for visualizing exchanges, queues, and message flow for this system's level of complexity.
    - **Scope:** N/A
    
  - **Alerting Requirements:**
    
    - **Condition:** Message count in DLQ is greater than 0.  
**Severity:** critical  
**Response Time:** Immediate email notification  
**Escalation Path:**
    
    - System Administrator
    
    - **Condition:** Main work queue depth exceeds a configurable threshold (e.g., 1000 messages).  
**Severity:** warning  
**Response Time:** Immediate email notification  
**Escalation Path:**
    
    - System Administrator
    
    
  
- **Implementation Priority:**
  
  - **Component:** Core Messaging Infrastructure (Exchanges, Queues, Producer/Consumer)  
**Priority:** high  
**Dependencies:**
    
    
**Estimated Effort:** Medium  
  - **Component:** Dead Letter Queue and Basic Retry Logic  
**Priority:** high  
**Dependencies:**
    
    - Core Messaging Infrastructure
    
**Estimated Effort:** Low  
  - **Component:** Correlation ID Propagation and Structured Logging  
**Priority:** medium  
**Dependencies:**
    
    - Core Messaging Infrastructure
    
**Estimated Effort:** Low  
  - **Component:** Dashboard Monitoring Integration  
**Priority:** medium  
**Dependencies:**
    
    - Core Messaging Infrastructure
    
**Estimated Effort:** Low  
  
- **Risk Assessment:**
  
  - **Risk:** RabbitMQ server unavailability causes the entire asynchronous workflow to halt.  
**Impact:** high  
**Probability:** low  
**Mitigation:** The Windows Service must implement robust, exponential-backoff reconnection logic. The System Health Dashboard (REQ-1-091) will make the disconnected state visible to administrators.  
  - **Risk:** A persistent poison message blocks a critical queue (e.g., dicom_store_queue), preventing all new studies from being processed.  
**Impact:** high  
**Probability:** medium  
**Mitigation:** The DLQ strategy (REQ-1-006) is the primary mitigation. It moves the blocking message out of the main queue, allowing other messages to be processed. Alerting on DLQ'd messages (REQ-1-092) ensures timely manual intervention.  
  - **Risk:** Message payload schema changes in a new client version, breaking the consumer.  
**Impact:** medium  
**Probability:** medium  
**Mitigation:** Adherence to the 'Tolerant Reader' pattern and making all new schema fields optional. Contracts are managed in a shared library, reducing the risk of mismatches.  
  
- **Recommendations:**
  
  - **Category:** Reliability  
**Recommendation:** Implement idempotent consumers, especially for database write operations. A message might be redelivered due to network issues or service restarts before an ACK is sent.  
**Justification:** This prevents duplicate data insertion. The consumer should check if the record (e.g., based on SOPInstanceUID) already exists before attempting to insert it.  
**Priority:** high  
  - **Category:** Configuration  
**Recommendation:** Externalize all RabbitMQ connection details and queue names into the application's configuration, managed via the Settings UI where appropriate.  
**Justification:** Avoids hard-coding values, making the application easier to deploy and manage in different environments (dev, test, prod).  
**Priority:** high  
  - **Category:** Simplicity  
**Recommendation:** Resist adding more complex messaging patterns like Topic Exchanges, Fanout, or Sagas unless a future requirement explicitly demands them.  
**Justification:** The current requirements are fully satisfied by a simple direct-exchange-to-queue command pattern. Adding complexity now would violate YAGNI ('You Ain't Gonna Need It') and increase maintenance overhead.  
**Priority:** high  
  


---

