using Application.Repositories;
using Domain.Dtos.PracticalActions;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Business.PracticalActions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<PracticalActionSummary> Find(Guid id, CancellationToken cancellationToken)
        {
            return await _repo.FirstOrDefaultAsync<PracticalActionSummary>(b => b.Id == id, cancellationToken: cancellationToken);
        }

        [Route("Create")]
        [HttpPost]
        public async Task<CommandResponse> Create([FromBody] CreatePracticalActionCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Delete")]
        [HttpDelete]
        public async Task<CommandResponse> Remove([FromQuery] DeletePracticalActionCommand command, CancellationToken cancellation)
        {
            return await _mediator.Send(command, cancellation);
        }
    }
}
