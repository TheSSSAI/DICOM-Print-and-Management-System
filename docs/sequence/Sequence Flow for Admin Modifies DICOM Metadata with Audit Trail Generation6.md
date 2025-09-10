# Specification

# 1. Overview
## 1. Auditing of DICOM Metadata Modification
An administrator enables edit mode and modifies a DICOM tag, which creates a temporary 'working copy' of the study. When the changes are saved, the system records the modification in the audit trail, capturing the user, timestamp, original value, and new value for the changed tag, ensuring a compliant record of PHI changes.

### 1.1. Diagram Id
SEQ-CPL-006

### 1.4. Type
ComplianceFlow

### 1.5. Purpose
To maintain a compliant and secure record of all changes made to Protected Health Information (PHI), as required by HIPAA.

### 1.6. Complexity
Medium

### 1.7. Priority
High

### 1.8. Frequency
OnDemand

### 1.9. Participants

- REPO-09-PRE
- REPO-08-APC
- REPO-03-LOG
- REPO-10-DAC
- EXT-PGSQL

### 1.10. Key Interactions

- Admin enables 'Edit Mode' for a study.
- WorkingCopyManager creates a copy of the DICOM files.
- Admin modifies a tag value in the UI and clicks 'Save'.
- The change is applied to the working copy DICOM file.
- An AuditLog entry is created with event type 'Metadata Modified'.
- The details JSONB field stores the Tag, Original Value, and New Value.
- The audit record is persisted to the PostgreSQL database.

### 1.11. Triggers

- An Admin saves a change to a DICOM tag (FR-3.4.2.2).

### 1.12. Outcomes

- The DICOM metadata change is saved to a working copy.
- A detailed, immutable audit record of the change is created.

### 1.13. Business Rules

- Original DICOM files must never be modified (REQ-NFR-003).
- All modifications to DICOM metadata must be recorded in an audit trail (FR-3.4.2.2).
- There shall be only one 'working copy' version of a study at any time (REQ-NFR-003).

### 1.14. Error Scenarios

- Failed to write to the working copy file on disk.
- Failed to write the audit log record to the database.

### 1.15. Integration Points

- Local File System
- PostgreSQL Database


---

# 2. Details
## 2. Admin Modifies DICOM Metadata with Audit Trail Generation
This sequence details the process for an Administrator modifying DICOM metadata, which is a critical compliance event. The system ensures compliance with HIPAA and REQ-NFR-003 by never altering original files. Instead, it applies changes to a temporary 'working copy' and creates an immutable, detailed audit record for each modification. The successful persistence of the audit log is the final commit step of the entire operation.

### 2.1. Diagram Id
SEQ-CPL-006

### 2.4. Participants

- **Repository Id:** REPO-09-PRE  
**Display Name:** Presentation (ViewModel)  
**Type:** UI Layer  
**Technology:** WPF / .NET 8  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #4CAF50
    - **Stereotype:** <<ViewModel>>
    
- **Repository Id:** REPO-08-APC  
**Display Name:** Application Service  
**Type:** Service Layer  
**Technology:** .NET 8  
**Order:** 2  
**Style:**
    
    - **Shape:** component
    - **Color:** #2196F3
    - **Stereotype:** <<Service>>
    
- **Repository Id:** REPO-03-LOG  
**Display Name:** Domain Logic  
**Type:** Domain Layer  
**Technology:** .NET 8 / fo-dicom  
**Order:** 3  
**Style:**
    
    - **Shape:** component
    - **Color:** #FFC107
    - **Stereotype:** <<Domain>>
    
- **Repository Id:** REPO-10-DAC  
**Display Name:** Data Access (Repository)  
**Type:** Repository Layer  
**Technology:** Entity Framework Core 8  
**Order:** 4  
**Style:**
    
    - **Shape:** component
    - **Color:** #9C27B0
    - **Stereotype:** <<Repository>>
    
- **Repository Id:** EXT-PGSQL  
**Display Name:** PostgreSQL Database  
**Type:** External Database  
**Technology:** PostgreSQL 16  
**Order:** 5  
**Style:**
    
    - **Shape:** database
    - **Color:** #795548
    - **Stereotype:** <<Database>>
    

### 2.5. Interactions

- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-08-APC  
**Message:** 1. SaveDicomChangesAsync(studyId, changedTags)  
**Sequence Number:** 1  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** Success/Failure Result  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** Direct method call
    - **Method:** async Task<Result> SaveDicomChangesAsync(Guid studyId, List<DicomTagChange> changedTags)
    - **Parameters:** studyId: The unique identifier for the study being edited. changedTags: A collection of objects containing TagID, OriginalValue, and NewValue.
    - **Authentication:** Requires authenticated Admin session context.
    - **Error Handling:** Catches exceptions from service layer and translates to user-friendly UI notifications.
    - **Performance:** Initiation should be < 50ms.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-08-APC  
**Message:** 2. VerifyUserIsAdmin()  
**Sequence Number:** 2  
**Type:** Security Check  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Internal method call
    - **Method:** void VerifyUserIsAdmin()
    - **Parameters:** Relies on current user session state provided by IAuthenticationService.
    - **Authentication:** This is the primary authorization checkpoint for this entire workflow.
    - **Error Handling:** Throws SecurityException or UnauthorizedAccessException if the current user is not in the 'Administrator' role.
    - **Performance:** Negligible.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-03-LOG  
