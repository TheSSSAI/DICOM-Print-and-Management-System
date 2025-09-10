# Specification

# 1. Cicd Pipeline Design

- **System Overview:**
  
  - **Analysis Date:** 2025-06-13
  - **Technology Stack:**
    
    - .NET 8
    - WPF
    - Windows Service
    - PostgreSQL
    - RabbitMQ
    - xUnit
    - MSIX
    
  - **Architecture Patterns:**
    
    - Client-Server
    - Message Queue
    - Repository Pattern
    
  - **Pipeline Objectives:**
    
    - Automate build, test, and packaging of the .NET solution.
    - Enforce code quality and security standards through automated scanning.
    - Orchestrate deployments across Training, Pilot, and Production environments.
    - Ensure traceable and repeatable delivery with versioned artifacts.
    
  - **Key Requirements:**
    
    - REQ-TEC-003: Must use .NET 8, xUnit, Roslyn Analyzers, and produce a signed MSIX installer.
    - REQ-NFR-004: Must include OWASP Dependency-Check for vulnerability scanning.
    - REQ-8.1: Must support a phased rollout (Pilot -> Production).
    - REQ-8.2: A separate Data Migration Utility must be built and versioned.
    - REQ-8.3: Must support deployment to a dedicated Training environment.
    
  
- **Build And Test Automation Requirements:**
  
  - **Build Targets:**
    
    - **Target:** Main Application Solution (.sln)  
**Components:**
    
    - WPF Client
    - Windows Service
    - Shared Libraries
    
**Output:** Compiled binaries  
**Priority:** high  
**Build Frequency:** On every commit to feature and main branches  
    - **Target:** Data Migration Utility  
**Components:**
    
    - Console Application
    
**Output:** Standalone executable  
**Priority:** medium  
**Build Frequency:** On commit to its specific source path  
    
  - **Testing Compliance:**
    
    - **Framework:** xUnit  
**Applicable Targets:**
    
    - Main Application Solution
    - Data Migration Utility
    
**Test Types:**
    
    - Unit Tests
    
**Reporting Required:** True  
**Coverage Threshold:** 75%  
**Justification:** REQ-TEC-003 specifies xUnit as the testing framework.  
    - **Framework:** Docker Compose  
**Applicable Targets:**
    
    - Windows Service
    
**Test Types:**
    
    - Integration Tests
    
**Reporting Required:** True  
**Coverage Threshold:** N/A  
**Justification:** Implicitly required to validate interaction with PostgreSQL and RabbitMQ dependencies before deployment.  
    
  - **Build Environment Specifications:**
    
    - **Environment:** Build Agent  
**Description:** The virtual machine or container responsible for executing CI pipeline jobs.  
**Requirements:**
    
    - Windows Server OS
    - .NET 8 SDK
    - Visual Studio 2022 Build Tools
    - Code Signing Certificate
    
**Access Needs:** Access to source code repository, artifact repository, and secret store.  
    
  
- **Code Quality And Security Analysis:**
  
  - **Scan Types:**
    
    - **Type:** Static Code Analysis (SAST)  
**Tool:** Integrated Roslyn Analyzers  
**Applicable Targets:**
    
    - Main Application Solution
    
**Trigger:** During `dotnet build` step  
**Configuration:** Configured via .editorconfig and project files.  
**Justification:** REQ-TEC-003 mandates the use of Roslyn Analyzers.  
    - **Type:** Software Composition Analysis (SCA)  
**Tool:** OWASP Dependency-Check  
**Applicable Targets:**
    
    - Main Application Solution
    
**Trigger:** Dedicated pipeline stage after build and test.  
**Configuration:** Scans all NuGet packages for known vulnerabilities (CVEs).  
**Justification:** REQ-NFR-004 mandates automated dependency vulnerability scanning.  
    
  - **Scan Frequency:**
    
    - **Target:** Feature Branches  
**Frequency:** On every push  
**Purpose:** Provide fast feedback to developers before merging code.  
**Pipeline Stage:** CI Pipeline  
    - **Target:** Main Branch  
**Frequency:** On every push (post-merge)  
**Purpose:** Ensure the main line of code is always secure and high-quality before creating a release candidate.  
**Pipeline Stage:** CI Pipeline  
    
  
- **Deployment Strategy Design:**
  
  - **Deployment Environments:**
    
    - **Environment:** Training  
