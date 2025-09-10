# Specification

# 1. Overview
## 1. Database Backup and Recovery Procedure
An administrator executes a documented procedure for database backup or recovery. For backup, pgdump is used to create a file. For recovery, services are stopped, the database is restored using pgrestore, services are restarted, and a validation report is checked to ensure data integrity, meeting the defined RPO and RTO.

### 1.1. Diagram Id
SEQ-REC-011

### 1.4. Type
RecoveryFlow

### 1.5. Purpose
To ensure business continuity and data durability by providing a reliable mechanism to back up and restore critical application data.

### 1.6. Complexity
Medium

### 1.7. Priority
Critical

### 1.8. Frequency
Daily

### 1.9. Participants

- EXT-PGSQL
- EXT-OS

### 1.10. Key Interactions

- Backup: A scheduled script or manual command executes pg_dump.
- Backup: The dump file is compressed and stored in a secure, separate backup location.
- Restore: The application services are stopped.
- Restore: The existing database is dropped/cleared.
- Restore: The pg_restore command is executed with the backup file as input.
- Restore: The application services are restarted and data is verified.

### 1.11. Triggers

- A scheduled backup job runs.
- A system failure requires data restoration.

### 1.12. Outcomes

- A consistent, point-in-time backup of the database is created.
- The system's data is successfully restored to the state of the last valid backup.

### 1.13. Business Rules

- Recovery Point Objective (RPO) is 24 hours (REQ-NFR-005).
- Recovery Time Objective (RTO) is 4 hours (REQ-NFR-005).
- Procedures must be documented and periodically tested.
- A post-restore validation report must be generated to compare record counts and ensure data integrity (REQ-8.2).

### 1.14. Error Scenarios

- Backup file is corrupted and cannot be restored.
- Restore process fails due to version mismatch or other errors.

### 1.15. Integration Points

- PostgreSQL Database
- Backup Storage System


---

# 2. Details
## 2. Implementation: Database Backup and Recovery Automation
Provides a detailed technical sequence for automated database backup and manual, documented recovery procedures. The backup flow details the execution of pg_dump, compression, and secure storage. The recovery flow specifies the pre-requisite service shutdown, database restoration via pg_restore, service restart, and mandatory post-restore data validation to meet RTO/RPO objectives.

### 2.1. Diagram Id
SEQ-REC-011-IMPL

### 2.4. Participants

- **Repository Id:** ADMIN_ACTOR  
**Display Name:** Administrator / Scheduler  
**Type:** External Actor/System  
**Technology:** Windows Task Scheduler / Cron / CLI  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #90CAF9
    - **Stereotype:** OS
    
- **Repository Id:** dicom-windows-service-002  
**Display Name:** DMPS Windows Service  
**Type:** Application Service  
**Technology:** .NET 8, Microsoft.Extensions.Hosting  
**Order:** 2  
**Style:**
    
    - **Shape:** participant
    - **Color:** #A5D6A7
    - **Stereotype:** Service
    
- **Repository Id:** POSTGRESQL_DB  
**Display Name:** PostgreSQL Database  
**Type:** Database  
**Technology:** PostgreSQL 16  
**Order:** 3  
**Style:**
    
    - **Shape:** database
    - **Color:** #FFCC80
    - **Stereotype:** External
    
- **Repository Id:** BACKUP_STORAGE  
**Display Name:** Backup Storage System  
**Type:** Storage  
**Technology:** Network Share (UNC) / S3 Bucket / NAS  
**Order:** 4  
**Style:**
    
    - **Shape:** cloud
    - **Color:** #CE93D8
    - **Stereotype:** External
    

### 2.5. Interactions

- **Source Id:** ADMIN_ACTOR  
**Target Id:** ADMIN_ACTOR  
**Message:** Initiate Backup Procedure (Scheduled or Manual)  
**Sequence Number:** 1  
**Type:** System Trigger  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** OS Scheduler
    - **Method:** Execute Backup Script
    - **Parameters:**
      
      
    - **Authentication:** Service Account with necessary permissions
    - **Error Handling:** Log script execution failure to system logs.
    - **Performance:** RPO requires this to run at least every 24 hours.
    
- **Source Id:** ADMIN_ACTOR  
**Target Id:** POSTGRESQL_DB  
**Message:** Execute `pg_dump` to create a database backup file.  
**Sequence Number:** 2  
**Type:** CLI Command  
**Is Synchronous:** True  
**Return Message:** Returns exit code and backup file stream.  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** CLI / PostgreSQL Wire Protocol
    - **Method:** pg_dump
    - **Parameters:**
      
      - Command: `pg_dump -U [user] -h [host] --format=c -f [temp_path] [db_name]`
      - Authentication via `.pgpass` or environment variables.
      
    - **Authentication:** Database user with read-only or backup privileges.
    - **Error Handling:** Check command exit code. Any non-zero value indicates failure. Log `stderr` for diagnostics.
    - **Performance:** Duration depends on DB size. Must complete within the backup window.
    