**Message:** 3. ApplyChangesToWorkingCopyAsync(studyId, changedTags)  
**Sequence Number:** 3  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** Task completes  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** Direct method call
    - **Method:** async Task ApplyChangesToWorkingCopyAsync(Guid studyId, List<DicomTagChange> changedTags)
    - **Parameters:** Delegated from the initial service call.
    - **Authentication:** N/A
    - **Error Handling:** Propagates exceptions related to file system access (e.g., IOException, UnauthorizedAccessException) or DICOM parsing errors.
    - **Performance:** Dependent on file size and disk I/O, but should be < 200ms for typical studies.
    
**Nested Interactions:**
    
    - **Source Id:** REPO-03-LOG  
**Target Id:** REPO-03-LOG  
**Message:** 3.1. [fo-dicom] Load, modify, and save DICOM file  
**Sequence Number:** 3.1  
**Type:** Internal Processing  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Library call
    - **Method:** DicomFile.Open(...), dataset.AddOrUpdate(...), file.Save(...)
    - **Parameters:** Path to the working copy file, DICOM tags and new values.
    - **Authentication:** File system permissions apply.
    - **Error Handling:** Throws exceptions on file not found, access denied, disk full, or invalid DICOM data.
    - **Performance:** Directly tied to disk speed and file size.
    
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-03-LOG  
**Message:** 4. CreateAuditLogsForChanges(adminUserId, studyId, changedTags)  
**Sequence Number:** 4  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** List<AuditLog>  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** Direct method call
    - **Method:** List<AuditLog> CreateAuditLogsForChanges(Guid adminUserId, Guid studyId, List<DicomTagChange> changedTags)
    - **Parameters:** User ID of the acting admin, study ID, and the list of changes.
    - **Authentication:** N/A
    - **Error Handling:** Should not fail unless input is invalid.
    - **Performance:** Negligible, in-memory object creation.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-10-DAC  
**Message:** 5. AddAuditLogsAsync(auditLogs)  
**Sequence Number:** 5  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** Task completes  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** Direct method call (via interface)
    - **Method:** async Task AddAuditLogsAsync(List<AuditLog> auditLogs)
    - **Parameters:** The list of fully-formed AuditLog entities to be persisted.
    - **Authentication:** Database connection credentials used.
    - **Error Handling:** Propagates DbUpdateException on database write failure. This is a critical failure for the entire operation.
    - **Performance:** Target < 100ms for batch insert.
    
- **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 6. INSERT INTO "AuditLog" [...]  
**Sequence Number:** 6  
**Type:** Database Query  
**Is Synchronous:** True  
**Return Message:** Rows affected  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** SQL/TCP
    - **Method:** INSERT
    - **Parameters:** The values for each audit log record, with the 'details' field being a serialized JSONB object like `{"tag": "(0008,103E)", "originalValue": "OLD_DESC", "newValue": "NEW_DESC"}`.
    - **Authentication:** Connection uses credentials managed by the service.
    - **Error Handling:** Database can raise constraint violations, connection errors, or other standard SQL errors.
    - **Performance:** Performance is critical. The AuditLog table must be indexed appropriately, especially on timestamp and userId.
    

### 2.6. Notes

- **Content:** Compliance Criticality: The persistence of the audit log (Step 5 & 6) is the commit point for the entire operation. If this step fails, the entire save operation must be considered a failure, and the user must be notified. An unaudited modification of PHI is a severe compliance violation.  
**Position:** bottom  
**Participant Id:** None  
**Sequence Number:** 6  
- **Content:** Transactional Boundary: Standard file system operations do not participate in database transactions. Therefore, a failure at step 5 after step 3 has succeeded leaves the system in a temporarily inconsistent state (file changed, but not audited). The error handling strategy must prioritize alerting and preventing further action until the audit is successfully recorded.  
**Position:** top  
**Participant Id:** REPO-08-APC  
**Sequence Number:** 5  

### 2.7. Implementation Guidance

- **Security Requirements:** Access to this entire flow MUST be restricted to users with the 'Administrator' role, enforced at the Application Service layer (Step 2). The 'AuditLog' table in the database must have strict write permissions. The JSONB 'details' field must be properly escaped/serialized to prevent injection vulnerabilities if ever rendered directly.
- **Performance Targets:** The entire end-to-end operation from the user's click to the success response should be completed in under 500ms to ensure a responsive user experience. Database inserts for audit logs should be batched if multiple tags are changed in one save operation.
- **Error Handling Strategy:** If the security check (Step 2) fails, a `SecurityException` must be thrown and handled globally. If the file write (Step 3) fails, the exception should be caught by the Application Service, logged, and the user should be notified with an error. If the database write (Step 5/6) fails, this is a CRITICAL error. The system must log this failure with high severity, alert an administrator, and inform the user that the save failed and must be retried. The system must not proceed in a state where a change is saved but not audited.
- **Testing Considerations:** 1. Test the Admin role enforcement. 2. Test file write failures (e.g., mock a disk full `IOException`). 3. Test database write failures (e.g., mock a `DbUpdateException`) and verify the system's critical error response. 4. Validate the structure and content of the JSONB 'details' field in the database. 5. Verify that the original DICOM files are never touched.
- **Monitoring Requirements:** A high rate of audit log persistence failures (errors in Step 5/6) should trigger a high-priority alert to system administrators, as it indicates a potential database issue or a systemic problem preventing compliance records from being written.
- **Deployment Considerations:** The `AuditLog` table schema, including appropriate indexes on `eventTimestamp` and `userId`, must be created via an Entity Framework Core migration before this feature is deployed.


---

