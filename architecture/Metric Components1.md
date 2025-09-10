# Specification

# 1. Telemetry And Metrics Analysis

- **System Overview:**
  
  - **Analysis Date:** 2025-06-13
  - **Technology Stack:**
    
    - .NET 8
    - C# 12
    - WPF
    - PostgreSQL 16
    - RabbitMQ
    - Serilog
    
  - **Monitoring Components:**
    
    - Structured Application Logging (Serilog)
    - Distributed Tracing via Correlation ID
    - System Health Probe Service
    - Critical Error Alerting
    
  - **Requirements:**
    
    - REQ-REP-002 (System Health Dashboard)
    - REQ-NFR-002 (Performance Requirements)
    - REQ-NFR-005 (Reliability)
    - REQ-INT-003 (Software Interfaces)
    
  - **Environment:** production
  
- **Standard System Metrics Selection:**
  
  - **Hardware Utilization Metrics:**
    
    - **Name:** system.cpu.usage_percent  
**Type:** gauge  
**Unit:** percent  
**Description:** CPU utilization for the host running the Windows Service.  
**Collection:**
    
    - **Interval:** 60s
    - **Method:** OS Performance Counters
    
**Thresholds:**
    
    - **Warning:** > 80%
    - **Critical:** > 95%
    
**Justification:** Provides baseline operational visibility into resource consumption of the background service host.  
    - **Name:** system.memory.usage_bytes  
**Type:** gauge  
**Unit:** bytes  
**Description:** Total memory utilization for the host running the Windows Service.  
**Collection:**
    
    - **Interval:** 60s
    - **Method:** OS Performance Counters
    
**Thresholds:**
    
    - **Warning:** > 85%
    - **Critical:** > 95%
    
**Justification:** Monitors for potential memory pressure on the server which could impact service performance.  
    - **Name:** storage.disk.free_percent  
**Type:** gauge  
**Unit:** percent  
**Description:** Percentage of free disk space on the volume used for DICOM storage.  
**Collection:**
    
    - **Interval:** 300s
    - **Method:** System Health Probe
    
**Thresholds:**
    
    - **Warning:** < 20%
    - **Critical:** < 10%
    
**Justification:** Directly required by REQ-REP-002 for the System Health Dashboard to prevent service failure due to full disks.  
    
  - **Runtime Metrics:**
    
    - **Name:** dotnet.gc.heap_size_bytes  
**Type:** gauge  
**Unit:** bytes  
**Description:** Total allocated memory in the .NET garbage collector heap for the Windows Service process.  
**Technology:** .NET  
**Collection:**
    
    - **Interval:** 60s
    - **Method:** .NET Diagnostics Counters
    
**Criticality:** medium  
    - **Name:** dotnet.threadpool.queue_length  
**Type:** gauge  
**Unit:** count  
**Description:** Number of work items currently queued for the .NET thread pool in the Windows Service.  
**Technology:** .NET  
**Collection:**
    
    - **Interval:** 30s
    - **Method:** .NET Diagnostics Counters
    
**Criticality:** high  
    
  - **Request Response Metrics:**
    
    
  - **Availability Metrics:**
    
    - **Name:** system.service.status  
**Type:** gauge  
**Unit:** boolean  
**Description:** Availability status of the background Windows Service. 1 for Running, 0 for Stopped.  
**Calculation:** Probe response from Named Pipe status check.  
**Sla Target:** 99.9%  
    
  - **Scalability Metrics:**
    
    
  
- **Application Specific Metrics Design:**
  
  - **Transaction Metrics:**
    
    - **Name:** dicom.ingestion.studies_total  
**Type:** counter  
**Unit:** studies  
**Description:** Total number of DICOM studies successfully received by the C-STORE SCP and queued for processing.  
**Business_Context:** Core data ingestion workflow.  
**Dimensions:**
    
    - sending_ae_title
    - modality
    
**Collection:**
    
    - **Interval:** real-time
    - **Method:** application instrumentation
    
**Aggregation:**
    
    - **Functions:**
      
      - sum
      - rate
      
    - **Window:** 1m
    
    - **Name:** jobs.processed_total  
**Type:** counter  
**Unit:** jobs  
**Description:** Total number of asynchronous jobs processed by the background service.  
**Business_Context:** Core printing and export workflow.  
**Dimensions:**
    
    - job_type
    - status
    
**Collection:**
    
    - **Interval:** real-time
    - **Method:** application instrumentation
    
