# Specification

# 1. Requirements

## 1.1. 64
### 1.1.2. Section
3.1 DICOM Printing

### 1.1.3. Section_Id
REQ-FNC-001

### 1.1.4. Section_Requirement_Text
FR-3.1.1: Windows Print Service Integration
The system shall provide a DICOM Print Service that integrates directly with the Windows Print API to enable printing on commercial inkjet/laser printers.
The application installer shall register and start a Windows Service named "DICOM Print Service".
The service shall automatically detect all system-installed printers.
The service shall fetch and display driver-supported resolutions for the selected printer.

<<$Addition>> FR-3.1.1.1: Application-Service Communication
Enhancement Justification: The original requirement specified a Windows Service but omitted how the user-facing application interacts with it. This addition defines the communication link and necessary error handling, ensuring a robust and user-friendly printing process.
The main desktop application shall communicate with the Windows Service via Named Pipes for Inter-Process Communication (IPC).
The application shall be able to submit a serialized print job object, containing image data, layout, and overlay information, to the service.
The application shall gracefully handle and notify the user with a non-blocking message if the Windows Service is stopped or unresponsive.
The "Print" button in the UI shall be disabled if the print service is not available.

FR-3.1.2: Print Layout Configuration
The print preview UI shall provide predefined page templates: Single image (A4/A3), 2x2 grid (A4), and 1+3 comparison (A3).
The print preview UI shall provide modality-specific presets, such as 1x1 for X-ray and 4x4 for CT slices.
Users shall be able to customize page margins within a range of 5-20mm.
Users shall be able to select page orientation (Portrait/Landscape).
Users shall be able to select image scaling options: Fit-to-Page and Actual Size.

<<$Addition>> FR-3.1.2.1: Configurable Auto-Arrangement
Enhancement Justification: The term "Auto-arrange" is ambiguous. This addition provides the necessary functionality for administrators to define and customize these automated behaviors, making the feature adaptable to different clinical workflows.
The system shall support auto-arranging images on the print layout based on configurable rules.
An Admin user shall be able to configure the rules for auto-arrangement in the Settings Page.
The configuration shall allow mapping specific modalities to default layouts and defining the sorting order of images.

FR-3.1.2.2: Manual Rearrangement
Users shall be able to manually rearrange images within the print preview grid via drag-and-drop.

FR-3.1.3: Branding & De-identification Overlays
Users shall be able to upload a custom logo in PNG or JPEG format.
The logo position shall be configurable to Header, Footer, or custom X/Y coordinates.
Logo scaling shall be adjustable from 10% to 50% of the page width.
Users shall be able to add custom text fields with configurable font, size, and color.
Custom text fields shall support variables that are replaced at print time (e.g., `[PatientID]`, `[Date]`).

<<$Change>> FR-3.1.3.1: Non-Destructive Templated Overlays
Enhancement Justification: This clarifies a potential ambiguity. It specifies that overlays applied during printing are non-destructive, distinguishing this feature from the permanent pixel anonymization feature (FR-3.8.2). This prevents accidental modification of original medical data.
The system shall support non-destructive, print-time pixel-based overlay masks for de-identification.
Applying a blackout overlay in the print preview shall not alter the original DICOM file on disk.
The system shall provide pre-built templates for de-identification (e.g., Chest X-Ray De-ID).
Overlay templates shall support configurable rectangular and polygonal regions.

<<$Addition>> FR-3.1.3.2: Overlay Template Management
Enhancement Justification: The requirement for "configurable regions" implies the need for an editor. This addition explicitly defines the requirement for a template management UI, making the feature usable.
The system shall provide an interface for Admin users to manage overlay templates within the Settings Page.
The interface shall allow an Admin user to create new overlay templates by drawing and saving rectangular/polygonal regions.
Admin users shall be able to edit and delete existing templates.

FR-3.1.4: Print Output Formats
The system shall send the configured print job to a selected Windows printer.
The application shall list and allow selection from all available Windows printers.
The final output from the physical printer shall match the print preview UI.
The system shall generate a PDF file of the print layout.
The generated PDF shall be PDF/A-3 compliant.
The system shall provide an option to embed DICOM metadata into the PDF file.
The system shall provide an option to password-protect the PDF using AES-256 encryption.
The system shall be able to attach the print output as a PDF or image(s) to an email.
The Settings Page shall provide fields to configure an SMTP server (host, port, credentials, TLS/STARTTLS).
Users shall be able to attach the output as a single PDF or as individual image files.
<<$Change>> Exported images for email attachments shall be in a user-selectable format (JPEG/PNG).
Enhancement Justification: The original requirement was ambiguous about the format of "images". This change specifies the supported formats and behavior for multi-image attachments, creating a clear and testable requirement.
If multiple images are selected for email attachment, they shall be attached as individual files.
The system shall allow the creation of pre-filled email templates with variables (e.g., `[PatientName]`, `[StudyDate]`).

<<$Addition>> FR-3.1.4.4: Email Security Warning
Enhancement Justification: This adds a critical security checkpoint to mitigate the risk of HIPAA violations by ensuring users are aware when they are about to transmit unsecured Protected Health Information.
As a business rule, if an email template is configured to include PHI variables or if an attached PDF is not password-protected, the system shall display a warning to the user about sending unsecured PHI before sending the email.
When the "Send" button is clicked, the system shall check for PHI variables in the email body/subject and check if any PDF attachment is unencrypted.
If either condition is true, the system shall display a modal dialog with a warning message that requires explicit user confirmation to proceed.

### 1.1.5. Requirement_Type
functional

### 1.1.6. Priority


### 1.1.7. Original_Text


### 1.1.8. Change_Comments
False

### 1.1.9. Enhancement_Justification


