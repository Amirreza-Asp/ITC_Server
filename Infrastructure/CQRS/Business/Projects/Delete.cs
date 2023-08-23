using Domain.Dtos.Shared;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Projects
{
    public class DeleteProjectCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeleteProjectCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Projects.FindAsync(request.Id);

            if (entity == null)
                return CommandResponse.Success(200);

            _context.Projects.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "خطای سرور");
        }
    }
}
