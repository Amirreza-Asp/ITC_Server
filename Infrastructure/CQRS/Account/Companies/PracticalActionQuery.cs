using Domain.Dtos.Companies;
using Domain.Dtos.PracticalActions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Companies
{
    public class CompanyPracticalActionQuery : IRequest<List<CompanyPracticalActions>>
    {
        public List<Guid> Companies { get; set; }
        public String BigGoal { get; set; }
    }

    public class CompanyPracticalActionQueryHandler : IRequestHandler<CompanyPracticalActionQuery, List<CompanyPracticalActions>>
    {
        private readonly ApplicationDbContext _context;

        public CompanyPracticalActionQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompanyPracticalActions>> Handle(CompanyPracticalActionQuery request, CancellationToken cancellationToken)
        {
            var data =
               await _context.PracticalActions
                   .Include(b => b.Indicators)
                      .ThenInclude(b => b.Indicator)
                   .Where(b =>
                           request.Companies.Contains(b.OperationalObjective.BigGoal.CompanyId) &&
                           (String.IsNullOrWhiteSpace(request.BigGoal) || b.OperationalObjective.BigGoal.Title.Contains(request.BigGoal.Trim())))
                   .Select(op => new CompanyPracticalActions
                   {
                       CompanyId = op.OperationalObjective.BigGoal.CompanyId,
                       CompanyName = op.OperationalObjective.BigGoal.Company.Title,
                       PracticalActions = new List<PracticalActionListDto>
                       {
                            new PracticalActionListDto
                            {
                                Id = op.Id,
                                BigGoal = op.OperationalObjective.BigGoal.Title,
                                Progress = op.Progress,
                                Title = op.Title
                            }
                       }
                   })
                   .ToListAsync(cancellationToken);

            var coo = new List<CompanyPracticalActions>();
            data.ForEach(item =>
            {
                if (coo.Any(b => b.CompanyId == item.CompanyId))
                    coo.Find(b => b.CompanyId == item.CompanyId).PracticalActions.Add(item.PracticalActions.First());
                else
                    coo.Add(item);
            });

            return coo;
        }
    }
}
