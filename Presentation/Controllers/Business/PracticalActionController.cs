using Application.Repositories;
using Domain;
using Domain.Dtos.PracticalActions;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Domain.Utiltiy;
using Infrastructure.CQRS.Business.PracticalActions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class PracticalActionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<PracticalAction> _repo;

        public PracticalActionController(IMediator mediator, IRepository<PracticalAction> repo)
        {
            _mediator = mediator;
            _repo = repo;
        }

        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<PracticalActionSummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<PracticalActionSummary>(query, cancellationToken);
        }

        [HttpGet("Find/{id}")]
        public async Task<PracticalActionDetails> Find(Guid id, CancellationToken cancellationToken)
        {
            var data = await _repo.FirstOrDefaultAsync<PracticalActionDetails>(b => b.Id == id, cancellationToken: cancellationToken);
            if (data == null)
                return null;

            data.Indicators.ForEach(item =>
            {
                item.ScheduleProgress = Calculator.CalcProgress(item);
                item.ScheduleCurrentValue = Calculator.CalcCurrentValue(item);
            });

            return data;
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.Company_AddPracticalAction)]
        public async Task<CommandResponse> Create([FromBody] CreatePracticalActionCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Delete")]
        [HttpDelete]
        [AccessControl(PermissionsSD.Company_RemovePracticalAction)]
        public async Task<CommandResponse> Remove([FromQuery] DeletePracticalActionCommand command, CancellationToken cancellation)
        {
            return await _mediator.Send(command, cancellation);
        }

        [Route("AddIndicator")]
        [HttpPost]
        [AccessControl(PermissionsSD.Company_ManagePracticalActionIndicator)]
        public async Task<CommandResponse> AddIndicator([FromBody] AddPracticalActionIndicatorCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("RemoveIndicator")]
        [HttpDelete]
        [AccessControl(PermissionsSD.Company_ManagePracticalActionIndicator)]
        public async Task<CommandResponse> RemoveIndicator([FromQuery] RemovePracticalActionIndicatorCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
