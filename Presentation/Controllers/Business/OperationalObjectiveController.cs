using Application.Repositories;
using AutoMapper;
using Domain.Dtos.OperationalObjectives;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Business.OperationalObjectives;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationalObjectiveController : ControllerBase
    {
        private readonly IRepository<OperationalObjective> _repo;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public OperationalObjectiveController(IRepository<OperationalObjective> repo, IMediator mediator, IMapper mapper)
        {
            _repo = repo;
            _mediator = mediator;
            _mapper = mapper;
        }


        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<OperationalObjectiveSummary>> GetAll(GridQuery query, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<OperationalObjectiveSummary>(query, cancellationToken);
        }

        [HttpGet("Find/{id}")]
        public async Task<OperationalObjectiveDetails> Find(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _repo.FirstOrDefaultAsync(
                filter: b => b.Id == id,
                source => source.
                    Include(b => b.Projects)
                        .ThenInclude(d => d.Financials)
                    .Include(b => b.PracticalActions)
                        .ThenInclude(d => d.Financials),
                cancellationToken: cancellationToken);

            var oboCard = _mapper.Map<OperationalObjectiveDetails>(entity);
            oboCard.ProjectActions = _mapper.Map<List<ProjectActionCard>>(entity.Projects);
            oboCard.ProjectActions.AddRange(_mapper.Map<List<ProjectActionCard>>(entity.PracticalActions));

            return oboCard;
        }


        [Route("Create")]
        [HttpPost]
        public async Task<CommandResponse> Create([FromBody] CreateOperationalObjectiveCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
