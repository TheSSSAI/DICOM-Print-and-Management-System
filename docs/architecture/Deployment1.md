# Specification

# 1. Deployment Environment Analysis

- **System Overview:**
  
  - **Analysis Date:** 2025-06-13
  - **Technology Stack:**
    
    - .NET 8
    - WPF
    - C# 12
    - PostgreSQL 16
    - RabbitMQ 3.12+
    - Windows Server
    
  - **Architecture Patterns:**
    
    - Client-Server
    - Background Worker Service
    - Message Queue (RabbitMQ)
    - Repository Pattern
    
  - **Data Handling Needs:**
    
    - Protected Health Information (PHI)
    - High-volume DICOM file storage
    - Transactional metadata in PostgreSQL
    - Data Encryption at Rest and in Transit
    
  - **Performance Expectations:** High UI responsiveness with long-running tasks offloaded to a background service. High-throughput data ingestion from medical modalities. Fast loading of large DICOM image series.
  - **Regulatory Requirements:**
    
    - HIPAA Security Rule
    - HIPAA Privacy Rule
    - Data Residency Requirements
    
  
- **Environment Strategy:**
  
  - **Environment Types:**
    
    - **Type:** Development  
**Purpose:** Individual developer workspaces for feature development, unit testing, and debugging.  
**Usage Patterns:**
    
    - Code compilation
    - Local debugging
    - Unit testing
    
**Isolation Level:** shared  
**Data Policy:** Use small, developer-generated, fully anonymized sample data. Direct connection to other environments is prohibited.  
**Lifecycle Management:** Ephemeral, created and destroyed by developers as needed. May use containerized dependencies (PostgreSQL/RabbitMQ) via Docker Compose as per REQ-6.1.  
    - **Type:** Testing  
**Purpose:** Shared environment for Quality Assurance (QA) to perform integration, system, and regression testing on CI/CD builds.  
**Usage Patterns:**
    
    - Automated integration tests
    - Manual QA validation
    - Performance testing
    
**Isolation Level:** partial  
**Data Policy:** Populated with a comprehensive, fully anonymized or masked dataset, periodically refreshed from a sanitized production backup.  
**Lifecycle Management:** Persistent environment, updated frequently by the CI/CD pipeline.  
    - **Type:** Staging  
**Purpose:** A production-like environment for User Acceptance Testing (UAT), final pre-release validation, and migration dry-runs.  
**Usage Patterns:**
    
    - UAT by business stakeholders
    - Data migration validation (REQ-8.2)
    - Training environment sandbox (REQ-8.3)
    
**Isolation Level:** complete  
**Data Policy:** Contains a recent, fully anonymized or heavily masked copy of the production dataset. No live PHI is permitted.  
**Lifecycle Management:** Stable, persistent environment, updated only with release candidates after they pass QA.  
    - **Type:** Production  
**Purpose:** The live environment used by end-users (Technicians, Administrators) for daily clinical and administrative operations.  
**Usage Patterns:**
    
    - Live data ingestion from modalities
    - DICOM viewing and printing
    - System administration
    
**Isolation Level:** complete  
**Data Policy:** Contains live Protected Health Information (PHI). Subject to the strictest access controls, monitoring, and auditing.  
**Lifecycle Management:** Highly available and strictly controlled. Changes are only made through a formal change management process.  
    - **Type:** DR  
**Purpose:** Disaster Recovery site to ensure business continuity in case of a primary production site failure, meeting RTO/RPO requirements.  
**Usage Patterns:**
    
    - Inactive (hot/warm standby)
    - Periodic failover testing
    
**Isolation Level:** complete  
**Data Policy:** Contains a replicated, near real-time copy of production data.  
**Lifecycle Management:** Continuously maintained and tested as part of the business continuity plan.  
    
  - **Promotion Strategy:**
    
    - **Workflow:** Development -> Testing -> Staging -> Production
    - **Approval Gates:**
      
      - QA sign-off to promote from Testing to Staging
      - UAT and Business Owner sign-off to promote from Staging to Production
      
    - **Automation Level:** semi-automated
    - **Rollback Procedure:** Restore previous application version from artifact repository and revert database schema using EF Core Migrations. For file storage, restore from pre-deployment snapshot.
    
  - **Isolation Strategies:**
    
    - **Environment:** Production  
