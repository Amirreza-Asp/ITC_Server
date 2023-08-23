using Domain.Dtos.Shared;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.PracticalActions
{
    public class DeletePracticalActionCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class DeletePracticalActionCommandHandler : IRequestHandler<DeletePracticalActionCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeletePracticalActionCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(DeletePracticalActionCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.PracticalActions.FindAsync(request.Id);

            if (entity == null)
                return CommandResponse.Success(200);

            _context.PracticalActions.Remove(entity);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "خطای سرور");
        }
    }
}
