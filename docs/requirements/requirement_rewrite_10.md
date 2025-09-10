### **Software Requirements Specification**

---

### **1. Introduction**

This document outlines the functional, non-functional, and technical requirements for the DICOM Management and Printing System. It serves as the primary source of truth for development, quality assurance, and project management stakeholders.

---

### **2. System Overview and Roles**

#### **2.1 System Architecture (REQ-TEC-002)**
*   The system shall be architected as a Client-Server system with a decoupled Background Worker Service.
*   The system shall be composed of a main WPF desktop client application for user interaction and a single, consolidated background Windows Service. This architecture requires pre-existing and configured server components (PostgreSQL, RabbitMQ) for operation.
*   The background service shall be responsible for hosting all background functionalities and processing asynchronous, long-running tasks, including but not limited to:
    *   Acting as the C-STORE SCP listener.
    *   Consuming messages from a queue for database writes originating from C-STORE operations.
    *   Processing PDF generation requests.
    *   Handling physical print job spooling.
*   Communication between the main application (producer) and the background service (consumer) for asynchronous tasks shall be handled via a message queue (RabbitMQ).
*   All messages published to RabbitMQ for critical data processing (e.g., database writes, print jobs) shall be marked as persistent to ensure they survive a message broker restart.
*   The message queue architecture shall include a dead-letter queue (DLQ) mechanism to handle and isolate messages that fail processing after a configurable number of retries.
*   Direct, synchronous communication for real-time status checks (e.g., verifying if the service is running) shall be handled via Named Pipes.

#### **2.2 Design and Implementation Constraints (REQ-TEC-001)**
*   The application shall be developed using the .NET 8 framework, C# programming language, and Windows Presentation Foundation (WPF).
*   The application shall use the `fo-dicom` library for all DICOM operations.
*   The application shall use PostgreSQL for the local data store to handle high-volume record insertion from modalities with large numbers of images per study.
*   To ensure guaranteed and high-throughput data insertion, the C-STORE SCP service shall publish received study metadata to a message queue (RabbitMQ), which will be consumed by a separate process within the background service responsible for database writing.
*   The application shall integrate with the existing Odoo Web Portal APIs for license validation.
*   The application shall be developed in accordance with the HIPAA Security Rule (45 CFR Part 160 and Subparts A and C of Part 164) and Privacy Rule for PHI handling, anonymization, and security.

#### **2.3 Technology Stack (REQ-TEC-003)**
*   **Core Framework & Language:**
    *   The application shall be built on the .NET 8 Framework.
    *   The application shall be written in C# 12.
*   **Desktop UI:**
    *   The desktop UI shall be built with WPF (Windows Presentation Foundation).
    *   The UI shall use the MVVM (Model-View-ViewModel) design pattern.
    *   The UI shall leverage the `Material Design in XAML Toolkit` for a modern component library.
*   **Backend & Services:**
    *   The Windows Service shall be built using `Microsoft.Extensions.Hosting.WindowsServices`.
    *   The application shall use `Microsoft.Extensions.DependencyInjection` for dependency management.
*   **Inter-Process Communication:**
    *   Synchronous status checks between the UI and the Windows Service shall use Named Pipes.
    *   Asynchronous task and data processing shall be managed via a message queue (RabbitMQ).
*   **Data & Storage:**
    *   The database shall be PostgreSQL (version 16).
    *   The ORM shall be Entity Framework Core 8 with EF Core Migrations for schema management.
*   **DICOM & Graphics:**
    *   The DICOM library shall be `fo-dicom`.
    *   Graphics rendering shall use the WPF Integrated Renderer (DirectX) and `Vortice.Windows` for high-performance viewing.
*   **Libraries & Tooling:**
    *   The logging framework shall be Serilog, configured with `Serilog.Sinks.File` and `Serilog.Sinks.EventLog`.
    *   PDF generation shall use the `QuestPDF` library to ensure PDF/A compliance.
*   **Deployment:**
    *   The installation package shall be a signed MSIX Installer.
*   **Development & Quality Assurance:**
    *   The primary IDE shall be Visual Studio 2022 with the .NET 8 SDK.
    *   Source control shall be managed with Git.
    *   Static code analysis shall be performed using integrated Roslyn Analyzers.
    *   The testing framework shall be `xUnit`, with `Moq` for mocking and `FluentAssertions` for assertion clarity.

#### **2.4 User Roles and Permissions (REQ-USR-001)**
*   The system shall enforce a role-based access control model with two distinct roles: Technician and Administrator.
*   **Technician Role Permissions:**
    *   The system shall allow a Technician to view and search for studies.
    *   The system shall allow a Technician to perform all viewing operations, including zoom, pan, and Window Width/Level (WW/WL).
    *   The system shall allow a Technician to create and save annotations.
    *   The system shall allow a Technician to use all print and export functions, including Print to Printer, PDF, and Email.
    *   The system shall prevent a Technician from editing DICOM metadata.
    *   The system shall prevent a Technician from changing system settings, including PACS, Users, and Storage Paths.
    *   The system shall prevent a Technician from viewing audit trails.
