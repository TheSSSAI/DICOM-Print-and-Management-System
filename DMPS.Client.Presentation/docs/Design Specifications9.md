# 1 Sds

## 1.1 Analysis Metadata

| Property | Value |
|----------|-------|
| Analysis Timestamp | 2024-07-31T10:00:00Z |
| Repository Component Id | DMPS.Client.Presentation |
| Analysis Completeness Score | 100 |
| Critical Findings Count | 1 |
| Analysis Methodology | Systematic analysis of cached context, including r... |

## 1.2 Repository Analysis

### 1.2.1 Repository Definition

#### 1.2.1.1 Scope Boundaries

- Responsible for all user-facing elements, including XAML Views, ViewModels, and UI-specific services like navigation and dialog management, strictly following the MVVM pattern.
- Must implement the high-performance, GPU-accelerated DICOM viewer control using the Vortice.Windows library.
- Must not contain core business logic, direct database access, or direct interaction with the message queue; all such operations are delegated to the Application Services layer (REPO-08-APC).

#### 1.2.1.2 Technology Stack

- WPF on .NET 8.0 with C# 12
- Material Design in XAML Toolkit v5.0.0 for UI controls and theming.
- Vortice.Windows v3.2.1 for low-level DirectX interop for the DICOM viewer.

#### 1.2.1.3 Architectural Constraints

- Must strictly adhere to the Model-View-ViewModel (MVVM) pattern, with minimal code-behind in Views.
- All potentially long-running operations initiated by the UI must be executed asynchronously to maintain UI responsiveness (<500ms response time), offloading work to the application services layer.

#### 1.2.1.4 Dependency Relationships

##### 1.2.1.4.1 Service Consumption: REPO-08-APC (DMPS.Client.Application)

###### 1.2.1.4.1.1 Dependency Type

Service Consumption

###### 1.2.1.4.1.2 Target Component

REPO-08-APC (DMPS.Client.Application)

###### 1.2.1.4.1.3 Integration Pattern

Dependency Injection

###### 1.2.1.4.1.4 Reasoning

The Presentation layer is decoupled from application logic. ViewModels receive service interfaces (e.g., IAuthenticationService, IPrintJobService) via constructor injection to orchestrate business operations without knowing their implementation details.

##### 1.2.1.4.2.0 Model Consumption: REPO-01-SHK (DMPS.Shared.Core)

###### 1.2.1.4.2.1 Dependency Type

Model Consumption

###### 1.2.1.4.2.2 Target Component

REPO-01-SHK (DMPS.Shared.Core)

###### 1.2.1.4.2.3 Integration Pattern

Direct Reference

###### 1.2.1.4.2.4 Reasoning

To ensure a consistent data model across the entire application, this repository consumes domain entities and DTOs (e.g., User, Study) directly from the Shared Kernel for use in ViewModel properties and service call parameters.

#### 1.2.1.5.0.0 Analysis Insights

The primary complexity and risk factor for this repository is the implementation of the GPU-accelerated DICOM viewer using Vortice.Windows. This requires specialized graphics programming knowledge beyond standard WPF development. The rest of the repository follows well-defined MVVM patterns, acting as a pure presentation layer that delegates all business logic and long-running tasks.

## 1.3.0.0.0.0 Requirements Mapping

### 1.3.1.0.0.0 Functional Requirements

#### 1.3.1.1.0.0 Requirement Id

##### 1.3.1.1.1.0 Requirement Id

REQ-1-013

##### 1.3.1.1.2.0 Requirement Description

The WPF client application MUST be developed using the MVVM pattern.

##### 1.3.1.1.3.0 Implementation Implications

- A strict separation must be maintained between XAML files (Views) and C# classes (ViewModels).
- ViewModels must not have a direct reference to any UI elements from the View.
- Use of an MVVM toolkit like CommunityToolkit.Mvvm is recommended to reduce boilerplate for INotifyPropertyChanged and ICommand.

##### 1.3.1.1.4.0 Required Components

