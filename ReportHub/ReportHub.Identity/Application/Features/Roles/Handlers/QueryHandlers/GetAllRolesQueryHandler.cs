using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver.Linq;
using ReportHub.Identity.Application.Features.Roles.DTOs;
using ReportHub.Identity.Application.Features.Roles.Queries;
using ReportHub.Identity.Application.Validators.Exceptions;
using ReportHub.Identity.Domain.Entities;
using System.Collections.Immutable;

namespace ReportHub.Identity.Application.Features.Roles.Handlers.QueryHandlers;

public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, IEnumerable<RoleForGettingDto>>
{
    private readonly RoleManager<Role> _roleManager;
    private readonly IMapper _mapper;
    public GetAllRolesQueryHandler(RoleManager<Role> roleManager, IMapper mapper)
    {
        _roleManager = roleManager;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RoleForGettingDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleManager.Roles.ToListAsync();

        if(roles is null || !roles.Any())
        {
            throw new NoContentException();
        }

        return _mapper.Map<IEnumerable<RoleForGettingDto>>(roles);
    }
}
