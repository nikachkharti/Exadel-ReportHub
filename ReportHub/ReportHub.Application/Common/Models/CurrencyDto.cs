namespace ReportHub.Application.Common.Models;

public class CurrencyDto
{
    public decimal Amount { get; set; }
    public string Base { get; set; }
    public DateTime Date { get; set; }
    public Dictionary<string, decimal> Rates { get; set; }
}
