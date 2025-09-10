# Specification

# 1. Entities

## 1.1. User
Represents system users with authentication, role, and profile information. Complies with REQ-1-014, REQ-1-067.

### 1.1.3. Attributes

### 1.1.3.1. userId
Unique identifier for the user.

#### 1.1.3.1.2. Type
Guid

#### 1.1.3.1.3. Is Required
True

#### 1.1.3.1.4. Is Primary Key
True

#### 1.1.3.1.5. Is Unique
True

### 1.1.3.2. username
The unique username for login.

#### 1.1.3.2.2. Type
VARCHAR

#### 1.1.3.2.3. Is Required
True

#### 1.1.3.2.4. Is Unique
True

#### 1.1.3.2.5. Size
100

### 1.1.3.3. passwordHash
Salted hash of the user's password, using BCrypt. (REQ-1-082)

#### 1.1.3.3.2. Type
VARCHAR

#### 1.1.3.3.3. Is Required
True

#### 1.1.3.3.4. Size
255

### 1.1.3.4. firstName
User's first name. Encrypted at rest. (REQ-1-083)

#### 1.1.3.4.2. Type
TEXT

#### 1.1.3.4.3. Is Required
True

### 1.1.3.5. lastName
User's last name. Encrypted at rest. (REQ-1-083)

#### 1.1.3.5.2. Type
TEXT

#### 1.1.3.5.3. Is Required
True

### 1.1.3.6. roleId
Foreign key linking to the user's assigned role.

#### 1.1.3.6.2. Type
Guid

#### 1.1.3.6.3. Is Required
True

#### 1.1.3.6.4. Is Foreign Key
True

### 1.1.3.7. isActive
Flag indicating if the user account is active or disabled.

#### 1.1.3.7.2. Type
BOOLEAN

#### 1.1.3.7.3. Is Required
True

#### 1.1.3.7.4. Default Value
true

### 1.1.3.8. isTemporaryPassword
Flag indicating if the user must change their password on next login. (REQ-1-043)

#### 1.1.3.8.2. Type
BOOLEAN

#### 1.1.3.8.3. Is Required
True

#### 1.1.3.8.4. Default Value
false

### 1.1.3.9. passwordLastChangedAt
Timestamp of the last password change, used for expiration policy. (REQ-1-042)

#### 1.1.3.9.2. Type
DateTime

#### 1.1.3.9.3. Is Required
True

### 1.1.3.10. createdAt
Timestamp of when the user was created.

#### 1.1.3.10.2. Type
DateTime

#### 1.1.3.10.3. Is Required
True

#### 1.1.3.10.4. Default Value
NOW()

### 1.1.3.11. updatedAt
Timestamp of the last update to the user record.

#### 1.1.3.11.2. Type
DateTime

#### 1.1.3.11.3. Is Required
True

#### 1.1.3.11.4. Default Value
NOW()


### 1.1.4. Primary Keys

- userId

### 1.1.5. Unique Constraints

### 1.1.5.1. uq_user_username
#### 1.1.5.1.2. Columns

- username


## 1.2. Role
Defines the roles available in the system, such as 'Technician' and 'Administrator'. (REQ-1-014)

### 1.2.3. Caching Strategy
Cache-aside on application startup with message queue invalidation

### 1.2.4. Attributes

### 1.2.4.1. roleId
Unique identifier for the role.

#### 1.2.4.1.2. Type
Guid

#### 1.2.4.1.3. Is Required
True

#### 1.2.4.1.4. Is Primary Key
True

#### 1.2.4.1.5. Is Unique
True

### 1.2.4.2. roleName
The name of the role (e.g., 'Administrator', 'Technician').

#### 1.2.4.2.2. Type
VARCHAR

#### 1.2.4.2.3. Is Required
True

#### 1.2.4.2.4. Is Unique
True

#### 1.2.4.2.5. Size
50

### 1.2.4.3. description
A brief description of the role's permissions.

#### 1.2.4.3.2. Type
TEXT

#### 1.2.4.3.3. Is Required
False


### 1.2.5. Primary Keys

- roleId

### 1.2.6. Unique Constraints

### 1.2.6.1. uq_role_rolename
#### 1.2.6.1.2. Columns

- roleName


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

#### 1.3.3.1.5. Is Unique
True

### 1.3.3.2. userId
Foreign key linking to the user.

#### 1.3.3.2.2. Type
Guid

#### 1.3.3.2.3. Is Required
True

