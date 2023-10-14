using Application.Repositories;
using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.Account.Users;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Account.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMediator _mediator;
        private readonly IRepository<UserJoinRequest> _userJoinRequestsRepo;
        private readonly IUserAccessor _userAccessor;

        public UserController(IRepository<User> userRepository, IMediator mediator, IRepository<UserJoinRequest> userJoinRequestsRepo, IUserAccessor userAccessor)
        {
            _userRepository = userRepository;
            _mediator = mediator;
            _userJoinRequestsRepo = userJoinRequestsRepo;
            _userAccessor = userAccessor;
        }

        [HttpPost]
        [Route("GetAll")]
        [AccessControl(PermissionsSD.General_UsersList)]
        public async Task<ListActionResult<UserSummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            return
                await _userRepository
                    .GetAllAsync<UserSummary>(query, b => b.CompanyId == _userAccessor.GetCompanyId() && b.IsActive, cancellationToken);
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
            var companyId = _userAccessor.GetCompanyId();
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
