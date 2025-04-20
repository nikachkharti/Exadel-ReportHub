using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;
using ReportHub.Infrastructure.Repository;
using System.Linq.Expressions;
using System.Reflection;

namespace ReportHub.Tests.Infrastructure
{
    public class PlanRepository_Should : IAsyncLifetime
    {
        private readonly MongoDbRunner _mongoRunner;
        private readonly IMongoDatabase _database;
        private readonly PlanRepository _planRepository;
        private readonly ClientRepository _clientRepository;
        private readonly ItemRepository _itemRepository;

        public PlanRepository_Should()
        {
            _mongoRunner = MongoDbRunner.Start();
            _database = new MongoClient(_mongoRunner.ConnectionString).GetDatabase("TestDb");
            _clientRepository = SetupClientRepository();
            _itemRepository = SetupItemRepository();
            _planRepository = SetupPlanRepository();
        }

        #region CONFIGURATION
        public Task DisposeAsync()
        {
            _mongoRunner.Dispose();
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            await SeedTestData();
        }

        private async Task SeedTestData()
        {
            var plansCollection = _database.GetCollection<Plan>("plans");
            var clientCollection = _database.GetCollection<Client>("clients");
            var itemsCollection = _database.GetCollection<Item>("items");

            await clientCollection.InsertManyAsync(new List<Client>()
            {
                new Client()
                {
                    Id = "67fa2d8114e2389cd8064452",
                    Name = "Alpha Soft",
                    Specialization = "Software Development"
                },
                new Client()
                {
                    Id = "67fa2d8114e2389cd8064453",
                    Name = "Brick CO",
                    Specialization = "Builiding and Development"
                },
                new Client()
                {
                    Id = "67fa2d8114e2389cd8064454",
                    Name = "Eduworld",
                    Specialization = "Education and schoolarship"
                }
            });
            await itemsCollection.InsertManyAsync(new List<Item>()
            {
                new Item()
                        {
                            Id = "67fa2d8114e2389cd8064460",
                            ClientId = "67fa2d8114e2389cd8064452",
                            Name = "CRM system building",
                            Currency = "USD",
                            Description = "Building of CRM system for small and medium companies",
                            Price = 3000.00m
                        },
                new Item()
                        {
                            Id = "67fa2d8114e2389cd8064461",
                            ClientId = "67fa2d8114e2389cd8064452",
                            Name = "Landing page building",
                            Currency = "USD",
                            Description = "Building of landing page",
                            Price = 1000.00m
                        },
                new Item()
                        {
                            Id = "67fa2d8114e2389cd8064462",
                            ClientId = "67fa2d8114e2389cd8064452",
                            Name = "AML monitoring",
                            Currency = "USD",
                            Description = "Providing an AML monitoring service for small and medium companies",
                            Price = 18000.00m
                        },
                new Item()
                        {
                            Id = "67fa2d8114e2389cd8064463",
                            ClientId = "67fa2d8114e2389cd8064453",
                            Name = "Villa Building",
                            Currency = "EUR",
                            Description = "Villa building",
                            Price = 180000.00m
                        },
                new Item()
                        {
                            Id = "67fa2d8114e2389cd8064464",
                            ClientId = "67fa2d8114e2389cd8064453",
                            Name = "Destroy service",
                            Currency = "EUR",
                            Description = "Destroy service for old buildings",
                            Price = 9000.00m
                        },
                new Item()
                        {
                            Id = "67fa2d8114e2389cd8064465",
                            ClientId = "67fa2d8114e2389cd8064454",
                            Name = "Course of frontend software development",
                            Currency = "USD",
                            Description = "Learn HTML CSS and Javascript",
                            Price = 350.00m
                        },
                new Item()
                        {
                            Id = "67fa2d8114e2389cd8064466",
                            ClientId = "67fa2d8114e2389cd8064454",
                            Name = "Course of fullstrack software development",
                            Currency = "USD",
                            Description = "Learn full stack programming",
                            Price = 700.00m
                        }
            });
            await plansCollection.InsertManyAsync(new List<Plan>()
            {
                new Plan()
                        {
                            Id = "680234508ed022f95e0789d9",
                            ClientId = "67fa2d8114e2389cd8064452",
                            ItemId = "67fa2d8114e2389cd8064460",
                            Amount = 3000.00m,
                            StartDate = DateTime.UtcNow.AddDays(1),
                            EndDate = DateTime.UtcNow.AddMonths(3),
                            Status = PlanStatus.Planned
                        },
                new Plan()
                        {
                            Id = "680234508ed022f95e0789da",
                            ClientId = "67fa2d8114e2389cd8064453",
                            ItemId = "67fa2d8114e2389cd8064464",
                            Amount = 9000.00m,
                            StartDate = DateTime.UtcNow.AddDays(10),
                            EndDate = DateTime.UtcNow.AddMonths(6),
                            Status = PlanStatus.Planned
                        },
                new Plan()
                        {
                            Id = "680234508ed022f95e0789db",
                            ClientId = "67fa2d8114e2389cd8064454",
                            ItemId = "67fa2d8114e2389cd8064465",
                            Amount = 350.00m,
                            StartDate = DateTime.UtcNow.AddDays(-5),
                            EndDate = DateTime.UtcNow.AddMonths(1),
                            Status = PlanStatus.InProgress
                        }
            });
        }


