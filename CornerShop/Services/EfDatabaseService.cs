using CornerShop.Data;
using CornerShop.Models;
using Microsoft.EntityFrameworkCore;

namespace CornerShop.Services
{
    public class EfDatabaseService : IDatabaseService
    {
        private readonly ApplicationDbContext _context;

        public EfDatabaseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task InitializeDatabase()
        {
            await _context.Database.EnsureCreatedAsync();
        }

        public async Task<List<Product>> SearchProducts(string searchTerm)
        {
            return await _context.Products
                .Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()))
                .ToListAsync();
        }

        public async Task<Product?> GetProductByName(string name)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<bool> UpdateProductStock(string productName, int quantity)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Name == productName);

            if (product == null) return false;

            product.StockQuantity -= quantity;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> CreateSale(Sale sale)
        {
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            return sale.Id;
        }

        public async Task<bool> CancelSale(string saleId)
        {
            var sale = await _context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null) return false;

            // Restore stock for each item
            foreach (var item in sale.Items)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Name == item.ProductName);
                if (product != null)
                {
                    product.StockQuantity += item.Quantity;
                }
            }

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Sale>> GetRecentSales(int limit = 10)
        {
            return await _context.Sales
                .Include(s => s.Items)
                .OrderByDescending(s => s.Date)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Sale?> GetSaleById(string saleId)
        {
            return await _context.Sales
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == saleId);
        }

        public async Task CreateProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
    }
}
