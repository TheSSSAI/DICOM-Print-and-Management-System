sequenceDiagram
    participant "LoginViewModel" as LoginViewModel
    participant "AuthenticationService" as AuthenticationService
    actor "UserRepository" as UserRepository
    participant "PostgreSQL DB" as PostgreSQLDB
    participant "Security & Logging Utilities" as SecurityLoggingUtilities

    activate AuthenticationService
    LoginViewModel->>AuthenticationService: 1. LoginAsync(username, password)
    AuthenticationService-->>LoginViewModel: Task<bool> (result: false)
    activate UserRepository
    AuthenticationService->>UserRepository: 2. GetUserByUsernameAsync(username)
    UserRepository-->>AuthenticationService: Task<User> (result: null or User object)
    activate PostgreSQLDB
    UserRepository->>PostgreSQLDB: 3. SELECT * FROM "Users" WHERE "Username" = @p0
    PostgreSQLDB-->>UserRepository: 0 or 1 row(s)
    activate SecurityLoggingUtilities
    AuthenticationService->>SecurityLoggingUtilities: 4. BCrypt.Verify(password, user.HashedPassword)
    SecurityLoggingUtilities-->>AuthenticationService: bool (result: false)
    AuthenticationService->>SecurityLoggingUtilities: 5. Log.Warning("Failed login attempt for {Username}", username)

    note over AuthenticationService: Authentication failure is determined by one of two paths: 1. User not found: GetUserByUsernameAsy...
    note over LoginViewModel: The service MUST return a generic failure (e.g., boolean false) to the ViewModel. This prevents t...

    deactivate SecurityLoggingUtilities
    deactivate PostgreSQLDB
    deactivate UserRepository
    deactivate AuthenticationService
