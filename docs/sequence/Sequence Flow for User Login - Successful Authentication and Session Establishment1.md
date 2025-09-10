# Specification

# 1. Overview
## 1. User Login - Successful Authentication
A user provides valid credentials to the WPF client's LoginView. The client's AuthenticationService validates these credentials against the local PostgreSQL database. Upon success, the PasswordHasher confirms the password hash, the UserRepository retrieves the user's role, and a user session object is created. This session initializes the application's security context, granting access and enabling UI features based on the user's role-based permissions.

### 1.1. Diagram Id
SEQ-AFL-001

### 1.4. Type
AuthenticationFlow

### 1.5. Purpose
To securely authenticate a user and establish a valid session with appropriate role-based permissions.

### 1.6. Complexity
Medium

### 1.7. Priority
Critical

### 1.8. Frequency
OnDemand

### 1.9. Participants

- REPO-09-PRE
- REPO-08-APC
- REPO-10-DAC
- EXT-PGSQL
- REPO-12-CCC

### 1.10. Key Interactions

- User enters credentials in LoginView.
- AuthenticationService in the Application Layer receives credentials.
- PasswordHasher (BCrypt) verifies the provided password against the stored hash.
- UserRepository retrieves user and role data from PostgreSQL.
- AuthenticationService returns a user session object upon success.
- The session is established and the main UI is configured for the user's role.

### 1.11. Triggers

- User clicks the 'Login' button.

### 1.12. Outcomes

- User is successfully logged in.
- The application's main window is displayed with features enabled/disabled according to the user's role.
- An active user session is created and managed by the application service.

### 1.13. Business Rules

- Access is granted only with valid, active credentials.
- UI elements must be enabled/disabled based on the logged-in user's role (FR-3.3.2).

### 1.14. Error Scenarios

- Database is unavailable.
- User account is disabled or locked.

### 1.15. Integration Points

- PostgreSQL Database


---

# 2. Details
## 2. User Login - Successful Authentication and Session Establishment
This sequence details the complete technical flow for a successful user authentication against the local PostgreSQL database. The process begins with the user submitting credentials via the WPF LoginViewModel. The Application Service orchestrates the validation by fetching the user's record via the Entity Framework Core-based repository and verifying the password hash using the BCrypt utility. Upon successful verification, a security audit event is logged, and a user session is created and returned to the presentation layer, which then establishes the application's security context and initializes the main UI according to the user's role-based permissions.

### 2.1. Diagram Id
SEQ-AFL-001

### 2.4. Participants

- **Repository Id:** REPO-09-PRE  
**Display Name:** LoginViewModel  
**Type:** ViewModel  
**Technology:** WPF / .NET 8  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #4287f5
    - **Stereotype:** Presentation
    
- **Repository Id:** REPO-08-APC  
**Display Name:** AuthenticationService  
**Type:** Application Service  
**Technology:** .NET 8 / C# 12  
**Order:** 2  
**Style:**
    
    - **Shape:** participant
    - **Color:** #f5a742
    - **Stereotype:** Application
    
- **Repository Id:** REPO-10-DAC  
**Display Name:** UserRepository  
**Type:** Repository  
**Technology:** Entity Framework Core 8  
**Order:** 3  
**Style:**
    
    - **Shape:** participant
    - **Color:** #8d42f5
    - **Stereotype:** Data Access
    
- **Repository Id:** EXT-PGSQL  
**Display Name:** PostgreSQL DB  
**Type:** Database  
**Technology:** PostgreSQL 16  
**Order:** 4  
**Style:**
    
    - **Shape:** database
    - **Color:** #2c786c
    - **Stereotype:** External System
    
- **Repository Id:** REPO-12-CCC  
**Display Name:** PasswordHasher  
**Type:** Utility  
**Technology:** BCrypt.Net-Next  
**Order:** 5  
**Style:**
    
    - **Shape:** participant
    - **Color:** #f542a7
    - **Stereotype:** Cross-Cutting
    

### 2.5. Interactions

- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-08-APC  
**Message:** 1. LoginAsync(username, password)  
**Sequence Number:** 1  
**Type:** Method Call  
**Is Synchronous:** False  
**Return Message:** 10. return UserSession object  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Method Call
    - **Method:** Task<UserSession> LoginAsync(string username, string password)
    - **Parameters:** Username and plaintext password from user input.
    - **Authentication:** N/A
    - **Error Handling:** Catches AuthenticationException and translates it into a user-friendly error message for the view.
    - **Performance:** End-to-end latency for the entire login process should be < 500ms.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-10-DAC  
