sequenceDiagram
    participant "LoginViewModel" as LoginViewModel
    participant "AuthenticationService" as AuthenticationService
    actor "UserRepository" as UserRepository
    participant "PostgreSQL DB" as PostgreSQLDB
    participant "PasswordHasher" as PasswordHasher

    activate AuthenticationService
    LoginViewModel->>AuthenticationService: 1. 1. LoginAsync(username, password)
    AuthenticationService-->>LoginViewModel: 10. return UserSession object
    activate UserRepository
    AuthenticationService->>UserRepository: 2. 2. GetUserByUsernameAsync(username)
    UserRepository-->>AuthenticationService: 5. return User domain object
    activate PostgreSQLDB
    UserRepository->>PostgreSQLDB: 3. 3. Execute SQL Query
    PostgreSQLDB-->>UserRepository: 4. Return user record
    activate PasswordHasher
    AuthenticationService->>PasswordHasher: 6. 6. VerifyPassword(password, user.PasswordHash)
    PasswordHasher-->>AuthenticationService: 7. return true
    AuthenticationService->>UserRepository: 8. 8. LogAuditEventAsync(auditEvent)
    UserRepository-->>AuthenticationService: 9. return Task completion

    note over LoginViewModel: Upon receiving the UserSession object, the LoginViewModel is responsible for establishing the app...

    deactivate PasswordHasher
    deactivate PostgreSQLDB
    deactivate UserRepository
    deactivate AuthenticationService