*   **Admin Role Permissions:**
    *   The system shall grant an Admin all permissions of the Technician role.
    *   The system shall allow an Admin to edit DICOM metadata tags from a configurable list.
    *   The system shall allow an Admin to manage user accounts and roles.
    *   The system shall allow an Admin to configure all system, print, and network settings.
    *   The system shall allow an Admin to view and export audit trails.

#### **2.5 Business Rules and Constraints**
*   **2.5.1 Data Residency:** All Protected Health Information (PHI) data, including DICOM files and database records, shall be stored and processed within the geographical boundaries specified by the deploying organization's legal and regulatory requirements.
*   **2.5.2 Data Retention Policy:** The system shall enforce a configurable data retention policy. By default, all study data shall be retained for a minimum of 7 years. An Administrator shall be able to configure auto-purge rules to permanently delete studies older than the defined retention period. This action must be logged in the audit trail.
*   **2.5.3 Incident Response Support:** The system shall provide capabilities to support security incident response. An Administrator shall have the ability to immediately disable any user account, force a logout of a specific user session, and view active sessions from the Administration dashboard.

---

### **3. Functional Requirements**

#### **3.1 DICOM Printing (REQ-FNC-001)**

*   **FR-3.1.1: Windows Print Service Integration**
    *   The system shall provide a DICOM printing capability that integrates directly with the Windows Print API to enable printing on commercial inkjet/laser printers.
    *   The application installer shall register and start a consolidated Windows Service named \"DICOM Service\".
    *   The service shall automatically detect all system-installed printers.
    *   The service shall fetch and display driver-supported resolutions for the selected printer.

*   **FR-3.1.1.1: Application-Service Communication**
    *   The main desktop application shall communicate with the Windows Service via Named Pipes for synchronous status checks.
    *   The application shall submit a serialized print job object, containing image data, layout, and overlay information, to a message queue (RabbitMQ) for asynchronous processing by the service.
    *   The application shall gracefully handle and notify the user with a non-blocking message if the Windows Service is stopped or unresponsive.
    *   The \"Print\" button in the UI shall be disabled if the print service is not available, as determined by a synchronous status check.

*   **FR-3.1.2: Print Layout Configuration**
    *   The print preview UI shall provide predefined page templates: Single image (A4/A3), 2x2 grid (A4), and 1+3 comparison (A3).
    *   The print preview UI shall provide modality-specific presets, such as 1x1 for X-ray and 4x4 for CT slices.
    *   Users shall be able to customize page margins within a range of 5-20mm.
    *   Users shall be able to select page orientation (Portrait/Landscape).
    *   Users shall be able to select image scaling options: Fit-to-Page and Actual Size.

*   **FR-3.1.2.1: Configurable Auto-Arrangement**
    *   The system shall support auto-arranging images on the print layout based on configurable rules.
    *   An Admin user shall be able to configure the rules for auto-arrangement in the Settings Page.
    *   The configuration shall allow mapping specific modalities to default layouts and defining the sorting order of images.

*   **FR-3.1.2.2: Manual Rearrangement**
    *   Users shall be able to manually rearrange images within the print preview grid via drag-and-drop.

*   **FR-3.1.3: Branding & De-identification Overlays**
    *   Users shall be able to upload a custom logo in PNG or JPEG format.
    *   The logo position shall be configurable to Header, Footer, or custom X/Y coordinates.
    *   Logo scaling shall be adjustable from 10% to 50% of the page width.
    *   Users shall be able to add custom text fields with configurable font, size, and color.
    *   Custom text fields shall support variables that are replaced at print time (e.g., `[PatientID]`, `[Date]`).

*   **FR-3.1.3.1: Non-Destructive Templated Overlays**
    *   The system shall support non-destructive, print-time pixel-based overlay masks for de-identification.
    *   Applying a blackout overlay in the print preview shall not alter the original DICOM file on disk.
    *   The system shall provide pre-built templates for de-identification (e.g., Chest X-Ray De-ID).
    *   Overlay templates shall support configurable rectangular and polygonal regions.

*   **FR-3.1.3.2: Overlay Template Management**
    *   The system shall provide an interface for Admin users to manage overlay templates within the Settings Page.
    *   The interface shall allow an Admin user to create new overlay templates by drawing and saving rectangular/polygonal regions.
    *   Admin users shall be able to edit and delete existing templates.

