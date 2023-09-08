using Application.Repositories;
using Domain.Dtos.Account.Roles;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Account.Roles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class RoleController : ControllerBase
    {
        private readonly IRepository<Role> _roleRepo;
        private readonly IMediator _mediator;

        public RoleController(IRepository<Role> roleRepo, IMediator mediator)
        {
            _roleRepo = roleRepo;
            _mediator = mediator;
        }

        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<RoleSummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            return await _roleRepo.GetAllAsync<RoleSummary>(query, cancellationToken);
        }

        [Route("Find")]
        [HttpGet]
        public async Task<RoleDetails> Find([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            return await _roleRepo.FirstOrDefaultAsync<RoleDetails>(b => b.Id == id, cancellationToken);
        }

        [Route("Create")]
        [HttpPost]
        public async Task<CommandResponse> Create([FromBody] CreateRoleCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

    }
}
