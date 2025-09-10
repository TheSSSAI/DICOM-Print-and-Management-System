erDiagram
    AuditLog {
        BIGINT auditLogId PK
        Guid userId
        DateTime eventTimestamp
        VARCHAR eventType
        VARCHAR entityName
        VARCHAR entityId
        JSONB details
        Guid correlationId
    }
    UserSession {
        string userSessionId PK
        Hash sessionData
        integer ttl
    }