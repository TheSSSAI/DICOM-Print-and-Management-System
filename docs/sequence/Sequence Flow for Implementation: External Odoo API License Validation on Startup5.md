# Specification

# 1. Overview
## 1. Application License Validation on Startup
When the WPF client starts, it communicates with the external Odoo Web Portal API over HTTPS to validate its license key. The system is designed to handle various API responses, including success, failure (4xx/5xx errors), timeouts, and network unavailability, with appropriate retry and fallback logic.

### 1.1. Diagram Id
SEQ-INT-005

### 1.4. Type
IntegrationFlow

### 1.5. Purpose
To ensure the application is properly licensed and to enforce access restrictions if the license is invalid.

### 1.6. Complexity
Medium

### 1.7. Priority
Critical

### 1.8. Frequency
OnDemand

### 1.9. Participants

- REPO-08-APC
- REPO-11-INF
- EXT-ODOO

### 1.10. Key Interactions

- Application startup sequence is initiated.
- LicenseValidationService is invoked.
- OdooApiClient makes an HTTPS request to the Odoo API endpoint with the license key.
- The service receives and parses the JSON response.
- Application state is set to 'Licensed', 'Invalid', or 'GracePeriod' based on the response.

### 1.11. Triggers

- Application startup (REQ-INT-003).

### 1.12. Outcomes

- The application proceeds with full functionality if the license is valid.
- The application enters a 72-hour grace period if the API is unreachable.
- The application enters a restricted, read-only mode if the license is invalid or the grace period expires.

### 1.13. Business Rules

- Communication must use TLS 1.2 or higher (REQ-INT-004).
- A 72-hour grace period is allowed for network failures (REQ-INT-003).

### 1.14. Error Scenarios

- Odoo API returns 401/403 (Unauthorized).
- Odoo API returns 5xx (Server Error).
- Odoo API returns 429 (Too Many Requests).
- Local network connectivity is down.

### 1.15. Integration Points

- Odoo Web Portal REST API


---

# 2. Details
## 2. Implementation: External Odoo API License Validation on Startup
A detailed technical breakdown of the startup license validation process. The DMPS.Client.Application orchestrates the call via its ILicenseValidationService, which depends on an ILicenseApiClient adapter from the Infrastructure layer. The adapter handles HTTPS communication, JSON data transformation, and implements a robust retry/fallback strategy for network and API errors, ultimately determining the application's operational mode (Licensed, GracePeriod, or ReadOnly) as per REQ-INT-003.

### 2.1. Diagram Id
SEQ-INT-005

### 2.4. Participants

- **Repository Id:** REPO-08-APC  
**Display Name:** DMPS.Client.Application  
**Type:** Application Service  
**Technology:** .NET 8  
**Order:** 1  
**Style:**
    
    - **Shape:** component
    - **Color:** #B3E5FC
    - **Stereotype:** Layer: Application
    
- **Repository Id:** REPO-11-INF  
**Display Name:** I/O Infrastructure (LicenseApiClient)  
**Type:** Infrastructure  
**Technology:** .NET 8 / HttpClient  
**Order:** 2  
**Style:**
    
    - **Shape:** component
    - **Color:** #C8E6C9
    - **Stereotype:** Layer: Infrastructure
    
- **Repository Id:** EXT-ODOO  
**Display Name:** External Odoo Licensing API  
**Type:** External System  
**Technology:** REST/JSON API  
**Order:** 3  
**Style:**
    
    - **Shape:** actor
    - **Color:** #FFCCBC
    - **Stereotype:** External API
    

### 2.5. Interactions

- **Source Id:** None  
**Target Id:** REPO-08-APC  
**Message:** Application starts. Invoke ILicenseValidationService.ValidateLicenseOnStartupAsync()  
**Sequence Number:** 1  
**Type:** System Trigger  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** ValidateLicenseOnStartupAsync
    - **Parameters:** None
    - **Authentication:** N/A
    - **Error Handling:** Logs unhandled exceptions during startup.
    - **Performance:** Must not block UI thread for more than 100ms before showing splash screen.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-11-INF  
**Message:** Invoke ILicenseApiClient.ValidateLicenseAsync(licenseKey)  
**Sequence Number:** 2  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** Returns Task<LicenseStatusResult>  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process (Dependency Injection)
    - **Method:** ValidateLicenseAsync
    - **Parameters:** string licenseKey (retrieved from Windows Credential Manager)
    - **Authentication:** N/A
    - **Error Handling:** Propagates exceptions from the API client.
    - **Performance:** Awaits the asynchronous result from the infrastructure layer.
    
