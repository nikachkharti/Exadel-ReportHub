namespace ReportHub.Web.Models.Clients
{
    public record ClientForGettingDto
    (
        string Id,
        string Name,
        string Specialization,
        bool IsDeleted
    );
}
