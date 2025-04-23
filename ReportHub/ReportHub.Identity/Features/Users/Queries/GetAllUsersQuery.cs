using MediatR;
using ReportHub.Identity.Features.Users.DTOs;

namespace ReportHub.Identity.Features.Users.Queries;

public record GetAllUsersQuery(int currentPage) : IRequest<IEnumerable<UserForGettingDto>>;
