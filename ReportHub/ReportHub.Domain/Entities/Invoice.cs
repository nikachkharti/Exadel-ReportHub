using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReportHub.Domain.Entities
{
    public class Invoice
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        //Ensure the format of InvoiceId follows this pattern after inserting of new value => 2025(Current year)001 2025(Current year)002 2025(Current year)003
        public string InvoiceId { get; set; }
        public DateTime IssueDate { get; set; }

        //Ensure that due date is always greater than IssueDate
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }

        //Ensure that currency length is 3
        public string Currency { get; set; }

        //Ensure that payment status is either Paid Pending or Overdue
        public string PaymentStatus { get; set; }
    }
}