**Message:** 2. GetUserByUsernameAsync(username)  
**Sequence Number:** 2  
**Type:** Method Call  
**Is Synchronous:** False  
**Return Message:** 5. return User domain object  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Method Call
    - **Method:** Task<User> GetUserByUsernameAsync(string username)
    - **Parameters:** The username to be queried.
    - **Authentication:** N/A
    - **Error Handling:** Throws UserNotFoundException if no user matches the username. Throws DataAccessException for database connectivity issues.
    - **Performance:** P99 latency should be < 50ms.
    
- **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** 3. Execute SQL Query  
**Sequence Number:** 3  
**Type:** Database Query  
**Is Synchronous:** True  
**Return Message:** 4. Return user record  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** TCP/IP (PostgreSQL Wire Protocol)
    - **Method:** SELECT u."UserId", u."Username", u."PasswordHash", u."Role" FROM "Users" AS u WHERE u."Username" = @username LIMIT 1
    - **Parameters:** Sanitized username parameter.
    - **Authentication:** Credentials from secure connection string (managed via Windows Credential Manager). Connection must use TLS.
    - **Error Handling:** Database connection errors are propagated as NpgsqlException, which is caught and wrapped by the repository.
    - **Performance:** Query execution must be optimized with an index on the 'Username' column. P99 latency < 20ms.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-12-CCC  
**Message:** 6. VerifyPassword(password, user.PasswordHash)  
**Sequence Number:** 6  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** 7. return true  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Method Call
    - **Method:** bool Verify(string text, string hash)
    - **Parameters:** Plaintext password and the BCrypt hash fetched from the database.
    - **Authentication:** N/A
    - **Error Handling:** Returns false for an incorrect password. May throw exceptions for malformed hash strings.
    - **Performance:** This is a CPU-bound operation. P99 latency should be < 100ms on target hardware.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-10-DAC  
**Message:** 8. LogAuditEventAsync(auditEvent)  
**Sequence Number:** 8  
**Type:** Method Call  
**Is Synchronous:** False  
**Return Message:** 9. return Task completion  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Process Method Call
    - **Method:** Task LogAuditEventAsync(AuditEvent auditEvent)
    - **Parameters:** An AuditEvent object containing: EventType='LoginSuccess', UserId, Timestamp, etc.
    - **Authentication:** N/A
    - **Error Handling:** Logging failures should be handled gracefully (logged to a file) and must not prevent the user from successfully logging in.
    - **Performance:** The call is awaited but should not significantly impact the overall login time.
    

### 2.6. Notes

- **Content:** Upon receiving the UserSession object, the LoginViewModel is responsible for establishing the application's security context (e.g., setting Thread.CurrentPrincipal) and navigating the user to the main application window.  
**Position:** bottom-left  
**Participant Id:** REPO-09-PRE  
**Sequence Number:** 10  
- **Content:** The entire flow from the ViewModel's perspective is asynchronous to ensure the UI remains responsive during the authentication process.  
**Position:** top-right  
**Sequence Number:** 1  

### 2.7. Implementation Guidance

- **Security Requirements:** User passwords must be securely hashed using BCrypt as per REQ-NFR-004. Database connection strings must be stored securely using Windows Credential Manager, not in plaintext config files (REQ-NFR-004). All communication with the PostgreSQL database must be encrypted using TLS (REQ-NFR-004). A security audit trail of all successful login events must be created as per FR-3.4.2.2.
- **Performance Targets:** The end-to-end authentication process (from button click to main window display) must complete in under 500ms on target hardware with an SSD. The database query must utilize an index on the 'Username' column to ensure high performance.
- **Error Handling Strategy:** If the database is unavailable, the UserRepository should throw a DataAccessException. The AuthenticationService will catch this and re-throw a specific AuthenticationException with a 'Service Unavailable' message. If the user is not found or the password verification returns false, the AuthenticationService should throw an AuthenticationException with an 'Invalid credentials' message. These exceptions are caught by the LoginViewModel and displayed to the user without revealing the specific cause of failure (e.g., user not found vs. bad password).
- **Testing Considerations:** Unit test the AuthenticationService by mocking IUserRepository and IPasswordHasher to validate its orchestration logic for both success and failure cases. Write integration tests for the UserRepository that connect to a test database to verify EF Core mappings and queries. Security testing should verify that password hashes are correctly generated and stored and that the system is not vulnerable to timing attacks.
- **Monitoring Requirements:** All successful login attempts must be logged to the audit trail. All failed login attempts (invalid user, incorrect password) must be logged to the diagnostic log at a 'Warning' level, including the source IP address if possible, to monitor for brute-force attacks. A critical error should be logged if the database becomes unreachable during an authentication attempt.
- **Deployment Considerations:** The application installer must ensure the target environment has network connectivity to the PostgreSQL server. The database connection string, including credentials, must be configured post-installation and stored via the Windows Credential Manager.


---

