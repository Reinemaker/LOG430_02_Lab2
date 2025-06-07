# Development Guide

## Project Setup

1. **Prerequisites**
   - .NET 6.0 or later
   - MongoDB
   - SQLite (included with EF Core)
   - IDE (Visual Studio, VS Code, etc.)

2. **Clone and Build**
   ```bash
   git clone [repository-url]
   cd CornerShop
   dotnet build
   ```

## Project Structure

```
CornerShop/
├── Models/
│   ├── Product.cs
│   ├── Sale.cs
│   └── SaleItem.cs
├── Services/
│   ├── IDatabaseService.cs
│   ├── MongoDatabaseService.cs
│   ├── EfDatabaseService.cs
│   ├── IProductService.cs
│   ├── ProductService.cs
│   ├── ISaleService.cs
│   ├── SaleService.cs
│   ├── ISyncService.cs
│   └── SyncService.cs
└── Program.cs
```

## Database Implementation

### MongoDB
- Uses MongoDB.Driver
- Document-based storage
- BSON serialization
- ObjectId for document IDs

### Entity Framework Core
- Uses Microsoft.EntityFrameworkCore.Sqlite
- Code-first approach
- Automatic migrations
- LINQ queries

## Adding New Features

1. **Model Changes**
   - Update model classes
   - Add BSON attributes for MongoDB
   - Add data annotations for EF Core
   - Update both database services

2. **Service Layer**
   - Update interfaces
   - Implement in both database services
   - Add synchronization logic
   - Update business logic

3. **UI Changes**
   - Update Program.cs
   - Add input validation
   - Improve user feedback
   - Handle errors gracefully

## Database Synchronization

### SyncService Implementation
- Bidirectional sync
- Conflict resolution
- Error handling
- Transaction management

### Sync Points
- After critical operations
- On database switch
- Manual sync
- Error recovery

## Testing

1. **Unit Tests**
   - Test business logic
   - Mock database services
   - Verify sync operations
   - Test error handling

2. **Integration Tests**
   - Test database operations
   - Verify sync functionality
   - Test error recovery
   - Performance testing

## Best Practices

1. **Code Organization**
   - Follow SOLID principles
   - Use dependency injection
   - Maintain separation of concerns
   - Document public APIs

2. **Database Operations**
   - Use transactions where appropriate
   - Handle errors gracefully
   - Implement retry logic
   - Log important operations

3. **Synchronization**
   - Keep sync logic simple
   - Handle conflicts properly
   - Log sync operations
   - Monitor sync performance

## Deployment

1. **Requirements**
   - .NET 6.0 runtime
   - MongoDB server
   - SQLite (included)

2. **Configuration**
   - Update connection strings
   - Set sync preferences
   - Configure logging
   - Set error handling

3. **Monitoring**
   - Monitor database sync
   - Check error logs
   - Track performance
   - Verify data integrity 