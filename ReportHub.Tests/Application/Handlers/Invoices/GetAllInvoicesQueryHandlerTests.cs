using Moq;
using AutoMapper;
using ReportHub.Application.Features.Invoices.Mapping;
using ReportHub.Application.Features.Invoices.Handlers.QueryHandlers;
using ReportHub.Application.Features.Invoices.Queries;
using ReportHub.Application.Contracts.RepositoryContracts;

namespace ReportHub.Tests.Application.Handlers.Invoices
{
    public class GetAllInvoicesQueryHandlerTests
    {
        private readonly Mock<IInvoiceRepository> _mockRepo;
        private readonly IMapper _mapper;
        private readonly GetAllInvoicesQueryHandler _handler;

        public GetAllInvoicesQueryHandlerTests()
        {
            _mockRepo = new Mock<IInvoiceRepository>();
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<InvoiceProfile>());
            _mapper = new Mapper(mapperConfig);

            _handler = new GetAllInvoicesQueryHandler(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ReturnsAllInvoices()
        {
            var invoices = new List<Invoice>
            {
                new Invoice { Id = "INV-001", IssueDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(30), Amount = 100, Currency = "USD", PaymentStatus = "Paid" },
                new Invoice { Id = "INV-002", IssueDate = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(15), Amount = 200, Currency = "EUR", PaymentStatus = "Pending" }
            };

            _mockRepo.Setup(repo => repo.GetAll(It.IsAny<CancellationToken>())).ReturnsAsync(invoices);

            var query = new GetAllInvoicesQuery();

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            var resultList = result.ToList();

            Assert.Equal(invoices[0].Id, resultList[0].InvoiceId);
            Assert.Equal(invoices[0].Amount, resultList[0].Amount);
            Assert.Equal(invoices[1].Currency, resultList[1].Currency);
            Assert.Equal(invoices[1].PaymentStatus, resultList[1].PaymentStatus);

            _mockRepo.Verify(repo => repo.GetAll(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
