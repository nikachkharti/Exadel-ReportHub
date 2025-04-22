using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReportHub.Domain.Entities;

public class ExchangeRate : SoftDeletion
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Base { get; set; }
    public DateTime Date { get; set; }
    public Dictionary<string, decimal> Rates { get; set; }
}
