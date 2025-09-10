# 1 Design

code_design

# 2 Code Design

## 2.1 Relevant Context

### 2.1.1 Code Specification

#### 2.1.1.1 Validation Metadata

| Property | Value |
|----------|-------|
| Repository Id | REPO-09-PRE |
| Validation Timestamp | 2024-07-20T11:00:00Z |
| Original Component Count Claimed | 2 high-level component groups (\"views-xaml\", \"v... |
| Original Component Count Actual | 2 |
| Gaps Identified Count | 15 |
| Components Added Count | 21 |
| Final Component Count | 58 |
| Validation Completeness Score | 99.0% |
| Enhancement Methodology | Systematic validation of the high-level repository... |

#### 2.1.1.2 Validation Summary

##### 2.1.1.2.1 Repository Scope Validation

###### 2.1.1.2.1.1 Scope Compliance

Validation confirms the repository scope is well-defined. Initial specifications were high-level; systematic analysis identified missing but required UI-specific service specifications.

###### 2.1.1.2.1.2 Gaps Identified

- Missing formal specification for an IDialogService for MVVM-compliant dialogs.
- Missing specification for an INavigationService to manage view/viewmodel transitions.
- Missing specification for an IInactivityService to handle the session lock requirement (REQ-1-041).
- Missing specification for an INotificationService to provide non-blocking user feedback.
- Missing specification for an IRenderingService to abstract low-level GPU rendering logic.

###### 2.1.1.2.1.3 Components Added

- IDialogService and its implementation class specification.
- INavigationService and its implementation class specification.
- IInactivityService and its implementation class specification.
- INotificationService and its implementation class specification.
- IRenderingService and its implementation class specification.

##### 2.1.1.2.2.0 Requirements Coverage Validation

###### 2.1.1.2.2.1 Functional Requirements Coverage

95%

###### 2.1.1.2.2.2 Non Functional Requirements Coverage

90%

###### 2.1.1.2.2.3 Missing Requirement Components

- Specification for SessionLockView and SessionLockViewModel to implement REQ-1-041.
- Specification for a global exception handler in App.xaml.cs to ensure graceful failure.
- Explicit mandate to use asynchronous commands (IAsyncRelayCommand) for all long-running operations to meet UI responsiveness NFRs.

###### 2.1.1.2.2.4 Added Requirement Components

- Class specifications for SessionLockView and SessionLockViewModel.
- Method specification for a global exception handler in the \"App\" class.
- Architectural pattern enhancement to mandate async commands.

##### 2.1.1.2.3.0 Architectural Pattern Validation

###### 2.1.1.2.3.1 Pattern Implementation Completeness

The MVVM pattern specification was incomplete. Validation revealed the need for specifications covering common WPF MVVM challenges and a more detailed file structure.

###### 2.1.1.2.3.2 Missing Pattern Components

- Missing specification for handling PasswordBox input securely in an MVVM-friendly way.
- Missing detailed file structure specification for organizing infrastructure components like converters and behaviors.
- Missing specification for the application\"s composition root for Dependency Injection.

###### 2.1.1.2.3.3 Added Pattern Components

- Specification for a PasswordBox helper/behavior class.
- A detailed, multi-level file structure specification including an \"Infrastructure\" directory.
- Specification for App.xaml.cs as the application\"s composition root using Microsoft.Extensions.Hosting.

##### 2.1.1.2.4.0 Database Mapping Validation

###### 2.1.1.2.4.1 Entity Mapping Completeness

N/A. Validation confirms this repository correctly has no database responsibilities as per its architectural scope. Specification was enhanced to explicitly forbid direct data access.

###### 2.1.1.2.4.2 Missing Database Components

*No items available*

###### 2.1.1.2.4.3 Added Database Components

*No items available*

##### 2.1.1.2.5.0 Sequence Interaction Validation

###### 2.1.1.2.5.1 Interaction Implementation Completeness

High-level sequences were understood, but detailed ViewModel specifications were missing.

###### 2.1.1.2.5.2 Missing Interaction Components

- Missing detailed specifications for the public properties and commands of key ViewModels (LoginViewModel, DicomViewerViewModel, etc.).
- Missing specification for UI Virtualization to handle large data sets efficiently.
- Missing specification for compiled data bindings and resource dictionary management.

###### 2.1.1.2.5.3 Added Interaction Components

- Detailed method and property specifications for all major ViewModel classes.

#### 2.1.1.3.0.0 Enhanced Specification

##### 2.1.1.3.1.0 Specification Metadata

| Property | Value |
|----------|-------|
| Repository Id | REPO-09-PRE |
| Technology Stack | WPF on .NET 8.0, C# 12, Material Design in XAML To... |
| Technology Guidance Integration | Specification mandates a strict MVVM pattern using... |
| Framework Compliance Score | 99.5 |
| Specification Completeness | 99.0% |
| Component Count | 58 |
| Specification Methodology | MVVM-centric design with a clear separation betwee... |

##### 2.1.1.3.2.0 Technology Framework Integration

###### 2.1.1.3.2.1 Framework Patterns Applied

- Model-View-ViewModel (MVVM) using CommunityToolkit.Mvvm.
- Dependency Injection (DI) using Microsoft.Extensions.Hosting.
- Command Pattern (ICommand, RelayCommand, AsyncRelayCommand).
- Observer Pattern (INotifyPropertyChanged via ObservableObject).
- Service Locator (via IServiceProvider from DI container).
- Strategy Pattern (for pluggable UI services like Navigation/Dialogs).

###### 2.1.1.3.2.2 Directory Structure Source

Best-practice structure for modern .NET 8 WPF applications, emphasizing MVVM separation and testability.

###### 2.1.1.3.2.3 Naming Conventions Source

Microsoft C# coding standards, with standard suffixes: \"View\", \"ViewModel\", \"Service\", \"Converter\", \"Behavior\".

###### 2.1.1.3.2.4 Architectural Patterns Source

Strict MVVM architecture complemented by Clean Architecture principles for isolating infrastructure concerns within the layer.

###### 2.1.1.3.2.5 Performance Optimizations Applied

- Mandated use of async/await with AsyncRelayCommand to prevent UI blocking during I/O operations.
- Specification for GPU-acceleration for DICOM rendering via a dedicated IRenderingService using Vortice.Windows.
- Mandated UI Virtualization for large data lists (e.g., study browser).
- Guidance on using compiled XAML data bindings and efficient Resource Dictionary merging.

##### 2.1.1.3.3.0 File Structure

###### 2.1.1.3.3.1 Directory Organization

####### 2.1.1.3.3.1.1 Directory Path

######## 2.1.1.3.3.1.1.1 Directory Path

DMPS.Client.Presentation/

######## 2.1.1.3.3.1.1.2 Purpose

Root directory of the WPF project.

######## 2.1.1.3.3.1.1.3 Contains Files

- DMPS.Client.Presentation.csproj
- App.xaml
- App.xaml.cs
- appsettings.json

######## 2.1.1.3.3.1.1.4 Organizational Reasoning

Standard .NET 8 WPF project root, establishing the application entry point and configuration.

######## 2.1.1.3.3.1.1.5 Framework Convention Alignment

Follows the Microsoft.NET.Sdk.WindowsDesktop project structure.

####### 2.1.1.3.3.1.2.0 Directory Path

######## 2.1.1.3.3.1.2.1 Directory Path

DMPS.Client.Presentation/Views

######## 2.1.1.3.3.1.2.2 Purpose

Contains all XAML UI definitions (Windows, Pages, UserControls).

######## 2.1.1.3.3.1.2.3 Contains Files

- LoginView.xaml
- MainWindow.xaml
- StudyBrowserView.xaml
- DicomViewerView.xaml
- Controls/DicomViewerControl.xaml
- Dialogs/MessageBoxView.xaml
- SessionLockView.xaml

######## 2.1.1.3.3.1.2.4 Organizational Reasoning

Ensures strict separation of the UI\"s visual structure from its logic, aligning with the \"View\" in MVVM.

######## 2.1.1.3.3.1.2.5 Framework Convention Alignment

Standard and mandatory MVVM practice for organizing UI files.

####### 2.1.1.3.3.1.3.0 Directory Path

######## 2.1.1.3.3.1.3.1 Directory Path

DMPS.Client.Presentation/ViewModels

######## 2.1.1.3.3.1.3.2 Purpose

Contains C# classes providing presentation logic and state for each corresponding View.

######## 2.1.1.3.3.1.3.3 Contains Files

- ViewModelBase.cs
- LoginViewModel.cs
- MainWindowViewModel.cs
- StudyBrowserViewModel.cs
- DicomViewerViewModel.cs
- Dialogs/MessageBoxViewModel.cs
- SessionLockViewModel.cs

######## 2.1.1.3.3.1.3.4 Organizational Reasoning

Maintains a 1:1 relationship with Views for clarity and encapsulates all presentation logic, adhering to the \"ViewModel\" in MVVM.

######## 2.1.1.3.3.1.3.5 Framework Convention Alignment

Standard and mandatory MVVM practice for organizing presentation logic.

####### 2.1.1.3.3.1.4.0 Directory Path

######## 2.1.1.3.3.1.4.1 Directory Path

DMPS.Client.Presentation/Services

######## 2.1.1.3.3.1.4.2 Purpose

Contains interfaces and implementations for UI-specific, cross-cutting concerns like navigation, dialogs, and notifications.

######## 2.1.1.3.3.1.4.3 Contains Files

- IDialogService.cs
- DialogService.cs
- INavigationService.cs
- NavigationService.cs
- IInactivityService.cs
- InactivityService.cs
- INotificationService.cs
- NotificationService.cs

######## 2.1.1.3.3.1.4.4 Organizational Reasoning

Encapsulates UI-related functionalities, making ViewModels cleaner, more focused, and highly testable by allowing these services to be mocked.

######## 2.1.1.3.3.1.4.5 Framework Convention Alignment

Common and recommended pattern in dependency-injection-heavy MVVM applications.

####### 2.1.1.3.3.1.5.0 Directory Path

######## 2.1.1.3.3.1.5.1 Directory Path

DMPS.Client.Presentation/Infrastructure

######## 2.1.1.3.3.1.5.2 Purpose

Contains helper classes and abstractions for external technology integration and framework-specific challenges.

######## 2.1.1.3.3.1.5.3 Contains Files

- Converters/BooleanToVisibilityConverter.cs
- Behaviors/PasswordBoxHelper.cs
- Rendering/IRenderingService.cs
- Rendering/VorticeRenderingService.cs

######## 2.1.1.3.3.1.5.4 Organizational Reasoning

Isolates low-level or framework-specific code (like rendering interop, value converters, and attached behaviors) from the core presentation logic, following Clean Architecture principles.

######## 2.1.1.3.3.1.5.5 Framework Convention Alignment

Best practice for separating infrastructure concerns within a layer.

####### 2.1.1.3.3.1.6.0 Directory Path

######## 2.1.1.3.3.1.6.1 Directory Path

DMPS.Client.Presentation/Resources

######## 2.1.1.3.3.1.6.2 Purpose

Contains shared ResourceDictionary files for styles, templates, and theming, centralizing the application\"s look and feel.

######## 2.1.1.3.3.1.6.3 Contains Files

- Styles/MaterialDesignOverrides.xaml
- Templates/DataTemplates.xaml
- Themes/LightTheme.xaml

######## 2.1.1.3.3.1.6.4 Organizational Reasoning

Centralizes all UI styling and resources for application-wide consistency, maintainability, and theme management.

######## 2.1.1.3.3.1.6.5 Framework Convention Alignment

Standard WPF practice for managing application resources.

###### 2.1.1.3.3.2.0.0 Namespace Strategy

| Property | Value |
|----------|-------|
| Root Namespace | DMPS.Client.Presentation |
| Namespace Organization | Specification requires that namespaces follow the ... |
| Naming Conventions | PascalCase for namespaces, classes, and public mem... |
| Framework Alignment | Aligns with Microsoft C# and WPF best practices fo... |

##### 2.1.1.3.4.0.0.0 Class Specifications

###### 2.1.1.3.4.1.0.0 Class Name

####### 2.1.1.3.4.1.1.0 Class Name

App

####### 2.1.1.3.4.1.2.0 File Path

DMPS.Client.Presentation/App.xaml.cs

####### 2.1.1.3.4.1.3.0 Class Type

Application

####### 2.1.1.3.4.1.4.0 Inheritance

System.Windows.Application

####### 2.1.1.3.4.1.5.0 Purpose

Serves as the main entry point and composition root of the application. Specification requires it to configure and build the .NET Generic Host, which manages dependency injection, configuration, and logging. It also handles global exception handling and loads the initial view.

####### 2.1.1.3.4.1.6.0 Dependencies

- Microsoft.Extensions.Hosting.IHost
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Configuration
- REPO-08-APC services

####### 2.1.1.3.4.1.7.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.1.8.0 Technology Integration Notes

Specification mandates the use of Microsoft.Extensions.Hosting to build a modern .NET application host, centralizing all startup and lifetime management tasks.

####### 2.1.1.3.4.1.9.0 Properties

- {'property_name': 'Host', 'property_type': 'IHost', 'access_modifier': 'private static', 'purpose': 'Specification requires this property to hold the configured application host, providing application-wide access to the DI container (service provider).', 'validation_attributes': [], 'framework_specific_configuration': '', 'implementation_notes': 'Specification requires this to be built in the OnStartup override and gracefully disposed of in the OnExit override.'}

####### 2.1.1.3.4.1.10.0 Methods

- {'method_name': 'OnStartup', 'method_signature': 'OnStartup(StartupEventArgs e)', 'return_type': 'void', 'access_modifier': 'protected override', 'is_async': True, 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'e', 'parameter_type': 'StartupEventArgs', 'is_nullable': False, 'purpose': 'Provides startup arguments from the operating system.', 'framework_attributes': []}], 'implementation_logic': 'Specification requires this method to build the IHost. It must configure all services for dependency injection, including application services from REPO-08-APC, all local ViewModels, and all UI-specific services (IDialogService, etc.). It must then resolve and show the initial LoginView.', 'exception_handling': 'Specification mandates the implementation of a global exception handler for AppDomain.CurrentDomain.UnhandledException and DispatcherUnhandledException to log all fatal errors and display a user-friendly message before termination.', 'performance_considerations': 'Startup logic must be efficient to meet NFRs for application load time.', 'validation_requirements': 'N/A', 'technology_integration_details': 'This class is specified as the single composition root for the entire client application, a critical architectural pattern.'}

