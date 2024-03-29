﻿using Application.Repositories;
using Application.Services.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Domain.Dtos.Companies;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using Domain.Queries.Shared;
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

            var nestedItems = await GetNestedCompanies(companyId.Value, cancellation);
            return nestedItems;
        }

        async Task<NestedCompanies> GetNestedCompanies(Guid id, CancellationToken cancellation)
        {
            var company =
                await _context.Company
                 .Where(b => b.Id == id)
                 .Include(b => b.Childs)
                 .FirstOrDefaultAsync();

            if (company == null)
            {
                return null;
            }

            var companySpec = await _context.Company
                    .Where(b => b.Id == company.Id)
                    .Select(b => new
                    CompanySpec
                    {
                        CompanyId = b.Id,
                        UsersCount = b.Acts.Count(),
                        IndicatorsCount = b.Indicators.Count(),
                        Agent = b.Acts.Where(b => b.RoleId == SD.AgentId).Select(b => String.Concat(b.User.Name + " " + b.User.Family)).FirstOrDefault()
                    })
                    .FirstOrDefaultAsync();


            var nestedItems =
                new NestedCompanies()
                {
                    Title = company.Title,
                    Id = company.Id,
                    IndicatorCount = companySpec.IndicatorsCount,
                    UsersCount = companySpec.UsersCount,
                    Agent = companySpec.Agent
                };

            var companyChilds = company.Childs;

            foreach (var subCompany in companyChilds)
            {
                var item = await GetNestedCompanies(subCompany.Id, cancellation);
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

        public async Task<List<CompanySummary>> GetChildsCompaniesPagenationAsync(Guid id, GridQuery query, CancellationToken cancellationToken)
        {
            var data =
                _context.Company.FromSqlRaw(
                     "WITH RecursiveCTE AS " +
                     "(SELECT *   " +
                     " FROM Company " +
                    " WHERE Id = '{0}'" +
                     " UNION ALL" +
                     " SELECT c.* " +
                     " FROM Company c " +
                     " INNER JOIN  RecursiveCTE r ON c.ParentId = r.Id)\n" +
                     " SELECT * FROM RecursiveCTE ORDER BY Id " +
                    $" OFFSET {(query.Page - 1) * query.Size} ROWS " +
                    $" FETCH NEXT {query.Size} ROWS ONLY;", id.ToString())
                .AsEnumerable()
                .ToList();


            return _mapper.Map<List<CompanySummary>>(data);
        }
    }

    public class CompanySpec
    {
        public Guid CompanyId { get; set; }
        public int UsersCount { get; set; }
        public int IndicatorsCount { get; set; }
        public String Agent { get; set; }
    }

    public class ChildCountDTO
    {
        public int TotalChildCount { get; set; }
    }
}
