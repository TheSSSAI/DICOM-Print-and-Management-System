sequenceDiagram
    participant "Administrator" as Administrator
    participant "Presentation Layer" as PresentationLayer
    participant "Application Service" as ApplicationService
    participant "Security Infrastructure" as SecurityInfrastructure
    participant "Data Access Layer" as DataAccessLayer
    participant "PostgreSQL Database" as PostgreSQLDatabase

    activate PresentationLayer
    Administrator->>PresentationLayer: 1. 1. Fills 'Add User' form (username, role) and clicks 'Create'
    activate ApplicationService
    PresentationLayer->>ApplicationService: 2. 2. CreateUserAsync(username, role)
    ApplicationService-->>PresentationLayer: 404: UserCreationResult { IsSuccess: true, TemporaryPassword: '...' }
    activate DataAccessLayer
    ApplicationService->>DataAccessLayer: 3. 3. [Validation] GetByUsernameAsync(username)
    DataAccessLayer-->>ApplicationService: 6. User object or null
    DataAccessLayer->>PostgreSQLDatabase: 4. 4. SELECT * FROM users WHERE username = @p0
    PostgreSQLDatabase-->>DataAccessLayer: 5. Returns 0 or 1 row
    activate SecurityInfrastructure
    ApplicationService->>SecurityInfrastructure: 7. 7. HashPassword(plainTextPassword)
    SecurityInfrastructure-->>ApplicationService: 8. Hashed password string (BCrypt format)
    ApplicationService->>DataAccessLayer: 9. 9. [Transaction] CreateUserWithAuditAsync(userEntity, auditLogEntity)
    DataAccessLayer-->>ApplicationService: 12. void (or throws exception)
    DataAccessLayer->>PostgreSQLDatabase: 10. 10. BEGIN TRANSACTION; INSERT INTO users (...); INSERT INTO audit_logs (...);
    PostgreSQLDatabase-->>DataAccessLayer: 11. COMMIT TRANSACTION;
    PresentationLayer->>Administrator: 13. 13. Displays success notification: 'User created. Temporary password: ...'

    note over ApplicationService: If username exists at step 6, the Application Service throws a DuplicateUsernameException. The Pr...
    note over ApplicationService: A cryptographically secure random number generator must be used to create the temporary password ...
    note over ApplicationService: The AuditLog entity created before step 9 must contain the Admin's UserID, EventType ('User Creat...
    note over DataAccessLayer: The new User entity must have the 'force_password_change' flag set to true, satisfying REQ-FNC-003.

    deactivate SecurityInfrastructure
    deactivate DataAccessLayer
    deactivate ApplicationService
    deactivate PresentationLayer