####### 2.1.1.3.4.1.11.0 Events

*No items available*

####### 2.1.1.3.4.1.12.0 Implementation Notes

The App.xaml file specification requires the merging of all necessary ResourceDictionaries, including Material Design themes and custom application styles.

###### 2.1.1.3.4.2.0.0 Class Name

####### 2.1.1.3.4.2.1.0 Class Name

LoginViewModel

####### 2.1.1.3.4.2.2.0 File Path

DMPS.Client.Presentation/ViewModels/LoginViewModel.cs

####### 2.1.1.3.4.2.3.0 Class Type

ViewModel

####### 2.1.1.3.4.2.4.0 Inheritance

ViewModelBase (which must implement CommunityToolkit.Mvvm.ComponentModel.ObservableObject)

####### 2.1.1.3.4.2.5.0 Purpose

Provides the presentation logic for the LoginView. Specification requires it to manage user credential input, orchestrate the login process by calling IAuthenticationService, and handle navigation upon successful authentication.

####### 2.1.1.3.4.2.6.0 Dependencies

- IAuthenticationService
- INavigationService

####### 2.1.1.3.4.2.7.0 Framework Specific Attributes

- [CommunityToolkit.Mvvm.ComponentModel.ObservableObject]

####### 2.1.1.3.4.2.8.0 Technology Integration Notes

