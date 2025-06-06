﻿using Microsoft.Extensions.Options;
using ReportHub.Application.Contracts.RepositoryContracts;
using ReportHub.Domain.Entities;
using ReportHub.Infrastructure.Configurations;

namespace ReportHub.Infrastructure.Repository;

public class ClientUserRepository : MongoRepositoryBase<ClientUser>, IClientUserRepository
{
    public ClientUserRepository(IOptions<MongoDbSettings> options) : base(options, "ClientUsers")
    {
    }
}
