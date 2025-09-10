# Specification

# 1. Overview
## 1. Poison Message Handling via Dead-Letter Queue
An asynchronous message in a RabbitMQ queue consistently fails processing in the background service. After a configured number of retries, the consumer rejects the message, which is then automatically routed by RabbitMQ to a Dead-Letter Queue (DLQ). A monitoring process detects messages in the DLQ and triggers an alert containing the message's Correlation ID for an administrator to investigate.

### 1.1. Diagram Id
SEQ-ERH-007

### 1.4. Type
ErrorHandling

### 1.5. Purpose
To prevent a single failing message from blocking a processing queue and to preserve the failed message for analysis and potential manual reprocessing, ensuring system reliability.

### 1.6. Complexity
Medium

### 1.7. Priority
Critical

### 1.8. Frequency
OnDemand

### 1.9. Participants

- REPO-02-SVC
- EXT-RABBITMQ

### 1.10. Key Interactions

- Message consumer fails to process a message due to a persistent error.
- An in-process retry mechanism attempts reprocessing N times.
- After the final attempt, the consumer rejects the message (requeue=false).
- RabbitMQ broker routes the rejected message to the configured Dead-Letter Exchange (DLX).
- The message is placed in the Dead-Letter Queue (DLQ).
- The System Health Probe detects a non-zero DLQ count and triggers an alert with the message's Correlation ID.

### 1.11. Triggers

- A message fails processing after exhausting all configured retries (REQ-NFR-005).

### 1.12. Outcomes

- The failing message is isolated from the main processing queue.
- The main queue is unblocked and can continue processing other messages.
- An administrator is alerted to the failure for manual intervention.

### 1.13. Business Rules

- Message queue architecture must include a DLQ mechanism (REQ-TEC-002).

### 1.14. Error Scenarios

- This is the error handling sequence itself.

### 1.15. Integration Points

- RabbitMQ Broker


---

# 2. Details
## 2. Poison Message Handling and Alerting via RabbitMQ Dead-Letter Queue
This sequence details the technical implementation of handling a non-transient, persistent processing failure of an asynchronous message. It follows the Retry and Dead Letter Channel patterns to ensure system reliability by preventing queue blockage. After exhausting a configured number of retries, the message is rejected and routed to a Dead-Letter Queue (DLQ). A separate monitoring component detects the message in the DLQ and triggers a critical alert for manual administrative intervention.

### 2.1. Diagram Id
SEQ-ERH-007

### 2.4. Participants

- **Repository Id:** REPO-02-SVC  
**Display Name:** DMPS.Service.Application  
**Type:** Windows Service  
**Technology:** .NET 8, RabbitMQ.Client, Polly  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #1168BD
    - **Stereotype:** Message Consumer & Health Probe
    
- **Repository Id:** EXT-RABBITMQ  
**Display Name:** RabbitMQ Broker  
**Type:** Message Broker  
**Technology:** RabbitMQ Server 3.12+  
**Order:** 2  
**Style:**
    
    - **Shape:** database
    - **Color:** #FF6600
    - **Stereotype:** External Dependency
    
- **Repository Id:** EXT-ALERTING  
**Display Name:** Alerting System  
**Type:** Notification Service  
**Technology:** SMTP / Webhook Endpoint  
**Order:** 3  
**Style:**
    
    - **Shape:** actor
    - **Color:** #D32F2F
    - **Stereotype:** External System
    

### 2.5. Interactions

- **Source Id:** EXT-RABBITMQ  
**Target Id:** REPO-02-SVC  
**Message:** [AMQP] basic.deliver(message)  
**Sequence Number:** 1  
**Type:** MessageDelivery  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** AMQP 0-9-1
    - **Method:** basic.deliver
    - **Parameters:** Message payload, deliveryTag, properties (including Correlation ID)
    - **Authentication:** Connection-level credentials.
    - **Error Handling:** The consumer (DMPS.Service.Application) is responsible for acknowledging or rejecting the message.
    - **Performance:** High-throughput, low-latency delivery.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-02-SVC  
**Message:** [LOOP Start: 3 retries] Attempt to process message with Correlation ID: {id}  
**Sequence Number:** 2  
**Type:** InternalProcessing  
**Is Synchronous:** True  
**Return Message:** Processing result (success/failure)  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** MessageConsumer.ProcessMessageAsync(message)
    - **Parameters:** Deserialized message DTO
    - **Authentication:** N/A
    - **Error Handling:** A persistent exception (e.g., DbUpdateException, JsonException) is caught. The Polly retry policy is triggered.
    - **Performance:** Execution time varies by job type. Must be within expected processing limits.
    
**Nested Interactions:**
    
    - **Source Id:** REPO-02-SVC  
**Target Id:** REPO-02-SVC  
**Message:** [Internal] Log processing failure: Exception {ex}, CorrelationID: {id}  
**Sequence Number:** 2.1  
**Type:** Logging  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Serilog
    - **Method:** ILogger.Error()
    - **Parameters:** Exception details, message context, and Correlation ID.
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** Minimal overhead.
    
    
- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-02-SVC  
**Message:** [LOOP End] Final retry attempt failed. Preparing to reject message.  
**Sequence Number:** 3  
**Type:** InternalStateChange  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** Polly.OnRetryFinalAttempt
    - **Parameters:** Final exception, context.
    - **Authentication:** N/A
    - **Error Handling:** The final state triggers the message rejection logic.
    - **Performance:** N/A
    
