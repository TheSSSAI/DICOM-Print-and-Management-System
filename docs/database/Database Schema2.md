# Specification

# 1. Database Design

## 1.1. User
Represents system users with authentication, role, and profile information. Complies with REQ-1-014, REQ-1-067.

### 1.1.3. Attributes

### 1.1.3.1. userId
#### 1.1.3.1.2. Type
Guid

#### 1.1.3.1.3. Is Required
True

#### 1.1.3.1.4. Is Primary Key
True

### 1.1.3.2. username
#### 1.1.3.2.2. Type
VARCHAR

#### 1.1.3.2.3. Is Required
True

#### 1.1.3.2.4. Is Unique
True

#### 1.1.3.2.5. Size
100

### 1.1.3.3. passwordHash
#### 1.1.3.3.2. Type
VARCHAR

#### 1.1.3.3.3. Is Required
True

#### 1.1.3.3.4. Size
255

### 1.1.3.4. firstName
#### 1.1.3.4.2. Type
TEXT

#### 1.1.3.4.3. Is Required
True

### 1.1.3.5. lastName
#### 1.1.3.5.2. Type
TEXT

#### 1.1.3.5.3. Is Required
True

### 1.1.3.6. roleId
#### 1.1.3.6.2. Type
Guid

#### 1.1.3.6.3. Is Required
True

#### 1.1.3.6.4. Is Foreign Key
True

### 1.1.3.7. isActive
#### 1.1.3.7.2. Type
BOOLEAN

#### 1.1.3.7.3. Is Required
True

### 1.1.3.8. isTemporaryPassword
#### 1.1.3.8.2. Type
BOOLEAN

#### 1.1.3.8.3. Is Required
True

### 1.1.3.9. passwordLastChangedAt
#### 1.1.3.9.2. Type
DateTime

#### 1.1.3.9.3. Is Required
True

### 1.1.3.10. createdAt
#### 1.1.3.10.2. Type
DateTime

#### 1.1.3.10.3. Is Required
True

### 1.1.3.11. updatedAt
#### 1.1.3.11.2. Type
DateTime

#### 1.1.3.11.3. Is Required
True


### 1.1.4. Primary Keys

- userId

### 1.1.5. Unique Constraints

### 1.1.5.1. uq_user_username
#### 1.1.5.1.2. Columns

- username


### 1.1.6. Indexes


## 1.2. Role
Defines the roles available in the system, such as 'Technician' and 'Administrator'. This is read-heavy, rarely modified data, ideal for cache-aside strategy on application startup. (REQ-1-014)

### 1.2.3. Attributes

### 1.2.3.1. roleId
#### 1.2.3.1.2. Type
Guid

#### 1.2.3.1.3. Is Required
True

#### 1.2.3.1.4. Is Primary Key
True

### 1.2.3.2. roleName
#### 1.2.3.2.2. Type
VARCHAR

#### 1.2.3.2.3. Is Required
True

#### 1.2.3.2.4. Is Unique
True

#### 1.2.3.2.5. Size
50

### 1.2.3.3. description
#### 1.2.3.3.2. Type
TEXT

#### 1.2.3.3.3. Is Required
False


### 1.2.4. Primary Keys

- roleId

### 1.2.5. Unique Constraints

### 1.2.5.1. uq_role_rolename
#### 1.2.5.1.2. Columns

- roleName


### 1.2.6. Indexes


## 1.3. PasswordHistory
Stores a history of a user's previous password hashes to enforce reuse policy. (REQ-1-042)

### 1.3.3. Attributes

### 1.3.3.1. passwordHistoryId
#### 1.3.3.1.2. Type
Guid

#### 1.3.3.1.3. Is Required
True

#### 1.3.3.1.4. Is Primary Key
True

### 1.3.3.2. userId
#### 1.3.3.2.2. Type
Guid

#### 1.3.3.2.3. Is Required
True

#### 1.3.3.2.4. Is Foreign Key
True

### 1.3.3.3. passwordHash
#### 1.3.3.3.2. Type
VARCHAR

