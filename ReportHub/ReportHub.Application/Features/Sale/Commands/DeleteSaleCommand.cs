using MediatR;
using System.Security;

namespace ReportHub.Application.Features.Sale.Commands
{
    public record DeleteSaleCommand(int Id) : IRequest<string>;
}
