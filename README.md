# Corner Shop Management System

A comprehensive point-of-sale system for managing a corner shop, built with .NET Core and supporting both MongoDB and Entity Framework Core (SQLite) databases.

## Documentation

- [Architecture Documentation](docs/architecture.md) - Technical architecture and design decisions
- [Usage Guide](docs/usage.md) - How to use the application
- [Development Guide](docs/development.md) - Guide for developers
- [Architecture Decision Records](docs/ADR/) - Technical decisions and their rationale
- [UML Diagrams](docs/UML/) - System design and architecture views

## System Architecture

### 2-Tier Architecture
- **Client Tier**: Console application (.NET Core)
- **Server Tier**: Dual database system
  - MongoDB (NoSQL)
  - SQLite with Entity Framework Core (RDBMS)

### Key Components
- **Presentation Layer**: Console interface (Program.cs)
- **Business Layer**: Services for business logic
- **Data Layer**: Database services and synchronization

## Features

### Product Management
- Search products with case-insensitive matching
- Check stock levels with category grouping
- Stock status indicators:
  - Out of Stock (0)
  - Low Stock (1-5)
  - In Stock (>5)
- Automatic stock updates on sales

### Sales Management
- Create sales with multiple items
- Real-time stock validation
- Sale confirmation with detailed summary
- Cancel sales with automatic stock restoration
- View recent sales with detailed information
- Support for 3 concurrent registers

### Database Features
- Dual database support:
  - MongoDB for document-based storage
  - Entity Framework Core with SQLite for relational storage
- Automatic database synchronization
- Easy switching between databases
- Transaction management
- Data consistency checks

### User Interface
- Clear menu navigation
- Detailed prompts and feedback
- Input validation and error handling
- Confirmation steps for important actions
- Role-based access (Cashier/Manager)

## Getting Started

### Prerequisites
- .NET 6.0 or later
- MongoDB (for MongoDB database option)
- SQLite (included with EF Core)
- Docker and Docker Compose (for containerized deployment)

### Installation

#### Local Development
1. Clone the repository
2. Navigate to the project directory
3. Run the application:
   ```bash
   dotnet run
   ```

#### Docker Deployment
1. Build and run with Docker Compose:
   ```bash
   docker-compose up --build
   ```

### Database Setup

The application supports two database options:

1. **MongoDB**
   - Default connection: mongodb://localhost:27017
   - Database name: cornerShop
   - Document-based storage
   - Flexible schema

2. **Entity Framework Core (SQLite)**
   - Database file: cornerShop.db
   - Automatically created on first run
   - Relational data model
   - ACID transactions

## Technical Choices

### Platform
- .NET Core 6.0
  - Cross-platform support
  - Modern language features
  - Excellent database integration
  - Built-in async support
  - See [Platform Choice ADR](docs/ADR/001-platform-choice.md) for details

### Architecture
- 2-tier client-server architecture
- 3-layer application design
- Clear separation of concerns
- See [Separation of Responsibilities ADR](docs/ADR/002-separation-of-responsibilities.md) for details

### Databases
- MongoDB
  - Document-based storage
  - Flexible schema
  - Native .NET support
- Entity Framework Core with SQLite
  - Relational data model
  - ACID transactions
  - Built-in .NET integration
  - See [Database Architecture ADR](docs/ADR/001-database-architecture.md) for details

## Testing

The project includes:
- Unit tests for business logic
- Integration tests for database operations
- Test coverage for critical paths
- Automated test execution

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.