## 1.2. 65
### 1.2.2. Section
3.2 DICOM PACS Integration

### 1.2.3. Section_Id
REQ-FNC-002

### 1.2.4. Section_Requirement_Text
FR-3.2.1: Storage & Retrieval
The application shall be able to act as a C-STORE SCP, listening on a configurable port for incoming studies.
Incoming studies received via C-STORE shall be automatically routed to predefined local folders.
The application shall be able to act as a C-FIND and C-MOVE SCU.
Users shall be able to query a configured PACS by Study Date, Modality, or Patient ID.
Users shall be able to initiate a C-MOVE request to retrieve selected studies from the PACS to the local storage.

<<$Addition>> FR-3.2.1.1: Routing Rule Configuration
Enhancement Justification: The "Auto-routing" feature lacks definition. This addition specifies that the rules governing this routing must be user-configurable, making the feature flexible and adaptable to various network environments.
The criteria for auto-routing incoming studies shall be configurable by an Admin user.
The Settings Page shall provide a UI for creating routing rules.
A routing rule shall be definable based on criteria such as the sending AE Title, Modality, or other specified DICOM tags.
Each rule shall specify a destination folder path.

FR-3.2.2: PACS Configuration
<<$Change>> Admin users shall add, remove, and edit PACS server configurations via the application's Settings Page.
Enhancement Justification: This resolves a contradiction between the original requirement for a "JSON configuration file" and the expectation of a user-friendly "Settings Page" (FR-3.7). Centralizing configuration in the UI improves usability and reduces errors associated with manual file editing, providing a consistent user experience.
Configuration of PACS servers via an external JSON file shall be disallowed.
The Settings Page shall contain a section for PACS management, accessible only to Admins.
The UI shall allow configuration of AE Title, IP Address, Port, and capabilities (C-STORE, C-FIND, C-MOVE) for each PACS.
The UI shall provide a "Verify" button that performs a C-ECHO test to the selected PACS configuration.

### 1.2.5. Requirement_Type
functional

### 1.2.6. Priority


### 1.2.7. Original_Text


### 1.2.8. Change_Comments
False

### 1.2.9. Enhancement_Justification


## 1.3. 66
### 1.3.2. Section
3.3 DICOM Security

### 1.3.3. Section_Id
REQ-FNC-003

### 1.3.4. Section_Requirement_Text
FR-3.3.1: Data Protection
The local SQLite database file shall be encrypted using AES-256.
Application logs shall be written to a local file and the Windows Event Log.
Any PHI (e.g., Patient Name, Patient ID) written to logs shall be masked (e.g., "PatientName: ******").

FR-3.3.2: Access Control
The application shall require user login for access.
UI elements and features shall be enabled or disabled based on the logged-in user's role as defined in the permissions matrix (Section 2.3.2).

<<$Addition>> FR-3.3.2.1: Session Timeout Behavior
Enhancement Justification: The term "session timeout" is ambiguous for a desktop application. This clarifies the exact behavior (lock screen, password re-entry), ensuring the security requirement is implemented effectively without data loss.
The application session shall automatically lock after 15 minutes of no keyboard or mouse input.
When the session locks, the application shall display a lock screen overlay.
The current application state, including open images, shall be preserved behind the lock screen.
The user shall be required to re-enter their password to unlock the session and resume activity.

### 1.3.5. Requirement_Type
functional

### 1.3.6. Priority


### 1.3.7. Original_Text


### 1.3.8. Change_Comments
False

### 1.3.9. Enhancement_Justification


## 1.4. 62
### 1.4.2. Section
3.4 DICOM Metadata Viewing/Editing

### 1.4.3. Section_Id
REQ-FNC-004

### 1.4.4. Section_Requirement_Text
Enhancement Justification: To support the new non-destructive editing workflow defined in REQ-NFR-003, the audit trail must capture all state-changing actions related to file modifications. This requirement is enhanced to explicitly include the logging of 'Working Copy Created', 'Working Copy Modified', and 'Working Copy Discarded' events.

--- Original Text ---
FR-3.4.1: Metadata Viewer
The application shall provide a dedicated panel to display the metadata of the selected image or series.
Metadata shall be grouped by category: Patient, Study, Series, and Equipment.
The metadata viewer shall include a search bar to allow filtering metadata for specific tags by name or tag number (e.g., (0010,0010)).

FR-3.4.2: Metadata Editing
Only users with the Admin role shall be able to enter edit mode for metadata.
By default, only non-critical tags like Series Description (0008,103E) shall be editable.

<<$Addition>> FR-3.4.2.1: Configurable Editable Tags
Enhancement Justification: Hard-coding the list of editable tags is inflexible. This addition allows administrators to customize data governance policies according to their institution's needs.
The list of DICOM tags that are editable shall be configurable by an Admin user.
The Settings Page shall include a UI for Admins to add or remove DICOM tags from an "editable" list.

FR-3.4.2.2: Audit Trail for Modifications
All modifications to DICOM metadata shall be recorded in an audit trail.
Every change to a DICOM tag shall be logged with the user's ID, a timestamp, the original value, and the new value.

<<$Addition>> FR-3.4.2.3: Audit Trail Viewer
Enhancement Justification: An audit trail is useless without a way to view or export it. This addition provides the necessary UI for administrators to perform security and data integrity audits.
The system shall provide an interface for Admin users to review and export the audit trail.
An "Audit Trail" section in the Settings Page shall display the modification logs in a readable format.
The audit trail viewer shall support filtering by user and date range.
The audit trail viewer shall provide an "Export to CSV" button to save the filtered log data.