        private ItemRepository SetupItemRepository()
        {
            var settings = Options.Create(new MongoDbSettings
            {
                ConnectionString = _mongoRunner.ConnectionString,
                DatabaseName = "TestDb"
            });

            var repository = new ItemRepository(settings);

            // Set in-memory database and collection using reflection
            typeof(MongoRepositoryBase<Item>)
                .GetProperty("Database", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(repository, _database);

            typeof(MongoRepositoryBase<Item>)
                .GetProperty("Collection", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(repository, _database.GetCollection<Item>("items"));

            return repository;
        }
        private ClientRepository SetupClientRepository()
        {
            var settings = Options.Create(new MongoDbSettings
            {
                ConnectionString = _mongoRunner.ConnectionString,
                DatabaseName = "TestDb"
            });

            var repository = new ClientRepository(settings);

            // Set in-memory database and collection using reflection
            typeof(MongoRepositoryBase<Client>)
                .GetProperty("Database", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(repository, _database);

            typeof(MongoRepositoryBase<Client>)
                .GetProperty("Collection", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(repository, _database.GetCollection<Client>("clients"));

            return repository;
        }
        private PlanRepository SetupPlanRepository()
        {
            var settings = Options.Create(new MongoDbSettings
            {
                ConnectionString = _mongoRunner.ConnectionString,
                DatabaseName = "TestDb"
            });

            var repository = new PlanRepository(settings);

            // Set in-memory database and collection using reflection
            typeof(MongoRepositoryBase<Plan>)
                .GetProperty("Database", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(repository, _database);

            typeof(MongoRepositoryBase<Plan>)
                .GetProperty("Collection", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(repository, _database.GetCollection<Plan>("plans"));

            return repository;
        }

        #endregion

        [Fact]
        public async Task GetAllPlans_Of_Client()
        {
            var allPlans = await _planRepository.GetAll(p => p.ClientId == "67fa2d8114e2389cd8064452");
            Assert.NotNull(allPlans);
            Assert.Single(allPlans);
        }

        [Fact]
        public async Task Get_Plan_By_Id()
        {
            var planId = "680234508ed022f95e0789d9";

            var plan = await _planRepository.Get(p => p.Id == planId);

            Assert.NotNull(plan);
            Assert.Equal(planId, plan.Id);
            Assert.Equal(3000.00m, plan.Amount);
        }

        [Fact]
        public async Task Delete_Plan()
        {
            var planId = "680234508ed022f95e0789db";

            await _planRepository.Delete(p => p.Id == planId);
            var deleted = await _planRepository.Get(p => p.Id == planId);

            Assert.Null(deleted);
        }

        [Fact]
        public async Task Update_Existing_Plan()
        {
            // Arrange
            var planId = "680234508ed022f95e0789d9";
            var originalPlan = await _planRepository.Get(p => p.Id == planId);
            var updates = new Dictionary<Expression<Func<Plan, object>>, object>()
            {
                { c => c.Amount, 111.15m },
                { c => c.Status,PlanStatus.Completed }
            };

            // Act
            await _planRepository.UpdateMultipleFields(p => p.Amount == 3000.00m, updates);
            var updatedPlan = await _planRepository.Get(p => p.Id == planId);

            // Assert
            Assert.Equal(111.15m, updatedPlan.Amount); // Amount is updated
            Assert.Equal(PlanStatus.Completed, updatedPlan.Status); // Status is updated
        }


        [Fact]
        public async Task CreatePlan_ShouldInsertNewPlan()
        {
            var newPlan = new Plan()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                ClientId = "67fa2d8114e2389cd8064452",
                ItemId = "67fa2d8114e2389cd8064460",
                Amount = 5000.00m,
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddMonths(2),
                Status = PlanStatus.Planned
            };

            await _planRepository.Insert(newPlan);

            var inserted = await _planRepository.Get(p => p.Id == newPlan.Id);

            Assert.NotNull(inserted);
            Assert.Equal(newPlan.ClientId, inserted.ClientId);
            Assert.Equal(5000.00m, inserted.Amount);
        }

    }
}