*   **FR-3.1.4: Print Output Formats**
    *   The system shall send the configured print job to a selected Windows printer.
    *   The application shall list and allow selection from all available Windows printers.
    *   The final output from the physical printer shall match the print preview UI.
    *   The system shall generate a PDF file of the print layout.
    *   The generated PDF shall be PDF/A-3 compliant.
    *   The system shall provide an option to embed DICOM metadata into the PDF file.
    *   The system shall provide an option to password-protect the PDF using AES-256 encryption.
    *   The system shall be able to attach the print output as a PDF or image(s) to an email.
    *   The Settings Page shall provide fields to configure an SMTP server (host, port, credentials, TLS/STARTTLS).
    *   Users shall be able to attach the output as a single PDF or as individual image files.
    *   Exported images for email attachments shall be in a user-selectable format (JPEG/PNG).
    *   If multiple images are selected for email attachment, they shall be attached as individual files.
    *   The system shall allow the creation of pre-filled email templates with variables (e.g., `[PatientName]`, `[StudyDate]`).

*   **FR-3.1.4.4: Email Security Warning**
    *   As a business rule, if an email template is configured to include PHI variables or if an attached PDF is not password-protected, the system shall display a warning to the user about sending unsecured PHI before sending the email.
    *   When the \"Send\" button is clicked, the system shall check for PHI variables in the email body/subject and check the security status of all attachments. An attachment is considered unsecured if it is a non-password-protected PDF or an inherently unencrypted file type such as JPEG or PNG.
    *   If PHI variables are present in the email or any attachment is deemed unsecured, the system shall display a modal dialog with a warning message that requires explicit user confirmation to proceed.

*   **FR-3.1.5: Print Job Management**
    *   The application shall provide a \"Print Queue\" UI view.
    *   This view shall display the status of all submitted print jobs (e.g., Queued, Processing, Completed, Failed).
    *   Users shall be able to cancel their own pending print jobs.
    *   Admin users shall be able to view, cancel, and re-prioritize any print job in the queue.
    *   If a print job fails (e.g., printer offline, out of paper), the system shall mark the job as 'Failed' and provide a reason for the failure in the queue view.
    *   Users shall be able to retry a failed print job from the queue.

#### **3.2 DICOM PACS Integration (REQ-FNC-002)**

*   **FR-3.2.1: Storage & Retrieval**
    *   The application shall be able to act as a C-STORE SCP, listening on a configurable port for incoming studies.
    *   Incoming studies received via C-STORE shall be automatically routed to predefined local folders.
    *   The application shall be able to act as a C-FIND and C-MOVE SCU.
    *   Users shall be able to query a configured PACS by Study Date, Modality, or Patient ID.
    *   Users shall be able to initiate a C-MOVE request to retrieve selected studies from the PACS to the local storage.
    *   The system shall provide clear user feedback on the status of C-MOVE operations, including progress, success, or failure with a descriptive error message (e.g., 'Destination AE Title unknown', 'Connection refused').

*   **FR-3.2.1.1: Routing Rule Configuration**
    *   The criteria for auto-routing incoming studies shall be configurable by an Admin user.
    *   The Settings Page shall provide a UI for creating routing rules.
    *   A routing rule shall be definable based on criteria such as the sending AE Title, Modality, or other specified DICOM tags.
    *   Each rule shall specify a destination folder path.

*   **FR-3.2.2: PACS Configuration**
    *   Admin users shall add, remove, and edit PACS server configurations via the application's Settings Page.
    *   Configuration of PACS servers via an external JSON file shall be disallowed.
    *   The Settings Page shall contain a section for PACS management, accessible only to Admins.
    *   The UI shall allow configuration of AE Title, IP Address, Port, and capabilities (C-STORE, C-FIND, C-MOVE) for each PACS.
    *   The UI shall provide a \"Verify\" button that performs a C-ECHO test to the selected PACS configuration.

#### **3.3 DICOM Security (REQ-FNC-003)**

