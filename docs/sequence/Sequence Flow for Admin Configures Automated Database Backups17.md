# Specification

# 1. Overview
## 1. Admin Configures Automated Database Backups
An administrator navigates to the Administration area to configure the automated database backup policy. They specify the time of day for the backup to run and the target UNC or local path for storing the backup files. The system saves this configuration to be used by a scheduled background task.

### 1.1. Diagram Id
SEQ-OPF-017

### 1.4. Type
OperationalFlow

### 1.5. Purpose
To allow administrators to manage the system's data backup strategy to meet RPO requirements.

### 1.6. Complexity
Low

### 1.7. Priority
Medium

### 1.8. Frequency
OnDemand

### 1.9. Participants

- REPO-09-PRE
- REPO-08-APC
- REPO-10-DAC
- EXT-PGSQL

### 1.10. Key Interactions

- Admin navigates to the 'Backup Configuration' page.
- Admin sets a daily backup time (e.g., 02:00) and provides a valid storage path (e.g., \\backupserver\dmps).
- Admin clicks 'Save'.
- The application service validates the path is writable.
- The configuration settings are persisted to the database (e.g., a SystemSettings table).

### 1.11. Triggers

- Admin updates and saves the backup configuration settings (FR-3.9.2.2).

### 1.12. Outcomes

- The system's automated backup schedule and location are configured and stored.
- Subsequent automated backups will use the new settings.

### 1.13. Business Rules

- The target backup location must be a writable path accessible by the service account.

### 1.14. Error Scenarios

- The specified backup path is invalid or not writable.

### 1.15. Integration Points

- PostgreSQL Database
- Network File System (for validation)


---

# 2. Details
## 2. Admin Configures Automated Database Backups
A detailed sequence for an Administrator configuring the automated database backup policy. The sequence covers UI interaction, application service orchestration, validation of the target backup path's writability by the service account, and persistence of the configuration to the database. This flow is critical for establishing the system's disaster recovery posture.

### 2.1. Diagram Id
SEQ-OPF-017

### 2.4. Participants

- **Repository Id:** REPO-09-PRE  
**Display Name:** Presentation Layer (WPF)  
**Type:** UI Layer  
**Technology:** WPF, MVVM, .NET 8  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #4CAF50
    - **Stereotype:** <<ViewModel>>
    
- **Repository Id:** REPO-08-APC  
**Display Name:** Client Application Service  
**Type:** Application Service  
**Technology:** .NET 8, C# 12  
**Order:** 2  
**Style:**
    
    - **Shape:** participant
    - **Color:** #2196F3
    - **Stereotype:** <<Service>>
    
- **Repository Id:** REPO-10-DAC  
**Display Name:** Data Access Layer  
**Type:** Data Access  
**Technology:** Entity Framework Core 8  
**Order:** 3  
**Style:**
    
    - **Shape:** participant
    - **Color:** #FFC107
    - **Stereotype:** <<Repository>>
    
- **Repository Id:** EXT-PGSQL  
**Display Name:** PostgreSQL Database  
**Type:** Database  
**Technology:** PostgreSQL 16  
**Order:** 4  
**Style:**
    
    - **Shape:** database
    - **Color:** #9C27B0
    - **Stereotype:** <<External>>
    

### 2.5. Interactions

- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-09-PRE  
**Message:** 1. Admin enters Backup Time ('02:00') and Path ('\\backupserver\dmps'), clicks 'Save'.  
**Sequence Number:** 1  
**Type:** UI Interaction  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** WPF Commanding
    - **Method:** BackupSettingsViewModel.SaveCommand.Execute()
    - **Parameters:** BackupConfigurationDto { TimeOfDay: '02:00:00', Path: '\\backupserver\dmps' }
    - **Authentication:** Requires authenticated Admin session.
    - **Error Handling:** Client-side validation for format of time and non-empty path.
    - **Performance:** UI response < 50ms.
    
- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-08-APC  
**Message:** 2. UpdateBackupSettings(dto)  
**Sequence Number:** 2  
**Type:** Service Call  
**Is Synchronous:** True  
**Return Message:** Task<Result>  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Call
    - **Method:** ISystemSettingsService.UpdateBackupSettingsAsync(BackupConfigurationDto dto)
    - **Parameters:** dto: Contains backup time and path.
    - **Authentication:** RBAC check for Administrator role performed before call.
    - **Error Handling:** Handles exceptions from lower layers and translates them into a user-friendly Result object.
    - **Performance:** Method execution should be asynchronous to not block the UI thread.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-10-DAC  
**Message:** 3. SaveBackupSettings(settings)  
**Sequence Number:** 3  
**Type:** Data Operation  
**Is Synchronous:** True  
**Return Message:** Task<Result>  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** Abstracted IPC Command
    - **Method:** ISystemSettingsRepository.SaveBackupSettingsAsync(BackupSettings settings)
    - **Parameters:** settings: A domain object containing the configuration.
    - **Authentication:** The service processing this command runs under a specific service account.
    - **Error Handling:** Propagates exceptions related to file system access or database failures.
    - **Performance:** Latency includes IPC overhead, file system check, and DB transaction.
    
