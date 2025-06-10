using CornerShop.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

// Configure MongoDB
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
var mongoClient = new MongoClient(mongoConnectionString);
var database = mongoClient.GetDatabase("cornerShop");

// Register services
builder.Services.AddSingleton<IMongoDatabase>(database);
builder.Services.AddScoped<CornerShop.Services.IDatabaseService>(sp =>
    new CornerShop.Services.MongoDatabaseService("mongodb://localhost:27017", "cornerShop"));
builder.Services.AddSingleton<IStoreService, StoreService>();
builder.Services.AddScoped<CornerShop.Services.SyncService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialize database
var dbService = app.Services.GetRequiredService<IDatabaseService>();
await dbService.InitializeDatabase();

var storeService = app.Services.GetRequiredService<IStoreService>();
var stores = await storeService.GetAllStores();
foreach (var store in stores)
{
    CornerShop.Services.LocalStoreDatabaseHelper.CreateLocalDatabaseForStore(store.Id);
}

app.Run();