#### 1.3.3.3.3. Is Required
True

#### 1.3.3.3.4. Size
255

### 1.3.3.4. createdAt
#### 1.3.3.4.2. Type
DateTime

#### 1.3.3.4.3. Is Required
True


### 1.3.4. Primary Keys

- passwordHistoryId

### 1.3.5. Unique Constraints


### 1.3.6. Indexes

### 1.3.6.1. idx_passwordhistory_user_created
#### 1.3.6.1.2. Columns

- userId
- createdAt DESC

#### 1.3.6.1.3. Type
BTree


## 1.4. Patient
Stores patient demographic information. All PHI fields must be encrypted using pgcrypto. (REQ-1-010, REQ-1-083)

### 1.4.3. Attributes

### 1.4.3.1. patientId
#### 1.4.3.1.2. Type
Guid

#### 1.4.3.1.3. Is Required
True

#### 1.4.3.1.4. Is Primary Key
True

### 1.4.3.2. dicomPatientId
#### 1.4.3.2.2. Type
TEXT

#### 1.4.3.2.3. Is Required
True

### 1.4.3.3. patientName
#### 1.4.3.3.2. Type
TEXT

#### 1.4.3.3.3. Is Required
False

### 1.4.3.4. patientBirthDate
#### 1.4.3.4.2. Type
TEXT

#### 1.4.3.4.3. Is Required
False

### 1.4.3.5. patientSex
#### 1.4.3.5.2. Type
TEXT

#### 1.4.3.5.3. Is Required
False

### 1.4.3.6. createdAt
#### 1.4.3.6.2. Type
DateTime

#### 1.4.3.6.3. Is Required
True

### 1.4.3.7. updatedAt
#### 1.4.3.7.2. Type
DateTime

#### 1.4.3.7.3. Is Required
True


### 1.4.4. Primary Keys

- patientId

### 1.4.5. Unique Constraints


### 1.4.6. Indexes

### 1.4.6.1. idx_patient_name_gin_trgm
#### 1.4.6.1.2. Columns

- patientName

#### 1.4.6.1.3. Type
GIN

### 1.4.6.2. idx_patient_dicompatientid_gin_trgm
#### 1.4.6.2.2. Columns

- dicomPatientId

#### 1.4.6.2.3. Type
GIN


## 1.5. Study
Stores study-level DICOM metadata. Includes support for 'Working Copy' functionality and data retention policies. (REQ-1-010, REQ-NFR-003, REQ-1-018)

### 1.5.3. Attributes

### 1.5.3.1. studyId
#### 1.5.3.1.2. Type
Guid

#### 1.5.3.1.3. Is Required
True

#### 1.5.3.1.4. Is Primary Key
True

### 1.5.3.2. patientId
#### 1.5.3.2.2. Type
Guid

#### 1.5.3.2.3. Is Required
True

#### 1.5.3.2.4. Is Foreign Key
True

### 1.5.3.3. originalStudyId
#### 1.5.3.3.2. Type
Guid

#### 1.5.3.3.3. Is Required
False

#### 1.5.3.3.4. Is Foreign Key
True

### 1.5.3.4. studyInstanceUid
#### 1.5.3.4.2. Type
VARCHAR

#### 1.5.3.4.3. Is Required
True

#### 1.5.3.4.4. Size
128

### 1.5.3.5. studyDate
#### 1.5.3.5.2. Type
DateTime

#### 1.5.3.5.3. Is Required
False

### 1.5.3.6. studyDescription
#### 1.5.3.6.2. Type
TEXT

#### 1.5.3.6.3. Is Required
False

### 1.5.3.7. accessionNumber
#### 1.5.3.7.2. Type
TEXT

#### 1.5.3.7.3. Is Required
False

### 1.5.3.8. referringPhysicianName
#### 1.5.3.8.2. Type
TEXT

#### 1.5.3.8.3. Is Required
False

### 1.5.3.9. isDeleted
#### 1.5.3.9.2. Type
BOOLEAN

