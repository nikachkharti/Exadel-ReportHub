using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;
using ReportHub.Infrastructure.Repository;
using System.Linq.Expressions;
using System.Reflection;

namespace ReportHub.Tests.Infrastructure
{
    public class ClientRepository_Should : IAsyncLifetime
    {
        private readonly MongoDbRunner _mongoRunner;
        private readonly IMongoDatabase _database;
        private readonly ClientRepository _repository;

        public ClientRepository_Should()
        {
            _mongoRunner = MongoDbRunner.Start();
            _database = new MongoClient(_mongoRunner.ConnectionString).GetDatabase("TestDb");
            _repository = SetupRepository();
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
            var collection = _database.GetCollection<Client>("clients");
            await collection.InsertManyAsync(new List<Client>()
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
        }

        private ClientRepository SetupRepository()
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


        #region GET

        [Fact]
        public async Task Return_All_Clients()
        {
            var result = await _repository.GetAll();
            Assert.NotNull(result);
            var clientList = Assert.IsAssignableFrom<IEnumerable<Client>>(result);
            Assert.Equal(3, clientList.Count());
        }


        [Fact]
        public async Task Return_All_Clients_Sorted_By_Id_Ascending()
        {
            var result = await _repository.GetAll(c => c.Id, ascending: true);

            Assert.NotNull(result);
            var sortedList = result.ToList();

            Assert.Equal(3, sortedList.Count);
            Assert.Equal("67fa2d8114e2389cd8064452", sortedList[0].Id);
            Assert.Equal("67fa2d8114e2389cd8064453", sortedList[1].Id);
            Assert.Equal("67fa2d8114e2389cd8064454", sortedList[2].Id);
        }


        [Fact]
        public async Task Return_All_Clients_Sorted_By_Id_Descending()
        {
            var result = await _repository.GetAll(c => c.Id, ascending: false);

            Assert.NotNull(result);
            var sortedList = result.ToList();

            Assert.Equal(3, sortedList.Count);
            Assert.Equal("67fa2d8114e2389cd8064454", sortedList[0].Id);
            Assert.Equal("67fa2d8114e2389cd8064453", sortedList[1].Id);
            Assert.Equal("67fa2d8114e2389cd8064452", sortedList[2].Id);
        }


        [Fact]
        public async Task Return_All_Clients_Sorted_By_Name_Ascending()
        {
            var result = await _repository.GetAll(c => c.Name, ascending: true);

            Assert.NotNull(result);
            var sortedList = result.ToList();

            Assert.Equal(3, sortedList.Count);
            Assert.Equal("Alpha Soft", sortedList[0].Name);
            Assert.Equal("Brick CO", sortedList[1].Name);
            Assert.Equal("Eduworld", sortedList[2].Name);
        }


        [Fact]
        public async Task Return_All_Clients_Sorted_By_Name_Descending()
        {
            var result = await _repository.GetAll(c => c.Name, ascending: false);

            Assert.NotNull(result);
            var sortedList = result.ToList();

            Assert.Equal(3, sortedList.Count);
            Assert.Equal("Eduworld", sortedList[0].Name);
            Assert.Equal("Brick CO", sortedList[1].Name);
            Assert.Equal("Alpha Soft", sortedList[2].Name);
        }


        [Fact]
        public async Task Return_First_Page()
        {
            var result = (await _repository.GetAll(1, 2)).ToList();

            Assert.NotNull(result);

            Assert.Equal(2, result.Count);
            Assert.Equal("67fa2d8114e2389cd8064452", result[0].Id);
            Assert.Equal("67fa2d8114e2389cd8064453", result[1].Id);
        }

        [Fact]
        public async Task Return_Last_Page()
        {
            var result = (await _repository.GetAll(2, 1)).ToList();

            Assert.NotNull(result);

            Assert.Single(result);
            Assert.Equal("67fa2d8114e2389cd8064453", result[0].Id);
        }


        [Fact]
        public async Task Return_Empty_For_OutOfRangePage()
        {
            var result = await _repository.GetAll(4, 2);

            Assert.NotNull(result);
            Assert.Empty(result);
        }


        [Fact]
        public async Task Return_Single_Client_By_Id()
        {
            Expression<Func<Client, bool>> filter = client => client.Id == "67fa2d8114e2389cd8064452";
            var result = await _repository.Get(filter);

            Assert.NotNull(result);
            Assert.Equal("67fa2d8114e2389cd8064452", result.Id);
            Assert.Equal("Alpha Soft", result.Name);
            Assert.Equal("Software Development", result.Specialization);
        }


        [Fact]
        public async Task Return_Null_When_No_MatchFound()
        {
            Expression<Func<Client, bool>> filter = invoice => invoice.Name == "Test";
            var result = await _repository.Get(filter);

            Assert.Null(result);
        }



        #endregion

        #region INSERT

        [Fact]
        public async Task Insert_New_Client()
        {
            var beforeCount = (await _repository.GetAll()).Count();
            var newClient = new Client()
            {
                Name = "Eduworld",
                Specialization = "Education and schoolarship"
            };

            await _repository.Insert(newClient);

            var afterCount = (await _repository.GetAll()).Count();
            Assert.Equal(beforeCount + 1, afterCount);
        }


        [Fact]
        public async Task Insert_Multiple_New_Clients()
        {
            var beforeCount = (await _repository.GetAll()).Count();
            var newClients = new List<Client>()
            {
                new Client() {Name = "Test Client #1",Specialization = "Software development"},
                new Client() {Name = "Test Client #2",Specialization = "Softwre development"}
            };

            await _repository.InsertMultiple(newClients);

            var afterCount = (await _repository.GetAll()).Count();
            Assert.Equal(beforeCount + 2, afterCount);
        }

        #endregion


        #region DELETE

        [Fact]
        public async Task Delete_Client()
        {
            var beforeCount = (await _repository.GetAll()).Count();

            await _repository.Delete(c => c.Id == "67fa2d8114e2389cd8064454");

            var afterCount = (await _repository.GetAll()).Count();
            Assert.Equal(beforeCount - 1, afterCount);
        }


        [Fact]
        public async Task Not_Delete_When_Client_Not_Found()
        {
            var beforeCount = (await _repository.GetAll()).Count();

            await _repository.Delete(c => c.Id == "67fa2d8114e2389cd8064456");

            var afterCount = (await _repository.GetAll()).Count();
            Assert.Equal(beforeCount, afterCount);
        }


        #endregion


        #region UPDATE

        [Fact]
        public async Task Update_Specific_Field()
        {
            // Arrange
            var originalClient = (await _repository.GetAll(c => c.Id == "67fa2d8114e2389cd8064452")).First();
            var newName = "Updated alpha soft";

            // Act
            await _repository.UpdateSingleField(c => c.Id == "67fa2d8114e2389cd8064452", c => c.Name, newName);
            var updatedClient = await _repository.Get(c => c.Id == "67fa2d8114e2389cd8064452");

            // Assert
            Assert.Equal(newName, updatedClient.Name);
            Assert.Equal(originalClient.Id, updatedClient.Id); // Other fields remain unchanged
            Assert.Equal(originalClient.Specialization, updatedClient.Specialization);
        }

        [Fact]
        public async Task Not_Update_If_Client_Not_Found()
        {
            // Arrange
            var beforeCount = (await _repository.GetAll()).Count();

            // Act
            await _repository.UpdateSingleField(c => c.Id == "67fa2d8114e2389cd8064457", c => c.Specialization, "TEST");

            // Assert
            var afterCount = (await _repository.GetAll()).Count();
            Assert.Equal(beforeCount, afterCount); // Count remains unchanged
        }


        [Fact]
        public async Task Update_Multiple_Fields()
        {
            // Arrange
            var originalInvoice = (await _repository.GetAll(c => c.Id == "67fa2d8114e2389cd8064452")).First();

            var updates = new Dictionary<Expression<Func<Client, object>>, object>()
            {
                { c => c.Name, "TEST NAME" },
                { c => c.Specialization, "TEST SPECIALIZATION" }
            };

            // Act
            await _repository.UpdateMultipleFields(c => c.Name == "Alpha Soft", updates);
            var updatedClient = (await _repository.Get(c => c.Id == "67fa2d8114e2389cd8064452"));

            // Assert
            Assert.Equal("TEST NAME", updatedClient.Name); // Name is updated
            Assert.Equal("TEST SPECIALIZATION", updatedClient.Specialization); // Specialization is updated
        }

        #endregion

    }
}
