﻿using ReportHub.Domain.Entities;

namespace ReportHub.Application.Contracts.RepositoryContracts
{
    public interface IClientRepository : IMongoRepositoryBase<Client>
    {
    }
}