- Views (XAML files for each screen/control)
- ViewModels (C# classes for presentation logic)

##### 1.3.1.1.5.0 Analysis Reasoning

This is the core architectural mandate for the repository, ensuring testability, maintainability, and effective use of WPF's data binding capabilities.

#### 1.3.1.2.0.0 Requirement Id

##### 1.3.1.2.1.0 Requirement Id

REQ-1-052

##### 1.3.1.2.2.0 Requirement Description

The DICOM viewer MUST utilize GPU-accelerated rendering for optimal performance.

##### 1.3.1.2.3.0 Implementation Implications

- A custom control must be developed that integrates Vortice.Windows for DirectX rendering within a WPF application, likely using D3DImage.
- Logic for managing GPU resources (textures, shaders, render targets) must be carefully implemented to avoid memory leaks and performance issues.

##### 1.3.1.2.4.0 Required Components

- DicomViewerControl (View)
- DicomViewerViewModel (ViewModel)
- RenderingService (encapsulating Vortice.Windows logic)

##### 1.3.1.2.5.0 Analysis Reasoning

This requirement dictates the use of the specified Vortice.Windows library and introduces the most complex technical challenge within the repository.

#### 1.3.1.3.0.0 Requirement Id

##### 1.3.1.3.1.0 Requirement Id

REQ-1-041

##### 1.3.1.3.2.0 Requirement Description

The application MUST automatically lock the session after 15 minutes of inactivity.

##### 1.3.1.3.3.0 Implementation Implications

- The presentation layer must implement low-level keyboard and mouse hooks to detect user activity system-wide.
- A LockScreenView and LockScreenViewModel must be created to display an overlay and handle re-authentication.
- The UI must respond to a global 'LockSession' event published by the application service layer.

##### 1.3.1.3.4.0 Required Components

- GlobalInputMonitor (Utility)
- LockScreenView (View)
- LockScreenViewModel (ViewModel)

##### 1.3.1.3.5.0 Analysis Reasoning

As per sequence SEQ-SEC-009, this repository is responsible for the UI-facing aspects of the session lock feature: activity detection and displaying the lock screen.

### 1.3.2.0.0.0 Non Functional Requirements

#### 1.3.2.1.0.0 Requirement Type

##### 1.3.2.1.1.0 Requirement Type

Performance

##### 1.3.2.1.2.0 Requirement Specification

UI must remain responsive (<500ms) even during background operations.

##### 1.3.2.1.3.0 Implementation Impact

All command handlers in ViewModels that trigger long-running operations (e.g., printing, exporting, querying PACS) must be implemented using 'async/await' and call asynchronous methods on the injected application services. This prevents blocking the UI thread.

##### 1.3.2.1.4.0 Design Constraints

- No synchronous I/O operations on the UI thread.
- Leverage UI virtualization (e.g., VirtualizingStackPanel) for large lists of data like the study browser.

##### 1.3.2.1.5.0 Analysis Reasoning

This NFR is a primary driver for the client-server architecture. This repository fulfills its part by ensuring all potentially blocking calls are asynchronous, delegating the actual work to other components.

#### 1.3.2.2.0.0 Requirement Type

##### 1.3.2.2.1.0 Requirement Type

Security

##### 1.3.2.2.2.0 Requirement Specification

Dynamically enable/disable UI controls based on the logged-in user's role.

##### 1.3.2.2.3.0 Implementation Impact

ViewModels must expose properties reflecting the current user's role or permissions (e.g., 'IsAdminUser'). XAML views will use data binding with converters (e.g., 'BooleanToVisibilityConverter') to control the visibility and 'IsEnabled' state of administrative UI elements.

##### 1.3.2.2.4.0 Design Constraints

- Authorization logic is not implemented here, but the UI must react to the authorization state provided by the application service layer.

##### 1.3.2.2.5.0 Analysis Reasoning

This ensures the UI correctly enforces the Role-Based Access Control (RBAC) model defined in the system requirements (REQ-1-014).

### 1.3.3.0.0.0 Requirements Analysis Summary

This repository is the implementation point for all user-facing functional requirements. Its design is heavily influenced by non-functional requirements for performance and security, which mandate an asynchronous, role-aware, and strictly separated MVVM architecture. The feature set ranges from standard CRUD UIs to a highly complex, custom-rendered graphics component.

## 1.4.0.0.0.0 Architecture Analysis

### 1.4.1.0.0.0 Architectural Patterns

- {'pattern_name': 'Model-View-ViewModel (MVVM)', 'pattern_application': 'The entire repository is structured around MVVM. Views (XAML) are responsible for presentation, ViewModels for presentation logic and state, and Models (from REPO-01-SHK) for data representation. This decouples the UI from logic, enabling testability and maintainability.', 'required_components': ['BaseViewModel (optional, for shared INotifyPropertyChanged logic)', 'Individual Views and corresponding ViewModels for each feature'], 'implementation_strategy': "Use a standard MVVM framework or toolkit (e.g., CommunityToolkit.Mvvm) to implement 'ObservableObject' for state and 'RelayCommand'/'AsyncRelayCommand' for actions. Use Dependency Injection to provide services to ViewModels.", 'analysis_reasoning': "This pattern is explicitly mandated by REQ-1-013 and is the industry standard for building robust WPF applications, leveraging the framework's data binding capabilities."}

### 1.4.2.0.0.0 Integration Points

- {'integration_type': 'Service Layer Integration', 'target_components': ['REPO-08-APC (DMPS.Client.Application)'], 'communication_pattern': 'Synchronous & Asynchronous Method Calls (In-Process)', 'interface_requirements': ['Must consume interfaces like IAuthenticationService, IPrintJobService, and ISystemStatusService.', 'All method calls that involve I/O or potentially long execution times must be asynchronous and return a Task.'], 'analysis_reasoning': 'This is the sole integration point for business and application logic. The Presentation layer is a consumer of services provided by the Application layer, ensuring a clean separation of concerns as defined by the overall layered architecture.'}

### 1.4.3.0.0.0 Layering Strategy

| Property | Value |
|----------|-------|
| Layer Organization | This repository constitutes the 'Presentation Laye... |
| Component Placement | Components are organized by their role in the MVVM... |
| Analysis Reasoning | The layering aligns with the provided architecture... |

## 1.5.0.0.0.0 Database Analysis

### 1.5.1.0.0.0 Entity Mappings

- {'entity_name': 'User, Patient, Study, etc.', 'database_table': 'N/A', 'required_properties': ['ViewModels will consume properties from domain entities defined in REPO-01-SHK.'], 'relationship_mappings': ['N/A'], 'access_patterns': ['This repository does not access the database directly. It requests data from the application service layer, which in turn uses repositories.', "Data is typically consumed as DTOs or domain entities and wrapped in ViewModel classes for UI presentation (e.g., adding an 'IsSelected' property)."], 'analysis_reasoning': 'As a pure presentation layer, this repository has no direct database responsibilities. Its data intelligence is limited to consuming and presenting data models provided by its dependencies.'}

### 1.5.2.0.0.0 Data Access Requirements

- {'operation_type': 'Read/Write Operations', 'required_methods': ["All data operations are delegated through service interfaces (e.g., 'IAuthenticationService.LoginAsync')."], 'performance_constraints': 'Data display performance is critical. For large collections (e.g., study lists), the UI must use virtualization to avoid performance degradation.', 'analysis_reasoning': "This repository's responsibility is to request data operations, not implement them. The key requirement is to handle the returned data efficiently for display."}

### 1.5.3.0.0.0 Persistence Strategy

| Property | Value |
|----------|-------|
| Orm Configuration | N/A |
| Migration Requirements | N/A |
| Analysis Reasoning | Persistence is handled by the Data Access Layer (R... |

## 1.6.0.0.0.0 Sequence Analysis

### 1.6.1.0.0.0 Interaction Patterns

#### 1.6.1.1.0.0 Sequence Name

##### 1.6.1.1.1.0 Sequence Name

SEQ-AFL-001: User Login - Successful Authentication

##### 1.6.1.1.2.0 Repository Role

Initiator

##### 1.6.1.1.3.0 Required Interfaces

- IAuthenticationService

##### 1.6.1.1.4.0 Method Specifications

- {'method_name': 'LoginAsync', 'interaction_context': "Called from the LoginViewModel's 'LoginCommand' when the user clicks the login button.", 'parameter_analysis': "Accepts 'string username' and 'string password' (or 'SecureString') from the UI input fields.", 'return_type_analysis': "Returns a 'Task<bool>' or a user session object. The ViewModel awaits this result to determine whether to navigate to the main application window or display an error.", 'analysis_reasoning': 'This method call delegates the core authentication logic to the application service layer, keeping the ViewModel clean and testable.'}

##### 1.6.1.1.5.0 Analysis Reasoning

The sequence confirms this repository's role as the starting point for user-driven workflows, translating UI events into service calls.

#### 1.6.1.2.0.0 Sequence Name

##### 1.6.1.2.1.0 Sequence Name

SEQ-EVP-003: Asynchronous Print Job Submission

##### 1.6.1.2.2.0 Repository Role

Initiator

##### 1.6.1.2.3.0 Required Interfaces

- IPrintJobService

##### 1.6.1.2.4.0 Method Specifications

- {'method_name': 'SubmitPrintJobAsync', 'interaction_context': "Called from the 'PrintPreviewViewModel' when the user clicks the 'Print' button.", 'parameter_analysis': "Accepts a 'PrintJobData' DTO containing all necessary information for the backend to process the job (layout, image UIDs, printer name, etc.).", 'return_type_analysis': "Returns a 'Task' that completes once the job has been successfully queued. The ViewModel can then display a non-blocking 'Job Queued' notification to the user.", 'analysis_reasoning': 'This interaction demonstrates the asynchronous offloading pattern, which is critical for maintaining UI responsiveness. The UI does not wait for the print job to finish, only for it to be accepted.'}

##### 1.6.1.2.5.0 Analysis Reasoning

This sequence highlights the repository's responsibility to package user intent into asynchronous commands for backend processing.

### 1.6.2.0.0.0 Communication Protocols

#### 1.6.2.1.0.0 Protocol Type

##### 1.6.2.1.1.0 Protocol Type

In-Process Dependency Injection

##### 1.6.2.1.2.0 Implementation Requirements

A DI container (e.g., 'Microsoft.Extensions.DependencyInjection') must be configured at application startup to register all services from REPO-08-APC and resolve them for ViewModel constructors.

##### 1.6.2.1.3.0 Analysis Reasoning

This is the primary communication pattern with the immediate lower layer, enabling loose coupling and high testability.

#### 1.6.2.2.0.0 Protocol Type

##### 1.6.2.2.1.0 Protocol Type

In-Process Event Aggregator

##### 1.6.2.2.2.0 Implementation Requirements

A singleton event aggregator service should be used for ViewModel-to-ViewModel communication, allowing decoupled components to publish and subscribe to application-wide UI events.

##### 1.6.2.2.3.0 Analysis Reasoning

This pattern is essential for coordinating state between different parts of the UI without creating tight coupling between ViewModels.

## 1.7.0.0.0.0 Critical Analysis Findings

- {'finding_category': 'Implementation Complexity', 'finding_description': 'The requirement for a GPU-accelerated DICOM viewer using the low-level Vortice.Windows library represents a significant technical challenge and risk. It requires specialized skills in DirectX and graphics programming, which are distinct from standard WPF/MVVM development.', 'implementation_impact': 'This component will require a dedicated development effort, potentially by a specialist developer. It must be architected carefully within its own service or control to isolate its complexity from the rest of the more conventional MVVM application. Insufficient expertise in this area could lead to major performance issues, memory leaks, or project delays.', 'priority_level': 'High', 'analysis_reasoning': "This single component drives a disproportionate amount of the repository's complexity and risk, and successful implementation is critical for a core feature of the application."}

## 1.8.0.0.0.0 Analysis Traceability

### 1.8.1.0.0.0 Cached Context Utilization

Analysis was performed by systematically cross-referencing the repository's definition ('REPO-09-PRE'), the overall system architecture ('ARCHITECTURE' file), all relevant sequence diagrams ('SEQ-*'), and the database design ('DATABASE DESIGN' for model context).

### 1.8.2.0.0.0 Analysis Decision Trail

- Identified the repository as a pure Presentation Layer based on its description and 'scope_boundaries'.
- Confirmed its dependencies and communication patterns by analyzing its 'dependency_contracts' against the 'ARCHITECTURE' specification.
- Mapped all UI-related functional requirements to this repository as the sole implementer.
- Pinpointed the 'Vortice.Windows' DICOM viewer as the highest complexity/risk component based on the technology stack and requirements.

### 1.8.3.0.0.0 Assumption Validations

- Validated the assumption that all long-running tasks are handled by 'REPO-08-APC' by cross-referencing sequence diagrams like 'SEQ-EVP-003' which show the hand-off to the service layer.
- Validated the assumption of strict MVVM adherence by checking the repository's 'architectural_constraints' and 'technology_standards'.

### 1.8.4.0.0.0 Cross Reference Checks

- The interfaces required by sequence diagrams (e.g., 'IAuthenticationService' in 'SEQ-AFL-001') were verified to exist in the 'dependency_contracts' section of the repository definition.
- The responsibilities listed in the 'ARCHITECTURE' file for the 'Presentation Layer' were confirmed to align with the 'scope_boundaries' of this repository.

# 2.0.0.0.0.0 Code Instructions

#TECH and REPO STRUCTURE INSTRUCTIONS\n# DesktopFrontend REPOSITORY GUIDELINES FOR WPF on .NET 8.0\nAct as an expert DesktopFrontend architect with deep expertise in WPF on .NET 8.0 development, focusing on **framework-native MVVM implementation, high-performance UI rendering, and modern .NET runtime optimizations**. Ensure all outputs maintain **military-grade architectural precision with 100% technology stack alignment** while optimizing for **WPF's data binding capabilities and .NET 8.0's performance enhancements**.\n\n- Technology-Informed Thinking Steps\n{\n  \"thinking_steps\": [\n    {\n      \"step\": \"Understand DesktopFrontend's Core Domain Responsibilities\",\n      \"details\": \"Identified the primary domain concerns, functional patterns, and architectural requirements specific to DesktopFrontend repositories, including **user interface presentation, user interaction handling, view state management, navigation control, data visualization, and application configuration management**, all geared towards a rich, responsive desktop user experience.\"\n    },\n    {\n      \"step\": \"Analyze WPF on .NET 8.0 Framework Capabilities and Patterns\",\n      \"details\": \"Assessed WPF on .NET 8.0-native architectural patterns, framework conventions, version-specific features, and optimization opportunities that align with repository type requirements, focusing on **XAML for declarative UI, robust data binding, command pattern, styling/templating, and leveraging .NET 8.0 runtime performance enhancements and modern C# 12 features**.\"\n    },\n    {\n      \"step\": \"Synthesize Technology-Repository Alignment\",\n      \"details\": \"Determined optimal integration points between DesktopFrontend domain requirements and WPF on .NET 8.0 framework capabilities, identifying native patterns and performance optimizations, specifically **adopting the MVVM pattern with 'CommunityToolkit.Mvvm', utilizing Dependency Injection for service and ViewModel resolution, and structuring UI with User Controls, Data Templates, and Styles for maintainability and reusability**.\"\n    },\n    {\n      \"step\": \"Design Framework-Native Repository Structure\",\n      \"details\": \"Architected repository organization using WPF on .NET 8.0-specific conventions, configuration patterns, and framework-native separation of concerns, creating distinct directories for **Views, ViewModels, Models, Services, Utilities (Converters, Behaviors), and Resources, along with leveraging 'Microsoft.Extensions.Hosting' for application bootstrapping and DI container setup**.\"\n    },\n    {\n      \"step\": \"Integrate Technology-Specific Quality Patterns\",\n      \"details\": \"Embedded WPF on .NET 8.0-native testing, validation, performance optimization, and security patterns appropriate for DesktopFrontend implementations, including **unit testing for ViewModels and Services, 'IDataErrorInfo' or 'INotifyDataErrorInfo' for input validation, asynchronous operations for UI responsiveness, and secure handling of local application configuration**.\"\n    }\n  ]\n}\n\n- WPF on .NET 8.0-Optimized Structure for **user interface presentation and interaction logic** including:\n  *   **'Application'**: **Entry point and lifecycle management** (e.g., 'App.xaml', 'App.xaml.cs') responsible for application startup, global resource definitions, theme management, and dependency injection container configuration using 'Microsoft.Extensions.Hosting' for a modern and extensible setup.\n  *   **'Views'**: **XAML-based user interface definitions** (e.g., 'MainWindow.xaml', 'UserControls/*.xaml', 'Pages/*.xaml') where the visual structure and layout are declared. This component strictly contains UI elements and relies on data binding to ViewModels, aligning with the \"View\" aspect of MVVM.\n  *   **'ViewModels'**: **Presentation logic and view state management** (e.g., 'MainWindowViewModel.cs', 'UserControlViewModel.cs', 'PageViewModel.cs') containing properties, commands, and logic that interacts with the Model and exposes data to the View via data binding. Leverages 'CommunityToolkit.Mvvm' for 'ObservableObject', 'RelayCommand', and 'AsyncRelayCommand' to efficiently manage UI updates and asynchronous operations, utilizing C# 12 features like primary constructors for dependency injection.\n  *   **'Models'**: **Domain entities and data transfer objects (DTOs)** (e.g., 'User.cs', 'Product.cs', 'Order.cs') representing the core business data. These are typically plain C# objects, potentially implementing 'INotifyPropertyChanged' if they need to be directly bound and updated in the UI, or primarily serving as data structures exchanged with services, maintaining domain purity.\n  *   **'Services'**: **Application logic, data access, and API interactions** (e.g., 'UserService.cs', 'ProductService.cs', 'DialogService.cs') encapsulating specific business operations, external system interactions (e.g., calling REST APIs via 'HttpClient'), or system-level functionalities. These are injected into ViewModels and follow interface-based contracts for testability and separation of concerns.\n  *   **'Utilities'**: **Helper classes for common WPF scenarios** including 'Converters' (e.g., 'BooleanToVisibilityConverter.cs' for transforming data for UI presentation), 'Behaviors' (e.g., 'InputValidationBehavior.cs' for extending UI elements without subclassing), and 'AttachedProperties' (e.g., 'FocusExtension.cs' for custom UI logic), enabling cleaner XAML and reusable logic without bloating ViewModels.\n  *   **'Commands'**: **Application-specific command definitions and implementations** (e.g., 'OpenSettingsCommand.cs' if not using 'RelayCommand' from a toolkit) that encapsulate actions triggered by user interactions, ensuring a clear separation of action logic from UI elements. Typically integrated directly into ViewModels via 'ICommand' implementations.\n  *   **'Resources'**: **Global styles, templates, and assets** (e.g., 'Styles/Colors.xaml', 'Styles/Buttons.xaml', 'Templates/DataTemplates.xaml', 'Images/Icons.xaml') for consistent look and feel across the application. Leverages WPF's resource dictionary merging for modular and theme-aware UI design, optimized for performance through compiled XAML resources.\n\n- **Framework-Native MVVM Architecture**: The entire application is structured around the Model-View-ViewModel pattern, leveraging WPF's strong data binding capabilities and 'ICommand' interface. ViewModels encapsulate presentation logic and state, cleanly separated from XAML Views, promoting testability and maintainability. 'CommunityToolkit.Mvvm' is used for modern, efficient MVVM implementation, including source generators for 'INotifyPropertyChanged' boilerplate reduction.\n- **Dependency Injection with Microsoft.Extensions.Hosting**: The application bootstraps using 'Microsoft.Extensions.Hosting' (or 'Microsoft.Extensions.DependencyInjection' directly) to manage the lifecycle and resolve dependencies for ViewModels and Services. This provides a robust, testable, and extensible architecture, allowing for easy swapping of service implementations and centralizing configuration.\n- **Version-Optimized Asynchronous Operations**: Utilizing C# 'async/await' patterns extensively in ViewModels and Services, particularly with 'AsyncRelayCommand' from 'CommunityToolkit.Mvvm', ensures that the UI remains responsive during long-running operations (e.g., API calls, data processing). This leverages .NET 8.0's improved asynchronous performance and modern language features.\n- **Technology-Specific UI Validation and Error Handling**: Input validation is handled at the ViewModel level using WPF-native interfaces like 'IDataErrorInfo' or 'INotifyDataErrorInfo', or through data annotations, ensuring that validation logic is close to the data it validates. Global error handling and logging are configured in 'App.xaml.cs' to gracefully manage unhandled exceptions, providing a consistent user experience and aiding debugging.\n\n\n\n# Layer enhancement Instructions\njson\n## DesktopFrontend REPOSITORY CONSIDERATIONS FOR WPF, .NET 8.0, Material Design in XAML Toolkit v5.0.0, Vortice.Windows v3.2.1\n\n- Technology-Informed Structural Thinking Steps\n{\n  \"thinking_steps\": [\n    {\n      \"step\": \"Understand DesktopFrontend's Core Structural Requirements\",\n      \"details\": \"Identified the primary organizational patterns, directory structures, and file organization conventions specific to DesktopFrontend repositories, including UI-centric concerns, strong MVVM adherence, user interaction flows, data presentation, and clear separation of presentation logic from views and domain models. Emphasized the need for modularity in UI components, reusability of styles and templates, and effective state management.\"\n    },\n    {\n      \"step\": \"Analyze WPF, .NET 8.0, Material Design in XAML Toolkit v5.0.0, Vortice.Windows v3.2.1 Framework-Native Organization Patterns\",\n      \"details\": \"Assessed WPF's native XAML/code-behind separation, data binding, command patterns, and resource dictionary usage. Evaluated .NET 8.0's modern SDK-style project structure, 'global using' directives, and 'file-scoped namespaces' for project simplification. Incorporated Material Design in XAML Toolkit's impact on theming, custom control organization, and dialog/snackbar patterns. Analyzed Vortice.Windows' implications for low-level graphics/interop, suggesting a dedicated integration layer or service.\"\n    },\n    {\n      \"step\": \"Synthesize Technology-Repository Structural Alignment\",\n      \"details\": \"Determined optimal integration between DesktopFrontend's MVVM requirements and WPF's native capabilities by creating distinct 'Views', 'ViewModels', and 'Models' directories. Aligned Material Design's components with a 'Resources/Themes/Controls' structure. Planned for specific 'Vortice.Windows' integration within 'Services/Rendering' or 'Infrastructure/Interop' to encapsulate low-level details, ensuring the core domain remains clean. Leveraged .NET 8.0 for modern project file structure and C# features.\"\n    },\n    {\n      \"step\": \"Design Framework-Native Repository Organization\",\n      \"details\": \"Architected repository structure using WPF's convention-over-configuration for MVVM, placing XAML views and corresponding C# view models in parallel directory hierarchies. Integrated Material Design themes and custom styles into dedicated 'Resources' folders. Utilized .NET 8.0 project file features for simplified dependencies. Defined distinct layers for application services, common infrastructure elements, and core domain abstractions, all within the context of a 'Microsoft.NET.Sdk.WindowsDesktop' project.\"\n    },\n    {\n      \"step\": \"Optimize for Technology Ecosystem Integration\",\n      \"details\": \"Structured repository for optimal integration with Visual Studio tooling, MSBuild processes, and NuGet package management (e.g., for Material Design, Vortice.Windows). Ensured project references and folder structures facilitate efficient development, debugging, and testing workflows characteristic of the .NET ecosystem. Considered performance optimizations from .NET 8.0 and organized code to support efficient XAML compilation and data binding, while maintaining DesktopFrontend domain clarity and responsiveness.\"\n    }\n  ]\n}\n\nWhen building the WPF, .NET 8.0, Material Design in XAML Toolkit v5.0.0, Vortice.Windows v3.2.1-optimized structure for this DesktopFrontend repository type, prioritize:\n\n-   **WPF MVVM-Centric Organization**: Adhere strictly to the MVVM pattern by clearly separating 'Views' (XAML), 'ViewModels' (presentation logic), and 'Models' (data/domain entities). This is the cornerstone of maintainable WPF applications.\n-   **Framework-Native Resource Management**: Centralize WPF resources like 'Styles', 'DataTemplates', 'ControlTemplates', and 'Brushes' within 'ResourceDictionary' files, often grouped by 'Themes' or 'Components', to leverage Material Design's theming capabilities efficiently.\n-   **Version-Optimized Project Structure (.NET 8.0)**: Utilize the modern SDK-style '.csproj' file, 'global using' directives, and 'file-scoped namespaces' to reduce boilerplate code and simplify project configuration, enhancing developer productivity and readability.\n-   **Technology-Integrated Interop Layer (Vortice.Windows)**: Create a dedicated 'Integration' or 'Rendering' layer/service to encapsulate low-level 'Vortice.Windows' API calls, preventing contamination of the UI or ViewModel layers and allowing for easier updates or alternative implementations.\n\nEssential WPF, .NET 8.0, Material Design in XAML Toolkit v5.0.0, Vortice.Windows v3.2.1-native directories and files should include:\n*   **'./src/{AppName}.Desktop/'**: Main WPF application project root, typically an SDK-style project ('.csproj') with 'Sdk=\"Microsoft.NET.Sdk.WindowsDesktop\"'.\n*   **'./src/{AppName}.Desktop/App.xaml(.cs)'**: Application entry point, responsible for global resources, startup logic, and dependency injection setup.\n*   **'./src/{AppName}.Desktop/Views/'**: Contains all XAML files representing UI components (e.g., 'MainWindow.xaml', 'UserControls/*.xaml', 'Pages/*.xaml'). Organized logically by feature or bounded context.\n*   **'./src/{AppName}.Desktop/ViewModels/'**: Contains C# classes that act as the data context for views, implementing 'INotifyPropertyChanged' and exposing 'ICommand's. Mirrors the 'Views' directory structure for clear mapping.\n*   **'./src/{AppName}.Desktop/Models/'**: Plain Old C# Objects (POCOs) representing the data or domain entities, often consumed by ViewModels.\n*   **'./src/{AppName}.Desktop/Services/'**: Interfaces ('I...Service.cs') and implementations ('...Service.cs') for application-level concerns (e.g., data access, navigation, dialogs, user settings). This layer often interacts with shared 'Infrastructure' or 'Domain' projects.\n*   **'./src/{AppName}.Desktop/Infrastructure/'**: Contains cross-cutting concerns specific to the desktop application, such as 'Converters', 'Behaviors', 'AttachedProperties', 'ValidationRules', 'CustomCommands', and potentially a 'Vortice' sub-folder for 'Vortice.Windows' specific interop or rendering components.\n*   **'./src/{AppName}.Desktop/Resources/'**: Contains shared 'ResourceDictionary' files, often grouped into 'Styles', 'Templates', and 'Themes'. This is where Material Design styles and overrides are typically defined and merged.\n*   **'./src/{AppName}.Desktop/Controls/'**: Reusable custom 'UserControl's or custom-rendered controls that encapsulate complex UI logic, potentially leveraging Material Design components or 'Vortice.Windows' for custom drawing surfaces.\n*   **'./src/{AppName}.Desktop/Properties/appsettings.json'**: .NET 8.0 native configuration file for application settings, leveraging the 'Microsoft.Extensions.Configuration' patterns.\n*   **'./src/{AppName}.Core/'**: An optional, separate .NET 8.0 class library project ('.csproj') for domain models, interfaces, shared constants, and core logic that is independent of the UI and potentially reusable by other application layers or clients.\n\nCritical WPF, .NET 8.0, Material Design in XAML Toolkit v5.0.0, Vortice.Windows v3.2.1-optimized interfaces with other components:\n*   **'ViewModels <-> Services'**: ViewModels interact with 'IService' interfaces (e.g., 'INavigationService', 'IDialogService') defined in the 'Services' or 'Core' layer, often resolved via Dependency Injection. This boundary ensures testability and separation of concerns.\n*   **'Views <-> ViewModels'**: Views bind to ViewModel properties and commands using WPF's data binding and command mechanisms. The 'DataContext' property in XAML explicitly defines this interface, with 'DataTemplates' mapping ViewModel types to View instances for automatic UI generation.\n*   **'Application <-> External Configuration (appsettings.json)'**: The application's startup logic (e.g., in 'App.xaml.cs') loads settings from 'appsettings.json' (and 'appsettings.{Environment}.json') using .NET 8.0's configuration providers, providing a standardized way to externalize application parameters.\n*   **'Rendering Layer <-> Vortice.Windows APIs'**: A dedicated 'IRenderer' or 'IVorticeInteropService' interface within the 'Infrastructure' or 'Services' layer abstracts direct calls to 'Vortice.Windows' APIs, providing a clean boundary for high-performance graphics or low-level OS interactions.\n\nFor this DesktopFrontend repository type with WPF, .NET 8.0, Material Design in XAML Toolkit v5.0.0, Vortice.Windows v3.2.1, the JSON structure should particularly emphasize:\n-   **'\"Views\": { \"type\": \"Folder\", \"description\": \"Contains WPF XAML files for user interfaces, logically grouped by feature or bounded context (e.g., 'Authentication', 'Dashboard', 'Settings'). Includes Windows, UserControls, and Pages. Directly binds to corresponding ViewModels.\" }'**: Framework-native structural optimization leveraging WPF's UI definition.\n-   **'\"ViewModels\": { \"type\": \"Folder\", \"description\": \"Contains C# classes implementing presentation logic for Views. Mirrors the 'Views' folder structure. Implements INotifyPropertyChanged and ICommand, utilizing .NET 8.0 features like 'global using' for cleaner code.\" }'**: Technology-specific configuration and setup patterns for MVVM.\n-   **'\"Resources\": { \"type\": \"Folder\", \"description\": \"Centralized location for WPF ResourceDictionary files. Includes 'Styles', 'Templates', 'Themes' (for Material Design overrides and application-wide theming), 'Colors', 'Icons'. Merged into App.xaml for global availability.\" }'**: Modern framework organizational capabilities for UI consistency and theming.\n-   **'\"Services\": { \"type\": \"Folder\", \"description\": \"Contains interfaces and implementations for application services (e.g., INavigationService, IDataService, IDialogService). Promotes testability and separation of business logic from UI concerns. May include an 'IRenderingService' abstracting Vortice.Windows interactions.\" }'**: Ecosystem-aligned structural patterns for Dependency Injection and service locator patterns common in modern .NET applications.\n-   **'\"Infrastructure\": { \"type\": \"Folder\", \"description\": \"Houses cross-cutting concerns like Converters, Behaviors, AttachedProperties, ValidationRules, and CustomCommands. Crucially, a 'Vortice' subfolder within Infrastructure would encapsulate any direct Vortice.Windows interop logic or rendering components, isolating low-level details.\" }'**: Technology-informed file organization for WPF-specific infrastructure and specialized technology integration.\n\n

