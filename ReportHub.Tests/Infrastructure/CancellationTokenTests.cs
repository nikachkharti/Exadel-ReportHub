namespace ReportHub.Tests.Infrastructure
{
    public class CancellationTokenTests : IAsyncLifetime
    {
        private readonly MongoDbRunner _mongoRunner;
        private readonly IMongoDatabase _database;
        private readonly InvoiceRepository _repository;

        public CancellationTokenTests()
        {
            _mongoRunner = MongoDbRunner.Start();
            _database = new MongoClient(_mongoRunner.ConnectionString).GetDatabase("TestDb");
            _repository = SetupRepository();
        }

        #region CONFIGURATION
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
        #endregion


        [Fact]
        public async Task GetAll_ShouldCancel_WhenCancellationRequested()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await _repository.GetAll(cts.Token);
            });
        }


        [Fact]
        public async Task GetAll_WithFilterSortBy_ShouldCancel_WhenCancellationRequested()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await _repository.GetAll(
                    filter: invoice => invoice.PaymentStatus == "Paid",
                    sortBy: invoice => invoice.IssueDate,
                    ascending: true,
                    cancellationToken: cts.Token
                );
            });
        }


        [Fact]
        public async Task Get_ShouldCancel_WhenCancellationRequested()
        {
            //Arrange
            Expression<Func<Invoice, bool>> filter = invoice => invoice.Currency == "USD";
            var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _repository.Get(filter, cts.Token));
        }




    }
}