#### 1.5.3.9.3. Is Required
True

### 1.5.3.10. createdAt
#### 1.5.3.10.2. Type
DateTime

#### 1.5.3.10.3. Is Required
True


### 1.5.4. Primary Keys

- studyId

### 1.5.5. Unique Constraints


### 1.5.6. Indexes

### 1.5.6.1. uq_study_studyinstanceuid_original
#### 1.5.6.1.2. Columns

- studyInstanceUid

#### 1.5.6.1.3. Type
BTree

### 1.5.6.2. idx_study_patient_date
#### 1.5.6.2.2. Columns

- patientId
- studyDate DESC

#### 1.5.6.2.3. Type
BTree

### 1.5.6.3. idx_study_originalstudyid
#### 1.5.6.3.2. Columns

- originalStudyId

#### 1.5.6.3.3. Type
BTree

### 1.5.6.4. idx_study_active
#### 1.5.6.4.2. Columns

- patientId

#### 1.5.6.4.3. Type
BTree


## 1.6. Series
Stores series-level DICOM metadata.

### 1.6.3. Attributes

### 1.6.3.1. seriesId
#### 1.6.3.1.2. Type
Guid

#### 1.6.3.1.3. Is Required
True

#### 1.6.3.1.4. Is Primary Key
True

### 1.6.3.2. studyId
#### 1.6.3.2.2. Type
Guid

#### 1.6.3.2.3. Is Required
True

#### 1.6.3.2.4. Is Foreign Key
True

### 1.6.3.3. seriesInstanceUid
#### 1.6.3.3.2. Type
VARCHAR

#### 1.6.3.3.3. Is Required
True

#### 1.6.3.3.4. Is Unique
True

#### 1.6.3.3.5. Size
128

### 1.6.3.4. modality
#### 1.6.3.4.2. Type
VARCHAR

#### 1.6.3.4.3. Is Required
False

#### 1.6.3.4.4. Size
16

### 1.6.3.5. seriesNumber
#### 1.6.3.5.2. Type
INT

#### 1.6.3.5.3. Is Required
False

### 1.6.3.6. seriesDescription
#### 1.6.3.6.2. Type
TEXT

#### 1.6.3.6.3. Is Required
False

### 1.6.3.7. bodyPartExamined
#### 1.6.3.7.2. Type
VARCHAR

#### 1.6.3.7.3. Is Required
False

#### 1.6.3.7.4. Size
64

### 1.6.3.8. createdAt
#### 1.6.3.8.2. Type
DateTime

#### 1.6.3.8.3. Is Required
True


### 1.6.4. Primary Keys

- seriesId

### 1.6.5. Unique Constraints

### 1.6.5.1. uq_series_seriesinstanceuid
#### 1.6.5.1.2. Columns

- seriesInstanceUid


### 1.6.6. Indexes

### 1.6.6.1. idx_series_modality
#### 1.6.6.1.2. Columns

- modality

#### 1.6.6.1.3. Type
BTree


## 1.7. Image
Stores instance-level DICOM metadata and the path to the physical file. (REQ-1-056)

### 1.7.3. Attributes

### 1.7.3.1. imageId
#### 1.7.3.1.2. Type
Guid

#### 1.7.3.1.3. Is Required
True

#### 1.7.3.1.4. Is Primary Key
True

### 1.7.3.2. seriesId
#### 1.7.3.2.2. Type
Guid

#### 1.7.3.2.3. Is Required
True

#### 1.7.3.2.4. Is Foreign Key
True

### 1.7.3.3. sopInstanceUid
#### 1.7.3.3.2. Type
VARCHAR

#### 1.7.3.3.3. Is Required
True

#### 1.7.3.3.4. Is Unique
True

#### 1.7.3.3.5. Size
128

### 1.7.3.4. instanceNumber
#### 1.7.3.4.2. Type
INT

#### 1.7.3.4.3. Is Required
False

### 1.7.3.5. filePath
#### 1.7.3.5.2. Type
TEXT

