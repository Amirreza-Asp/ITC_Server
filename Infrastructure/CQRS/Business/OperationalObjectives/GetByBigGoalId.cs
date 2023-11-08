using AutoMapper;
using Domain.Dtos.OperationalObjectives;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.OperationalObjectives
{
    public class GetOperationalObjectivesByBigGoalIdQuery : IRequest<List<OperationalObjectiveCard>>
    {
        [Required]
        public Guid BigGoalId { get; set; }
    }

    public class GetOperationalObjectivesByBigGoalIdQueryHandler : IRequestHandler<GetOperationalObjectivesByBigGoalIdQuery, List<OperationalObjectiveCard>>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetOperationalObjectivesByBigGoalIdQueryHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<OperationalObjectiveCard>> Handle(GetOperationalObjectivesByBigGoalIdQuery request, CancellationToken cancellationToken)
        {
            var data =
                await _context.OperationalObjectives
                    .Where(b => b.BigGoalId == request.BigGoalId)
                    .Include(b => b.Indicators)
                        .ThenInclude(b => b.Indicator)
                            .ThenInclude(b => b.Progresses)
                    .Include(b => b.Projects)
                        .ThenInclude(b => b.Indicators)
                            .ThenInclude(b => b.Indicator)
                                .ThenInclude(b => b.Progresses)
                    .Include(b => b.PracticalActions)
                        .ThenInclude(b => b.Indicators)
                            .ThenInclude(b => b.Indicator)
                                .ThenInclude(b => b.Progresses)
                    .ToListAsync(cancellationToken);


            var opCards =
                _mapper.Map<List<OperationalObjectiveCard>>(data);

            for (int i = 0; i < opCards.Count; i++)
            {
                opCards[i].Progress = data[i].Progress;

                var sumRealProgress = data[i].Indicators.Sum(
                                e => (!e.Indicator.Progresses.Any() ? 0 : e.Indicator.Progresses.OrderByDescending(b => b.ProgressTime).First().Value - e.Indicator.InitValue) * 100 / (e.Indicator.GoalValue - e.Indicator.InitValue));
                opCards[i].RealProgress = data[i].Indicators.Any() ? (int)sumRealProgress / data[i].Indicators.Count() : 0;

                // projects
                for (int j = 0; j < opCards[i].Projects.Count; j++)
                {
                    opCards[i].Projects[j].Progress = data[i].Projects.ElementAt(j).Progress;

                    var sumProjectsRealProgress = data[i].Projects.ElementAt(j).Indicators.Sum(
                                    e => (!e.Indicator.Progresses.Any() ? 0 : e.Indicator.Progresses.OrderByDescending(b => b.ProgressTime).First().Value - e.Indicator.InitValue) * 100 / (e.Indicator.GoalValue - e.Indicator.InitValue));
                    opCards[i].Projects.ElementAt(j).RealProgress = data[i].Projects.ElementAt(j).Indicators.Any() ? (int)sumProjectsRealProgress / data[i].Projects.ElementAt(j).Indicators.Count() : 0;

                }

                // actions
                for (int j = 0; j < opCards[i].Actions.Count; j++)
                {
                    opCards[i].Actions[j].Progress = data[i].PracticalActions.ElementAt(j).Progress;

                    var sumActionsRealProgress = data[i].PracticalActions.ElementAt(j).Indicators.Sum(
                                  e => (!e.Indicator.Progresses.Any() ? 0 : e.Indicator.Progresses.OrderByDescending(b => b.ProgressTime).First().Value - e.Indicator.InitValue) * 100 / (e.Indicator.GoalValue - e.Indicator.InitValue));
                    opCards[i].Actions.ElementAt(j).RealProgress = data[i].PracticalActions.ElementAt(j).Indicators.Any() ? (int)sumActionsRealProgress / data[i].PracticalActions.ElementAt(j).Indicators.Count() : 0;
                }
            };



            return opCards;
        }



    }
}
