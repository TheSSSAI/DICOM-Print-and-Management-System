# PROJECT DOCUMENTATION
---
## [Detail Requirement Analysis](https://github.com/TheSSSAI/DICOM-Print-and-Management-System/tree/main/docs/requirements)


## [User Stories](https://github.com/TheSSSAI/DICOM-Print-and-Management-System/tree/main/docs/user-story)


## [Architecture](https://github.com/TheSSSAI/DICOM-Print-and-Management-System/tree/main/docs/architecture)


## [Database](https://github.com/TheSSSAI/DICOM-Print-and-Management-System/tree/main/docs/database)


## [Sequence Diagram](https://github.com/TheSSSAI/DICOM-Print-and-Management-System/tree/main/docs/sequence)


## [UI UX Mockups](https://github.com/TheSSSAI/DICOM-Print-and-Management-System/tree/main/docs/ui-mockups)

[{"id":8,"display_name":"DMPS.Client.Application"},{"id":9,"display_name":"DMPS.Client.Presentation"},{"id":3,"display_name":"DMPS.CrossCutting.Logging"},{"id":4,"display_name":"DMPS.CrossCutting.Security"},{"id":2,"display_name":"DMPS.Data.Access"},{"id":5,"display_name":"DMPS.Infrastructure.Communication"},{"id":6,"display_name":"DMPS.Infrastructure.Dicom"},{"id":7,"display_name":"DMPS.Infrastructure.IO"},{"id":10,"display_name":"DMPS.Service.Worker"},{"id":1,"display_name":"DMPS.Shared.Core"}]

---

