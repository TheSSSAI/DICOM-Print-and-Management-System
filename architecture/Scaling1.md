# Specification

# 1. Deployment Model Analysis

- **System Overview:**
  
  - **Analysis Date:** 2025-06-13
  - **System Name:** DICOM Management and Printing System
  - **Architecture Style:** Client-Server with Decoupled Background Service
  - **Technology Stack:**
    
    - .NET 8
    - WPF
    - Windows Service
    - PostgreSQL 16
    - RabbitMQ
    
  
- **Infrastructure Requirements:**
  
  - **Compute:**
    
    - **Component:** Application Server  
**Description:** Hosts the .NET 8 'DICOM Service' (Windows Service). Responsible for all background processing, C-STORE SCP, and data management.  
**Service Type:** Virtual Machine  
**Os:** Windows Server 2019 or later  
**Runtime:** .NET 8 Desktop Runtime  
    - **Component:** Database Server  
**Description:** Hosts the PostgreSQL 16 database for all application metadata.  
**Service Type:** Virtual Machine  
**Os:** Linux (Recommended) or Windows Server  
**Runtime:** PostgreSQL 16  
    - **Component:** Message Broker Server  
**Description:** Hosts the RabbitMQ instance for inter-process communication.  
**Service Type:** Virtual Machine  
**Os:** Linux (Recommended) or Windows Server  
**Runtime:** RabbitMQ, Erlang OTP  
    - **Component:** Client Workstations  
**Description:** User desktops running the WPF client application.  
**Service Type:** Physical or Virtual Desktop  
**Os:** Windows 10 (20H2+) or Windows 11  
**Runtime:** .NET 8 Desktop Runtime  
    
  - **Storage:**
    
    - **Component:** Database Storage  
**Description:** Storage for the PostgreSQL data directory. Must support Full Disk Encryption.  
**Storage Type:** Block Storage (SSD Recommended)  
**Performance:** High IOPS, Low Latency  
**Requirements:**
    
    - REQ-NFR-004: Encryption at Rest (Full Disk)
    
    - **Component:** DICOM File Storage  
**Description:** Centralized storage for all DICOM files. Must be accessible via a UNC path from the Application Server and Client Workstations.  
**Storage Type:** Network File Share (SMB/CIFS)  
**Performance:** High Throughput  
**Requirements:**
    
    - REQ-FNC-006: Hierarchical storage path
    
    - **Component:** Backup Storage  
**Description:** Storage for PostgreSQL database dumps and DICOM file archives.  
**Storage Type:** Network File Share or Backup Appliance  
**Performance:** Standard Throughput  
**Requirements:**
    
    - REQ-NFR-005: RPO of 24 hours
    
    
  - **Database:**
    
    - **Service:** PostgreSQL
    - **Version:** 16
    - **Configuration:** Must have the 'pgcrypto' extension installed and enabled (REQ-NFR-001). All connections must use TLS (REQ-NFR-004).
    
  - **Messaging:**
    
    - **Service:** RabbitMQ
    - **Version:** 3.12.x or later
    - **Configuration:** Durable queues, persistent messages, and a dead-letter exchange must be configured (REQ-TEC-002).
    
  - **Caching:**
    
    - **Service:** In-Process Memory
    - **Description:** No external caching infrastructure (e.g., Redis) is required. Caching is handled in-memory by the application.
    - **Configuration:** N/A
    
  
- **Hosting Strategy:**
  
  - **Model:** On-Premise
  - **Justification:** The system handles sensitive Protected Health Information (PHI) with strict data residency requirements (REQ-2.5.1). The requirements for an Administrator's Guide to install prerequisites (PostgreSQL, RabbitMQ) and the classic client-server architecture strongly indicate a self-hosted, on-premise deployment model.
  - **Architecture Considerations:** The architecture is a traditional N-tier system, not cloud-native. Deployment scripts (PowerShell DSC, Ansible) are recommended for consistent setup (REQ-6.1).
  - **Migration Path:** Data migration from the legacy system is handled by a dedicated utility (REQ-8.2), not a cloud migration.
  
- **Availability And Resiliency:**
  
  - **Deployment Models:**
    
    - **Model Name:** Small Clinic (Single-Node)  
**Description:** A minimal, non-redundant setup for small-scale deployments where high availability is not a primary concern. Suitable for evaluation or low-volume clinics.  
**Topology:** All server components (Application Service, PostgreSQL, RabbitMQ, File Share) co-located on a single physical or virtual server.  
**Availability:** Single point of failure.  
**Disaster Recovery:** Backup and Restore. RPO: 24 hours, RTO: 4 hours (REQ-NFR-005).  
    - **Model Name:** Hospital (High-Availability)  
