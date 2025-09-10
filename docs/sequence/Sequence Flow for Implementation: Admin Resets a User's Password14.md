# Specification

# 1. Overview
## 1. Admin Resets a User's Password
An administrator selects a user from the User Management UI and initiates a password reset. The system generates a new secure temporary password, updates the user's record with the new hashed password, and flags the account to require a password change on next login. The action is logged in the audit trail.

### 1.1. Diagram Id
SEQ-BUP-014

### 1.4. Type
BusinessProcess

### 1.5. Purpose
To provide a secure and auditable method for an administrator to restore access for a user who has forgotten their password.

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

- Admin selects a user and clicks 'Reset Password'.
- A confirmation dialog is shown.
- Application service generates a new temporary password.
- PasswordHasher (BCrypt) creates a salted hash.
- The user record in the database is updated with the new hash and the 'force password change' flag.
- An 'User Password Reset' event is written to the audit trail.

### 1.11. Triggers

- Admin confirms the password reset action for a selected user (FR-3.9.2.1).

### 1.12. Outcomes

- The user's password is reset.
- The administrator is shown the new temporary password to provide to the user.
- The action is recorded in the audit trail for security and compliance.

### 1.13. Business Rules

- Only Administrators can perform password resets.
- The reset password must be temporary and force a change on next login (FR-3.3.2.2).

### 1.14. Error Scenarios

- Database is unavailable.
- Admin attempts to reset their own password through this flow (may be disallowed).

### 1.15. Integration Points

- PostgreSQL Database


---

# 2. Details
## 2. Implementation: Admin Resets a User's Password
A detailed technical sequence for an administrator resetting a user password. The process is initiated from the UI, orchestrated by the Application Service, and involves secure password generation/hashing, an atomic database transaction for updating the user record and creating an audit log entry, and finally providing feedback to the administrator.

### 2.1. Diagram Id
SEQ-BUP-014-IMPL

### 2.4. Participants

- **Repository Id:** User  
**Display Name:** Administrator  
**Type:** Actor  
**Technology:** Human  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #999999
    - **Stereotype:** User
    
- **Repository Id:** REPO-09-PRE  
**Display Name:** Presentation Layer  
**Type:** Layer  
**Technology:** WPF, .NET 8, MVVM  
**Order:** 2  
**Style:**
    
    - **Shape:** component
    - **Color:** #4285F4
    - **Stereotype:** User Interface
    
- **Repository Id:** REPO-08-APC  
**Display Name:** Application Service  
**Type:** ApplicationService  
**Technology:** .NET 8  
**Order:** 3  
**Style:**
    
    - **Shape:** component
    - **Color:** #34A853
    - **Stereotype:** Orchestrator
    
- **Repository Id:** REPO-12-CCC  
**Display Name:** Cross-Cutting Concerns  
**Type:** Utility  
**Technology:** .NET 8, BCrypt.Net-Next  
**Order:** 4  
**Style:**
    
    - **Shape:** component
    - **Color:** #FBBC05
    - **Stereotype:** Security & Logging
    
- **Repository Id:** REPO-10-DAC  
**Display Name:** Data Access Layer  
**Type:** DataAccess  
**Technology:** Entity Framework Core 8  
**Order:** 5  
**Style:**
    
    - **Shape:** component
    - **Color:** #EA4335
    - **Stereotype:** Repository
    
- **Repository Id:** EXT-PGSQL  
**Display Name:** PostgreSQL Database  
**Type:** Database  
**Technology:** PostgreSQL 16  
**Order:** 6  
**Style:**
    
    - **Shape:** database
    - **Color:** #336791
    - **Stereotype:** External System
    

### 2.5. Interactions

- **Source Id:** User  
**Target Id:** REPO-09-PRE  
**Message:** 1. Clicks 'Reset Password' for selected user 'jdoe'.  
**Sequence Number:** 1  
**Type:** UserAction  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** UI Event
    - **Method:** Button.Click
    - **Parameters:** SelectedUser: { UserId: '...', Username: 'jdoe' }
    - **Authentication:** Admin session already established.
    - **Error Handling:** N/A
    - **Performance:** Immediate UI feedback (<100ms).
    
- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-09-PRE  
**Message:** 2. UserManagementViewModel executes ResetPasswordCommand; calls DialogService.  
**Sequence Number:** 2  
**Type:** InternalMethodCall  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** IDialogService.ShowConfirmationAsync('Reset Password?', 'Are you sure...')
    - **Parameters:** Title, Message
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** Immediate.
    
