# Specification

# 1. Alerting And Incident Response Analysis

- **System Overview:**
  
  - **Analysis Date:** 2025-06-13
  - **Technology Stack:**
    
    - .NET 8
    - WPF
    - PostgreSQL 16
    - RabbitMQ
    - Windows Server
    
  - **Metrics Configuration:**
    
    - Windows Service Status
    - PostgreSQL Connectivity
    - RabbitMQ Connectivity & Queue Depth
    - DICOM Storage Path Disk Space
    - Windows Event Log
    
  - **Monitoring Needs:**
    
    - Ensure uptime and availability of the core background service.
    - Prevent data loss from failed asynchronous jobs.
    - Proactively manage storage capacity to avoid service interruptions.
    - Ensure core dependencies (Database, Message Broker) are available.
    - Be notified of critical, unhandled application errors.
    
  - **Environment:** production
  
- **Alert Condition And Threshold Design:**
  
  - **Critical Metrics Alerts:**
    
    - **Metric:** rabbitmq.dlq.message_count  
**Condition:** is greater than  
**Threshold Type:** static  
**Value:** 0  
**Justification:** REQ-REP-002 mandates monitoring the DLQ. Any message in the DLQ represents a failed, non-recoverable job (e.g., DICOM store, print) that requires manual intervention.  
**Business Impact:** Potential data loss or failure to complete a critical user-requested task like printing.  
    - **Metric:** filesystem.dicom_storage.free_space_percent  
**Condition:** is less than  
**Threshold Type:** static  
**Value:** 10  
**Justification:** REQ-REP-002 mandates monitoring storage space with a critical threshold. If storage is full, the system cannot accept new DICOM studies, halting a primary function.  
**Business Impact:** System ceases to be able to ingest new patient data, halting clinical workflows.  
    - **Metric:** windows_service.status  
**Condition:** is not equal to  
**Threshold Type:** static  
**Value:** Running  
**Justification:** REQ-REP-002 requires monitoring the service status. If the service is stopped, all background processing, DICOM SCP, and printing functions are offline.  
**Business Impact:** Total outage of all backend functionality including data ingestion and printing.  
    - **Metric:** dependency.connectivity.status  
**Condition:** is equal to  
**Threshold Type:** static  
**Value:** Disconnected  
**Justification:** REQ-REP-002 requires monitoring connectivity to PostgreSQL and RabbitMQ. Loss of either dependency results in a critical failure of the service's ability to process data.  
**Business Impact:** System is unable to persist data or process asynchronous jobs, leading to data backlogs and functional outages.  
    - **Metric:** rabbitmq.main_queue.message_count  
**Condition:** is greater than  
**Threshold Type:** static  
**Value:** 1000  
**Justification:** REQ-REP-002 requires monitoring queue depth. A sustained high queue depth indicates the consumer service is unable to keep up with the workload, which could be a symptom of performance degradation or a silent failure.  
**Business Impact:** Significant delays in processing critical data and print jobs, impacting user workflow.  
    - **Metric:** windows_event_log.new_event.level  
**Condition:** is equal to  
**Threshold Type:** static  
**Value:** Error  
**Justification:** REQ-REP-002 explicitly requires alerting on critical errors logged to the Windows Event Log. This captures unhandled exceptions and other severe application failures.  
**Business Impact:** Indicates a potentially serious software defect or misconfiguration that could lead to data corruption or instability.  
    
  - **Threshold Strategies:**
    
    
  - **Baseline Deviation Alerts:**
    
    
  - **Predictive Alerts:**
    
    
  - **Compound Conditions:**
    
    
  
- **Severity Level Classification:**
  
  - **Severity Definitions:**
    
    - **Level:** Critical  
**Criteria:** A complete loss of a primary system function (e.g., service down, DB unreachable, storage full) or a condition that requires immediate manual intervention to prevent data loss (e.g., DLQ message).  
**Business Impact:** Major disruption to clinical workflows, potential for data loss.  
**Customer Impact:** Core features are unusable.  
**Response Time:** < 15 minutes  
**Escalation Required:** True  
    - **Level:** High  
**Criteria:** A significant degradation of service or an impending critical failure (e.g., high queue depth, low storage warning). Unhandled application exceptions that do not cause a full outage.  
**Business Impact:** Significant delay or disruption to user workflows.  
**Customer Impact:** System is slow or some features are intermittently failing.  
**Response Time:** < 1 hour  
**Escalation Required:** True  
    - **Level:** Warning  