- **Source Id:** ADMIN_ACTOR  
**Target Id:** ADMIN_ACTOR  
**Message:** Compress the generated backup file (e.g., gzip).  
**Sequence Number:** 3  
**Type:** Local Operation  
**Is Synchronous:** True  
**Return Message:** Compressed file is created.  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Filesystem
    - **Method:** gzip / 7-Zip
    - **Parameters:**
      
      - Input: [temp_path]
      - Output: [temp_path].gz
      
    - **Authentication:** N/A
    - **Error Handling:** Verify file existence and non-zero size post-compression.
    - **Performance:** CPU-bound operation.
    
- **Source Id:** ADMIN_ACTOR  
**Target Id:** BACKUP_STORAGE  
**Message:** Copy compressed backup file to secure, off-host location.  
**Sequence Number:** 4  
**Type:** File Transfer  
**Is Synchronous:** True  
**Return Message:** Confirmation of successful transfer.  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** SMB, NFS, S3 API
    - **Method:** robocopy / scp / aws s3 cp
    - **Parameters:**
      
      - Source: [temp_path].gz
      - Destination: [secure_backup_path]
      
    - **Authentication:** Service account, SSH key, or IAM role.
    - **Error Handling:** Verify file transfer by comparing checksums (e.g., md5sum) of source and destination files.
    - **Performance:** Network-bound operation.
    
- **Source Id:** ADMIN_ACTOR  
**Target Id:** ADMIN_ACTOR  
**Message:** Log successful backup completion and clean up temporary files.  
**Sequence Number:** 5  
**Type:** Logging & Cleanup  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Filesystem / Logging Framework
    - **Method:** Write to Log File, Delete Temp File
    - **Parameters:**
      
      - Log Message: Backup successful, File: [filename], Size: [size], Checksum: [checksum]
      
    - **Authentication:** N/A
    - **Error Handling:** Log cleanup failures as warnings.
    - **Performance:** Low impact.
    
- **Source Id:** ADMIN_ACTOR  
**Target Id:** ADMIN_ACTOR  
**Message:** Initiate Restore Procedure (Manual trigger due to system failure/disaster event).  
**Sequence Number:** 6  
**Type:** System Trigger  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** Manual CLI
    - **Method:** Execute Restore Script
    - **Parameters:**
      
      
    - **Authentication:** Administrator credentials.
    - **Error Handling:** Procedure must only be executed after authorization.
    - **Performance:** RTO of 4 hours begins from this point.
    
- **Source Id:** ADMIN_ACTOR  
**Target Id:** dicom-windows-service-002  
**Message:** Stop the 'DICOM Service' to release database connections.  
**Sequence Number:** 7  
**Type:** Service Control  
**Is Synchronous:** True  
**Return Message:** Confirmation that service is in 'Stopped' state.  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Windows Service Control Manager
    - **Method:** net stop "DICOM Service"
    - **Parameters:**
      
      
    - **Authentication:** Local Administrator privileges.
    - **Error Handling:** If service fails to stop, script must terminate. Forceful termination may be required.
    - **Performance:** Should complete within 30 seconds.
    
- **Source Id:** ADMIN_ACTOR  
**Target Id:** BACKUP_STORAGE  
**Message:** Retrieve and decompress the latest valid backup file.  
**Sequence Number:** 8  
**Type:** File Transfer  
**Is Synchronous:** True  
**Return Message:** Provides decompressed backup file to a temporary local path.  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** SMB, NFS, S3 API
    - **Method:** copy / gunzip
    - **Parameters:**
      
      - Source: [secure_backup_path]
      - Destination: [local_restore_path]
      
    - **Authentication:** Service account, SSH key, or IAM role.
    - **Error Handling:** Validate checksum of retrieved file against stored checksum. If corrupt, attempt to retrieve the next most recent backup.
    - **Performance:** Network-bound operation.
    
- **Source Id:** ADMIN_ACTOR  
**Target Id:** POSTGRESQL_DB  
**Message:** Prepare database for restore (DROP/CREATE).  
**Sequence Number:** 9  
**Type:** DB Administration  
**Is Synchronous:** True  
**Return Message:** Confirmation that database is clean and ready.  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** SQL Client (psql)
    - **Method:** DROP DATABASE [db_name]; CREATE DATABASE [db_name] OWNER [owner];
    - **Parameters:**
      
      
    - **Authentication:** Database superuser or owner privileges.
    - **Error Handling:** Terminate all active connections to the database before dropping. Script must fail if connections cannot be terminated.
    - **Performance:** Fast operation.
    