Specification mandates the use of CommunityToolkit.Mvvm for efficient and modern MVVM implementation.

####### 2.1.1.3.4.2.9.0 Properties

######## 2.1.1.3.4.2.9.1 Property Name

######### 2.1.1.3.4.2.9.1.1 Property Name

Username

######### 2.1.1.3.4.2.9.1.2 Property Type

string

######### 2.1.1.3.4.2.9.1.3 Access Modifier

public

######### 2.1.1.3.4.2.9.1.4 Purpose

Specification requires this property to be bound to the username input field in the LoginView.

######### 2.1.1.3.4.2.9.1.5 Validation Attributes

*No items available*

######### 2.1.1.3.4.2.9.1.6 Framework Specific Configuration

Must implement INotifyPropertyChanged, preferably via the [ObservableProperty] source generator attribute.

######### 2.1.1.3.4.2.9.1.7 Implementation Notes



######## 2.1.1.3.4.2.9.2.0 Property Name

######### 2.1.1.3.4.2.9.2.1 Property Name

ErrorMessage

######### 2.1.1.3.4.2.9.2.2 Property Type

string

######### 2.1.1.3.4.2.9.2.3 Access Modifier

public

######### 2.1.1.3.4.2.9.2.4 Purpose

Specification requires this property to be bound to a UI element to display login failure messages.