#### 1.3.3.2.4. Is Foreign Key
True

### 1.3.3.3. passwordHash
A previously used password hash.

#### 1.3.3.3.2. Type
VARCHAR

#### 1.3.3.3.3. Is Required
True

#### 1.3.3.3.4. Size
255

### 1.3.3.4. createdAt
Timestamp when this password was set.

#### 1.3.3.4.2. Type
DateTime

#### 1.3.3.4.3. Is Required
True

#### 1.3.3.4.4. Default Value
NOW()


### 1.3.4. Primary Keys

- passwordHistoryId

### 1.3.5. Indexes

### 1.3.5.1. idx_passwordhistory_user_created
#### 1.3.5.1.2. Columns

- userId
- createdAt DESC

#### 1.3.5.1.3. Type
BTree


## 1.4. Patient
Stores patient demographic information extracted from DICOM metadata. All PHI fields must be encrypted using pgcrypto. (REQ-1-010, REQ-1-083)

### 1.4.3. Attributes

### 1.4.3.1. patientId
#### 1.4.3.1.2. Type
Guid

#### 1.4.3.1.3. Is Required
True

#### 1.4.3.1.4. Is Primary Key
True

#### 1.4.3.1.5. Is Unique
True

### 1.4.3.2. dicomPatientId
Patient ID (0010,0020). Encrypted at rest.

#### 1.4.3.2.2. Type
TEXT

#### 1.4.3.2.3. Is Required
True

### 1.4.3.3. patientName
Patient's Name (0010,0010). Encrypted at rest.

#### 1.4.3.3.2. Type
TEXT

#### 1.4.3.3.3. Is Required
False

### 1.4.3.4. patientBirthDate
Patient's Birth Date (0010,0030). Encrypted at rest.

#### 1.4.3.4.2. Type
TEXT

#### 1.4.3.4.3. Is Required
False

### 1.4.3.5. patientSex
Patient's Sex (0010,0040). Encrypted at rest.

#### 1.4.3.5.2. Type
TEXT

#### 1.4.3.5.3. Is Required
False

### 1.4.3.6. createdAt
#### 1.4.3.6.2. Type
DateTime

#### 1.4.3.6.3. Is Required
True

#### 1.4.3.6.4. Default Value
NOW()

### 1.4.3.7. updatedAt
#### 1.4.3.7.2. Type
DateTime

#### 1.4.3.7.3. Is Required
True

#### 1.4.3.7.4. Default Value
NOW()


### 1.4.4. Primary Keys

- patientId

### 1.4.5. Indexes

### 1.4.5.1. idx_patient_name_gin_trgm
#### 1.4.5.1.2. Columns

- patientName

#### 1.4.5.1.3. Type
GIN

#### 1.4.5.1.4. Options
gin_trgm_ops

### 1.4.5.2. idx_patient_dicompatientid_gin_trgm
#### 1.4.5.2.2. Columns

- dicomPatientId

#### 1.4.5.2.3. Type
GIN

#### 1.4.5.2.4. Options
gin_trgm_ops


## 1.5. Study
Stores study-level DICOM metadata. (REQ-1-010)

### 1.5.3. Partitioning

- **Type:** RANGE
- **Columns:**
  
  - studyDate
  
- **Strategy:** Yearly or Quarterly

### 1.5.4. Attributes

### 1.5.4.1. studyId
#### 1.5.4.1.2. Type
Guid

#### 1.5.4.1.3. Is Required
True

#### 1.5.4.1.4. Is Primary Key
True

#### 1.5.4.1.5. Is Unique
True

### 1.5.4.2. patientId
Foreign key linking to the Patient record.

#### 1.5.4.2.2. Type
Guid

#### 1.5.4.2.3. Is Required
True

#### 1.5.4.2.4. Is Foreign Key
True

### 1.5.4.3. studyInstanceUid
Study Instance UID (0020,000D).

#### 1.5.4.3.2. Type
VARCHAR

#### 1.5.4.3.3. Is Required
True

#### 1.5.4.3.4. Is Unique
True

#### 1.5.4.3.5. Size
128

### 1.5.4.4. studyDate
Study Date (0008,0020).

#### 1.5.4.4.2. Type
DateTime

#### 1.5.4.4.3. Is Required
False

### 1.5.4.5. studyDescription
Study Description (0008,1030).

#### 1.5.4.5.2. Type
TEXT