**Aggregation:**
    
    - **Functions:**
      
      - sum
      - rate
      
    - **Window:** 1m
    
    
  - **Cache Performance Metrics:**
    
    
  - **External Dependency Metrics:**
    
    - **Name:** dependency.pacs.operation.duration_seconds  
**Type:** histogram  
**Unit:** seconds  
**Description:** Latency of DICOM network operations (C-ECHO, C-FIND, C-MOVE) to a remote PACS.  
**Dependency:** Remote PACS  
**Circuit Breaker Integration:** True  
**Sla:**
    
    - **Response Time:** 5s
    - **Availability:** 99.5%
    
    - **Name:** dependency.odoo_api.request.duration_seconds  
**Type:** histogram  
**Unit:** seconds  
**Description:** Latency of license validation requests to the Odoo API.  
**Dependency:** Odoo Licensing API  
**Circuit Breaker Integration:** False  
**Sla:**
    
    - **Response Time:** 2s
    - **Availability:** 99.8%
    
    
  - **Error Metrics:**
    
    - **Name:** jobs.errors_total  
**Type:** counter  
**Unit:** errors  
**Description:** Count of failed asynchronous jobs, categorized by type and reason.  
**Error Types:**
    
    - PrinterOffline
    - InvalidData
    - ProcessingTimeout
    
**Dimensions:**
    
    - job_type
    - failure_reason
    
**Alert Threshold:** > 0 in 5m  
    - **Name:** dependency.errors_total  
**Type:** counter  
**Unit:** errors  
**Description:** Count of errors when communicating with external dependencies.  
**Error Types:**
    
    - ConnectionRefused
    - Timeout
    - HttpStatus5xx
    
**Dimensions:**
    
    - dependency_name
    - operation
    
**Alert Threshold:** > 5 in 5m  
    
  - **Throughput And Latency Metrics:**
    
    - **Name:** jobs.processing.duration_seconds  
**Type:** histogram  
**Unit:** seconds  
**Description:** Time taken to complete an asynchronous job from the moment it is dequeued until completion.  
**Percentiles:**
    
    - p50
    - p90
    - p95
    - p99
    
**Buckets:**
    
    - 0.1
    - 0.5
    - 1
    - 5
    - 10
    - 30
    - 60
    
**Sla Targets:**
    
    - **P95:** < 10s for PDF generation
    - **P99:** < 30s for PDF generation
    
    
  
- **Business Kpi Identification:**
  
  - **Critical Business Metrics:**
    
    
  - **User Engagement Metrics:**
    
    
  - **Conversion Metrics:**
    
    
  - **Operational Efficiency Kpis:**
    
    - **Name:** rabbitmq.queue.depth  
**Type:** gauge  
**Unit:** messages  
**Description:** Number of unprocessed messages in a given queue. A direct measure of system backlog.  
**Calculation:** Direct query of RabbitMQ management API.  
**Benchmark Target:** < 100 for main queues, 0 for DLQ  
    - **Name:** dicom.ingestion.e2e_latency_seconds  
**Type:** histogram  
**Unit:** seconds  
**Description:** End-to-end latency from C-STORE receive to database write completion, tracked via Correlation ID.  
**Calculation:** Time difference between 'start' and 'end' log events with the same Correlation ID.  
**Benchmark Target:** p95 < 5s  
    
  - **Revenue And Cost Metrics:**
    
    
  - **Customer Satisfaction Indicators:**
    
    
  
- **Collection Interval Optimization:**
  
  - **Sampling Frequencies:**
    
    - **Metric Category:** Hardware Utilization  
**Interval:** 60s  
**Justification:** Sufficient for tracking long-term trends and major resource shifts.  
**Resource Impact:** low  
    - **Metric Category:** Application Transactions  
**Interval:** 15s  
**Justification:** Provides timely insight into core application performance and error rates without excessive overhead.  
**Resource Impact:** medium  
    - **Metric Category:** System Health Probes  
**Interval:** 60s  
**Justification:** Aligns with the REQ-REP-002 dashboard requirements for near real-time status.  
**Resource Impact:** low  
    
  - **High Frequency Metrics:**
    
    
  - **Cardinality Considerations:**
    
    - **Metric Name:** dicom.ingestion.studies_total  
**Estimated Cardinality:** medium-to-high  
**Dimension Strategy:** Use 'sending_ae_title' and 'modality' dimensions.  
**Mitigation Approach:** Ensure monitoring backend supports this cardinality. If costs are prohibitive, consider a more generic dimension like 'ae_title_group'.  
    
  - **Aggregation Periods:**
    
    - **Metric Type:** Latency Histograms  