**Criteria:** A condition that indicates a potential future problem if left unaddressed (e.g., storage space approaching low threshold).  
**Business Impact:** No immediate impact, but poses a future risk to service availability.  
**Customer Impact:** None.  
**Response Time:** < 24 hours  
**Escalation Required:** False  
    
  - **Business Impact Matrix:**
    
    
  - **Customer Impact Criteria:**
    
    
  - **Sla Violation Severity:**
    
    
  - **System Health Severity:**
    
    
  
- **Notification Channel Strategy:**
  
  - **Channel Configuration:**
    
    - **Channel:** email  
**Purpose:** Primary notification channel for all alerts, as specified in REQ-REP-002.  
**Applicable Severities:**
    
    - Critical
    - High
    - Warning
    
**Time Constraints:** 24/7  
**Configuration:**
    
    
    - **Channel:** teams  
**Purpose:** High-priority channel for critical alerts requiring immediate attention from the on-call team.  
**Applicable Severities:**
    
    - Critical
    
**Time Constraints:** 24/7  
**Configuration:**
    
    
    
  - **Routing Rules:**
    
    - **Condition:** Severity == 'Critical'  
**Severity:** Critical  
**Alert Type:** any  
**Channels:**
    
    - email
    - teams
    
**Priority:** 1  
    - **Condition:** Severity == 'High'  
**Severity:** High  
**Alert Type:** any  
**Channels:**
    
    - email
    
**Priority:** 2  
    - **Condition:** Severity == 'Warning'  
**Severity:** Warning  
**Alert Type:** any  
**Channels:**
    
    - email
    
**Priority:** 3  
    
  - **Time Based Routing:**
    
    
  - **Ticketing Integration:**
    
    
  - **Emergency Notifications:**
    
    
  - **Chat Platform Integration:**
    
    
  
- **Alert Correlation Implementation:**
  
  - **Grouping Requirements:**
    
    - **Grouping Criteria:** Component (e.g., PostgreSQL, RabbitMQ, WindowsService)  
**Time Window:** 5 minutes  
**Max Group Size:** 10  
**Suppression Strategy:** Suppress consequential alerts. If a 'Database Unreachable' alert is active, suppress 'High Queue Depth' and 'Critical Application Error' alerts that are likely symptoms of the root cause.  
    
  - **Parent Child Relationships:**
    
    
  - **Topology Based Correlation:**
    
    
  - **Time Window Correlation:**
    
    
  - **Causal Relationship Detection:**
    
    
  - **Maintenance Window Suppression:**
    
    
  
- **False Positive Mitigation:**
  
  - **Noise Reduction Strategies:**
    
    
  - **Confirmation Counts:**
    
    - **Alert Type:** Dependency Connectivity Failure  
**Confirmation Threshold:** 3  
**Confirmation Window:** 90 seconds  
**Reset Condition:** A single successful check resets the count.  
    
  - **Dampening And Flapping:**
    
    
  - **Alert Validation:**
    
    
  - **Smart Filtering:**
    
    
  - **Quorum Based Alerting:**
    
    
  
- **On Call Management Integration:**
  
  - **Escalation Paths:**
    
    - **Severity:** Critical  
**Escalation Levels:**
    
    - **Level:** 1  
**Recipients:**
    
    - Primary On-Call Administrator
    
**Escalation Time:** 15 minutes  
**Requires Acknowledgment:** True  
    - **Level:** 2  
**Recipients:**
    
    - Secondary On-Call Administrator
    - Administrator Group Lead
    
**Escalation Time:** 30 minutes  
**Requires Acknowledgment:** True  
    
**Ultimate Escalation:** Head of IT Operations  
    
  - **Escalation Timeframes:**
    
    
  - **On Call Rotation:**
    
    
  - **Acknowledgment Requirements:**
    
    - **Severity:** Critical  
**Acknowledgment Timeout:** 15 minutes  
**Auto Escalation:** True  
**Requires Comment:** False  
    
  - **Incident Ownership:**
    
    
  - **Follow The Sun Support:**
    
    
  
- **Project Specific Alerts Config:**
  
  - **Alerts:**
    
    - **Name:** Dead-Letter Queue (DLQ) Contains Messages  
**Description:** One or more messages have failed processing permanently and have been moved to the DLQ. This requires immediate manual intervention to prevent data loss.  
**Condition:** rabbitmq.dlq.message_count > 0  
**Threshold:** Static, > 0  
**Severity:** Critical  
**Channels:**
    
    - email
    - teams
    
