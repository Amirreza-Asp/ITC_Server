using Domain.Dtos.Shared;
using Domain.SubEntities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.PracticalActions
{
    public class UpdatePracticalActionCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public String Title { get; set; }

        public String Contractor { get; set; }


        [Required]
        public DateTime StartedAt { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        public List<String> Financials { get; set; } = new List<String>();

        [Required]
        public Guid LeaderId { get; set; }
    }

    public class UpdatePracticalActionCommandHandler : IRequestHandler<UpdatePracticalActionCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdatePracticalActionCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdatePracticalActionCommand request, CancellationToken cancellationToken)
        {
            var action =
                await _context.PracticalActions
                    .Where(b => b.Id == request.Id)
                    .Include(b => b.Financials)
                    .FirstOrDefaultAsync(cancellationToken);

            if (action == null)
                return CommandResponse.Failure(400, "اقدام انتخاب شده وجود ندارد");

            action.StartedAt = request.StartedAt;
            action.Deadline = request.Deadline;
            action.LeaderId = request.LeaderId;
            action.Title = request.Title;
            action.Contractor = request.Contractor;

            action.Financials = request.Financials.Select(b => new Financial { Title = b }).ToList();

            _context.PracticalActions.Update(action);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
