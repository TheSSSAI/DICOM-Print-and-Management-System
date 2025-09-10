sequenceDiagram
    participant "Presentation Layer (WPF)" as PresentationLayerWPF
    actor "Client Application Service" as ClientApplicationService
    participant "Data Access Layer" as DataAccessLayer
    participant "PostgreSQL Database" as PostgreSQLDatabase

    activate PresentationLayerWPF
    PresentationLayerWPF->>PresentationLayerWPF: 1. 1. Admin enters Backup Time ('02:00') and Path ('\\backupserver\dmps'), clicks 'Save'.
    activate ClientApplicationService
    PresentationLayerWPF->>ClientApplicationService: 2. 2. UpdateBackupSettings(dto)
    ClientApplicationService-->>PresentationLayerWPF: Task<Result>
    activate DataAccessLayer
    ClientApplicationService->>DataAccessLayer: 3. 3. SaveBackupSettings(settings)
    DataAccessLayer-->>ClientApplicationService: Task<Result>
    DataAccessLayer->>DataAccessLayer: 4. 4. [Internal] Validate path writability
    DataAccessLayer-->>DataAccessLayer: bool
    activate PostgreSQLDatabase
    DataAccessLayer->>PostgreSQLDatabase: 5. 5. UPSERT SystemSettings (BackupTime, BackupPath)
    PostgreSQLDatabase-->>DataAccessLayer: Command Complete
    PostgreSQLDatabase->>DataAccessLayer: 6. 6. Success (2 rows affected)
    DataAccessLayer->>ClientApplicationService: 7. 7. Returns Success Result
    ClientApplicationService->>PresentationLayerWPF: 8. 8. Returns Success Result
    PresentationLayerWPF->>PresentationLayerWPF: 9. 9. Display 'Settings saved successfully' notification

    note over ClientApplicationService: Interaction 3 represents an abstracted command pattern. The Client Application Service (REPO-08-A...

    deactivate PostgreSQLDatabase
    deactivate DataAccessLayer
    deactivate ClientApplicationService
    deactivate PresentationLayerWPF
