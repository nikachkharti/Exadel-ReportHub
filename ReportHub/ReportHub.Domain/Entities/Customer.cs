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
        public string Address { get; set; }
    }
}
