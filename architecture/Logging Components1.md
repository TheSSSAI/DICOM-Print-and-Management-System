# Specification

# 1. Logging And Observability Analysis

- **System Overview:**
  
  - **Analysis Date:** 2024-10-27
  - **Technology Stack:**
    
    - .NET 8
    - C# 12
    - WPF
    - Serilog
    - RabbitMQ
    - PostgreSQL
    
  - **Monitoring Requirements:**
    
    - REQ-REP-002: Serilog framework with File and EventLog sinks
    - REQ-REP-002: Masking of all PHI in logs
    - REQ-REP-002: Configurable log rotation and retention (30 days)
    - REQ-REP-002: Use of Correlation ID for end-to-end tracing
    - REQ-REP-002: Critical errors and security events written to Windows Event Log
    - REQ-FNC-003: PHI masking in logs to comply with HIPAA
    
  - **System Architecture:** Decoupled Client-Server with a background Windows Service and Message Queue (RabbitMQ)
  - **Environment:** production
  
- **Log Level And Category Strategy:**
  
  - **Default Log Level:** INFO
  - **Environment Specific Levels:**
    
    - **Environment:** Development  
**Log Level:** DEBUG  
**Justification:** Provides detailed diagnostic information for developers during feature development and troubleshooting, without impacting production performance.  
    - **Environment:** Production  
**Log Level:** INFO  
**Justification:** Captures significant operational events, warnings, and errors without the performance overhead of verbose logging.  
    
  - **Component Categories:**
    
    - **Component:** WPF Client  
**Category:** DMPS.Client  
**Log Level:** INFO  
**Verbose Logging:** False  
**Justification:** Logs user-initiated actions, UI-level errors, and communication events with the background service.  
    - **Component:** Windows Service  
**Category:** DMPS.Service  
**Log Level:** INFO  
**Verbose Logging:** False  
**Justification:** General logs for the service lifecycle, hosted service management, and core operations.  
    - **Component:** DICOM C-STORE SCP  
**Category:** DMPS.Service.DicomScp  
**Log Level:** INFO  
**Verbose Logging:** True  
**Justification:** Logs all incoming DICOM association requests, successes, and failures. Verbose logging can be enabled at DEBUG level for network troubleshooting.  
    - **Component:** RabbitMQ Consumer  
**Category:** DMPS.Service.MessageConsumer  
**Log Level:** INFO  
**Verbose Logging:** False  
**Justification:** Logs message consumption events, processing status (success/failure), and retries for tasks like database writes and printing.  
    
  - **Sampling Strategies:**
    
    
  - **Logging Approach:**
    
    - **Structured:** True
    - **Format:** JSON
    - **Standard Fields:**
      
      - Timestamp
      - Level
      - MessageTemplate
      - RenderedMessage
      - Exception
      - SourceContext
      
    - **Custom Fields:**
      
      - CorrelationId
      - ThreadId
      - ProcessName
      
    
  
- **Log Aggregation Architecture:**
  
  - **Collection Mechanism:**
    
    - **Type:** library
    - **Technology:** Serilog
    - **Configuration:**
      
      - **Sinks:**
        
        - Serilog.Sinks.File
        - Serilog.Sinks.EventLog
        
      - **Enrichers:**
        
        - FromLogContext
        - WithThreadId
        - WithProcessName
        
      - **Destructuring:** Default
      
    - **Justification:** Directly implements the requirements of REQ-REP-002 using the specified technology stack.
    
  - **Strategy:**
    
    - **Approach:** local
    - **Reasoning:** The system requirements specify logging to local files and the local Windows Event Log. A centralized logging solution is not required and would be out of scope.
    - **Local Retention:** 30 days
    
  - **Shipping Methods:**
    
    
  - **Buffering And Batching:**
    
    - **Buffer Size:** Default (handled by Serilog sinks)
    - **Batch Size:** 0
    - **Flush Interval:** Default (handled by Serilog sinks)
    - **Backpressure Handling:** N/A
    
  - **Transformation And Enrichment:**
    
    - **Transformation:** Correlation ID Enrichment  
**Purpose:** To enable end-to-end tracing of asynchronous operations across the client, message queue, and service, as per REQ-REP-002.  
**Stage:** collection  
    - **Transformation:** PHI Masking  
**Purpose:** To redact Protected Health Information (e.g., Patient Name, Patient ID) from all log messages before they are written to any sink, fulfilling REQ-FNC-003.  
**Stage:** collection  
    
  - **High Availability:**
    
    - **Required:** False
    - **Redundancy:** N/A
    - **Failover Strategy:** N/A
    
  
- **Retention Policy Design:**
  
  - **Retention Periods:**
    
    - **Log Type:** Application Diagnostic Logs (File)  