- **Source Id:** User  
**Target Id:** REPO-09-PRE  
**Message:** 3. Confirms password reset in dialog.  
**Sequence Number:** 3  
**Type:** UserAction  
**Is Synchronous:** True  
**Return Message:** Returns confirmation result.  
**Has Return:** True  
**Technical Details:**
    
    - **Protocol:** UI Event
    - **Method:** ConfirmationDialog.Confirm
    - **Parameters:** Result: true
    - **Authentication:** N/A
    - **Error Handling:** If user cancels, the sequence terminates here.
    - **Performance:** Immediate.
    
- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-08-APC  
**Message:** 4. Calls user management service to initiate reset.  
**Sequence Number:** 4  
**Type:** MethodCall  
**Is Synchronous:** True  
**Return Message:** Returns result object with temporary password.  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process (DI)
    - **Method:** IUserManagementService.ResetUserPasswordAsync(userId)
    - **Parameters:** userId: Guid
    - **Authentication:** Service method is protected by an [Authorize(Role='Admin')] attribute.
    - **Error Handling:** ViewModel catches exceptions (e.g., DataAccessException) and displays an error notification.
    - **Performance:** End-to-end process target < 1 second.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-12-CCC  
**Message:** 5. Generates a new secure temporary password.  
**Sequence Number:** 5  
**Type:** MethodCall  
**Is Synchronous:** True  
**Return Message:** Returns plaintext temporary password.  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process (DI)
    - **Method:** ISecurePasswordGenerator.Generate()
    - **Parameters:** N/A
    - **Authentication:** N/A
    - **Error Handling:** Throws InvalidOperationException if CSPRNG is unavailable.
    - **Performance:** Target < 50ms.
    
- **Source Id:** REPO-12-CCC  
**Target Id:** REPO-08-APC  
**Message:** 6. Returns temporary password: 'tP@s5w0rd!xyz'  
**Sequence Number:** 6  
**Type:** Return  
**Is Synchronous:** True  
**Has Return:** True  
**Return Message:** plaintextPassword: string  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** 
    - **Parameters:** 
    - **Authentication:** 
    - **Error Handling:** 
    - **Performance:** 
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-12-CCC  
**Message:** 7. Hashes the new temporary password using BCrypt.  
**Sequence Number:** 7  
**Type:** MethodCall  
**Is Synchronous:** True  
**Return Message:** Returns BCrypt hash string.  
**Has Return:** True  
**Technical Details:**
    
    - **Protocol:** In-Process (DI)
    - **Method:** IPasswordHasher.HashPassword(plaintextPassword)
    - **Parameters:** plaintextPassword: 'tP@s5w0rd!xyz'
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** Dependent on BCrypt work factor; target < 200ms.
    
- **Source Id:** REPO-12-CCC  
**Target Id:** REPO-08-APC  
**Message:** 8. Returns hashed password  
**Sequence Number:** 8  
**Type:** Return  
**Is Synchronous:** True  
**Has Return:** True  
**Return Message:** hashedPassword: string  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** 
    - **Parameters:** 
    - **Authentication:** 
    - **Error Handling:** 
    - **Performance:** 
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-10-DAC  
**Message:** 9. Updates user record and logs audit event within a single transaction.  
**Sequence Number:** 9  
**Type:** MethodCall  
**Is Synchronous:** True  
**Return Message:** Returns success status.  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process (DI)
    - **Method:** IUserRepository.UpdatePasswordAndLogAuditAsync(...)
    - **Parameters:** userId: Guid, newHash: string, adminUserId: Guid, details: string
    - **Authentication:** N/A
    - **Error Handling:** Method manages a DB transaction, rolling back on any failure (user update or audit log insert). Throws DataAccessException.
    - **Performance:** Target < 500ms.
    
- **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 10. BEGIN TRANSACTION  
**Sequence Number:** 10  
**Type:** DatabaseQuery  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** SQL
    - **Method:** EF Core: context.Database.BeginTransactionAsync()
    - **Parameters:** 
    - **Authentication:** DB connection string credentials.
    - **Error Handling:** Throws NpgsqlException on connection failure.
    - **Performance:** 
    
- **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 11. EXECUTE UPDATE "Users" SET "PasswordHash" = @p0, "ForcePasswordChange" = TRUE WHERE "Id" = @p1  
**Sequence Number:** 11  
**Type:** DatabaseQuery  
**Is Synchronous:** True  
**Return Message:** Returns rows affected count.  
**Has Return:** True  
**Technical Details:**
    
    - **Protocol:** SQL
    - **Method:** EF Core: user.PasswordHash = ...; await context.SaveChangesAsync()
    - **Parameters:** Hashed password, userId
    - **Authentication:** N/A
    - **Error Handling:** Throws DbUpdateConcurrencyException or NpgsqlException.
    - **Performance:** Indexed query; target < 100ms.
    