**Periods:**
    
    - 1m
    - 5m
    - 1h
    
**Retention Strategy:** Raw data for short term, aggregated percentiles for long term.  
    
  - **Collection Methods:**
    
    - **Method:** real-time  
**Applicable Metrics:**
    
    - jobs.processed_total
    - jobs.errors_total
    
**Implementation:** In-process application instrumentation.  
**Performance:** High performance, low overhead.  
    
  
- **Aggregation Method Selection:**
  
  - **Statistical Aggregations:**
    
    - **Metric Name:** jobs.processed_total  
**Aggregation Functions:**
    
    - rate
    
**Windows:**
    
    - 1m
    - 5m
    
**Justification:** Calculating the rate of processed jobs is a key throughput indicator.  
    
  - **Histogram Requirements:**
    
    - **Metric Name:** jobs.processing.duration_seconds  
**Buckets:**
    
    - 0.1
    - 0.5
    - 1
    - 2.5
    - 5
    - 10
    
**Percentiles:**
    
    - p95
    - p99
    
**Accuracy:** High accuracy required to validate performance requirements from REQ-NFR-002.  
    
  - **Percentile Calculations:**
    
    - **Metric Name:** dicom.ingestion.e2e_latency_seconds  
**Percentiles:**
    
    - p90
    - p95
    - p99
    
**Algorithm:** t-digest  
**Accuracy:** Sufficient for operational monitoring.  
    
  - **Metric Types:**
    
    - **Name:** rabbitmq.queue.depth  
**Implementation:** gauge  
**Reasoning:** Represents a point-in-time value that can go up or down.  
**Resets Handling:** N/A  
    - **Name:** jobs.errors_total  
**Implementation:** counter  
**Reasoning:** Represents a monotonically increasing count of events.  
**Resets Handling:** Alerts should be based on the rate of increase, not the absolute value, to handle application restarts.  
    
  - **Dimensional Aggregation:**
    
    
  - **Derived Metrics:**
    
    - **Name:** error_rate_percent  
**Calculation:** (sum(rate(jobs.errors_total[5m])) / sum(rate(jobs.processed_total[5m]))) * 100  
**Source Metrics:**
    
    - jobs.errors_total
    - jobs.processed_total
    
**Update Frequency:** Every 60s  
    
  
- **Storage Requirements Planning:**
  
  - **Retention Periods:**
    
    - **Metric Type:** High-resolution (15s)  
**Retention Period:** 14 days  
**Justification:** Allows for detailed incident investigation in the immediate aftermath.  
**Compliance Requirement:** None  
    - **Metric Type:** Low-resolution (5m)  
**Retention Period:** 1 year  
**Justification:** Sufficient for long-term trend analysis and capacity planning.  
**Compliance Requirement:** None  
    
  - **Data Resolution:**
    
    - **Time Range:** 0-14 days  
**Resolution:** 15s  
**Query Performance:** High  
**Storage Optimization:** None  
    - **Time Range:** 14 days - 1 year  
**Resolution:** 5m  
**Query Performance:** Medium  
**Storage Optimization:** Downsampling  
    
  - **Downsampling Strategies:**
    
    - **Source Resolution:** 15s  
**Target Resolution:** 5m  
**Aggregation Method:** avg for gauges, sum for counters, merge for histograms  
**Trigger Condition:** After 14 days  
    
  - **Storage Performance:**
    
    - **Write Latency:** < 100ms
    - **Query Latency:** < 2s for standard dashboard queries
    - **Throughput Requirements:** 1000 DPM (data points per minute)
    - **Scalability Needs:** Must handle a 2x increase in metric volume.
    
  - **Query Optimization:**
    
    
  - **Cost Optimization:**
    
    - **Strategy:** Downsampling and Retention Policies  
**Implementation:** Configure time-series database to automatically downsample and expire old data.  
**Expected Savings:** Significant reduction in long-term storage costs.  
**Tradeoffs:** Loss of high-resolution data for historical analysis.  
    
  
- **Project Specific Metrics Config:**
  
  - **Standard Metrics:**
    
    
  - **Custom Metrics:**
    
    
  - **Dashboard Metrics:**
    
    
  - **Implementation Priority:**
    
    
  - **Risk Assessment:**
    
    
  - **Recommendations:**
    
    
  


---

