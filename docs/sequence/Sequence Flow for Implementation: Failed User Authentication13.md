# Specification

# 1. Overview
## 1. User Login - Failed Authentication
A user provides invalid credentials (incorrect password or non-existent username). The system securely validates the credentials, finds no match, and returns a generic error message to the user without revealing which part of the credential was incorrect. The failed attempt is logged.

### 1.1. Diagram Id
SEQ-AFL-013

### 1.4. Type
AuthenticationFlow

### 1.5. Purpose
To securely reject login attempts with invalid credentials and provide appropriate feedback without compromising security.

### 1.6. Complexity
Low

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

- User enters invalid credentials.
- AuthenticationService fails to verify the password hash or cannot find the user in the database.
- The service returns an 'Authentication Failed' result.
- The LoginView displays a generic error: 'Invalid username or password'.
- A 'Login Failed' event is written to the security log.

### 1.11. Triggers

- User submits login form with invalid credentials.

### 1.12. Outcomes

- Access to the application is denied.
- A user-friendly error message is displayed.
- The failed login attempt is logged for security monitoring.

### 1.13. Business Rules

- The system must not indicate whether the username or the password was incorrect.
- Account lockout policies may be triggered after multiple failed attempts (future requirement).

### 1.14. Error Scenarios

- This is an error handling sequence itself.

### 1.15. Integration Points

- PostgreSQL Database


---

# 2. Details
## 2. Implementation: Failed User Authentication
Provides a detailed technical sequence for handling a failed user login attempt. This flow covers both scenarios: a non-existent username and an existing username with an incorrect password. It specifies the use of BCrypt for secure password verification and Serilog for logging the security event, culminating in a generic, security-conscious error message presented to the user.

### 2.1. Diagram Id
SEQ-AFL-013

### 2.4. Participants

- **Repository Id:** REPO-09-PRE  
**Display Name:** LoginViewModel  
**Type:** ViewModel  
**Technology:** WPF, .NET 8  
**Order:** 1  
**Style:**
    
    - **Shape:** participant
    - **Color:** #FFDDC1
    - **Stereotype:** Presentation
    
- **Repository Id:** REPO-08-APC  
**Display Name:** AuthenticationService  
**Type:** Application Service  
**Technology:** .NET 8, C# 12  
**Order:** 2  
**Style:**
    
    - **Shape:** participant
    - **Color:** #C2EFEB
    - **Stereotype:** Application
    
- **Repository Id:** REPO-10-DAC  
**Display Name:** UserRepository  
**Type:** Repository  
**Technology:** Entity Framework Core 8  
**Order:** 3  
**Style:**
    
    - **Shape:** participant
    - **Color:** #D4C2EB
    - **Stereotype:** Data Access
    
- **Repository Id:** EXT-PGSQL  
**Display Name:** PostgreSQL DB  
**Type:** Database  
**Technology:** PostgreSQL 16  
**Order:** 4  
**Style:**
    
    - **Shape:** database
    - **Color:** #E6E6FA
    - **Stereotype:** External
    
- **Repository Id:** REPO-12-CCC  
**Display Name:** Security & Logging Utilities  
**Type:** Utility  
**Technology:** BCrypt.Net-Next, Serilog  
**Order:** 5  
**Style:**
    
    - **Shape:** participant
    - **Color:** #FFFACD
    - **Stereotype:** Cross-Cutting
    

### 2.5. Interactions

- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-08-APC  
**Message:** LoginAsync(username, password)  
**Sequence Number:** 1  
**Type:** Method Call  
**Is Synchronous:** False  
**Return Message:** Task<bool> (result: false)  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Call
    - **Method:** IAuthenticationService.LoginAsync
    - **Parameters:** [string] username, [SecureString/string] password
    - **Authentication:** N/A (Internal call)
    - **Error Handling:** Handles exceptions from service layer, translates to user-friendly error state.
    - **Performance:** Must not block UI thread.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-10-DAC  
