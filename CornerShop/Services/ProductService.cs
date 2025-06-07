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

        public async Task<List<Product>> SearchProducts(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term cannot be empty", nameof(searchTerm));

            return await _databaseService.SearchProducts(searchTerm);
        }

        public async Task<Product?> GetProductByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty", nameof(name));

            return await _databaseService.GetProductByName(name);
        }

        public async Task<bool> UpdateStock(string productName, int quantity)
        {
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentException("Product name cannot be empty", nameof(productName));

            if (quantity == 0)
                throw new ArgumentException("Quantity cannot be zero", nameof(quantity));

            return await _databaseService.UpdateProductStock(productName, quantity);
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _databaseService.GetAllProducts();
        }

        public async Task<bool> ValidateProductExists(string productName)
        {
            var product = await GetProductByName(productName);
            return product != null;
        }

        public async Task<bool> ValidateStockAvailability(string productName, int quantity)
        {
            var product = await GetProductByName(productName);
            if (product == null) return false;
            return product.StockQuantity >= quantity;
        }
    }
}
