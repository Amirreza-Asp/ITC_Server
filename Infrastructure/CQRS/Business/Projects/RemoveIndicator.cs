using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Projects
{
    public class RemoveProjectIndicatorCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid IndicatorId { get; set; }

        [Required]
        public Guid ProjectId { get; set; }
    }

    public class RemoveProjectIndicatorCommandHandler : IRequestHandler<RemoveProjectIndicatorCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveProjectIndicatorCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveProjectIndicatorCommand request, CancellationToken cancellationToken)
        {
            var inp =
                await _context.ProjectIndicators
                    .Where(b => b.IndicatorId == request.IndicatorId && b.ProjectId == request.ProjectId)
                    .FirstOrDefaultAsync(cancellationToken);

            if (inp == null)
                return CommandResponse.Success();

            _context.ProjectIndicators.Remove(inp);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