--- Enhanced Specification ---
FR-3.4.1: Metadata Viewer
The application shall provide a dedicated panel to display the metadata of the selected image or series.
Metadata shall be grouped by category: Patient, Study, Series, and Equipment.
The metadata viewer shall include a search bar to allow filtering metadata for specific tags by name or tag number (e.g., (0010,0010)).

FR-3.4.2: Metadata Editing
Only users with the Admin role shall be able to enter edit mode for metadata.
By default, only non-critical tags like Series Description (0008,103E) shall be editable.

<<$Addition>> FR-3.4.2.1: Configurable Editable Tags
Enhancement Justification: Hard-coding the list of editable tags is inflexible. This addition allows administrators to customize data governance policies according to their institution's needs.
The list of DICOM tags that are editable shall be configurable by an Admin user.
The Settings Page shall include a UI for Admins to add or remove DICOM tags from an "editable" list.

FR-3.4.2.2: Audit Trail for Modifications
All modifications to DICOM metadata and file state shall be recorded in an audit trail.
Every change to a DICOM tag shall be logged with the user's ID, a timestamp, the original value, and the new value.
The audit trail shall also log file state change events, including 'Working Copy Created', 'Working Copy Modifications Saved', and 'Working Copy Discarded', with the user's ID and a timestamp.

<<$Addition>> FR-3.4.2.3: Audit Trail Viewer
Enhancement Justification: An audit trail is useless without a way to view or export it. This addition provides the necessary UI for administrators to perform security and data integrity audits.
The system shall provide an interface for Admin users to review and export the audit trail.
An "Audit Trail" section in the Settings Page shall display the modification logs in a readable format.
The audit trail viewer shall support filtering by user and date range.
The audit trail viewer shall provide an "Export to CSV" button to save the filtered log data.

### 1.4.5. Requirement_Type
functional

### 1.4.6. Priority


### 1.4.7. Original_Text


### 1.4.8. Change_Comments
False

### 1.4.9. Enhancement_Justification


## 1.5. 67
### 1.5.2. Section
3.5 DICOM Image Viewing

### 1.5.3. Section_Id
REQ-FNC-005

### 1.5.4. Section_Requirement_Text
FR-3.5.1: Image Rendering
The application shall provide a basic DICOM viewer.
The viewer shall be able to render both monochrome and color DICOM images.
The viewer shall support multi-frame series (e.g., CT/MRI/US), allowing users to scroll through frames.
The viewer shall correctly apply modality-specific rendering protocols, such as default windowing for CT and inversion for X-rays.
The viewer shall display overlay annotations and embedded measurements from the DICOM file.
The viewer shall support GPU-accelerated rendering using DirectX.

FR-3.5.2: Viewer UI Requirements
The viewer UI shall provide a side-by-side comparison mode.
When in comparison mode, scroll and zoom actions shall be synchronizable between the two viewports.
The viewer UI shall display a thumbnail grid for easy navigation within a series.

### 1.5.5. Requirement_Type
functional

### 1.5.6. Priority


### 1.5.7. Original_Text


### 1.5.8. Change_Comments
False

### 1.5.9. Enhancement_Justification


## 1.6. 55
### 1.6.2. Section
3.6 DICOM File Management

### 1.6.3. Section_Id
REQ-FNC-006

### 1.6.4. Section_Requirement_Text
Enhancement Justification: User requested the ability to specify and test the accessibility of the DICOM storage location. Enhanced requirement to include a new sub-requirement (FR-3.6.2.1) for a configurable storage path with a 'Test' button to verify write/delete permissions, ensuring system reliability and preventing runtime I/O errors.

--- Original Text ---
FR-3.6.1: Import/Export
Users shall be able to import DICOM files by dragging and dropping them from local folders, CDs, USB drives, or network shares onto the application window.
Users shall be able to export selected images or series to non-DICOM formats (JPEG/PNG) with configurable resolution.

<<$Addition>> FR-3.6.1.1: Import Conflict Handling
Enhancement Justification: This addresses a common data conflict scenario. Providing clear options for handling duplicate UIDs prevents accidental data loss and ensures data integrity within the local storage.
The system shall handle cases where an imported study's UID already exists in the local database.
Upon detecting a duplicate Study Instance UID during import, the system shall prompt the user with a dialog.
The dialog shall provide three options: "Overwrite" the existing study, "Discard" the import, or "Save as New" (which generates a new UID for the imported study).

FR-3.6.2: Organization and Validation
Local storage of DICOM files shall follow a hierarchical structure: `[StoragePath]\[PatientID]\[StudyUID]\[SeriesUID]`.
The UI shall support tag-based search by Patient Name, Study Date, and Modality.
During import, the system shall verify DICOM compliance (e.g., valid header, required tags) and reject non-compliant files.

--- Enhanced Specification ---
FR-3.6.1: Import/Export
Users shall be able to import DICOM files by dragging and dropping them from local folders, CDs, USB drives, or network shares onto the application window.
Users shall be able to export selected images or series to non-DICOM formats (JPEG/PNG) with configurable resolution.

<<$Addition>> FR-3.6.1.1: Import Conflict Handling
Enhancement Justification: This addresses a common data conflict scenario. Providing clear options for handling duplicate UIDs prevents accidental data loss and ensures data integrity within the local storage.
The system shall handle cases where an imported study's UID already exists in the local database.
Upon detecting a duplicate Study Instance UID during import, the system shall prompt the user with a dialog.
The dialog shall provide three options: "Overwrite" the existing study, "Discard" the import, or "Save as New" (which generates a new UID for the imported study).

FR-3.6.2: Organization and Validation
Local storage of DICOM files shall follow a hierarchical structure: `[StoragePath]\[PatientID]\[StudyUID]\[SeriesUID]`.
The UI shall support tag-based search by Patient Name, Study Date, and Modality.
During import, the system shall verify DICOM compliance (e.g., valid header, required tags) and reject non-compliant files.

