# Specification

# 1. Overview
## 1. Admin Views and Exports Audit Trail
An administrator navigates to the Audit Trail viewer within the Administration area. They can filter the log by user and date range. The system queries the database and displays the results. The administrator can then export the filtered view as a CSV file.

### 1.1. Diagram Id
SEQ-CPL-016

### 1.4. Type
ComplianceFlow

### 1.5. Purpose
To enable review and reporting of auditable events for security investigations and regulatory compliance.

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
- REPO-11-INF

### 1.10. Key Interactions

- Admin accesses the 'Audit Trail' section.
- Admin enters filter criteria (e.g., user='bsmith', date_range='last 7 days') and applies them.
- The application service requests the filtered data from the AuditLogRepository.
- The repository executes a query against the PostgreSQL database.
- The results are returned and displayed in a data grid in the UI.
- Admin clicks 'Export to CSV', and the system generates and saves the file.

### 1.11. Triggers

- Admin accesses the audit trail viewer (FR-3.4.2.3).

### 1.12. Outcomes

- The administrator can view a filtered list of all auditable events.
- A CSV export of the audit trail is created for external reporting or archival.

### 1.13. Business Rules

- Only users with the Admin role can view the audit trail (REQ-USR-001).
- The export must be in CSV format (FR-3.4.2.3).

### 1.14. Error Scenarios

- Database query fails.
- File system error prevents saving the CSV file.

### 1.15. Integration Points

- PostgreSQL Database
- Local File System


---

# 2. Details
## 2. Admin Creates New User Account with Audit
Technical sequence for an Administrator provisioning a new user account. The process includes input validation, secure temporary password generation and hashing, atomic database insertion of the user and a corresponding audit trail entry, and secure feedback to the administrator. This flow is critical for maintaining a secure and compliant user lifecycle management process.

### 2.1. Diagram Id
SEQ-CPL-017

### 2.4. Participants

- **Repository Id:** USER  
**Display Name:** Administrator  
**Type:** Actor  
**Technology:** Human  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #999999
    - **Stereotype:** 
    
- **Repository Id:** REPO-09-PRE  
**Display Name:** Presentation Layer  
**Type:** UI Component  
**Technology:** WPF / MVVM  
**Order:** 2  
**Style:**
    
    - **Shape:** component
    - **Color:** #1168BD
    - **Stereotype:** UserManagementViewModel
    
- **Repository Id:** REPO-08-APC  
**Display Name:** Application Service  
**Type:** Service Layer  
**Technology:** .NET 8 Service  
**Order:** 3  
**Style:**
    
    - **Shape:** component
    - **Color:** #11BD52
    - **Stereotype:** UserManagementService
    
- **Repository Id:** REPO-04-SEC  
**Display Name:** Security Infrastructure  
**Type:** Infrastructure Component  
**Technology:** BCrypt.Net-Next  
**Order:** 4  
**Style:**
    
    - **Shape:** component
    - **Color:** #F28C38
    - **Stereotype:** IPasswordHasher
    
- **Repository Id:** REPO-10-DAC  
**Display Name:** Data Access Layer  
**Type:** Repository  
**Technology:** Entity Framework Core 8  
**Order:** 5  
**Style:**
    
    - **Shape:** component
    - **Color:** #F2C038
    - **Stereotype:** UserRepository, AuditLogRepository
    
- **Repository Id:** EXT-PGSQL  
**Display Name:** PostgreSQL Database  
**Type:** Database  
**Technology:** PostgreSQL 16  
**Order:** 6  
**Style:**
    
    - **Shape:** database
    - **Color:** #8C38F2
    - **Stereotype:** 
    

### 2.5. Interactions

- **Source Id:** USER  
**Target Id:** REPO-09-PRE  
**Message:** 1. Fills 'Add User' form (username, role) and clicks 'Create'  
**Sequence Number:** 1  
**Type:** User Input  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** WPF Events
    - **Method:** CreateUserCommand.Execute
    - **Parameters:** UserCreationModel { Username: 'newuser', Role: 'Technician' }
    - **Authentication:** Requires active, authenticated Admin session.
    - **Error Handling:** Client-side validation prevents submission if fields are empty.
    - **Performance:**
      
      - **Latency:** <100ms UI response
      
    
- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-08-APC  
**Message:** 2. CreateUserAsync(username, role)  
**Sequence Number:** 2  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** 404: UserCreationResult { IsSuccess: true, TemporaryPassword: '...' }  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** IUserManagementService.CreateUserAsync
    - **Parameters:** string username, UserRole role
    - **Authentication:** Service method should be protected by an authorization attribute checking for 'Admin' role.
    - **Error Handling:** Catches exceptions (e.g., DuplicateUsernameException) and translates them into a user-friendly result object.
    - **Performance:**
      
      - **Latency:** <500ms for entire operation
      
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-10-DAC  
**Message:** 3. [Validation] GetByUsernameAsync(username)  
**Sequence Number:** 3  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** 6. User object or null  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** IUserRepository.GetByUsernameAsync
    - **Parameters:** string username
    - **Authentication:** N/A (Internal call)
    - **Error Handling:** Handles DbContext exceptions.
    - **Performance:**
      
      - **Latency:** <50ms (requires index on username column)
      
    