- **Source Id:** REPO-02-SVC  
**Target Id:** EXT-RABBITMQ  
**Message:** [AMQP] basic.reject(deliveryTag, requeue=false)  
**Sequence Number:** 4  
**Type:** MessageRejection  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** AMQP 0-9-1
    - **Method:** IModel.BasicReject
    - **Parameters:** deliveryTag from the original message, requeue flag set to false.
    - **Authentication:** N/A
    - **Error Handling:** Setting requeue=false is critical. If true, it would cause an infinite processing loop for a poison message.
    - **Performance:** Low-latency.
    
- **Source Id:** EXT-RABBITMQ  
**Target Id:** EXT-RABBITMQ  
**Message:** [Internal Broker Logic] Message rejected. Route to configured Dead-Letter Exchange (DLX).  
**Sequence Number:** 5  
**Type:** MessageRouting  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** AMQP Internal
    - **Method:** Dead-Lettering
    - **Parameters:** The original work queue must be configured with 'x-dead-letter-exchange' and 'x-dead-letter-routing-key' arguments.
    - **Authentication:** N/A
    - **Error Handling:** If the DLX does not exist or has no bound queue, the message will be dropped. Configuration must be validated.
    - **Performance:** High-performance broker operation.
    
- **Source Id:** EXT-RABBITMQ  
**Target Id:** EXT-RABBITMQ  
**Message:** [Internal Broker Logic] Message published to Dead-Letter Queue (DLQ) via DLX.  
**Sequence Number:** 6  
**Type:** Queueing  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** AMQP Internal
    - **Method:** Queueing
    - **Parameters:** The DLQ must be durable to prevent message loss on broker restart.
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** Standard queueing performance.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** EXT-RABBITMQ  
**Message:** [HTTP GET] Request queue stats for DLQ.  
**Sequence Number:** 7  
**Type:** MonitoringProbe  
**Is Synchronous:** True  
**Return Message:** [HTTP 200 OK] Queue statistics JSON  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** HTTP/1.1
    - **Method:** GET /api/queues/{vhost}/{dlq_name}
    - **Parameters:** Virtual host name, DLQ name.
    - **Authentication:** Basic Authentication with management user credentials.
    - **Error Handling:** Handle HTTP 4xx/5xx errors, network timeouts. A failed probe should also trigger an alert.
    - **Performance:** Polling interval should be configured (e.g., 60 seconds) to avoid excessive load on the management API.
    
- **Source Id:** EXT-RABBITMQ  
**Target Id:** REPO-02-SVC  
**Message:** [HTTP 200 OK] Return stats with message_count > 0  
**Sequence Number:** 8  
**Type:** MonitoringResponse  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** HTTP/1.1
    - **Method:** Response
    - **Parameters:** JSON payload containing queue metrics, e.g., `{ 'messages': 1, 'messages_unacknowledged': 0, ... }`
    - **Authentication:** N/A
    - **Error Handling:** The Health Probe must parse the JSON and check the 'messages' field.
    - **Performance:** N/A
    
- **Source Id:** REPO-02-SVC  
**Target Id:** EXT-ALERTING  
**Message:** [SMTP/Webhook] Send critical alert: "Poison message detected in DLQ..."  
**Sequence Number:** 9  
**Type:** AlertNotification  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** SMTP or HTTPS
    - **Method:** SEND or POST
    - **Parameters:** Alert payload including: Timestamp, Severity: CRITICAL, Component: RabbitMQ, Message: '1 or more messages found in Dead-Letter Queue {dlq_name}. Manual intervention required.'
    - **Authentication:** SMTP credentials or API key for webhook.
    - **Error Handling:** Log any failures to send the alert.
    - **Performance:** Should be sent within seconds of detection.
    

### 2.6. Notes

- **Content:** This sequence is the implementation of REQ-TEC-002 and REQ-NFR-005. It is a critical reliability pattern.  
**Position:** top-left  
**Participant Id:** None  
**Sequence Number:** None  
- **Content:** The manual recovery process for a DLQ'd message is: 1. Inspect the message payload and headers in RabbitMQ UI. 2. Use the Correlation ID to trace logs and find the root cause. 3. Fix the underlying issue (e.g., deploy a code fix, correct bad data). 4. Manually republish the message from the DLQ to the original exchange for reprocessing.  
**Position:** bottom  
**Participant Id:** None  
**Sequence Number:** 9  

### 2.7. Implementation Guidance

- **Security Requirements:** Access to the RabbitMQ Management UI and the DLQ must be strictly controlled, as messages may contain sensitive PHI. Alert messages must NOT contain raw message payloads or PHI.
- **Performance Targets:** Recovery Time Objective (RTO) for this failure is dependent on administrator response time to the alert. The system's automated actions (rejection, routing, probing) should complete in under 1 second.
- **Error Handling Strategy:** This sequence defines the error handling strategy for persistent message processing failures. The strategy prioritizes system availability (by unblocking the main queue) and data preservation (by saving the message in the DLQ) over automated recovery.
- **Testing Considerations:** An integration test must be created to simulate a poison message. It should: 1. Publish a malformed message. 2. Verify the consumer fails and rejects it. 3. Poll the RabbitMQ Management API to confirm the message count in the DLQ is 1. 4. Verify that an alert was triggered (e.g., by checking a mock email service).
- **Monitoring Requirements:** The System Health Dashboard (REQ-REP-002) must prominently display the message count of the DLQ. A value greater than zero should be displayed in a critical/error state (e.g., red color). This metric should be a primary indicator for system health.
- **Deployment Considerations:** The RabbitMQ broker must be provisioned with the correct DLX, DLQ, and queue arguments during initial environment setup. This can be automated using Infrastructure as Code (e.g., Ansible, Terraform) or a startup configuration script.


---

