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
        private readonly IRepository<Project> _projectRepo;
        private readonly IRepository<ProjectIndicator> _projectIndicatorRepo;

        public ProjectController(IMediator mediator, IRepository<Project> repo, IRepository<ProjectIndicator> projectIndicatorRepo)
        {
            _mediator = mediator;
            _projectRepo = repo;
            _projectIndicatorRepo = projectIndicatorRepo;
        }


        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<ProjectSummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            return await _projectRepo.GetAllAsync<ProjectSummary>(query, cancellationToken);
        }

        [HttpGet("Find/{id}")]
        public async Task<ProjectDetails> Find(Guid id, CancellationToken cancellationToken)
        {
            var data = await _projectRepo.FirstOrDefaultAsync<ProjectDetails>(b => b.Id == id, cancellationToken: cancellationToken);
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
        [AccessControl(PermissionsSD.Company_AddProject)]
        public async Task<CommandResponse> Create([FromBody] CreateProjectCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Update")]
        [HttpPut]
        [AccessControl(PermissionsSD.Company_EditProject)]
        public async Task<CommandResponse> Update([FromBody] UpdateProjectCommand command, CancellationToken cancellationToken)
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
        [AccessControl(PermissionsSD.Company_ManageProjectIndicator)]
        public async Task<CommandResponse> AddIndicator([FromBody] AddProjectIndicatorCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }


        [Route("GetIndicators")]
        [HttpGet]
        public async Task<List<IndicatorDetails>> GetIndicators([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            return await _projectIndicatorRepo.GetAllAsync<IndicatorDetails>(b => b.ProjectId == id, cancellationToken: cancellationToken);
        }

        [Route("RemoveIndicator")]
        [HttpDelete]
        [AccessControl(PermissionsSD.Company_ManageProjectIndicator)]
        public async Task<CommandResponse> RemoveIndicator([FromQuery] RemoveProjectIndicatorCommand command, CancellationToken cancellation)
        {
            return await _mediator.Send(command, cancellation);
        }
    }
}
