using Application.Repositories;
using Domain.Dtos.Projects;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Business.Projects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Project> _repo;

        public ProjectController(IMediator mediator, IRepository<Project> repo)
        {
            _mediator = mediator;
            _repo = repo;
        }


        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<ProjectSummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<ProjectSummary>(query, cancellationToken);
        }

        [HttpGet("Find/{id}")]
        public async Task<ProjectSummary> Find(Guid id, CancellationToken cancellationToken)
        {
            return await _repo.FirstOrDefaultAsync<ProjectSummary>(b => b.Id == id, cancellationToken: cancellationToken);
        }

        [Route("Create")]
        [HttpPost]
        public async Task<CommandResponse> Create([FromBody] CreateProjectCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