#REPOSITORY DOCUMENTS
 [## Repository : DMPS.Client.Application](https://github.com/TheSSSAI/DICOM-Print-and-Management-SystemDMPS.Client.Application/docs)


,[## Repository : DMPS.Client.Presentation](https://github.com/TheSSSAI/DICOM-Print-and-Management-SystemDMPS.Client.Presentation/docs)


,[## Repository : DMPS.CrossCutting.Logging](https://github.com/TheSSSAI/DICOM-Print-and-Management-SystemDMPS.CrossCutting.Logging/docs)


,[## Repository : DMPS.CrossCutting.Security](https://github.com/TheSSSAI/DICOM-Print-and-Management-SystemDMPS.CrossCutting.Security/docs)


,[## Repository : DMPS.Data.Access](https://github.com/TheSSSAI/DICOM-Print-and-Management-SystemDMPS.Data.Access/docs)


,[## Repository : DMPS.Infrastructure.Communication](https://github.com/TheSSSAI/DICOM-Print-and-Management-SystemDMPS.Infrastructure.Communication/docs)


,[## Repository : DMPS.Infrastructure.Dicom](https://github.com/TheSSSAI/DICOM-Print-and-Management-SystemDMPS.Infrastructure.Dicom/docs)


,[## Repository : DMPS.Infrastructure.IO](https://github.com/TheSSSAI/DICOM-Print-and-Management-SystemDMPS.Infrastructure.IO/docs)


,[## Repository : DMPS.Service.Worker](https://github.com/TheSSSAI/DICOM-Print-and-Management-SystemDMPS.Service.Worker/docs)


,[## Repository : DMPS.Shared.Core](https://github.com/TheSSSAI/DICOM-Print-and-Management-SystemDMPS.Shared.Core/docs)



---

# Specification

# 1. Id
80


---

# 2. Section
DICOM Print and Management System Summary


---

# 3. Section_Id
SUMMARY-001


---

# 4. Section_Requirement_Text
The DICOM Print and Management System is designed to provide comprehensive DICOM printing, PACS integration, security, and image management capabilities. Key features include:

**DICOM Printing:**
*   **Windows Print Service Integration:** Direct integration with Windows Print API for commercial inkjet/laser printers, auto-detection of installed printers, and a dedicated DICOM PRINT Windows service with proper user rights.
*   **Print Layout Configuration:** Predefined page templates (e.g., Single image, 2x2 grid, 1+3 comparison, modality-specific presets), customizable margins, orientation, and image scaling (Fit-to-Page, Actual Size). Multi-image fitting with auto-arrangement by modality and manual drag-and-drop rearrangement in print preview.
*   **Branding & De-identification:** User logo upload with positioning and scaling, pixel-based templated overlay masks for de-identification (e.g., blackout patient name/date), prebuilt templates, and custom text fields with variables.
*   **Print Output Formats:** Direct printing to configured Windows printers, generation of PDF/A-3 compliant files with optional embedded DICOM metadata and AES-256 password protection, and direct email attachment of PDFs/images via SMTP with prefilled templates.

**DICOM PACS Integration:**
*   **Storage & Retrieval:** Auto-routing of incoming studies to local folders, and Query/Retrieve functionality by Study Date, Modality, or Patient ID.
*   **Configuration:** Management of PACS servers via a JSON configuration file.

**DICOM Security:**
*   **Data Protection:** AES-256 encryption for the local database and masking of PHI in logs.
*   **Access Control:** Role-based permissions (e.g., Technician: View/Print, Admin: Edit Metadata) and a session timeout after 15 minutes of inactivity.

**DICOM Metadata Viewing/Editing:**
*   **Viewer:** A basic viewer for image preview, group-wise metadata display (Patient, Study, Series, Equipment), and search functionality for specific tags.
*   **Editing:** Restricted editing of non-critical tags with an audit trail for modifications.

**Supporting Functional Requirements:**
*   **DICOM Image Viewing:** Support for monochrome and color DICOM rendering, multi-frame support, modality-specific rendering protocols (windowing, inversion), and display of overlay annotations/measurements. <<$Change>>Performance targets include loading images up to 500MB within 3 seconds on standard hardware, and for larger images (up to 2GB), initiating display of the first frame or a low-resolution preview within 5 seconds with progressive loading.<<$Change>> Optional GPU-accelerated rendering. UI features include side-by-side comparison and a thumbnail grid.
*   **DICOM File Management:** Drag-and-drop import from various sources, export to JPEG/PNG, hierarchical storage, tag-based search, and DICOM compliance validation during import.
*   **DICOM Network Communication:** Support for DICOM C-ECHO, C-STORE (SCU/SCP), C-FIND, C-MOVE with configurable AE Titles, ports, and transfer syntaxes. Includes error handling with retries and logging, and auto-rejection of non-DICOM files.
*   **DICOM Data Manipulation:** Basic tools like zoom, rotation, pan, flip, and Window Level adjustment with presets. Non-destructive annotations (arrows, text, ROI) stored in DICOM GSPS, and measurement tools (length, angle, area).
*   **DICOM Compliance:** Provision of a DICOM Conformance Statement and validation via DVTk.
*   **Workflow Optimization:** Customizable hanging protocols and keyboard shortcuts for common actions.
*   **User Interface (UI):** Three main tabs (Incoming/Print Queue, Local Storage, Query/Retrieve), Dark/Light theme toggle, and high-contrast mode for accessibility (WCAG 2.1 compliant).
*   **Settings Page:** Configuration for system (storage path, auto-purge, network proxy), user preferences (annotation colors, windowing presets), print settings (page size, printer, logo, overlays, email), and anonymization profiles.
*   **Anonymization:** DICOM metadata anonymization (manual/preset profiles, pseudonymization) and pixel anonymization (overlay shapes/text, save templates).

**Non-Functional Requirements:**
*   **Performance:** Application launch within 5 seconds on HDD and 3 seconds on SSD. Handling of 10+ concurrent DICOM transfers. PDF generation within 10 seconds for 50-page documents.
*   **Compatibility:** Compatible with Windows 10/11 printers and Adobe Acrobat Reader 9+ for generated PDFs.
*   **Security:** Anonymized outputs meet HIPAA Safe Harbor, TLS for SMTP and web portal API calls, password-protected PDFs for PHI, and OWASP Top 10 compliance.
*   **Localization:** Support for English (default) and Spanish (UI only, Phase 1).

**Assumptions & Dependencies:**
*   Assumes availability of Odoo web portal APIs for license management and preconfigured DICOM-compliant PACS servers.
*   Dependencies include .NET 6+ Desktop Runtime, fo-dicom library, and SQLite for local storage.

**Out-of-Scope:** Development of Odoo web portal, AI-based image analysis, 3D reconstruction, EHR integration, custom printer drivers, and advanced DICOM-compliant print pipelines.


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