**Retention Period:** 30 days  
**Justification:** Explicitly defined in REQ-REP-002 to balance troubleshooting needs with disk space management.  
**Compliance Requirement:** N/A  
    - **Log Type:** Critical Error Logs (Windows Event Log)  
**Retention Period:** System-managed  
**Justification:** Retention is managed by Windows OS policy for the Application Event Log, as per REQ-REP-002.  
**Compliance Requirement:** HIPAA (indirectly, for security event logging)  
    
  - **Compliance Requirements:**
    
    - **Regulation:** HIPAA  
**Applicable Log Types:**
    
    - Application Diagnostic Logs (File)
    - Critical Error Logs (Windows Event Log)
    
**Minimum Retention:** N/A (for diagnostic logs)  
**Special Handling:** All PHI must be masked before logging.  
    
  - **Volume Impact Analysis:**
    
    - **Estimated Daily Volume:** Low to Medium
    - **Storage Cost Projection:** Negligible (local storage)
    - **Compression Ratio:** N/A (not required, but can be enabled on file sink)
    
  - **Storage Tiering:**
    
    
  - **Compression Strategy:**
    
    
  - **Anonymization Requirements:**
    
    - **Data Type:** PHI (Patient Health Information)  
**Method:** Masking (e.g., 'PatientName: ******')  
**Timeline:** real-time (at log creation)  
**Compliance:** HIPAA (REQ-FNC-003, REQ-REP-002)  
    
  
- **Search Capability Requirements:**
  
  - **Essential Capabilities:**
    
    - **Capability:** Filtering by Correlation ID  
**Performance Requirement:** Manual (human speed)  
**Justification:** The primary troubleshooting requirement is to trace a single operation through the system using its unique ID (REQ-REP-002).  
    - **Capability:** Text-based search of local log files  
**Performance Requirement:** Manual (human speed)  
**Justification:** Standard practice for local file-based logs using OS tools (e.g., text editors, PowerShell).  
    
  - **Performance Characteristics:**
    
    - **Search Latency:** N/A
    - **Concurrent Users:** 1
    - **Query Complexity:** simple
    - **Indexing Strategy:** N/A
    
  - **Indexed Fields:**
    
    
  - **Full Text Search:**
    
    - **Required:** False
    
  - **Correlation And Tracing:**
    
    - **Correlation Ids:**
      
      - CorrelationId
      
    - **Trace Id Propagation:** RabbitMQ Message Header
    - **Span Correlation:** False
    - **Cross Service Tracing:** False
    
  - **Dashboard Requirements:**
    
    
  
- **Storage Solution Selection:**
  
  - **Selected Technology:**
    
    - **Primary:** Local File System and Windows Event Log
    - **Reasoning:** Explicitly mandated by the requirements in REQ-REP-002. No other solution is necessary.
    - **Alternatives:**
      
      
    
  - **Scalability Requirements:**
    
    - **Expected Growth Rate:** Low
    - **Peak Load Handling:** Handled by Serilog's asynchronous sinks.
    - **Horizontal Scaling:** False
    
  - **Cost Performance Analysis:**
    
    
  - **Backup And Recovery:**
    
    - **Backup Frequency:** System-level backups
    - **Recovery Time Objective:** N/A
    - **Recovery Point Objective:** N/A
    - **Testing Frequency:** N/A
    
  - **Geo Distribution:**
    
    - **Required:** False
    
  - **Data Sovereignty:**
    
    
  
- **Access Control And Compliance:**
  
  - **Access Control Requirements:**
    
    - **Role:** System Administrator  
**Permissions:**
    
    - read
    
**Log Types:**
    
    - Application Diagnostic Logs (File)
    - Critical Error Logs (Windows Event Log)
    
**Justification:** Access to logs should be restricted to authorized personnel for troubleshooting and security incident investigation.  
    
  - **Sensitive Data Handling:**
    
    - **Data Type:** PHI  
**Handling Strategy:** mask  
**Fields:**
    
    - PatientName
    - PatientID
    - AccessionNumber
    - ReferringPhysicianName
    
**Compliance Requirement:** HIPAA (REQ-FNC-003, REQ-REP-002)  
    
  - **Encryption Requirements:**
    
    - **In Transit:**
      
      - **Required:** False
      - **Protocol:** N/A
      - **Certificate Management:** N/A
      
    - **At Rest:**
      
      - **Required:** False
      - **Algorithm:** Full Disk Encryption (Recommended)
      - **Key Management:** OS-managed
      
    
  - **Audit Trail:**
    
    - **Log Access:** False
    - **Retention Period:** N/A
    - **Audit Log Location:** N/A
    - **Compliance Reporting:** False
    
  - **Regulatory Compliance:**
    
    - **Regulation:** HIPAA  
**Applicable Components:**
    
    - WPF Client
    - Windows Service
    
**Specific Requirements:**
    
    - PHI in logs must be masked.
    