#### 1.7.3.5.3. Is Required
True

### 1.7.3.6. fileSize
#### 1.7.3.6.2. Type
BIGINT

#### 1.7.3.6.3. Is Required
True

### 1.7.3.7. createdAt
#### 1.7.3.7.2. Type
DateTime

#### 1.7.3.7.3. Is Required
True


### 1.7.4. Primary Keys

- imageId

### 1.7.5. Unique Constraints

### 1.7.5.1. uq_image_sopinstanceuid
#### 1.7.5.1.2. Columns

- sopInstanceUid


### 1.7.6. Indexes

### 1.7.6.1. idx_image_series_instance
#### 1.7.6.1.2. Columns

- seriesId
- instanceNumber ASC

#### 1.7.6.1.3. Type
BTree


## 1.8. PresentationState
Represents a DICOM Grayscale Softcopy Presentation State (GSPS) object. (REQ-1-061)

### 1.8.3. Attributes

### 1.8.3.1. presentationStateId
#### 1.8.3.1.2. Type
Guid

#### 1.8.3.1.3. Is Required
True

#### 1.8.3.1.4. Is Primary Key
True

### 1.8.3.2. seriesId
#### 1.8.3.2.2. Type
Guid

#### 1.8.3.2.3. Is Required
True

#### 1.8.3.2.4. Is Foreign Key
True

### 1.8.3.3. sopInstanceUid
#### 1.8.3.3.2. Type
VARCHAR

#### 1.8.3.3.3. Is Required
True

#### 1.8.3.3.4. Is Unique
True

#### 1.8.3.3.5. Size
128

### 1.8.3.4. filePath
#### 1.8.3.4.2. Type
TEXT

#### 1.8.3.4.3. Is Required
True

### 1.8.3.5. createdByUserId
#### 1.8.3.5.2. Type
Guid

#### 1.8.3.5.3. Is Required
True

#### 1.8.3.5.4. Is Foreign Key
True

### 1.8.3.6. createdAt
#### 1.8.3.6.2. Type
DateTime

#### 1.8.3.6.3. Is Required
True

### 1.8.3.7. updatedAt
#### 1.8.3.7.2. Type
DateTime

#### 1.8.3.7.3. Is Required
True


### 1.8.4. Primary Keys

- presentationStateId

### 1.8.5. Unique Constraints

### 1.8.5.1. uq_presentationstate_sopinstanceuid
#### 1.8.5.1.2. Columns

- sopInstanceUid


### 1.8.6. Indexes


## 1.9. SystemSetting
A key-value store for global application settings. This is read-heavy, rarely modified data, ideal for cache-aside strategy on application startup. (e.g., REQ-1-018, REQ-1-042, REQ-1-059)

### 1.9.3. Attributes

### 1.9.3.1. settingKey
#### 1.9.3.1.2. Type
VARCHAR

#### 1.9.3.1.3. Is Required
True

#### 1.9.3.1.4. Is Primary Key
True

#### 1.9.3.1.5. Size
100

### 1.9.3.2. settingValue
#### 1.9.3.2.2. Type
TEXT

#### 1.9.3.2.3. Is Required
True

### 1.9.3.3. description
#### 1.9.3.3.2. Type
TEXT

#### 1.9.3.3.3. Is Required
False

### 1.9.3.4. updatedAt
#### 1.9.3.4.2. Type
DateTime

#### 1.9.3.4.3. Is Required
True


### 1.9.4. Primary Keys

- settingKey

### 1.9.5. Unique Constraints


### 1.9.6. Indexes


## 1.10. PacsConfiguration
Stores connection details for remote PACS servers. This is read-heavy, rarely modified data, ideal for cache-aside strategy on application startup. (REQ-1-038)

### 1.10.3. Attributes

### 1.10.3.1. pacsConfigurationId
#### 1.10.3.1.2. Type
Guid

#### 1.10.3.1.3. Is Required
True

#### 1.10.3.1.4. Is Primary Key
True