**Isolation Type:** complete  
**Implementation:** Physically or logically separate network (VLAN/VPC), dedicated server hardware or VMs, separate storage, and unique credentials.  
**Justification:** Required by HIPAA to protect live PHI from risks associated with non-production environments.  
    - **Environment:** Staging  
**Isolation Type:** complete  
**Implementation:** Logically separate network (VLAN/VPC), dedicated VMs, and unique credentials. Mimics production isolation.  
**Justification:** Provides a safe environment for UAT and migration dry-runs without impacting production.  
    
  - **Scaling Approaches:**
    
    - **Environment:** Production  
**Scaling Type:** vertical  
**Triggers:**
    
    - Sustained high CPU/Memory utilization
    - Increased data ingestion volume
    
**Limits:** Dependent on hypervisor or physical hardware constraints.  
    
  - **Provisioning Automation:**
    
    - **Tool:** ansible
    - **Templating:** YAML playbooks and Jinja2 templates for configuring Windows Server roles, PostgreSQL, and RabbitMQ.
    - **State Management:** Managed by Ansible inventory files.
    - **Cicd Integration:** True
    
  
- **Resource Requirements Analysis:**
  
  - **Workload Analysis:**
    
    - **Workload Type:** Background Processing  
**Expected Load:** High-throughput, bursty data ingestion (C-STORE) and continuous, low-latency queue processing.  
**Peak Capacity:** 10 simultaneous C-STORE operations.  
**Resource Profile:** cpu-intensive  
    - **Workload Type:** Database Operations  
**Expected Load:** High volume of small writes from DICOM ingestion, moderate reads for user queries.  
**Peak Capacity:** Sustained high insert rate during peak clinical hours.  
**Resource Profile:** io-intensive  
    - **Workload Type:** DICOM File Storage  
**Expected Load:** Large sequential writes for new studies, random reads for viewing.  
**Peak Capacity:** Multi-terabyte storage capacity with predictable growth.  
**Resource Profile:** io-intensive  
    
  - **Compute Requirements:**
    
    - **Environment:** Production (Standard)  
**Instance Type:** Virtual Machine  
**Cpu Cores:** 8  
**Memory Gb:** 32  
**Instance Count:** 2  
**Auto Scaling:** None  
**Justification:** Sized for a standard clinic workload, with one server for the App (Windows Service) and one for DB/MQ.  
    - **Environment:** Production (High-Availability)  
**Instance Type:** Virtual Machine  
**Cpu Cores:** 16  
**Memory Gb:** 64  
**Instance Count:** 4  
**Auto Scaling:** None  
**Justification:** HA deployment with clustered DB/MQ and multiple App servers behind a load balancer to handle hospital-level workloads and provide redundancy.  
    - **Environment:** Staging  
**Instance Type:** Virtual Machine  
**Cpu Cores:** 4  
**Memory Gb:** 16  
**Instance Count:** 2  
**Auto Scaling:** None  
**Justification:** Sufficient resources to mirror the production architecture for accurate UAT and performance testing.  
    - **Environment:** Testing  
**Instance Type:** Virtual Machine  
**Cpu Cores:** 2  
**Memory Gb:** 8  
**Instance Count:** 1  
**Auto Scaling:** None  
**Justification:** Minimum viable resources for integrated QA testing and CI/CD validation.  
    
  - **Storage Requirements:**
    
    - **Environment:** Production  
**Storage Type:** ssd  
**Capacity:** 10TB+ (for DICOM files), 1TB (for Database)  
**Iops Requirements:** High IOPS for database volume, high throughput for DICOM file store.  
**Redundancy:** RAID 6/10 or equivalent cloud provider redundancy.  
**Encryption:** True  
    - **Environment:** Staging  