#### 1.5.4.5.3. Is Required
False

### 1.5.4.6. accessionNumber
Accession Number (0008,0050). Encrypted at rest.

#### 1.5.4.6.2. Type
TEXT

#### 1.5.4.6.3. Is Required
False

### 1.5.4.7. referringPhysicianName
Referring Physician's Name (0008,0090). Encrypted at rest.

#### 1.5.4.7.2. Type
TEXT

#### 1.5.4.7.3. Is Required
False

### 1.5.4.8. createdAt
#### 1.5.4.8.2. Type
DateTime

#### 1.5.4.8.3. Is Required
True

#### 1.5.4.8.4. Default Value
NOW()


### 1.5.5. Primary Keys

- studyId

### 1.5.6. Unique Constraints

### 1.5.6.1. uq_study_studyinstanceuid
#### 1.5.6.1.2. Columns

- studyInstanceUid


### 1.5.7. Indexes

### 1.5.7.1. idx_study_patient_date
#### 1.5.7.1.2. Columns

- patientId
- studyDate DESC

#### 1.5.7.1.3. Type
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

#### 1.6.3.1.5. Is Unique
True

### 1.6.3.2. studyId
Foreign key linking to the Study record.

#### 1.6.3.2.2. Type
Guid

#### 1.6.3.2.3. Is Required
True

#### 1.6.3.2.4. Is Foreign Key
True

### 1.6.3.3. seriesInstanceUid
Series Instance UID (0020,000E).

#### 1.6.3.3.2. Type
VARCHAR

#### 1.6.3.3.3. Is Required
True

#### 1.6.3.3.4. Is Unique
True

#### 1.6.3.3.5. Size
128

### 1.6.3.4. modality
Modality (0008,0060).

#### 1.6.3.4.2. Type
VARCHAR

#### 1.6.3.4.3. Is Required
False

#### 1.6.3.4.4. Size
16

### 1.6.3.5. seriesNumber
Series Number (0020,0011).

#### 1.6.3.5.2. Type
INT

#### 1.6.3.5.3. Is Required
False

### 1.6.3.6. seriesDescription
Series Description (0008,103E).

#### 1.6.3.6.2. Type
TEXT

#### 1.6.3.6.3. Is Required
False

### 1.6.3.7. bodyPartExamined
Body Part Examined (0018,0015).

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

#### 1.6.3.8.4. Default Value
NOW()


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

#### 1.7.3.1.5. Is Unique
True

### 1.7.3.2. seriesId
Foreign key linking to the Series record.

#### 1.7.3.2.2. Type
Guid

#### 1.7.3.2.3. Is Required
True

#### 1.7.3.2.4. Is Foreign Key
True

### 1.7.3.3. sopInstanceUid
SOP Instance UID (0008,0018).

#### 1.7.3.3.2. Type
VARCHAR

#### 1.7.3.3.3. Is Required
True

#### 1.7.3.3.4. Is Unique
True

#### 1.7.3.3.5. Size
128

### 1.7.3.4. instanceNumber
Instance Number (0020,0013).

#### 1.7.3.4.2. Type
INT

#### 1.7.3.4.3. Is Required
False

### 1.7.3.5. filePath
The full path to the DICOM file on the storage system.

#### 1.7.3.5.2. Type
TEXT

#### 1.7.3.5.3. Is Required
True

### 1.7.3.6. fileSize
Size of the DICOM file in bytes.

#### 1.7.3.6.2. Type
BIGINT

#### 1.7.3.6.3. Is Required
True

### 1.7.3.7. isDeleted
Soft delete flag for data retention policies. (REQ-1-018)

#### 1.7.3.7.2. Type
BOOLEAN

#### 1.7.3.7.3. Is Required
True

#### 1.7.3.7.4. Default Value
false

### 1.7.3.8. createdAt
#### 1.7.3.8.2. Type
DateTime

#### 1.7.3.8.3. Is Required
True

#### 1.7.3.8.4. Default Value
NOW()


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

### 1.7.6.2. idx_image_active
#### 1.7.6.2.2. Columns

- seriesId

#### 1.7.6.2.3. Type
BTree

#### 1.7.6.2.4. Condition
isDeleted = false


## 1.8. PresentationState
Represents a DICOM Grayscale Softcopy Presentation State (GSPS) object, storing annotations and measurements. (REQ-1-061)

### 1.8.3. Attributes

### 1.8.3.1. presentationStateId
#### 1.8.3.1.2. Type
Guid

