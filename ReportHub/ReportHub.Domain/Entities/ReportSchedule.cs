using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ReportHub.Domain.Entities
{
    public class ReportSchedule : SoftDeletion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string CronExpression { get; set; } // For schedule timing (e.g., "0 9 * * 1" = Every Monday 9 AM)
        public ReportFormat Format { get; set; }   // CSV or Excel
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
