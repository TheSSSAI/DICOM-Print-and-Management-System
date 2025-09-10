sequenceDiagram
    participant "Presentation (ViewModel)" as PresentationViewModel
    participant "Application Service" as ApplicationService
    participant "Domain Logic" as DomainLogic
    participant "Data Access (Repository)" as DataAccessRepository
    participant "PostgreSQL Database" as PostgreSQLDatabase

    activate ApplicationService
    PresentationViewModel->>ApplicationService: 1. 1. SaveDicomChangesAsync(studyId, changedTags)
    ApplicationService-->>PresentationViewModel: Success/Failure Result
    ApplicationService->>ApplicationService: 2. 2. VerifyUserIsAdmin()
    activate DomainLogic
    ApplicationService->>DomainLogic: 3. 3. ApplyChangesToWorkingCopyAsync(studyId, changedTags)
    DomainLogic-->>ApplicationService: Task completes
    DomainLogic->>DomainLogic: 3.1. 3.1. [fo-dicom] Load, modify, and save DICOM file
    ApplicationService->>DomainLogic: 4. 4. CreateAuditLogsForChanges(adminUserId, studyId, changedTags)
    DomainLogic-->>ApplicationService: List<AuditLog>
    activate DataAccessRepository
    ApplicationService->>DataAccessRepository: 5. 5. AddAuditLogsAsync(auditLogs)
    DataAccessRepository-->>ApplicationService: Task completes
    activate PostgreSQLDatabase
    DataAccessRepository->>PostgreSQLDatabase: 6. 6. INSERT INTO "AuditLog" [...]
    PostgreSQLDatabase-->>DataAccessRepository: Rows affected

    note over ApplicationService: Transactional Boundary: Standard file system operations do not participate in database transactio...

    deactivate PostgreSQLDatabase
    deactivate DataAccessRepository
    deactivate DomainLogic
    deactivate ApplicationService
