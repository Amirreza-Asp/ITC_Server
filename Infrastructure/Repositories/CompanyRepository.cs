using Application.Repositories;
using AutoMapper;
using Domain.Entities.Account;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public CompanyRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<List<string>> GetProvinceCitiesAsync(string province, CancellationToken cancellationToken)
        {
            return
                await _context.Company
                    .Where(b => b.ProvinceName == province)
                    .Select(b => b.CityName)
                    .Distinct()
                    .ToListAsync(cancellationToken);
        }

        public async Task<List<string>> GetProvincesAsync(CancellationToken cancellationToken)
        {
            return
                await _context.Company
                    .Select(b => b.ProvinceName)
                    .Distinct()
                    .ToListAsync(cancellationToken);
        }

        public async Task<List<string>> GetTypesAsync(CancellationToken cancellationToken)
        {
            return
               await _context.Company
                   .Select(b => b.UniversityType)
                   .Distinct()
                   .ToListAsync(cancellationToken);
        }
    }
}
