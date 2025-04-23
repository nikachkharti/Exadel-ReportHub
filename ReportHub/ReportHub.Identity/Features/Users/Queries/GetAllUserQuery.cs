using MediatR;
using ReportHub.Identity.Features.Users.DTOs;

namespace ReportHub.Identity.Features.Users.Queries;

public record GetAllUserQuery : IRequest<IEnumerable<UserForGettingDto>>;
