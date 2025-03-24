using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ReportHub.Application.Contracts;
using ReportHub.Domain.Entities;

namespace ReportHub.Infrastructure.Middleware
{
    public class DataSeedingMiddleware
    {
        private readonly RequestDelegate _next;

        public DataSeedingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var invoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();

                // Check if data already exists
                var existingInvoices = await invoiceRepository.GetAll(pageNumber: 1, pageSize: 1);
                if (!existingInvoices.Any())
                {
                    Console.WriteLine("Seeding initial invoice data...");

                    var invoices = new List<Invoice>
                    {
                        new Invoice { InvoiceId = "INV2025001", IssueDate = DateTime.UtcNow.AddDays(-10), DueDate = DateTime.UtcNow.AddDays(20), Amount = 5000.75m, Currency = "USD", PaymentStatus = "Paid" },
                        new Invoice { InvoiceId = "INV2025002", IssueDate = DateTime.UtcNow.AddDays(-15), DueDate = DateTime.UtcNow.AddDays(15), Amount = 7500.50m, Currency = "EUR", PaymentStatus = "Pending" },
                        new Invoice { InvoiceId = "INV2025003", IssueDate = DateTime.UtcNow.AddDays(-5), DueDate = DateTime.UtcNow.AddDays(30), Amount = 10234.00m, Currency = "USD", PaymentStatus = "Overdue" }
                    };

                    foreach (var invoice in invoices)
                    {
                        await invoiceRepository.Insert(invoice);
                    }

                    Console.WriteLine("Database seeding completed.");
                }
                else
                {
                    Console.WriteLine("Database already contains data. Skipping seeding.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Data seeding failed: {ex.Message}");
            }

            await _next(context);
        }
    }
}
