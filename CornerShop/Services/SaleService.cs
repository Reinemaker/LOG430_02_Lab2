using CornerShop.Models;

namespace CornerShop.Services
{
    public class SaleService : ISaleService
    {
        private readonly IDatabaseService _databaseService;
        private readonly IProductService _productService;

        public SaleService(IDatabaseService databaseService, IProductService productService)
        {
            _databaseService = databaseService;
            _productService = productService;
        }

        public async Task<string> CreateSale(Sale sale)
        {
            if (sale == null)
                throw new ArgumentNullException(nameof(sale));

            if (!sale.Items.Any())
                throw new ArgumentException("Sale must have at least one item", nameof(sale));

            if (!await ValidateSaleItems(sale.Items, sale.StoreId))
                throw new InvalidOperationException("One or more items in the sale are invalid");

            // Update stock for each item
            foreach (var item in sale.Items)
            {
                await _productService.UpdateStock(item.ProductName, sale.StoreId, -item.Quantity);
            }

            sale.TotalAmount = await CalculateSaleTotal(sale.Items, sale.StoreId);
            return await _databaseService.CreateSale(sale);
        }

        public async Task<List<Sale>> GetRecentSales(string storeId, int limit = 10)
        {
            if (limit <= 0)
                throw new ArgumentException("Limit must be greater than zero", nameof(limit));
            if (string.IsNullOrWhiteSpace(storeId))
                throw new ArgumentException("Store ID cannot be empty", nameof(storeId));
            return await _databaseService.GetRecentSales(storeId, limit);
        }

        public async Task<Sale?> GetSaleById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Sale ID cannot be empty", nameof(id));

            return await _databaseService.GetSaleById(id);
        }

        public async Task<bool> CancelSale(string saleId, string storeId)
        {
            var sale = await GetSaleById(saleId);
            if (sale == null) return false;

            // Restore stock for each item
            foreach (var item in sale.Items)
            {
                await _productService.UpdateStock(item.ProductName, storeId, item.Quantity);
            }

            return await _databaseService.CancelSale(saleId);
        }

        public async Task<decimal> CalculateSaleTotal(List<SaleItem> items, string storeId)
        {
            if (items == null || !items.Any())
                throw new ArgumentException("Items list cannot be empty", nameof(items));
            if (string.IsNullOrWhiteSpace(storeId))
                throw new ArgumentException("Store ID cannot be empty", nameof(storeId));

            decimal total = 0;
            foreach (var item in items)
            {
                var product = await _productService.GetProductByName(item.ProductName, storeId) ?? throw new InvalidOperationException($"Product {item.ProductName} not found");
                total += product.Price * item.Quantity;
            }
            return total;
        }

        public async Task<bool> ValidateSaleItems(List<SaleItem> items, string storeId)
        {
            if (items == null || !items.Any())
                return false;
            if (string.IsNullOrWhiteSpace(storeId))
                throw new ArgumentException("Store ID cannot be empty", nameof(storeId));

            foreach (var item in items)
            {
                if (!await _productService.ValidateProductExists(item.ProductName, storeId))
                    return false;

                if (!await _productService.ValidateStockAvailability(item.ProductName, storeId, item.Quantity))
                    return false;
            }

            return true;
        }
    }
}
