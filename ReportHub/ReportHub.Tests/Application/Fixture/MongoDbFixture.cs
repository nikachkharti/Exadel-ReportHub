using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;
using ReportHub.Infrastructure.Repository;
using System.Reflection;

namespace ReportHub.Tests.Application.Fixture
{
    public class MongoDbFixture : IAsyncLifetime, IDisposable
    {
        private readonly MongoDbRunner _mongoRunner;
        public IMongoDatabase Database { get; }
        public InvoiceRepository Repository { get; }


        public MongoDbFixture()
        {
            _mongoRunner = MongoDbRunner.Start();
            Database = new MongoClient(_mongoRunner.ConnectionString).GetDatabase("TestDb");
            Repository = SetupRepository();
        }

        private InvoiceRepository SetupRepository()
        {
            var settings = Options.Create(new MongoDbSettings
            {
                ConnectionString = _mongoRunner.ConnectionString,
                DatabaseName = "TestDb"
            });

            var repository = new InvoiceRepository(settings);

            typeof(MongoRepositoryBase<Invoice>)
                .GetProperty("Database", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(repository, Database);

            typeof(MongoRepositoryBase<Invoice>)
                .GetProperty("Collection", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(repository, Database.GetCollection<Invoice>("invoices"));

            return repository;
        }

        public async Task InitializeAsync()
        {
            var collection = Database.GetCollection<Invoice>("invoices");
            await collection.InsertManyAsync(new List<Invoice>()
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
            });
        }


        public void Dispose()
        {
            _mongoRunner?.Dispose();
        }

        public Task DisposeAsync()
        {
            _mongoRunner.Dispose();
            return Task.CompletedTask;
        }

    }

}
