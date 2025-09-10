# Specification

# 1. Overview
## 1. Query and Retrieve Study from External PACS
A user performs a C-FIND query against a configured external PACS. Results are displayed, and the user selects studies to retrieve. The system then initiates a C-MOVE request, which commands the external PACS to send the selected studies to the application's C-STORE SCP. User feedback on progress is provided by monitoring the resulting C-STORE sub-operations.

### 1.1. Diagram Id
SEQ-INT-010

### 1.4. Type
IntegrationFlow

### 1.5. Purpose
To enable users to search for and import studies from a central PACS into the local system for viewing, printing, or other operations.

### 1.6. Complexity
High

### 1.7. Priority
High

### 1.8. Frequency
OnDemand

### 1.9. Participants

- REPO-09-PRE
- REPO-08-APC
- REPO-11-INF
- EXT-PACS
- REPO-02-SVC

### 1.10. Key Interactions

- User enters search criteria (Patient ID, Study Date, etc.).
- The client application initiates a DICOM C-FIND request to the PACS.
- The PACS responds with a list of matching studies.
- The user selects one or more studies and clicks 'Retrieve'.
- The client application initiates a DICOM C-MOVE request to the PACS, specifying the local SCP's AE Title as the destination.
- The PACS initiates one or more C-STORE associations to the application's SCP.
- The study is ingested via the standard ingestion sequence (SEQ-EVP-002).

### 1.11. Triggers

- User performs a search and initiates a retrieve operation from the Query/Retrieve UI (FR-3.2.1).

### 1.12. Outcomes

- The selected studies are successfully transferred from the PACS and stored locally.
- The user receives feedback on the progress and success or failure of the retrieval.

### 1.13. Business Rules

- The system must act as a C-FIND and C-MOVE SCU.

### 1.14. Error Scenarios

- PACS connection is refused.
- C-FIND query returns no results.
- C-MOVE fails (e.g., 'Destination AE Title unknown').

### 1.15. Integration Points

- DICOM Network Protocol (C-FIND, C-MOVE)
- External PACS Server


---

# 2. Details
## 2. Implementation: Query and Retrieve Study from External PACS
Technical implementation sequence for a user-initiated DICOM C-FIND query and subsequent C-MOVE retrieval from an external PACS. This diagram details the protocol-level interactions, asynchronous handoffs, and error handling strategies required for robust integration. It highlights the adapter pattern for DICOM communication and the circuit breaker pattern for resilience against external system failures.

### 2.1. Diagram Id
SEQ-INT-010-IMPL

### 2.4. Participants

- **Repository Id:** REPO-09-PRE  
**Display Name:** Presentation (WPF)  
**Type:** UI Layer  
**Technology:** WPF, MVVM  
**Order:** 1  
**Style:**
    
    - **Shape:** actor
    - **Color:** #4CAF50
    - **Stereotype:** User Interface
    
- **Repository Id:** REPO-08-APC  
**Display Name:** Application Services (Client)  
**Type:** Application Layer  
**Technology:** .NET 8 Service  
**Order:** 2  
**Style:**
    
    - **Shape:** rectangle
    - **Color:** #2196F3
    - **Stereotype:** Orchestrator
    
- **Repository Id:** REPO-11-INF  
**Display Name:** Infrastructure (fo-dicom SCU)  
**Type:** Infrastructure Layer  
**Technology:** fo-dicom, .NET 8  
**Order:** 3  
**Style:**
    
    - **Shape:** rectangle
    - **Color:** #FFC107
    - **Stereotype:** DICOM Adapter
    
- **Repository Id:** EXT-PACS  
**Display Name:** External PACS  
**Type:** External System  
**Technology:** DICOM  
**Order:** 4  
**Style:**
    
    - **Shape:** database
    - **Color:** #9E9E9E
    - **Stereotype:** External Dependency
    
- **Repository Id:** REPO-02-SVC  
**Display Name:** Background Service (SCP)  
**Type:** Service Layer  
**Technology:** .NET 8 Windows Service  
**Order:** 5  
**Style:**
    
    - **Shape:** rectangle
    - **Color:** #E91E63
    - **Stereotype:** DICOM SCP
    

### 2.5. Interactions

- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-08-APC  
**Message:** 1. Execute Search Command: `QueryRetrieveViewModel.SearchCommand.Execute(searchCriteria)`  
**Sequence Number:** 1  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** Returns Task for async operation  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Method Call
    - **Method:** Execute
    - **Parameters:** `DicomSearchCriteria` DTO containing PatientID, StudyDate, Modality, etc.
    - **Authentication:** User Session Token
    - **Error Handling:** Catch `ArgumentNullException` if criteria are invalid.
    - **Performance:** < 50ms
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-11-INF  
**Message:** 2. Query PACS: `await _dicomScuService.FindAsync(pacsConfig, findRequest)`  
**Sequence Number:** 2  
**Type:** Service Call  
**Is Synchronous:** True  
**Return Message:** Task<List<DicomQueryResult>>  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Method Call
    - **Method:** FindAsync
    - **Parameters:** `PacsConfiguration` object, `DicomCFindRequest` object
    - **Authentication:** N/A
    - **Error Handling:** Propagate `DicomIntegrationException` from infrastructure layer.
    - **Performance:** Depends on network and external PACS
    
- **Source Id:** REPO-11-INF  
**Target Id:** EXT-PACS  
**Message:** 3. Send DICOM C-FIND Request  
**Sequence Number:** 3  
**Type:** Network Request  
**Is Synchronous:** True  
**Return Message:** C-FIND-RSP (stream of responses)  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** DICOM
    - **Method:** C-FIND-RQ
    - **Parameters:** `DicomDataset` with query-level keys (e.g., `(0008,0052)`=STUDY) and matching keys (e.g., `(0010,0010)`=PatientName). Priority is NORMAL.
    - **Authentication:** AE Title based. Optional DICOM TLS.
    - **Error Handling:** Circuit Breaker pattern. Catch `DicomAssociationRejectedException`, `DicomNetworkException`. Handle DICOM error status codes in responses.
    - **Performance:** SLA defined by external PACS. Network latency is a key factor.
    
**Nested Interactions:**
    
    
- **Source Id:** REPO-11-INF  
**Target Id:** REPO-08-APC  
**Message:** 4. Return mapped query results  
**Sequence Number:** 4  
**Type:** Return Value  
**Is Synchronous:** True  
**Return Message:** `List<DicomQueryResult>`  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** return
    - **Parameters:** List of DTOs, each mapping a C-FIND-RSP `DicomDataset` to a structured result.
    - **Authentication:** N/A
    - **Error Handling:** If list is empty, handle as 'No Results Found' scenario.
    - **Performance:** < 10ms for mapping
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-09-PRE  
**Message:** 5. Update UI with search results: `ViewModel.SearchResults = results`  
**Sequence Number:** 5  
**Type:** UI Update  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** MVVM Data Binding
    - **Method:** Property Set
    - **Parameters:** `ObservableCollection<DicomQueryResult>`
    - **Authentication:** N/A
    - **Error Handling:** UI gracefully handles empty collection.
    - **Performance:** UI rendering performance depends on list size. Virtualization should be used.
    
- **Source Id:** REPO-09-PRE  
**Target Id:** REPO-08-APC  
**Message:** 6. Execute Retrieve Command: `ViewModel.RetrieveCommand.Execute(selectedStudies)`  
**Sequence Number:** 6  
**Type:** Method Call  
**Is Synchronous:** True  
**Return Message:** Returns Task for async operation  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Method Call
    - **Method:** Execute
    - **Parameters:** `List<DicomQueryResult>` containing UIDs of studies to retrieve.
    - **Authentication:** User Session Token
    - **Error Handling:** Validate that at least one study is selected.
    - **Performance:** < 50ms
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-11-INF  
**Message:** 7. Move Studies: `await _dicomScuService.MoveAsync(pacsConfig, moveRequest)`  
**Sequence Number:** 7  
**Type:** Service Call  
**Is Synchronous:** True  
**Return Message:** Task<DicomMoveResult>  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** In-Process Method Call
    - **Method:** MoveAsync
    - **Parameters:** `PacsConfiguration`, `DicomCMoveRequest` specifying destination AE Title.
    - **Authentication:** N/A
    - **Error Handling:** Propagate exceptions. Handle progress update events raised by the service.
    - **Performance:** Depends on external PACS and network
    
- **Source Id:** REPO-11-INF  
**Target Id:** EXT-PACS  
**Message:** 8. Send DICOM C-MOVE Request  
**Sequence Number:** 8  
**Type:** Network Request  
**Is Synchronous:** True  
**Return Message:** C-MOVE-RSP (indicates start of async transfer)  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** DICOM
    - **Method:** C-MOVE-RQ
    - **Parameters:** `DicomDataset` with query-level key `(0008,0052)`=STUDY, `(0008,0054)`=Destination AE Title, and UIDs of studies to move.
    - **Authentication:** AE Title based. Optional DICOM TLS.
    - **Error Handling:** Handle error statuses like 'A801: Move Destination Unknown'. Catch network exceptions.
    - **Performance:** Initial response should be fast. The actual data transfer is asynchronous.
    
