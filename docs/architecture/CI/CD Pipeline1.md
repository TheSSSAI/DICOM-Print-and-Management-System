# Specification

# 1. Pipelines

## 1.1. DMPS Application Build and Release
Builds, tests, analyzes, signs, and deploys the DMPS Client and Service components based on REQ-TEC-003 and REQ-NFR-004.

### 1.1.4. Stages

### 1.1.4.1. Build & Analyze
#### 1.1.4.1.2. Steps

- dotnet restore
- dotnet build --configuration Release
- dotnet test --logger trx --collect:"XPlat Code Coverage"

#### 1.1.4.1.3. Environment

- **Dotnet_Sdk_Version:** 8.0

#### 1.1.4.1.4. Quality Gates

- **Name:** Static Analysis & Unit Test  
**Criteria:**
    
    - all Roslyn Analyzers passed
    - all xUnit tests passed
    - code coverage >= 80%
    
**Blocking:** True  

### 1.1.4.2. Security Scan
#### 1.1.4.2.2. Steps

- run-owasp-dependency-check --format JSON

#### 1.1.4.2.3. Environment

- **Fail_On_Severity:** HIGH

#### 1.1.4.2.4. Quality Gates

- **Name:** Dependency Vulnerability Check  
**Criteria:**
    
    - zero critical CVEs
    - zero high CVEs
    
**Blocking:** True  

### 1.1.4.3. Package & Sign
#### 1.1.4.3.2. Steps

- dotnet publish -p:SolutionDir=. -c Release -f net8.0-windows
- create-msix-package-from-publish-output
- sign-msix-package --certificate $CODE_SIGN_CERT
- archive-service-binaries --output DMPS.Service.${{build.number}}.zip

#### 1.1.4.3.3. Environment

- **Artifact_Version:** 1.0.${{build.number}}

#### 1.1.4.3.4. Quality Gates

- **Name:** Artifact Signing Verification  
**Criteria:**
    
    - MSIX package is successfully signed
    
**Blocking:** True  

### 1.1.4.4. Deploy to Staging (Pilot)
#### 1.1.4.4.2. Steps

- deploy-service-binaries --target Staging --package DMPS.Service.${{build.number}}.zip
- dotnet ef database update --connection $STAGING_DB_CONNECTION_STRING
- publish-msix-to-staging-distribution-point

#### 1.1.4.4.3. Environment

- **Deploy_Environment:** Staging

#### 1.1.4.4.4. Quality Gates

- **Name:** Manual Approval for Pilot  
**Criteria:**
    
    - Approval by QA Lead
    
**Blocking:** True  

### 1.1.4.5. Deploy to Production
#### 1.1.4.5.2. Steps

- deploy-service-binaries --target Production --package DMPS.Service.${{build.number}}.zip
- dotnet ef database update --connection $PROD_DB_CONNECTION_STRING
- publish-msix-to-production-distribution-point

#### 1.1.4.5.3. Environment

- **Deploy_Environment:** Production

#### 1.1.4.5.4. Quality Gates

- **Name:** Manual Approval for Production  
**Criteria:**
    
    - Approval by Release Manager
    
**Blocking:** True  


## 1.2. Data Migration Utility CI
Builds and packages the data migration utility required for system transition as per REQ-8.2.

### 1.2.4. Stages

### 1.2.4.1. Build & Test
#### 1.2.4.1.2. Steps

- dotnet restore
- dotnet build --configuration Release
- dotnet test

#### 1.2.4.1.3. Environment

- **Dotnet_Sdk_Version:** 8.0

#### 1.2.4.1.4. Quality Gates

- **Name:** Build and Test Success  
**Criteria:**
    
    - build successful
    - all tests passed
    
**Blocking:** True  

### 1.2.4.2. Package
#### 1.2.4.2.2. Steps

- dotnet publish -c Release
- archive-published-output --output DMPS.MigrationUtility.${{build.number}}.zip

#### 1.2.4.2.3. Environment

- **Artifact_Name:** DMPS.MigrationUtility.zip




---

# 2. Configuration

- **Artifact Repository:** Internal-Artifactory/NuGet-Release
- **Default Branch:** main
- **Retention Policy:** 90d
- **Notification Channel:** email:release-team@example.com


---