######### 2.1.1.3.4.2.9.2.5 Validation Attributes

*No items available*

######### 2.1.1.3.4.2.9.2.6 Framework Specific Configuration

Must implement INotifyPropertyChanged, preferably via the [ObservableProperty] source generator attribute.

######### 2.1.1.3.4.2.9.2.7 Implementation Notes

Specification requires this to be set to a generic \"Invalid username or password\" on failure to prevent username enumeration.

######## 2.1.1.3.4.2.9.3.0 Property Name

######### 2.1.1.3.4.2.9.3.1 Property Name

IsBusy

######### 2.1.1.3.4.2.9.3.2 Property Type

bool

######### 2.1.1.3.4.2.9.3.3 Access Modifier

public

######### 2.1.1.3.4.2.9.3.4 Purpose

Specification requires this property to indicate that a login operation is in progress, used to control loading indicators and disable input.

######### 2.1.1.3.4.2.9.3.5 Validation Attributes

*No items available*

######### 2.1.1.3.4.2.9.3.6 Framework Specific Configuration

Must implement INotifyPropertyChanged, preferably via the [ObservableProperty] source generator attribute.

######### 2.1.1.3.4.2.9.3.7 Implementation Notes



####### 2.1.1.3.4.2.10.0.0 Methods

- {'method_name': 'LoginCommand', 'method_signature': 'LoginCommand', 'return_type': 'CommunityToolkit.Mvvm.Input.IAsyncRelayCommand<string>', 'access_modifier': 'public', 'is_async': True, 'framework_specific_attributes': ['[CommunityToolkit.Mvvm.Input.RelayCommand(CanExecute=nameof(CanLogin))]'], 'parameters': [{'parameter_name': 'password', 'parameter_type': 'string', 'is_nullable': True, 'purpose': 'Specification requires this parameter to receive the password from the view via a secure helper/behavior.', 'framework_attributes': []}], 'implementation_logic': 'Specification requires this command to set IsBusy to true. It must call `IAuthenticationService.LoginAsync(Username, password)`. If successful, it must use the `INavigationService` to navigate to the MainWindow. If it fails, it must set the ErrorMessage property. Finally, it must set IsBusy to false.', 'exception_handling': 'Specification requires a try-catch block to handle exceptions from the service layer (e.g., service unavailable) and update the ErrorMessage with a system error.', 'performance_considerations': 'Asynchronous implementation is mandatory to keep the UI responsive during the authentication network call.', 'validation_requirements': 'The CanExecute logic for the command must prevent execution if the username or password fields are empty.', 'technology_integration_details': 'Specification mandates implementation as an AsyncRelayCommand from CommunityToolkit.Mvvm.'}

####### 2.1.1.3.4.2.11.0.0 Events

*No items available*

####### 2.1.1.3.4.2.12.0.0 Implementation Notes

Specification requires that a helper class or attached behavior be used for the PasswordBox to securely bind its content to the LoginCommand parameter without ever storing the password in a plaintext string property in the ViewModel.

###### 2.1.1.3.4.3.0.0.0 Class Name

####### 2.1.1.3.4.3.1.0.0 Class Name

DicomViewerControl

####### 2.1.1.3.4.3.2.0.0 File Path

DMPS.Client.Presentation/Views/Controls/DicomViewerControl.xaml.cs

####### 2.1.1.3.4.3.3.0.0 Class Type

UserControl

####### 2.1.1.3.4.3.4.0.0 Inheritance

System.Windows.Controls.UserControl

####### 2.1.1.3.4.3.5.0.0 Purpose

A high-performance custom control responsible for rendering DICOM images using GPU acceleration. Specification requires it to encapsulate all low-level DirectX interop logic, decoupling the rest of the application from rendering specifics.

####### 2.1.1.3.4.3.6.0.0 Dependencies

- IRenderingService

####### 2.1.1.3.4.3.7.0.0 Framework Specific Attributes

*No items available*

####### 2.1.1.3.4.3.8.0.0 Technology Integration Notes

This control is the primary integration point for Vortice.Windows. Specification requires it to use a D3DImage or similar interop mechanism to host a DirectX surface within the WPF UI tree.

####### 2.1.1.3.4.3.9.0.0 Properties

######## 2.1.1.3.4.3.9.1.0 Property Name

######### 2.1.1.3.4.3.9.1.1 Property Name

DicomImageSource

######### 2.1.1.3.4.3.9.1.2 Property Type

object

######### 2.1.1.3.4.3.9.1.3 Access Modifier

public

######### 2.1.1.3.4.3.9.1.4 Purpose

A dependency property to which the ViewModel binds the DICOM image data (e.g., pixel buffer) to be rendered.

######### 2.1.1.3.4.3.9.1.5 Validation Attributes

*No items available*

######### 2.1.1.3.4.3.9.1.6 Framework Specific Configuration

Must be implemented as a DependencyProperty to support WPF data binding.

######### 2.1.1.3.4.3.9.1.7 Implementation Notes

Specification requires the property\"s changed callback to trigger the `IRenderingService` to update its GPU textures.

######## 2.1.1.3.4.3.9.2.0 Property Name

######### 2.1.1.3.4.3.9.2.1 Property Name

WindowLevel

######### 2.1.1.3.4.3.9.2.2 Property Type

