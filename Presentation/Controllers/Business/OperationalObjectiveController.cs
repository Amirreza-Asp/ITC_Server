using Application.Repositories;
using AutoMapper;
using Domain;
using Domain.Dtos.OperationalObjectives;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Business.OperationalObjectives;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

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

        [HttpGet("GetAllByBigGoalId/{bigGoalId}")]
        public async Task<List<OperationalObjectiveDetails>> GetAll(Guid bigGoalId, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<OperationalObjectiveDetails>(
                 b => b.BigGoalId == bigGoalId,
                 cancellationToken: cancellationToken);
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
    }
}