FR-3.6.2.1: Storage Location Verification
An Admin user shall be able to specify any Windows-accessible storage location (local path, UNC path) for DICOM files via the Settings Page.
The Settings Page shall provide a "Test" button next to the storage path input field.
Upon clicking the "Test" button, the system shall attempt to write a temporary file with a unique random name to the specified path, verify its creation, and then delete it.
The system shall provide immediate feedback to the user indicating whether the location is accessible and writable.

### 1.6.5. Requirement_Type
functional

### 1.6.6. Priority


### 1.6.7. Original_Text


### 1.6.8. Change_Comments
False

### 1.6.9. Enhancement_Justification


## 1.7. 68
### 1.7.2. Section
3.7 DICOM Data Manipulation & Workflow

### 1.7.3. Section_Id
REQ-FNC-007

### 1.7.4. Section_Requirement_Text
FR-3.7.1: Basic Tools
The viewer shall provide a zoom functionality supporting a range from 10% to 800%.
The viewer shall provide rotation functionality in 90-degree increments and free rotation (0-360 degrees).
The viewer shall provide Pan and Flip (horizontal/vertical) tools.
The viewer shall provide Window Level (WW/WL) adjustment that can be controlled interactively.
The viewer shall provide WW/WL presets (e.g., Lung, Bone).

FR-3.7.2: Annotations and Measurements
The system shall support creating non-destructive annotations and measurements.
Users shall be able to draw arrows, add text, and define Regions of Interest (ROI).
The system shall provide measurement tools for length, angle, and area.
Measurements shall display units (mm/cm) derived from the DICOM metadata.
All annotations and measurements shall be saved as separate DICOM Grayscale Softcopy Presentation State (GSPS) objects, linked to the original series.

FR-3.7.3: Hanging Protocols
The system shall support customizable display layouts (hanging protocols).
Users shall be able to define and save custom layouts (e.g., 2x2, 1+3) on a per-user basis.

<<$Addition>> FR-3.7.3.1: Hanging Protocol Activation
Enhancement Justification: This clarifies how hanging protocols are activated, defining both automated, intelligent application and manual override, which is crucial for workflow efficiency.
Hanging protocols shall be triggerable automatically or selectable manually.
An Admin shall be able to configure rules to automatically apply a hanging protocol when a study is opened, based on matching Modality and Body Part Examined tags.
A user shall be able to manually select a saved hanging protocol from a dropdown list in the viewer.

FR-3.7.4: Keyboard Shortcuts
The application shall support keyboard shortcuts for common actions.
The application shall use `Ctrl+P` to open the Print Preview window.
The application shall use `Ctrl+W` to activate the Window Level (WW/WL) tool.
Other common actions such as Zoom and Pan shall have corresponding keyboard shortcuts.

### 1.7.5. Requirement_Type
functional

### 1.7.6. Priority


### 1.7.7. Original_Text


### 1.7.8. Change_Comments
False

### 1.7.9. Enhancement_Justification


## 1.8. 69
### 1.8.2. Section
3.8 Anonymization

### 1.8.3. Section_Id
REQ-FNC-008

### 1.8.4. Section_Requirement_Text
FR-3.8.1: DICOM Metadata Anonymization
The system shall provide tools to remove or replace PHI from DICOM metadata.
A manual mode shall allow users to select specific tags to anonymize.
The system shall provide preset profiles: "Basic De-ID" (removes ~10 critical tags) and "Full De-ID" (removes 30+ tags as per HIPAA Safe Harbor).
The system shall provide a pseudonymization option to replace the original Patient ID with a hashed or randomized ID, with a configurable algorithm.
Anonymization actions shall create new DICOM files, leaving the original files untouched.

FR-3.8.2: Pixel Anonymization
<<$Change>> The system shall allow users to apply permanent, destructive overlays of shapes or text on image pixels to mask identifiers.
Enhancement Justification: This clarifies the distinction between this feature and the non-destructive print-time overlays (FR-3.1.3.1). Specifying that this action is permanent and creates new files is critical for data integrity and user understanding.
This action shall create a new, anonymized DICOM series and shall not modify the original file.
A UI shall be provided to draw blackout shapes or burn text into the image pixel data.
Executing the pixel anonymization shall save a new set of DICOM files with the modifications.
The original DICOM files shall be preserved without modification.
Users shall be able to save pixel anonymization templates as JSON files for reuse.

### 1.8.5. Requirement_Type
functional

### 1.8.6. Priority


### 1.8.7. Original_Text


### 1.8.8. Change_Comments
False

### 1.8.9. Enhancement_Justification


## 1.9. 70
### 1.9.2. Section
3.9 System Settings and Configuration

### 1.9.3. Section_Id
REQ-FNC-009

### 1.9.4. Section_Requirement_Text
FR-3.9.1: Settings Page
A centralized Settings Page shall be available for application configuration.
<<$Change>> The Settings Page shall be located under a main "Administration" tab/area.
Enhancement Justification: This clarifies the location and access control for the settings page, aligning with the role-based security model.
Access to the Administration area shall be restricted to users with the Admin role.

FR-3.9.2: System Configuration
Admins shall be able to configure system-level settings.
Admins shall be able to set the default storage path for incoming DICOM studies.
Admins shall be able to configure auto-purge rules (e.g., delete studies older than 365 days).
Admins shall be able to configure network proxy settings for DICOM and web communication.

<<$Addition>> FR-3.9.2.1: User Management
Enhancement Justification: This fills a critical gap. A system with role-based access control requires a mechanism for managing user accounts.
A UI shall be provided for Admins to manage user accounts.
Admins shall be able to create new user accounts, specifying a username, password, and role.
Admins shall be able to edit existing users, including resetting passwords and changing roles.
Admins shall be able to delete user accounts.

