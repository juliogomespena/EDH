# Entrepreneur Digital Hub (EDH)
## 📋 Overview

The Entrepreneur Digital Hub (EDH) is a desktop application designed to provide microentrepreneurs with limited technology access and experience, a simple business management system.

### [PRD Simplified / ADR](https://1drv.ms/f/c/fb6b4ab555c1063e/EmJqTJxLTY1BrY-d8lEOtQkBndWn69qWH2rSaxwVNm70fg?e=Xzi9bD)
### [Diagrams and Stories Map](https://miro.com/app/board/uXjVIDdbgWQ=/?share_link_id=517581523863)
<br>

Key features include:
- Product/Service management
- Inventory tracking
- Sales processing
- Financial management
- Simple reporting
- User-friendly interface

## 🏗️ Architecture

EDH follows a Modular Monolith architecture with Clean Architecture principles:

- **Application Type**: WPF (Windows Presentation Foundation) desktop application
- **UI Design**: Material Design for XAML
- **Design Pattern**: MVVM (Model-View-ViewModel) using Prism Framework
- **Data Storage**: Entity Framework Core with SQLite
- **Module Communication**: Event Aggregator pattern using Prism's EventAggregator

### Matrix Structure Architecture

The application is organized using a combination of technical layers (horizontal slices) and business domains (vertical slices), creating a matrix structure:

#### Technical Layers (Horizontal Slices)
1. **Core Layer**: Domain entities, interfaces, value objects, business rules
2. **Infrastructure Layer**: Data access, external service integrations, cross-cutting concerns
3. **Application Layer**: Application services, DTOs, validation, workflows
4. **Presentation Layer**: ViewModels, Commands, Navigation services, UI components
5. **Shell**: Application bootstrapping, module composition, main window container

#### Business Modules (Vertical Slices)
1. **Items**: Products/Services management
2. **Inventory**: Inventory management
3. **Sales**: Sales processing
4. **Financial**: Financial management
5. **Reports**: Reporting
6. **Settings**: System configuration

## 📂 Project Structure

```
EDH.sln
├── 0-Shared
│   ├── EDH.Common
│   │   └── (Shared utilities, helpers, base classes)
│   └── EDH.Core
│       └── (Shared domain models, interfaces, etc.)
│
├── 1-Modules
│   ├── EDH.Items
│   │   ├── EDH.Items.Core
│   │   │   └── (Domain models, interfaces, logic)
│   │   ├── EDH.Items.Infrastructure
│   │   │   └── (Repositories, data access)
│   │   ├── EDH.Items.Application
│   │   │   └── (Services, use cases, DTOs)
│   │   └── EDH.Items.Presentation
│   │       └── (ViewModels, Views, UI logic)
│   │
│   ├── EDH.Inventory
│   │   ├── EDH.Inventory.Core
│   │   ├── EDH.Inventory.Infrastructure
│   │   ├── EDH.Inventory.Application
│   │   └── EDH.Inventory.Presentation
│   │
│   ├── EDH.Sales
│   │   ├── EDH.Sales.Core
│   │   ├── EDH.Sales.Infrastructure
│   │   ├── EDH.Sales.Application
│   │   └── EDH.Sales.Presentation
│   │
│   ├── EDH.Financial
│   │   ├── EDH.Financial.Core
│   │   ├── EDH.Financial.Infrastructure
│   │   ├── EDH.Financial.Application
│   │   └── EDH.Financial.Presentation
│   │
│   ├── EDH.Reports
│   │   ├── EDH.Reports.Core
│   │   ├── EDH.Reports.Infrastructure
│   │   ├── EDH.Reports.Application
│   │   └── EDH.Reports.Presentation
│   │
│   └── EDH.Settings
│       ├── EDH.Settings.Core
│       ├── EDH.Settings.Infrastructure
│       ├── EDH.Settings.Application
│       └── EDH.Settings.Presentation
│
├── 2-Infrastructure
│   ├── EDH.Infrastructure.Data
│   │   └── (Shared database context, migrations)
│   └── EDH.Infrastructure.Common
│       └── (Logging, authentication, etc.)
│
└── 3-Presentation
    ├── EDH.Shell
    │   └── (Main application, composition root)
    └── EDH.Presentation.Common
        └── (Shared UI resources, templates)
```

## 🚀 Getting Started

### Prerequisites
- [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or another compatible IDE

### Building the Application
1. Clone the repository
   ```
   git clone https://github.com/juliogomespena/EDH.git
   ```
2. Open the solution file `EDH.sln` in Visual Studio
3. Restore NuGet packages
4. Build the solution
5. Run the application

## 🧩 Development Principles

- SOLID principles
- DDD principles
- Strong emphasis on simplicity and maintainability
- Good practices
- Clear dependency direction (dependencies point inward)

## 🔄 Module Communication

EDH uses the Event Aggregator pattern for communication between modules:
- Modules publish events when state changes
- Interested modules subscribe to relevant events
- Events are defined in the Core layer
- Direct service calls are used only when truly necessary

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
