# Specification

# 1. Error Handling

- **Strategies:**
  
  - **Type:** Retry  
**Configuration:**
    
    - **Description:** Handles transient errors during asynchronous message processing within the background service (e.g., temporary database lock, network hiccup to printer). This precedes dead-lettering.
    - **Retry Attempts:** 3
    - **Backoff Strategy:** Exponential
    - **Retry Intervals:**
      
      - **Interval1:** 5s
      - **Interval2:** 15s
      - **Interval3:** 30s
      
    - **Error Handling Rules:**
      
      - MessageProcessingTransientError
      
    
  - **Type:** DeadLetter  
**Configuration:**
    
    - **Description:** Isolates non-recoverable 'poison' messages after retry attempts are exhausted, as required by REQ-NFR-005 and REQ-TEC-006. This prevents blocking the main processing queue.
    - **Dead Letter Queue:** dicom_service_jobs.dlq
    - **Error Handling Rules:**
      
      - PoisonMessageError
      
    
  - **Type:** Retry  
**Configuration:**
    
    - **Description:** Handles transient network errors for synchronous outbound requests from the client or service, such as license validation or C-ECHO tests, as required by REQ-INT-003.
    - **Retry Attempts:** 3
    - **Backoff Strategy:** LinearWithJitter
    - **Retry Intervals:**
      
      - **Interval1:** 2s
      - **Interval2:** 4s
      - **Interval3:** 8s
      
    - **Error Handling Rules:**
      
      - OdooApiHttp5xxError
      - DicomNetworkTimeout
      
    
  - **Type:** Fallback  
**Configuration:**
    
    - **Description:** Implements the specific license validation fallback behavior mandated by REQ-INT-003 when the Odoo API is unreachable after retries.
    - **Fallback Response:** Activate 72-hour grace period with full functionality. If failure persists, switch to read-only mode.
    - **Error Handling Rules:**
      
      - LicenseValidationUnreachable
      
    
  - **Type:** CircuitBreaker  
**Configuration:**
    
    - **Description:** Protects the system from repeated attempts to connect to an unavailable external PACS, preventing resource exhaustion and providing faster failure feedback.
    - **Failure Threshold:** 5
    - **Open Duration:** 60s
    - **Half Open Retry Delay:** 30s
    - **Error Handling Rules:**
      
      - PACSConnectionFailure
      
    
  
- **Monitoring:**
  
  - **Error Types:**
    
    - MessageProcessingTransientError
    - PoisonMessageError
    - OdooApiHttp5xxError
    - LicenseValidationUnreachable
    - DicomNetworkTimeout
    - PACSConnectionFailure
    - UnhandledSystemException
    
  - **Alerting:** As per REQ-REP-002, critical alerts are automatically sent to a configured administrator email address. Triggers include messages being moved to the dead-letter queue, the circuit breaker for a PACS opening, and critical thresholds being crossed on the System Health Dashboard (e.g., DLQ depth > 0, service stopped). All errors are logged to the Windows Event Log and a local file with a unique correlation ID.
  


---

