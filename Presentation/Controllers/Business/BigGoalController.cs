using Application.Repositories;
using Domain;
using Domain.Dtos.BigGoals;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Business.BigGoals.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class BigGoalController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<BigGoal> _repo;

        public BigGoalController(IMediator mediator, IRepository<BigGoal> repo)
        {
            _mediator = mediator;
            _repo = repo;
        }

        [Route("DropDown")]
        [HttpPost]
        public async Task<ListActionResult<BigGoalSummary>> DropDown(GridQuery query, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<BigGoalSummary>(query, cancellationToken);
        }

        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<BigGoalsListDto>> GetAll(GridQuery query, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<BigGoalsListDto>(query, cancellationToken);
        }




        [HttpGet("Find/{id}")]
        public async Task<BigGoal> Find(Guid id, CancellationToken cancellationToken)
        {
            return await _repo.FirstOrDefaultAsync(b => b.Id == id, cancellationToken: cancellationToken);
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.Company_AddBigGoal)]
        public async Task<CommandResponse> Create([FromBody] CreateBigGoalCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


    }
}