#### 1.8.3.1.3. Is Required
True

#### 1.8.3.1.4. Is Primary Key
True

#### 1.8.3.1.5. Is Unique
True

### 1.8.3.2. seriesId
Foreign key linking to the Series this presentation state applies to.

#### 1.8.3.2.2. Type
Guid

#### 1.8.3.2.3. Is Required
True

#### 1.8.3.2.4. Is Foreign Key
True

### 1.8.3.3. sopInstanceUid
SOP Instance UID of the GSPS object.

#### 1.8.3.3.2. Type
VARCHAR

#### 1.8.3.3.3. Is Required
True

#### 1.8.3.3.4. Is Unique
True

#### 1.8.3.3.5. Size
128

### 1.8.3.4. filePath
The full path to the GSPS DICOM file.

#### 1.8.3.4.2. Type
TEXT

#### 1.8.3.4.3. Is Required
True

### 1.8.3.5. createdByUserid
The user who created the annotations.

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

#### 1.8.3.6.4. Default Value
NOW()

### 1.8.3.7. updatedAt
#### 1.8.3.7.2. Type
DateTime

#### 1.8.3.7.3. Is Required
True

#### 1.8.3.7.4. Default Value
NOW()


### 1.8.4. Primary Keys

- presentationStateId

### 1.8.5. Unique Constraints

### 1.8.5.1. uq_presentationstate_sopinstanceuid
#### 1.8.5.1.2. Columns

- sopInstanceUid


## 1.9. AuditLog
Records a comprehensive trail of all significant system and user actions for security and compliance. (REQ-1-047, REQ-1-048, REQ-1-049)

### 1.9.3. Partitioning

- **Type:** RANGE
- **Columns:**
  
  - eventTimestamp
  
- **Strategy:** Monthly

### 1.9.4. Attributes

### 1.9.4.1. auditLogId
#### 1.9.4.1.2. Type
BIGINT

#### 1.9.4.1.3. Is Identity
True

#### 1.9.4.1.4. Is Required
True

#### 1.9.4.1.5. Is Primary Key
True

### 1.9.4.2. userId
The user who performed the action. Null for system-initiated events.

#### 1.9.4.2.2. Type
Guid

#### 1.9.4.2.3. Is Required
False

#### 1.9.4.2.4. Is Foreign Key
True

### 1.9.4.3. eventTimestamp
#### 1.9.4.3.2. Type
DateTime

#### 1.9.4.3.3. Is Required
True

#### 1.9.4.3.4. Default Value
NOW()

### 1.9.4.4. eventType
Type of event (e.g., 'UserLogin', 'DicomEdit', 'StudyDeleted').

#### 1.9.4.4.2. Type
VARCHAR

#### 1.9.4.4.3. Is Required
True

#### 1.9.4.4.4. Size
100

### 1.9.4.5. entityName
The name of the entity affected (e.g., 'User', 'Study').

#### 1.9.4.5.2. Type
VARCHAR

#### 1.9.4.5.3. Is Required
False

#### 1.9.4.5.4. Size
100

### 1.9.4.6. entityId
The primary key or UID of the affected entity.

#### 1.9.4.6.2. Type
VARCHAR

#### 1.9.4.6.3. Is Required
False

#### 1.9.4.6.4. Size
128

### 1.9.4.7. details
A JSON object containing event-specific details, such as old and new values for a modification.

#### 1.9.4.7.2. Type
JSONB

#### 1.9.4.7.3. Is Required
False

### 1.9.4.8. correlationId
ID to trace an operation across client, queue, and service. (REQ-1-090)

#### 1.9.4.8.2. Type
Guid

#### 1.9.4.8.3. Is Required
False


### 1.9.5. Primary Keys

- auditLogId

### 1.9.6. Indexes

### 1.9.6.1. idx_auditlog_user_timestamp
#### 1.9.6.1.2. Columns

- userId
- eventTimestamp DESC

#### 1.9.6.1.3. Type
BTree

### 1.9.6.2. idx_auditlog_event_timestamp
#### 1.9.6.2.2. Columns

- eventType
- eventTimestamp DESC

#### 1.9.6.2.3. Type
BTree

### 1.9.6.3. idx_auditlog_correlationid
#### 1.9.6.3.2. Columns

- correlationId

#### 1.9.6.3.3. Type
BTree