**Storage Type:** ssd  
**Capacity:** 1TB (for DICOM files), 250GB (for Database)  
**Iops Requirements:** Medium IOPS to reflect production performance characteristics.  
**Redundancy:** RAID 1/5 or equivalent.  
**Encryption:** True  
    
  - **Special Hardware Requirements:**
    
    - **Requirement:** gpu  
**Justification:** Client machines require a DirectX 11 compatible graphics card for GPU-accelerated rendering of DICOM images as per REQ-NFR-001.  
**Environment:** Production  
**Specifications:** DirectX 11 compatible GPU on end-user workstations.  
    
  - **Scaling Strategies:**
    
    - **Environment:** Production  
**Strategy:** reactive  
**Implementation:** Vertical scaling (adding CPU/RAM to VMs) is the primary strategy. For HA deployments, horizontal scaling of the Windows Service can be achieved by adding more app server nodes.  
**Cost Optimization:** Right-sizing instances based on observed long-term utilization metrics.  
    
  
- **Security Architecture:**
  
  - **Authentication Controls:**
    
    - **Method:** sso  
**Scope:** Administrator access to servers.  
**Implementation:** Integration with Active Directory for server login (RDP/SSH).  
**Environment:** All  
    
  - **Authorization Controls:**
    
    - **Model:** rbac  
**Implementation:** AD groups mapped to server roles (e.g., AppAdmins, DbAdmins) to enforce least privilege.  
**Granularity:** coarse  
**Environment:** All  
    
  - **Certificate Management:**
    
    - **Authority:** internal
    - **Rotation Policy:** Annual rotation for all internal TLS certificates.
    - **Automation:** True
    - **Monitoring:** True
    
  - **Encryption Standards:**
    
    - **Scope:** data-in-transit  
**Algorithm:** TLS 1.2 / 1.3  
**Key Management:** Internal Certificate Authority  
**Compliance:**
    
    - HIPAA
    
    - **Scope:** data-at-rest  
**Algorithm:** AES-256  
**Key Management:** PostgreSQL pgcrypto for column-level, Full Disk Encryption (BitLocker/LUKS) for server volumes.  
**Compliance:**
    
    - HIPAA
    
    
  - **Access Control Mechanisms:**
    
    - **Type:** security-groups  
**Configuration:** Stateful firewall rules applied at the VM level.  
**Environment:** All  
**Rules:**
    
    - Allow App Server to DB Server on TCP/5432
    - Allow App Server to MQ Server on TCP/5671 (AMQPS)
    - Deny all other inter-server traffic by default
    
    
  - **Data Protection Measures:**
    
    - **Data Type:** phi  
**Protection Method:** encryption  
**Implementation:** pgcrypto for database columns containing PHI, FDE for all volumes.  
**Compliance:**
    
    - HIPAA
    
    - **Data Type:** phi  
**Protection Method:** masking  
**Implementation:** A database script to be run on sanitized production backups before restoring to non-production environments.  
**Compliance:**
    
    - HIPAA
    
    
  - **Network Security:**
    
    - **Control:** firewall  
**Implementation:** Stateful network firewalls segmenting each environment and application tier.  
**Rules:**
    
    - Default deny all inbound/outbound traffic
    
**Monitoring:** True  
    - **Control:** ids  
**Implementation:** Network-based Intrusion Detection System monitoring traffic for malicious patterns.  
**Rules:**
    
    
**Monitoring:** True  
    
  - **Security Monitoring:**
    
    - **Type:** siem  
**Implementation:** Forwarding of Windows Event Logs (including application security events) and firewall logs to a central SIEM for correlation and alerting.  
**Frequency:** real-time  
**Alerting:** True  
    - **Type:** vulnerability-scanning  
