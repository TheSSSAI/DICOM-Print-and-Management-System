sequenceDiagram
    participant "Presentation Layer" as PresentationLayer
    actor "Client Application Service" as ClientApplicationService
    participant "RabbitMQ Broker" as RabbitMQBroker
    participant "Background Service" as BackgroundService
    participant "Infrastructure Services" as InfrastructureServices
    participant "Data Access Layer" as DataAccessLayer
    participant "Windows Print Spooler" as WindowsPrintSpooler

    activate ClientApplicationService
    PresentationLayer->>ClientApplicationService: 1. Invoke IPrintJobService.SubmitPrintJobAsync(printJobData)
    activate DataAccessLayer
    ClientApplicationService->>DataAccessLayer: 2. CreatePrintJobRecordAsync(jobDetails)
    DataAccessLayer-->>ClientApplicationService: printJobId
    ClientApplicationService->>RabbitMQBroker: 3. Publish(SubmitPrintJobCommand)
    ClientApplicationService->>PresentationLayer: 4. Return 'Queued' status acknowledgement
    activate BackgroundService
    RabbitMQBroker->>BackgroundService: 5. Deliver Message: SubmitPrintJobCommand
    BackgroundService->>DataAccessLayer: 6. UpdatePrintJobStatusAsync(jobId, 'Processing')
    activate InfrastructureServices
    BackgroundService->>InfrastructureServices: 7. GetDicomFilesForPrinting(imageUids)
    InfrastructureServices-->>BackgroundService: List<DicomFile>
    BackgroundService->>InfrastructureServices: 8. GeneratePrintDocument(dicomFiles, layout)
    InfrastructureServices-->>BackgroundService: PrintDocument object
    InfrastructureServices->>WindowsPrintSpooler: 9. SpoolJobToPrinter(printDocument)
    WindowsPrintSpooler-->>InfrastructureServices: Success/Failure
    BackgroundService->>DataAccessLayer: 10. UpdatePrintJobStatusAsync(jobId, 'Completed')
    BackgroundService->>RabbitMQBroker: 11. Acknowledge Message (BasicAck)

    note over RabbitMQBroker: Message Queue Configuration (REQ-TEC-002): - Exchange Type: Direct - Queue: Durable - Messages: P...
    note over BackgroundService: Error Handling Flow: If any step from 6-9 fails after retries, the service sends a Negative Ackno...
    note over BackgroundService: Idempotency: The consumer should be idempotent. Before starting processing (step 6), it could che...
    note over ClientApplicationService: Correlation & Tracing (REQ-REP-002): A unique Correlation ID is generated in step 1 by REPO-08-AP...

    deactivate InfrastructureServices
    deactivate BackgroundService
    deactivate DataAccessLayer
    deactivate ClientApplicationService
