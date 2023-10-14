using Application.Repositories;
using Application.Services.Interfaces;
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
        private readonly IUserAccessor _userAccessor;

        public BigGoalController(IMediator mediator, IRepository<BigGoal> repo, IUserAccessor userAccessor)
        {
            _mediator = mediator;
            _repo = repo;
            _userAccessor = userAccessor;
        }

        [Route("DropDown")]
        [HttpPost]
        public async Task<ListActionResult<BigGoalSummary>> DropDown(GridQuery query, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId();
            return await _repo.GetAllAsync<BigGoalSummary>(query, b => b.CompanyId == companyId.Value, cancellationToken);
        }

        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<BigGoalsListDto>> GetAll(GridQuery query, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId();
            return await _repo.GetAllAsync<BigGoalsListDto>(query, b => b.CompanyId == companyId.Value, cancellationToken);
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