## 1.10. SystemSetting
A key-value store for global application settings configurable by an Administrator. (e.g., REQ-1-018, REQ-1-042, REQ-1-059)

### 1.10.3. Caching Strategy
Cache-aside on application startup with message queue invalidation

### 1.10.4. Attributes

### 1.10.4.1. settingKey
The unique key for the setting (e.g., 'DataRetentionDays').

#### 1.10.4.1.2. Type
VARCHAR

#### 1.10.4.1.3. Is Required
True

#### 1.10.4.1.4. Is Primary Key
True

#### 1.10.4.1.5. Size
100

### 1.10.4.2. settingValue
The value of the setting.

#### 1.10.4.2.2. Type
TEXT

#### 1.10.4.2.3. Is Required
True

### 1.10.4.3. description
Explanation of what the setting controls.

#### 1.10.4.3.2. Type
TEXT

#### 1.10.4.3.3. Is Required
False

### 1.10.4.4. updatedAt
#### 1.10.4.4.2. Type
DateTime

#### 1.10.4.4.3. Is Required
True

#### 1.10.4.4.4. Default Value
NOW()


### 1.10.5. Primary Keys

- settingKey

## 1.11. PacsConfiguration
Stores connection details for remote PACS servers. (REQ-1-038)

### 1.11.3. Caching Strategy
Cache-aside on application startup with message queue invalidation

### 1.11.4. Attributes

### 1.11.4.1. pacsConfigurationId
#### 1.11.4.1.2. Type
Guid

#### 1.11.4.1.3. Is Required
True

#### 1.11.4.1.4. Is Primary Key
True

### 1.11.4.2. aeTitle
The Application Entity Title of the remote PACS.

#### 1.11.4.2.2. Type
VARCHAR

#### 1.11.4.2.3. Is Required
True

#### 1.11.4.2.4. Size
16

### 1.11.4.3. hostname
The IP address or hostname of the remote PACS.

#### 1.11.4.3.2. Type
VARCHAR

#### 1.11.4.3.3. Is Required
True

#### 1.11.4.3.4. Size
255

### 1.11.4.4. port
The port number for DICOM communication.

#### 1.11.4.4.2. Type
INT

#### 1.11.4.4.3. Is Required
True

### 1.11.4.5. supportsCFind
#### 1.11.4.5.2. Type
BOOLEAN

#### 1.11.4.5.3. Is Required
True

#### 1.11.4.5.4. Default Value
true

### 1.11.4.6. supportsCMove
#### 1.11.4.6.2. Type
BOOLEAN

#### 1.11.4.6.3. Is Required
True

#### 1.11.4.6.4. Default Value
true

### 1.11.4.7. supportsCStore
#### 1.11.4.7.2. Type
BOOLEAN

#### 1.11.4.7.3. Is Required
True

#### 1.11.4.7.4. Default Value
true

### 1.11.4.8. createdAt
#### 1.11.4.8.2. Type
DateTime

#### 1.11.4.8.3. Is Required
True

#### 1.11.4.8.4. Default Value
NOW()

### 1.11.4.9. updatedAt
#### 1.11.4.9.2. Type
DateTime

#### 1.11.4.9.3. Is Required
True

#### 1.11.4.9.4. Default Value
NOW()


### 1.11.5. Primary Keys

- pacsConfigurationId

## 1.12. AutoRoutingRule
Defines rules for automatically routing incoming C-STORE studies to specific folders. (REQ-1-037)

### 1.12.3. Attributes

### 1.12.3.1. autoRoutingRuleId
#### 1.12.3.1.2. Type
Guid

#### 1.12.3.1.3. Is Required
True

#### 1.12.3.1.4. Is Primary Key
True

### 1.12.3.2. ruleName
#### 1.12.3.2.2. Type
VARCHAR

#### 1.12.3.2.3. Is Required
True

#### 1.12.3.2.4. Size
100

### 1.12.3.3. criteria
JSON object defining matching criteria (e.g., {"SendingAETitle": "MODALITY_AE", "Modality": "CT"}).

#### 1.12.3.3.2. Type
JSONB

#### 1.12.3.3.3. Is Required
True

### 1.12.3.4. destinationPath
The folder path where matching studies will be stored.

#### 1.12.3.4.2. Type
TEXT

#### 1.12.3.4.3. Is Required
True

### 1.12.3.5. priority
Order in which rules are evaluated (lower numbers first).

#### 1.12.3.5.2. Type
INT

