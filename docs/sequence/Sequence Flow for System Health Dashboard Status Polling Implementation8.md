# Specification

# 1. Overview
## 1. System Health Dashboard Status Polling
An IHostedService within the Windows Service continuously polls the status of its key components and dependencies on a timed interval. It checks connectivity to PostgreSQL and RabbitMQ, measures queue depths, and verifies available disk space. This data is then made available via an internal cache for the Administration UI's health dashboard to consume.

### 1.1. Diagram Id
SEQ-OPF-008

### 1.4. Type
OperationalFlow

### 1.5. Purpose
To provide administrators with a real-time overview of the system's health, enabling proactive monitoring and faster troubleshooting.

### 1.6. Complexity
Medium

### 1.7. Priority
High

### 1.8. Frequency
Hourly

### 1.9. Participants

- REPO-02-SVC
- EXT-PGSQL
- EXT-RABBITMQ
- EXT-OS

### 1.10. Key Interactions

- A scheduled timer in the Health Probe Service fires (e.g., every 60 seconds).
- The service attempts to open and close a connection to PostgreSQL.
- The service verifies its connection to RabbitMQ is open.
- It queries the RabbitMQ Management API for queue depths (main and DLQ).
- It uses OS APIs to check the free space of the configured DICOM storage volume.
- The collected statuses are updated in a singleton service, ready for the UI to query.

### 1.11. Triggers

- Scheduled polling interval (e.g., every 60 seconds).

### 1.12. Outcomes

- The health status of all monitored components is refreshed.
- The System Health Dashboard in the Admin UI displays up-to-date information.

### 1.13. Business Rules

- The dashboard must display the status of the Windows Service, DB, MQ, queue depths, and disk space (REQ-REP-002).

### 1.14. Error Scenarios

- A dependency (e.g., PostgreSQL) is down, and its status is marked as 'Disconnected'.
- Disk space falls below a critical threshold, triggering an alert.

### 1.15. Integration Points

- PostgreSQL Database
- RabbitMQ Management API
- Local File System


---

# 2. Details
## 2. System Health Dashboard Status Polling Implementation
Provides a detailed technical implementation for the IHostedService within the DMPS Windows Service responsible for continuous system health monitoring. On a scheduled interval, this service polls critical dependencies including the PostgreSQL database, RabbitMQ broker, and local file system. The collected status information (connectivity, queue depths, disk space) is then updated in a singleton in-memory cache, making it available for consumption by the Administration UI's health dashboard, as mandated by REQ-REP-002.

### 2.1. Diagram Id
SEQ-OPF-008

### 2.4. Participants

- **Repository Id:** REPO-02-SVC  
**Display Name:** DMPS Windows Service (HealthProbeHostedService)  
**Type:** HostedService  
**Technology:** .NET 8, Microsoft.Extensions.Hosting  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #1168bd
    - **Stereotype:** <<HostedService>>
    
- **Repository Id:** EXT-PGSQL  
**Display Name:** PostgreSQL Database  
**Type:** Database  
**Technology:** PostgreSQL 16  
**Order:** 2  
**Style:**
    
    - **Shape:** database
    - **Color:** #228B22
    - **Stereotype:** <<External>>
    
- **Repository Id:** EXT-RABBITMQ  
**Display Name:** RabbitMQ Broker  
**Type:** MessageBroker  
**Technology:** RabbitMQ  
**Order:** 3  
**Style:**
    
    - **Shape:** queue
    - **Color:** #FF4500
    - **Stereotype:** <<External>>
    
- **Repository Id:** EXT-OS  
**Display Name:** Operating System  
**Type:** Platform  
**Technology:** Windows Server  
**Order:** 4  
**Style:**
    
    - **Shape:** component
    - **Color:** #6A5ACD
    - **Stereotype:** <<Platform>>
    

### 2.5. Interactions

- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-02-SVC  
**Message:** 1. [Timer Tick] Scheduled polling interval fires (e.g., every 60s).  
**Sequence Number:** 1  
**Type:** Self  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Timer
    - **Method:** PeriodicTimer.WaitForNextTickAsync(CancellationToken)
    - **Parameters:** Configurable polling interval (e.g., TimeSpan.FromSeconds(60))
    - **Authentication:** N/A
    - **Error Handling:** The main loop is wrapped in a try/catch block to log any unhandled exceptions in the polling process without crashing the service.
    - **Performance:** Low overhead. Timer fires on a background thread.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** EXT-PGSQL  
**Message:** 2. Check PostgreSQL Connectivity  
**Sequence Number:** 2  
**Type:** Synchronous  
**Is Synchronous:** True  
**Return Message:** 3. Connection Status  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** TCP/IP (Npgsql)
    - **Method:** DbConnection.OpenAsync() followed by DbConnection.CloseAsync()
    - **Parameters:** Connection String from secure configuration (Windows Credential Manager).
    - **Authentication:** Database user credentials from connection string.
    - **Error Handling:** Catch Npgsql.NpgsqlException. On failure, update status to 'Disconnected' with error details. Connection timeout should be configured to ~5 seconds.
    - **Performance:** Operation should complete within 5 seconds.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** EXT-RABBITMQ  