FR-3.9.3: User Preferences
Individual users shall be able to customize their experience.
Users shall be able to customize their default annotation colors.
Users shall be able to create and save their own default windowing presets.

### 1.9.5. Requirement_Type
functional

### 1.9.6. Priority


### 1.9.7. Original_Text


### 1.9.8. Change_Comments
False

### 1.9.9. Enhancement_Justification


## 1.10. 71
### 1.10.2. Section
4.1 User Interfaces

### 1.10.3. Section_Id
REQ-INT-001

### 1.10.4. Section_Requirement_Text
The UI shall be clean, modern, and intuitive, following standard Windows application design conventions.
<<$Change>> The main application window shall be organized into four primary areas/tabs: Incoming/Print Queue, Local Storage, Query/Retrieve, and Administration.
Enhancement Justification: The original three-tab layout omitted a place for critical administrative functions. Adding a dedicated "Administration" area provides a logical home for settings, user management, and audit trails, improving UI organization.
A toggle switch shall be available for users to switch between a Dark and Light theme.
The application shall provide a high-contrast mode to comply with WCAG 2.1 AA for low-vision users.
The application shall be primarily controlled via keyboard and mouse.

### 1.10.5. Requirement_Type
interface

### 1.10.6. Priority


### 1.10.7. Original_Text


### 1.10.8. Change_Comments
False

### 1.10.9. Enhancement_Justification


## 1.11. 72
### 1.11.2. Section
4.2 Hardware Interfaces

### 1.11.3. Section_Id
REQ-INT-002

### 1.11.4. Section_Requirement_Text
The application shall interface with any Windows 10/11-supported inkjet or laser printer via the standard Windows Print API (GDI+).
The application shall not require direct hardware communication.

### 1.11.5. Requirement_Type
interface

### 1.11.6. Priority


### 1.11.7. Original_Text


### 1.11.8. Change_Comments
False

### 1.11.9. Enhancement_Justification


## 1.12. 73
### 1.12.2. Section
4.3 Software Interfaces

### 1.12.3. Section_Id
REQ-INT-003

### 1.12.4. Section_Requirement_Text
The application shall use the `fo-dicom` library for all DICOM-related operations.
The application shall interface with a local SQLite database via `Microsoft.Data.Sqlite` with SQLCipher support.
The database shall be managed by Entity Framework Core 6+.
The application shall communicate with the Odoo web portal's REST API over HTTPS for license validation.
The application shall use the native WPF renderer (DirectX-based) for standard UI.
The application shall use `Vortice.Windows` (DirectX 11/12 wrapper) for high-performance rendering of large DICOM images.

### 1.12.5. Requirement_Type
interface

### 1.12.6. Priority


### 1.12.7. Original_Text


### 1.12.8. Change_Comments
False

### 1.12.9. Enhancement_Justification


## 1.13. 74
### 1.13.2. Section
4.4 Communication Interfaces

### 1.13.3. Section_Id
REQ-INT-004

### 1.13.4. Section_Requirement_Text
The application shall support DICOM network communication over TCP/IP.
The application shall support configurable AE Titles, ports, and transfer syntaxes (e.g., JPEG Lossless, RLE).
The application shall support DICOM TLS if configured.
All communication with the Odoo licensing API shall use HTTPS over TLS 1.2+.
The application shall use SMTP to send emails, with support for STARTTLS encryption.

### 1.13.5. Requirement_Type
interface

### 1.13.6. Priority


### 1.13.7. Original_Text


### 1.13.8. Change_Comments
False

### 1.13.9. Enhancement_Justification


## 1.14. 63
### 1.14.2. Section
5.0 Non-Functional Requirements

### 1.14.3. Section_Id
REQ-NFR-001

### 1.14.4. Section_Requirement_Text
Enhancement Justification: The architectural shift to include PostgreSQL and RabbitMQ (REQ-TEC-001) introduces new external dependencies. This requirement is enhanced to include these services in the system requirements and to mandate that the application installer must check for their presence and guide the user in configuration, filling a critical deployment gap.

--- Original Text ---
The application shall operate on Windows 10 (Version 20H2 or later) or Windows 11.
The system shall require a minimum hardware configuration of an x64-based, 2 GHz or faster processor.
The system shall require a minimum of 8 GB of RAM; 16 GB is recommended.
The system shall require 500 MB of hard drive space for installation, with additional space required for local DICOM storage.
The system shall recommend a Solid State Drive (SSD) for optimal performance.
The system shall require a minimum display resolution of 1920x1080.
The system shall require a DirectX 11 compatible graphics card for GPU-accelerated rendering.
The application installer shall include the .NET 6+ Desktop Runtime dependency.

--- Enhanced Specification ---
The application shall operate on Windows 10 (Version 20H2 or later) or Windows 11.
The system shall require a minimum hardware configuration of an x64-based, 2 GHz or faster processor.
The system shall require a minimum of 8 GB of RAM; 16 GB is recommended.
The system shall require 500 MB of hard drive space for installation, with additional space required for local DICOM storage.
The system shall recommend a Solid State Drive (SSD) for optimal performance.
The system shall require a minimum display resolution of 1920x1080.
The system shall require a DirectX 11 compatible graphics card for GPU-accelerated rendering.
The system requires network access to a configured PostgreSQL server (version 13+) and a RabbitMQ server instance.
The application installer shall include the .NET 6+ Desktop Runtime dependency.
The installer shall perform prerequisite checks for connectivity to the required PostgreSQL and RabbitMQ services and shall prevent installation from completing if connectivity cannot be established with user-provided connection details.

