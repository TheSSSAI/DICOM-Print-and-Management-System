sequenceDiagram
    participant "Presentation Layer" as PresentationLayer
    participant "Application Services" as ApplicationServices

    PresentationLayer->>ApplicationServices: 1. User activity detected (keyboard/mouse input). Invoke service to reset timer.
    activate ApplicationServices
    ApplicationServices->>ApplicationServices: 2. 15-minute inactivity timer elapses. Update state to 'Locked' and prepare to notify UI.
    ApplicationServices->>ApplicationServices: 2.1. Log 'Session Locked' event to the audit trail.
    ApplicationServices->>PresentationLayer: 3. Broadcast 'LockSession' event to all listeners.
    activate PresentationLayer
    PresentationLayer->>PresentationLayer: 4. Handle 'LockSession' event by displaying a modal lock screen overlay.
    PresentationLayer->>ApplicationServices: 5. User enters password and clicks 'Unlock'. Request session unlock validation.
    ApplicationServices-->>PresentationLayer: Returns a boolean indicating if the password was valid.
    ApplicationServices->>PresentationLayer: 6. ALT [Unlock Succeeded]: Return 'true'. Log success and broadcast 'UnlockSession' event.
    PresentationLayer-->>ApplicationServices: bool: true
    ApplicationServices->>ApplicationServices: 6.1. Log 'SessionUnlockSuccess' to audit trail.
    PresentationLayer->>PresentationLayer: 7. Dismiss the lock screen overlay, restoring user access.
    ApplicationServices->>PresentationLayer: 8. ALT [Unlock Failed]: Return 'false'. Log failure.
    PresentationLayer-->>ApplicationServices: bool: false
    ApplicationServices->>ApplicationServices: 8.1. Log 'SessionUnlockFailure' to audit trail.
    PresentationLayer->>PresentationLayer: 9. Display 'Invalid password' error message to the user.

    note over PresentationLayer: A global, low-level keyboard and mouse hook (e.g., using P/Invoke) is required in the Presentatio...
    note over ApplicationServices: Password verification within UnlockSessionAsync involves retrieving the current user's hashed pas...

    deactivate PresentationLayer
    deactivate ApplicationServices
