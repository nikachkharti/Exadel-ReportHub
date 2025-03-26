using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Helper;
using ReportHub.Infrastructure.Repository;
using System.Linq.Expressions;
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



        #region UPDATE

        [Fact]
        public async Task UpdateSingleField_ShouldUpdateOnlySpecifiedField()
        {
            // Arrange
            var filter = Builders<Invoice>.Filter.Eq(i => i.InvoiceId, "INV2025001");
            var originalInvoice = (await _repository.GetAll(i => i.InvoiceId == "INV2025001")).First();
            var newAmount = 999.00m; // New amount to set

            // Act
            await _repository.UpdateSingleField(i => i.InvoiceId == "INV2025001", i => i.Amount, newAmount);
            var updatedInvoice = (await _repository.GetAll(i => i.InvoiceId == "INV2025001")).First();

            // Assert
            Assert.Equal(newAmount, updatedInvoice.Amount); // Ensure Amount is updated
            Assert.Equal(originalInvoice.InvoiceId, updatedInvoice.InvoiceId); // Other fields remain unchanged
            Assert.Equal(originalInvoice.Currency, updatedInvoice.Currency);
        }

        [Fact]
        public async Task UpdateSingleField_ShouldNotModify_WhenInvoiceDoesNotExist()
        {
            // Arrange
            var beforeCount = (await _repository.GetAll()).Count();

            // Act
            await _repository.UpdateSingleField(i => i.InvoiceId == "INV9999", i => i.Amount, 500);

            // Assert
            var afterCount = (await _repository.GetAll()).Count();
            Assert.Equal(beforeCount, afterCount); // Count remains unchanged
        }

        [Fact]
        public async Task UpdateMultipleFields_ShouldUpdateOnlySpecifiedFields()
        {
            // Arrange
            var filter = Builders<Invoice>.Filter.Eq(i => i.InvoiceId, "INV2025001");
            var originalInvoice = (await _repository.GetAll(i => i.InvoiceId == "INV2025001")).First();

            var updates = new Dictionary<Expression<Func<Invoice, object>>, object>()
            {
                { i => i.Amount, 999.00m },
                { i => i.Currency, "GEL" }
            };

            // Act
            await _repository.UpdateMultipleFields(i => i.InvoiceId == "INV2025001", updates);
            var updatedInvoice = (await _repository.GetAll(i => i.InvoiceId == "INV2025001")).First();

            // Assert
            Assert.Equal(999.00m, updatedInvoice.Amount); // Amount is updated
            Assert.Equal("GEL", updatedInvoice.Currency); // Currency is updated
            Assert.Equal(originalInvoice.PaymentStatus, updatedInvoice.PaymentStatus); // Payment status remains unchanged
        }


        [Fact]
        public async Task UpdateMultipleFields_ShouldNotModify_WhenInvoiceDoesNotExist()
        {
            // Arrange
            var beforeInvoices = await _repository.GetAll();

            var updates = new Dictionary<Expression<Func<Invoice, object>>, object>()
            {
                { i => i.Amount, 500 },
                { i => i.Currency, "CAD" }
            };

            // Act
            await _repository.UpdateMultipleFields(i => i.InvoiceId == "INV9999", updates);

            // Assert
            var afterInvoices = await _repository.GetAll();
            Assert.Equal(beforeInvoices.Count(), afterInvoices.Count()); // Count remains unchanged
        }

        #endregion

        #region INSERT

        [Fact]
        public async Task Insert_ShouldIncreaseCount_WhenNewInvoiceInserted()
        {
            var beforeCount = (await _repository.GetAll()).Count();
            var newInvoice = new Invoice()
            {
                InvoiceId = "INV2025004",
                IssueDate = DateTime.UtcNow.AddDays(-2),
                DueDate = DateTime.UtcNow.AddDays(30),
                Amount = 87.00m,
                Currency = "GBP",
                PaymentStatus = "Paid"
            };

            await _repository.Insert(newInvoice);

            var afterCount = (await _repository.GetAll()).Count();
            Assert.Equal(beforeCount + 1, afterCount);
        }


        [Fact]
        public async Task InsertMultiple_ShouldIncreaseCount_WhenNewInvoicesInserted()
        {
            var beforeCount = (await _repository.GetAll()).Count();
            var newInvoices = new List<Invoice>()
            {
                new Invoice { InvoiceId = "INV2025004", IssueDate = DateTime.UtcNow.AddDays(-5), DueDate = DateTime.UtcNow.AddDays(30), Amount = 10234.00m, Currency = "GBP", PaymentStatus = "Overdue" },
                new Invoice { InvoiceId = "INV2025005", IssueDate = DateTime.UtcNow.AddDays(-5), DueDate = DateTime.UtcNow.AddDays(30), Amount = 10234.00m, Currency = "GEL", PaymentStatus = "Paid" }
            };

            await _repository.InsertMultiple(newInvoices);

            var afterCount = (await _repository.GetAll()).Count();
            Assert.Equal(beforeCount + 2, afterCount);
        }

        #endregion

        #region DELETE

        [Fact]
        public async Task Delete_ShouldDecreaseCount_WhenInvoiceExists()
        {
            var beforeCount = (await _repository.GetAll()).Count();
            Expression<Func<Invoice, bool>> filter = invoice => invoice.InvoiceId == "INV2025001";

            await _repository.Delete(filter);

            var afterCount = (await _repository.GetAll()).Count();
            Assert.Equal(beforeCount - 1, afterCount);
        }

        [Fact]
        public async Task Delete_ShouldNotChangeCount_WhenInvoiceDoesNotExist()
        {
            var beforeCount = (await _repository.GetAll()).Count();
            Expression<Func<Invoice, bool>> filter = invoice => invoice.InvoiceId == "NON_EXISTENT";

            await _repository.Delete(filter);

            var afterCount = (await _repository.GetAll()).Count();
            Assert.Equal(beforeCount, afterCount);
        }

        [Fact]
        public async Task DeleteMultiple_ShouldDecreaseCount_WhenAmountAbove200()
        {
            var beforeCount = (await _repository.GetAll()).Count();
            Expression<Func<Invoice, bool>> filter = invoice => invoice.Amount > 200;

            await _repository.DeleteMultiple(filter);

            var afterCount = (await _repository.GetAll()).Count();
            Assert.Equal(beforeCount - 3, afterCount); // Three invoices have Amount > 200
        }


        #endregion

        #region GET


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


        [Fact]
        public async Task GetAll_ShouldReturnFirstPage()
        {
            var result = await _repository.GetAll(1, 2);

            Assert.NotNull(result);
            var pagedList = result.ToList();

            Assert.Equal(2, pagedList.Count);
            Assert.Equal("INV2025001", pagedList[0].InvoiceId);
            Assert.Equal("INV2025002", pagedList[1].InvoiceId);
        }


        [Fact]
        public async Task GetAll_ShouldReturnSecondPage()
        {
            var result = await _repository.GetAll(pageNumber: 2, pageSize: 1);

            Assert.NotNull(result);
            var pagedList = result.ToList();

            Assert.Single(pagedList);
            Assert.Equal("INV2025002", pagedList[0].InvoiceId);
        }


        [Fact]
        public async Task GetAll_ShouldReturnEmptyForOutOfRangePage()
        {
            var result = await _repository.GetAll(4, 2);

            Assert.NotNull(result);
            Assert.Empty(result);
        }


        [Fact]
        public async Task GetAll_ShouldReturnAllForLargePageSize()
        {
            var result = await _repository.GetAll(1, 10);

            Assert.NotNull(result);
            var pagedList = result.ToList();

            Assert.Equal(3, pagedList.Count);
        }


        [Fact]
        public async Task GetAll_ShouldReturnFirstPageSortedByAmountAscending()
        {
            var result = await _repository.GetAll(1, 2, invoice => invoice.Amount);

            Assert.NotNull(result);
            var pagedList = result.ToList();

            Assert.Equal(2, pagedList.Count);
            Assert.Equal(5000.75m, pagedList[0].Amount); // Lowest amount
            Assert.Equal(7500.50m, pagedList[1].Amount);
        }


        [Fact]
        public async Task GetAll_ShouldReturnFirstPageSortedByAmountDescending()
        {
            var result = await _repository.GetAll(1, 2, invoice => invoice.Amount, ascending: false);

            Assert.NotNull(result);
            var pagedList = result.ToList();

            Assert.Equal(2, pagedList.Count);
            Assert.Equal(10234.00m, pagedList[0].Amount); // Highest amount
            Assert.Equal(7500.50m, pagedList[1].Amount);
        }


        [Fact]
        public async Task GetAll_ShouldSortByInvoiceIdAscending()
        {
            var result = await _repository.GetAll(1, 2, invoice => invoice.InvoiceId);

            Assert.NotNull(result);
            var pagedList = result.ToList();

            Assert.Equal(2, pagedList.Count);
            Assert.Equal("INV2025001", pagedList[0].InvoiceId);
            Assert.Equal("INV2025002", pagedList[1].InvoiceId);
        }


        [Fact]
        public async Task Get_ShouldReturnInvoiceByInvoiceId()
        {
            Expression<Func<Invoice, bool>> filter = invoice => invoice.InvoiceId == "INV2025003";
            var result = await _repository.Get(filter);

            Assert.NotNull(result);
            Assert.Equal("INV2025003", result.InvoiceId);
            Assert.Equal(10234.00m, result.Amount);
        }


        [Fact]
        public async Task Get_ShouldReturnFirstMatchingInvoiceByCurrency()
        {
            Expression<Func<Invoice, bool>> filter = invoice => invoice.Currency == "USD";
            var result = await _repository.Get(filter);

            Assert.NotNull(result);
            Assert.Equal("USD", result.Currency);
            // The first inserted invoice with USD currency is InvoiceId "2025001"
            Assert.Equal("INV2025001", result.InvoiceId);
        }


        [Fact]
        public async Task Get_ShouldReturnNullWhenNoMatchFound()
        {
            Expression<Func<Invoice, bool>> filter = invoice => invoice.Currency == "AUD";
            var result = await _repository.Get(filter);

            Assert.Null(result);
        }

        [Fact]
        public async Task Get_ShouldReturnFirstInvoiceMatchingAmountGreaterThan200()
        {
            Expression<Func<Invoice, bool>> filter = invoice => invoice.Amount > 200;
            var result = await _repository.Get(filter);

            Assert.NotNull(result);
            Assert.True(result.Amount > 200);
        }


        [Fact]
        public async Task Get_ShouldReturnNullForInvalidFieldFilter()
        {
            Expression<Func<Invoice, bool>> filter = invoice => invoice.InvoiceId == "A999";
            var result = await _repository.Get(filter);

            Assert.Null(result);
        }

        #endregion
    }
}
