﻿using Domain.Entities.Account;

namespace Application.Repositories
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<List<String>> GetProvincesAsync(CancellationToken cancellationToken);
        Task<List<String>> GetProvinceCitiesAsync(String province, CancellationToken cancellationToken);
        Task<List<String>> GetTypesAsync(CancellationToken cancellationToken);
    }
}