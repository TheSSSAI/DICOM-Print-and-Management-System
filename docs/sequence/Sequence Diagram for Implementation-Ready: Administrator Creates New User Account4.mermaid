sequenceDiagram
    participant "Presentation Layer (ViewModel)" as PresentationLayerViewModel
    participant "Application Service" as ApplicationService
    participant "Password Hasher Utility" as PasswordHasherUtility
    participant "Data Access Layer" as DataAccessLayer
    participant "PostgreSQL Database" as PostgreSQLDatabase

    activate ApplicationService
    PresentationLayerViewModel->>ApplicationService: 1. 1. CreateUserAsync(username: string, role: UserRole)
    ApplicationService-->>PresentationLayerViewModel: 17. (isSuccess: bool, temporaryPassword?: string)
    ApplicationService->>DataAccessLayer: 2. 2. CheckUsernameExistsAsync(username: string)
    DataAccessLayer-->>ApplicationService: 5. false
    DataAccessLayer->>PostgreSQLDatabase: 3. 3. SELECT 1 FROM "Users" WHERE "Username" = @username LIMIT 1
    PostgreSQLDatabase-->>DataAccessLayer: 4. No rows returned
    ApplicationService->>PasswordHasherUtility: 6. 6. HashPassword(temporaryPassword: string)
    PasswordHasherUtility-->>ApplicationService: 7. hashedPassword: string
    activate DataAccessLayer
    ApplicationService->>DataAccessLayer: 8. 8. CreateUserWithAuditAsync(user: User, audit: AuditLog)
    DataAccessLayer-->>ApplicationService: 15. createdUser: User
    DataAccessLayer->>PostgreSQLDatabase: 9. 9. BEGIN TRANSACTION
    PostgreSQLDatabase-->>DataAccessLayer: 10. OK
    DataAccessLayer->>PostgreSQLDatabase: 11. 11. INSERT INTO "Users" (Username, PasswordHash, Role, ForcePasswordChange) VALUES (...)
    PostgreSQLDatabase-->>DataAccessLayer: 12. 1 row affected
    DataAccessLayer->>PostgreSQLDatabase: 13. 13. INSERT INTO "AuditLog" (UserId, EventType, EntityName, Details) VALUES (...)
    PostgreSQLDatabase-->>DataAccessLayer: 14. 1 row affected
    DataAccessLayer->>PostgreSQLDatabase: 15. 15. COMMIT TRANSACTION
    PostgreSQLDatabase-->>DataAccessLayer: 16. OK

    note over PresentationLayerViewModel: Business Rule: FR-3.9.2.1. Admin initiates user creation from the UI.
    note over ApplicationService: Business Rule: Usernames must be unique. This check prevents a unique constraint violation in the...
    note over ApplicationService: Security: FR-3.3.2.2. A secure, random temporary password is generated in memory.
    note over PasswordHasherUtility: Security: REQ-NFR-004. Password is not stored in plaintext. It is hashed using BCrypt, which incl...
    note over DataAccessLayer: Data Integrity: The creation of the User and its corresponding AuditLog entry are performed withi...
    note over PresentationLayerViewModel: UI Feedback: The ViewModel receives the temporary password and displays it in a success notificat...

    deactivate DataAccessLayer
    deactivate ApplicationService
