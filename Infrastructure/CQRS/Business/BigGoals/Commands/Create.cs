using Application.Repositories;
using AutoMapper;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.BigGoals.Commands
{
    public class CreateBigGoalCommand : IRequest<CommandResponse>
    {
        [Required]
        public String Title { get; set; }

        public String Description { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        [Required]
        public DateTime Deadline { get; set; }
    }

    public class CreateBigGoalCommandHandler : IRequestHandler<CreateBigGoalCommand, CommandResponse>
    {
        private readonly IRepository<BigGoal> _repo;
        private readonly IMapper _mapper;

        public CreateBigGoalCommandHandler(IRepository<BigGoal> repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<CommandResponse> Handle(CreateBigGoalCommand request, CancellationToken cancellationToken)
        {
            var bigGoal = _mapper.Map<BigGoal>(request);

            _repo.Create(bigGoal);

            if (await _repo.SaveAsync(cancellationToken))
            {
                return CommandResponse.Success(bigGoal.Id);
            }

            return CommandResponse.Failure(500);
        }
    }
}