- **Source Id:** ADMIN_ACTOR  
**Target Id:** POSTGRESQL_DB  
**Message:** Execute `pg_restore` using the retrieved backup file.  
**Sequence Number:** 10  
**Type:** CLI Command  
**Is Synchronous:** True  
**Return Message:** Returns exit code.  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** CLI / PostgreSQL Wire Protocol
    - **Method:** pg_restore
    - **Parameters:**
      
      - Command: `pg_restore -U [user] -h [host] -d [db_name] --clean --if-exists [local_restore_path]`
      
    - **Authentication:** Database superuser or owner privileges.
    - **Error Handling:** Check command exit code. Non-zero indicates failure. Log `stderr`. Do not proceed if restore fails.
    - **Performance:** Major contributor to RTO. Depends on DB size and hardware performance.
    
- **Source Id:** ADMIN_ACTOR  
**Target Id:** dicom-windows-service-002  
**Message:** Start the 'DICOM Service'.  
**Sequence Number:** 11  
**Type:** Service Control  
**Is Synchronous:** True  
**Return Message:** Confirmation that service is in 'Running' state.  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Windows Service Control Manager
    - **Method:** net start "DICOM Service"
    - **Parameters:**
      
      
    - **Authentication:** Local Administrator privileges.
    - **Error Handling:** If service fails to start, check application event logs for errors. The restore is considered failed.
    - **Performance:** Should complete within 30 seconds.
    
- **Source Id:** ADMIN_ACTOR  
**Target Id:** POSTGRESQL_DB  
**Message:** Execute post-restore validation script to verify data integrity.  
**Sequence Number:** 12  
**Type:** Data Validation  
**Is Synchronous:** True  
**Return Message:** Returns validation report (e.g., record counts).  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** SQL Client (psql)
    - **Method:** Execute Validation SQL Script
    - **Parameters:**
      
      - Script: `SELECT 'users', COUNT(*) FROM users UNION ALL SELECT 'studies', COUNT(*) FROM studies;`
      
    - **Authentication:** Database user with read permissions.
    - **Error Handling:** Compare returned counts against pre-disaster benchmarks or expected values. Any discrepancy indicates a failed restore and requires incident escalation.
    - **Performance:** Should be a fast operation.
    
- **Source Id:** ADMIN_ACTOR  
**Target Id:** ADMIN_ACTOR  
**Message:** Log successful restore, archive validation report, and communicate system availability.  
**Sequence Number:** 13  
**Type:** Logging & Communication  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Logging / Email
    - **Method:** Write to Log File, Send Notification
    - **Parameters:**
      
      - Log Message: Restore successful, Validation: [OK/Fail], RTO: [duration]
      
    - **Authentication:** N/A
    - **Error Handling:** Failure to communicate does not invalidate the restore but must be handled separately.
    - **Performance:** Low impact.
    

### 2.6. Notes

- **Content:** --- BACKUP PROCEDURE ---  
**Position:** top  
**Participant Id:** None  
**Sequence Number:** 1  
- **Content:** --- RESTORE PROCEDURE ---  
**Position:** top  
**Participant Id:** None  
**Sequence Number:** 6  

### 2.7. Implementation Guidance

- **Security Requirements:** Backup files must be encrypted at rest on the Backup Storage System. Access to the storage system and the ability to execute restore procedures must be strictly limited to authorized administrators. Database credentials for scripts must be managed securely (e.g., Windows Credential Manager, .pgpass with strict permissions), not stored in plaintext within scripts.
- **Performance Targets:** Recovery Point Objective (RPO) is 24 hours, meaning backups must be successfully completed at least daily. Recovery Time Objective (RTO) is 4 hours, measured from the start of the restore procedure (Step 6) to the confirmation of system availability (Step 13). Restore script performance should be benchmarked during tests.
- **Error Handling Strategy:** Backup script failures must trigger a critical alert to administrators. For restore, if a backup file is corrupt, the script must automatically attempt to use the previous day's backup. If the `pg_restore` command fails, the entire procedure must be halted, and the application services must NOT be started. A failed data validation (Step 12) is considered a failed restore and must trigger an immediate incident response.
- **Testing Considerations:** The entire restore procedure must be tested on a non-production environment at least semi-annually to validate backup integrity and ensure the 4-hour RTO can be met. Test results must be documented.
- **Monitoring Requirements:** All backup and restore attempts (success and failure) must be logged to a secure, append-only log. The available capacity of the Backup Storage System must be monitored to prevent backup failures due to insufficient space. An automated alert must be configured for any backup script failure.
- **Deployment Considerations:** All backup and restore scripts must be stored in source control (Git). The machine designated for running these procedures must have the correct version of PostgreSQL client tools (`pg_dump`, `pg_restore`) installed and have network access to all participants (DB, Service Host, Storage).


---

