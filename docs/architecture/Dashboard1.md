# Specification

# 1. Dashboards

## 1.1. System Health Dashboard
Provides a real-time overview of the DICOM Management System's operational status, dependencies, and processing queues, as required by REQ-REP-002 for the Administration area.

### 1.1.3. Type
operational

### 1.1.4. Panels

- **Title:** Core Component Status  
**Type:** stat_group  
**Metrics:**
    
    - system.service.status
    - dependency.postgresql.status
    - dependency.rabbitmq.status
    
- **Title:** Dead-Letter Queue (DLQ)  
**Type:** stat  
**Metrics:**
    
    - rabbitmq.dlq.message_count
    
- **Title:** DICOM Storage Capacity  
**Type:** gauge  
**Metrics:**
    
    - storage.dicom.free_percent
    
- **Title:** Main Processing Queue Depth  
**Type:** stat  
**Metrics:**
    
    - rabbitmq.main_queue.message_count
    



---

