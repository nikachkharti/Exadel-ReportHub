using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;
using ReportHub.Infrastructure.Repository;
using System.Reflection;

namespace ReportHub.Tests.Infrastructure
{
    public class CustomerRepository_Should : IAsyncLifetime
    {
        private readonly MongoDbRunner _mongoRunner;
        private readonly IMongoDatabase _database;
        private readonly CustomerRepository _repository;

        public CustomerRepository_Should()
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
            var collection = _database.GetCollection<Customer>("customers");
            await collection.InsertManyAsync(new List<Customer>()
            {
               new Customer()
               {
                   Id = "67fa2d8114e2389cd8064457",
                   Name = "John Doe",
                   Address = "Doe street 12",
                   Email = "jonhode1@gmail.com"
               },
               new Customer()
               {
                   Id = "67fa2d8114e2389cd8064458",
                   Name = "Bill Butcher",
                   Address = "Butch street 1",
                   Email = "bb@gmail.com"
               },
               new Customer()
               {
                   Id = "67fa2d8114e2389cd8064459",
                   Name = "Freddy Krueger",
                   Address = "Krug street 31",
                   Email = "freddy@gmail.com"
               },
               new Customer()
               {
                   Id = "67fa2d8114e2389cd806445a",
                   Name = "John Cenna",
                   Address = "Cen street 20",
                   Email = "joncena@gmail.com"
               }
            });
        }

        private CustomerRepository SetupRepository()
        {
            var settings = Options.Create(new MongoDbSettings
            {
                ConnectionString = _mongoRunner.ConnectionString,
                DatabaseName = "TestDb"
            });

            var repository = new CustomerRepository(settings);

            // Set in-memory database and collection using reflection
            typeof(MongoRepositoryBase<Customer>)
                .GetProperty("Database", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(repository, _database);

            typeof(MongoRepositoryBase<Customer>)
                .GetProperty("Collection", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(repository, _database.GetCollection<Client>("customers"));

            return repository;
        }

        #endregion



        #region GET

        [Fact]
        public async Task Return_All_Customers()
        {
            var result = await _repository.GetAll();
            Assert.NotNull(result);
            var customerList = Assert.IsAssignableFrom<IEnumerable<Customer>>(result);
            Assert.Equal(4, customerList.Count());
        }

        [Fact]
        public async Task Return_All_Customers_Sorted_By_Name_Ascending()
        {
            var result = await _repository.GetAll(c => c.Name, ascending: true);
            var sortedList = result.ToList();

            Assert.Equal(4, sortedList.Count);
            Assert.Equal("Bill Butcher", sortedList[0].Name);
            Assert.Equal("Freddy Krueger", sortedList[1].Name);
            Assert.Equal("John Cenna", sortedList[2].Name);
            Assert.Equal("John Doe", sortedList[3].Name);
        }

        [Fact]
        public async Task Return_All_Customers_Sorted_By_Email_Descending()
        {
            var result = await _repository.GetAll(c => c.Email, ascending: false);
            var sortedList = result.ToList();

            Assert.Equal(4, sortedList.Count);
            Assert.Equal("jonhode1@gmail.com", sortedList[0].Email);
            Assert.Equal("joncena@gmail.com", sortedList[1].Email);
            Assert.Equal("freddy@gmail.com", sortedList[2].Email);
            Assert.Equal("bb@gmail.com", sortedList[3].Email);
        }

        [Fact]
        public async Task Return_Single_Customer_By_Id()
        {
            var result = await _repository.Get(c => c.Id == "67fa2d8114e2389cd8064457");

            Assert.NotNull(result);
            Assert.Equal("John Doe", result.Name);
            Assert.Equal("Doe street 12", result.Address);
            Assert.Equal("jonhode1@gmail.com", result.Email);
        }

        [Fact]
        public async Task Return_Null_When_Customer_Not_Found()
        {
            var result = await _repository.Get(c => c.Name == "Non Existent");
            Assert.Null(result);
        }


        #endregion


        #region INSERT
        [Fact]
        public async Task Insert_New_Customer()
        {
            var beforeCount = (await _repository.GetAll()).Count();
            var newCustomer = new Customer()
            {
                Name = "New Customer",
                Address = "New Address",
                Email = "new@example.com"
            };

            await _repository.Insert(newCustomer);

            var afterCount = (await _repository.GetAll()).Count();
            Assert.Equal(beforeCount + 1, afterCount);
        }

        [Fact]
        public async Task Insert_Multiple_Customers()
        {
            var beforeCount = (await _repository.GetAll()).Count();

            var newCustomers = new List<Customer>
            {
                new Customer { Name = "Customer A", Address = "Addr A", Email = "a@email.com" },
                new Customer { Name = "Customer B", Address = "Addr B", Email = "b@email.com" }
            };

            await _repository.InsertMultiple(newCustomers);

            var afterCount = (await _repository.GetAll()).Count();
            Assert.Equal(beforeCount + 2, afterCount);
        }
        #endregion


        #region DELETE

        [Fact]
        public async Task Delete_Customer()
        {
            var beforeCount = (await _repository.GetAll()).Count();

            await _repository.Delete(c => c.Id == "67fa2d8114e2389cd806445a");

            var afterCount = (await _repository.GetAll()).Count();
            Assert.Equal(beforeCount - 1, afterCount);
        }

        [Fact]
        public async Task Not_Delete_If_Customer_Not_Found()
        {
            var beforeCount = (await _repository.GetAll()).Count();

            await _repository.Delete(c => c.Id == "67fa2d8114e2389cd806443a");

            var afterCount = (await _repository.GetAll()).Count();
            Assert.Equal(beforeCount, afterCount);
        }

        #endregion


    }
}
