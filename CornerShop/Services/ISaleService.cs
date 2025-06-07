using CornerShop.Models;

namespace CornerShop.Services
{
    public interface ISaleService
    {
        Task<string> CreateSale(Sale sale);
        Task<List<Sale>> GetRecentSales(int limit = 10);
        Task<Sale?> GetSaleById(string id);
        Task<bool> CancelSale(string saleId);
        Task<decimal> CalculateSaleTotal(List<SaleItem> items);
        Task<bool> ValidateSaleItems(List<SaleItem> items);
    }
}
