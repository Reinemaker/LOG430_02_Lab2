using System.Collections.Generic;
using System.Threading.Tasks;
using CornerShop.Models;

namespace CornerShop.Services;

public interface IDatabaseService
{
    Task InitializeDatabase();
    Task<List<Product>> SearchProducts(string searchTerm);
    Task<Product?> GetProductByName(string name);
    Task<bool> UpdateProductStock(string productName, int quantity);
    Task<List<Product>> GetAllProducts();
    Task<string> CreateSale(Sale sale);
    Task<List<Sale>> GetRecentSales(int limit = 10);
    Task<Sale?> GetSaleById(string id);
    Task<bool> CancelSale(string saleId);
    Task CreateProduct(Product product);
}
