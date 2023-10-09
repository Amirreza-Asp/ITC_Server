using Application.Utility;
using Domain.Dtos.Shared;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastructure.CQRS.Account.Users
{
    public class ManageUserRoleCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
    }


    public class ManageUserRoleCommandHandler : IRequestHandler<ManageUserRoleCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ManageUserRoleCommandHandler(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResponse> Handle(ManageUserRoleCommand request, CancellationToken cancellationToken)
        {
            var user =
                await _context.Users
                    .Include(b => b.Token)
                    .FirstOrDefaultAsync(b => b.IsActive && request.Id == b.Id);

            if (user == null)
                return CommandResponse.Failure(400, "کاربر انتخاب شده در سیستم وجود ندارد");

            var companyId = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).GetCompanyId();
            if (!_context.Roles.Any(b => b.Id == request.RoleId && b.CompanyId == companyId))
                return CommandResponse.Failure(400, "نقش انتخاب شده در سیستم وجود ندارد");

            user.RoleId = request.RoleId;
            _context.Users.Update(user);

            if (user.Token != null)
            {
                user.Token.IsActive = false;
                _context.Tokens.Update(user.Token);
            }


            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
