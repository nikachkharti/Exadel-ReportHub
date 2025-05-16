using System;

namespace ReportHub.Web.Models.Invoices
{
    public record GetStatistcsQuery
    (
        DateTime? StartDate = null,
        DateTime? EndDate = null,
        string ClientId = "",
        string CustomerId = ""
    );
}