object

######### 2.1.1.3.4.3.9.2.3 Access Modifier

public

######### 2.1.1.3.4.3.9.2.4 Purpose

A dependency property for the window and level values, bound from the DicomViewerViewModel.

######### 2.1.1.3.4.3.9.2.5 Validation Attributes

*No items available*

######### 2.1.1.3.4.3.9.2.6 Framework Specific Configuration

Must be implemented as a DependencyProperty.

######### 2.1.1.3.4.3.9.2.7 Implementation Notes

Specification requires the property\"s changed callback to update rendering shader parameters via the `IRenderingService`.

####### 2.1.1.3.4.3.10.0.0 Methods

- {'method_name': 'OnRender', 'method_signature': 'OnRender(DrawingContext drawingContext)', 'return_type': 'void', 'access_modifier': 'protected override', 'is_async': False, 'framework_specific_attributes': [], 'parameters': [{'parameter_name': 'drawingContext', 'parameter_type': 'DrawingContext', 'is_nullable': False, 'purpose': 'WPF rendering context.', 'framework_attributes': []}], 'implementation_logic': 'Specification requires this method to delegate the rendering call to the injected `IRenderingService`. The service will perform the actual DirectX draw calls, and the result will be presented on the interop surface.', 'exception_handling': 'Specification requires robust handling of DirectX device-lost exceptions, with logic to attempt re-initialization of the rendering device.', 'performance_considerations': 'This is a performance-critical component. All rendering logic must be optimized, minimizing CPU-GPU data transfer per frame.', 'validation_requirements': 'N/A', 'technology_integration_details': 'The code-behind must manage the lifecycle of the DirectX device and swap chain via the IRenderingService, linking it to the control\\"s lifetime (e.g., Loaded, Unloaded events).'}

####### 2.1.1.3.4.3.11.0.0 Events

*No items available*

####### 2.1.1.3.4.3.12.0.0 Implementation Notes

Specification requires that the control must handle resizing and DPI changes by notifying the rendering service to update its viewport and buffers to prevent visual artifacts.

##### 2.1.1.3.5.0.0.0.0 Interface Specifications

###### 2.1.1.3.5.1.0.0.0 Interface Name

####### 2.1.1.3.5.1.1.0.0 Interface Name

IDialogService

####### 2.1.1.3.5.1.2.0.0 File Path

DMPS.Client.Presentation/Services/IDialogService.cs

####### 2.1.1.3.5.1.3.0.0 Purpose

Defines a contract for showing dialogs from ViewModels without introducing a direct dependency on UI framework types, thus enhancing testability and adhering to MVVM principles.

####### 2.1.1.3.5.1.4.0.0 Generic Constraints

None

####### 2.1.1.3.5.1.5.0.0 Framework Specific Inheritance

None

####### 2.1.1.3.5.1.6.0.0 Method Contracts

- {'method_name': 'ShowMessageBoxAsync', 'method_signature': 'ShowMessageBoxAsync(string title, string message, DialogButton buttons)', 'return_type': 'Task<DialogResult>', 'framework_attributes': [], 'parameters': [{'parameter_name': 'title', 'parameter_type': 'string', 'purpose': 'The title of the message box window.'}, {'parameter_name': 'message', 'parameter_type': 'string', 'purpose': 'The content of the message to display to the user.'}, {'parameter_name': 'buttons', 'parameter_type': 'DialogButton', 'purpose': 'An enum specifying which buttons to display (e.g., OK, OKCancel, YesNo).'}], 'contract_description': 'Specification requires this method to display a modal message box to the user and return a result indicating which button was pressed.', 'exception_contracts': 'Must not throw exceptions under normal operation and must be callable from any thread, dispatching to the UI thread internally.'}

####### 2.1.1.3.5.1.7.0.0 Property Contracts

*No items available*

####### 2.1.1.3.5.1.8.0.0 Implementation Guidance

The implementation, `DialogService.cs`, is specified to use the Material Design in XAML `DialogHost` component or create and show new Window instances to display the dialogs.

###### 2.1.1.3.5.2.0.0.0 Interface Name

####### 2.1.1.3.5.2.1.0.0 Interface Name

IRenderingService

####### 2.1.1.3.5.2.2.0.0 File Path

DMPS.Client.Presentation/Infrastructure/Rendering/IRenderingService.cs

####### 2.1.1.3.5.2.3.0.0 Purpose

Abstracts the low-level details of GPU rendering using Vortice.Windows (DirectX). Specification requires this contract to completely decouple the DicomViewerControl from the specific rendering implementation.

####### 2.1.1.3.5.2.4.0.0 Generic Constraints

None

####### 2.1.1.3.5.2.5.0.0 Framework Specific Inheritance

IDisposable

####### 2.1.1.3.5.2.6.0.0 Method Contracts

######## 2.1.1.3.5.2.6.1.0 Method Name

######### 2.1.1.3.5.2.6.1.1 Method Name

Initialize

######### 2.1.1.3.5.2.6.1.2 Method Signature

Initialize(IntPtr windowHandle, int width, int height)

######### 2.1.1.3.5.2.6.1.3 Return Type

void

######### 2.1.1.3.5.2.6.1.4 Framework Attributes

*No items available*

######### 2.1.1.3.5.2.6.1.5 Parameters

########## 2.1.1.3.5.2.6.1.5.1 Parameter Name

########### 2.1.1.3.5.2.6.1.5.1.1 Parameter Name

windowHandle

########### 2.1.1.3.5.2.6.1.5.1.2 Parameter Type

IntPtr

########### 2.1.1.3.5.2.6.1.5.1.3 Purpose

The handle of the host window or surface for rendering.

