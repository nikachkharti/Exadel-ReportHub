using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Helper;
using ReportHub.Infrastructure.Repository;

namespace ReportHub.Tests.Infrastructure
{
    public class InvoiceRepositoryTests : IDisposable
    {
        private readonly MongoDbRunner _mongoRunner;
        private readonly IMongoDatabase _database;
        private readonly InvoiceRepository _repository;

        public InvoiceRepositoryTests()
        {
            // Step 1: Start in-memory MongoDB
            _mongoRunner = MongoDbRunner.Start();

            var mongoClient = new MongoClient(_mongoRunner.ConnectionString);
            _database = mongoClient.GetDatabase("TestDb");

            // Step 2: Configure MongoDB settings
            var settings = Options.Create(new MongoDbSettings
            {
                ConnectionString = _mongoRunner.ConnectionString,
                DatabaseName = "TestDb"
            });

            // Step 3: Initialize repository
            _repository = new InvoiceRepository(settings);

            // Step 4: Manually set the in-memory database
            typeof(MongoRepositoryBase<Invoice>)
                .GetProperty("Database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_repository, _database);

            typeof(MongoRepositoryBase<Invoice>)
                .GetProperty("Collection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_repository, _database.GetCollection<Invoice>("Invoices"));
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllInvoices()
        {
            // Arrange: Insert test data into in-memory MongoDB
            var collection = _database.GetCollection<Invoice>("Invoices");
            await collection.InsertManyAsync(new List<Invoice>()
            {
                new Invoice { InvoiceId = "INV2025001", IssueDate = DateTime.UtcNow.AddDays(-10), DueDate = DateTime.UtcNow.AddDays(20), Amount = 5000.75m, Currency = "USD", PaymentStatus = "Paid" },
                new Invoice { InvoiceId = "INV2025002", IssueDate = DateTime.UtcNow.AddDays(-15), DueDate = DateTime.UtcNow.AddDays(15), Amount = 7500.50m, Currency = "EUR", PaymentStatus = "Pending" },
                new Invoice { InvoiceId = "INV2025003", IssueDate = DateTime.UtcNow.AddDays(-5), DueDate = DateTime.UtcNow.AddDays(30), Amount = 10234.00m, Currency = "USD", PaymentStatus = "Overdue" }
            });

            // Act
            var result = await _repository.GetAll();

            // Assert
            Assert.NotNull(result);
            var invoiceList = Assert.IsAssignableFrom<IEnumerable<Invoice>>(result);
            Assert.Equal(3, invoiceList.Count());
        }



        public void Dispose()
        {
            _mongoRunner.Dispose();
        }
    }
}
