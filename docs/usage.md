# Usage Guide

## Starting the Application

1. Navigate to the project directory
2. Run the application:
   ```bash
   dotnet run
   ```
3. Choose your preferred database:
   - MongoDB
   - Entity Framework Core (SQLite)

## Main Menu Options

### 1. Search Products
- Enter product name (case-insensitive)
- View matching products with details
- See stock status indicators

### 2. Create Sale
1. Select register number (0-2)
2. Add products:
   - Enter product name
   - Enter quantity
   - View running total
3. Confirm sale:
   - Review sale summary
   - Confirm with 'y'
4. Automatic database sync

### 3. Cancel Sale
1. Select register number
2. View recent sales list:
   - Numbered list of sales
   - Sale details and totals
3. Choose sale by number
4. Confirm cancellation
5. Automatic database sync

### 4. Check Stock
- View products by category
- See stock status:
  - Out of Stock (0)
  - Low Stock (1-5)
  - In Stock (>5)
- View stock summary

### 5. Switch Database
- Choose between:
  - MongoDB
  - Entity Framework Core
- Automatic data sync

### 6. Sync Databases
- Manual synchronization
- Sync all data between databases
- View sync status

## Database Synchronization

The system automatically synchronizes data:
- After creating a sale
- After canceling a sale
- When switching databases
- On manual sync request

## Error Handling

The application provides clear error messages for:
- Invalid input
- Database errors
- Stock validation
- Sale operations

## Best Practices

1. **Creating Sales**
   - Verify product names
   - Check stock levels
   - Review sale summary

2. **Canceling Sales**
   - Select correct register
   - Verify sale details
   - Confirm cancellation

3. **Database Management**
   - Regular sync checks
   - Monitor both databases
   - Backup important data

## Troubleshooting

1. **Product Not Found**
   - Check spelling
   - Try different case
   - Verify database sync

2. **Sale Errors**
   - Verify register number
   - Check stock levels
   - Ensure database sync

3. **Database Issues**
   - Check connections
   - Verify sync status
   - Review error messages 