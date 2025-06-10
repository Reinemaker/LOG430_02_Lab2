using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CornerShop.Models;

public class Store
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    [Required]
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [BsonElement("location")]
    public string Location { get; set; } = string.Empty;

    [BsonElement("address")]
    public string Address { get; set; } = string.Empty;

    [BsonElement("isHeadquarters")]
    public bool IsHeadquarters { get; set; }

    [BsonElement("lastSyncTime")]
    public DateTime LastSyncTime { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = "Active"; // Active, Inactive, Maintenance
}
