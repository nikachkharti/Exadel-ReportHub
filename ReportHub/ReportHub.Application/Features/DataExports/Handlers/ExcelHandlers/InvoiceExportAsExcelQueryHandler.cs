using MediatR;
using ReportHub.Application.Contracts.FileContracts;
using ReportHub.Application.Contracts.IdentityContracts;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.DataExports.Queries.ExcelQueries;
using ReportHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReportHub.Application.Features.DataExports.Handlers.ExcelHandlers;

public class InvoiceExportAsExcelQueryHandler : IRequestHandler<InvoiceExportAsExcelQuery, Stream>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IExcelService _excelService;
    private readonly IInvoiceLogRepository _invoiceLogRepository;
    private readonly IItemRepository _itemRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IPlanRepository _planRepository; // Added plan repository
    private readonly IUserContextService _userContext;
    private IEnumerable<Invoice> invoices;

    public InvoiceExportAsExcelQueryHandler(
        IInvoiceRepository invoiceRepository,
        IInvoiceLogRepository invoiceLogRepository,
        IUserContextService userContextService,
        IItemRepository itemRepository,
        IClientRepository clientRepository,
        IPlanRepository planRepository, // Added plan repository
        IExcelService excelService)
    {
        _invoiceRepository = invoiceRepository;
        _excelService = excelService;
        _userContext = userContextService;
        _invoiceLogRepository = invoiceLogRepository;
        _itemRepository = itemRepository;
        _clientRepository = clientRepository;
        _planRepository = planRepository; // Initialize plan repository
    }

    public async Task<Stream> Handle(InvoiceExportAsExcelQuery request, CancellationToken cancellationToken)
    {
        var authenticatedUserId = _userContext.GetUserId();

        try
        {
            invoices = await _invoiceRepository.GetAll(cancellationToken);
            var summary = await GetEnhancedSummary(invoices, cancellationToken);

            if (invoices.Any())
            {
                foreach (var invoice in invoices)
                {
                    await _invoiceLogRepository.Insert(new InvoiceLog()
                    {
                        UserId = authenticatedUserId,
                        InvoiceId = invoice.Id,
                        TimeStamp = DateTime.UtcNow,
                        Status = "Success",
                        IsDeleted = invoice.IsDeleted
                    }, cancellationToken);
                }
            }

            return await _excelService.WriteAllAsync(invoices, summary, cancellationToken);
        }
        catch
        {
            if (invoices.Any())
            {
                foreach (var invoice in invoices)
                {
                    await _invoiceLogRepository.Insert(new InvoiceLog()
                    {
                        UserId = authenticatedUserId,
                        InvoiceId = invoice.Id,
                        TimeStamp = DateTime.UtcNow,
                        Status = "Failure"
                    }, cancellationToken);
                }
            }

            throw;
        }
    }

    private async Task<IReadOnlyDictionary<string, object>> GetEnhancedSummary(
        IEnumerable<Invoice> invoices,
        CancellationToken cancellationToken)
    {
        var summary = new Dictionary<string, object>
        {
            // Basic counts
            { "Invoice Summary", "" },
            { "Total Invoices", invoices.Count() },
            { "Paid", invoices.Count(x => x.PaymentStatus.Equals("Paid")) },
            { "Pending", invoices.Count(x => x.PaymentStatus.Equals("Pending")) },
            { "Overdue", invoices.Count(x => x.PaymentStatus.Equals("Overdue")) },
            
            // Time-based metrics
            { "Time-Based Metrics", "" },
            { "Created Today", invoices.Count(x => x.IssueDate.Date == DateTime.Today) },
            { "Due This Week", invoices.Count(x =>
                (x.DueDate - DateTime.Today).TotalDays <= 7 &&
                (x.DueDate - DateTime.Today).TotalDays >= 0 &&
                x.PaymentStatus != "Paid") },
            { "Overdue By >30 Days", invoices.Count(x =>
                (DateTime.Today - x.DueDate).TotalDays > 30 &&
                x.PaymentStatus != "Paid") }
        };

        // Financial metrics
        decimal totalValue = 0;
        decimal paidValue = 0;
        decimal pendingValue = 0;
        decimal overdueValue = 0;

        foreach (var invoice in invoices)
        {
            var items = await _itemRepository.GetAll(i => invoice.ItemIds.Contains(i.Id), cancellationToken);
            decimal invoiceTotal = items.Sum(i => i.Price);

            totalValue += invoiceTotal;

            switch (invoice.PaymentStatus)
            {
                case "Paid":
                    paidValue += invoiceTotal;
                    break;
                case "Pending":
                    pendingValue += invoiceTotal;
                    break;
                case "Overdue":
                    overdueValue += invoiceTotal;
                    break;
            }
        }

        summary.Add("Financial Metrics", "");
        summary.Add("Total Invoice Value", totalValue.ToString("N2"));
        summary.Add("Paid Value", paidValue.ToString("N2"));
        summary.Add("Pending Value", pendingValue.ToString("N2"));
        summary.Add("Overdue Value", overdueValue.ToString("N2"));

        // Client metrics
        var clientGroups = invoices.GroupBy(x => x.ClientId).ToList();
        summary.Add("Client Metrics", "");
        summary.Add("Total Clients", clientGroups.Count);

        if (clientGroups.Any())
        {
            var topClient = clientGroups.OrderByDescending(g => g.Count()).First();
            var clientDetails = await _clientRepository.Get(c => c.Id == topClient.Key, cancellationToken);
            summary.Add("Top Client By Count", clientDetails?.Name ?? topClient.Key);
            summary.Add("Top Client Invoice Count", topClient.Count());
        }

        // Plan metrics - new section
        var plans = await _planRepository.GetAll(cancellationToken);
        
        summary.Add("Plan Statistics", "");
        summary.Add("Total Plans", plans.Count());
        summary.Add("In Progress Plans", plans.Count(x => x.Status == PlanStatus.InProgress));
        summary.Add("Planned Plans", plans.Count(x => x.Status == PlanStatus.Planned));
        summary.Add("Completed Plans", plans.Count(x => x.Status == PlanStatus.Completed));
        summary.Add("Cancelled Plans", plans.Count(x => x.Status == PlanStatus.Canceled));
        
        // Calculate plan value metrics
        decimal totalPlanValue = plans.Sum(x => x.Amount);
        summary.Add("Total Plan Value", totalPlanValue.ToString("N2"));
        
        // Plans starting this month
        var currentMonth = DateTime.Today.Month;
        var currentYear = DateTime.Today.Year;
        summary.Add("Plans Starting This Month", plans.Count(x => 
            x.StartDate.Month == currentMonth && 
            x.StartDate.Year == currentYear));
            
        // Plans ending this month
        summary.Add("Plans Ending This Month", plans.Count(x => 
            x.EndDate.Month == currentMonth && 
            x.EndDate.Year == currentYear));
            
        // Average plan duration in days
        var avgDuration = plans.Any() 
            ? plans.Average(x => (x.EndDate - x.StartDate).TotalDays) 
            : 0;
        summary.Add("Average Plan Duration (Days)", Math.Round(avgDuration, 1));

        return summary;
    }
}