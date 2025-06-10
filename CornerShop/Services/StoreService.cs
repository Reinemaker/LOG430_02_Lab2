using MongoDB.Driver;
using CornerShop.Models;
using MongoDB.Bson;

namespace CornerShop.Services;

public interface IStoreService
{
    Task<List<Store>> GetAllStores();
    Task<Store?> GetStoreById(string storeId);
    Task<string> CreateStore(Store store);
    Task<bool> UpdateStore(Store store);
    Task<bool> DeleteStore(string storeId);
    Task<bool> SyncStoreData(string storeId);
    Task<List<Store>> GetStoresNeedingSync();
    Task<Dictionary<string, object>> GetStoreStatistics(string storeId);
}

public class StoreService : IStoreService
{
    private readonly IMongoCollection<Store> _stores;
    private readonly IMongoCollection<Product> _products;
    private readonly IMongoCollection<Sale> _sales;
    private readonly IDatabaseService _databaseService;

    public StoreService(IMongoDatabase database, IDatabaseService databaseService)
    {
        _stores = database.GetCollection<Store>("stores");
        _products = database.GetCollection<Product>("products");
        _sales = database.GetCollection<Sale>("sales");
        _databaseService = databaseService;
    }

    public async Task<List<Store>> GetAllStores()
    {
        return await _stores.Find(_ => true).ToListAsync();
    }

    public async Task<Store?> GetStoreById(string storeId)
    {
        var filter = Builders<Store>.Filter.Eq(s => s.Id, storeId);
        return await _stores.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<string> CreateStore(Store store)
    {
        await _stores.InsertOneAsync(store);
        return store.Id;
    }

    public async Task<bool> UpdateStore(Store store)
    {
        var filter = Builders<Store>.Filter.Eq(s => s.Id, store.Id);
        var result = await _stores.ReplaceOneAsync(filter, store);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteStore(string storeId)
    {
        var filter = Builders<Store>.Filter.Eq(s => s.Id, storeId);
        var result = await _stores.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }

    public async Task<bool> SyncStoreData(string storeId)
    {
        try
        {
            var store = await GetStoreById(storeId);
            if (store == null) return false;

            // Update products
            var productFilter = Builders<Product>.Filter.Eq(p => p.StoreId, storeId);
            var products = await _products.Find(productFilter).ToListAsync();

            // Update sales
            var saleFilter = Builders<Sale>.Filter.Eq(s => s.StoreId, storeId);
            var sales = await _sales.Find(saleFilter).ToListAsync();

            // Update store's last sync time
            store.LastSyncTime = DateTime.UtcNow;
            await UpdateStore(store);

            return true;
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Error syncing store {storeId}: {ex.Message}");
            return false;
        }
    }

    public async Task<List<Store>> GetStoresNeedingSync()
    {
        var threshold = DateTime.UtcNow.AddHours(-1); // Stores not synced in the last hour
        var filter = Builders<Store>.Filter.Lt(s => s.LastSyncTime, threshold);
        return await _stores.Find(filter).ToListAsync();
    }

    public async Task<Dictionary<string, object>> GetStoreStatistics(string storeId)
    {
        var stats = new Dictionary<string, object>();

        // Get total products
        var productCount = await _products.CountDocumentsAsync(
            Builders<Product>.Filter.Eq(p => p.StoreId, storeId));
        stats["TotalProducts"] = (ulong)productCount;

        // Get total sales
        var salesFilter = Builders<Sale>.Filter.Eq(s => s.StoreId, storeId);
        var totalSales = await _sales.Find(salesFilter).ToListAsync();
        stats["TotalSales"] = (ulong)totalSales.Count;
        stats["TotalRevenue"] = totalSales.Sum(s => s.TotalAmount);

        // Get low stock products
        var lowStockFilter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Eq(p => p.StoreId, storeId),
            Builders<Product>.Filter.Where(p => p.StockQuantity <= p.MinimumStockLevel)
        );
        var lowStockProducts = await _products.Find(lowStockFilter).ToListAsync();
        stats["LowStockProducts"] = (ulong)lowStockProducts.Count;

        return stats;
    }
}
