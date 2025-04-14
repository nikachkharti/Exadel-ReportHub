using MediatR;

namespace ReportHub.Application.Features.Clients.Commands;

public record CreateClientCommand(string Name, string Specialization) : BaseClientCommand(Name, Specialization);