- **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 12. EXECUTE INSERT INTO "AuditLogs" (UserId, EventType, ...) VALUES (@p0, 'UserPasswordReset', ...)  
**Sequence Number:** 12  
**Type:** DatabaseQuery  
**Is Synchronous:** True  
**Return Message:** Returns new row ID.  
**Has Return:** True  
**Technical Details:**
    
    - **Protocol:** SQL
    - **Method:** EF Core: context.AuditLogs.Add(...); await context.SaveChangesAsync()
    - **Parameters:** Audit event data object
    - **Authentication:** N/A
    - **Error Handling:** Throws NpgsqlException on insert failure.
    - **Performance:** Target < 100ms.
    
- **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 13. COMMIT TRANSACTION  
**Sequence Number:** 13  
**Type:** DatabaseQuery  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Technical Details:**
    
    - **Protocol:** SQL
    - **Method:** EF Core: transaction.CommitAsync()
    - **Parameters:** 
    - **Authentication:** N/A
    - **Error Handling:** If commit fails, an exception is thrown and logged.
    - **Performance:** 
    
- **Source Id:** REPO-10-DAC  
**Target Id:** REPO-08-APC  
**Message:** 14. Returns success  
**Sequence Number:** 14  
**Type:** Return  
**Is Synchronous:** True  
**Has Return:** True  
**Return Message:** success: true  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** 
    - **Parameters:** 
    - **Authentication:** 
    - **Error Handling:** 
    - **Performance:** 
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-09-PRE  
**Message:** 15. Returns result with temporary password  
**Sequence Number:** 15  
**Type:** Return  
**Is Synchronous:** True  
**Has Return:** True  
**Return Message:** Result { IsSuccess: true, Value: 'tP@s5w0rd!xyz' }  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** 
    - **Parameters:** 
    - **Authentication:** 
    - **Error Handling:** 
    - **Performance:** 
    
- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-09-PRE  
**Message:** 16. ViewModel calls NotificationService to display success.  
**Sequence Number:** 16  
**Type:** InternalMethodCall  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** INotificationService.ShowSuccess('Password reset', 'Temporary password: ...')
    - **Parameters:** Title, Message, Temporary Password
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** Immediate.
    
- **Source Id:** REPO-09-PRE  
**Target Id:** User  
**Message:** 17. Displays success notification with temporary password and warning.  
**Sequence Number:** 17  
**Type:** UIUpdate  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Technical Details:**
    
    - **Protocol:** UI Render
    - **Method:** Toast Notification
    - **Parameters:** Message content
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** Immediate.
    

### 2.6. Notes

- **Content:** Business Rule Enforcement: The entire operation is gated by an authorization check on the Application Service method, ensuring only Administrators can perform this action (FR-3.9.2.1).  
**Position:** top-left  
**Participant Id:** None  
**Sequence Number:** 4  
- **Content:** Atomicity Guarantee: The user password update and the audit log creation are performed within a single database transaction (Steps 10-13). If either step fails, the entire operation is rolled back, ensuring data consistency and a complete audit trail.  
**Position:** bottom-right  
**Participant Id:** REPO-10-DAC  
**Sequence Number:** 9  
- **Content:** Security Compliance: The system generates a temporary password and flags the account to require a password change on next login, fulfilling FR-3.3.2.2.  
**Position:** bottom-left  
**Participant Id:** REPO-08-APC  
**Sequence Number:** 11  

### 2.7. Implementation Guidance

- **Security Requirements:** Access to this functionality must be strictly limited to users with the 'Administrator' role, enforced at the Application Service layer. Passwords must never be logged in plaintext. The temporary password must be generated using a cryptographically secure pseudo-random number generator (CSPRNG). The audit trail entry is mandatory for compliance (FR-3.4.2.2).
- **Performance Targets:** The entire user-facing operation, from confirmation click (3) to success notification (17), should complete in under 1 second on standard hardware.
- **Error Handling Strategy:** If the database transaction fails for any reason (e.g., connection loss, constraint violation), the Application Service must catch the `DataAccessException` from the repository, log the error, and return a failure result to the Presentation Layer. The UI will then display a user-friendly error message, such as 'Failed to reset password due to a database error.'
- **Testing Considerations:** Unit tests should cover the password generation and hashing logic. Integration tests are critical to verify the transactional behavior of the `UpdatePasswordAndLogAuditAsync` method, including a scenario where the audit log insert fails to ensure the user password update is rolled back. End-to-end tests should validate the complete flow from the UI.
- **Monitoring Requirements:** Log the initiation of the password reset process at INFO level. Log successful completion at INFO level, including the admin's user ID and the target user's ID. Any exception during the process, especially database errors, must be logged at ERROR level with full exception details.
- **Deployment Considerations:** Ensure the `Users` table has a `ForcePasswordChange` (boolean) column and the `AuditLogs` table exists, as managed by EF Core migrations. Ensure database connection strings and other secrets are managed securely via the Windows Credential Manager (REQ-NFR-004).


---