**Implementation:** Authenticated scans of all servers for known vulnerabilities.  
**Frequency:** Quarterly  
**Alerting:** True  
    
  - **Backup Security:**
    
    - **Encryption:** True
    - **Access Control:** Backups stored in a separate, access-restricted storage location with unique credentials.
    - **Offline Storage:** True
    - **Testing Frequency:** Semi-Annually
    
  - **Compliance Frameworks:**
    
    - **Framework:** hipaa  
**Applicable Environments:**
    
    - Production
    - DR
    
**Controls:**
    
    - Access Control
    - Audit Controls
    - Integrity
    - Transmission Security
    
**Audit Frequency:** Annually  
    
  
- **Network Design:**
  
  - **Network Segmentation:**
    
    - **Environment:** Production  
**Segment Type:** private  
**Purpose:** Application Tier: Hosts the Windows Service.  
**Isolation:** logical  
    - **Environment:** Production  
**Segment Type:** isolated  
**Purpose:** Data Tier: Hosts PostgreSQL and RabbitMQ. No direct access from outside the App Tier.  
**Isolation:** logical  
    - **Environment:** Production  
**Segment Type:** private  
**Purpose:** Management Tier: Hosts administrative tools and bastion/jump hosts.  
**Isolation:** logical  
    
  - **Subnet Strategy:**
    
    - **Environment:** Production  
**Subnet Type:** private  
**Cidr Block:** 10.10.10.0/24  
**Availability Zone:** Primary  
**Routing Table:** RT-App  
    - **Environment:** Production  
**Subnet Type:** database  
**Cidr Block:** 10.10.20.0/24  
**Availability Zone:** Primary  
**Routing Table:** RT-Data  
    
  - **Security Group Rules:**
    
    - **Group Name:** SG-App-Server  
**Direction:** inbound  
**Protocol:** tcp  
**Port Range:** 11112  
**Source:** DICOM-Modality-IP-Range  
**Purpose:** Allow DICOM C-STORE from modalities.  
    - **Group Name:** SG-DB-Server  
**Direction:** inbound  
**Protocol:** tcp  
**Port Range:** 5432  
**Source:** SG-App-Server  
**Purpose:** Allow PostgreSQL connections from the App Tier.  
    - **Group Name:** SG-MQ-Server  
**Direction:** inbound  
**Protocol:** tcp  
**Port Range:** 5671  
**Source:** SG-App-Server  
**Purpose:** Allow secure RabbitMQ connections from the App Tier.  
    
  - **Connectivity Requirements:**
    
    - **Source:** Production App Server  
**Destination:** Odoo Licensing API (Internet)  
**Protocol:** https.tcp.443  
**Bandwidth:** 1Mbps  
**Latency:** <500ms  
    
  - **Network Monitoring:**
    
    - **Type:** flow-logs  
**Implementation:** VPC Flow Logs or equivalent netflow data sent to SIEM.  
**Alerting:** True  
**Retention:** 90 days  
    
  - **Bandwidth Controls:**
    
    
  - **Service Discovery:**
    
    - **Method:** dns
    - **Implementation:** Internal DNS records for database and message broker endpoints.
    - **Health Checks:** False
    
  - **Environment Communication:**
    
    - **Source Environment:** Production  
**Target Environment:** DR  
**Communication Type:** replication  
**Security Controls:**
    
    - IPsec VPN Tunnel
    - Strict firewall rules
    
    
  
- **Data Management Strategy:**
  
  - **Data Isolation:**
    
    - **Environment:** Production  
**Isolation Level:** complete  
**Method:** separate-instances  
**Justification:** HIPAA requirement to prevent data spillage and unauthorized access to PHI.  
    
  - **Backup And Recovery:**
    
    - **Environment:** Production  
**Backup Frequency:** Daily (Database), Continuous (File Storage Snapshots)  
**Retention Period:** 30 days  
**Recovery Time Objective:** 4 hours  
**Recovery Point Objective:** 24 hours  
**Testing Schedule:** Semi-Annually  
    
  - **Data Masking Anonymization:**
    
    - **Environment:** Staging  
**Data Type:** PHI  
**Masking Method:** static  
**Coverage:** complete  
**Compliance:**
    
    - HIPAA
    
    - **Environment:** Testing  
