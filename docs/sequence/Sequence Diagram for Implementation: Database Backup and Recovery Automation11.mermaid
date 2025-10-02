sequenceDiagram
    participant "Administrator / Scheduler" as AdministratorScheduler
    participant "DMPS Windows Service" as DMPSWindowsService
    participant "PostgreSQL Database" as PostgreSQLDatabase
    participant "Backup Storage System" as BackupStorageSystem

    activate AdministratorScheduler
    AdministratorScheduler->>AdministratorScheduler: 1. Initiate Backup Procedure (Scheduled or Manual)
    AdministratorScheduler->>PostgreSQLDatabase: 2. Execute pg_dump to create a database backup file.
    PostgreSQLDatabase-->>AdministratorScheduler: Returns exit code and backup file stream.
    AdministratorScheduler->>AdministratorScheduler: 3. Compress the generated backup file (e.g., gzip).
    AdministratorScheduler-->>AdministratorScheduler: Compressed file is created.
    AdministratorScheduler->>BackupStorageSystem: 4. Copy compressed backup file to secure, off-host location.
    BackupStorageSystem-->>AdministratorScheduler: Confirmation of successful transfer.
    AdministratorScheduler->>AdministratorScheduler: 5. Log successful backup completion and clean up temporary files.
    AdministratorScheduler->>AdministratorScheduler: 6. Initiate Restore Procedure (Manual trigger due to system failure/disaster event).
    AdministratorScheduler->>DMPSWindowsService: 7. Stop the 'DICOM Service' to release database connections.
    DMPSWindowsService-->>AdministratorScheduler: Confirmation that service is in 'Stopped' state.
    AdministratorScheduler->>BackupStorageSystem: 8. Retrieve and decompress the latest valid backup file.
    BackupStorageSystem-->>AdministratorScheduler: Provides decompressed backup file to a temporary local path.
    AdministratorScheduler->>PostgreSQLDatabase: 9. Prepare database for restore (DROP/CREATE).
    PostgreSQLDatabase-->>AdministratorScheduler: Confirmation that database is clean and ready.
    AdministratorScheduler->>PostgreSQLDatabase: 10. Execute pg_restore using the retrieved backup file.
    PostgreSQLDatabase-->>AdministratorScheduler: Returns exit code.
    AdministratorScheduler->>DMPSWindowsService: 11. Start the 'DICOM Service'.
    DMPSWindowsService-->>AdministratorScheduler: Confirmation that service is in 'Running' state.
    AdministratorScheduler->>PostgreSQLDatabase: 12. Execute post-restore validation script to verify data integrity.
    PostgreSQLDatabase-->>AdministratorScheduler: Returns validation report (e.g., record counts).
    AdministratorScheduler->>AdministratorScheduler: 13. Log successful restore, archive validation report, and communicate system availability.


    deactivate AdministratorScheduler
