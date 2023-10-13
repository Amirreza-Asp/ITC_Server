using Application.Repositories;
using AutoMapper;
using Domain;
using Domain.Dtos.OperationalObjectives;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Domain.Utiltiy;
using Infrastructure.CQRS.Business.OperationalObjectives;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationalObjectiveController : ControllerBase
    {
        private readonly IRepository<OperationalObjective> _repo;
        private readonly IRepository<OperationalObjectiveIndicator> _increpo;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public OperationalObjectiveController(IRepository<OperationalObjective> repo, IMediator mediator, IMapper mapper, IRepository<OperationalObjectiveIndicator> increpo)
        {
            _repo = repo;
            _mediator = mediator;
            _mapper = mapper;
            _increpo = increpo;
        }


        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<OperationalObjectiveSummary>> GetAll(GridQuery query, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<OperationalObjectiveSummary>(query, cancellationToken);
        }

        [HttpGet("GetAllByBigGoalId/{bigGoalId}")]
        public async Task<List<OperationalObjectiveDetails>> GetAll(Guid bigGoalId, CancellationToken cancellationToken)
        {
            var data = await _repo.GetAllAsync<OperationalObjectiveDetails>(
                 b => b.BigGoalId == bigGoalId,
                 include: source => source
                    .Include(e => e.Projects)
                        .ThenInclude(e => e.Indicators)
                            .ThenInclude(b => b.Indicator)
                    .Include(e => e.PracticalActions)
                        .ThenInclude(e => e.Indicators)
                            .ThenInclude(b => b.Indicator),
                 cancellationToken: cancellationToken);

            var incs = await _increpo.GetAllAsync<OperationalObjectiveIndicator>(
                b => data.Select(b => b.Id).Contains(b.OperationalObjectiveId), include: source => source.Include(b => b.OperationalObjective));

            data.ForEach(item =>
            {
                item.Progress = Calculator.CalcProgress(incs.Where(b => b.OperationalObjectiveId == item.Id).Select(b => b.Indicator));
            });

            return data;
        }

        [Route("Find")]
        [HttpGet]
        public async Task<OperationalObjectiveDetails> Find([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            return
                await _repo.FirstOrDefaultAsync<OperationalObjectiveDetails>(b => b.Id == id);
        }


        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.Company_AddOperationalObjective)]
        public async Task<CommandResponse> Create([FromBody] CreateOperationalObjectiveCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [HttpDelete]
        [Route("Delete")]
        [AccessControl(PermissionsSD.Company_RemoveOperationalObjective)]
        public async Task<CommandResponse> Remove([FromQuery] DeleteOperationObjectiveCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [HttpPost]
        [Route("AddIndicator")]
        public async Task<CommandResponse> AddIndicator([FromBody] AddOperationalObjectiveIndicatorCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