### 1.14.5. Requirement_Type
non_functional

### 1.14.6. Priority


### 1.14.7. Original_Text


### 1.14.8. Change_Comments
False

### 1.14.9. Enhancement_Justification


## 1.15. 56
### 1.15.2. Section
5.1 Performance Requirements

### 1.15.3. Section_Id
REQ-NFR-002

### 1.15.4. Section_Requirement_Text
Enhancement Justification: User requested a more robust handling mechanism for long-running tasks like PDF generation and printing. Enhanced requirement to specify the use of a message queue (e.g., RabbitMQ) for these tasks. This change decouples these processes from the main application, improves scalability, and ensures guaranteed, queued processing even under heavy load.

--- Original Text ---
The application shall launch in under 5 seconds on an HDD.
The application shall launch in under 3 seconds on an SSD.
<<$Change>> The system shall load DICOM images up to 500MB in size within 3 seconds on standard hardware.
<<$Change>> For images exceeding 500MB (up to 2GB), the system shall initiate display of the first available frame or a low-resolution preview within 5 seconds.
<<$Change>> For images exceeding 500MB, full data loading and rendering shall complete progressively in a background thread.
Enhancement Justification: The original requirement of "Load images _2GB in size within _3 seconds on standard hardware" was deemed infeasible. The revised requirement sets a realistic performance target for common large studies and provides a progressive loading strategy for exceptionally large files, ensuring a responsive user experience.
The application shall handle at least 10 simultaneous C-STORE operations without significant degradation in UI responsiveness.
PDF generation time shall be under 10 seconds for a 50-page document (A4, 300dpi).
All long-running tasks (file I/O, network requests, PDF generation) shall be executed asynchronously using `async/await` to keep the UI responsive at all times.

--- Enhanced Specification ---
The application shall launch in under 5 seconds on an HDD.
The application shall launch in under 3 seconds on an SSD.
<<$Change>> The system shall load DICOM images up to 500MB in size within 3 seconds on standard hardware.
<<$Change>> For images exceeding 500MB (up to 2GB), the system shall initiate display of the first available frame or a low-resolution preview within 5 seconds.
<<$Change>> For images exceeding 500MB, full data loading and rendering shall complete progressively in a background thread.
Enhancement Justification: The original requirement of "Load images _2GB in size within _3 seconds on standard hardware" was deemed infeasible. The revised requirement sets a realistic performance target for common large studies and provides a progressive loading strategy for exceptionally large files, ensuring a responsive user experience.
The application shall handle at least 10 simultaneous C-STORE operations without significant degradation in UI responsiveness.
PDF generation for a 50-page document (A4, 300dpi) shall be submitted to a background processing queue in under 2 seconds. The generation itself shall be handled by a message queue system (e.g., RabbitMQ) to ensure guaranteed and batched processing.
Physical printing jobs shall be submitted to a background processing queue managed by a message queue system (e.g., RabbitMQ) to ensure reliable, sequential processing.
All other long-running tasks (file I/O, network requests) shall be executed asynchronously using `async/await` to keep the UI responsive at all times.

### 1.15.5. Requirement_Type
non_functional

### 1.15.6. Priority


### 1.15.7. Original_Text


### 1.15.8. Change_Comments
False

### 1.15.9. Enhancement_Justification


## 1.16. 57
### 1.16.2. Section
5.2 Safety Requirements

### 1.16.3. Section_Id
REQ-NFR-003

### 1.16.4. Section_Requirement_Text
Enhancement Justification: User provided detailed clarification on the file modification workflow. Enhanced requirement to specify the state management for DICOM files, detailing the creation of a single working copy, the ability to save or discard changes, and the requirement to log these state-change actions in the audit trail. This ensures data integrity and provides a clear, non-destructive editing process.

--- Original Text ---
The system shall ensure that original DICOM files are never modified by viewing, printing, or annotation operations.
Modification actions, including metadata editing and anonymization, shall create new files.

--- Enhanced Specification ---
The system shall ensure that original DICOM files are never modified by viewing, printing, or annotation operations.
Modification actions, including metadata editing and anonymization, shall create a working copy of the original files, leaving the original files untouched.
There shall be only one 'original' version and one 'modified' (working copy) version of a study at any time; multiple versioning is not required.
A user can perform subsequent modifications on the existing working copy, overwriting it upon saving.
A user shall have the option to discard all modifications, which will delete the working copy and revert the view to the original files.
The system shall maintain a clear state indicator in the UI to show whether the user is viewing the original or modified version.
All state-changing actions on the working copy (creation, saving modifications, discarding modifications) shall be recorded in the audit trail, logging the user, timestamp, and action performed.

### 1.16.5. Requirement_Type
non_functional

### 1.16.6. Priority


### 1.16.7. Original_Text


### 1.16.8. Change_Comments
False

### 1.16.9. Enhancement_Justification


## 1.17. 60
### 1.17.2. Section
5.3 Security Requirements

### 1.17.3. Section_Id
REQ-NFR-004

### 1.17.4. Section_Requirement_Text
Enhancement Justification: The change to PostgreSQL (REQ-TEC-001) made the previous SQLite-specific file encryption requirements obsolete. This requirement is enhanced to specify appropriate security measures for a client-server RDBMS, including transport layer security for connections and strategies for data-at-rest encryption.

