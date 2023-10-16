using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.PracticalActions
{
    public class RemovePracticalActionIndicatorCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid PracticalActionId { get; set; }

        [Required]
        public Guid IndicatorId { get; set; }
    }

    public class RemovePracticalActionIndicatorCommandHandler : IRequestHandler<RemovePracticalActionIndicatorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemovePracticalActionIndicatorCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemovePracticalActionIndicatorCommand request, CancellationToken cancellationToken)
        {
            var inpa =
                await _context.PracticalActionIndicators
                    .Where(b => b.IndicatorId == request.IndicatorId && b.PracticalActionId == request.PracticalActionId)
                    .FirstOrDefaultAsync(cancellationToken);

            if (inpa == null)
                return CommandResponse.Success();

            _context.PracticalActionIndicators.Remove(inpa);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
