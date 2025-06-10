using System.Threading.Tasks;

namespace CornerShop.Services
{
    public class StockAlertService
    {
        public async Task CheckAndAlertAsync()
        {
            // TODO: Check all stores' SQLite for products below critical threshold
            // TODO: If found, send alert to head office (e.g., insert alert in MongoDB)
        }
    }
}