#### 1.12.3.5.3. Is Required
True

#### 1.12.3.5.4. Default Value
0

### 1.12.3.6. isEnabled
#### 1.12.3.6.2. Type
BOOLEAN

#### 1.12.3.6.3. Is Required
True

#### 1.12.3.6.4. Default Value
true

### 1.12.3.7. createdAt
#### 1.12.3.7.2. Type
DateTime

#### 1.12.3.7.3. Is Required
True

#### 1.12.3.7.4. Default Value
NOW()

### 1.12.3.8. updatedAt
#### 1.12.3.8.2. Type
DateTime

#### 1.12.3.8.3. Is Required
True

#### 1.12.3.8.4. Default Value
NOW()


### 1.12.4. Primary Keys

- autoRoutingRuleId

### 1.12.5. Indexes

### 1.12.5.1. idx_autoroutingrule_criteria_gin
#### 1.12.5.1.2. Columns

- criteria

#### 1.12.5.1.3. Type
GIN


## 1.13. PrintJob
Represents a print job in the processing queue. (REQ-1-033)

### 1.13.3. Attributes

### 1.13.3.1. printJobId
#### 1.13.3.1.2. Type
Guid

#### 1.13.3.1.3. Is Required
True

#### 1.13.3.1.4. Is Primary Key
True

### 1.13.3.2. submittedByUserId
#### 1.13.3.2.2. Type
Guid

#### 1.13.3.2.3. Is Required
True

#### 1.13.3.2.4. Is Foreign Key
True

### 1.13.3.3. jobPayload
A JSON object containing all details for the print job, including layout, image references, and settings.

#### 1.13.3.3.2. Type
JSONB

#### 1.13.3.3.3. Is Required
True

### 1.13.3.4. status
Current status: Queued, Processing, Completed, Failed, Canceled.

#### 1.13.3.4.2. Type
VARCHAR

#### 1.13.3.4.3. Is Required
True

#### 1.13.3.4.4. Size
20

#### 1.13.3.4.5. Default Value
'Queued'

### 1.13.3.5. printerName
#### 1.13.3.5.2. Type
VARCHAR

#### 1.13.3.5.3. Is Required
True

#### 1.13.3.5.4. Size
255

### 1.13.3.6. failureReason
Descriptive reason if the job failed.

#### 1.13.3.6.2. Type
TEXT

#### 1.13.3.6.3. Is Required
False

### 1.13.3.7. priority
Job priority, allowing Admins to re-order the queue.

#### 1.13.3.7.2. Type
INT

#### 1.13.3.7.3. Is Required
True

#### 1.13.3.7.4. Default Value
0

### 1.13.3.8. submittedAt
#### 1.13.3.8.2. Type
DateTime

#### 1.13.3.8.3. Is Required
True

#### 1.13.3.8.4. Default Value
NOW()

### 1.13.3.9. processedAt
Timestamp when processing started or completed.

#### 1.13.3.9.2. Type
DateTime

#### 1.13.3.9.3. Is Required
False


### 1.13.4. Primary Keys

- printJobId

### 1.13.5. Indexes

### 1.13.5.1. idx_printjob_polling
#### 1.13.5.1.2. Columns

- status
- priority DESC
- submittedAt ASC

#### 1.13.5.1.3. Type
BTree


## 1.14. UserSession
Tracks active user sessions to support features like viewing active sessions and force logout. (REQ-1-019, REQ-1-041)

### 1.14.3. Caching Strategy
External distributed cache (Redis) recommended for high-traffic environments

### 1.14.4. Attributes

### 1.14.4.1. userSessionId
#### 1.14.4.1.2. Type
Guid

#### 1.14.4.1.3. Is Required
True

#### 1.14.4.1.4. Is Primary Key
True

### 1.14.4.2. userId
#### 1.14.4.2.2. Type
Guid

#### 1.14.4.2.3. Is Required
True

#### 1.14.4.2.4. Is Foreign Key
True

### 1.14.4.3. loginTimestamp
#### 1.14.4.3.2. Type
DateTime

#### 1.14.4.3.3. Is Required
True

#### 1.14.4.3.4. Default Value
NOW()

### 1.14.4.4. lastActivityTimestamp
#### 1.14.4.4.2. Type
DateTime

#### 1.14.4.4.3. Is Required
True

#### 1.14.4.4.4. Default Value
NOW()

### 1.14.4.5. ipAddress
#### 1.14.4.5.2. Type
VARCHAR

