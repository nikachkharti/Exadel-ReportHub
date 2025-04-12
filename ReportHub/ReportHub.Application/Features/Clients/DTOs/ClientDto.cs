using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ReportHub.Application.Contracts;

public class ClientDto
{
    public string Id { get; set; }
    public string Name { get; set; }
}