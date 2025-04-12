using Moq;
using AutoMapper;
using ReportHub.Application.Contracts;
using ReportHub.Application.Features.Invoices.Mapping;
using ReportHub.Application.Features.Invoices.Queries;
using ReportHub.Application.Features.Invoices.Handlers.QueryHandlers;

namespace ReportHub.Tests.Application.Handlers.Invoices
{
    public class GetInvoicesByIdQueryHandlerTests
    {
        private readonly Mock<IInvoiceRepository> _mockRepo;
        private readonly IMapper _mapper;
        private readonly GetInvoicesByIdQueryHandler _handler;

        public GetInvoicesByIdQueryHandlerTests()
        {
            _mockRepo = new Mock<IInvoiceRepository>();
            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<InvoiceProfile>());
            _mapper = new Mapper(mapperConfig);

            _handler = new GetInvoicesByIdQueryHandler(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task Handle_ReturnsInvoiceById()
        {
            var invoice = new Invoice
            {
                Id = "INV-001",
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                Amount = 100,
                Currency = "USD",
                PaymentStatus = "Paid"
            };

            _mockRepo
                .Setup(repo => repo.Get(It.IsAny<Expression<Func<Invoice, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(invoice);

            var query = new GetInvoicesByIdQuery("INV-001");

            var result = await _handler.Handle(query, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(invoice.Id, result.InvoiceId);
            Assert.Equal(invoice.Amount, result.Amount);
            Assert.Equal(invoice.Currency, result.Currency);
            Assert.Equal(invoice.PaymentStatus, result.PaymentStatus);

            _mockRepo.Verify(repo => repo.Get(It.IsAny<Expression<Func<Invoice, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