### 1.10.3.2. aeTitle
#### 1.10.3.2.2. Type
VARCHAR

#### 1.10.3.2.3. Is Required
True

#### 1.10.3.2.4. Size
16

### 1.10.3.3. hostname
#### 1.10.3.3.2. Type
VARCHAR

#### 1.10.3.3.3. Is Required
True

#### 1.10.3.3.4. Size
255

### 1.10.3.4. port
#### 1.10.3.4.2. Type
INT

#### 1.10.3.4.3. Is Required
True

### 1.10.3.5. supportsCFind
#### 1.10.3.5.2. Type
BOOLEAN

#### 1.10.3.5.3. Is Required
True

### 1.10.3.6. supportsCMove
#### 1.10.3.6.2. Type
BOOLEAN

#### 1.10.3.6.3. Is Required
True

### 1.10.3.7. supportsCStore
#### 1.10.3.7.2. Type
BOOLEAN

#### 1.10.3.7.3. Is Required
True

### 1.10.3.8. createdAt
#### 1.10.3.8.2. Type
DateTime

#### 1.10.3.8.3. Is Required
True

### 1.10.3.9. updatedAt
#### 1.10.3.9.2. Type
DateTime

#### 1.10.3.9.3. Is Required
True


### 1.10.4. Primary Keys

- pacsConfigurationId

### 1.10.5. Unique Constraints


### 1.10.6. Indexes


## 1.11. AutoRoutingRule
Defines rules for automatically routing incoming C-STORE studies. (REQ-1-037)

### 1.11.3. Attributes

### 1.11.3.1. autoRoutingRuleId
#### 1.11.3.1.2. Type
Guid

#### 1.11.3.1.3. Is Required
True

#### 1.11.3.1.4. Is Primary Key
True

### 1.11.3.2. ruleName
#### 1.11.3.2.2. Type
VARCHAR

#### 1.11.3.2.3. Is Required
True

#### 1.11.3.2.4. Size
100

### 1.11.3.3. criteria
#### 1.11.3.3.2. Type
JSONB

#### 1.11.3.3.3. Is Required
True

### 1.11.3.4. destinationPath
#### 1.11.3.4.2. Type
TEXT

#### 1.11.3.4.3. Is Required
True

### 1.11.3.5. priority
#### 1.11.3.5.2. Type
INT

#### 1.11.3.5.3. Is Required
True

### 1.11.3.6. isEnabled
#### 1.11.3.6.2. Type
BOOLEAN

#### 1.11.3.6.3. Is Required
True

### 1.11.3.7. createdAt
#### 1.11.3.7.2. Type
DateTime

#### 1.11.3.7.3. Is Required
True

### 1.11.3.8. updatedAt
#### 1.11.3.8.2. Type
DateTime

#### 1.11.3.8.3. Is Required
True


### 1.11.4. Primary Keys

- autoRoutingRuleId

### 1.11.5. Unique Constraints


### 1.11.6. Indexes

### 1.11.6.1. idx_autoroutingrule_criteria_gin
#### 1.11.6.1.2. Columns

- criteria

#### 1.11.6.1.3. Type
GIN


## 1.12. PrintJob
Represents a print job in the processing queue. (REQ-1-033)

### 1.12.3. Attributes

### 1.12.3.1. printJobId
#### 1.12.3.1.2. Type
Guid

#### 1.12.3.1.3. Is Required
True

#### 1.12.3.1.4. Is Primary Key
True

### 1.12.3.2. submittedByUserId
#### 1.12.3.2.2. Type
Guid

#### 1.12.3.2.3. Is Required
True

#### 1.12.3.2.4. Is Foreign Key
True

### 1.12.3.3. jobPayload
#### 1.12.3.3.2. Type
JSONB

#### 1.12.3.3.3. Is Required
True

### 1.12.3.4. status
#### 1.12.3.4.2. Type
VARCHAR

#### 1.12.3.4.3. Is Required
True

#### 1.12.3.4.4. Size
20

