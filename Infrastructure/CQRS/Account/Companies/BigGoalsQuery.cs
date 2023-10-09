using AutoMapper;
using Domain.Dtos.BigGoals;
using Domain.Dtos.Companies;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Companies
{
    public class CompanyBigGoalsQuery : IRequest<List<CompanyBigGoals>>
    {
        public List<Guid> Companies { get; set; }
        public String Year { get; set; }
        public int ProgressFrom { get; set; } = 0;
        public int ProgressTo { get; set; } = 100;
    }

    public class CompanyBigGoalsQueryHandler : IRequestHandler<CompanyBigGoalsQuery, List<CompanyBigGoals>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CompanyBigGoalsQueryHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CompanyBigGoals>> Handle(CompanyBigGoalsQuery request, CancellationToken cancellationToken)
        {
            var data =
                await _context.Company
                    .Where(b =>
                            request.Companies.Contains(b.Id) &&
                            (request.Year == null || b.BigGoals.Any(s => s.ProgramYear.Year.Contains(request.Year))) &&
                            b.BigGoals.Any(s => s.Progress >= request.ProgressFrom) &&
                            b.BigGoals.Any(s => s.Progress <= request.ProgressTo))
                    .Select(company => new CompanyBigGoals
                    {
                        CompanyId = company.Id,
                        CompanyName = company.NameUniversity,
                        BigGoals =
                            company.BigGoals
                              .Where(b =>
                                (request.Year == null || b.ProgramYear.Year.Contains(request.Year)) &&
                                b.Progress >= request.ProgressFrom &&
                                b.Progress <= request.ProgressTo)
                              .Select(b => new BigGoalsListDto
                              {
                                  Id = b.Id,
                                  Progress = b.Progress,
                                  Title = b.Title,
                                  Year = b.ProgramYear.Year
                              })
                              .ToList()
                    })
                    .ToListAsync(cancellationToken);

            return data;
        }
    }
}
