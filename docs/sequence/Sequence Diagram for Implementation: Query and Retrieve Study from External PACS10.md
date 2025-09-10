sequenceDiagram
    participant "Presentation (WPF)" as PresentationWPF
    actor "Application Services (Client)" as ApplicationServicesClient
    participant "Infrastructure (fo-dicom SCU)" as InfrastructurefodicomSCU
    participant "External PACS" as ExternalPACS
    participant "Background Service (SCP)" as BackgroundServiceSCP

    activate ApplicationServicesClient
    PresentationWPF->>ApplicationServicesClient: 1. 1. Execute Search Command: QueryRetrieveViewModel.SearchCommand.Execute(searchCriteria)
    ApplicationServicesClient-->>PresentationWPF: Returns Task for async operation
    activate InfrastructurefodicomSCU
    ApplicationServicesClient->>InfrastructurefodicomSCU: 2. 2. Query PACS: await _dicomScuService.FindAsync(pacsConfig, findRequest)
    InfrastructurefodicomSCU-->>ApplicationServicesClient: Task<List<DicomQueryResult>>
    activate ExternalPACS
    InfrastructurefodicomSCU->>ExternalPACS: 3. 3. Send DICOM C-FIND Request
    ExternalPACS-->>InfrastructurefodicomSCU: C-FIND-RSP (stream of responses)
    InfrastructurefodicomSCU->>ApplicationServicesClient: 4. 4. Return mapped query results
    ApplicationServicesClient-->>InfrastructurefodicomSCU: List<DicomQueryResult>
    ApplicationServicesClient->>PresentationWPF: 5. 5. Update UI with search results: ViewModel.SearchResults = results
    PresentationWPF->>ApplicationServicesClient: 6. 6. Execute Retrieve Command: ViewModel.RetrieveCommand.Execute(selectedStudies)
    ApplicationServicesClient-->>PresentationWPF: Returns Task for async operation
    ApplicationServicesClient->>InfrastructurefodicomSCU: 7. 7. Move Studies: await _dicomScuService.MoveAsync(pacsConfig, moveRequest)
    InfrastructurefodicomSCU-->>ApplicationServicesClient: Task<DicomMoveResult>
    InfrastructurefodicomSCU->>ExternalPACS: 8. 8. Send DICOM C-MOVE Request
    ExternalPACS-->>InfrastructurefodicomSCU: C-MOVE-RSP (indicates start of async transfer)
    activate BackgroundServiceSCP
    ExternalPACS->>BackgroundServiceSCP: 9. 9. Initiate DICOM C-STORE (async)
    BackgroundServiceSCP-->>ExternalPACS: C-STORE-RSP
    InfrastructurefodicomSCU->>ApplicationServicesClient: 10. 10. Report final status DicomMoveResult.Success
    ApplicationServicesClient-->>InfrastructurefodicomSCU: DicomMoveResult
    ApplicationServicesClient->>PresentationWPF: 11. 11. Update UI with completion status: ViewModel.Status = 'Retrieve Complete'

    note over ExternalPACS: The C-MOVE-RQ (Step 8) only initiates the transfer. The actual data transfer happens via one or m...
    note over BackgroundServiceSCP: The Background Service (SCP) handles the C-STORE-RQ via the standard ingestion sequence (SEQ-EVP-...
    note over InfrastructurefodicomSCU: Progress updates (e.g., number of completed sub-operations) can be received via pending C-MOVE-RS...

    deactivate BackgroundServiceSCP
    deactivate ExternalPACS
    deactivate InfrastructurefodicomSCU
    deactivate ApplicationServicesClient