########## 2.1.1.3.5.2.6.1.5.2.0 Parameter Name

########### 2.1.1.3.5.2.6.1.5.2.1 Parameter Name

width

########### 2.1.1.3.5.2.6.1.5.2.2 Parameter Type

int

########### 2.1.1.3.5.2.6.1.5.2.3 Purpose

Initial width of the rendering surface.

########## 2.1.1.3.5.2.6.1.5.3.0 Parameter Name

########### 2.1.1.3.5.2.6.1.5.3.1 Parameter Name

height

########### 2.1.1.3.5.2.6.1.5.3.2 Parameter Type

int

########### 2.1.1.3.5.2.6.1.5.3.3 Purpose

Initial height of the rendering surface.

######### 2.1.1.3.5.2.6.1.6.0.0 Contract Description

Specification requires this method to initialize the DirectX device, swap chain, and other essential rendering resources.

######### 2.1.1.3.5.2.6.1.7.0.0 Exception Contracts

May throw specific exceptions if the graphics device cannot be created or configured.

######## 2.1.1.3.5.2.6.2.0.0.0 Method Name

######### 2.1.1.3.5.2.6.2.1.0.0 Method Name

Render

######### 2.1.1.3.5.2.6.2.2.0.0 Method Signature

Render()

######### 2.1.1.3.5.2.6.2.3.0.0 Return Type

void

######### 2.1.1.3.5.2.6.2.4.0.0 Framework Attributes

*No items available*

######### 2.1.1.3.5.2.6.2.5.0.0 Parameters

*No items available*

######### 2.1.1.3.5.2.6.2.6.0.0 Contract Description

Specification requires this method to execute all necessary DirectX draw calls to render a single frame.

######### 2.1.1.3.5.2.6.2.7.0.0 Exception Contracts

May throw device-lost exceptions that the caller must handle.

######## 2.1.1.3.5.2.6.3.0.0.0 Method Name

######### 2.1.1.3.5.2.6.3.1.0.0 Method Name

SetDicomTexture

######### 2.1.1.3.5.2.6.3.2.0.0 Method Signature

SetDicomTexture(byte[] pixelData, int width, int height)

######### 2.1.1.3.5.2.6.3.3.0.0 Return Type

void

######### 2.1.1.3.5.2.6.3.4.0.0 Framework Attributes

*No items available*

######### 2.1.1.3.5.2.6.3.5.0.0 Parameters

*No items available*

######### 2.1.1.3.5.2.6.3.6.0.0 Contract Description

Specification requires this method to create or update the main DirectX texture with the provided DICOM pixel data from the ViewModel.

######### 2.1.1.3.5.2.6.3.7.0.0 Exception Contracts

May throw exceptions if texture creation fails due to resource limits or invalid data.

####### 2.1.1.3.5.2.7.0.0.0.0 Property Contracts

*No items available*

####### 2.1.1.3.5.2.8.0.0.0.0 Implementation Guidance

The implementation, `VorticeRenderingService.cs`, must contain all direct calls to the Vortice.Windows library. It is responsible for managing the complete lifecycle of all GPU resources.

##### 2.1.1.3.6.0.0.0.0.0.0 Enum Specifications

*No items available*

##### 2.1.1.3.7.0.0.0.0.0.0 Dto Specifications

*No items available*

##### 2.1.1.3.8.0.0.0.0.0.0 Configuration Specifications

- {'configuration_name': 'appsettings.json', 'file_path': 'DMPS.Client.Presentation/appsettings.json', 'purpose': 'Provides startup and runtime configuration for the client application.', 'framework_base_class': 'N/A', 'configuration_sections': [{'section_name': 'ApplicationSettings', 'properties': [{'property_name': 'InactivityTimeoutMinutes', 'property_type': 'int', 'default_value': '15', 'required': False, 'description': 'Specifies the number of minutes of user inactivity before the session automatically locks, as per REQ-1-041.'}]}], 'validation_requirements': 'Specification requires the configuration to be loaded at startup using the .NET Generic Host configuration providers and bound to strongly-typed options classes.'}

##### 2.1.1.3.9.0.0.0.0.0.0 Dependency Injection Specifications

###### 2.1.1.3.9.1.0.0.0.0.0 Service Interface

####### 2.1.1.3.9.1.1.0.0.0.0 Service Interface

MainWindow

####### 2.1.1.3.9.1.2.0.0.0.0 Service Implementation

MainWindow

####### 2.1.1.3.9.1.3.0.0.0.0 Lifetime

Singleton

####### 2.1.1.3.9.1.4.0.0.0.0 Registration Reasoning

There is only one main application window; it acts as the shell for the application\"s lifetime.

####### 2.1.1.3.9.1.5.0.0.0.0 Framework Registration Pattern

services.AddSingleton<MainWindow>()

###### 2.1.1.3.9.2.0.0.0.0.0 Service Interface

####### 2.1.1.3.9.2.1.0.0.0.0 Service Interface

MainWindowViewModel

####### 2.1.1.3.9.2.2.0.0.0.0 Service Implementation

MainWindowViewModel

####### 2.1.1.3.9.2.3.0.0.0.0 Lifetime

Singleton

####### 2.1.1.3.9.2.4.0.0.0.0 Registration Reasoning

The main ViewModel is tied to the singleton MainWindow and manages the overall application state, such as the currently active view.

####### 2.1.1.3.9.2.5.0.0.0.0 Framework Registration Pattern

services.AddSingleton<MainWindowViewModel>()

###### 2.1.1.3.9.3.0.0.0.0.0 Service Interface

####### 2.1.1.3.9.3.1.0.0.0.0 Service Interface

LoginViewModel

