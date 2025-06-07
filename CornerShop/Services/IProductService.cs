using CornerShop.Models;

namespace CornerShop.Services
{
    public interface IProductService
    {
        Task<List<Product>> SearchProducts(string searchTerm);
        Task<Product?> GetProductByName(string name);
        Task<bool> UpdateStock(string productName, int quantity);
        Task<List<Product>> GetAllProducts();
        Task<bool> ValidateProductExists(string productName);
        Task<bool> ValidateStockAvailability(string productName, int quantity);
    }
}
