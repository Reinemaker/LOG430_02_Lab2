using CornerShop.Models;

namespace CornerShop.Services
{
    public class ProductService : IProductService
    {
        private readonly IDatabaseService _databaseService;

        public ProductService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<Product>> SearchProducts(string searchTerm, string? storeId = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term cannot be empty", nameof(searchTerm));
            return await _databaseService.SearchProducts(searchTerm, storeId);
        }

        public async Task<Product?> GetProductByName(string name, string storeId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty", nameof(name));
            if (string.IsNullOrWhiteSpace(storeId))
                throw new ArgumentException("Store ID cannot be empty", nameof(storeId));
            return await _databaseService.GetProductByName(name, storeId);
        }

        public async Task<bool> UpdateStock(string productName, string storeId, int quantity)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Product name cannot be empty", nameof(productName));
            if (string.IsNullOrWhiteSpace(storeId))
                throw new ArgumentException("Store ID cannot be empty", nameof(storeId));
            if (quantity == 0)
                throw new ArgumentException("Quantity cannot be zero", nameof(quantity));
            return await _databaseService.UpdateProductStock(productName, storeId, quantity);
        }

        public async Task<List<Product>> GetAllProducts(string storeId)
        {
            if (string.IsNullOrWhiteSpace(storeId))
                throw new ArgumentException("Store ID cannot be empty", nameof(storeId));
            return await _databaseService.GetAllProducts(storeId);
        }

        public async Task<bool> ValidateProductExists(string productName, string storeId)
        {
            var product = await GetProductByName(productName, storeId);
            return product != null;
        }

        public async Task<bool> ValidateStockAvailability(string productName, string storeId, int quantity)
        {
            var product = await GetProductByName(productName, storeId);
            if (product == null) return false;
            return product.StockQuantity >= quantity;
        }
    }
}
