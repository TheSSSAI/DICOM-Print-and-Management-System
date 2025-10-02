sequenceDiagram
    actor "Presentation Layer (Client)" as PresentationLayerClient
    actor "Application Service (Client)" as ApplicationServiceClient
    participant "Windows Service (Backend)" as WindowsServiceBackend
    participant "Data Access Layer (Service)" as DataAccessLayerService
    participant "PostgreSQL Database" as PostgreSQLDatabase
    participant "RabbitMQ Broker" as RabbitMQBroker

    activate ApplicationServiceClient
    PresentationLayerClient->>ApplicationServiceClient: 1. 1. User initiates Drag-and-Drop: Triggers HandleFileDropAsync(string[] filePaths)
    ApplicationServiceClient->>ApplicationServiceClient: 2. 2. [Loop] Validate files and extract Study UIDs
    ApplicationServiceClient-->>ApplicationServiceClient: List of valid DicomObjects and rejected file reports.
    activate WindowsServiceBackend
    ApplicationServiceClient->>WindowsServiceBackend: 3. 3. [Loop] Check for duplicate Study Instance UID
    WindowsServiceBackend-->>ApplicationServiceClient: 4. Returns {"exists": true/false}
    activate DataAccessLayerService
    WindowsServiceBackend->>DataAccessLayerService: 3.1. 3.1. StudyExistsAsync(studyUid)
    DataAccessLayerService-->>WindowsServiceBackend: Returns boolean.
    DataAccessLayerService->>PostgreSQLDatabase: 3.2. 3.2. SELECT 1 FROM "Studies" WHERE "StudyInstanceUid" = @p0 LIMIT 1
    PostgreSQLDatabase-->>DataAccessLayerService: Returns 1 row or 0 rows.
    ApplicationServiceClient->>PresentationLayerClient: 5. 5. If duplicate found, show conflict resolution dialog (FR-3.6.1.1)
    PresentationLayerClient-->>ApplicationServiceClient: 6. Returns user's choice (Overwrite, Discard, Save as New).
    ApplicationServiceClient->>RabbitMQBroker: 7. 7. For valid studies, publish ProcessDicomStoreCommand
    ApplicationServiceClient->>PresentationLayerClient: 8. 8. Display non-blocking notification: 'Import Queued'
    activate RabbitMQBroker
    WindowsServiceBackend->>RabbitMQBroker: 9. 9. Consume ProcessDicomStoreCommand message
    WindowsServiceBackend->>WindowsServiceBackend: 10. 10. Move files from staging to permanent storage path
    WindowsServiceBackend->>DataAccessLayerService: 11. 11. Call AddStudyAsync(study) within a transaction
    DataAccessLayerService->>PostgreSQLDatabase: 12. 12. Execute Transaction: BEGIN; INSERT...; COMMIT;
    PostgreSQLDatabase-->>DataAccessLayerService: Transaction success/failure.
    WindowsServiceBackend->>RabbitMQBroker: 13. 13. Acknowledge message (BasicAck) on success

    note over ApplicationServiceClient: CRITICAL: The duplicate check in step 3 must be synchronous to provide immediate feedback to the ...
    note over WindowsServiceBackend: The processing logic from step 9 onwards is identical to the C-STORE SCP ingestion path (SEQ-EVP-...
    note over WindowsServiceBackend: On processing failure (e.g., disk full, DB error), the message will be NACK'd. After configured r...

    deactivate RabbitMQBroker
    deactivate DataAccessLayerService
    deactivate WindowsServiceBackend
    deactivate ApplicationServiceClient
