using Application.Repositories;
using Application.Utility;
using Domain;
using Domain.Dtos.Account.Roles;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Account.Roles;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;
using System.Security.Claims;

namespace Presentation.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IMediator _mediator;

        public RoleController(IRepository<Role> roleRepository, IMediator mediator)
        {
            _roleRepository = roleRepository;
            _mediator = mediator;
        }

        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<RoleSummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            return
                await _roleRepository.GetAllAsync<RoleSummary>(
                    query,
                    filters: b => b.CompanyId == claimsIdentity.GetCompanyId(),
                    cancellationToken);
        }

        [Route("SelectList")]
        [HttpGet]
        public async Task<List<SelectSummary>> SelectList(CancellationToken cancellationToken)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            return
                await _roleRepository.GetAllAsync<SelectSummary>(b => b.CompanyId == claimsIdentity.GetCompanyId());
        }

        [Route("Find")]
        [HttpGet]
        public async Task<RoleDetails> Find([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            return await _roleRepository.FirstOrDefaultAsync<RoleDetails>(b => b.Id == id, cancellationToken);
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.General_AddRole)]
        public async Task<CommandResponse> Create([FromBody] CreateRoleCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Update")]
        [HttpPut]
        [AccessControl(PermissionsSD.General_EditRole)]
        public async Task<CommandResponse> Update([FromBody] UpdateRoleCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("Remove")]
        [HttpDelete]
        [AccessControl(PermissionsSD.General_RemoveRole)]
        public async Task<CommandResponse> Remove([FromQuery] DeleteRoleCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
