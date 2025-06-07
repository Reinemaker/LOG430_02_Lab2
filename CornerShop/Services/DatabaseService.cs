using MongoDB.Driver;
using CornerShop.Models;
using MongoDB.Bson;

namespace CornerShop.Services;

public class DatabaseService : IDatabaseService
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<Product> _products;
    private readonly IMongoCollection<Sale> _sales;

    public DatabaseService(string connectionString = "mongodb://localhost:27017", string databaseName = "cornerShop")
    {
        try
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
            _products = _database.GetCollection<Product>("products");
            _sales = _database.GetCollection<Sale>("sales");
            Console.WriteLine("Successfully connected to MongoDB!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to MongoDB: {ex.Message}");
            throw;
        }
    }

    public DatabaseService(IMongoDatabase database)
    {
        _database = database;
        _products = _database.GetCollection<Product>("products");
        _sales = _database.GetCollection<Sale>("sales");
    }

    public async Task InitializeDatabase()
    {
        // Create indexes
        var productIndexKeys = Builders<Product>.IndexKeys.Ascending(p => p.Name);
        var productIndexModel = new CreateIndexModel<Product>(productIndexKeys);
        await _products.Indexes.CreateOneAsync(productIndexModel);

        var saleIndexKeys = Builders<Sale>.IndexKeys.Ascending(s => s.Date);
        var saleIndexModel = new CreateIndexModel<Sale>(saleIndexKeys);
        await _sales.Indexes.CreateOneAsync(saleIndexModel);
    }

    public async Task<List<Product>> SearchProducts(string searchTerm)
    {
        var filter = Builders<Product>.Filter.Regex(p => p.Name,
            new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"));
        var cursor = await _products.FindAsync(filter);
        return await cursor.ToListAsync();
    }

    public async Task<Product?> GetProductByName(string name)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Name, name);
        return await _products.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateProductStock(string productName, int quantity)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Name, productName);
        var update = Builders<Product>.Update.Inc(p => p.StockQuantity, -quantity);
        var result = await _products.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<string> CreateSale(Sale sale)
    {
        await _sales.InsertOneAsync(sale);
        return sale.Id;
    }

    public async Task<List<Sale>> GetRecentSales(int limit = 10)
    {
        var filter = Builders<Sale>.Filter.Empty;
        var sort = Builders<Sale>.Sort.Descending(s => s.Date);
        return await _sales.Find(filter).Sort(sort).Limit(limit).ToListAsync();
    }

    public async Task<Sale?> GetSaleById(string saleId)
    {
        var filter = Builders<Sale>.Filter.Eq(s => s.Id, saleId);
        return await _sales.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> CancelSale(string saleId)
    {
        var filter = Builders<Sale>.Filter.Eq(s => s.Id, saleId);
        var sale = await _sales.Find(filter).FirstOrDefaultAsync();

        if (sale == null) return false;

        // Restore stock for each item
        foreach (var item in sale.Items)
        {
            await UpdateProductStock(item.ProductName, -item.Quantity);
        }

        var deleteResult = await _sales.DeleteOneAsync(filter);
        return deleteResult.DeletedCount > 0;
    }

    public async Task<List<Product>> GetAllProducts()
    {
        return await _products.Find(_ => true).ToListAsync();
    }

    public async Task PrintAllProductNames()
    {
        var products = await GetAllProducts();
        foreach (var product in products)
        {
            Console.WriteLine($"Product: {product.Name}");
            Console.WriteLine();
        }
    }

    public async Task CreateProduct(Product product)
    {
        await _products.InsertOneAsync(product);
    }
}