**Message:** 4. Check RabbitMQ Connectivity & Queue Depths  
**Sequence Number:** 4  
**Type:** Synchronous  
**Is Synchronous:** True  
**Return Message:** 5. Connection Status & Queue Metrics  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** HTTP/REST
    - **Method:** GET /api/queues/{vhost}/{queue_name}
    - **Parameters:** vhost, queue_name (main work queue), queue_name (DLQ). Requires HttpClient configured with BaseAddress for RabbitMQ Management API.
    - **Authentication:** Basic Authentication with credentials for a monitoring user, stored in Windows Credential Manager.
    - **Error Handling:** Catch HttpRequestException or handle non-200 status codes. On failure, update status to 'Disconnected'. If connection is OK but API fails, log warning. Parse JSON response for 'messages' and 'messages_ready' fields.
    - **Performance:** API call timeout should be configured to ~10 seconds.
    
**Nested Interactions:**
    
    - **Source Id:** REPO-02-SVC  
**Target Id:** REPO-02-SVC  
**Message:** 4.1 Internally verify IConnection.IsOpen property is true.  
**Sequence Number:** 4.1  
**Type:** Self  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Memory Check
    - **Method:** RabbitMQ.Client.IConnection.IsOpen
    - **Parameters:** N/A
    - **Authentication:** N/A
    - **Error Handling:** If false, update status to 'Disconnected' and skip the API call for queue depths.
    - **Performance:** Immediate.
    
    
- **Source Id:** REPO-02-SVC  
**Target Id:** EXT-OS  
**Message:** 6. Check DICOM Storage Disk Space  
**Sequence Number:** 6  
**Type:** Synchronous  
**Is Synchronous:** True  
**Return Message:** 7. Free Space Information  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** .NET API (System.IO)
    - **Method:** new DriveInfo(driveName).AvailableFreeSpace
    - **Parameters:** The configured DICOM storage path (local or UNC). The drive letter or UNC root is extracted from this path.
    - **Authentication:** Service account's file system permissions.
    - **Error Handling:** Catch System.IO.IOException or ArgumentException if the path is invalid or inaccessible. Update status to 'Error' with details.
    - **Performance:** Very fast for local drives, can be slower for UNC paths.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-02-SVC  
**Message:** 8. Update In-Memory Health Status Cache  
**Sequence Number:** 8  
**Type:** Self  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Process Call
    - **Method:** IHealthStatusCache.UpdateStatus(newHealthStatus)
    - **Parameters:** A DTO containing all collected metrics (DB status, MQ status, queue depths, disk space).
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** Immediate. The cache is a thread-safe singleton service registered with DI.
    
- **Source Id:** REPO-02-SVC  
**Target Id:** REPO-02-SVC  
**Message:** 9. [OPT] If any metric crosses a critical threshold, trigger alert.  
**Sequence Number:** 9  
**Type:** Self  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Process Call
    - **Method:** IAlertingService.SendCriticalAlertAsync(alertDetails)
    - **Parameters:** Alert details object specifying the component, metric, value, and threshold.
    - **Authentication:** N/A
    - **Error Handling:** The alerting service should handle its own exceptions (e.g., SMTP failures) and log them without disrupting the health probe.
    - **Performance:** Should be non-blocking (`Task.Run` or fire-and-forget) to not delay the next polling cycle.
    

### 2.6. Notes

- **Content:** The HealthStatusCache should be implemented as a thread-safe singleton service, injected via DI into the HealthProbeHostedService and any controller that exposes health data.  
**Position:** top_right  
**Participant Id:** REPO-02-SVC  
**Sequence Number:** 8  
- **Content:** Alerting thresholds (e.g., disk space < 10% critical, DLQ count > 0 critical) must be managed via external application configuration to allow for environment-specific tuning.  
**Position:** bottom_right  
**Participant Id:** REPO-02-SVC  
**Sequence Number:** 9  

### 2.7. Implementation Guidance

- **Security Requirements:** Credentials for PostgreSQL and the RabbitMQ Management API must be retrieved from a secure store like Windows Credential Manager, not from plaintext config files (REQ-NFR-004). The service account running the DMPS Windows Service requires read permissions on the DICOM storage path.
- **Performance Targets:** The entire polling cycle should complete within a fraction of the polling interval (e.g., under 15 seconds for a 60-second interval). Network timeouts for external checks must be aggressive to prevent the probe from getting stuck.
- **Error Handling Strategy:** Each health check (DB, MQ, Disk) must be individually wrapped in a try/catch block. The failure of one check must not prevent the others from running. The final cached status must accurately reflect the state of each component, whether 'Connected', 'Disconnected', or 'Error'.
- **Testing Considerations:** Unit test the probe logic by mocking dependency interfaces (e.g., IDbConnection, IHttpClientFactory). Integration tests should run against a dedicated test environment where dependencies can be intentionally stopped (e.g., shut down PostgreSQL service) to validate failure detection and status reporting.
- **Monitoring Requirements:** The health probe itself is a core monitoring component. Its activity should be logged at an INFO level for each polling cycle. When a critical threshold is breached, the event must be logged as an ERROR to the Windows Event Log in addition to sending an email alert, fulfilling REQ-REP-002.
- **Deployment Considerations:** All configurable values (polling interval, connection strings, API endpoints, alerting thresholds) must be part of the application's external configuration, allowing for different settings in development, staging, and production environments.


---

