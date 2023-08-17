using Application.Repositories;
using Domain.Dtos.Shared;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Business.Systems;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Domain.Entities.Business.System> _repository;

        public SystemController(IMediator mediator, IRepository<Domain.Entities.Business.System> repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<Domain.Entities.Business.System>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            return await _repository.GetAllAsync<Domain.Entities.Business.System>(query, cancellationToken);
        }

        [HttpGet("Find/{id}")]
        public async Task<Domain.Entities.Business.System> Find(Guid id, CancellationToken cancellationToken)
        {
            return await _repository.FirstOrDefaultAsync<Domain.Entities.Business.System>(b => b.Id == id, cancellationToken: cancellationToken);
        }

        [Route("Create")]
        [HttpPost]
        public async Task<CommandResponse> Create([FromBody] CreateSystemCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
