# Corner Shop Multi-Store Management System

A modern, distributed point-of-sale and inventory management system for multi-store businesses. Each store operates independently with a local SQLite database, while the head office consolidates data using a central MongoDB database. The system is designed for reliability, offline support, and easy synchronization.

## Features
- **Multi-store support**: Each store has its own local SQLite database for products and sales.
- **Offline operation**: Stores can operate independently, even without internet connectivity.
- **Centralized reporting**: The head office uses MongoDB to view consolidated sales and stock across all stores.
- **One-click sync**: Admins can synchronize all stores' local data to the central database from the Reports page.
- **Modern web interface**: Built with ASP.NET Core MVC for a responsive, user-friendly experience.

## Architecture
- **Web Client**: ASP.NET Core MVC web application
- **Store Data Layer**: Local SQLite database per store
- **Central Data Layer**: MongoDB for consolidated data and reporting
- **Sync Service**: Pushes local sales to MongoDB for global reporting

## Getting Started
1. Clone the repository
2. Run the application with `dotnet run`
3. On store creation, a new SQLite database file is created for that store
4. Use the Reports page to sync all stores to MongoDB for consolidated reporting

## Documentation
- [Technical Docs](docs/README.md)
- [UML Diagrams](docs/UML/)
- [Architecture Decision Records](docs/ADR/)

## License
MIT