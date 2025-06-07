using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CornerShop.Models;

public class Sale
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [BsonElement("date")]
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [BsonElement("items")]
    public List<SaleItem> Items { get; set; } = new List<SaleItem>();

    [BsonElement("total")]
    public decimal Total { get; set; }

    [BsonElement("isCancelled")]
    public bool IsCancelled { get; set; }

    public Sale()
    {
        Date = DateTime.UtcNow;
    }
}