- **Source Id:** REPO-11-INF  
**Target Id:** EXT-ODOO  
**Message:** POST /api/license/validate  
**Sequence Number:** 3  
**Type:** API Request  
**Is Synchronous:** True  
**Return Message:** HTTP Response (e.g., 200, 401, 500) with JSON body  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** HTTPS/1.1 (TLS 1.2+)
    - **Method:** POST
    - **Parameters:** HttpContent with JSON payload: {"licenseKey": "..."}. Headers: {"Content-Type": "application/json", "Authorization": "Bearer <api_token_from_secure_storage>"}
    - **Authentication:** Bearer Token
    - **Error Handling:** Wrapped in a retry policy. Catches HttpRequestException for network failures. Checks HttpResponseMessage.StatusCode for API-level errors.
    - **Performance:** Configured with a 15-second timeout per attempt.
    
- **Source Id:** REPO-11-INF  
**Target Id:** REPO-11-INF  
**Message:** Deserialize JSON response into LicenseStatus DTO. Map HTTP status code to internal status.  
**Sequence Number:** 4  
**Type:** Internal Processing  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** Private method for response handling
    - **Parameters:** HttpResponseMessage response
    - **Authentication:** N/A
    - **Error Handling:** Handles JsonException on deserialization failure, maps it to an 'InvalidResponse' status.
    - **Performance:** Sub-millisecond operation.
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-08-APC  
**Message:** Update application state based on LicenseStatusResult  
**Sequence Number:** 5  
**Type:** State Change  
**Is Synchronous:** True  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** Private method for state management
    - **Parameters:** LicenseStatusResult result
    - **Authentication:** N/A
    - **Error Handling:** Logs the final determined state (Licensed, GracePeriod, ReadOnly).
    - **Performance:** Near-instantaneous state update.
    

### 2.6. Notes

- **Content:** Retry Logic (Implemented in REPO-11-INF): A retry policy (e.g., using Polly) wraps the HTTPS request. It retries up to 3 times with exponential backoff on: 
- HttpRequestException (network failure)
- HTTP 5xx (Server Error)
- HTTP 429 (Too Many Requests)  
**Position:** top  
**Participant Id:** REPO-11-INF  
**Sequence Number:** 3  
- **Content:** Fallback Logic (Implemented in REPO-08-APC): 
- If result is ApiUnreachable (after all retries fail), start a 72-hour grace period timer.
- If result is Invalid (from HTTP 401/403), immediately set application to ReadOnly mode.
- If grace period expires, transition to ReadOnly mode.  
**Position:** bottom  
**Participant Id:** REPO-08-APC  
**Sequence Number:** 5  

### 2.7. Implementation Guidance

- **Security Requirements:** The License Key and any API tokens must be retrieved from the Windows Credential Manager (REQ-NFR-004). All communication must use HTTPS with a minimum of TLS 1.2, preferring TLS 1.3 (REQ-INT-004). The HttpClient handler must be configured to enforce this policy and perform standard certificate validation.
- **Performance Targets:** The entire validation sequence, including 3 retries, should have a total timeout of 30 seconds. P95 latency for a successful API call from the client's perspective should be under 2 seconds. The application startup should not be blocked waiting for this; a splash screen should be shown while validation occurs in the background.
- **Error Handling Strategy:** Implement a retry policy using a library like Polly. For transient errors (network issues, 5xx, 429), retry up to 3 times with exponential backoff and jitter. If all retries fail, the API client should return a specific `ApiUnreachable` status. For definitive failures (401, 403), it should return an `Invalid` status. Log all failed attempts and state transitions.
- **Testing Considerations:** Unit test the LicenseValidationService by mocking the ILicenseApiClient interface. Integration tests for the ILicenseApiClient should use a mock HTTP server (e.g., WireMock.Net) to simulate various Odoo API responses (200 OK, 401, 403, 500, timeouts) and verify the correct internal status DTO is returned.
- **Monitoring Requirements:** Log the outcome of every validation attempt (Success, Failure, GracePeriodActivated) at an INFO level. Log retry attempts and API failures at a WARN level. Use metrics to track API call latency and failure rates to detect widespread issues with the licensing service.
- **Deployment Considerations:** The Odoo API endpoint URL must be a configurable setting, not hard-coded. The application installer should perform a prerequisite check to ensure the machine has outbound internet access (HTTPS on port 443) to the configured license server domain.


---

