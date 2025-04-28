using ReportHub.Identity.Application.Validators.Exceptions;
using ReportHub.Identity.Domain.Entities;
using ReportHub.Identity.Infrastructure.Repositories;
using System.Linq.Expressions;

namespace ReportHub.Identity.Application.Features.UserClients.Handlers.QueryHandlers;

public abstract class BaseGetAllQueryHandler
{
    protected readonly IUserClientRepository _userClientRepository;

    protected BaseGetAllQueryHandler(IUserClientRepository userClientRepository)
    {
        _userClientRepository = userClientRepository;
    }

    protected async Task<IEnumerable<UserClient>> GetAllAsync(Expression<Func<UserClient, bool>> predicate)
    {
        var result = await _userClientRepository.GetAllAsync(predicate);

        if (result is null || !result.Any())
            throw new NotFoundException();

        return result;
    }
}
