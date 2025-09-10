# Architecture Design Specification

# 1. Patterns

## 1.1. Message Queue
### 1.1.2. Type
MessageQueue

### 1.1.3. Implementation
Asynchronous messaging via RabbitMQ, configured with durable queues and persistent messages.

### 1.1.4. Applicability
Essential for decoupling the WPF client from the Windows Service for all asynchronous and long-running tasks. It is used to submit print jobs (REQ-1-021), offload PDF generation (REQ-1-003), and critically, to buffer incoming DICOM study metadata from the high-throughput C-STORE SCP for reliable, sequential database insertion (REQ-1-010). This pattern ensures UI responsiveness and system resilience.

## 1.2. Dead Letter Channel
### 1.2.2. Type
ErrorHandling

### 1.2.3. Implementation
A dedicated Dead-Letter Exchange (DLX) and Dead-Letter Queue (DLQ) configured in RabbitMQ.

### 1.2.4. Applicability
Mandated by REQ-1-006 as the primary error handling strategy for messaging. Messages that fail processing a configurable number of times are automatically routed to the DLQ. This prevents message loss, isolates problematic tasks for manual inspection, and ensures the main processing queues are not blocked by poison messages.

## 1.3. Remote Procedure Invocation (RPI)
### 1.3.2. Type
RequestReply

### 1.3.3. Implementation
Synchronous, direct Inter-Process Communication (IPC) using .NET Named Pipes on the local machine.

### 1.3.4. Applicability
Used exclusively for the specific requirement of performing real-time, synchronous status checks between the WPF client and the Windows Service (REQ-1-007, REQ-1-021). The client makes a direct, blocking call to the service to verify its operational status before attempting to queue an asynchronous task like printing.

## 1.4. Request-Reply
### 1.4.2. Type
RequestReply

### 1.4.3. Implementation
Synchronous communication with an external web service via a REST API over HTTPS (TLS 1.2 minimum).

### 1.4.4. Applicability
Necessary for the license validation workflow as defined in REQ-1-011 and REQ-1-073. The client application sends a request to the Odoo Web Portal's REST API and waits for a direct response to validate the system's license status. This is a classic client-server external integration.

## 1.5. File Transfer
### 1.5.2. Type
DataIntegration

### 1.5.3. Implementation
Writing and reading DICOM files to/from a shared, configurable file system location (local or UNC path).

### 1.5.4. Applicability
The fundamental pattern for managing the core DICOM data. The database stores study metadata, but the large binary image data is integrated by writing files to a defined storage path (REQ-1-056) and storing a reference (filePath) in the database. All subsequent operations, like viewing or printing, use this path to access the DICOM files.



---

