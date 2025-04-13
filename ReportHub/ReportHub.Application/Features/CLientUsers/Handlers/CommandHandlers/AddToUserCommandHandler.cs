using MediatR;
using Microsoft.AspNetCore.Http;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Application.Features.CLientUsers.Commands;
using ReportHub.Domain.Entities;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ReportHub.Application.Features.CLientUsers.Handlers.CommandHandlers
{
    public class AddToUserCommandHandler : IRequestHandler<AddUserToClientCommand, bool>
    {
        private readonly IClientUserRepository _clientUserRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddToUserCommandHandler(IClientUserRepository clientUserRepository, IHttpContextAccessor httpContextAccessor)
        {
            _clientUserRepository = clientUserRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(AddUserToClientCommand request, CancellationToken cancellationToken)
        {
            var clientUser = new ClientUser
            {
                ClientId = request.ClientId,
                UserId = request.UserId,
                Role = request.Role
            };
            var token = GetBearerTokenFromRequest();
            if (string.IsNullOrEmpty(token))
                return false;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var requestBody = new
            {
                userId = request.UserId,
                roleName = request.Role
            };
            var result = await client.PostAsJsonAsync("https://localhost:7171/api/Admin/assign-role", requestBody, cancellationToken);

            if (!result.IsSuccessStatusCode)
            {
                return false;
            }

            await _clientUserRepository.Insert(clientUser, cancellationToken);

            return true;
        }

        private string GetBearerTokenFromRequest()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext?
                .Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authorizationHeader) ||
                !authorizationHeader.StartsWith("Bearer "))
                return null;

            return authorizationHeader.Substring("Bearer ".Length).Trim();
        }
    }
}
