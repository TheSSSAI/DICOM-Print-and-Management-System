# Specification

# 1. Overview
## 1. Administrator Creates a New User Account
An administrator uses the User Management UI to create a new user. The system validates the input, generates a secure temporary password, hashes it using BCrypt, and stores the new user record. The account is flagged to require a password change on the first login. The user creation and audit log entry are performed in a single database transaction.

### 1.1. Diagram Id
SEQ-BUP-004

### 1.4. Type
BusinessProcess

### 1.5. Purpose
To provision system access for new staff members in a secure and auditable manner.

### 1.6. Complexity
Medium

### 1.7. Priority
High

### 1.8. Frequency
OnDemand

### 1.9. Participants

- REPO-09-PRE
- REPO-08-APC
- REPO-10-DAC
- EXT-PGSQL
- REPO-12-CCC

### 1.10. Key Interactions

- Admin fills out the 'Add User' form.
- Application service validates that the username is unique.
- A temporary password is generated.
- PasswordHasher (BCrypt) creates a salted hash of the password.
- Within a single database transaction: a new user record is created with the role and 'force password change' flag, and an audit log entry is created for the 'User Created' event.
- The transaction is committed.

### 1.11. Triggers

- Admin submits the 'Add User' form (FR-3.9.2.1).

### 1.12. Outcomes

- A new user account is created and persisted atomically.
- The administrator is shown the temporary password to securely provide to the user.
- The creation event is logged for compliance.

### 1.13. Business Rules

- Usernames must be unique.
- New users must change their password on first login (FR-3.3.2.2).
- All user management actions must be audited (FR-3.4.2.2).

### 1.14. Error Scenarios

- The chosen username already exists, causing a unique constraint violation.
- The database is unavailable, causing the transaction to be rolled back.

### 1.15. Integration Points

- PostgreSQL Database


---

# 2. Details
## 2. Implementation-Ready: Administrator Creates New User Account
This sequence details the end-to-end technical process for an Administrator creating a new user account. It covers UI interaction, application service orchestration, business rule validation (username uniqueness), secure password generation and hashing (BCrypt), and the critical atomic database transaction that persists both the new user record and the corresponding audit log entry. The process ensures data integrity through transaction management and provides immediate feedback to the administrator.

### 2.1. Diagram Id
SEQ-BUP-004

### 2.4. Participants

- **Repository Id:** REPO-09-PRE  
**Display Name:** Presentation Layer (ViewModel)  
**Type:** UI/ViewModel  
**Technology:** WPF, .NET 8, MVVM  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #4CAF50
    - **Stereotype:** UI
    
- **Repository Id:** REPO-08-APC  
**Display Name:** Application Service  
**Type:** Service Layer  
**Technology:** .NET 8, C# 12  
**Order:** 2  
**Style:**
    
    - **Shape:** component
    - **Color:** #2196F3
    - **Stereotype:** Service
    
- **Repository Id:** REPO-12-CCC  
**Display Name:** Password Hasher Utility  
**Type:** Utility Component  
**Technology:** BCrypt.Net-Next  
**Order:** 3  
**Style:**
    
    - **Shape:** component
    - **Color:** #FFC107
    - **Stereotype:** Utility
    
- **Repository Id:** REPO-10-DAC  
**Display Name:** Data Access Layer  
**Type:** Repository Layer  
**Technology:** Entity Framework Core 8  
**Order:** 4  
**Style:**
    
    - **Shape:** component
    - **Color:** #9C27B0
    - **Stereotype:** Repository
    
- **Repository Id:** EXT-PGSQL  
**Display Name:** PostgreSQL Database  
**Type:** Database  
**Technology:** PostgreSQL 16  
**Order:** 5  
**Style:**
    
    - **Shape:** database
    - **Color:** #795548
    - **Stereotype:** External
    

### 2.5. Interactions

- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-08-APC  
**Message:** 1. CreateUserAsync(username: string, role: UserRole)  
**Sequence Number:** 1  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** 17. (isSuccess: bool, temporaryPassword?: string)  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** Direct Method Call
    - **Method:** IUserManagementService.CreateUserAsync
    - **Parameters:** UserCreateRequestDTO { Username: 'new.user', Role: 'Technician' }
    - **Authentication:** Requires authenticated Admin principal. Checked via attribute or middleware.
    - **Error Handling:** Catches and handles ValidationException (e.g., duplicate user), InfrastructureException (e.g., DB offline) and translates to ViewModel state.
    - **Performance:** End-to-end operation should complete < 500ms.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-10-DAC  
**Message:** 2. CheckUsernameExistsAsync(username: string)  
**Sequence Number:** 2  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** 5. false  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Direct Method Call
    - **Method:** IUserRepository.GetByUsernameAsync
    - **Parameters:** 'new.user'
    - **Authentication:** N/A (Internal Call)
    - **Error Handling:** Propagates DbException for connection issues. Throws ValidationException if user is found.
    - **Performance:** DB query latency should be < 50ms.
    
- **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 3. SELECT 1 FROM "Users" WHERE "Username" = @username LIMIT 1  
**Sequence Number:** 3  
**Type:** Database Query  
**Is Synchronous:** True  
**Return Message:** 4. No rows returned  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** SQL/Npgsql
    - **Method:** SELECT
    - **Parameters:** @username = 'new.user'
    - **Authentication:** Via connection string credentials.
    - **Error Handling:** Database driver throws NpgsqlException on connection failure.
    - **Performance:** Query must use an index on the 'Username' column.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-12-CCC  
