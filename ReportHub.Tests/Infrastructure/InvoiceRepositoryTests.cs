using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Helper;
using ReportHub.Infrastructure.Repository;
using System.Reflection;

namespace ReportHub.Tests.Infrastructure
{
    public class InvoiceRepositoryTests : IAsyncLifetime
    {
        private readonly MongoDbRunner _mongoRunner;
        private readonly IMongoDatabase _database;
        private readonly InvoiceRepository _repository;

        public InvoiceRepositoryTests()
        {
            _mongoRunner = MongoDbRunner.Start();
            _database = new MongoClient(_mongoRunner.ConnectionString).GetDatabase("TestDb");
            _repository = SetupRepository();
        }

        private InvoiceRepository SetupRepository()
        {
            var settings = Options.Create(new MongoDbSettings
            {
                ConnectionString = _mongoRunner.ConnectionString,
                DatabaseName = "TestDb"
            });

            var repository = new InvoiceRepository(settings);

            // Set in-memory database and collection using reflection
            typeof(MongoRepositoryBase<Invoice>)
                .GetProperty("Database", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(repository, _database);

            typeof(MongoRepositoryBase<Invoice>)
                .GetProperty("Collection", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(repository, _database.GetCollection<Invoice>("Invoices"));

            return repository;
        }
        private async Task SeedTestData()
        {
            var collection = _database.GetCollection<Invoice>("Invoices");
            await collection.InsertManyAsync(new List<Invoice>()
            {
                new Invoice { InvoiceId = "INV2025001", IssueDate = DateTime.UtcNow.AddDays(-10), DueDate = DateTime.UtcNow.AddDays(20), Amount = 5000.75m, Currency = "USD", PaymentStatus = "Paid" },
                new Invoice { InvoiceId = "INV2025002", IssueDate = DateTime.UtcNow.AddDays(-15), DueDate = DateTime.UtcNow.AddDays(15), Amount = 7500.50m, Currency = "EUR", PaymentStatus = "Pending" },
                new Invoice { InvoiceId = "INV2025003", IssueDate = DateTime.UtcNow.AddDays(-5), DueDate = DateTime.UtcNow.AddDays(30), Amount = 10234.00m, Currency = "USD", PaymentStatus = "Overdue" }
            });
        }
        public async Task InitializeAsync() => await SeedTestData();
        public Task DisposeAsync()
        {
            _mongoRunner.Dispose();
            return Task.CompletedTask;
        }


        [Fact]
        public async Task GetAll_ShouldReturnAllInvoices()
        {
            var result = await _repository.GetAll();

            Assert.NotNull(result);
            var invoiceList = Assert.IsAssignableFrom<IEnumerable<Invoice>>(result);
            Assert.Equal(3, invoiceList.Count());
        }

        [Fact]
        public async Task GetAll_ShouldReturnSortedByInvoiceId_Ascending()
        {
            var result = await _repository.GetAll(i => i.InvoiceId, ascending: true);

            Assert.NotNull(result);
            var sortedList = result.ToList();

            Assert.Equal(3, sortedList.Count);
            Assert.Equal("INV2025001", sortedList[0].InvoiceId);
            Assert.Equal("INV2025002", sortedList[1].InvoiceId);
            Assert.Equal("INV2025003", sortedList[2].InvoiceId);
        }

        [Fact]
        public async Task GetAll_ShouldReturnSortedByInvoiceId_Descending()
        {
            var result = await _repository.GetAll(i => i.InvoiceId, ascending: false);

            Assert.NotNull(result);
            var sortedList = result.ToList();

            Assert.Equal(3, sortedList.Count);
            Assert.Equal("INV2025003", sortedList[0].InvoiceId);
            Assert.Equal("INV2025002", sortedList[1].InvoiceId);
            Assert.Equal("INV2025001", sortedList[2].InvoiceId);
        }

        [Fact]
        public async Task GetAll_ShouldReturnSortedByAmount_Ascending()
        {
            var result = await _repository.GetAll(i => i.Amount, ascending: true);

            Assert.NotNull(result);
            var sortedList = result.ToList();

            Assert.Equal(3, sortedList.Count);
            Assert.Equal(5000.75m, sortedList[0].Amount);
            Assert.Equal(7500.50m, sortedList[1].Amount);
            Assert.Equal(10234.00m, sortedList[2].Amount);
        }

        [Fact]
        public async Task GetAll_ShouldReturnSortedByAmount_Descending()
        {
            var result = await _repository.GetAll(i => i.Amount, ascending: false);

            Assert.NotNull(result);
            var sortedList = result.ToList();

            Assert.Equal(3, sortedList.Count);
            Assert.Equal(10234.00m, sortedList[0].Amount);
            Assert.Equal(7500.50m, sortedList[1].Amount);
            Assert.Equal(5000.75m, sortedList[2].Amount);
        }
    }
}
