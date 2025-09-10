# Specification

# 1. Overview
## 1. Client Synchronous Status Check via Named Pipes
Before enabling a UI action dependent on the background service (like printing), the WPF client makes a synchronous request/reply call via a local Named Pipe to get its status. This proactive check prevents queueing tasks that are guaranteed to fail, reducing errors and improving user feedback.

### 1.1. Diagram Id
SEQ-SIT-012

### 1.4. Type
ServiceInteraction

### 1.5. Purpose
To provide immediate feedback to the user and prevent them from initiating tasks that are destined to fail because a required background service is not running.

### 1.6. Complexity
Low

### 1.7. Priority
Medium

### 1.8. Frequency
OnDemand

### 1.9. Participants

- REPO-08-APC
- REPO-11-INF
- REPO-02-SVC

### 1.10. Key Interactions

- Client UI is about to render the Print Preview screen.
- The ISystemStatusService is called from the ViewModel.
- The NamedPipeClient writes a status request string ('PING') to the pipe.
- The background service's NamedPipeServer reads the request.
- The server immediately writes a status response string ('PONG') back to the pipe.
- The client reads the response and updates the UI accordingly (e.g., enables 'Print' button).

### 1.11. Triggers

- A UI element's state depends on the service status (FR-3.1.1.1).

### 1.12. Outcomes

- The client receives the real-time status of the background service.
- UI elements like the 'Print' button are correctly enabled or disabled.

### 1.13. Business Rules

- Direct, synchronous communication for real-time status checks shall be handled via Named Pipes (REQ-TEC-002).

### 1.14. Error Scenarios

- The service is stopped, and the pipe connection fails with a TimeoutException, indicating the service is unavailable.

### 1.15. Integration Points

- .NET Named Pipes


---

# 2. Details
## 2. Implementation: Client Synchronous Status Check via Named Pipes
Technical sequence for the WPF client's synchronous status check of the background Windows Service. This interaction uses a local .NET Named Pipe for low-latency request/reply communication. The primary goal is to verify service availability before enabling UI controls like the 'Print' button, thereby providing immediate user feedback and preventing the queuing of tasks destined to fail. This sequence models both the successful 'PONG' response and the TimeoutException error scenario.

### 2.1. Diagram Id
SEQ-SIT-012-IMPL

### 2.4. Participants

- **Repository Id:** REPO-08-APC  
**Display Name:** Client Application Service  
**Type:** Application Service  
**Technology:** .NET 8, C# 12  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #4285F4
    - **Stereotype:** Layer: Application
    
- **Repository Id:** REPO-05-COM  
**Display Name:** NamedPipeClient (Infrastructure)  
**Type:** IPC Client  
**Technology:** System.IO.Pipes.NamedPipeClientStream  
**Order:** 2  
**Style:**
    
    - **Shape:** participant
    - **Color:** #DB4437
    - **Stereotype:** Layer: Infrastructure
    
- **Repository Id:** REPO-05-COM  
**Display Name:** NamedPipeServer (Infrastructure)  
**Type:** IPC Server  
**Technology:** System.IO.Pipes.NamedPipeServerStream  
**Order:** 3  
**Style:**
    
    - **Shape:** participant
    - **Color:** #F4B400
    - **Stereotype:** Layer: Infrastructure
    
- **Repository Id:** REPO-02-SVC  
**Display Name:** Background Hosted Service  
**Type:** Background Service  
**Technology:** Microsoft.Extensions.Hosting.IHostedService  
**Order:** 4  
**Style:**
    
    - **Shape:** participant
    - **Color:** #0F9D58
    - **Stereotype:** Process: DICOM Service
    

### 2.5. Interactions

- **Source Id:** REPO-08-APC  
**Target Id:** REPO-05-COM  
**Message:** 1. Call IsBackgroundServiceRunningAsync()  
**Sequence Number:** 1  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** 10. return serviceIsRunning (bool)  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** ISystemStatusService.IsBackgroundServiceRunningAsync()
    - **Parameters:** CancellationToken
    - **Authentication:** N/A (In-process call)
    - **Error Handling:** Catches TimeoutException from the client and translates it to a 'false' return value.
    - **Performance:** Method orchestration should add <1ms overhead.
    
**Nested Interactions:**
    
    - **Source Id:** REPO-05-COM  
**Target Id:** REPO-05-COM  
**Message:** 2. Connect to Named Pipe  
**Sequence Number:** 2  
**Type:** IPC Connection  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Named Pipes
    - **Method:** NamedPipeClientStream.Connect(timeout)
    - **Parameters:** timeout: int (e.g., 2000ms)
    - **Authentication:** PipeSecurity ACLs restrict access to local user/system.
    - **Error Handling:** Throws TimeoutException if the server pipe is not available within the specified timeout. This is the primary indicator that the service is down.
    - **Performance:** Local connection establishment is typically <5ms.
    
    - **Source Id:** REPO-05-COM  
**Target Id:** REPO-05-COM  
**Message:** 3. Write request message: 'PING'  
**Sequence Number:** 3  
**Type:** IPC Write  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** Named Pipes
    - **Method:** StreamWriter.WriteLineAsync("PING")
    - **Parameters:** Payload: string, UTF-8 encoded
    - **Authentication:** N/A (occurs over established pipe)
    - **Error Handling:** Throws IOException on pipe failure.
    - **Performance:** Extremely low latency (<1ms).
    
    - **Source Id:** REPO-05-COM  