**Purpose:** A sandboxed environment for hands-on user training and practice, as per REQ-8.3.  
**Deployment Trigger:** Automatic upon successful CI build on `main` branch.  
**Data Policy:** Populated with anonymized sample data.  
    - **Environment:** Pilot  
**Purpose:** A production-like environment for a limited group of users to validate functionality and performance, as per REQ-8.1.  
**Deployment Trigger:** Manual approval after successful deployment to Training.  
**Data Policy:** Live production data.  
    - **Environment:** Production  
**Purpose:** The live environment for all end-users.  
**Deployment Trigger:** Manual approval after successful validation in Pilot.  
**Data Policy:** Live production data.  
    
  - **Deployment Sequence:**
    
    - **Sequence:** 1  
**Environment:** Training  
**Description:** Initial automated deployment for basic validation and user training purposes.  
**Dependencies:**
    
    - Successful CI build and artifact publication
    
    - **Sequence:** 2  
**Environment:** Pilot  
**Description:** First phase of production rollout to a limited user base. Requires manual sign-off.  
**Dependencies:**
    
    - Successful deployment to Training
    - QA/Business approval
    
    - **Sequence:** 3  
**Environment:** Production  
**Description:** Full rollout to all users. Requires final management sign-off.  
**Dependencies:**
    
    - Successful validation in Pilot environment
    - Change Advisory Board (CAB) approval
    
    
  
- **Quality Gate And Approval Workflow:**
  
  - **Automated Quality Gates:**
    
    - **Gate:** Unit Test Success  
**Metric:** 100% test pass rate  
**Stage:** CI Pipeline  
**Enforcement:** Pipeline fails if any test fails.  
    - **Gate:** Code Coverage  
**Metric:** Coverage must not decrease and must be >= 75%  
**Stage:** CI Pipeline  
**Enforcement:** Pipeline fails if coverage drops below the threshold.  
    - **Gate:** Security Scan  
**Metric:** No new critical or high severity vulnerabilities found.  
**Stage:** CI Pipeline  
**Enforcement:** Pipeline fails if critical/high vulnerabilities are detected by OWASP Dependency-Check.  
    
  - **Manual Approval Gates:**
    
    - **Gate:** Pilot Deployment Approval  
**Approvers:**
    
    - QA Lead
    - Product Owner
    
**Stage:** CD Pipeline  
**Procedure:** Approvers review test results and Training environment validation before promoting to Pilot.  
    - **Gate:** Production Deployment Approval  
**Approvers:**
    
    - Change Advisory Board (CAB)
    - Head of IT Operations
    
**Stage:** CD Pipeline  
**Procedure:** Final sign-off based on successful pilot phase and operational readiness.  
    
  
- **Artifact Management Strategy:**
  
  - **Artifact Types:**
    
    - **Type:** Signed MSIX Package  
**Description:** The primary, immutable deployment artifact for the application.  
**Versioning:** Semantic Versioning 2.0.0  
**Retention:** Release versions retained indefinitely.  
    - **Type:** Test and Scan Reports  
**Description:** Evidence artifacts including unit test results, coverage reports, and security scan outputs.  
**Versioning:** Tied to the build number.  
**Retention:** Retained for 180 days for audit purposes.  
    - **Type:** Data Migration Utility  
**Description:** Versioned executable or zip package for the migration tool.  
**Versioning:** Semantic Versioning 2.0.0  
**Retention:** Release versions retained indefinitely.  
    
  - **Artifact Promotion Workflow:**
    
    - **Strategy:** A single artifact is built once in the CI pipeline. This exact, unmodified artifact is then promoted across all deployment environments (Training -> Pilot -> Production). This ensures that what was tested is what gets deployed.
    - **Repository:** A dedicated artifact repository like Azure Artifacts or JFrog Artifactory is required.
    - **Signing:** Artifacts are signed during the packaging stage of the CI pipeline, before being published, to ensure integrity.
    
  
- **Rollback And Recovery Design:**
  
  - **Rollback Mechanisms:**
    
    - **Component:** Application (MSIX)  
**Mechanism:** Redeploy Previous Version  
**Automation Level:** Automated  
**Description:** The CD pipeline can be re-run with the version number of the last known-good release as a parameter, which will install the older version over the faulty one.  
    - **Component:** Database (PostgreSQL)  