**Description:** A resilient setup for larger organizations requiring high uptime for clinical operations.  
**Topology:** Distributed across multiple servers to eliminate single points of failure.  
**Availability:** Achieved via clustering and replication:
- **PostgreSQL:** Primary/Standby streaming replication.
- **RabbitMQ:** 3-node cluster with mirrored queues.
- **Application Service:** Active/Passive deployment using Windows Server Failover Clustering.
- **DICOM File Storage:** Clustered File Server (e.g., using WSFC with SOFS) or redundant NAS appliance.  
**Disaster Recovery:** Backup and Restore to a secondary site, with potential for log shipping for lower RPO.  
    
  - **Data Residency:**
    
    - **Constraint:** Mandatory
    - **Details:** All PHI data must be stored and processed within the geographical boundaries defined by the deploying organization (REQ-2.5.1). On-premise deployment inherently satisfies this.
    
  
- **Scaling And Resource Allocation:**
  
  - **Scaling Strategy:** Vertical Scaling
  - **Justification:** The architecture is centered around a 'single, consolidated background Windows Service' (REQ-TEC-002), which does not lend itself to horizontal scaling. Performance is increased by allocating more CPU, Memory, and faster storage to the servers (scaling up).
  - **Resource Allocation:**
    
    - **Application Server:** Minimum: 2 vCPU, 4 GB RAM. Recommended: 4+ vCPU, 8+ GB RAM.
    - **Database Server:** Minimum: 2 vCPU, 4 GB RAM. Recommended: 4+ vCPU, 16+ GB RAM, High IOPS Storage.
    - **Message Broker Server:** Minimum: 2 vCPU, 4 GB RAM. Recommended: 2+ vCPU, 8+ GB RAM.
    - **Client Workstation:** Minimum: 2 GHz CPU, 8 GB RAM, DirectX 11 GPU (REQ-NFR-001).
    
  
- **Networking Topology:**
  
  - **Network Segmentation:** A dedicated, isolated VLAN is recommended for the server components (Application, Database, Message Broker, File Storage) to restrict access and enhance security.
  - **Firewall Configuration:**
    
    - **Source:** Client Workstations  
**Destination:** Database Server  
**Port Protocol:** TCP 5432 (PostgreSQL over TLS)  
**Notes:** Allow clients to query the database.  
    - **Source:** Client Workstations  
**Destination:** Message Broker Server  
**Port Protocol:** TCP 5671 (AMQPS)  
**Notes:** Allow clients to publish tasks.  
    - **Source:** Application Server  
**Destination:** Database Server  
**Port Protocol:** TCP 5432 (PostgreSQL over TLS)  
**Notes:** Allow service to perform DB operations.  
    - **Source:** Modalities (Medical Devices)  
**Destination:** Application Server  
**Port Protocol:** TCP 11112 (Configurable DICOM Port)  
**Notes:** Allow C-STORE operations.  
    - **Source:** Application Server  
**Destination:** External PACS  
**Port Protocol:** TCP (Configurable DICOM Ports)  
**Notes:** Allow C-FIND/C-MOVE/C-ECHO operations.  
    - **Source:** Client Workstations, Application Server  
**Destination:** Internet (Odoo API)  
**Port Protocol:** TCP 443 (HTTPS)  
**Notes:** Allow license validation.  
    - **Source:** Client Workstations, Application Server  
**Destination:** SMTP Server  
**Port Protocol:** TCP 587 (SMTPS/STARTTLS)  
**Notes:** Allow sending emails.  
    - **Source:** Client Workstations, Application Server  
**Destination:** DICOM File Storage Server  
**Port Protocol:** TCP 445 (SMB)  
**Notes:** Allow access to DICOM files.  
    
  - **Dns:** Internal DNS is required for resolving the hostnames of the database, message broker, and file storage servers.
  
- **Security Implementation:**
  
  - **Secrets Management:**
    
    - **Tool:** Windows Credential Manager
    - **Justification:** Explicitly required by REQ-NFR-004 to avoid storing sensitive credentials (database, SMTP, API keys) in plaintext configuration files. Credentials will be stored on the Application Server and Client Workstations.
    
  - **Identity And Access:** Infrastructure access should be controlled via Active Directory groups. Application access is managed by the application's internal RBAC model.
  - **Data Protection:**
    
    - **In Transit:** TLS is mandatory for all connections to the PostgreSQL database (REQ-NFR-004) and Odoo Licensing API (REQ-INT-004). Support for DICOM TLS and SMTPS/STARTTLS is also required.
    - **At Rest:** Full Disk Encryption is required for the database server. Column-level encryption using pgcrypto is also required for specific PHI fields (REQ-NFR-004).
    
  - **Compliance:** The on-premise deployment model with network segmentation, encryption, and secure credential management directly supports the system's HIPAA compliance requirements.
  


---