*   **FR-3.3.1: Data Protection**
    *   Application logs shall be written to a local file and the Windows Event Log.
    *   Any PHI (e.g., Patient Name, Patient ID) written to logs shall be masked (e.g., \"PatientName: ******\").

*   **FR-3.3.2: Access Control**
    *   The application shall require user login for access.
    *   UI elements and features shall be enabled or disabled based on the logged-in user's role as defined in the permissions matrix (Section 2.4).

*   **FR-3.3.2.1: Session Timeout Behavior**
    *   The application session shall automatically lock after 15 minutes of no keyboard or mouse input.
    *   When the session locks, the application shall display a lock screen overlay.
    *   The current application state, including open images, shall be preserved behind the lock screen.
    *   The user shall be required to re-enter their password to unlock the session and resume activity.

*   **FR-3.3.2.2: Password Policy Management**
    *   The system shall enforce a configurable password policy for all user accounts.
    *   In the Administration area, an Admin user shall be able to configure the password policy.
    *   Configurable policy settings shall include:
        *   Minimum password length (Default: 12).
        *   Complexity requirements (e.g., must contain uppercase, lowercase, number, symbol).
        *   Password expiration period in days (e.g., 90 days).
        *   Password history count to prevent immediate reuse of old passwords (Default: 5).
    *   When an Admin resets a user's password, the system shall generate a temporary password and require the user to set a new password upon their next login.

#### **3.4 DICOM Metadata Viewing/Editing (REQ-FNC-004)**

*   **FR-3.4.1: Metadata Viewer**
    *   The application shall provide a dedicated panel to display the metadata of the selected image or series.
    *   Metadata shall be grouped by category: Patient, Study, Series, and Equipment.
    *   The metadata viewer shall include a search bar to allow filtering metadata for specific tags by name or tag number (e.g., (0010,0010)).

*   **FR-3.4.2: Metadata Editing**
    *   Only users with the Admin role shall be able to enter edit mode for metadata.
    *   Entering edit mode shall automatically create a 'working copy' of the study's files, as defined in REQ-NFR-003, to ensure original files are never modified.
    *   By default, only non-critical tags like Series Description (0008,103E) shall be editable.

*   **FR-3.4.2.1: Configurable Editable Tags**
    *   The list of DICOM tags that are editable shall be configurable by an Admin user.
    *   The Settings Page shall include a UI for Admins to add or remove DICOM tags from an \"editable\" list.

*   **FR-3.4.2.2: Audit Trail for Modifications**
    *   All modifications to DICOM metadata, file state, and user accounts shall be recorded in an audit trail.
    *   Every change to a DICOM tag shall be logged with the user's ID, a timestamp, the original value, and the new value.
    *   The audit trail shall also log file state change events, including 'Working Copy Created', 'Working Copy Modifications Saved', and 'Working Copy Discarded', with the user's ID and a timestamp.
    *   The audit trail shall log all user account management events, including user creation, deletion, role changes, and password resets, initiated by an administrator.

*   **FR-3.4.2.3: Audit Trail Viewer**
    *   The system shall provide an interface for Admin users to review and export the audit trail.
    *   An \"Audit Trail\" section in the Settings Page shall display the modification logs in a readable format.
    *   The audit trail viewer shall support filtering by user and date range.
    *   The audit trail viewer shall provide an \"Export to CSV\" button to save the filtered log data.

#### **3.5 DICOM Image Viewing (REQ-FNC-005)**

*   **FR-3.5.1: Image Rendering**
    *   The application shall provide a basic DICOM viewer.
    *   The viewer shall be able to render both monochrome and color DICOM images.
    *   The viewer shall support multi-frame series (e.g., CT/MRI/US), allowing users to scroll through frames.
    *   The viewer shall correctly apply modality-specific rendering protocols, such as default windowing for CT and inversion for X-rays.
    *   The viewer shall display overlay annotations and embedded measurements from the DICOM file.
    *   The viewer shall support GPU-accelerated rendering using DirectX.
    *   For large series, the viewer shall employ progressive loading, initially displaying a downsampled preview of the images and loading full-resolution data on-demand as the user scrolls or zooms.

*   **FR-3.5.2: Viewer UI Requirements**
    *   The viewer UI shall provide a side-by-side comparison mode.
    *   When in comparison mode, scroll and zoom actions shall be synchronizable between the two viewports.
    *   The viewer UI shall display a thumbnail grid for easy navigation within a series.

#### **3.6 DICOM File Management (REQ-FNC-006)**

*   **FR-3.6.1: Import/Export**
    *   Users shall be able to import DICOM files by dragging and dropping them from local folders, CDs, USB drives, or network shares onto the application window.
    *   Users shall be able to export selected images or series to non-DICOM formats (JPEG/PNG) with configurable resolution.

*   **FR-3.6.1.1: Import Conflict Handling**
    *   The system shall handle cases where an imported study's UID already exists in the local database.
    *   Upon detecting a duplicate Study Instance UID during import, the system shall prompt the user with a dialog.
    *   The dialog shall provide three options: \"Overwrite\" the existing study, \"Discard\" the import, or \"Save as New\" (which generates a new UID for the imported study).

*   **FR-3.6.2: Organization and Validation**
    *   Local storage of DICOM files shall follow a hierarchical structure: `[StoragePath]\\[PatientID]\\[StudyUID]\\[SeriesUID]`.
    *   The UI shall support tag-based search by Patient Name, Study Date, and Modality.
    *   During import, the system shall verify DICOM compliance (e.g., valid header, required tags) and reject non-compliant files, providing a report of which files were rejected and why.
    *   The system shall include a scheduled background task to periodically verify the integrity of the local storage, checking for file existence against database records and reporting any discrepancies.
    *   Discrepancies found by the integrity check shall be logged, and a summary report shall be generated and made available to administrators. If configured, an alert email shall be sent.

*   **FR-3.6.2.1: Storage Location Verification**
    *   An Admin user shall be able to specify any Windows-accessible storage location (local path, UNC path) for DICOM files via the Settings Page.
    *   The Settings Page shall provide a \"Test\" button next to the storage path input field.
    *   Upon clicking the \"Test\" button, the system shall attempt to write a temporary file with a unique random name to the specified path, verify its creation, and then delete it.
    *   The system shall provide immediate feedback to the user indicating whether the location is accessible and writable.

#### **3.7 DICOM Data Manipulation & Workflow (REQ-FNC-007)**

*   **FR-3.7.1: Basic Tools**
    *   The viewer shall provide a zoom functionality supporting a range from 10% to 800%.
    *   The viewer shall provide rotation functionality in 90-degree increments and free rotation (0-360 degrees).
    *   The viewer shall provide Pan and Flip (horizontal/vertical) tools.
    *   The viewer shall provide Window Level (WW/WL) adjustment that can be controlled interactively.
    *   The viewer shall provide WW/WL presets (e.g., Lung, Bone).

*   **FR-3.7.2: Annotations and Measurements**
    *   The system shall support creating non-destructive annotations and measurements.
    *   Users shall be able to draw arrows, add text, and define Regions of Interest (ROI).
    *   The system shall provide measurement tools for length, angle, and area.
    *   Measurements shall display units (mm/cm) derived from the DICOM metadata.
    *   All annotations and measurements shall be saved as separate DICOM Grayscale Softcopy Presentation State (GSPS) objects, linked to the original series.

*   **FR-3.7.3: Hanging Protocols**
    *   The system shall support customizable display layouts (hanging protocols).
    *   Users shall be able to define and save custom layouts (e.g., 2x2, 1+3) on a per-user basis.

*   **FR-3.7.3.1: Hanging Protocol Activation**
    *   Hanging protocols shall be triggerable automatically or selectable manually.
    *   An Admin shall be able to configure rules to automatically apply a hanging protocol when a study is opened, based on matching Modality and Body Part Examined tags.
    *   A user shall be able to manually select a saved hanging protocol from a dropdown list in the viewer.

*   **FR-3.7.4: Keyboard Shortcuts**
    *   The application shall support keyboard shortcuts for common actions.
    *   The application shall use `Ctrl+P` to open the Print Preview window.
    *   The application shall use `Ctrl+W` to activate the Window Level (WW/WL) tool.
    *   Other common actions such as Zoom and Pan shall have corresponding keyboard shortcuts.

#### **3.8 Anonymization (REQ-FNC-008)**

*   **FR-3.8.1: DICOM Metadata Anonymization**
    *   The system shall provide tools to remove or replace PHI from DICOM metadata.
    *   A manual mode shall allow users to select specific tags to anonymize.
    *   The system shall provide preset profiles: \"Basic De-ID\" (removes ~10 critical tags) and \"Full De-ID\" (removes 30+ tags as per HIPAA Safe Harbor).
    *   The system shall provide a pseudonymization option to replace the original Patient ID with a hashed or randomized ID, with a configurable algorithm.
    *   Anonymization actions shall create new DICOM files, leaving the original files untouched.
    *   Upon completion of an anonymization action, the system shall generate a summary report detailing the original and new study UIDs and a list of all tags that were modified, removed, or replaced.

*   **FR-3.8.2: Pixel Anonymization**
    *   The system shall allow users to apply permanent, destructive overlays of shapes or text on image pixels to mask identifiers.
    *   This action shall create a new, anonymized DICOM series and shall not modify the original file.
    *   A UI shall be provided to draw blackout shapes or burn text into the image pixel data.
    *   Executing the pixel anonymization shall save a new set of DICOM files with the modifications.
    *   The original DICOM files shall be preserved without modification.
    *   Users shall be able to save pixel anonymization templates as JSON files for reuse.

#### **3.9 System Settings and Configuration (REQ-FNC-009)**

*   **FR-3.9.1: Settings Page**
    *   A centralized Settings Page shall be available for application configuration.
    *   The Settings Page shall be located under a main \"Administration\" tab/area.
    *   Access to the Administration area shall be restricted to users with the Admin role.

*   **FR-3.9.2: System Configuration**
    *   Admins shall be able to configure system-level settings.
    *   Admins shall be able to set the default storage path for incoming DICOM studies.
    *   Admins shall be able to configure auto-purge rules (e.g., delete studies older than 365 days).
    *   Admins shall be able to configure network proxy settings for DICOM and web communication.

*   **FR-3.9.2.1: User Management**
    *   A UI shall be provided for Admins to manage user accounts.
    *   Admins shall be able to create new user accounts, specifying a username, password, and role.
    *   Admins shall be able to edit existing users, including resetting passwords and changing roles.
    *   Admins shall be able to delete user accounts.
    *   The user management UI shall provide access to configure the system-wide password policy as defined in FR-3.3.2.2.

*   **FR-3.9.2.2: Data Backup Configuration**
    *   The Administration area shall provide a UI for configuring automated database backups.
    *   An Admin user shall be able to schedule automated daily backups.
    *   The UI shall allow the Admin to specify a target backup location (local or UNC path).

*   **FR-3.9.3: User Preferences**
    *   Individual users shall be able to customize their experience.
    *   Users shall be able to customize their default annotation colors.
    *   Users shall be able to create and save their own default windowing presets.

---

### **4. Interface Requirements**

#### **4.1 User Interfaces (REQ-INT-001)**
*   The UI shall be clean, modern, and intuitive, following standard Windows application design conventions.
*   The main application window shall be organized into four primary areas/tabs: Incoming/Print Queue, Local Storage, Query/Retrieve, and Administration.
*   A toggle switch shall be available for users to switch between a Dark and Light theme.
*   The application shall provide a high-contrast mode to comply with WCAG 2.1 AA for low-vision users.
*   The application shall be fully navigable using a keyboard (e.g., Tab, Shift+Tab, Enter, Arrow keys).
*   The application shall provide consistent user feedback for actions, including progress indicators for long-running operations (e.g., import, export) and non-blocking toast notifications for success or failure of background tasks.
*   All interactive UI controls shall provide tooltips on hover.
*   The application shall be primarily controlled via keyboard and mouse.

#### **4.2 Hardware Interfaces (REQ-INT-002)**
*   The application shall interface with any Windows 10/11-supported inkjet or laser printer via the standard Windows Print API (GDI+).
*   The application shall not require direct hardware communication.

#### **4.3 Software Interfaces (REQ-INT-003)**
*   The application shall use the `fo-dicom` library for all DICOM-related operations.
*   The database shall be managed by Entity Framework Core 8.
*   The application shall communicate with the Odoo web portal's REST API over HTTPS for license validation.
*   **License Validation Workflow:**
    *   On application startup, the system shall attempt to validate its license against the Odoo API.
    *   The system shall gracefully handle specific HTTP error codes from the API (e.g., 401/403 Unauthorized, 429 Too Many Requests, 5xx Server Error) with appropriate user messaging and retry logic.
    *   If the license cannot be validated due to network unavailability, the application shall enter a grace period of 72 hours with full functionality, displaying a non-blocking warning to the user.
    *   If the license is confirmed as invalid or the grace period expires, the application shall switch to a restricted, read-only mode.
    *   In restricted mode, functions such as printing, exporting, editing, and receiving new studies via C-STORE shall be disabled until a valid license is confirmed.
*   The application shall use the native WPF renderer (DirectX-based) for standard UI.
*   The application shall use `Vortice.Windows` (DirectX 11/12 wrapper) for high-performance rendering of large DICOM images.

#### **4.4 Communication Interfaces (REQ-INT-004)**
*   The application shall support DICOM network communication over TCP/IP.
*   The application shall support configurable AE Titles, ports, and transfer syntaxes (e.g., JPEG Lossless, RLE).
*   The application shall support DICOM TLS if configured.
*   All communication with the Odoo licensing API shall use TLS 1.2 or higher. The system shall be configured to prefer TLS 1.3 where supported by both client and server.
*   The application shall use SMTP to send emails, with support for STARTTLS encryption.

---

### **5. Non-Functional Requirements**

#### **5.1 System Requirements (REQ-NFR-001)**
*   The application shall operate on Windows 10 (Version 20H2 or later) or Windows 11.
*   The system shall require a minimum hardware configuration of an x64-based, 2 GHz or faster processor.
*   The system shall require a minimum of 8 GB of RAM; 16 GB is recommended.
*   The system shall require 500 MB of hard drive space for installation, with additional space required for local DICOM storage.
*   The system shall recommend a Solid State Drive (SSD) for optimal performance.
*   The system shall require a minimum display resolution of 1920x1080.
*   The system shall require a DirectX 11 compatible graphics card for GPU-accelerated rendering.
*   The system requires network access to a configured PostgreSQL server (version 16) and a RabbitMQ server instance.
*   The application installer shall include the .NET 8 Desktop Runtime dependency.
*   The installer shall perform prerequisite checks for connectivity to the required PostgreSQL and RabbitMQ services and shall prevent installation from completing if connectivity cannot be established with user-provided connection details.
*   The installer's prerequisite check for PostgreSQL shall verify that the `pgcrypto` extension is installed and enabled on the target database.

#### **5.2 Performance Requirements (REQ-NFR-002)**
*   The application shall launch in under 5 seconds on an HDD.
*   The application shall launch in under 3 seconds on an SSD.
*   The system shall load DICOM images up to 500MB in size within 3 seconds on standard hardware.
*   For images exceeding 500MB (up to 2GB), the system shall initiate display of the first available frame or a low-resolution preview within 5 seconds.
*   For images exceeding 500MB, full data loading and rendering shall complete progressively in a background thread.
*   The application shall handle at least 10 simultaneous C-STORE operations without significant degradation in UI responsiveness.
*   UI responsiveness for user-initiated actions (e.g., button clicks, menu opening) shall be under 500ms, even during heavy background processing.
*   PDF generation for a 50-page document (A4, 300dpi) shall be submitted to the background processing queue in under 2 seconds. The generation itself shall be handled by the message queue system (RabbitMQ) to ensure guaranteed and batched processing.
*   Physical printing jobs shall be submitted to the background processing queue managed by the message queue system (RabbitMQ) to ensure reliable, sequential processing.
*   All other long-running tasks (file I/O, network requests) shall be executed asynchronously using `async/await` to keep the UI responsive at all times.

#### **5.3 Safety and Data Integrity Requirements (REQ-NFR-003)**
*   The system shall ensure that original DICOM files are never modified by viewing, printing, or annotation operations.
*   Modification actions, including metadata editing and anonymization, shall create a working copy of the original files, leaving the original files untouched.
*   There shall be only one 'original' version and one 'modified' (working copy) version of a study at any time; multiple versioning is not required.
*   A user can perform subsequent modifications on the existing working copy, overwriting it upon saving.
*   A user shall have the option to discard all modifications, which will delete the working copy and revert the view to the original files.
*   The system shall maintain a clear state indicator in the UI to show whether the user is viewing the original or modified version.
*   All state-changing actions on the working copy (creation, saving modifications, discarding modifications) shall be recorded in the audit trail.
*   **Working Copy Persistence:**
    *   If a user closes the application while a study has an unsaved working copy, the working copy files shall be persisted.
    *   Upon the next time any user opens that same study, the application shall present a modal dialog prompting the user to either \"Continue with unsaved changes\" (which loads the working copy) or \"Discard changes\" (which deletes the working copy and loads the original).

#### **5.4 Security Requirements (REQ-NFR-004)**
*   User identity shall be managed locally.
*   User passwords shall be stored in the database using a strong, salted hashing algorithm (BCrypt).
*   A role-based access control (RBAC) model shall be strictly enforced as per the permissions matrix.
*   All network connections from the application to the PostgreSQL database server shall be encrypted using TLS.
*   Data at rest within the PostgreSQL database shall be encrypted. PHI-containing columns shall be encrypted using the `pgcrypto` extension. Full-disk encryption on the database server is also required.
*   All sensitive credentials, including those for the database, SMTP server, and external APIs, shall be stored securely using the Windows Credential Manager and not in plaintext configuration files.
*   Communication with the Odoo licensing API shall use TLS 1.2 or higher.
*   SMTP communication shall be configured to use STARTTLS.
*   DICOM communication shall support DICOM TLS.
*   The \"Full De-ID\" anonymization profile shall ensure that outputs meet the criteria for HIPAA Safe Harbor as defined in 45 CFR 164.514(b)(2).
*   The application shall be developed following secure coding best practices to prevent common vulnerabilities in desktop applications, including but not limited to input validation to prevent injection attacks, secure handling of credentials, and prevention of buffer overflows.
*   The development lifecycle shall include automated dependency vulnerability scanning using OWASP Dependency-Check to identify and mitigate risks from third-party libraries.

#### **5.5 Software Quality and Reliability Attributes (REQ-NFR-005)**
*   The application shall handle network and service interruptions gracefully, such as PACS connection loss or a stopped background service, providing clear user notifications and attempting to reconnect automatically where appropriate.
*   The background Windows Service shall be configured for automatic restart on failure.
*   The message queue consumer shall handle poison messages by moving them to a dead-letter queue after a configurable number of failed processing attempts, preventing blockage of the main queue.
*   Generated PDFs shall be viewable in Adobe Acrobat Reader 9+ and other common PDF viewers.
*   The application shall be developed using a modular architecture to separate concerns (UI, Core Logic, Data Access).
*   The MVVM design pattern shall be used in the WPF application.
*   The application UI shall support English (default) and Spanish for UI text in Phase 1.
*   **Data Backup and Recovery:**
    *   The system shall have a defined Recovery Point Objective (RPO) of 24 hours.
    *   The system shall have a defined Recovery Time Objective (RTO) of 4 hours.
    *   The administrative documentation must include a detailed, step-by-step backup procedure using standard PostgreSQL tools (e.g., `pg_dump`) and a corresponding restore procedure.
    *   The documentation shall mandate and describe a process for performing and documenting periodic recovery tests (at least semi-annually) to validate the backup integrity and RTO.

---

### **6. Deployment and Documentation**

#### **6.1 Deployment Guide**
*   A separate deployment guide and a consolidated Administrator's Guide shall be provided for system administrators.
*   The deployment guide shall include recommended installation and configuration procedures for all prerequisite services, including PostgreSQL and RabbitMQ.
*   The deployment guide shall provide reference architectures and baseline configuration settings for both single-node (small clinic) and high-availability (hospital) environments.
*   The deployment guide shall provide example scripts for configuration management using Infrastructure as Code (IaC) principles (e.g., Ansible playbooks or PowerShell DSC scripts) to ensure consistent and repeatable server setup.
*   The deployment guide shall include detailed rollback procedures for application updates.
*   The installer shall bundle a containerized (Docker Compose) setup of the prerequisite services for evaluation and testing purposes.
*   The Administrator's Guide shall cover all administrative functions, including user management, system configuration, backup and recovery procedures, and interpreting the system health dashboard and logs.

---

### **7. Reports, Monitoring, and Logging**

#### **7.1 Reports (REQ-REP-001)**
*   The application shall provide a feature for Admin users to export the metadata modification audit trail as a CSV file.

#### **7.2 Monitoring & Logging (REQ-REP-002)**
*   The Serilog logging framework shall be configured to write to two destinations (sinks): `Serilog.Sinks.File` and `Serilog.Sinks.EventLog`.
*   Logs shall be written to a local rolling text file stored in the user's `AppData\\Logs` directory.
*   Log files shall be subject to a configurable rotation and retention policy (e.g., rotate daily, retain for 30 days) to manage disk space usage.
*   All PHI written to any log destination, including local text files and the Windows Event Log, shall be masked.
*   For each user-initiated asynchronous operation (e.g., print, C-MOVE), a unique correlation ID shall be generated and included in all related log entries across the client, message queue, and background service to facilitate end-to-end tracing.
*   Logs for critical errors and security-related events shall be written to the Windows Event Log.
*   Logs shall include timestamps, severity levels, and detailed context for events such as DICOM network operations, file I/O, and application errors.
*   **System Health Dashboard:**
    *   The Administration area shall contain a system health dashboard.
    *   The dashboard shall display the real-time status of key components:
        *   Windows Service (Running/Stopped).
        *   Connectivity to PostgreSQL (Connected/Disconnected).
        *   Connectivity to RabbitMQ (Connected/Disconnected).
        *   Current message queue depth (number of unprocessed messages).
        *   Number of messages in the dead-letter queue.
        *   Available disk space for the configured DICOM storage path, with configurable warning and critical thresholds (e.g., warn at 20% free, critical at 10% free).
*   **Critical Error Alerting:**
    *   The system shall provide a mechanism for an Admin to configure an administrator email address for alerts.
    *   When a critical error is logged to the Windows Event Log, or a monitored metric on the health dashboard crosses a critical threshold, the system shall automatically send an alert email to the configured address.

---

### **8. Transition Requirements**

#### **8.1 Implementation Strategy**
*   The system shall be deployed using a Phased Rollout methodology.
*   An initial pilot phase shall be conducted with a limited group of representative users (e.g., one department or clinic) to validate system functionality, performance, and user acceptance in a production environment.
*   The full rollout shall proceed in scheduled waves to remaining user groups based on the successful completion of the pilot phase.
*   Each phase shall have clearly defined entry and exit criteria.

#### **8.2 Data Migration**
*   A dedicated data migration utility shall be provided to migrate study metadata and DICOM files from the legacy system.
*   The migration process shall follow an Extract, Transform, Load, and Validate (ETLV) sequence.
*   A comprehensive data mapping document shall be created, detailing the mapping of every required data field from the legacy system schema to the new PostgreSQL schema.
*   The migration utility shall generate detailed logs of all operations, including successes, failures, and data transformation warnings.
*   A mandatory migration dry-run shall be performed in a pre-production environment to validate the process, estimate timing, and identify potential issues.
*   Post-migration, a validation report shall be generated, comparing record counts and key data points between the source and target systems to ensure data integrity and completeness.

#### **8.3 Training Plan**
*   Role-based training materials shall be developed for both Technician and Administrator roles.
*   Training materials shall include user manuals, quick-start guides, and demonstration videos.
*   Training shall be delivered via instructor-led sessions (in-person or virtual) for the initial pilot group and key administrators.
*   A dedicated, sandboxed training environment, populated with anonymized sample data, shall be provided for hands-on user training and practice.
*   Computer-Based Training (CBT) modules shall be made available for subsequent user groups and for onboarding new staff post-deployment.

#### **8.4 System Cutover**
*   A detailed cutover plan shall be documented, outlining all activities, responsibilities, and timelines for the transition from the legacy system to the new system.
*   The plan shall include a pre-cutover checklist to ensure all prerequisites (e.g., infrastructure readiness, final data migration, user account creation) are met.
*   The cutover shall be scheduled during a period of low operational activity (e.g., a weekend).
*   The plan shall define a clear "point of no return," after which reverting to the legacy system is no longer feasible.
*   A rollback plan shall be documented, defining the specific steps and criteria for aborting the cutover and reverting to the legacy system if critical issues arise before the point of no return.
*   A post-cutover support structure shall be established, including on-site or readily available "floor walker" support for end-users during the initial days of operation.

#### **8.5 Legacy System Decommissioning**
*   The legacy system shall operate in a read-only parallel mode for a defined period (e.g., 30 days) post-cutover to allow for data validation and user adjustment.
*   After the parallel operation period, user access to the legacy system shall be revoked.
*   A full archival backup of the legacy system's database and file store shall be performed and stored in a secure, long-term archive location in accordance with data retention policies.
*   The legacy system hardware and software shall be formally decommissioned only after successful completion and sign-off of the archival process.