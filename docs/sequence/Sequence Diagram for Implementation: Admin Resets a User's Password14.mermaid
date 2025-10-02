sequenceDiagram
    participant "Administrator" as Administrator
    participant "Presentation Layer" as PresentationLayer
    participant "Application Service" as ApplicationService
    participant "Cross-Cutting Concerns" as CrossCuttingConcerns
    participant "Data Access Layer" as DataAccessLayer
    participant "PostgreSQL Database" as PostgreSQLDatabase

    activate PresentationLayer
    Administrator->>PresentationLayer: 1. 1. Clicks 'Reset Password' for selected user 'jdoe'.
    PresentationLayer->>PresentationLayer: 2. 2. UserManagementViewModel executes ResetPasswordCommand; calls DialogService.
    Administrator->>PresentationLayer: 3. 3. Confirms password reset in dialog.
    PresentationLayer-->>Administrator: Returns confirmation result.
    activate ApplicationService
    PresentationLayer->>ApplicationService: 4. 4. Calls user management service to initiate reset.
    ApplicationService-->>PresentationLayer: Returns result object with temporary password.
    activate CrossCuttingConcerns
    ApplicationService->>CrossCuttingConcerns: 5. 5. Generates a new secure temporary password.
    CrossCuttingConcerns-->>ApplicationService: Returns plaintext temporary password.
    CrossCuttingConcerns->>ApplicationService: 6. 6. Returns temporary password: 'tP@s5w0rd!xyz'
    ApplicationService-->>CrossCuttingConcerns: plaintextPassword: string
    ApplicationService->>CrossCuttingConcerns: 7. 7. Hashes the new temporary password using BCrypt.
    CrossCuttingConcerns-->>ApplicationService: Returns BCrypt hash string.
    CrossCuttingConcerns->>ApplicationService: 8. 8. Returns hashed password
    ApplicationService-->>CrossCuttingConcerns: hashedPassword: string
    activate DataAccessLayer
    ApplicationService->>DataAccessLayer: 9. 9. Updates user record and logs audit event within a single transaction.
    DataAccessLayer-->>ApplicationService: Returns success status.
    activate PostgreSQLDatabase
    DataAccessLayer->>PostgreSQLDatabase: 10. 10. BEGIN TRANSACTION
    DataAccessLayer->>PostgreSQLDatabase: 11. 11. EXECUTE UPDATE "Users" SET "PasswordHash" = @p0, "ForcePasswordChange" = TRUE WHERE "Id" = @p1
    PostgreSQLDatabase-->>DataAccessLayer: Returns rows affected count.
    DataAccessLayer->>PostgreSQLDatabase: 12. 12. EXECUTE INSERT INTO "AuditLogs" (UserId, EventType, ...) VALUES (@p0, 'UserPasswordReset', ...)
    PostgreSQLDatabase-->>DataAccessLayer: Returns new row ID.
    DataAccessLayer->>PostgreSQLDatabase: 13. 13. COMMIT TRANSACTION
    DataAccessLayer->>ApplicationService: 14. 14. Returns success
    ApplicationService-->>DataAccessLayer: success: true
    ApplicationService->>PresentationLayer: 15. 15. Returns result with temporary password
    PresentationLayer-->>ApplicationService: Result { IsSuccess: true, Value: 'tP@s5w0rd!xyz' }
    PresentationLayer->>PresentationLayer: 16. 16. ViewModel calls NotificationService to display success.
    PresentationLayer->>Administrator: 17. 17. Displays success notification with temporary password and warning.

    note over DataAccessLayer: Atomicity Guarantee: The user password update and the audit log creation are performed within a s...
    note over ApplicationService: Security Compliance: The system generates a temporary password and flags the account to require a...

    deactivate PostgreSQLDatabase
    deactivate DataAccessLayer
    deactivate CrossCuttingConcerns
    deactivate ApplicationService
    deactivate PresentationLayer
