using CornerShop.Services;
using CornerShop.Models;
using CornerShop.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace CornerShop
{
    internal class Program
    {
        private static IDatabaseService _databaseService = null!;
        private static DatabaseSyncService _syncService = null!;
        private static IProductService _productService = null!;
        private static ISaleService _saleService = null!;
        private static ICashRegisterService _cashRegisterService = null!;

        private static async Task Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();
                var serviceProvider = host.Services;

                // Initialize both databases
                var mongoDb = new MongoDatabaseService("mongodb://localhost:27017", "cornerShop");
                var efDb = serviceProvider.GetRequiredService<EfDatabaseService>();
                await mongoDb.InitializeDatabase();
                await efDb.InitializeDatabase();

                // Initialize sync service
                _syncService = new DatabaseSyncService(mongoDb, efDb);

                // Select database
                await SelectDatabase(mongoDb, efDb);

                // Initialize services with the selected database
                InitializeServices();

                while (true)
                {
                    try
                    {
                        Console.WriteLine("\n=== Corner Shop Management System ===");
                        Console.WriteLine("1. Search Products");
                        Console.WriteLine("2. Create Sale");
                        Console.WriteLine("3. Cancel Sale");
                        Console.WriteLine("4. Check Stock");
                        Console.WriteLine("5. Switch Database");
                        Console.WriteLine("6. Sync Databases");
                        Console.WriteLine("7. Exit");
                        Console.Write("Select an option: ");

                        var option = Console.ReadLine();

                        switch (option)
                        {
                            case "1":
                                await HandleProductSearch(_productService);
                                break;
                            case "2":
                                await HandleSaleCreation(_cashRegisterService);
                                break;
                            case "3":
                                await HandleSaleCancellation(_cashRegisterService);
                                break;
                            case "4":
                                await HandleStockCheck(_productService);
                                break;
                            case "5":
                                await SelectDatabase(mongoDb, efDb);
                                InitializeServices();
                                break;
                            case "6":
                                await HandleDatabaseSync();
                                break;
                            case "7":
                                return;
                            default:
                                Console.WriteLine("Invalid option. Please try again.");
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\nError: {ex.Message}");
                        Console.WriteLine("Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static void InitializeServices()
        {
            _productService = new ProductService(_databaseService);
            _saleService = new SaleService(_databaseService, _productService);
            _cashRegisterService = new CashRegisterService(_databaseService, _productService, _saleService);
        }

        private static async Task SelectDatabase(MongoDatabaseService mongoDb, EfDatabaseService efDb)
        {
            Console.WriteLine("\nSelect database:");
            Console.WriteLine("1. MongoDB");
            Console.WriteLine("2. Entity Framework Core (SQLite)");
            Console.Write("Enter your choice (1 or 2): ");

            var choice = Console.ReadLine();
            _databaseService = choice switch
            {
                "1" => mongoDb,
                "2" => efDb,
                _ => throw new ArgumentException("Invalid database choice")
            };

            Console.WriteLine($"\nUsing {(_databaseService is MongoDatabaseService ? "MongoDB" : "Entity Framework Core")} database.");
            await Task.CompletedTask;
        }

        private static async Task HandleDatabaseSync()
        {
            Console.WriteLine("\nSynchronizing databases...");
            await _syncService.SyncDatabases();
            Console.WriteLine("Databases synchronized successfully.");
        }

        private static async Task HandleProductSearch(IProductService productService)
        {
            Console.Write("Enter search term: ");
            var searchTerm = Console.ReadLine() ?? "";
            var products = await productService.SearchProducts(searchTerm);

            if (products.Any())
            {
                Console.WriteLine("\nFound Products:");
                foreach (var product in products)
                {
                    Console.WriteLine($"- {product.Name} (${product.Price:F2}, Stock: {product.StockQuantity})");
                }
            }
            else
            {
                Console.WriteLine("No products found.");
            }
        }

        private static async Task HandleSaleCreation(ICashRegisterService cashRegisterService)
        {
            Console.WriteLine("\n=== Create New Sale ===");

            // Get register number
            int registerId;
            do
            {
                Console.Write("Enter register number (0-2): ");
                if (!int.TryParse(Console.ReadLine(), out registerId) || registerId < 0 || registerId > 2)
                {
                    Console.WriteLine("Invalid register number. Please enter a number between 0 and 2.");
                }
            } while (registerId < 0 || registerId > 2);

            var sale = new Sale
            {
                Date = DateTime.UtcNow,
                Items = new List<SaleItem>()
            };

            Console.WriteLine("\nAdding items to sale (type 'done' when finished):");
            while (true)
            {
                // Get product name
                Console.Write("\nEnter product name (or 'done' to finish): ");
                var productName = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(productName) || productName.ToLower() == "done")
                    break;

                // Verify product exists
                var product = await _productService.GetProductByName(productName);
                if (product == null)
                {
                    Console.WriteLine($"Product '{productName}' not found. Please try again.");
                    continue;
                }

                // Show current stock
                Console.WriteLine($"Current stock: {product.StockQuantity} units");

                // Get quantity
                int quantity;
                do
                {
                    Console.Write("Enter quantity: ");
                    if (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0)
                    {
                        Console.WriteLine("Invalid quantity. Please enter a positive number.");
                    }
                    else if (quantity > product.StockQuantity)
                    {
                        Console.WriteLine($"Not enough stock. Only {product.StockQuantity} units available.");
                    }
                } while (quantity <= 0 || quantity > product.StockQuantity);

                // Add item to sale
                sale.Items.Add(new SaleItem
                {
                    ProductName = productName,
                    Quantity = quantity
                });

                // Show running total
                var itemTotal = product.Price * quantity;
                Console.WriteLine($"Added {quantity} x {productName} (${itemTotal:F2})");
            }

            if (!sale.Items.Any())
            {
                Console.WriteLine("\nNo items added to sale. Sale cancelled.");
                return;
            }

            // Show sale summary
            Console.WriteLine("\n=== Sale Summary ===");
            decimal total = 0;
            foreach (var item in sale.Items)
            {
                var product = await _productService.GetProductByName(item.ProductName);
                var itemTotal = product!.Price * item.Quantity;
                total += itemTotal;
                Console.WriteLine($"{item.Quantity} x {item.ProductName}: ${itemTotal:F2}");
            }
            Console.WriteLine($"Total: ${total:F2}");

            // Confirm sale
            Console.Write("\nConfirm sale? (y/n): ");
            var confirm = Console.ReadLine()?.ToLower();
            if (confirm != "y")
            {
                Console.WriteLine("Sale cancelled.");
                return;
            }

            await cashRegisterService.CreateSaleOnRegister(registerId, sale);
            Console.WriteLine($"\nSale created successfully on register {registerId}");

            // Sync databases after sale
            Console.WriteLine("\nSynchronizing databases...");
            await _syncService.SyncDatabases();
            Console.WriteLine("Databases synchronized successfully.");
        }

        private static async Task HandleSaleCancellation(ICashRegisterService cashRegisterService)
        {
            Console.WriteLine("\n=== Cancel Sale ===");

            // Get register number
            int registerId;
            do
            {
                Console.Write("Enter register number (0-2): ");
                if (!int.TryParse(Console.ReadLine(), out registerId) || registerId < 0 || registerId > 2)
                {
                    Console.WriteLine("Invalid register number. Please enter a number between 0 and 2.");
                }
            } while (registerId < 0 || registerId > 2);

            // Show recent sales
            var recentSales = await _saleService.GetRecentSales(10);
            if (!recentSales.Any())
            {
                Console.WriteLine("\nNo recent sales found.");
                return;
            }

            Console.WriteLine("\nRecent sales:");
            Console.WriteLine("----------------------------------------");
            for (int i = 0; i < recentSales.Count; i++)
            {
                var sale = recentSales[i];
                var total = sale.Items.Sum(item =>
                {
                    var product = _productService.GetProductByName(item.ProductName).Result;
                    return product?.Price * item.Quantity ?? 0;
                });

                Console.WriteLine($"{i + 1}. Sale ID: {sale.Id}");
                Console.WriteLine($"   Date: {sale.Date:g}");
                Console.WriteLine($"   Total: ${total:F2}");
                Console.WriteLine("   Items:");
                foreach (var item in sale.Items)
                {
                    Console.WriteLine($"     - {item.Quantity} x {item.ProductName}");
                }
                Console.WriteLine("----------------------------------------");
            }

            // Get sale selection
            int selection;
            do
            {
                Console.Write("\nEnter the number of the sale to cancel (0 to exit): ");
                if (!int.TryParse(Console.ReadLine(), out selection))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                if (selection == 0)
                {
                    Console.WriteLine("Cancellation aborted.");
                    return;
                }

                if (selection < 1 || selection > recentSales.Count)
                {
                    Console.WriteLine($"Please enter a number between 1 and {recentSales.Count}.");
                }
            } while (selection < 1 || selection > recentSales.Count);

            var saleToCancel = recentSales[selection - 1];

            // Show sale details
            Console.WriteLine("\nSelected sale details:");
            Console.WriteLine($"ID: {saleToCancel.Id}");
            Console.WriteLine($"Date: {saleToCancel.Date:g}");
            Console.WriteLine("Items:");
            foreach (var item in saleToCancel.Items)
            {
                var product = await _productService.GetProductByName(item.ProductName);
                var itemTotal = product!.Price * item.Quantity;
                Console.WriteLine($"  - {item.Quantity} x {item.ProductName} (${itemTotal:F2})");
            }

            // Confirm cancellation
            Console.Write("\nAre you sure you want to cancel this sale? (y/n): ");
            var confirm = Console.ReadLine()?.ToLower();
            if (confirm != "y")
            {
                Console.WriteLine("Cancellation aborted.");
                return;
            }

            var success = await cashRegisterService.CancelSaleOnRegister(registerId, saleToCancel.Id);
            if (success)
            {
                Console.WriteLine("\nSale cancelled successfully.");
                // Sync databases after cancellation
                Console.WriteLine("\nSynchronizing databases...");
                await _syncService.SyncDatabases();
                Console.WriteLine("Databases synchronized successfully.");
            }
            else
            {
                Console.WriteLine("\nFailed to cancel sale. It may not exist or belong to this register.");
            }
        }

        private static async Task HandleStockCheck(IProductService productService)
        {
            Console.WriteLine("\n=== Stock Check ===");

            // Get all products
            var products = await productService.GetAllProducts();

            // Group products by category
            var productsByCategory = products
                .GroupBy(p => p.Category)
                .OrderBy(g => g.Key);

            foreach (var category in productsByCategory)
            {
                Console.WriteLine($"\n{category.Key}:");
                Console.WriteLine("----------------------------------------");
                foreach (var product in category.OrderBy(p => p.Name))
                {
                    var stockStatus = product.StockQuantity switch
                    {
                        0 => "OUT OF STOCK",
                        <= 5 => "LOW STOCK",
                        _ => "In Stock"
                    };
                    Console.WriteLine($"{product.Name,-20} {product.StockQuantity,4} units  ({stockStatus})");
                }
            }

            // Show summary
            var outOfStock = products.Count(p => p.StockQuantity == 0);
            var lowStock = products.Count(p => p.StockQuantity > 0 && p.StockQuantity <= 5);

            Console.WriteLine("\n=== Stock Summary ===");
            Console.WriteLine($"Total Products: {products.Count}");
            Console.WriteLine($"Out of Stock: {outOfStock}");
            Console.WriteLine($"Low Stock: {lowStock}");
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Configure EF Core
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseSqlite("Data Source=cornerShop.db"));

                    // Register services
                    services.AddSingleton<EfDatabaseService>();
                });
    }
}