**Target Id:** REPO-02-SVC  
**Message:** 4. Listen for client connection  
**Sequence Number:** 4  
**Type:** IPC Listen  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Named Pipes
    - **Method:** NamedPipeServerStream.WaitForConnectionAsync()
    - **Parameters:** Hosted within a dedicated IHostedService loop.
    - **Authentication:** N/A
    - **Error Handling:** Handles cancellation via CancellationToken on service shutdown.
    - **Performance:** Blocking call, runs on a dedicated background thread.
    
    - **Source Id:** REPO-05-COM  
**Target Id:** REPO-05-COM  
**Message:** 5. Read request message: 'PING'  
**Sequence Number:** 5  
**Type:** IPC Read  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Named Pipes
    - **Method:** StreamReader.ReadLineAsync()
    - **Parameters:** Expects UTF-8 string.
    - **Authentication:** N/A
    - **Error Handling:** Handles malformed messages or stream closure.
    - **Performance:** Extremely low latency (<1ms).
    
    - **Source Id:** REPO-05-COM  
**Target Id:** REPO-02-SVC  
**Message:** 6. [Internal] Verify Host Service is Healthy  
**Sequence Number:** 6  
**Type:** Internal Check  
**Is Synchronous:** True  
**Return Message:** Returns true  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** Internal health status check
    - **Parameters:** N/A
    - **Authentication:** N/A
    - **Error Handling:** The server's existence implies it is running. More complex health checks could be added here.
    - **Performance:** Sub-millisecond.
    
    - **Source Id:** REPO-05-COM  
**Target Id:** REPO-05-COM  
**Message:** 7. Write response message: 'PONG'  
**Sequence Number:** 7  
**Type:** IPC Write  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Named Pipes
    - **Method:** StreamWriter.WriteLineAsync("PONG")
    - **Parameters:** Payload: string, UTF-8 encoded
    - **Authentication:** N/A
    - **Error Handling:** Throws IOException on pipe failure.
    - **Performance:** Extremely low latency (<1ms).
    
    - **Source Id:** REPO-05-COM  
**Target Id:** REPO-05-COM  
**Message:** 8. Read response message: 'PONG'  
**Sequence Number:** 8  
**Type:** IPC Read  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Named Pipes
    - **Method:** StreamReader.ReadLineAsync()
    - **Parameters:** Result is validated to be 'PONG'.
    - **Authentication:** N/A
    - **Error Handling:** If response is not 'PONG' or null, it's considered a failure.
    - **Performance:** Extremely low latency (<1ms).
    
    - **Source Id:** REPO-05-COM  
**Target Id:** REPO-05-COM  
**Message:** 9. Dispose Pipe Connection  
**Sequence Number:** 9  
**Type:** Resource Cleanup  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Named Pipes
    - **Method:** NamedPipeClientStream.Dispose()
    - **Parameters:** N/A
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** N/A
    
    

### 2.6. Notes

- **Content:** Error Scenario: If the Background Service is stopped, the call to 'Connect to Named Pipe' (Step 2) will fail after its configured timeout (e.g., 2000ms), throwing a TimeoutException. The ISystemStatusService implementation must catch this specific exception and return 'false' to the caller, indicating the service is unavailable.  
**Position:** bottom  
**Participant Id:** None  
**Sequence Number:** 2  
- **Content:** Configuration: The Named Pipe name (e.g., 'DicomServiceStatusPipe') must be a shared, constant value known to both the client and service applications. It should be defined in a shared library to prevent mismatches.  
**Position:** top  
**Participant Id:** REPO-05-COM  
**Sequence Number:** 2  

### 2.7. Implementation Guidance

- **Security Requirements:** The NamedPipeServerStream must be created with a PipeSecurity object that applies an Access Control List (ACL). This ACL should restrict access to the pipe to the same user account running the process, or to specific user groups (e.g., SYSTEM, Administrators), to prevent unauthorized local processes from interacting with the service.
- **Performance Targets:** As a local IPC mechanism, the end-to-end latency for this status check (from Step 1 to Step 10) must be under 50ms on a standard system. The connection timeout should be configured conservatively, e.g., 1-2 seconds, to provide rapid feedback to the user without excessive waiting.
- **Error Handling Strategy:** The client-side logic must be robust. The primary failure mode is a TimeoutException on connection, which definitively means the service is not running. All other IOExceptions on the pipe should also be treated as a service failure. The service-side pipe server must be wrapped in a try/catch/finally block to ensure the pipe handle is disposed of correctly, even if the client disconnects abruptly.
- **Testing Considerations:** Integration tests are critical. A test suite must be created that programmatically starts and stops the background service process and then runs the client-side status check logic to verify that it correctly reports 'true' when the service is up and 'false' (after a timeout) when the service is down.
- **Monitoring Requirements:** While this is a client-side check, logs should be generated for both success and failure cases. A failed status check (timeout) should be logged at a 'Warning' level, as it may indicate an improperly configured or crashed service. The frequency of these checks should be limited to user-initiated actions to avoid log spam.
- **Deployment Considerations:** The client installer and the service installer must be configured with the identical Named Pipe name. Any changes to this name require a coordinated update of both the client and service applications.


---