**Message:** GetUserByUsernameAsync(username)  
**Sequence Number:** 2  
**Type:** Method Call  
**Is Synchronous:** False  
**Return Message:** Task<User> (result: null or User object)  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Call
    - **Method:** IUserRepository.GetUserByUsernameAsync
    - **Parameters:** [string] username
    - **Authentication:** N/A
    - **Error Handling:** Propagates database exceptions (e.g., connectivity) to the caller.
    - **Performance:** Should be highly optimized with a database index on the username column.
    
- **Source Id:** REPO-10-DAC  
**Target Id:** EXT-PGSQL  
**Message:** SELECT * FROM "Users" WHERE "Username" = @p0  
**Sequence Number:** 3  
**Type:** Database Query  
**Is Synchronous:** True  
**Return Message:** 0 or 1 row(s)  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** TCP/IP (SQL)
    - **Method:** SELECT
    - **Parameters:** Parameterized query to prevent SQL injection.
    - **Authentication:** Database connection credentials.
    - **Error Handling:** Database engine handles query execution; connection errors handled by EF Core.
    - **Performance:** Query time < 10ms.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-12-CCC  
**Message:** BCrypt.Verify(password, user.HashedPassword)  
**Sequence Number:** 4  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** bool (result: false)  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Call
    - **Method:** BCrypt.Net.BCrypt.Verify
    - **Parameters:** [string] password, [string] hashedPassword
    - **Authentication:** N/A
    - **Error Handling:** N/A for this specific call.
    - **Performance:** BCrypt is computationally intensive by design; work factor must be tuned.
    
**Nested Interactions:**
    
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-12-CCC  
**Message:** Log.Warning("Failed login attempt for {Username}", username)  
**Sequence Number:** 5  
**Type:** Logging  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Call
    - **Method:** ILogger.Warning
    - **Parameters:** Structured log message with username parameter.
    - **Authentication:** N/A
    - **Error Handling:** Logging framework handles sink failures.
    - **Performance:** Logging is asynchronous to minimize performance impact.
    

### 2.6. Notes

- **Content:** Authentication failure is determined by one of two paths:
1. User not found: GetUserByUsernameAsync returns null (Interaction 2 returns null).
2. Password mismatch: GetUserByUsernameAsync returns a user, but BCrypt.Verify returns false (Interaction 4).  
**Position:** bottom  
**Participant Id:** REPO-08-APC  
**Sequence Number:** 4  
- **Content:** The service MUST return a generic failure (e.g., boolean `false`) to the ViewModel. This prevents the UI from distinguishing between an invalid username and an incorrect password, which is a critical security measure to prevent username enumeration.  
**Position:** bottom  
**Participant Id:** REPO-09-PRE  
**Sequence Number:** 5  

### 2.7. Implementation Guidance

- **Security Requirements:** Passwords must be hashed using BCrypt with a configurable work factor. Database connection credentials must be stored securely using Windows Credential Manager (REQ-NFR-004). The failed login attempt must be logged as a security event (FR-3.4.2.2). The UI error message must be generic to prevent username enumeration.
- **Performance Targets:** The end-to-end authentication process (from user click to UI feedback) should complete in under 500ms on average. This is dependent on the BCrypt work factor and database query performance.
- **Error Handling Strategy:** If the database connection fails during the `GetUserByUsernameAsync` call, the exception should be caught by the `AuthenticationService`, logged as a critical system error, and the UI should display a generic system error message (e.g., 'An error occurred. Please try again later.').
- **Testing Considerations:** Test cases must include: 1) Non-existent username. 2) Existing username with incorrect password. 3) Correct credentials (to validate non-failure). 4) Username with different casing if the system is case-insensitive. 5) Empty username/password to test client-side validation.
- **Monitoring Requirements:** The rate of failed login attempts should be monitored. A sudden spike in failed logins from a specific IP or for a specific user could indicate a brute-force or credential stuffing attack and should trigger an alert.
- **Deployment Considerations:** The BCrypt work factor should be stored as a configurable parameter to allow for tuning based on server hardware capabilities without requiring a code change.


---