- **Source Id:** EXT-PACS  
**Target Id:** REPO-02-SVC  
**Message:** 9. Initiate DICOM C-STORE (async)  
**Sequence Number:** 9  
**Type:** Network Request  
**Is Synchronous:** True  
**Return Message:** C-STORE-RSP  
**Has Return:** True  
**Is Activation:** True  
**Technical Details:**
    
    - **Protocol:** DICOM
    - **Method:** C-STORE-RQ
    - **Parameters:** Full `DicomDataset` including pixel data for a single SOP Instance.
    - **Authentication:** AE Title based (validates source AET of PACS).
    - **Error Handling:** Service responds with error status if unable to store file (e.g., out of disk space).
    - **Performance:** High throughput required to handle many incoming images.
    
- **Source Id:** REPO-11-INF  
**Target Id:** REPO-08-APC  
**Message:** 10. Report final status `DicomMoveResult.Success`  
**Sequence Number:** 10  
**Type:** Return Value  
**Is Synchronous:** True  
**Return Message:** `DicomMoveResult`  
**Has Return:** True  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** In-Process
    - **Method:** return
    - **Parameters:** A result object indicating success or failure, with details from the final C-MOVE-RSP.
    - **Authentication:** N/A
    - **Error Handling:** Translate DICOM error codes into application-level exceptions or error DTOs.
    - **Performance:** < 1ms
    
- **Source Id:** REPO-08-APC  
**Target Id:** REPO-09-PRE  
**Message:** 11. Update UI with completion status: `ViewModel.Status = 'Retrieve Complete'`  
**Sequence Number:** 11  
**Type:** UI Update  
**Is Synchronous:** False  
**Return Message:**   
**Has Return:** False  
**Is Activation:** False  
**Technical Details:**
    
    - **Protocol:** MVVM Data Binding / Event Aggregator
    - **Method:** Property Set
    - **Parameters:** Status string or enum.
    - **Authentication:** N/A
    - **Error Handling:** Display non-blocking toast notification for success or modal dialog for failure.
    - **Performance:** < 50ms
    

### 2.6. Notes

- **Content:** The C-MOVE-RQ (Step 8) only initiates the transfer. The actual data transfer happens via one or more separate, asynchronous C-STORE-RQ associations (Step 9) initiated by the External PACS to our Background Service.  
**Position:** bottom  
**Participant Id:** EXT-PACS  
**Sequence Number:** 8  
- **Content:** The Background Service (SCP) handles the C-STORE-RQ via the standard ingestion sequence (SEQ-EVP-002), which involves saving the file, parsing metadata, and queueing a message for database insertion. This is not detailed here to maintain focus on the integration flow.  
**Position:** bottom  
**Participant Id:** REPO-02-SVC  
**Sequence Number:** 9  
- **Content:** Progress updates (e.g., number of completed sub-operations) can be received via pending C-MOVE-RSP messages. The Infrastructure layer should raise events that the Application layer subscribes to, allowing for real-time progress feedback to the user's UI.  
**Position:** bottom  
**Participant Id:** REPO-11-INF  
**Sequence Number:** 8  

### 2.7. Implementation Guidance

- **Security Requirements:** PACS configurations, including AE Title, IP, and Port, must be managed securely and be configurable only by an Administrator. Support for DICOM TLS should be implemented and configurable for each PACS connection to encrypt data in transit.
- **Performance Targets:** All DICOM network calls from the client must be non-blocking (`async`/`await`) to keep the UI responsive. The C-STORE SCP must be capable of handling multiple simultaneous associations from the PACS without significant performance degradation.
- **Error Handling Strategy:** A Circuit Breaker pattern should be implemented in the `REPO-11-INF` layer for connections to external PACS. If a PACS is unavailable after N retries, the circuit should open for a configured duration. Specific DICOM error statuses (e.g., `A801: Move Destination Unknown`, `Cxxx: Cannot Understand`) must be caught and translated into user-friendly error messages.
- **Testing Considerations:** Integration testing is critical. A mock DICOM server (e.g., using `fo-dicom`'s `DicomServer`) must be created to simulate an external PACS. Test scenarios should cover successful C-FIND/C-MOVE, empty C-FIND results, and various C-MOVE failure statuses.
- **Monitoring Requirements:** Log all DICOM association requests and their outcomes (success/failure). Monitor the latency of C-FIND and C-MOVE operations against each configured PACS. Create alerts for high rates of DICOM network exceptions or error statuses, which could indicate a misconfigured or unavailable PACS.
- **Deployment Considerations:** The application must be configured with its own unique AE Title and listening port for the C-STORE SCP. These, along with external PACS configurations, must be part of the administrative settings of the application.


---

