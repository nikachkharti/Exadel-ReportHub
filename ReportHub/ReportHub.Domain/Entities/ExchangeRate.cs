namespace ReportHub.Domain.Entities;

public class ExchangeRate
{
    public DateTime Date { get; set; }
    public Dictionary<string, decimal> Rates { get; set; }
}