**Data Type:** PHI  
**Masking Method:** static  
**Coverage:** complete  
**Compliance:**
    
    - HIPAA
    
    
  - **Migration Processes:**
    
    - **Source Environment:** Staging  
**Target Environment:** Production  
**Migration Method:** dump-restore  
**Validation:** Post-deployment smoke tests and data validation scripts.  
**Rollback Plan:** Restore pre-migration database backup and application version.  
    
  - **Retention Policies:**
    
    - **Environment:** Production  
**Data Type:** Study Data  
**Retention Period:** 7 years (default, configurable)  
**Archival Method:** Data is purged from the live system after the retention period.  
**Compliance Requirement:** As per organization's legal and regulatory requirements.  
    
  - **Data Classification:**
    
    - **Classification:** restricted  
**Handling Requirements:**
    
    - Encryption at rest and in transit
    - Strict access control
    - Auditing
    
**Access Controls:**
    
    - Role-Based Access Control
    
**Environments:**
    
    - Production
    - DR
    
    
  - **Disaster Recovery:**
    
    - **Environment:** Production  
**Dr Site:** Geographically separate data center.  
**Replication Method:** asynchronous  
**Failover Time:** < 4 hours (RTO)  
**Testing Frequency:** Annually  
    
  
- **Monitoring And Observability:**
  
  - **Monitoring Components:**
    
    - **Component:** infrastructure  
**Tool:** Prometheus w/ Windows Exporter, or existing enterprise tool (e.g., SolarWinds, Nagios)  
**Implementation:** Monitor CPU, Memory, Disk, Network on all servers.  
**Environments:**
    
    - Production
    - Staging
    
    - **Component:** logs  
**Tool:** SIEM (e.g., Splunk, ELK Stack)  
**Implementation:** Aggregate Windows Event Logs from all servers.  
**Environments:**
    
    - Production
    - Staging
    
    - **Component:** alerting  
**Tool:** Alertmanager or built-in SIEM/monitoring tool alerting.  
**Implementation:** Configured to send emails for alerts as per REQ-REP-002.  
**Environments:**
    
    - Production
    
    
  - **Environment Specific Thresholds:**
    
    - **Environment:** Production  
**Metric:** DICOM Storage Free Space  
**Warning Threshold:** < 20%  
**Critical Threshold:** < 10%  
**Justification:** Required by REQ-REP-002 to prevent service interruption due to full storage.  
    - **Environment:** Production  
**Metric:** RabbitMQ Dead Letter Queue Depth  
**Warning Threshold:** > 0  
**Critical Threshold:** > 0  
**Justification:** Required by REQ-REP-002. Any message in the DLQ requires immediate manual intervention.  
    
  - **Metrics Collection:**
    
    - **Category:** application  
**Metrics:**
    
    - Windows Service Status (Running/Stopped)
    - PostgreSQL Connectivity
    - RabbitMQ Connectivity
    - RabbitMQ Main Queue Depth
    - RabbitMQ DLQ Depth
    
**Collection Interval:** 60s  
**Retention:** 90 days  
    
  - **Health Check Endpoints:**
    
    - **Component:** DICOM Service  
**Endpoint:** Named Pipe 'DicomServiceStatusPipe'  
**Check Type:** liveness  
**Timeout:** 2s  
**Frequency:** 60s  
    
  - **Logging Configuration:**
    
    - **Environment:** Production  
**Log Level:** info  
**Destinations:**
    
    - Local Rolling File
    - Windows Event Log (for Errors)
    
**Retention:** 30 days  
**Sampling:** none  
    - **Environment:** Development  
**Log Level:** debug  
**Destinations:**
    
    - Local Rolling File
    
**Retention:** 7 days  
**Sampling:** none  
    
  - **Escalation Policies:**
    
    - **Environment:** Production  
**Severity:** critical  
**Escalation Path:**
    
    - On-Call System Administrator
    - IT Operations Manager
    
**Timeouts:**
    
    - 15 minutes
    
