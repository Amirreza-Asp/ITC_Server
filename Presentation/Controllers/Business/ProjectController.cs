using Application.Repositories;
using Domain;
using Domain.Dtos.Projects;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Domain.Utiltiy;
using Infrastructure.CQRS.Business.Projects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

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
        public async Task<ProjectDetails> Find(Guid id, CancellationToken cancellationToken)
        {
            var data = await _repo.FirstOrDefaultAsync<ProjectDetails>(b => b.Id == id, cancellationToken: cancellationToken);
            if (data == null)
                return null;

            data.Indicators.ForEach(item =>
            {
                item.Progress = Calculator.CalcProgress(item);
                item.CurrentValue = Calculator.CalcCurrentValue(item);
            });

            return data;
        }




        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.Company_AddProject)]
        public async Task<CommandResponse> Create([FromBody] CreateProjectCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Delete")]
        [HttpDelete]
        [AccessControl(PermissionsSD.Company_RemoveProject)]
        public async Task<CommandResponse> Remove([FromQuery] DeleteProjectCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }


        [Route("AddIndicator")]
        [HttpPost]
        public async Task<CommandResponse> AddIndicator([FromBody] AddProjectIndicatorCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("RemoveIndicator")]
        [HttpDelete]
        public async Task<CommandResponse> RemoveIndicator([FromQuery] RemoveProjectIndicatorCommand command, CancellationToken cancellation)
        {
            return await _mediator.Send(command, cancellation);
        }
    }
}
