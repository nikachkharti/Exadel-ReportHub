using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReportHub.Domain.Entities;

public class Currency
{
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CurrencyCode { get; set; } // e.g., USD, EUR
        public string Name { get; set; } // e.g., US Dollar, Euro
        public string Symbol { get; set; } // e.g., $, €
        //It means that money is still in circulation.
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