--- Original Text ---
User identity shall be managed locally.
User passwords shall be stored in the database using a strong, salted hashing algorithm (BCrypt).
A role-based access control (RBAC) model shall be strictly enforced as per the permissions matrix in Section 2.3.2.
The local SQLite database file shall be encrypted using SQLCipher with AES-256.
The database encryption key shall be protected using the Windows DPAPI.
Communication with the Odoo licensing API shall use TLS 1.2+.
SMTP communication shall be configured to use STARTTLS.
DICOM communication shall support DICOM TLS.
The "Full De-ID" anonymization profile shall ensure that outputs meet the criteria for HIPAA Safe Harbor.
<<$Change>> The application shall be developed following secure coding best practices to prevent common vulnerabilities in desktop applications.
Enhancement Justification: The "OWASP Top 10" is primarily for web applications. This requirement is rephrased to be more specific and relevant to a desktop application's security context, making it more actionable and testable.
Secure coding practices shall include but not be limited to input validation to prevent injection attacks, secure handling of credentials, and prevention of buffer overflows.

--- Enhanced Specification ---
User identity shall be managed locally.
User passwords shall be stored in the database using a strong, salted hashing algorithm (BCrypt).
A role-based access control (RBAC) model shall be strictly enforced as per the permissions matrix in Section 2.3.2.
All network connections from the application to the PostgreSQL database server shall be encrypted using TLS.
Data at rest within the PostgreSQL database shall be encrypted using Transparent Data Encryption (TDE) or equivalent column-level encryption mechanisms.
Database credentials shall be stored securely using the Windows Credential Manager and not in plaintext configuration files.
Communication with the Odoo licensing API shall use TLS 1.2+.
SMTP communication shall be configured to use STARTTLS.
DICOM communication shall support DICOM TLS.
The "Full De-ID" anonymization profile shall ensure that outputs meet the criteria for HIPAA Safe Harbor.
<<$Change>> The application shall be developed following secure coding best practices to prevent common vulnerabilities in desktop applications.
Enhancement Justification: The "OWASP Top 10" is primarily for web applications. This requirement is rephrased to be more specific and relevant to a desktop application's security context, making it more actionable and testable.
Secure coding practices shall include but not be limited to input validation to prevent injection attacks, secure handling of credentials, and prevention of buffer overflows.

### 1.17.5. Requirement_Type
non_functional

### 1.17.6. Priority


### 1.17.7. Original_Text


### 1.17.8. Change_Comments
False

### 1.17.9. Enhancement_Justification


## 1.18. 75
### 1.18.2. Section
5.4 Software Quality Attributes

### 1.18.3. Section_Id
REQ-NFR-005

### 1.18.4. Section_Requirement_Text
The application shall handle network and service interruptions gracefully, such as PACS connection loss or a stopped print service.
Generated PDFs shall be viewable in Adobe Acrobat Reader 9+ and other common PDF viewers.
The application shall be developed using a modular architecture (Modular Monolith) to separate concerns (UI, Core Logic, Data Access).
The MVVM design pattern shall be used in the WPF application.
The application UI shall support English (default) and Spanish for UI text in Phase 1.

### 1.18.5. Requirement_Type
non_functional

### 1.18.6. Priority


### 1.18.7. Original_Text


### 1.18.8. Change_Comments
False

### 1.18.9. Enhancement_Justification


## 1.19. 76
### 1.19.2. Section
7.1 Reports

### 1.19.3. Section_Id
REQ-REP-001

### 1.19.4. Section_Requirement_Text
The application shall provide a feature for Admin users to export the metadata modification audit trail as a CSV file.

### 1.19.5. Requirement_Type
reports_alerts

### 1.19.6. Priority


### 1.19.7. Original_Text


### 1.19.8. Change_Comments
False

### 1.19.9. Enhancement_Justification


## 1.20. 77
### 1.20.2. Section
7.2 Monitoring & Logging

### 1.20.3. Section_Id
REQ-REP-002

### 1.20.4. Section_Requirement_Text
The Serilog logging framework shall be configured to write to two destinations (sinks).
Logs shall be written to a local rolling text file stored in the user's `AppData\Logs` directory.
All PHI in local text file logs shall be masked.
Logs for critical errors and security-related events shall be written to the Windows Event Log.
Logs shall include timestamps, severity levels, and detailed context for events such as DICOM network operations, file I/O, and application errors.

### 1.20.5. Requirement_Type
reports_alerts

### 1.20.6. Priority


### 1.20.7. Original_Text


### 1.20.8. Change_Comments
False

### 1.20.9. Enhancement_Justification


## 1.21. 58
### 1.21.2. Section
2.5 Design and Implementation Constraints

### 1.21.3. Section_Id
REQ-TEC-001

### 1.21.4. Section_Requirement_Text
Enhancement Justification: User requested a change in the database technology from SQLite to PostgreSQL to better handle high-volume data insertion. The user also suggested RabbitMQ for guaranteed record insertion. Enhanced requirement to specify PostgreSQL as the local data store and to incorporate a message queue for decoupling network reception from database writing, improving system robustness and performance under load.

--- Original Text ---
The application shall be developed using the .NET 6+ framework, C# programming language, and Windows Presentation Foundation (WPF).
The application shall use the `fo-dicom` library for all DICOM operations.
The application shall use SQLite for the local data store.
The application shall integrate with the existing Odoo Web Portal APIs for license validation.
The application shall be developed with HIPAA compliance considerations for PHI handling, anonymization, and security.

--- Enhanced Specification ---
The application shall be developed using the .NET 6+ framework, C# programming language, and Windows Presentation Foundation (WPF).
The application shall use the `fo-dicom` library for all DICOM operations.
The application shall use PostgreSQL for the local data store to handle high-volume record insertion from modalities with large numbers of images per study.
To ensure guaranteed and high-throughput data insertion, the C-STORE SCP service shall publish received study metadata to a message queue (e.g., RabbitMQ), which will be consumed by a separate process responsible for database writing.
The application shall integrate with the existing Odoo Web Portal APIs for license validation.
The application shall be developed with HIPAA compliance considerations for PHI handling, anonymization, and security.

