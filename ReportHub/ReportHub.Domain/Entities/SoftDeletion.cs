namespace ReportHub.Domain.Entities;

public abstract class SoftDeletion
{
    public bool IsDeleted { get; set; } = false;
}
