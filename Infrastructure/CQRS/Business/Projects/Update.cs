using Domain.Dtos.Shared;
using Domain.SubEntities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Projects
{
    public class UpdateProjectCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public String Title { get; set; }

        public String Contractor { get; set; }


        [Required]
        public DateTime StartedAt { get; set; }

        [Required]
        public DateTime GuaranteedFulfillmentAt { get; set; }

        public List<String> Financials { get; set; } = new List<String>();

        [Required]
        public Guid LeaderId { get; set; }
    }

    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateProjectCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var project =
                await _context.Projects
                    .Where(b => b.Id == request.Id)
                    .Include(b => b.Financials)
                    .FirstOrDefaultAsync(cancellationToken);

            if (project == null)
                return CommandResponse.Failure(400, "پروژه انتخاب شده وجود ندارد");

            project.StartedAt = request.StartedAt;
            project.GuaranteedFulfillmentAt = request.GuaranteedFulfillmentAt;
            project.LeaderId = request.LeaderId;
            project.Title = request.Title;
            project.Contractor = request.Contractor;

            project.Financials = request.Financials.Select(b => new Financial { Title = b }).ToList();

            _context.Projects.Update(project);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