### 1.21.5. Requirement_Type
technology

### 1.21.6. Priority


### 1.21.7. Original_Text


### 1.21.8. Change_Comments
False

### 1.21.9. Enhancement_Justification


## 1.22. 61
### 1.22.2. Section
2.6 System Architecture

### 1.22.3. Section_Id
REQ-TEC-002

### 1.22.4. Section_Requirement_Text
Enhancement Justification: The introduction of a message queue (REQ-NFR-002, REQ-TEC-001) for multiple background tasks (printing, PDF generation, database writes) expands the role of the background service. This requirement is enhanced to accurately describe the architecture, where the service acts as a consumer for various asynchronous jobs, not just print management.

--- Original Text ---
The system shall be architected as a Modular Monolith with a Background Service.
The system shall be composed of a main WPF desktop application for user interaction and a separate background Windows Service for print queue management.
Communication between the main application and the background service shall be handled via Named Pipes.

--- Enhanced Specification ---
The system shall be architected as a Modular Monolith with a decoupled Background Worker Service.
The system shall be composed of a main WPF desktop application for user interaction and a separate background Windows Service that acts as a consumer for a message queue.
The background service shall be responsible for processing asynchronous, long-running tasks, including but not limited to database writes from C-STORE operations, PDF generation, and physical print job spooling.
Communication between the main application (producer) and the background service (consumer) shall be handled via a message queue (e.g., RabbitMQ).
Direct communication for immediate tasks, such as submitting a print job to the Windows Print API, shall still utilize Named Pipes between the main application and the DICOM Print Service.

### 1.22.5. Requirement_Type
technology

### 1.22.6. Priority


### 1.22.7. Original_Text


### 1.22.8. Change_Comments
False

### 1.22.9. Enhancement_Justification


## 1.23. 59
### 1.23.2. Section
2.7 Technology Stack

### 1.23.3. Section_Id
REQ-TEC-003

### 1.23.4. Section_Requirement_Text
Enhancement Justification: This requirement was updated for consistency with REQ-TEC-001, which changed the core database from SQLite to PostgreSQL and introduced RabbitMQ for message queuing. The technology stack must accurately reflect all core system components.

--- Original Text ---
The application shall be built on the .NET 6+ Framework.
The application shall be written in C# 10+.
The desktop UI shall be built with WPF (Windows Presentation Foundation).
The UI shall use the MVVM (Model-View-ViewModel) design pattern.
The Windows Service shall be built using Microsoft.Extensions.Hosting.WindowsServices.
Inter-Process Communication shall use Named Pipes.
The database shall be SQLite with SQLCipher for AES-256 encryption.
The ORM shall be Entity Framework Core 6+.
The DICOM library shall be fo-dicom.
Graphics rendering shall use the WPF Integrated Renderer (DirectX) and Vortice.Windows for high-performance viewing.
The logging framework shall be Serilog.
<<$Change>> The installation package shall be a signed MSIX Installer.
Enhancement Justification: MSIX provides a modern, reliable, and clean installation and uninstallation experience on Windows 10/11. It simplifies updates and enhances security through containerization.

--- Enhanced Specification ---
The application shall be built on the .NET 6+ Framework.
The application shall be written in C# 10+.
The desktop UI shall be built with WPF (Windows Presentation Foundation).
The UI shall use the MVVM (Model-View-ViewModel) design pattern.
The Windows Service shall be built using Microsoft.Extensions.Hosting.WindowsServices.
Inter-Process Communication between the UI and the Windows Service shall use Named Pipes for print job submission.
The database shall be PostgreSQL (version 13+).
The ORM shall be Entity Framework Core 6+.
The DICOM library shall be fo-dicom.
Asynchronous task and data processing shall be managed via a message queue (RabbitMQ).
Graphics rendering shall use the WPF Integrated Renderer (DirectX) and Vortice.Windows for high-performance viewing.
The logging framework shall be Serilog.
<<$Change>> The installation package shall be a signed MSIX Installer.
Enhancement Justification: MSIX provides a modern, reliable, and clean installation and uninstallation experience on Windows 10/11. It simplifies updates and enhances security through containerization.

### 1.23.5. Requirement_Type
technology

### 1.23.6. Priority


### 1.23.7. Original_Text


### 1.23.8. Change_Comments
False

### 1.23.9. Enhancement_Justification


## 1.24. 78
### 1.24.2. Section
2.3 User Roles and Permissions

### 1.24.3. Section_Id
REQ-USR-001

### 1.24.4. Section_Requirement_Text
The system shall enforce a role-based access control model with two distinct roles: Technician and Administrator.
Technician Role Permissions:
The system shall allow a Technician to view and search for studies.
The system shall allow a Technician to perform all viewing operations, including zoom, pan, and Window Width/Level (WW/WL).
The system shall allow a Technician to create and save annotations.
The system shall allow a Technician to use all print and export functions, including Print to Printer, PDF, and Email.
The system shall prevent a Technician from editing DICOM metadata.
The system shall prevent a Technician from changing system settings, including PACS, Users, and Storage Paths.
The system shall prevent a Technician from viewing audit trails.
Admin Role Permissions:
The system shall grant an Admin all permissions of the Technician role.
The system shall allow an Admin to edit DICOM metadata tags from a configurable list.
The system shall allow an Admin to manage user accounts and roles.
The system shall allow an Admin to configure all system, print, and network settings.
The system shall allow an Admin to view and export audit trails.

### 1.24.5. Requirement_Type
business

### 1.24.6. Priority


### 1.24.7. Original_Text


### 1.24.8. Change_Comments
False

### 1.24.9. Enhancement_Justification




---