#### 1.14.4.5.3. Is Required
False

#### 1.14.4.5.4. Size
45

### 1.14.4.6. isActive
False if the session was terminated by logout or force logout.

#### 1.14.4.6.2. Type
BOOLEAN

#### 1.14.4.6.3. Is Required
True

#### 1.14.4.6.4. Default Value
true


### 1.14.5. Primary Keys

- userSessionId

## 1.15. HangingProtocol
Stores user-defined or system-wide display layouts (hanging protocols). (REQ-1-062, REQ-1-063)

### 1.15.3. Attributes

### 1.15.3.1. hangingProtocolId
#### 1.15.3.1.2. Type
Guid

#### 1.15.3.1.3. Is Required
True

#### 1.15.3.1.4. Is Primary Key
True

### 1.15.3.2. protocolName
#### 1.15.3.2.2. Type
VARCHAR

#### 1.15.3.2.3. Is Required
True

#### 1.15.3.2.4. Size
100

### 1.15.3.3. userId
The user who owns this protocol. Null for system-wide protocols.

#### 1.15.3.3.2. Type
Guid

#### 1.15.3.3.3. Is Required
False

#### 1.15.3.3.4. Is Foreign Key
True

### 1.15.3.4. layoutDefinition
JSON object defining the viewport layout (e.g., {"grid": "2x2"}).

#### 1.15.3.4.2. Type
JSONB

#### 1.15.3.4.3. Is Required
True

### 1.15.3.5. criteria
JSON object with matching criteria for auto-application (e.g., {"Modality": "CT", "BodyPartExamined": "CHEST"}).

#### 1.15.3.5.2. Type
JSONB

#### 1.15.3.5.3. Is Required
False

### 1.15.3.6. createdAt
#### 1.15.3.6.2. Type
DateTime

#### 1.15.3.6.3. Is Required
True

#### 1.15.3.6.4. Default Value
NOW()

### 1.15.3.7. updatedAt
#### 1.15.3.7.2. Type
DateTime

#### 1.15.3.7.3. Is Required
True

#### 1.15.3.7.4. Default Value
NOW()


### 1.15.4. Primary Keys

- hangingProtocolId

## 1.16. UserPreference
A key-value store for user-specific preferences, such as custom WW/WL presets or UI settings. (REQ-1-069)

### 1.16.3. Attributes

### 1.16.3.1. userId
#### 1.16.3.1.2. Type
Guid

#### 1.16.3.1.3. Is Required
True

#### 1.16.3.1.4. Is Foreign Key
True

#### 1.16.3.1.5. Is Primary Key
True

### 1.16.3.2. preferenceKey
The unique key for the preference (e.g., 'DefaultAnnotationColor').

#### 1.16.3.2.2. Type
VARCHAR

#### 1.16.3.2.3. Is Required
True

#### 1.16.3.2.4. Size
100

#### 1.16.3.2.6. Is Primary Key
True

### 1.16.3.3. preferenceValue
The value of the preference.

#### 1.16.3.3.2. Type
TEXT

#### 1.16.3.3.3. Is Required
True

### 1.16.3.4. updatedAt
#### 1.16.3.4.2. Type
DateTime

#### 1.16.3.4.3. Is Required
True

#### 1.16.3.4.4. Default Value
NOW()


### 1.16.4. Primary Keys

- userId
- preferenceKey



---

# 2. Relations

## 2.1. RoleHasUsers
### 2.1.3. Source Entity
Role

### 2.1.4. Target Entity
User

### 2.1.5. Type
OneToMany

### 2.1.6. Source Multiplicity
1

### 2.1.7. Target Multiplicity
0..*

### 2.1.8. Cascade Delete
False

### 2.1.9. Is Identifying
False

### 2.1.10. On Delete
Restrict

### 2.1.11. On Update
Cascade

## 2.2. UserHasPasswordHistory
### 2.2.3. Source Entity
User

### 2.2.4. Target Entity
PasswordHistory

### 2.2.5. Type
OneToMany

### 2.2.6. Source Multiplicity
1

### 2.2.7. Target Multiplicity
0..*

### 2.2.8. Cascade Delete
True

### 2.2.9. Is Identifying
True

### 2.2.10. On Delete
Cascade

### 2.2.11. On Update
Cascade

## 2.3. PatientHasStudies
### 2.3.3. Source Entity
Patient

### 2.3.4. Target Entity
Study

