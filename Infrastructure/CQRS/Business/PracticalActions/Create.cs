using AutoMapper;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.PracticalActions
{
    public class CreatePracticalActionCommand : IRequest<CommandResponse>
    {
        [Required]
        public String Title { get; set; }

        public String Contractor { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        public List<String> Financials { get; set; } = new List<String>();

        [Required]
        public Guid LeaderId { get; set; }

        [Required]
        public Guid OperationalObjectiveId { get; set; }
    }

    public class CreatePracticalActionCommandHandler : IRequestHandler<CreatePracticalActionCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreatePracticalActionCommandHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CommandResponse> Handle(CreatePracticalActionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.StartedAt > request.Deadline)
                    return CommandResponse.Failure(400, "زمان شروع نمیتواند از زمان پایان بیشتر باشد");

                if (!_context.People.Any(b => b.Id == request.LeaderId))
                    return CommandResponse.Failure(400, "راهبر انتخاب شده نامعتبر است");

                if (!_context.OperationalObjectives.Any(b => b.Id == request.OperationalObjectiveId))
                    return CommandResponse.Failure(400, "اقدام عملیاتی انتخاب شده نامعتبر است");

                var practicalAction = _mapper.Map<PracticalAction>(request);

                _context.PracticalActions.Add(practicalAction);

                if (await _context.SaveChangesAsync(cancellationToken) > 0)
                    return CommandResponse.Success(practicalAction.Id);

                return CommandResponse.Failure(400);
            }
            catch (Exception ex)
            {
                return CommandResponse.Failure(500, ex.Message);
            }

        }
    }
}
