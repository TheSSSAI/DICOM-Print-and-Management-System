sequenceDiagram
    participant "DMPS.Service.Application" as DMPSServiceApplication
    participant "RabbitMQ Broker" as RabbitMQBroker
    participant "Alerting System" as AlertingSystem

    activate DMPSServiceApplication
    RabbitMQBroker->>DMPSServiceApplication: 1. [AMQP] basic.deliver(message)
    DMPSServiceApplication->>DMPSServiceApplication: 2. [LOOP Start: 3 retries] Attempt to process message with Correlation ID: {id}
    DMPSServiceApplication-->>DMPSServiceApplication: Processing result (success/failure)
    DMPSServiceApplication->>DMPSServiceApplication: 2.1. [Internal] Log processing failure: Exception {ex}, CorrelationID: {id}
    DMPSServiceApplication->>DMPSServiceApplication: 3. [LOOP End] Final retry attempt failed. Preparing to reject message.
    activate RabbitMQBroker
    DMPSServiceApplication->>RabbitMQBroker: 4. [AMQP] basic.reject(deliveryTag, requeue=false)
    RabbitMQBroker->>RabbitMQBroker: 5. [Internal Broker Logic] Message rejected. Route to configured Dead-Letter Exchange (DLX).
    RabbitMQBroker->>RabbitMQBroker: 6. [Internal Broker Logic] Message published to Dead-Letter Queue (DLQ) via DLX.
    DMPSServiceApplication->>RabbitMQBroker: 7. [HTTP GET] Request queue stats for DLQ.
    RabbitMQBroker-->>DMPSServiceApplication: [HTTP 200 OK] Queue statistics JSON
    RabbitMQBroker->>DMPSServiceApplication: 8. [HTTP 200 OK] Return stats with message_count > 0
    activate AlertingSystem
    DMPSServiceApplication->>AlertingSystem: 9. [SMTP/Webhook] Send critical alert: "Poison message detected in DLQ..."


    deactivate AlertingSystem
    deactivate RabbitMQBroker
    deactivate DMPSServiceApplication
