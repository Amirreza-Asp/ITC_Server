using Application.Repositories;
using Application.Utility;
using Domain;
using Domain.Dtos.Account.Users;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Account.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;
using System.Security.Claims;

namespace Presentation.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMediator _mediator;
        private readonly IRepository<UserJoinRequest> _userJoinRequestsRepo;

        public UserController(IRepository<User> userRepository, IMediator mediator, IRepository<UserJoinRequest> userJoinRequestsRepo)
        {
            _userRepository = userRepository;
            _mediator = mediator;
            _userJoinRequestsRepo = userJoinRequestsRepo;
        }

        [HttpPost]
        [Route("GetAll")]
        [AccessControl(PermissionsSD.General_UsersList)]
        public async Task<ListActionResult<UserSummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            return
                await _userRepository
                    .GetAllAsync<UserSummary>(query, b => b.CompanyId == claimsIdentity.GetCompanyId() && b.IsActive, cancellationToken);
        }


        [Route("Delete")]
        [HttpDelete]
        [AccessControl(PermissionsSD.General_RemoveUser)]
        public async Task<CommandResponse> Remove([FromQuery] DeleteUserCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("ManageRole")]
        [HttpPut]
        [AccessControl(PermissionsSD.General_ManageUserRole)]
        public async Task<CommandResponse> ManageUserRole([FromBody] ManageUserRoleCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [Route("JoinRequests")]
        [HttpPost]
        [AccessControl(PermissionsSD.General_UsersRequests)]
        public async Task<ListActionResult<UserRequestsSummary>> GetJoinRequests([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            var companyId = (User.Identity as ClaimsIdentity).GetCompanyId();
            return await _userJoinRequestsRepo.GetAllAsync<UserRequestsSummary>(query, b => b.CompanyId == companyId, cancellationToken);
        }

        [HttpPost]
        [Route("RequestResult")]
        [AccessControl(PermissionsSD.General_UsersRequests)]
        public async Task<CommandResponse> RequestResult([FromBody] UserRequestResultCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