### 1.12.3.5. printerName
#### 1.12.3.5.2. Type
VARCHAR

#### 1.12.3.5.3. Is Required
True

#### 1.12.3.5.4. Size
255

### 1.12.3.6. failureReason
#### 1.12.3.6.2. Type
TEXT

#### 1.12.3.6.3. Is Required
False

### 1.12.3.7. priority
#### 1.12.3.7.2. Type
INT

#### 1.12.3.7.3. Is Required
True

### 1.12.3.8. correlationId
#### 1.12.3.8.2. Type
Guid

#### 1.12.3.8.3. Is Required
False

### 1.12.3.9. submittedAt
#### 1.12.3.9.2. Type
DateTime

#### 1.12.3.9.3. Is Required
True

### 1.12.3.10. processedAt
#### 1.12.3.10.2. Type
DateTime

#### 1.12.3.10.3. Is Required
False


### 1.12.4. Primary Keys

- printJobId

### 1.12.5. Unique Constraints


### 1.12.6. Indexes

### 1.12.6.1. idx_printjob_polling
#### 1.12.6.1.2. Columns

- status
- priority DESC
- submittedAt ASC

#### 1.12.6.1.3. Type
BTree


## 1.13. HangingProtocol
Stores user-defined or system-wide display layouts (hanging protocols). (REQ-1-062, REQ-1-063)

### 1.13.3. Attributes

### 1.13.3.1. hangingProtocolId
#### 1.13.3.1.2. Type
Guid

#### 1.13.3.1.3. Is Required
True

#### 1.13.3.1.4. Is Primary Key
True

### 1.13.3.2. protocolName
#### 1.13.3.2.2. Type
VARCHAR

#### 1.13.3.2.3. Is Required
True

#### 1.13.3.2.4. Size
100

### 1.13.3.3. userId
#### 1.13.3.3.2. Type
Guid

#### 1.13.3.3.3. Is Required
False

#### 1.13.3.3.4. Is Foreign Key
True

### 1.13.3.4. layoutDefinition
#### 1.13.3.4.2. Type
JSONB

#### 1.13.3.4.3. Is Required
True

### 1.13.3.5. criteria
#### 1.13.3.5.2. Type
JSONB

#### 1.13.3.5.3. Is Required
False

### 1.13.3.6. createdAt
#### 1.13.3.6.2. Type
DateTime

#### 1.13.3.6.3. Is Required
True

### 1.13.3.7. updatedAt
#### 1.13.3.7.2. Type
DateTime

#### 1.13.3.7.3. Is Required
True


### 1.13.4. Primary Keys

- hangingProtocolId

### 1.13.5. Unique Constraints


### 1.13.6. Indexes


## 1.14. UserPreference
A key-value store for user-specific preferences, such as custom WW/WL presets or UI settings. (REQ-1-069)

### 1.14.3. Attributes

### 1.14.3.1. userId
#### 1.14.3.1.2. Type
Guid

#### 1.14.3.1.3. Is Required
True

#### 1.14.3.1.4. Is Primary Key
True

#### 1.14.3.1.5. Is Foreign Key
True

### 1.14.3.2. preferenceKey
#### 1.14.3.2.2. Type
VARCHAR

#### 1.14.3.2.3. Is Required
True

#### 1.14.3.2.4. Is Primary Key
True

#### 1.14.3.2.5. Size
100

### 1.14.3.3. preferenceValue
#### 1.14.3.3.2. Type
TEXT

#### 1.14.3.3.3. Is Required
True

### 1.14.3.4. updatedAt
#### 1.14.3.4.2. Type
DateTime

#### 1.14.3.4.3. Is Required
True


### 1.14.4. Primary Keys

- userId
- preferenceKey

### 1.14.5. Unique Constraints


### 1.14.6. Indexes




---

# 2. Diagrams

