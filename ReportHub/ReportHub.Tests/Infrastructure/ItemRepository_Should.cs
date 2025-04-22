using DocumentFormat.OpenXml.Wordprocessing;
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
    public class ItemRepository_Should : IAsyncLifetime
    {
        private readonly MongoDbRunner _mongoRunner;
        private readonly IMongoDatabase _database;
        private readonly ItemRepository _itemRepository;
        private readonly ClientRepository _clientRepository;

        public ItemRepository_Should()
        {
            _mongoRunner = MongoDbRunner.Start();
            _database = new MongoClient(_mongoRunner.ConnectionString).GetDatabase("TestDb");
            _clientRepository = SetupClientRepository();
            _itemRepository = SetupItemRepository();
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
            var clientsCollection = _database.GetCollection<Client>("clients");
            var itemsCollection = _database.GetCollection<Item>("items");

            await clientsCollection.InsertManyAsync(new List<Client>()
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

        #endregion


        [Fact]
        public async Task Add_New_Item()
        {
            var newItem = new Item
            {
                Id = ObjectId.GenerateNewId().ToString(),
                ClientId = "67fa2d8114e2389cd8064452",
                Name = "New Software Development",
                Currency = "USD",
                Description = "A new project",
                Price = 5000.00m
            };

            await _itemRepository.Insert(newItem);

            var insertedItem = await _itemRepository.Get(i => i.Id == newItem.Id);

            Assert.NotNull(insertedItem);
            Assert.Equal("New Software Development", insertedItem.Name);
        }

        [Fact]
        public async Task Return_Correct_Item()
        {
            var itemId = "67fa2d8114e2389cd8064460";

            var item = await _itemRepository.Get(i => i.Id == itemId);

            Assert.NotNull(item);
            Assert.Equal("CRM system building", item.Name);
        }

        [Fact]
        public async Task Update_Existing_Item()
        {
            // Arrange
            var originalItem = await _itemRepository.Get(i => i.Id == "67fa2d8114e2389cd8064461");
            var newPrice = 1200.00m;
            var updatedDocument = new Item()
            {
                Id = originalItem.Id,
                ClientId = originalItem.ClientId,
                Currency = originalItem.Currency,
                Description = originalItem.Description,
                Name = originalItem.Name,
                Price = newPrice
            };

            // Act
            await _itemRepository.UpdateSingleDocument(i => i.Id == "67fa2d8114e2389cd8064461", updatedDocument);
            var updatedItem = await _itemRepository.Get(c => c.Id == "67fa2d8114e2389cd8064461");

            // Assert
            Assert.Equal(newPrice, updatedItem.Price);
            Assert.Equal(originalItem.Id, updatedItem.Id); // Other fields remain unchanged
            Assert.Equal(originalItem.Description, updatedItem.Description);
        }


        [Fact]
        public async Task Remove_Item()
        {
            var itemId = "67fa2d8114e2389cd8064461";
            await _itemRepository.Delete(i => i.Id == itemId);

            var deletedItem = await _itemRepository.Get(i => i.Id == itemId);
            Assert.NotNull(deletedItem);
            Assert.True(deletedItem.IsDeleted);
        }

        [Fact]
        public async Task GetAll_Should_Return_All_Items()
        {
            var items = await _itemRepository.GetAll();

            Assert.NotEmpty(items);
            Assert.Equal(7, items.Count()); // Initial seeded items
        }


        [Fact]
        public async Task Return_Correct_Items_Of_Client()
        {
            var clientId = "67fa2d8114e2389cd8064452";

            var items = await _itemRepository.GetAll(x => x.ClientId == clientId);

            Assert.All(items, i => Assert.Equal(clientId, i.ClientId));
            Assert.Equal(3, items.Count());
        }
    }
}
