using Application.Repositories;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Domain.Dtos.Companies;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly IUserAccessor _userAccessor;

        public CompanyRepository(ApplicationDbContext context, IMapper mapper, IUserAccessor userAccessor) : base(context, mapper)
        {
            _userAccessor = userAccessor;
        }

        public async Task<NestedCompanies> GetNestedAsync(CancellationToken cancellation)
        {
            var companyId = _userAccessor.GetCompanyId();
            var nestedItems = GetNestedCompanies(companyId.Value, cancellation);
            return nestedItems;
        }

        NestedCompanies GetNestedCompanies(Guid id, CancellationToken cancellation)
        {
            var company =
                 _context.Company
                    .Include(b => b.Childs)
                    .Include(b => b.Acts)
                    .Include(b => b.Indicators)
                    .Where(b => b.Id == id)
                    .FirstOrDefault();

            if (company == null)
                return null;

            var agentId = company.Acts.FirstOrDefault(b => b.RoleId == SD.AgentId)?.UserId;
            String agentName = "";
            if (agentId != default)
            {
                var agent = _context.Users.FirstOrDefault(b => b.Id == agentId);
                agentName = agent == null ? "" : agent.Name + " " + agent.Family;
            }

            var nestedItems =
                new NestedCompanies()
                {
                    Title = company.Title,
                    Id = company.Id,
                    IndicatorCount = company.Indicators.Count,
                    UsersCount = company.Acts.Count,
                    Agent = agentName
                };

            foreach (var subcategory in company.Childs)
            {
                var item = GetNestedCompanies(subcategory.Id, cancellation);

                nestedItems.Childs.Add(item);
            }

            return nestedItems;
        }

        public async Task<List<string>> GetProvinceCitiesAsync(string province, CancellationToken cancellationToken)
        {
            return
                await _context.Company
                    .Where(b => b.Province == province && b.City != null)
                    .Select(b => b.City)
                    .Distinct()
                    .ToListAsync(cancellationToken);
        }

        public async Task<List<string>> GetProvincesAsync(CancellationToken cancellationToken)
        {
            return
                await _context.Company
                    .Where(b => b.Province != null)
                    .Select(b => b.Province)
                    .Distinct()
                    .ToListAsync(cancellationToken);
        }

        public async Task<List<string>> GetTypesAsync(CancellationToken cancellationToken)
        {
            return
               await _context.Company
                   .Where(b => b.UniversityType != null)
                   .Select(b => b.UniversityType)
                   .Distinct()
                   .ToListAsync(cancellationToken);
        }

        public async Task<List<SelectSummary>> GetSelectListUsersAsync(CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId();

            return
                await _context.Users
                    .Where(b => b.Act.Any(b => b.CompanyId == companyId.Value))
                    .ProjectTo<SelectSummary>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
        }
    }
}
