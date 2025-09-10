sequenceDiagram
    actor "Client Application Service" as ClientApplicationService
    actor "NamedPipeServer (Infrastructure)" as NamedPipeServerInfrastructure
    participant "NamedPipeServer (Infrastructure)" as NamedPipeServerInfrastructure
    participant "Background Hosted Service" as BackgroundHostedService

    activate NamedPipeServerInfrastructure
    ClientApplicationService->>NamedPipeServerInfrastructure: 1. 1. Call IsBackgroundServiceRunningAsync()
    NamedPipeServerInfrastructure-->>ClientApplicationService: 10. return serviceIsRunning (bool)
    NamedPipeServerInfrastructure->>NamedPipeServerInfrastructure: 2. 2. Connect to Named Pipe
    NamedPipeServerInfrastructure->>NamedPipeServerInfrastructure: 3. 3. Write request message: 'PING'
    NamedPipeServerInfrastructure->>BackgroundHostedService: 4. 4. Listen for client connection
    NamedPipeServerInfrastructure->>NamedPipeServerInfrastructure: 5. 5. Read request message: 'PING'
    NamedPipeServerInfrastructure->>BackgroundHostedService: 6. 6. [Internal] Verify Host Service is Healthy
    BackgroundHostedService-->>NamedPipeServerInfrastructure: Returns true
    NamedPipeServerInfrastructure->>NamedPipeServerInfrastructure: 7. 7. Write response message: 'PONG'
    NamedPipeServerInfrastructure->>NamedPipeServerInfrastructure: 8. 8. Read response message: 'PONG'
    NamedPipeServerInfrastructure->>NamedPipeServerInfrastructure: 9. 9. Dispose Pipe Connection

    note over NamedPipeServerInfrastructure: Configuration: The Named Pipe name (e.g., 'DicomServiceStatusPipe') must be a shared, constant va...

    deactivate NamedPipeServerInfrastructure
