# Specification

# 1. Technologies

## 1.1. .NET 8 / C# 12
### 1.1.3. Version
.NET 8 (LTS), C# 12

### 1.1.4. Category
Core Platform & Language

### 1.1.5. Features

- High-performance, cross-platform runtime for client and service
- Long-Term Support (LTS) version ensuring stability and support
- Modern language features like async/await for UI responsiveness

### 1.1.6. Requirements

- REQ-1-008
- REQ-1-013
- REQ-1-079

### 1.1.7. Configuration


### 1.1.8. License

- **Type:** MIT
- **Cost:** Free

## 1.2. Windows Presentation Foundation (WPF)
### 1.2.3. Version
Part of .NET 8

### 1.2.4. Category
Desktop UI Framework

### 1.2.5. Features

- Modern framework for building Windows desktop applications
- Powerful data binding system, ideal for the MVVM pattern
- XAML-based declarative UI definition

### 1.2.6. Requirements

- REQ-1-001
- REQ-1-008
- REQ-1-013

### 1.2.7. Configuration


### 1.2.8. License

- **Type:** MIT
- **Cost:** Free

## 1.3. Microsoft.Extensions (Hosting, DependencyInjection)
### 1.3.3. Version
8.0.x

### 1.3.4. Category
Application Framework

### 1.3.5. Features

- Generic Host builder for robust Windows Service implementation
- Built-in Dependency Injection container for loose coupling
- Handles application configuration and lifecycle management

### 1.3.6. Requirements

- REQ-1-013

### 1.3.7. Configuration


### 1.3.8. License

- **Type:** MIT
- **Cost:** Free

## 1.4. PostgreSQL
### 1.4.3. Version
16

### 1.4.4. Category
Database Server

### 1.4.5. Features

- Robust, open-source relational database for storing study metadata
- ACID compliant, ensuring data integrity for critical health information
- Supports advanced features like the pgcrypto extension for PHI encryption

### 1.4.6. Requirements

- REQ-1-002
- REQ-1-010
- REQ-1-013
- REQ-1-083

### 1.4.7. Configuration

- **Notes:** The 'pgcrypto' extension must be enabled on the database, as verified by the installer (REQ-1-076).

### 1.4.8. License

- **Type:** PostgreSQL License (Open Source)
- **Cost:** Free

## 1.5. Entity Framework Core
### 1.5.3. Version
8.0.x

### 1.5.4. Category
Data Access (ORM)

### 1.5.5. Features

- Modern Object-Relational Mapper (ORM) for .NET
- Simplifies data access logic using C# objects and LINQ
- Manages database connections, transactions, and schema migrations

### 1.5.6. Requirements

- REQ-1-013

### 1.5.7. Configuration

- **Notes:** Will be used with the Npgsql.EntityFrameworkCore.PostgreSQL provider to connect to PostgreSQL.

### 1.5.8. License

- **Type:** Apache 2.0
- **Cost:** Free

## 1.6. RabbitMQ
### 1.6.3. Version
3.12.x or later (Server)

### 1.6.4. Category
Message Broker

### 1.6.5. Features

- Decouples the WPF client from the Windows Service for asynchronous tasks
- Supports durable queues and persistent messages for reliability
- Implements dead-letter exchanges for robust error handling

### 1.6.6. Requirements

- REQ-1-002
- REQ-1-004
- REQ-1-005
- REQ-1-006
- REQ-1-013

### 1.6.7. Configuration


### 1.6.8. License

- **Type:** Mozilla Public License 2.0
- **Cost:** Free

## 1.7. fo-dicom
### 1.7.3. Version
5.1.x

### 1.7.4. Category
Domain-Specific Library (Medical Imaging)

### 1.7.5. Features

- Comprehensive library for all DICOM-related functionality
- Implements C-STORE SCP, C-FIND/C-MOVE SCU
- Handles DICOM metadata parsing, editing, and image rendering

### 1.7.6. Requirements

- REQ-1-009
- REQ-1-013

### 1.7.7. Configuration


### 1.7.8. License

- **Type:** Microsoft Public License (MS-PL)
- **Cost:** Free

## 1.8. Serilog
### 1.8.3. Version
3.1.x

### 1.8.4. Category
Logging Framework

### 1.8.5. Features

