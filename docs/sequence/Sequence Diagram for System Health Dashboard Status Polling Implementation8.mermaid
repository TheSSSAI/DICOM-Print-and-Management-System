sequenceDiagram
    participant "DMPS Windows Service (HealthProbeHostedService)" as DMPSWindowsServiceHealthProbeHostedService
    participant "PostgreSQL Database" as PostgreSQLDatabase
    participant "RabbitMQ Broker" as RabbitMQBroker
    participant "Operating System" as OperatingSystem

    activate DMPSWindowsServiceHealthProbeHostedService
    DMPSWindowsServiceHealthProbeHostedService->>DMPSWindowsServiceHealthProbeHostedService: 1. 1. [Timer Tick] Scheduled polling interval fires (e.g., every 60s).
    DMPSWindowsServiceHealthProbeHostedService->>PostgreSQLDatabase: 2. 2. Check PostgreSQL Connectivity
    PostgreSQLDatabase-->>DMPSWindowsServiceHealthProbeHostedService: 3. Connection Status
    DMPSWindowsServiceHealthProbeHostedService->>RabbitMQBroker: 4. 4. Check RabbitMQ Connectivity & Queue Depths
    RabbitMQBroker-->>DMPSWindowsServiceHealthProbeHostedService: 5. Connection Status & Queue Metrics
    DMPSWindowsServiceHealthProbeHostedService->>DMPSWindowsServiceHealthProbeHostedService: 4.1. 4.1 Internally verify IConnection.IsOpen property is true.
    DMPSWindowsServiceHealthProbeHostedService->>OperatingSystem: 6. 6. Check DICOM Storage Disk Space
    OperatingSystem-->>DMPSWindowsServiceHealthProbeHostedService: 7. Free Space Information
    DMPSWindowsServiceHealthProbeHostedService->>DMPSWindowsServiceHealthProbeHostedService: 8. 8. Update In-Memory Health Status Cache
    DMPSWindowsServiceHealthProbeHostedService->>DMPSWindowsServiceHealthProbeHostedService: 9. 9. [OPT] If any metric crosses a critical threshold, trigger alert.

    note over DMPSWindowsServiceHealthProbeHostedService: The HealthStatusCache should be implemented as a thread-safe singleton service, injected via DI i...
    note over DMPSWindowsServiceHealthProbeHostedService: Alerting thresholds (e.g., disk space < 10% critical, DLQ count > 0 critical) must be managed via...

    deactivate DMPSWindowsServiceHealthProbeHostedService