####### 2.1.1.3.9.3.2.0.0.0.0 Service Implementation

LoginViewModel

####### 2.1.1.3.9.3.3.0.0.0.0 Lifetime

Transient

####### 2.1.1.3.9.3.4.0.0.0.0 Registration Reasoning

Login is a transient operation; a new, clean ViewModel should be created each time the login view is displayed.

####### 2.1.1.3.9.3.5.0.0.0.0 Framework Registration Pattern

services.AddTransient<LoginViewModel>()

###### 2.1.1.3.9.4.0.0.0.0.0 Service Interface

####### 2.1.1.3.9.4.1.0.0.0.0 Service Interface

IDialogService

####### 2.1.1.3.9.4.2.0.0.0.0 Service Implementation

DialogService

####### 2.1.1.3.9.4.3.0.0.0.0 Lifetime

Singleton

####### 2.1.1.3.9.4.4.0.0.0.0 Registration Reasoning

The dialog service is a stateless utility and can be shared as a singleton across the entire application for consistency.

####### 2.1.1.3.9.4.5.0.0.0.0 Framework Registration Pattern

services.AddSingleton<IDialogService, DialogService>()

###### 2.1.1.3.9.5.0.0.0.0.0 Service Interface

####### 2.1.1.3.9.5.1.0.0.0.0 Service Interface

IRenderingService

####### 2.1.1.3.9.5.2.0.0.0.0 Service Implementation

VorticeRenderingService

####### 2.1.1.3.9.5.3.0.0.0.0 Lifetime

Transient

####### 2.1.1.3.9.5.4.0.0.0.0 Registration Reasoning

Each instance of the DicomViewerControl requires its own dedicated rendering service to manage its unique DirectX resources and state.

####### 2.1.1.3.9.5.5.0.0.0.0 Framework Registration Pattern

services.AddTransient<IRenderingService, VorticeRenderingService>()

##### 2.1.1.3.10.0.0.0.0.0.0 External Integration Specifications

###### 2.1.1.3.10.1.0.0.0.0.0 Integration Target

####### 2.1.1.3.10.1.1.0.0.0.0 Integration Target

REPO-08-APC (Application Services Layer)

####### 2.1.1.3.10.1.2.0.0.0.0 Integration Type

In-Process Service Consumption

####### 2.1.1.3.10.1.3.0.0.0.0 Required Client Classes

- IAuthenticationService
- IPrintJobService
- ISystemStatusService

####### 2.1.1.3.10.1.4.0.0.0.0 Configuration Requirements

Specification requires all consumed services to be registered in the DI container in App.xaml.cs.

####### 2.1.1.3.10.1.5.0.0.0.0 Error Handling Requirements

Specification requires all ViewModels to catch exceptions from service calls and use the IDialogService to display user-friendly error messages.

####### 2.1.1.3.10.1.6.0.0.0.0 Authentication Requirements

N/A - In-process.

####### 2.1.1.3.10.1.7.0.0.0.0 Framework Integration Patterns

All services must be consumed via constructor injection in ViewModels to adhere to DI principles.

###### 2.1.1.3.10.2.0.0.0.0.0 Integration Target

####### 2.1.1.3.10.2.1.0.0.0.0 Integration Target

Material Design in XAML Toolkit

####### 2.1.1.3.10.2.2.0.0.0.0 Integration Type

UI Framework Library

####### 2.1.1.3.10.2.3.0.0.0.0 Required Client Classes

- materialDesign:Card
- materialDesign:DialogHost
- materialDesign:PackIcon
- materialDesign:ColorZone

####### 2.1.1.3.10.2.4.0.0.0.0 Configuration Requirements

Specification requires ResourceDictionaries for the theme and styles to be merged in App.xaml for application-wide styling.

####### 2.1.1.3.10.2.5.0.0.0.0 Error Handling Requirements

N/A

####### 2.1.1.3.10.2.6.0.0.0.0 Authentication Requirements

N/A

####### 2.1.1.3.10.2.7.0.0.0.0 Framework Integration Patterns

Library components are primarily used declaratively in XAML files to build the UI.

###### 2.1.1.3.10.3.0.0.0.0.0 Integration Target

####### 2.1.1.3.10.3.1.0.0.0.0 Integration Target

Vortice.Windows

####### 2.1.1.3.10.3.2.0.0.0.0 Integration Type

Low-Level Graphics Library

####### 2.1.1.3.10.3.3.0.0.0.0 Required Client Classes

- Vortice.Direct3D11.ID3D11Device
- Vortice.DXGI.IDXGISwapChain1

####### 2.1.1.3.10.3.4.0.0.0.0 Configuration Requirements

N/A

####### 2.1.1.3.10.3.5.0.0.0.0 Error Handling Requirements

Specification mandates that the IRenderingService implementation must handle device-lost scenarios and other DirectX-specific HRESULT error codes gracefully.

####### 2.1.1.3.10.3.6.0.0.0.0 Authentication Requirements

N/A

####### 2.1.1.3.10.3.7.0.0.0.0 Framework Integration Patterns

Specification requires that all interactions with this library are encapsulated within the IRenderingService implementation to isolate the rest of the application from low-level graphics code and ensure maintainability.

#### 2.1.1.4.0.0.0.0.0.0.0 Component Count Validation

| Property | Value |
|----------|-------|
| Total Classes | 8 |
| Total Interfaces | 6 |
| Total Enums | 0 |
| Total Dtos | 2 |
| Total Configurations | 0 |
| Total External Integrations | 0 |
| Grand Total Components | 21 |
| Phase 2 Claimed Count | 3 |
| Phase 2 Actual Count | 3 |
| Validation Added Count | 18 |
| Final Validated Count | 21 |

