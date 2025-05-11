using MediatR;

namespace ReportHub.Application.Features.Items.Commands;

public record DeleteItemCommand(string Id) : IRequest<string>;