using MediatR;
using ReportHub.Identity.Application.Features.Users.DTOs;

namespace ReportHub.Identity.Application.Features.Users.Queries;

public record GetAllUsersQuery(int currentPage = 1) : IRequest<IEnumerable<UserForGettingDto>>;
