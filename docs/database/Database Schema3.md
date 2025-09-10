# Specification

# 1. Database Design

## 1.1. AuditLog
Records a comprehensive trail of all significant actions. Optimized for high-volume, append-only writes and time-series queries. Ideal for a dedicated Time-Series or Document database like TimescaleDB or Elasticsearch to isolate write load from the transactional DB. (REQ-1-047, REQ-1-048, REQ-1-049)

### 1.1.3. Attributes

### 1.1.3.1. auditLogId
#### 1.1.3.1.2. Type
BIGINT

#### 1.1.3.1.3. Is Required
True

#### 1.1.3.1.4. Is Primary Key
True

### 1.1.3.2. userId
#### 1.1.3.2.2. Type
Guid

#### 1.1.3.2.3. Is Required
False

### 1.1.3.3. eventTimestamp
#### 1.1.3.3.2. Type
DateTime

#### 1.1.3.3.3. Is Required
True

### 1.1.3.4. eventType
#### 1.1.3.4.2. Type
VARCHAR

#### 1.1.3.4.3. Is Required
True

#### 1.1.3.4.4. Size
100

### 1.1.3.5. entityName
#### 1.1.3.5.2. Type
VARCHAR

#### 1.1.3.5.3. Is Required
False

#### 1.1.3.5.4. Size
100

### 1.1.3.6. entityId
#### 1.1.3.6.2. Type
VARCHAR

#### 1.1.3.6.3. Is Required
False

#### 1.1.3.6.4. Size
128

### 1.1.3.7. details
#### 1.1.3.7.2. Type
JSONB

#### 1.1.3.7.3. Is Required
False

### 1.1.3.8. correlationId
#### 1.1.3.8.2. Type
Guid

#### 1.1.3.8.3. Is Required
False


### 1.1.4. Primary Keys

- auditLogId

### 1.1.5. Unique Constraints


### 1.1.6. Indexes

### 1.1.6.1. idx_auditlog_timestamp_event
#### 1.1.6.1.2. Columns

- eventTimestamp DESC
- eventType

#### 1.1.6.1.3. Type
Composite

### 1.1.6.2. idx_auditlog_user_timestamp
#### 1.1.6.2.2. Columns

- userId
- eventTimestamp DESC

#### 1.1.6.2.3. Type
Composite


## 1.2. UserSession
Tracks active user sessions. Best implemented in a fast in-memory key-value store like Redis for high performance and to reduce load on the primary relational database. (REQ-1-019, REQ-1-041)

### 1.2.3. Attributes

### 1.2.3.1. userSessionId
#### 1.2.3.1.2. Type
string

#### 1.2.3.1.3. Is Required
True

#### 1.2.3.1.4. Is Primary Key
True

### 1.2.3.2. sessionData
#### 1.2.3.2.2. Type
Hash

#### 1.2.3.2.3. Is Required
True

### 1.2.3.3. ttl
#### 1.2.3.3.2. Type
integer

#### 1.2.3.3.3. Is Required
True


### 1.2.4. Primary Keys

- userSessionId

### 1.2.5. Unique Constraints


### 1.2.6. Indexes




---

# 2. Diagrams

- **Diagram_Title:** Logging and Caching Database Schema  
**Diagram_Area:** Application Support  
**Explanation:** This diagram displays the entities for the DicomAppAuxDB. It includes 'AuditLog', a time-series optimized table for tracking all significant application actions, and 'UserSession', a key-value structure designed for high-performance session caching. These entities are intentionally decoupled, with 'AuditLog' serving auditing and monitoring purposes and 'UserSession' handling ephemeral session state, reflecting their distinct roles in the system's architecture.  
**Mermaid_Text:** erDiagram
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


---