**Message:** 6. HashPassword(temporaryPassword: string)  
**Sequence Number:** 6  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** 7. hashedPassword: string  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Direct Method Call
    - **Method:** IPasswordHasher.HashPassword
    - **Parameters:** A cryptographically secure, randomly generated string.
    - **Authentication:** N/A (Internal Call)
    - **Error Handling:** N/A
    - **Performance:** BCrypt hashing is CPU-bound; must be fast enough not to block.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-10-DAC  
**Message:** 8. CreateUserWithAuditAsync(user: User, audit: AuditLog)  
**Sequence Number:** 8  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** 15. createdUser: User  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** Direct Method Call
    - **Method:** IUserRepository.CreateUserWithAuditAsync
    - **Parameters:** User entity with hashed password and 'ForcePasswordChange'=true; AuditLog entity with 'EventType'='UserCreated'
    - **Authentication:** N/A (Internal Call)
    - **Error Handling:** Wraps EF Core's transaction management. Rolls back transaction on any exception and re-throws as an InfrastructureException.
    - **Performance:** Transaction should be short-lived to avoid holding locks.
    
- **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 9. BEGIN TRANSACTION  
**Sequence Number:** 9  
**Type:** Database Command  
**Is Synchronous:** True  
**Return Message:** 10. OK  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** SQL/Npgsql
    - **Method:** BEGIN
    - **Parameters:** N/A
    - **Authentication:** N/A
    - **Error Handling:** Managed by Entity Framework Core transaction strategy.
    - **Performance:** N/A
    
- **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 11. INSERT INTO "Users" (Username, PasswordHash, Role, ForcePasswordChange) VALUES (...)  
**Sequence Number:** 11  
**Type:** Database Command  
**Is Synchronous:** True  
**Return Message:** 12. 1 row affected  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** SQL/Npgsql
    - **Method:** INSERT
    - **Parameters:** Values including username, BCrypt hash, role, and true.
    - **Authentication:** N/A
    - **Error Handling:** A unique constraint violation on 'Username' will cause the transaction to fail and be rolled back.
    - **Performance:** N/A
    
- **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 13. INSERT INTO "AuditLog" (UserId, EventType, EntityName, Details) VALUES (...)  
**Sequence Number:** 13  
**Type:** Database Command  
**Is Synchronous:** True  
**Return Message:** 14. 1 row affected  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** SQL/Npgsql
    - **Method:** INSERT
    - **Parameters:** Values linking the Admin's ID to the 'UserCreated' event.
    - **Authentication:** N/A
    - **Error Handling:** Any insertion failure will cause the transaction to fail and be rolled back.
    - **Performance:** N/A
    
- **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 15. COMMIT TRANSACTION  
**Sequence Number:** 15  
**Type:** Database Command  
**Is Synchronous:** True  
**Return Message:** 16. OK  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** SQL/Npgsql
    - **Method:** COMMIT
    - **Parameters:** N/A
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** N/A
    

### 2.6. Notes

- **Content:** Business Rule: FR-3.9.2.1. Admin initiates user creation from the UI.  
**Position:** Top  
**Participant Id:** REPO-09-PRE  
**Sequence Number:** 1  
- **Content:** Business Rule: Usernames must be unique. This check prevents a unique constraint violation in the database.  
**Position:** Middle  
**Participant Id:** REPO-08-APC  
**Sequence Number:** 2  
- **Content:** Security: FR-3.3.2.2. A secure, random temporary password is generated in memory.  
**Position:** Middle  
**Participant Id:** REPO-08-APC  
**Sequence Number:** 6  
- **Content:** Security: REQ-NFR-004. Password is not stored in plaintext. It is hashed using BCrypt, which includes a salt.  
**Position:** Middle  
**Participant Id:** REPO-12-CCC  
**Sequence Number:** 7  
- **Content:** Data Integrity: The creation of the User and its corresponding AuditLog entry are performed within a single atomic database transaction to prevent inconsistent states.  
**Position:** Bottom  
**Participant Id:** REPO-10-DAC  
**Sequence Number:** 9  
- **Content:** UI Feedback: The ViewModel receives the temporary password and displays it in a success notification, advising the Admin to share it securely.  
**Position:** Bottom  
**Participant Id:** REPO-09-PRE  
**Sequence Number:** 17  

### 2.7. Implementation Guidance

- **Security Requirements:** Access to the 'CreateUserAsync' endpoint must be strictly limited to users with the 'Administrator' role. The temporary password must be generated using a cryptographically secure random number generator. The password hash must be generated using BCrypt with a work factor of at least 12.
- **Performance Targets:** The entire user creation process, from UI submission to database commit, must complete in under 500ms under normal load conditions. The username uniqueness check requires an index on the 'Username' column in the 'Users' table to ensure performance.
- **Error Handling Strategy:** If the username already exists, the Application Service must throw a specific 'DuplicateUsernameException' which the Presentation Layer catches and translates into a user-friendly error message. If the database transaction fails for any reason (e.g., connection lost, constraint violation), the Data Access Layer must ensure the transaction is rolled back, leaving the database in a consistent state. An 'InfrastructureException' should be thrown upwards.
- **Testing Considerations:** Unit tests should cover the Application Service logic, including password generation and calls to dependencies. Integration tests are critical to verify the transactional behavior of the Data Access Layer, ensuring both user and audit log are created or neither are. Security testing must validate that passwords are never logged and are correctly hashed in the database. End-to-end tests should cover the full happy path and the duplicate username error case.
- **Monitoring Requirements:** Successful user creation events should be logged at an 'Information' level. Any exceptions, such as database failures or validation errors, should be logged at an 'Error' level, including a correlation ID that traces the initial request from the Presentation Layer.
- **Deployment Considerations:** Ensure that database migrations for creating the 'Users' and 'AuditLog' tables, including the unique constraint on 'Users.Username', have been successfully applied before deploying this feature.


---

