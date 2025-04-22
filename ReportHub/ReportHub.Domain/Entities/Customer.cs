using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ReportHub.Domain.Entities
{
    public class Customer
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string CountryId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string ClientId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
