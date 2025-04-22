using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ReportHub.Domain.Entities
{
    public class Sale
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ClientId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ItemId { get; set; }

        public decimal Amount { get; set; }
        public DateTime SaleDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