- **Diagram_Title:** DicomAppDB Entity Relationship Diagram  
**Diagram_Area:** Overall System  
**Explanation:** This diagram illustrates the complete schema for the DicomAppDB. The core entities revolve around DICOM data (Patient, Study, Series, Image). User management is handled by User, Role, and PasswordHistory. System configuration and operational data are stored in tables like SystemSetting, PacsConfiguration, AutoRoutingRule, and PrintJob. User-specific features like presentations, protocols, and preferences are also modeled.  
**Mermaid_Text:** erDiagram

    User {
        Guid userId PK
        VARCHAR username UK
        VARCHAR passwordHash
        TEXT firstName
        TEXT lastName
        Guid roleId FK
        BOOLEAN isActive
        BOOLEAN isTemporaryPassword
        DateTime passwordLastChangedAt
        DateTime createdAt
        DateTime updatedAt
    }

    Role {
        Guid roleId PK
        VARCHAR roleName UK
        TEXT description
    }

    PasswordHistory {
        Guid passwordHistoryId PK
        Guid userId FK
        VARCHAR passwordHash
        DateTime createdAt
    }

    Patient {
        Guid patientId PK
        TEXT dicomPatientId
        TEXT patientName
        TEXT patientBirthDate
        TEXT patientSex
        DateTime createdAt
        DateTime updatedAt
    }

    Study {
        Guid studyId PK
        Guid patientId FK
        Guid originalStudyId FK
        VARCHAR studyInstanceUid
        DateTime studyDate
        TEXT studyDescription
        TEXT accessionNumber
        BOOLEAN isDeleted
        DateTime createdAt
    }

    Series {
        Guid seriesId PK
        Guid studyId FK
        VARCHAR seriesInstanceUid UK
        VARCHAR modality
        INT seriesNumber
        TEXT seriesDescription
        VARCHAR bodyPartExamined
        DateTime createdAt
    }

    Image {
        Guid imageId PK
        Guid seriesId FK
        VARCHAR sopInstanceUid UK
        INT instanceNumber
        TEXT filePath
        BIGINT fileSize
        DateTime createdAt
    }

    PresentationState {
        Guid presentationStateId PK
        Guid seriesId FK
        VARCHAR sopInstanceUid UK
        TEXT filePath
        Guid createdByUserId FK
        DateTime createdAt
        DateTime updatedAt
    }

    HangingProtocol {
        Guid hangingProtocolId PK
        VARCHAR protocolName
        Guid userId FK
        JSONB layoutDefinition
        JSONB criteria
        DateTime createdAt
        DateTime updatedAt
    }

    UserPreference {
        Guid userId PK, FK
        VARCHAR preferenceKey PK
        TEXT preferenceValue
        DateTime updatedAt
    }

    PrintJob {
        Guid printJobId PK
        Guid submittedByUserId FK
        JSONB jobPayload
        VARCHAR status
        VARCHAR printerName
        TEXT failureReason
        INT priority
        DateTime submittedAt
        DateTime processedAt
    }

    SystemSetting {
        VARCHAR settingKey PK
        TEXT settingValue
        TEXT description
        DateTime updatedAt
    }

    PacsConfiguration {
        Guid pacsConfigurationId PK
        VARCHAR aeTitle
        VARCHAR hostname
        INT port
        BOOLEAN supportsCFind
        BOOLEAN supportsCMove
        BOOLEAN supportsCStore
        DateTime createdAt
        DateTime updatedAt
    }

    AutoRoutingRule {
        Guid autoRoutingRuleId PK
        VARCHAR ruleName
        JSONB criteria
        TEXT destinationPath
        INT priority
        BOOLEAN isEnabled
        DateTime createdAt
        DateTime updatedAt
    }

    // Relationships
    Role ||--o{ User : has
    User ||--o{ PasswordHistory : stores
    User ||--o{ PrintJob : submits
    User }o--o{ HangingProtocol : defines
    User ||--o{ UserPreference : has
    User ||--o{ PresentationState : creates
    Patient ||--o{ Study : has
    Study }o--o{ Study : "is working copy of"
    Study ||--o{ Series : contains
    Series ||--o{ Image : contains
    Series ||--o{ PresentationState : applies_to  


---

