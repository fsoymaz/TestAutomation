# Test Automation Project

A .NET 8.0 test automation project demonstrating CI/CD practices with Jenkins. This project showcases a clean architecture approach with dependency injection, unit testing using xUnit and Moq, and automated deployment pipelines.

## Table of Contents

- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Running Tests](#running-tests)
- [CI/CD Pipeline](#cicd-pipeline)
- [Branch Strategy](#branch-strategy)

## Project Structure

```
TestAutomation/
├── Calculator/
│   ├── Interfaces/
│   │   └── IInventoryRepository.cs
│   ├── Models/
│   │   └── Product.cs
│   └── Services/
│       └── OrderService.cs
├── Calculator.Tests/
│   └── OrderServiceTests.cs
├── Jenkinsfile
├── README.md
├── README_JENKINS.md
└── TestAutomation.sln
```

## Architecture

The project follows a clean architecture pattern with clear separation of concerns:

| Layer | Description |
|-------|-------------|
| **Models** | Domain entities representing business objects |
| **Interfaces** | Abstractions for dependency inversion |
| **Services** | Business logic implementation |
| **Tests** | Unit tests with mocked dependencies |

### Key Components

**OrderService**: Core business logic for order processing with the following validations:
- Order quantity must be greater than zero
- Maximum order limit of 50 items per transaction
- Stock availability verification
- Inventory update confirmation

**IInventoryRepository**: Repository abstraction for data access, enabling testability through dependency injection.

## Prerequisites

- .NET 8.0 SDK or later
- Docker (for Jenkins pipeline execution)
- Jenkins with Docker Pipeline and MSTest plugins (for CI/CD)

## Running Tests

Execute all unit tests:

```bash
dotnet test
```

Run tests with detailed output:

```bash
dotnet test --logger "console;verbosity=detailed"
```

Generate test results in TRX format:

```bash
dotnet test --logger "trx;LogFileName=test-results.trx"
```

## CI/CD Pipeline

The project uses Jenkins for continuous integration and deployment. The pipeline is defined in the `Jenkinsfile` and consists of the following stages:

### Pipeline Stages

| Stage | Trigger | Description |
|-------|---------|-------------|
| **Test** | All branches | Runs unit tests in a Docker container using .NET 8.0 SDK |
| **Promote to Test** | feature/* branches | Merges approved feature branches into the test branch |
| **Deploy to Main** | test branch | Deploys tested code to the main branch (production-ready) |

### Test Stage

Tests are executed within a Docker container to ensure a consistent environment:

```groovy
docker run --rm mcr.microsoft.com/dotnet/sdk:8.0 /bin/sh -c "
    dotnet restore &&
    dotnet build --configuration Release &&
    dotnet test --configuration Release --logger 'trx;LogFileName=test-results.trx'
"
```

### Deployment Considerations

When deploying to the main branch, the following files are excluded from production:
- `Calculator.Tests/` (test project)
- `Jenkinsfile` (CI/CD configuration)
- `README_JENKINS.md` (Jenkins documentation)

The `.gitignore` file is preserved to prevent build artifacts from being committed.

## Branch Strategy

| Branch | Purpose |
|--------|---------|
| `main` | Production-ready code |
| `test` | Integration and testing |
| `feature/*` | Feature development |

### Workflow

1. Create a feature branch from `test`
2. Develop and commit changes
3. Push to trigger automated tests
4. Upon approval, merge to `test` branch
5. After verification, deploy to `main`

## License

This project is provided for educational and demonstration purposes.
