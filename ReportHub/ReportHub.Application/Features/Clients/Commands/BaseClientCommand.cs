using MediatR;

namespace ReportHub.Application.Features.Clients.Commands;

public record BaseClientCommand(string Name, string Specialization) : IRequest<string>;