**Channels:**
    
    - Email
    - SMS/Pager
    
    
  - **Dashboard Configurations:**
    
    - **Dashboard Type:** operational  
**Audience:** System Administrators  
**Refresh Interval:** 1 minute  
**Metrics:**
    
    - Windows Service Status
    - DB/MQ Connectivity
    - Queue Depth (Main & DLQ)
    - DICOM Storage Free Space %
    - Server CPU/Memory Utilization
    
    
  
- **Project Specific Environments:**
  
  - **Environments:**
    
    
  - **Configuration:**
    
    
  - **Cross Environment Policies:**
    
    
  
- **Implementation Priority:**
  
  - **Component:** Production Environment Provisioning  
**Priority:** high  
**Dependencies:**
    
    
**Estimated Effort:** High  
**Risk Level:** medium  
  - **Component:** Security Controls Implementation (Firewall, Encryption)  
**Priority:** high  
**Dependencies:**
    
    - Production Environment Provisioning
    
**Estimated Effort:** High  
**Risk Level:** high  
  - **Component:** Monitoring and Alerting Configuration  
**Priority:** high  
**Dependencies:**
    
    - Production Environment Provisioning
    
**Estimated Effort:** Medium  
**Risk Level:** low  
  - **Component:** Staging and Testing Environment Provisioning  
**Priority:** medium  
**Dependencies:**
    
    
**Estimated Effort:** Medium  
**Risk Level:** low  
  - **Component:** Infrastructure as Code (IaC) Automation Scripts  
**Priority:** medium  
**Dependencies:**
    
    
**Estimated Effort:** High  
**Risk Level:** medium  
  
- **Risk Assessment:**
  
  - **Risk:** PHI Data Breach  
**Impact:** high  
**Probability:** low  
**Mitigation:** Strict enforcement of encryption (at-rest, in-transit), least-privilege access controls, network segmentation, and regular security audits.  
**Contingency Plan:** Incident response plan including containment, eradication, recovery, and mandatory breach notification procedures.  
  - **Risk:** System Outage due to Full DICOM Storage  
**Impact:** high  
**Probability:** medium  
**Mitigation:** Proactive disk space monitoring with critical alerts as defined in REQ-REP-002. Clear procedures for provisioning new storage.  
**Contingency Plan:** Emergency archival of old data or rapid provisioning of additional storage capacity.  
  - **Risk:** Data Loss due to Database or Storage Failure  
**Impact:** high  
**Probability:** low  
**Mitigation:** High-redundancy storage (RAID), daily database backups, and storage snapshots. Regular backup testing.  
**Contingency Plan:** Execute the Disaster Recovery plan, restoring from the most recent valid backup to meet RTO/RPO.  
  
- **Recommendations:**
  
  - **Category:** High Availability  
**Recommendation:** For hospital deployments, implement clustered instances for PostgreSQL and RabbitMQ, and deploy the Windows Service on multiple application servers.  
**Justification:** Provides redundancy for all critical server-side components, eliminating single points of failure and enabling zero-downtime maintenance.  
**Priority:** high  
**Implementation Notes:** Requires a network load balancer to distribute traffic to the application servers and a shared storage solution for clustered services.  
  - **Category:** Automation  
**Recommendation:** Leverage Infrastructure as Code (IaC) using the specified Ansible/PowerShell DSC from the beginning for all environments.  
**Justification:** Ensures consistency between environments, reduces manual configuration errors, and enables rapid provisioning and recovery of servers.  
**Priority:** high  
**Implementation Notes:** IaC scripts should be version-controlled in a Git repository alongside application code.  
  - **Category:** Security  
**Recommendation:** Implement a bastion host or jump box for all administrative access to production servers.  
**Justification:** Reduces the attack surface by preventing direct RDP/SSH access from the general network. All administrative sessions can be logged and monitored from a single point.  
**Priority:** high  
**Implementation Notes:** The bastion host should be placed in a separate management network segment with strict firewall rules.  
  


---