**Mechanism:** Fix Forward or Manual Restore  
**Automation Level:** Manual  
**Description:** Database rollbacks are high-risk. The primary strategy is to deploy a new 'fix-forward' patch. If a rollback is unavoidable, it requires executing EF Core 'down' migrations manually or restoring from the latest database backup.  
    
  - **Post Deployment Verification:**
    
    - **Method:** Automated Smoke Tests
    - **Frequency:** Immediately after every deployment
    - **Scope:**
      
      - Verify the Windows Service is running.
      - Perform a C-ECHO test to a configured PACS.
      - Check the application's health endpoint (if available) or basic database connectivity.
      
    - **Alerting On Failure:** True
    
  
- **Project Specific Pipelines:**
  
  - **Pipeline Definitions:**
    
    - **Name:** Application CI/CD Pipeline  
**Description:** A comprehensive pipeline that builds, tests, and packages the main application, then orchestrates its deployment through the required environments with appropriate quality gates.  
**Stages:**
    
    - Build
    - Test
    - Security Scan
    - Package & Publish
    - Deploy to Training
    - Approve for Pilot
    - Deploy to Pilot
    - Approve for Production
    - Deploy to Production
    
    - **Name:** Data Migration Utility CI Pipeline  
**Description:** A simpler pipeline to build, test, and package the standalone data migration utility specified in REQ-8.2.  
**Stages:**
    
    - Build
    - Unit Test
    - Package & Publish Artifact
    
    
  
- **Implementation Priority:**
  
  - **Component:** Application CI Pipeline (Build, Test, Package)  
**Priority:** high  
**Dependencies:**
    
    - Source code repository setup
    - Build agent configuration
    
**Estimated Effort:** Medium  
**Risk Level:** low  
  - **Component:** Application CD Pipeline (Training, Pilot, Production)  
**Priority:** high  
**Dependencies:**
    
    - CI Pipeline completion
    - Environment provisioning
    
**Estimated Effort:** High  
**Risk Level:** medium  
  - **Component:** Security Scanning Integration (OWASP)  
**Priority:** medium  
**Dependencies:**
    
    - CI Pipeline
    
**Estimated Effort:** Medium  
**Risk Level:** medium  
  
- **Risk Assessment:**
  
  - **Risk:** Inconsistent environments lead to deployment failures in later stages.  
**Impact:** high  
**Probability:** medium  
**Mitigation:** Utilize Infrastructure as Code (IaC) as suggested in REQ-6.1 to define and provision all environments (Training, Pilot, Prod) from the same templates, ensuring consistency.  
**Contingency Plan:** Manual intervention by DevOps team to align environment configurations. Document the deviation and update IaC scripts.  
  - **Risk:** Database migration script fails during production deployment.  
**Impact:** high  
**Probability:** medium  
**Mitigation:** All migrations are mandatorily tested by deploying through all lower environments (Training, Pilot) first. Implement pre-deployment database backups and ensure all migrations have a tested 'down' script for reversibility.  
**Contingency Plan:** Execute the 'down' migration script to revert the schema change. If not possible, restore the database from the pre-deployment backup. Initiate a 'fix-forward' patch release immediately.  
  - **Risk:** Code signing certificate expires, blocking all deployments.  
**Impact:** high  
**Probability:** low  
**Mitigation:** Store the certificate in a secure vault with automated monitoring and alerting for expiration dates at 90, 60, and 30 days.  
**Contingency Plan:** Emergency certificate renewal process. A designated team member must be on-call to handle such an event.  
  
- **Recommendations:**
  
  - **Category:** Configuration Management  
**Recommendation:** Strictly separate configuration from application artifacts. Use a centralized secret store like Azure Key Vault or HashiCorp Vault, integrated with the CD pipeline to inject environment-specific variables at deployment time.  
**Justification:** Enhances security by not storing secrets in source control and improves flexibility by allowing configuration changes without rebuilding the application.  
**Priority:** high  
  - **Category:** Infrastructure  
**Recommendation:** Leverage Infrastructure as Code (IaC) using PowerShell DSC or Ansible, as suggested in REQ-6.1, to automate the setup of build agents and deployment target environments.  
**Justification:** Ensures consistency across all environments, reduces manual setup errors, and makes the entire infrastructure versionable and repeatable.  
**Priority:** high  
  - **Category:** Testing  
**Recommendation:** Implement a suite of automated smoke tests that run immediately after each deployment to verify core application functionality.  
**Justification:** Provides rapid feedback on deployment success or failure, reducing the mean time to detect (MTTD) production issues.  
**Priority:** medium  
  


---

