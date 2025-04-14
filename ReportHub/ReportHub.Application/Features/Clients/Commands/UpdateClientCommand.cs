using MediatR;

namespace ReportHub.Application.Features.Clients.Commands;

public record UpdateClientCommand(string Id, string Name, string Specialization) : BaseClientCommand(Name, Specialization);