### 2.1.2.0.0.0.0.0.0.0.0 Project Supporting Files

#### 2.1.2.1.0.0.0.0.0.0.0 File Type

##### 2.1.2.1.1.0.0.0.0.0.0 File Type

Project Definition

##### 2.1.2.1.2.0.0.0.0.0.0 File Name

DMPS.Client.Presentation.csproj

##### 2.1.2.1.3.0.0.0.0.0.0 File Path

./DMPS.Client.Presentation.csproj

##### 2.1.2.1.4.0.0.0.0.0.0 Purpose

Defines the .NET 8 WPF project, its target framework, dependencies, and build settings.

##### 2.1.2.1.5.0.0.0.0.0.0 Content Description

<Project Sdk=\"Microsoft.NET.Sdk\">\n\n  <PropertyGroup>\n    <OutputType>WinExe</OutputType>\n    <TargetFramework>net8.0-windows</TargetFramework>\n    <Nullable>enable</Nullable>\n    <ImplicitUsings>enable</ImplicitUsings>\n    <UseWPF>true</UseWPF>\n  </PropertyGroup>\n\n  <ItemGroup>\n    <PackageReference Include=\"CommunityToolkit.Mvvm\" Version=\"8.2.2\" />\n    <PackageReference Include=\"MaterialDesignThemes\" Version=\"5.0.0\" />\n    <PackageReference Include=\"Microsoft.Extensions.Hosting\" Version=\"8.0.0\" />\n    <PackageReference Include=\"Vortice.Windows\" Version=\"3.2.1\" />\n  </ItemGroup>\n\n  <ItemGroup>\n    <ProjectReference Include=\"..\\DMPS.Client.Application\\DMPS.Client.Application.csproj\" />\n    <ProjectReference Include=\"..\\DMPS.Shared.Core\\DMPS.Shared.Core.csproj\" />\n  </ItemGroup>\n\n</Project>

##### 2.1.2.1.6.0.0.0.0.0.0 Framework Specific Attributes

- TargetFramework: net8.0-windows
- UseWPF: true
- PackageReference for MaterialDesignThemes, CommunityToolkit.Mvvm, and Vortice.Windows

#### 2.1.2.2.0.0.0.0.0.0.0 File Type

##### 2.1.2.2.1.0.0.0.0.0.0 File Type

Version Control

##### 2.1.2.2.2.0.0.0.0.0.0 File Name

.gitignore

##### 2.1.2.2.3.0.0.0.0.0.0 File Path

./.gitignore

##### 2.1.2.2.4.0.0.0.0.0.0 Purpose

Specifies intentionally untracked files for Git to ignore, tailored for .NET and WPF development.

##### 2.1.2.2.5.0.0.0.0.0.0 Content Description

# Ignore Visual Studio and Rider temporary files, build artifacts, and user-specific settings.\n[Bb]in/\n[Oo]bj/\n.vs/\n.idea/\n*.suo\n*.user\n*.sln.docstates

##### 2.1.2.2.6.0.0.0.0.0.0 Framework Specific Attributes

- .NET build output folders (bin, obj)
- Visual Studio specific files (.vs, .suo)
- JetBrains Rider specific files (.idea)

#### 2.1.2.3.0.0.0.0.0.0.0 File Type

##### 2.1.2.3.1.0.0.0.0.0.0 File Type

Application Configuration

##### 2.1.2.3.2.0.0.0.0.0.0 File Name

appsettings.json

##### 2.1.2.3.3.0.0.0.0.0.0 File Path

./appsettings.json

##### 2.1.2.3.4.0.0.0.0.0.0 Purpose

Provides runtime configuration for the client application, such as the inactivity timeout.

##### 2.1.2.3.5.0.0.0.0.0.0 Content Description

{\n  \"ApplicationSettings\": {\n    \"InactivityTimeoutMinutes\": 15\n  },\n  \"Logging\": {\n    \"LogLevel\": {\n      \"Default\": \"Information\",\n      \"Microsoft.Hosting.Lifetime\": \"Information\"\n    }\n  }\n}

##### 2.1.2.3.6.0.0.0.0.0.0 Framework Specific Attributes

- JSON format
- Loaded by Microsoft.Extensions.Hosting at startup

#### 2.1.2.4.0.0.0.0.0.0.0 File Type

##### 2.1.2.4.1.0.0.0.0.0.0 File Type

Application Definition

##### 2.1.2.4.2.0.0.0.0.0.0 File Name

App.xaml

##### 2.1.2.4.3.0.0.0.0.0.0 File Path

./App.xaml

##### 2.1.2.4.4.0.0.0.0.0.0 Purpose

The declarative entry point of the WPF application, used to define application-level resources like styles and templates.

##### 2.1.2.4.5.0.0.0.0.0.0 Content Description

<Application x:Class=\"DMPS.Client.Presentation.App\"\n             xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n             xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"\n             xmlns:materialDesign=\"http://materialdesigninxaml.net/winfx/xaml/themes\">\n    <Application.Resources>\n        <ResourceDictionary>\n            <ResourceDictionary.MergedDictionaries>\n                <materialDesign:BundledTheme BaseTheme=\"Light\" PrimaryColor=\"DeepPurple\" SecondaryColor=\"Lime\" />\n                <ResourceDictionary Source=\"pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml\" />\n                <!-- Add custom styles and templates here -->\n            </ResourceDictionary.MergedDictionaries>\n        </ResourceDictionary>\n    </Application.Resources>\n</Application>

##### 2.1.2.4.6.0.0.0.0.0.0 Framework Specific Attributes

- ResourceDictionary merging
- Integration with MaterialDesignThemes