**Correlation:**
    
    - **Group Id:** RabbitMQ
    - **Suppression Rules:**
      
      
    
**Escalation:**
    
    - **Enabled:** True
    - **Escalation Time:** 15 minutes
    - **Escalation Path:**
      
      - Primary On-Call Administrator
      - Secondary On-Call Administrator
      
    
**Suppression:**
    
    - **Maintenance Window:** True
    - **Dependency Failure:** False
    - **Manual Override:** True
    
**Validation:**
    
    - **Confirmation Count:** 1
    - **Confirmation Window:** 0s
    
**Remediation:**
    
    - **Automated Actions:**
      
      
    - **Runbook Url:** runbooks/rabbitmq/dlq-investigation.html
    - **Troubleshooting Steps:**
      
      - Log in to the RabbitMQ Management UI.
      - Inspect the message(s) in the dead-letter queue.
      - Search application logs for the Correlation ID found in the message headers.
      - Identify and fix the root cause of the processing failure.
      - Manually republish the message to the original queue for reprocessing.
      
    
    - **Name:** DICOM Storage Disk Space Critical  
**Description:** Available disk space for the primary DICOM storage location is critically low. The system will soon be unable to accept new studies.  
**Condition:** filesystem.dicom_storage.free_space_percent < 10  
**Threshold:** Static, < 10%  
**Severity:** Critical  
**Channels:**
    
    - email
    - teams
    
**Correlation:**
    
    - **Group Id:** Storage
    - **Suppression Rules:**
      
      
    
**Escalation:**
    
    - **Enabled:** True
    - **Escalation Time:** 15 minutes
    - **Escalation Path:**
      
      - Primary On-Call Administrator
      - Secondary On-Call Administrator
      
    
**Suppression:**
    
    - **Maintenance Window:** True
    - **Dependency Failure:** False
    - **Manual Override:** True
    
**Validation:**
    
    - **Confirmation Count:** 2
    - **Confirmation Window:** 5 minutes
    
**Remediation:**
    
    - **Automated Actions:**
      
      
    - **Runbook Url:** runbooks/storage/add-capacity.html
    - **Troubleshooting Steps:**
      
      - Verify the current disk usage on the storage server.
      - Use the application's Administration panel to run the data purge job based on the retention policy.
      - If purging is not sufficient, provision additional storage capacity for the DICOM storage path.
      - Consider archiving older data to secondary storage.
      
    
    - **Name:** Background 'DICOM Service' is Down  
**Description:** The core Windows Service responsible for all background processing is not running. All asynchronous functionality is offline.  
**Condition:** windows_service.status != 'Running'  
**Threshold:** Static  
**Severity:** Critical  
**Channels:**
    
    - email
    - teams
    
**Correlation:**
    
    - **Group Id:** WindowsService
    - **Suppression Rules:**
      
      - Suppress 'High Queue Depth' if service is down.
      
    
**Escalation:**
    
    - **Enabled:** True
    - **Escalation Time:** 15 minutes
    - **Escalation Path:**
      
      - Primary On-Call Administrator
      - Secondary On-Call Administrator
      
    
**Suppression:**
    
    - **Maintenance Window:** True
    - **Dependency Failure:** False
    - **Manual Override:** True
    
**Validation:**
    
    - **Confirmation Count:** 2
    - **Confirmation Window:** 60s
    
**Remediation:**
    
    - **Automated Actions:**
      
      
    - **Runbook Url:** runbooks/service/restart-service.html
    - **Troubleshooting Steps:**
      
      - Attempt to start the 'DICOM Service' via services.msc.
      - If it fails to start, check the Windows Event Log (Application Log) for errors from the service source.
      - Review the service's own log files for startup exceptions.
      - Ensure dependencies (PostgreSQL, RabbitMQ) are reachable from the service host.
      
    
    - **Name:** Database or Message Broker Unreachable  
**Description:** The background service cannot connect to one of its critical dependencies (PostgreSQL or RabbitMQ).  
**Condition:** postgresql.connection.status == 'Disconnected' OR rabbitmq.connection.status == 'Disconnected'  
**Threshold:** Static  
**Severity:** Critical  
**Channels:**
    
    - email
    - teams
    
**Correlation:**
    
    - **Group Id:** Dependencies
    - **Suppression Rules:**
      
      
    
**Escalation:**
    
    - **Enabled:** True
    - **Escalation Time:** 15 minutes
    - **Escalation Path:**
      
      - Primary On-Call Administrator
      - Secondary On-Call Administrator
      
    
