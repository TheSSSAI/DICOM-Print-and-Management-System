sequenceDiagram
    participant "External DICOM Modality" as ExternalDICOMModality
    participant "DMPS Windows Service" as DMPSWindowsService
    participant "Infrastructure Layer" as InfrastructureLayer
    participant "RabbitMQ Broker" as RabbitMQBroker
    participant "Data Access Layer" as DataAccessLayer
    participant "PostgreSQL Database" as PostgreSQLDatabase

    activate DMPSWindowsService
    ExternalDICOMModality->>DMPSWindowsService: 1. 1. [C-STORE-RQ] Initiate DICOM Association & send DICOM objects.
    DMPSWindowsService-->>ExternalDICOMModality: 20. [C-STORE-RSP] DICOM Success Response (0x0000)
    activate InfrastructureLayer
    DMPSWindowsService->>InfrastructureLayer: 2. 2. OnCStoreRequest(request): Delegate file handling and parsing.
    InfrastructureLayer-->>DMPSWindowsService: 4. return DicomMetadataDto
    InfrastructureLayer->>InfrastructureLayer: 3. 3. ParseDicomFiles(fileStreams): Extract metadata using fo-dicom and validate.
    DMPSWindowsService->>InfrastructureLayer: 5. 5. PublishAsync<ProcessDicomStoreCommand>(command): Queue metadata for persistence.
    InfrastructureLayer-->>DMPSWindowsService: Acknowledges successful publication.
    InfrastructureLayer->>RabbitMQBroker: 6. 6. [AMQP] basic.publish to 'dicom_system_exchange' with routing key 'dicom.store.process'.
    DMPSWindowsService->>ExternalDICOMModality: 7. 7. [C-STORE-RSP] Send DICOM success response (0x0000).
    RabbitMQBroker->>InfrastructureLayer: 8. 8. [AMQP] Delivers message from 'dicom_store_queue' to subscribed consumer.
    InfrastructureLayer-->>RabbitMQBroker: Invokes the message handler logic in the service.
    InfrastructureLayer->>DMPSWindowsService: 9. 9. HandleMessageAsync(ProcessDicomStoreCommand): Invoke consumer logic.
    activate DataAccessLayer
    DMPSWindowsService->>DataAccessLayer: 10. 10. PersistStudyTransactionAsync(command.Metadata): Persist metadata within a transaction.
    DataAccessLayer-->>DMPSWindowsService: 14. return persistence result (success/failure)
    DataAccessLayer->>PostgreSQLDatabase: 11. 11-13. [SQL] Execute transactional INSERT/UPDATE for Patient, Study, Series, Image.
    DMPSWindowsService->>InfrastructureLayer: 15. 15. MoveDicomFilesToPermanentStorage(tempPaths, permanentPaths).
    InfrastructureLayer-->>DMPSWindowsService: 16. return file move result (success/failure)
    DMPSWindowsService->>InfrastructureLayer: 17. 17. AcknowledgeMessage(deliveryTag): Confirm successful processing.
    InfrastructureLayer->>RabbitMQBroker: 18. 18. [AMQP] basic.ack to remove message from 'dicom_store_queue'.

    note over DMPSWindowsService: Decoupling Point (REQ-TEC-001): The C-STORE Success response is sent at Step 7, immediately after...
    note over DMPSWindowsService: Transactional Boundary: The database write (Step 10) and file move (Step 15) must be atomic. If t...
    note over DataAccessLayer: Idempotency: The consumer logic at Step 10 must be idempotent. The Data Access Layer should use '...
    note over RabbitMQBroker: Error Handling & DLQ (REQ-TEC-002): If any step after 9 fails permanently, the consumer sends a '...

    deactivate DataAccessLayer
    deactivate InfrastructureLayer
    deactivate DMPSWindowsService
