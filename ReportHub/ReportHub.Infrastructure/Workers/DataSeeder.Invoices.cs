using Microsoft.Extensions.DependencyInjection;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using Serilog;

namespace ReportHub.Infrastructure.Workers;

public partial class DataSeeder
{
    private async Task SeedInvoiesAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        var invoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();

        var existingInvoices = await invoiceRepository.GetAll(pageNumber: 1, pageSize: 1);

        if (existingInvoices.Any())
        {
            Log.Information("Database already contains invoice data. Skipping seeding...");
            return;
        }

        Log.Information("Seeding initial invoices data...");

        var invoices = GetInvoices();

        await invoiceRepository.InsertMultiple(invoices, cancellationToken);

        Log.Information("Item seeding completed");
    }

    private IEnumerable<Invoice> GetInvoices()
    {
        return new List<Invoice>()
        {
            new Invoice()
            {
                Id = "67fa2d8114e2389cd806446a",
                ClientId = "67fa2d8114e2389cd8064452",
                CustomerId = "67fa2d8114e2389cd8064457",
                IssueDate = DateTime.UtcNow.AddDays(-10),
                DueDate = DateTime.UtcNow.AddDays(20),
                Currency = "USD",
                Amount = 4000.00m,
                PaymentStatus = "Paid",
                ItemIds = new List<string>()
                {
                    "67fa2d8114e2389cd8064460",
                    "67fa2d8114e2389cd8064461"
                }
            },
            new Invoice()
            {
                Id = "67fa2d8114e2389cd806446b",
                ClientId = "67fa2d8114e2389cd8064452",
                CustomerId = "67fa2d8114e2389cd8064458",
                IssueDate = DateTime.UtcNow.AddDays(-10),
                DueDate = DateTime.UtcNow.AddDays(20),
                Currency = "USD",
                Amount = 1000.00m,
                PaymentStatus = "Paid",
                ItemIds = new List<string>()
                {
                    "67fa2d8114e2389cd8064461"
                }
            },
            new Invoice()
            {
                Id = "67fa2d8114e2389cd806446c",
                ClientId = "67fa2d8114e2389cd8064453",
                CustomerId = "67fa2d8114e2389cd8064459",
                IssueDate = DateTime.UtcNow.AddDays(-10),
                DueDate = DateTime.UtcNow.AddDays(20),
                Currency = "EUR",
                Amount = 189000.00m,
                PaymentStatus = "Paid",
                ItemIds = new List<string>()
                {
                    "67fa2d8114e2389cd8064464",
                    "67fa2d8114e2389cd8064463"
                }
            },
            new Invoice()
            {
                Id = "67fa2d8114e2389cd806446d",
                ClientId = "67fa2d8114e2389cd8064453",
                CustomerId = "67fa2d8114e2389cd8064459",
                IssueDate = DateTime.UtcNow.AddDays(-10),
                DueDate = DateTime.UtcNow.AddDays(20),
                Currency = "EUR",
                Amount = 180000.00m,
                PaymentStatus = "Paid",
                ItemIds = new List<string>()
                {
                    "67fa2d8114e2389cd8064463"
                }
            },
            new Invoice()
            {
                Id = "67fa2d8114e2389cd806446e",
                ClientId = "67fa2d8114e2389cd8064454",
                CustomerId = "67fa2d8114e2389cd806445a",
                IssueDate = DateTime.UtcNow.AddDays(-10),
                DueDate = DateTime.UtcNow.AddDays(20),
                Currency = "USD",
                Amount = 350.00m,
                PaymentStatus = "Paid",
                ItemIds = new List<string>()
                {
                    "67fa2d8114e2389cd8064465"
                }
            },
            new Invoice()
            {
                Id = "67fa2d8114e2389cd806446f",
                ClientId = "67fa2d8114e2389cd8064454",
                CustomerId = "67fa2d8114e2389cd806445a",
                IssueDate = DateTime.UtcNow.AddDays(-10),
                DueDate = DateTime.UtcNow.AddDays(20),
                Currency = "USD",
                Amount = 700.00m,
                PaymentStatus = "Paid",
                ItemIds = new List<string>()
                {
                    "67fa2d8114e2389cd8064466"
                }
            }
        };
    }
}