**Suppression:**
    
    - **Maintenance Window:** True
    - **Dependency Failure:** False
    - **Manual Override:** True
    
**Validation:**
    
    - **Confirmation Count:** 3
    - **Confirmation Window:** 90s
    
**Remediation:**
    
    - **Automated Actions:**
      
      
    - **Runbook Url:** runbooks/dependencies/connectivity-check.html
    - **Troubleshooting Steps:**
      
      - Identify which dependency is failing (PostgreSQL or RabbitMQ) from the alert details.
      - Verify the dependency's service is running on its host server.
      - Check network connectivity (ping, firewall rules, port access) from the application server to the dependency server.
      - Validate the connection credentials stored in the Windows Credential Manager.
      
    
    - **Name:** Main Processing Queue Depth High  
**Description:** The number of messages in a main work queue is excessively high, indicating the service is not processing tasks as fast as they are being created.  
**Condition:** rabbitmq.main_queue.message_count > 1000  
**Threshold:** Static, > 1000  
**Severity:** High  
**Channels:**
    
    - email
    
**Correlation:**
    
    - **Group Id:** RabbitMQ
    - **Suppression Rules:**
      
      
    
**Escalation:**
    
    - **Enabled:** False
    - **Escalation Time:** 
    - **Escalation Path:**
      
      
    
**Suppression:**
    
    - **Maintenance Window:** True
    - **Dependency Failure:** True
    - **Manual Override:** True
    
**Validation:**
    
    - **Confirmation Count:** 5
    - **Confirmation Window:** 15 minutes
    
**Remediation:**
    
    - **Automated Actions:**
      
      
    - **Runbook Url:** runbooks/rabbitmq/high-queue-depth.html
    - **Troubleshooting Steps:**
      
      - Verify the 'DICOM Service' is running and connected to RabbitMQ.
      - Check the application server for high CPU, memory, or disk I/O, which may be slowing down processing.
      - Investigate the performance of the PostgreSQL database for slow queries that could be blocking the consumer.
      - Review application logs for any recurring non-critical errors or warnings during message processing.
      
    
    
  - **Alert Groups:**
    
    
  - **Notification Templates:**
    
    
  
- **Implementation Priority:**
  
  - **Component:** Critical Failure Alerts (Service Down, Dependencies Unreachable, Storage Full, DLQ)  
**Priority:** high  
**Dependencies:**
    
    - System Health Probe Service
    
**Estimated Effort:** Medium  
**Risk Level:** low  
  - **Component:** Performance Degradation Alerts (High Queue Depth)  
**Priority:** medium  
**Dependencies:**
    
    - System Health Probe Service
    
**Estimated Effort:** Low  
**Risk Level:** medium  
  
- **Risk Assessment:**
  
  - **Risk:** Alert Fatigue  
**Impact:** high  
**Probability:** medium  
**Mitigation:** Alerts are strictly limited to actionable, critical, or high-severity events. Confirmation counts are used for connectivity checks to avoid flapping. Alert correlation will be used to suppress symptomatic alerts.  
**Contingency Plan:** Review and tune alert thresholds and conditions quarterly based on incident history.  
  - **Risk:** Monitoring System Failure  
**Impact:** high  
**Probability:** low  
**Mitigation:** The monitoring logic is a core part of the application itself. If the application is down, the 'Service Down' alert will be the final notification. External 'heartbeat' monitoring could be added for the service itself.  
**Contingency Plan:** Regular manual checks of the System Health Dashboard by administrators.  
  
- **Recommendations:**
  
  - **Category:** Observability  
**Recommendation:** Implement a 'heartbeat' mechanism where the Windows Service periodically sends a message to a dedicated, low-TTL RabbitMQ queue. An external checker can then monitor this queue to ensure the service is alive and processing.  
**Justification:** Provides an end-to-end check that the service is not only 'Running' in the OS sense but is actively processing messages, offering a more robust health check than a simple process status.  
**Priority:** medium  
**Implementation Notes:** This requires a small, separate monitoring process or script.  
  - **Category:** Incident Management  
**Recommendation:** Establish clear, documented runbooks for each defined alert, as linked in the remediation steps.  
**Justification:** Ensures a consistent, efficient, and correct response to incidents, reducing mean time to resolution (MTTR) and minimizing the risk of human error during a high-stress event.  
**Priority:** high  
**Implementation Notes:** Runbooks should be stored in a centralized, accessible location like a company wiki or shared document repository.  
  


---

