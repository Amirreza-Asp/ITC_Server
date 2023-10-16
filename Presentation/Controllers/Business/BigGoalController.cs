using Application.Repositories;
using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.BigGoals;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Domain.Utiltiy;
using Infrastructure.CQRS.Business.BigGoals;
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
        public async Task<BigGoalDetails> Find(Guid id, CancellationToken cancellationToken)
        {
            var data = await _repo.FirstOrDefaultAsync<BigGoalDetails>(b => b.Id == id, cancellationToken: cancellationToken);
            data.Progress = Calculator.CalcProgress(data.Indicators);

            data.Indicators.ToList().ForEach(item =>
            {
                item.Progress = Calculator.CalcProgress(item);
                item.CurrentValue = Calculator.CalcCurrentValue(item);
            });

            return data;
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.Company_AddBigGoal)]
        public async Task<CommandResponse> Create([FromBody] CreateBigGoalCommand command, CancellationToken cancellationToken) =>
            await _mediator.Send(command, cancellationToken);


        [Route("AddIndicator")]
        [HttpPost]
        public async Task<CommandResponse> AddIndicator([FromBody] AddBigGoalIndicatorCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }


        [Route("RemoveIndicator")]
        [HttpDelete]
        public async Task<CommandResponse> RemoveIndicator([FromQuery] RemoveBigGoalIndicatorCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
