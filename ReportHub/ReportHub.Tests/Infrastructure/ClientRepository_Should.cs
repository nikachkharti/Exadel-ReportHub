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




    }
}
