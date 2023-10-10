using Application.Repositories;
using Application.Utility;
using Domain;
using Domain.Dtos.Refrences;
using Domain.Dtos.Shared;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Business.Systems;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;
using System.Security.Claims;

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
        public async Task<ListActionResult<SystemDetails>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            var companyId = (User.Identity as ClaimsIdentity).GetCompanyId();
            return await _repository.GetAllAsync<SystemDetails>(query, b => b.CompanyId == companyId.Value, cancellationToken);
        }

        [HttpGet("Find/{id}")]
        public async Task<SystemDetails> Find(Guid id, CancellationToken cancellationToken)
        {
            return await _repository.FirstOrDefaultAsync<SystemDetails>(b => b.Id == id, cancellationToken: cancellationToken);
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.Company_AddSystem)]
        public async Task<CommandResponse> Create([FromBody] CreateSystemCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