- **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 4. SELECT * FROM users WHERE username = @p0  
**Sequence Number:** 4  
**Type:** Database Query  
**Is Synchronous:** True  
**Return Message:** 5. Returns 0 or 1 row  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Npgsql
    - **Method:** SQL SELECT
    - **Parameters:** username
    - **Authentication:** DB connection string credentials
    - **Error Handling:** Database connection errors are propagated as exceptions.
    - **Performance:**
      
      - **Latency:** <20ms
      
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-04-SEC  
**Message:** 7. HashPassword(plainTextPassword)  
**Sequence Number:** 7  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** 8. Hashed password string (BCrypt format)  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** IPasswordHasher.HashPassword
    - **Parameters:** string plainTextPassword (generated securely)
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:**
      
      - **Latency:** 50-100ms (BCrypt is intentionally slow)
      
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-10-DAC  
**Message:** 9. [Transaction] CreateUserWithAuditAsync(userEntity, auditLogEntity)  
**Sequence Number:** 9  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** 12. void (or throws exception)  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** IUnitOfWork.SaveChangesAsync / Custom Repository Method
    - **Parameters:** User user, AuditLog auditLog
    - **Authentication:** N/A
    - **Error Handling:** The entire operation is within a single database transaction. Failure of either insert causes a full rollback.
    - **Performance:**
      
      - **Latency:** <100ms
      
    
**Nested Interactions:**
    
    - **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 10. BEGIN TRANSACTION; INSERT INTO users (...); INSERT INTO audit_logs (...);  
**Sequence Number:** 10  
**Type:** Database Transaction  
**Is Synchronous:** True  
**Return Message:** 11. COMMIT TRANSACTION;  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Npgsql
    - **Method:** SQL INSERT
    - **Parameters:** User and AuditLog data
    - **Authentication:** DB connection string credentials
    - **Error Handling:** Unique constraint violation on 'username' will cause transaction to fail and roll back.
    - **Performance:**
      
      - **Latency:** Dependent on disk I/O
      
    
    
- **Source Id:** REPO-09-PRE  
**Target Id:** USER  
**Message:** 13. Displays success notification: 'User created. Temporary password: ...'  
**Sequence Number:** 13  
**Type:** UI Update  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** WPF UI Render
    - **Method:** NotificationService.ShowSuccess
    - **Parameters:** string message
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:**
      
      - **Latency:** <100ms
      
    

### 2.6. Notes

- **Content:** If username exists at step 6, the Application Service throws a DuplicateUsernameException. The Presentation Layer catches this and displays an inline error message. The sequence terminates.  
**Position:** top  
**Participant Id:** REPO-08-APC  
**Sequence Number:** 6  
- **Content:** A cryptographically secure random number generator must be used to create the temporary password before step 7.  
**Position:** bottom  
**Participant Id:** REPO-08-APC  
**Sequence Number:** 7  
- **Content:** The AuditLog entity created before step 9 must contain the Admin's UserID, EventType ('User Created'), TargetEntity ('User'), TargetEntityID (the new user's ID), and a timestamp, satisfying REQ-FNC-004.  
**Position:** bottom  
**Participant Id:** REPO-08-APC  
**Sequence Number:** 9  
- **Content:** The new User entity must have the 'force_password_change' flag set to true, satisfying REQ-FNC-003.  
**Position:** bottom  
**Participant Id:** REPO-10-DAC  
**Sequence Number:** 10  

### 2.7. Implementation Guidance

- **Security Requirements:** Access to the 'Create User' functionality must be strictly limited to users with the 'Administrator' role, enforced at both the UI layer (hiding controls) and the Application Service layer (authorization attribute). The temporary password must never be logged. The success notification must warn the Admin to transmit the password securely.
- **Performance Targets:** The entire end-to-end user creation process, from button click to success notification, should complete in under 500ms on standard hardware. The database must have a unique index on the `users.username` column to ensure fast duplicate checks.
- **Error Handling Strategy:** Handle duplicate usernames by catching a specific `DuplicateUsernameException` and displaying a user-friendly error. Handle database unavailability or transaction failures with a generic 'Failed to create user' error message, while logging the detailed exception for administrative review. All database operations for a single creation must be atomic.
- **Testing Considerations:** Unit tests should cover the Application Service logic, including the duplicate username check and password hashing calls. Integration tests must verify that a user and a corresponding audit log entry are created within a single transaction, and that the transaction is rolled back if either insert fails. Security testing should attempt to access the feature as a non-Admin user and verify access is denied.
- **Monitoring Requirements:** Successful user creation events should be logged at an 'Information' level. Failed attempts due to database errors should be logged at an 'Error' level. The content of the log must include the responsible Admin's ID and the username of the user being created, but not the password.
- **Deployment Considerations:** Ensure the `users` table in the database has the unique constraint on the `username` column applied before deployment.


---

