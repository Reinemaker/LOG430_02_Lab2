# Corner Shop - Technical Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [Technology Stack](#technology-stack)
3. [Architecture Decisions](#architecture-decisions)
4. [System Design](#system-design)
5. [Development Setup](#development-setup)
6. [Testing](#testing)
7. [Deployment](#deployment)

## Project Overview
Corner Shop is a console-based point-of-sale system designed for small retail businesses. It provides essential features for inventory management, sales processing, and basic reporting.

### Key Features
- Product inventory management
- Sales processing and tracking
- Basic reporting capabilities
- MongoDB integration for data persistence

## Technology Stack

### Core Technologies
- **.NET 8.0**: Chosen for its robust performance, cross-platform capabilities, and modern C# features
- **MongoDB**: Selected for its flexibility with document-based storage and ease of scaling
- **Docker**: Used for containerization to ensure consistent deployment across environments

### Development Tools
- **xUnit**: For unit testing
- **Moq**: For mocking dependencies in tests
- **dotnet-format**: For code formatting and style consistency

### Justification for Technology Choices

#### .NET 8.0
- Modern, cross-platform framework
- Strong typing and compile-time checks
- Excellent performance characteristics
- Rich ecosystem of libraries and tools

#### MongoDB
- Flexible schema design
- Easy to scale horizontally
- Excellent performance for read/write operations
- Native support for JSON-like documents

#### Docker
- Consistent development and deployment environments
- Easy dependency management
- Simplified deployment process
- Isolation of services

## Development Setup

### Prerequisites
- .NET 8.0 SDK
- Docker and Docker Compose
- MongoDB (if running locally without Docker)

### Local Development
1. Clone the repository
2. Navigate to the project directory
3. Run `dotnet restore` to restore dependencies
4. Run `dotnet build` to build the project
5. Run `dotnet test` to execute tests

### Docker Development
1. Build the Docker image:
   ```bash
   docker build -t cornershop .
   ```
2. Run with Docker Compose:
   ```bash
   docker-compose up
   ```

## Testing

### Unit Tests
- Located in `CornerShop.Tests/`
- Run tests using:
  ```bash
  dotnet test
  ```

### Test Coverage
- Using Coverlet for code coverage
- Generate coverage report:
  ```bash
  dotnet test /p:CollectCoverage=true
  ```

## Deployment

### Docker Deployment
1. Build the image:
   ```bash
   docker build -t cornershop .
   ```
2. Run the container:
   ```bash
   docker run -p 27017:27017 cornershop
   ```

### Docker Compose Deployment
1. Start all services:
   ```bash
   docker-compose up -d
   ```
2. Stop all services:
   ```bash
   docker-compose down
   ```

## System Architecture
See the [UML diagrams](UML/) for detailed system architecture views.

## Architecture Decisions
See the [Architecture Decision Records](ADR/) for detailed documentation of key architectural decisions. 