### 2.3.5. Type
OneToMany

### 2.3.6. Source Multiplicity
1

### 2.3.7. Target Multiplicity
0..*

### 2.3.8. Cascade Delete
True

### 2.3.9. Is Identifying
True

### 2.3.10. On Delete
Cascade

### 2.3.11. On Update
Cascade

## 2.4. StudyHasSeries
### 2.4.3. Source Entity
Study

### 2.4.4. Target Entity
Series

### 2.4.5. Type
OneToMany

### 2.4.6. Source Multiplicity
1

### 2.4.7. Target Multiplicity
1..*

### 2.4.8. Cascade Delete
True

### 2.4.9. Is Identifying
True

### 2.4.10. On Delete
Cascade

### 2.4.11. On Update
Cascade

## 2.5. SeriesHasImages
### 2.5.3. Source Entity
Series

### 2.5.4. Target Entity
Image

### 2.5.5. Type
OneToMany

### 2.5.6. Source Multiplicity
1

### 2.5.7. Target Multiplicity
1..*

### 2.5.8. Cascade Delete
True

### 2.5.9. Is Identifying
True

### 2.5.10. On Delete
Cascade

### 2.5.11. On Update
Cascade

## 2.6. SeriesHasPresentationStates
### 2.6.3. Source Entity
Series

### 2.6.4. Target Entity
PresentationState

### 2.6.5. Type
OneToMany

### 2.6.6. Source Multiplicity
1

### 2.6.7. Target Multiplicity
0..*

### 2.6.8. Cascade Delete
True

### 2.6.9. Is Identifying
True

### 2.6.10. On Delete
Cascade

### 2.6.11. On Update
Cascade

## 2.7. UserCreatedPresentationStates
### 2.7.3. Source Entity
User

### 2.7.4. Target Entity
PresentationState

### 2.7.5. Type
OneToMany

### 2.7.6. Source Multiplicity
1

### 2.7.7. Target Multiplicity
0..*

### 2.7.8. Cascade Delete
False

### 2.7.9. Is Identifying
False

### 2.7.10. On Delete
SetNull

### 2.7.11. On Update
Cascade

## 2.8. UserPerformedAuditedActions
### 2.8.3. Source Entity
User

### 2.8.4. Target Entity
AuditLog

### 2.8.5. Type
OneToMany

### 2.8.6. Source Multiplicity
0..1

### 2.8.7. Target Multiplicity
0..*

### 2.8.8. Cascade Delete
False

### 2.8.9. Is Identifying
False

### 2.8.10. On Delete
SetNull

### 2.8.11. On Update
Cascade

## 2.9. UserSubmittedPrintJobs
### 2.9.3. Source Entity
User

### 2.9.4. Target Entity
PrintJob

### 2.9.5. Type
OneToMany

### 2.9.6. Source Multiplicity
1

### 2.9.7. Target Multiplicity
0..*

### 2.9.8. Cascade Delete
False

### 2.9.9. Is Identifying
False

### 2.9.10. On Delete
SetNull

### 2.9.11. On Update
Cascade

## 2.10. UserHasSessions
### 2.10.3. Source Entity
User

### 2.10.4. Target Entity
UserSession

### 2.10.5. Type
OneToMany

### 2.10.6. Source Multiplicity
1

### 2.10.7. Target Multiplicity
0..*

### 2.10.8. Cascade Delete
True

### 2.10.9. Is Identifying
True

### 2.10.10. On Delete
Cascade

### 2.10.11. On Update
Cascade

## 2.11. UserOwnsHangingProtocols
### 2.11.3. Source Entity
User

### 2.11.4. Target Entity
HangingProtocol

### 2.11.5. Type
OneToMany

### 2.11.6. Source Multiplicity
0..1

### 2.11.7. Target Multiplicity
0..*

### 2.11.8. Cascade Delete
True

### 2.11.9. Is Identifying
False

### 2.11.10. On Delete
Cascade

### 2.11.11. On Update
Cascade

## 2.12. UserHasPreferences
### 2.12.3. Source Entity
User

### 2.12.4. Target Entity
UserPreference

### 2.12.5. Type
OneToMany

### 2.12.6. Source Multiplicity
1

### 2.12.7. Target Multiplicity
0..*

### 2.12.8. Cascade Delete
True

### 2.12.9. Is Identifying
True

### 2.12.10. On Delete
Cascade

### 2.12.11. On Update
Cascade



---