**Evidence Collection:** Review of log files to confirm masking is applied.  
    
  - **Data Protection Measures:**
    
    
  
- **Project Specific Logging Config:**
  
  - **Logging Config:**
    
    - **Level:** INFO
    - **Retention:** 30 Days (File)
    - **Aggregation:** Local
    - **Storage:** File System & Windows Event Log
    - **Configuration:**
      
      - **Serilog:**
        
        - **Minimum Level:**
          
          - **Default:** Information
          - **Override:**
            
            - **Microsoft:** Warning
            - **System:** Warning
            
          
        - **Write To:**
          
          - **Name:** File  
**Args:**
    
    - **Path:** Logs/dmps_.log
    - **Rolling Interval:** Day
    - **Retained File Count Limit:** 30
    - **Output Template:** {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}
    
          - **Name:** EventLog  
**Args:**
    
    - **Source:** DICOM Management System
    - **Restricted To Minimum Level:** Error
    
          
        - **Enrich:**
          
          - FromLogContext
          - WithMachineName
          - WithThreadId
          
        
      
    
  - **Component Configurations:**
    
    - **Component:** WPF Client  
**Log Level:** INFO  
**Output Format:** JSON  
**Destinations:**
    
    - Local File
    
**Custom Fields:**
    
    - CorrelationId
    
    - **Component:** Windows Service  
**Log Level:** INFO  
**Output Format:** JSON  
**Destinations:**
    
    - Local File
    - Windows Event Log (for errors)
    
**Custom Fields:**
    
    - CorrelationId
    
    
  - **Metrics:**
    
    
  - **Alert Rules:**
    
    - **Name:** Critical Application Error  
**Condition:** An event with log level 'Error' or 'Fatal' is written to the Windows Event Log by the 'DICOM Management System' source.  
**Severity:** Critical  
**Actions:**
    
    - **Type:** email  
**Target:** [AdminEmailAddress]  
**Configuration:**
    
    - **Smtp Server:** [Configured SMTP]
    - **Subject:** CRITICAL ALERT: Error in DICOM Management System
    
    
**Suppression Rules:**
    
    - Do not re-alert for the same exception type for 15 minutes.
    
**Escalation Path:**
    
    - System Administrator
    
    
  
- **Implementation Priority:**
  
  - **Component:** Core Serilog Pipeline  
**Priority:** high  
**Dependencies:**
    
    
**Estimated Effort:** Low  
**Risk Level:** low  
  - **Component:** PHI Masking Enricher  
**Priority:** high  
**Dependencies:**
    
    - Core Serilog Pipeline
    
**Estimated Effort:** Medium  
**Risk Level:** medium  
  - **Component:** Correlation ID Propagation  
**Priority:** medium  
**Dependencies:**
    
    - Core Serilog Pipeline
    
**Estimated Effort:** Low  
**Risk Level:** low  
  
- **Risk Assessment:**
  
  - **Risk:** PHI is inadvertently leaked into log files due to incomplete or incorrect masking logic.  
**Impact:** high  
**Probability:** medium  
**Mitigation:** Develop a comprehensive, unit-tested PHI masking utility. Conduct a peer review of the masking logic and perform manual validation of log outputs in a test environment with sample data.  
**Contingency Plan:** If a leak is discovered, immediately patch the masking logic. Purge affected log files in accordance with security policy and perform an incident response analysis.  
  - **Risk:** Log files consume excessive disk space, potentially causing application or system failure.  
**Impact:** medium  
**Probability:** low  
**Mitigation:** Strictly enforce the 30-day retention policy (REQ-REP-002). The System Health Dashboard will monitor available disk space and alert administrators if it falls below a critical threshold (REQ-REP-002).  
**Contingency Plan:** Manually archive or purge older log files. Adjust the log level to a less verbose setting if necessary.  
  
- **Recommendations:**
  
  - **Category:** Security  
**Recommendation:** The PHI masking implementation should be a standalone, testable component that uses regular expressions or other reliable methods to identify and redact PHI patterns.  
**Justification:** Ensures the critical HIPAA compliance requirement is robust, maintainable, and verifiable through automated testing, reducing the risk of accidental data leakage.  
**Priority:** high  
**Implementation Notes:** Create a dedicated utility class for masking and a suite of xUnit tests covering various PHI formats.  
  - **Category:** Maintainability  
**Recommendation:** Use structured logging properties for all contextual data instead of embedding values in the message string.  
**Justification:** Makes log data easier to query and parse, especially when searching for specific values like a StudyUID or a PrinterName, even with local file-based logs.  
**Priority:** medium  
**Implementation Notes:** Example: logger.Information("Processing print job {PrintJobId} for user {UserId}", jobId, userId); NOT logger.Information($"Processing print job {jobId} for user {userId}");  
  


---

