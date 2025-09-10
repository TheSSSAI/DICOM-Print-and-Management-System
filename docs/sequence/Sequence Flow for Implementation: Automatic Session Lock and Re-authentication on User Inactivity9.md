# Specification

# 1. Overview
## 1. Automatic Session Lock on User Inactivity
The WPF client application monitors for user keyboard and mouse input to protect PHI on unattended workstations. If no activity is detected for 15 minutes, the application automatically displays a lock screen overlay, preserving the current state. The user must re-enter their password to unlock the session and resume.

### 1.1. Diagram Id
SEQ-SEC-009

### 1.4. Type
SecurityFlow

### 1.5. Purpose
To protect against unauthorized access to Protected Health Information (PHI) on an unattended workstation, as required by HIPAA.

### 1.6. Complexity
Low

### 1.7. Priority
High

### 1.8. Frequency
OnDemand

### 1.9. Participants

- REPO-09-PRE
- REPO-08-APC

### 1.10. Key Interactions

- A session timer is reset on every user input event (keyboard or mouse).
- The timer reaches the 15-minute threshold without being reset.
- The SessionLockService triggers a global 'Lock' event within the application.
- The UI displays a modal lock screen overlay, blocking access to the underlying content.
- User enters their password.
- The password is validated locally, and if correct, the lock screen is dismissed.

### 1.11. Triggers

- 15 minutes of user inactivity (no keyboard or mouse input) (FR-3.3.2.1).

### 1.12. Outcomes

- The application is secured, preventing unauthorized viewing or interaction.
- The user's work state is preserved and can be resumed after re-authentication.

### 1.13. Business Rules

- Session must lock after a configurable period of 15 minutes of inactivity.

### 1.14. Error Scenarios

- User enters the incorrect password and is shown an error message.

### 1.15. Integration Points



---

# 2. Details
## 2. Implementation: Automatic Session Lock and Re-authentication on User Inactivity
Detailed technical sequence for monitoring user activity via global input hooks, enforcing a 15-minute inactivity lock as per HIPAA policy, and handling password-based re-authentication. This flow preserves the application state and is orchestrated by a dedicated SessionLockService. All lock and unlock attempts are logged to the audit trail.

### 2.1. Diagram Id
SEQ-SEC-009-IMPL

### 2.4. Participants

- **Repository Id:** REPO-09-PRE  
**Display Name:** Presentation Layer  
**Type:** WPF Client  
**Technology:** .NET 8, WPF, XAML  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #4CAF50
    - **Stereotype:** UI/View
    
- **Repository Id:** REPO-08-APC  
**Display Name:** Application Services  
**Type:** Application Logic  
**Technology:** .NET 8, C# 12  
**Order:** 2  
**Style:**
    
    - **Shape:** participant
    - **Color:** #2196F3
    - **Stereotype:** Service
    

### 2.5. Interactions

- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-08-APC  
**Message:** User activity detected (keyboard/mouse input). Invoke service to reset timer.  
**Sequence Number:** 1  
**Type:** Method Invocation  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Direct Method Call
    - **Method:** SessionLockService.ResetInactivityTimer()
    - **Parameters:** None
    - **Authentication:** N/A (In-process)
    - **Error Handling:** N/A (Fire-and-forget)
    - **Performance:** Must execute in < 1ms to avoid UI lag.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-08-APC  
**Message:** 15-minute inactivity timer elapses. Update state to 'Locked' and prepare to notify UI.  
**Sequence Number:** 2  
**Type:** Internal Event  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** System.Threading.Timer
    - **Method:** OnTimerElapsed()
    - **Parameters:** None
    - **Authentication:** N/A
    - **Error Handling:** Logs error if state transition fails.
    - **Performance:** Timer callback must be lightweight.
    
**Nested Interactions:**
    
    - **Source Id:** REPO-08-APC  
**Target Id:** REPO-08-APC  
**Message:** Log 'Session Locked' event to the audit trail.  
**Sequence Number:** 2.1  
**Type:** Service Call  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Direct Method Call
    - **Method:** IAuditLogService.LogEventAsync(eventType: 'SessionLocked', ...)
    - **Parameters:** Details include UserId, Timestamp, WorkstationID.
    - **Authentication:** N/A
    - **Error Handling:** Logged via primary logging framework.
    - **Performance:** Asynchronous to avoid blocking.
    
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-09-PRE  
**Message:** Broadcast 'LockSession' event to all listeners.  
**Sequence Number:** 3  
**Type:** Event Publication  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Process Event Aggregator
    - **Method:** eventAggregator.Publish(new LockSessionEvent())
    - **Parameters:** Event payload object.
    - **Authentication:** N/A
    - **Error Handling:** UI must gracefully handle if it fails to subscribe or process the event.
    - **Performance:** Near-instantaneous delivery.
    
- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-09-PRE  
**Message:** Handle 'LockSession' event by displaying a modal lock screen overlay.  
**Sequence Number:** 4  
**Type:** UI Operation  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** WPF UI Dispatcher
    - **Method:** IDialogService.ShowModal<LockScreenViewModel>()
    - **Parameters:** None. The view model is resolved via DI.
    - **Authentication:** N/A
    - **Error Handling:** Logs any exceptions related to UI rendering.
    - **Performance:** Lock screen must appear < 200ms after event is received.
    
- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-08-APC  
**Message:** User enters password and clicks 'Unlock'. Request session unlock validation.  
**Sequence Number:** 5  
**Type:** Asynchronous Method Invocation  
**Is Synchronous:** False  
**Return Message:** Returns a boolean indicating if the password was valid.  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** Direct Method Call
    - **Method:** Task<bool> IAuthenticationService.UnlockSessionAsync(string password)
    - **Parameters:** SecureString or char[] containing the user-entered password.
    - **Authentication:** N/A (Relies on existing authenticated session context).
    - **Error Handling:** Handles exceptions from dependencies (e.g., password hasher). Returns 'false' for any failure.
    - **Performance:** Password verification must complete in < 500ms.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-09-PRE  
**Message:** ALT [Unlock Succeeded]: Return 'true'. Log success and broadcast 'UnlockSession' event.  
**Sequence Number:** 6  
**Type:** Return Message  
**Is Synchronous:** False  
**Return Message:** bool: true  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Task<bool> Result
    - **Method:** return true;
    - **Parameters:** N/A
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** N/A
    
**Nested Interactions:**
    
    - **Source Id:** REPO-08-APC  
**Target Id:** REPO-08-APC  
**Message:** Log 'SessionUnlockSuccess' to audit trail.  
**Sequence Number:** 6.1  
**Type:** Service Call  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Direct Method Call
    - **Method:** IAuditLogService.LogEventAsync(eventType: 'SessionUnlockSuccess', ...)
    - **Parameters:** Details include UserId, Timestamp.
    - **Authentication:** N/A
    - **Error Handling:** Logged.
    - **Performance:** Asynchronous.
    
    
- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-09-PRE  
**Message:** Dismiss the lock screen overlay, restoring user access.  
**Sequence Number:** 7  
**Type:** UI Operation  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** WPF UI Dispatcher
    - **Method:** IDialogService.Close()
    - **Parameters:** None
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** UI should be interactive immediately.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-09-PRE  
**Message:** ALT [Unlock Failed]: Return 'false'. Log failure.  
**Sequence Number:** 8  
**Type:** Return Message  
**Is Synchronous:** False  
**Return Message:** bool: false  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Task<bool> Result
    - **Method:** return false;
    - **Parameters:** N/A
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** N/A
    
**Nested Interactions:**
    
    - **Source Id:** REPO-08-APC  
**Target Id:** REPO-08-APC  
**Message:** Log 'SessionUnlockFailure' to audit trail.  
**Sequence Number:** 8.1  
**Type:** Service Call  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** Direct Method Call
    - **Method:** IAuditLogService.LogEventAsync(eventType: 'SessionUnlockFailure', ...)
    - **Parameters:** Details include UserId, Timestamp, SourceIP (if available).
    - **Authentication:** N/A
    - **Error Handling:** Logged.
    - **Performance:** Asynchronous.
    
    
- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-09-PRE  
**Message:** Display 'Invalid password' error message to the user.  
**Sequence Number:** 9  
**Type:** UI Update  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** WPF Data Binding
    - **Method:** LockScreenViewModel.ErrorMessage = '...'
    - **Parameters:** String containing the error message.
    - **Authentication:** N/A
    - **Error Handling:** N/A
    - **Performance:** Instantaneous UI update.
    

### 2.6. Notes

- **Content:** A global, low-level keyboard and mouse hook (e.g., using P/Invoke) is required in the Presentation Layer to reliably detect system-wide user activity and call ResetInactivityTimer().  
**Position:** Top  
**Participant Id:** REPO-09-PRE  
**Sequence Number:** 1  
- **Content:** Password verification within UnlockSessionAsync involves retrieving the current user's hashed password and using the IPasswordHasher service to perform a secure, constant-time comparison.  
**Position:** Middle  
**Participant Id:** REPO-08-APC  
**Sequence Number:** 5  

### 2.7. Implementation Guidance

- **Security Requirements:** The lock screen overlay must be implemented as a top-level, system-modal window to prevent bypass vulnerabilities (e.g., Alt-Tab). Password validation must delegate to the BCrypt-based IPasswordHasher service. All lock and unlock attempts (successful and failed) must be recorded in the audit trail as a security event, fulfilling FR-3.4.2.2. Consider implementing brute-force protection (e.g., increasing delay after multiple failed attempts). The session timeout duration (15 minutes) must be a configurable setting.
- **Performance Targets:** User activity monitoring must have a negligible performance impact (<1% CPU). The lock screen must be displayed within 200ms of the timer event firing. The re-authentication process (password hash and compare) must complete in under 500ms.
- **Error Handling Strategy:** If the user provides an incorrect password, a clear but non-specific error message (e.g., 'Invalid password') is displayed. The password input field must be cleared after each attempt. The system must not provide details that could aid an attacker (e.g., 'User does not exist' vs 'Incorrect password').
- **Testing Considerations:** Unit tests are required for the SessionLockService timer and state logic. Integration tests must cover the password verification flow. End-to-end (E2E) automated UI tests are critical to validate that the global hooks work, the timer triggers correctly, the lock screen appears, and cannot be bypassed.
- **Monitoring Requirements:** Audit log entries for 'SessionLocked', 'SessionUnlockSuccess', and 'SessionUnlockFailure' are mandatory. A high frequency of 'SessionUnlockFailure' events for a single user or workstation in a short period should be considered for inclusion in a security monitoring dashboard or alert rule.
- **Deployment Considerations:** The inactivity timeout value (default 15 minutes) must be stored in a configuration that is editable by an Administrator in the application settings, as per FR-3.3.2.1.


---

