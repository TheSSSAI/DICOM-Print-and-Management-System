# Specification

# 1. Id
79


---

# 2. Section
DICOM Print and Management System Requirement Specification


---

# 3. Section_Id
SRS-001


---

# 4. Section_Requirement_Text
# CORE FEATURE 
## DICOM Printing
*   Windows Print Service Integration:
    *   Direct integration with Windows Print API for commercial inkjet/laser printers (non-DICOM).
    *   Auto-detect system-installed printers and fetch driver-supported resolutions (e.g., 300dpi_1200dpi).
    *   The DICOM PRINT Service to be created as a windows service with proper user rights.
*   Print Layout Configuration:
    *   Page Templates:
        *   Predefined layouts: Single image (A4/A3), 2x2 grid (A4), 1+3 comparison (A3), Modality-specific presets (e.g., 1x1 for X-ray, 4x4 for CT slices).
        *   Customizable margins (5_20mm), orientation (Portrait/Landscape), and image scaling (Fit-to-Page, Actual Size).
    *   Multi-Image Fitting:
        *   Auto-arrange images based on modality (e.g., stack ultrasound frames in grid, display MRI series in rows).
        *   Manual drag-and-drop rearrangement in the print preview UI.
    *   Branding & De-identification:
        *   User Logo: Upload custom logo (PNG/JPEG) with positioning (Header/Footer/Custom Coordinates) and scaling (10%_50% of page width).
        *   Templated Overlay:
            *   Pixel-based overlay masks for de-identification (e.g., blackout patient name/date in DICOM image pixels).
            *   Prebuilt templates (e.g., Chest X-Ray De-ID) with configurable regions (rectangular/polygonal).
        *   Custom Text: Add free-text fields (e.g., "Confidential", institution name) with font/size/color customization. Variables (e.g., [PatientID], [Date]) supported.
*   Print Output Formats:
    *   Send to printer
        *   Print using the set printer to the windows machine. 
        *   Need printer configuration
    *   Print to PDF:
        *   Generate PDF/A-3 compliant files with embedded DICOM metadata (optional).
        *   Password-protect PDFs (AES-256) for PHI security.
    *   Print to Email:
        *   Directly attach PDF/images to email via SMTP (configure SMTP server in settings).
        *   Prefilled email templates with variables (e.g., [PatientName], [StudyDate]).

## DICOM PACS Integration
*   Storage & Retrieval:
    *   Auto-routing of incoming studies to predefined local folders.
    *   Query/Retrieve by Study Date, Modality, or Patient ID.
*   Configuration:
    *   Add/remove PACS servers via JSON configuration file.

## DICOM Security
*   Data Protection:
    *   Encrypt local database using AES-256.
    *   Mask PHI (Protected Health Information) in logs.
*   Access Control:
    *   Role-based permissions (e.g., Technician: View/Print, Admin: Edit Metadata).
    *   Session timeout after 15 minutes of inactivity.

## DICOM Metadata Viewing/Editing
*   Viewer:
    *   A very basic viewer, with only minimum features essential for the requirement and image preview.
    *   Group-wise metadata display (Patient, Study, Series, Equipment).
    *   Search metadata for specific tags (e.g., (0010,0010) = Patient Name).
*   Editing:
    *   Restricted to non-critical tags (e.g., (0008,103E) = Series Description).
    *   Audit trail for modified tags (user, timestamp).

# Supporting Functional Requirements
## 1.1 DICOM Image Viewing
*   Support rendering of DICOM files (monochrome and color) with:
    *   Multi-frame support (e.g., CT/MRI series, ultrasound sequences).
    *   Modality-specific rendering protocols (e.g., windowing for CT, inversion for X-rays).
    *   Display of overlay annotations and embedded measurements.
*   Performance Requirements:
    *   <<$Change>>Load images up to 500MB in size within 3 seconds on standard hardware. For images exceeding 500MB (up to 2GB), the system should initiate display of the first available frame or a low-resolution preview within 5 seconds, with full data loading and rendering completing progressively.<<$Change>>
    *   Enhancement Justification: The original requirement of "Load images _2GB in size within _3 seconds on standard hardware" was deemed infeasible due to the extreme data volume for typical disk I/O, parsing, and rendering on a broad definition of 'standard hardware'. The revised requirement sets a realistic performance target for common large studies (up to 500MB) and provides a progressive loading strategy for exceptionally large files (up to 2GB), ensuring a responsive user experience without demanding unrealistic hardware performance.
    *   Support GPU-accelerated rendering (DirectX/Vulkan optional).
*   UI Requirements:
    *   Side-by-side comparison of images (sync scroll/zoom).
    *   Thumbnail grid for series navigation.

## 1.2 DICOM File Management
*   Import/Export:
    *   Drag-and-drop support from local folders, CDs, USB drives, and network shares.
    *   Export to JPEG/PNG (non-DICOM) with configurable resolution.
