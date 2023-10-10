using Application.Repositories;
using Application.Utility;
using Domain;
using Domain.Dtos.People;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Business.People;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;
using System.Security.Claims;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<Person> _repo;

        public PersonController(IMediator mediator, IRepository<Person> repo)
        {
            _mediator = mediator;
            _repo = repo;
        }


        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<PersonSummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            var companyId = (User.Identity as ClaimsIdentity).GetCompanyId();
            return await _repo.GetAllAsync<PersonSummary>(query, b => b.CompanyId == companyId.Value, cancellationToken);
        }

        [HttpGet("Find/{id}")]
        public async Task<PersonSummary> Find(Guid id, CancellationToken cancellationToken)
        {
            return await _repo.FirstOrDefaultAsync<PersonSummary>(b => b.Id == id, cancellationToken: cancellationToken);
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.Company_AddPerson)]
        public async Task<CommandResponse> Create([FromBody] CreatePersonCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
