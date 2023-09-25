using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Account.Roles
{
    public class DeleteRoleCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeleteRoleCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role =
                await _context.Roles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == request.Id);

            if (role == null)
                return CommandResponse.Success();

            if (_context.Users.Any(b => b.RoleId == request.Id))
                return CommandResponse.Failure(400, "این نقش در سیستم برای برخی کاربران استفاده شده و نمیتوان ان را حذف کرد");

            _context.Roles.Remove(role);

            if (await _context.SaveChangesAsync() > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
