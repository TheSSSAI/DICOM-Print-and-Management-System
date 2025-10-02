sequenceDiagram
    actor "DMPS.Client.Application" as DMPSClientApplication
    actor "I/O Infrastructure (LicenseApiClient)" as IOInfrastructureLicenseApiClient
    participant "External Odoo Licensing API" as ExternalOdooLicensingAPI

    activate IOInfrastructureLicenseApiClient
    DMPSClientApplication->>IOInfrastructureLicenseApiClient: 2. Invoke ILicenseApiClient.ValidateLicenseAsync(licenseKey)
    IOInfrastructureLicenseApiClient-->>DMPSClientApplication: Returns Task<LicenseStatusResult>
    activate ExternalOdooLicensingAPI
    IOInfrastructureLicenseApiClient->>ExternalOdooLicensingAPI: 3. POST /api/license/validate
    ExternalOdooLicensingAPI-->>IOInfrastructureLicenseApiClient: HTTP Response (e.g., 200, 401, 500) with JSON body
    IOInfrastructureLicenseApiClient->>IOInfrastructureLicenseApiClient: 4. Deserialize JSON response into LicenseStatus DTO. Map HTTP status code to internal status.
    DMPSClientApplication->>DMPSClientApplication: 5. Update application state based on LicenseStatusResult

    note over IOInfrastructureLicenseApiClient: Retry Logic (Implemented in REPO-11-INF): A retry policy (e.g., using Polly) wraps the HTTPS requ...
    note over DMPSClientApplication: Fallback Logic (Implemented in REPO-08-APC):  - If result is ApiUnreachable (after all retries fa...

    deactivate ExternalOdooLicensingAPI
    deactivate IOInfrastructureLicenseApiClient
