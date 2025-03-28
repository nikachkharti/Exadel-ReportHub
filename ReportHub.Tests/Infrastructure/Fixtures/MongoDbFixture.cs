namespace ReportHub.Tests.Infrastructure.Fixtures
{
    public class MongoDbFixture : IAsyncLifetime
    {
        public MongoDbRunner MongoRunner { get; private set; }
        public IMongoDatabase Database { get; private set; }
        public InvoiceRepository Repository { get; private set; }

        public MongoDbFixture()
        {
            MongoRunner = MongoDbRunner.Start();
            Database = new MongoClient(MongoRunner.ConnectionString).GetDatabase("TestDb");
            Repository = SetupRepository();
        }

        public Task DisposeAsync()
        {
            MongoRunner.Dispose();
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            await SeedTestData();
        }

        private async Task SeedTestData()
        {
            var collection = Database.GetCollection<Invoice>("Invoices");
            await collection.InsertManyAsync(
            new List<Invoice>()
            {
                new Invoice { InvoiceId = "INV2025001", IssueDate = DateTime.UtcNow.AddDays(-10), DueDate = DateTime.UtcNow.AddDays(20), Amount = 5000.75m, Currency = "USD", PaymentStatus = "Paid" },
                new Invoice { InvoiceId = "INV2025002", IssueDate = DateTime.UtcNow.AddDays(-15), DueDate = DateTime.UtcNow.AddDays(15), Amount = 7500.50m, Currency = "EUR", PaymentStatus = "Pending" },
                new Invoice { InvoiceId = "INV2025003", IssueDate = DateTime.UtcNow.AddDays(-5), DueDate = DateTime.UtcNow.AddDays(30), Amount = 10234.00m, Currency = "USD", PaymentStatus = "Overdue" }
            });
        }
        private InvoiceRepository SetupRepository()
        {
            var settings = Options.Create(new MongoDbSettings
            {
                ConnectionString = MongoRunner.ConnectionString,
                DatabaseName = "TestDb"
            });

            var repository = new InvoiceRepository(settings);

            // Set in-memory database and collection using reflection
            typeof(MongoRepositoryBase<Invoice>)
                .GetProperty("Database", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(repository, Database);

            typeof(MongoRepositoryBase<Invoice>)
                .GetProperty("Collection", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(repository, Database.GetCollection<Invoice>("Invoices"));

            return repository;
        }
    }
}
