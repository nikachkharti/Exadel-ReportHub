using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ReportHub.Domain.Entities
{
    public class InvoiceLog : SoftDeletion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string InvoiceId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Status { get; set; }
    }
}
