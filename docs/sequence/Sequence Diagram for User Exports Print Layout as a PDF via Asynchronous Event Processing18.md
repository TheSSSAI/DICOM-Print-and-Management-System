sequenceDiagram
    actor "User (Admin/Technician)" as UserAdminTechnician
    participant "Presentation Layer" as PresentationLayer
    actor "Client Application Service" as ClientApplicationService
    participant "RabbitMQ Broker" as RabbitMQBroker
    participant "Background Service (PDF Consumer)" as BackgroundServicePDFConsumer
    participant "Infrastructure Layer" as InfrastructureLayer

    activate PresentationLayer
    UserAdminTechnician->>PresentationLayer: 1. 1. Click 'Export to PDF' and select file path via Save File Dialog
    activate ClientApplicationService
    PresentationLayer->>ClientApplicationService: 2. 2. Invoke ExportToPdfAsync(viewModelData, outputPath)
    ClientApplicationService-->>PresentationLayer: 3. Return Task<Result>
    activate RabbitMQBroker
    ClientApplicationService->>RabbitMQBroker: 3. 4. Publish GeneratePdfCommand message
    RabbitMQBroker-->>ClientApplicationService: 5. Acknowledge publish
    ClientApplicationService->>PresentationLayer: 4. 6. Display 'PDF export has started' notification
    activate BackgroundServicePDFConsumer
    RabbitMQBroker->>BackgroundServicePDFConsumer: 5. 7. Deliver GeneratePdfCommand message
    activate InfrastructureLayer
    BackgroundServicePDFConsumer->>InfrastructureLayer: 6. 8. Request DICOM files: RetrieveDicomFilesAsync(imageReferences)
    InfrastructureLayer-->>BackgroundServicePDFConsumer: 9. Return DICOM data streams
    BackgroundServicePDFConsumer->>InfrastructureLayer: 7. 10. Generate PDF: GeneratePdfAsync(layout, dicomData)
    InfrastructureLayer-->>BackgroundServicePDFConsumer: 11. Return PDF byte array
    BackgroundServicePDFConsumer->>InfrastructureLayer: 8. 12. Save PDF file: SaveFileAsync(pdfBytes, outputPath)
    InfrastructureLayer-->>BackgroundServicePDFConsumer: 13. Return success
    BackgroundServicePDFConsumer->>RabbitMQBroker: 9. 14. Acknowledge message (basic.ack)

    note over BackgroundServicePDFConsumer: Error Handling: If any step within the consumer (8-13) fails, the service will catch the exceptio...
    note over BackgroundServicePDFConsumer: Idempotency: The PDF generation consumer should be designed to be idempotent. If a message is red...
    note over PresentationLayer: User Notification on Completion/Failure: The sequence shows a 'Job Started' notification. A robus...
    note over BackgroundServicePDFConsumer: Security: The background service runs under a specific service account. This account MUST have ap...

    deactivate InfrastructureLayer
    deactivate BackgroundServicePDFConsumer
    deactivate RabbitMQBroker
    deactivate ClientApplicationService
    deactivate PresentationLayer
