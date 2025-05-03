using ReportHub.Application.Features.Invoices.Handlers.QueryHandlers;
using ReportHub.Application.Features.Invoices.Queries;
using ReportHub.Application.Validators.Exceptions;
using ReportHub.Infrastructure.Repository;
using FluentAssertions;

namespace ReportHub.Tests.Application.Fixture
{
    public class GetAllInvoicesInADateRangeQueryHandler_Should : IClassFixture<MongoDbFixture>
    {
        private readonly InvoiceRepository _repository;

        public GetAllInvoicesInADateRangeQueryHandler_Should(MongoDbFixture fixture)
        {
            _repository = fixture.Repository;
        }

        [Fact]
        public async Task Handle_ShouldReturnCorrectCount_WhenInvoicesAreInDateRange()
        {
            // Arrange
            var handler = new GetAllInvoicesInADateRangeQueryHandler(_repository);
            var request = new GetAllInvoicesInADateRangeQuery(
                StartDate: DateTime.UtcNow.AddDays(-15),
                EndDate: DateTime.UtcNow.AddDays(25),
                ClientId: null,
                CustomerId: null,
                PageNumber: 1,
                PageSize: 10,
                SortingParameter: null
            );

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(6, result);
        }


        [Fact]
        public async Task Handle_ShouldFilterByClientId()
        {
            // Arrange
            var handler = new GetAllInvoicesInADateRangeQueryHandler(_repository);
            var request = new GetAllInvoicesInADateRangeQuery(
                StartDate: DateTime.UtcNow.AddDays(-15),
                EndDate: DateTime.UtcNow.AddDays(25),
                ClientId: "67fa2d8114e2389cd8064452", // Only 2 invoices have this ClientId
                CustomerId: null,
                PageNumber: 1,
                PageSize: 10,
                SortingParameter: null
            );

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(2, result);
        }


        [Fact]
        public async Task Handle_ShouldFilterByCustomerId()
        {
            // Arrange
            var handler = new GetAllInvoicesInADateRangeQueryHandler(_repository);
            var request = new GetAllInvoicesInADateRangeQuery(
                StartDate: DateTime.UtcNow.AddDays(-15),
                EndDate: DateTime.UtcNow.AddDays(25),
                ClientId: null,
                CustomerId: "67fa2d8114e2389cd806445a", // 2 invoices with this CustomerId
                PageNumber: 1,
                PageSize: 10,
                SortingParameter: null
            );

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(2, result);
        }


        [Fact]
        public async Task Handle_ShouldThrowBadRequestException_WhenStartDateIsAfterEndDate()
        {
            // Arrange
            var handler = new GetAllInvoicesInADateRangeQueryHandler(_repository);
            var request = new GetAllInvoicesInADateRangeQuery(
                StartDate: DateTime.UtcNow.AddDays(10),
                EndDate: DateTime.UtcNow.AddDays(-10),
                ClientId: null,
                CustomerId: null,
                PageNumber: 1,
                PageSize: 10,
                SortingParameter: null
            );

            // Act
            var act = async () => await handler.Handle(request, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Start date value can't be greater than end date");
        }


        [Fact]
        public async Task Handle_ShouldApplyPagination()
        {
            // Arrange
            var handler = new GetAllInvoicesInADateRangeQueryHandler(_repository);
            var request = new GetAllInvoicesInADateRangeQuery(
                StartDate: DateTime.UtcNow.AddDays(-15),
                EndDate: DateTime.UtcNow.AddDays(25),
                ClientId: null,
                CustomerId: null,
                PageNumber: 1,
                PageSize: 3, // Request only 3 items
                SortingParameter: null
            );

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().Be(3);
        }

    }
}