- **Source Id:** REPO-10-DAC  
**Target Id:** REPO-10-DAC  
**Message:** 4. [Internal] Validate path writability  
**Sequence Number:** 4  
**Type:** File System Check  
**Is Synchronous:** True  
**Return Message:** bool  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** File System API
    - **Method:** File.Create(Path.Combine(backupPath, tempFileName))
    - **Parameters:** Generates a temporary, unique filename to write to the target path.
    - **Authentication:** This operation is executed under the security context of the background Windows Service account.
    - **Error Handling:** Catches `UnauthorizedAccessException` or `IOException`. If caught, the entire operation fails and returns an error result.
    - **Performance:** Network latency to UNC path can impact performance; timeout of 5 seconds is recommended.
    
- **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 5. UPSERT SystemSettings (BackupTime, BackupPath)  
**Sequence Number:** 5  
**Type:** Database Write  
**Is Synchronous:** True  
**Return Message:** Command Complete  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** Npgsql
    - **Method:** UPDATE "SystemSettings" SET "Value" = @p0 WHERE "Key" = @p1; INSERT INTO ... ON CONFLICT ...
    - **Parameters:** Key: 'BackupScheduleTime', Value: '02:00:00'; Key: 'BackupStoragePath', Value: '\\backupserver\dmps'
    - **Authentication:** Connection uses credentials stored securely via Windows Credential Manager.
    - **Error Handling:** Handles DbUpdateException for concurrency or connection issues.
    - **Performance:** Transaction should complete in < 50ms.
    
- **Source Id:** EXT-PGSQL  
**Target Id:** REPO-10-DAC  
**Message:** 6. Success (2 rows affected)  
**Sequence Number:** 6  
**Type:** DB Confirmation  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Npgsql
    - **Method:** 
    - **Parameters:** 
    - **Authentication:** 
    - **Error Handling:** 
    - **Performance:** 
    
- **Source Id:** REPO-10-DAC  
**Target Id:** REPO-08-APC  
**Message:** 7. Returns Success Result  
**Sequence Number:** 7  
**Type:** Return  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Abstracted IPC Response
    - **Method:** 
    - **Parameters:** Result.Success() object.
    - **Authentication:** 
    - **Error Handling:** 
    - **Performance:** 
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-09-PRE  
**Message:** 8. Returns Success Result  
**Sequence Number:** 8  
**Type:** Return  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Process Call
    - **Method:** 
    - **Parameters:** Result.Success() object.
    - **Authentication:** 
    - **Error Handling:** 
    - **Performance:** 
    
- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-09-PRE  
**Message:** 9. Display 'Settings saved successfully' notification  
**Sequence Number:** 9  
**Type:** UI Update  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** UI Framework
    - **Method:** NotificationService.ShowSuccess("Settings saved successfully")
    - **Parameters:** 
    - **Authentication:** 
    - **Error Handling:** 
    - **Performance:** 
    

### 2.6. Notes

- **Content:** Interaction 3 represents an abstracted command pattern. The Client Application Service (REPO-08-APC) likely serializes a command and publishes it via RabbitMQ to the background service. The Data Access Layer (REPO-10-DAC) represents the service-side handler that consumes this command, performs the file system validation, and executes the database transaction.  
**Position:** bottom  
**Participant Id:** REPO-08-APC  
**Sequence Number:** 3  

### 2.7. Implementation Guidance

- **Security Requirements:** Access to the backup configuration UI must be strictly limited to users with the 'Administrator' role via RBAC checks. The configuration change event MUST be recorded in the audit trail, logging the Admin's ID, the timestamp, and the new values for the backup time and path (FR-3.4.2.2). The Windows Service account must be granted 'Modify' permissions on the target backup directory.
- **Performance Targets:** The end-to-end user-facing operation (from click to notification) should complete in under 1 second for local paths and 5 seconds for UNC paths. The database transaction itself should take less than 50ms.
- **Error Handling Strategy:** If the path writability check (Step 4) fails, the operation must be aborted before any database changes are made. A specific `Result.Failure('The specified backup path is invalid or not writable by the service.')` should be returned to the UI. The system must also gracefully handle potential database connection errors or timeouts during the `UPSERT` operation (Step 5) and return a generic 'Failed to save settings' error.
- **Testing Considerations:** Unit tests should cover the validation logic, mocking the file system API to simulate success, `UnauthorizedAccessException`, and `IOException`. Integration tests must validate the database `UPSERT` logic. E2E tests should be performed with a running service, using valid local paths, valid UNC paths, invalid paths (e.g., syntactically incorrect), and valid paths where the service account lacks write permissions.
- **Monitoring Requirements:** A structured log entry at the INFO level should be generated upon successful configuration change. Any validation or database failures should be logged at the WARNING level. The audit trail event for this change is critical for compliance and change tracking.
- **Deployment Considerations:** Deployment documentation must clearly state the requirement for the Windows Service account to have appropriate write permissions on the designated backup location. The installer could optionally include a step to prompt for a service account with the necessary privileges.


---

