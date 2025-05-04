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

        /// <summary>
        /// For schedule timing (e.g., "0 9 * * 1" = Every Monday 9 AM)
        /// </summary>
        public string CronExpression { get; set; }

        /// <summary>
        /// CSV - 0 Excel - 1 PDF - 2
        /// </summary>
        public ReportFormat Format { get; set; }
        public bool IsActive { get; set; }
    }
}