- Structured logging for rich, queryable log data
- Flexible sink architecture to output to files and Windows Event Log
- Supports log enrichment with correlation IDs for tracing operations

### 1.8.6. Requirements

- REQ-1-013
- REQ-1-039
- REQ-1-090

### 1.8.7. Configuration

- **Notes:** Will be configured with sinks for rolling file and Windows Event Log. PHI redaction will be implemented via a custom enricher or formatter.

### 1.8.8. License

- **Type:** Apache 2.0
- **Cost:** Free

## 1.9. QuestPDF
### 1.9.3. Version
2024.3.x

### 1.9.4. Category
Document Generation Library

### 1.9.5. Features

- Fluent C# API for composing PDF documents from code
- Generates PDF/A-3 compliant files as required
- Handles complex layouts, text, and image rendering for print exports

### 1.9.6. Requirements

- REQ-1-013
- REQ-1-029

### 1.9.7. Configuration


### 1.9.8. License

- **Type:** Dual: MIT (Community) / Commercial (Pro)
- **Cost:** Free or Paid

## 1.10. Material Design in XAML Toolkit
### 1.10.3. Version
5.0.x

### 1.10.4. Category
UI Component Library

### 1.10.5. Features

- Provides a modern, intuitive UI based on Google's Material Design
- Includes built-in support for Light/Dark themes and accessibility
- Offers a comprehensive set of pre-styled controls for rapid UI development

### 1.10.6. Requirements

- REQ-1-013
- REQ-1-070

### 1.10.7. Configuration


### 1.10.8. License

- **Type:** MIT
- **Cost:** Free

## 1.11. Vortice.Windows
### 1.11.3. Version
3.2.x

### 1.11.4. Category
Graphics Library (.NET Wrapper)

### 1.11.5. Features

- Low-level, high-performance DirectX bindings for .NET
- Enables GPU-accelerated rendering for the DICOM viewer
- Crucial for meeting performance targets for large image series

### 1.11.6. Requirements

- REQ-1-013
- REQ-1-052

### 1.11.7. Configuration


### 1.11.8. License

- **Type:** MIT
- **Cost:** Free

## 1.12. System.IO.Pipes (Named Pipes)
### 1.12.3. Version
Part of .NET 8

### 1.12.4. Category
Inter-Process Communication (IPC) Framework

### 1.12.5. Features

- Provides fast, synchronous communication between the client and service on the same machine
- Used exclusively for real-time status checks (e.g., service availability)
- Built into the .NET Base Class Library (BCL)

### 1.12.6. Requirements

- REQ-1-007
- REQ-1-013
- REQ-1-021

### 1.12.7. Configuration


### 1.12.8. License

- **Type:** MIT
- **Cost:** Free

## 1.13. BCrypt.Net-Next
### 1.13.3. Version
4.0.x

### 1.13.4. Category
Security Library

### 1.13.5. Features

- Implements the industry-standard BCrypt algorithm for password hashing
- Automatically handles salt generation to protect against rainbow table attacks
- Provides secure hashing and verification methods

### 1.13.6. Requirements

- REQ-1-082

### 1.13.7. Configuration


### 1.13.8. License

- **Type:** MIT
- **Cost:** Free

## 1.14. Windows Credential Manager
### 1.14.3. Version
OS Component

### 1.14.4. Category
Security API

### 1.14.5. Features

- Provides a secure, OS-level storage for sensitive data like passwords and API keys
- Prevents storing secrets in plaintext configuration files
- Accessible via .NET APIs for secure retrieval

### 1.14.6. Requirements

- REQ-1-084

### 1.14.7. Configuration


### 1.14.8. License

- **Type:** Proprietary (Part of Windows OS)
- **Cost:** Free

## 1.15. MSIX
### 1.15.3. Version
N/A

### 1.15.4. Category
Deployment Technology

### 1.15.5. Features

- Modern Windows application packaging format for reliable installation and uninstallation
- Supports prerequisite checks for external dependencies like PostgreSQL and RabbitMQ
- Enables application signing to ensure integrity and authenticity

### 1.15.6. Requirements

- REQ-1-013
- REQ-1-076

### 1.15.7. Configuration


### 1.15.8. License

- **Type:** Part of Windows SDK
- **Cost:** Free



---

# 2. Configuration



---