*   Organization:
    *   Hierarchical storage by Patient ID _ Study UID _ Series UID.
    *   Tag-based search (Patient Name, Study Date, Modality).
*   Validation:
    *   Verify DICOM compliance during import (valid header, required tags).

## 1.3 DICOM Network Communication
*   Protocol Support:
    *   DICOM C-ECHO, C-STORE (SCU/SCP), C-FIND, and C-MOVE.
    *   Configurable AE Titles, ports, and transfer syntaxes (e.g., JPEG Lossless, RLE).
*   Error Handling:
    *   Retry failed transfers (3 attempts) with logging.
    *   Auto-reject non-DICOM files received via network.

## 1.4 DICOM Data Manipulation
*   Basic Tools:
    *   Zoom (10%_800%), rotation (0__360_), pan, flip.
    *   Window Level (WW/WL) adjustment with presets (e.g., Lung, Bone).
*   Annotations:
    *   Non-destructive annotations (arrows, text, ROI) stored in DICOM GSPS.
    *   Measurement tools (length, angle, area) with DICOM-compliant units (mm/cm).

## 1.9 DICOM Compliance
*   Conformance Statement:
    *   Include DICOM Conformance Statement (DICOM PS3.x).
    *   Validate compliance via DVTk or similar tool during QA.

## 1.10 Workflow Optimization
*   Hanging Protocols:
    *   Customizable layouts (e.g., 2x2, 1+3) saved per user.
*   Shortcuts:
    *   Keyboard shortcuts for common actions (e.g., Ctrl+P = Print, Ctrl+W = WW/WL).

## 1.11 User Interface (UI)
*   Layout:
    *   Three main tabs: Incoming/Print Queue, Local Storage, Query/Retrieve.
    *   Dark/Light theme toggle.
*   Accessibility:
    *   High-contrast mode for low-vision users (WCAG 2.1 compliant).

## 1.12 Settings Page
*   System Configuration:
    *   Default storage path, auto-purge rules (delete studies >365 days old).
    *   Network proxy settings for DICOM communication.
*   User Preferences:
    *   Customize annotation colors, default windowing presets.
*   Print Configuration:
    *   Default page size (A4/A3), default printer selection.
    *   Manage logo upload, overlay templates, and email/SMTP settings.
*   Anonymization Profiles:
    *   Create/edit profiles for metadata and pixel anonymization.

## 1.13 Anonymization
*   DICOM Metadata Anonymization:
    *   Remove/replace PHI tags (e.g., Patient Name (0010,0010), Birth Date (0010,0030)) using:
        *   Manual Mode: Select tags to anonymize.
        *   Preset Profiles: Basic De-ID (removes 10 critical tags), Full De-ID (removes 30+ tags as per HIPAA).
    *   Pseudonymization: Replace original Patient ID with hashed/randomized IDs (configurable algorithm).
*   Pixel Anonymization:
    *   Overlay shapes/text on image pixels to mask identifiers (e.g., burn patient name into image).
    *   Save anonymization templates for reuse (JSON format).

# Non-Functional Requirements
## 2.1 Performance
*   Launch application _5 seconds on HDD, _3 seconds on SSD.
*   Handle concurrent DICOM transfers (10+ simultaneous C-STORE operations).

## 2.2 Compatibility
*   Printers: Compatible with all Windows 10/11-supported inkjet/laser printers.
*   PDF Readers: Generated PDFs viewable in Adobe Acrobat Reader 9+.

## 2.4 Security
*   Anonymization Compliance: Ensure anonymized outputs meet HIPAA Safe Harbor criteria.
*   Email Encryption: TLS for SMTP; PHI-containing emails require password-protected PDF attachments.
*   TLS 1.2+ for web portal API calls (license validation).
*   OWASP Top 10 compliance for input validation.

## 2.5 Performance
*   PDF generation time _10 seconds for 50-page documents (A4, 300dpi).

## 2.6 Localization
*   Support English (default) and Spanish (UI only, Phase 1).

# Assumptions & Dependencies
## Assumptions:
*   Web portal APIs for license management are already available (Odoo).
*   PACS servers are DICOM-compliant and preconfigured by the client.
## Dependencies:
*   .NET 6+ Desktop Runtime.
*   fo-dicom library for DICOM operations.
*   SQLite for local storage management.

# Out-of-Scope
*   Development of Odoo web portal (only API integration for licensing).
*   AI-based image analysis or 3D reconstruction.
*   Integration with EHR systems (Epic, Cerner).
*   Development of custom printer drivers.
*   Advanced DICOM-compliant print pipelines (e.g., GSPS for non-Windows printers).


---

# 5. Requirement_Type
other


---

# 6. Priority
False


---

# 7. Original_Text
False


---

# 8. Change_Comments
False


---

# 9. Enhancement_Justification
False


---

