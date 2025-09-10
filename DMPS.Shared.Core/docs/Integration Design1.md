# 1 Integration Specifications

## 1.1 Dependency Levels

### 1.1.1 Level

#### 1.1.1.1 Level

🔹 0

#### 1.1.1.2 Files

- src/DMPS.Shared.Core/Domain/Primitives/IAggregateRoot.cs
- src/DMPS.Shared.Core/Domain/Primitives/IDomainEvent.cs
- src/DMPS.Shared.Core/Domain/Primitives/ValueObject.cs
- src/DMPS.Shared.Core/Application/Contracts/Repositories/IUnitOfWork.cs
- src/DMPS.Shared.Core/Application/Contracts/DTOs/ProcessDicomStoreCommand.cs
- src/DMPS.Shared.Core/Application/Contracts/DTOs/SubmitPrintJobCommand.cs
- src/DMPS.Shared.Core/Application/Contracts/DTOs/GeneratePdfCommand.cs
- src/DMPS.Shared.Core/Application/Contracts/DTOs/DicomQueryCriteria.cs
- src/DMPS.Shared.Core/Application/Contracts/DTOs/AuditFilterCriteria.cs
- src/DMPS.Shared.Core/Common/Guard.cs
- src/DMPS.Shared.Core/Common/Result.cs
- src/DMPS.Shared.Core/CrossCutting/Exceptions/DomainException.cs
- src/DMPS.Shared.Core/Domain/Services/IPasswordPolicyValidator.cs

### 1.1.2.0 Level

#### 1.1.2.1 Level

🔹 1

#### 1.1.2.2 Files

- src/DMPS.Shared.Core/Domain/Primitives/Entity.cs
- src/DMPS.Shared.Core/Application/Contracts/Repositories/IGenericRepository.cs
- src/DMPS.Shared.Core/CrossCutting/Exceptions/EntityNotFoundException.cs
- src/DMPS.Shared.Core/CrossCutting/Exceptions/ValidationException.cs

### 1.1.3.0 Level

#### 1.1.3.1 Level

🔹 2

#### 1.1.3.2 Files

- src/DMPS.Shared.Core/Domain/Entities/Role.cs
- src/DMPS.Shared.Core/Domain/Entities/Patient.cs
- src/DMPS.Shared.Core/Domain/Entities/AuditLog.cs
- src/DMPS.Shared.Core/Domain/Entities/SystemSetting.cs
- src/DMPS.Shared.Core/Domain/Entities/PacsConfiguration.cs
- src/DMPS.Shared.Core/Domain/Entities/AutoRoutingRule.cs
- src/DMPS.Shared.Core/Domain/Entities/UserSession.cs
- src/DMPS.Shared.Core/Application/Contracts/Repositories/IPatientRepository.cs
- src/DMPS.Shared.Core/CrossCutting/Exceptions/DuplicateUsernameException.cs
- src/DMPS.Shared.Core/Application/Contracts/Repositories/ISystemSettingRepository.cs
- src/DMPS.Shared.Core/Application/Contracts/Repositories/IPacsConfigurationRepository.cs
- src/DMPS.Shared.Core/Application/Contracts/Repositories/IAutoRoutingRuleRepository.cs

### 1.1.4.0 Level

#### 1.1.4.1 Level

🔹 3

#### 1.1.4.2 Files

- src/DMPS.Shared.Core/Domain/Entities/User.cs
- src/DMPS.Shared.Core/Domain/Entities/Study.cs
- src/DMPS.Shared.Core/Application/Contracts/Repositories/IAuditLogRepository.cs

### 1.1.5.0 Level

#### 1.1.5.1 Level

🔹 4

#### 1.1.5.2 Files

- src/DMPS.Shared.Core/Domain/Entities/PasswordHistory.cs
- src/DMPS.Shared.Core/Domain/Entities/Series.cs
- src/DMPS.Shared.Core/Domain/Entities/PrintJob.cs
- src/DMPS.Shared.Core/Domain/Entities/HangingProtocol.cs
- src/DMPS.Shared.Core/Domain/Entities/UserPreference.cs
- src/DMPS.Shared.Core/Application/Contracts/Repositories/IUserRepository.cs
- src/DMPS.Shared.Core/Application/Contracts/Repositories/IStudyRepository.cs

### 1.1.6.0 Level

#### 1.1.6.1 Level

🔹 5

#### 1.1.6.2 Files

- src/DMPS.Shared.Core/Domain/Entities/Image.cs
- src/DMPS.Shared.Core/Domain/Entities/PresentationState.cs
- src/DMPS.Shared.Core/Application/Contracts/Repositories/IPrintJobRepository.cs
- src/DMPS.Shared.Core/Application/Contracts/Repositories/IHangingProtocolRepository.cs
- src/DMPS.Shared.Core/Domain/Services/PasswordPolicyValidator.cs

## 1.2.0.0 Total Files

43

## 1.3.0.0 Generation Order

- 0
- 1
- 2
- 3
- 4
- 5

