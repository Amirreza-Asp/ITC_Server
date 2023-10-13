using Domain.Dtos.Companies;
using Domain.Dtos.Projects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Companies
{
    public class CompanyProjectsQuery : IRequest<List<CompanyProjects>>
    {
        public List<Guid> Companies { get; set; }
        public String BigGoal { get; set; }
    }

    public class CompanyProjectsQueryHandler : IRequestHandler<CompanyProjectsQuery, List<CompanyProjects>>
    {
        private readonly ApplicationDbContext _context;

        public CompanyProjectsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompanyProjects>> Handle(CompanyProjectsQuery request, CancellationToken cancellationToken)
        {
            var data =
               await _context.Projects
                   .Include(b => b.Indicators)
                        .ThenInclude(b => b.Indicator)
                   .Where(b =>
                           request.Companies.Contains(b.OperationalObjective.BigGoal.CompanyId) &&
                           (String.IsNullOrWhiteSpace(request.BigGoal) || b.OperationalObjective.BigGoal.Title.Contains(request.BigGoal.Trim())))
                   .Select(op => new CompanyProjects
                   {
                       CompanyId = op.OperationalObjective.BigGoal.CompanyId,
                       CompanyName = op.OperationalObjective.BigGoal.Company.NameUniversity,
                       Projects = new List<ProjectListDto>
                       {
                            new ProjectListDto
                            {
                                Id = op.Id,
                                BigGoal = op.OperationalObjective.BigGoal.Title,
                                Progress = op.Progress,
                                Title = op.Title
                            }
                       }
                   })
                   .ToListAsync(cancellationToken);

            var coo = new List<CompanyProjects>();
            data.ForEach(item =>
            {
                if (coo.Any(b => b.CompanyId == item.CompanyId))
                    coo.Find(b => b.CompanyId == item.CompanyId).Projects.Add(item.Projects.First());
                else
                    coo.Add(item);
            });

            return coo;
        }
    }
}
