using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ReportHub.Application.Features.Sale.DTOs
{
    public record SaleForGettingDto(string Id, string ClientId, string ItemId, decimal Amount, DateTime SaleDate);
}
