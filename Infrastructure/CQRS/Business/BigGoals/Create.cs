using Application.Repositories;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.BigGoals
{
    public class CreateBigGoalCommand : IRequest<CommandResponse>
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        public Guid? ProgramYearId { get; set; }
    }

    public class CreateBigGoalCommandHandler : IRequestHandler<CreateBigGoalCommand, CommandResponse>
    {
        private readonly IRepository<BigGoal> _repo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserAccessor _userAccessor;

        public CreateBigGoalCommandHandler(IRepository<BigGoal> repo, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUserAccessor userAccessor)
        {
            _repo = repo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateBigGoalCommand request, CancellationToken cancellationToken)
        {
            if (request.Deadline < request.StartedAt)
                return CommandResponse.Failure(400, "تاریخ شروع نمیتواند از مهلت انجام بیشتر باشد");

            var comapnyId = _userAccessor.GetCompanyId();

            var bigGoal = _mapper.Map<BigGoal>(request);
            bigGoal.CompanyId = comapnyId.Value;

            _repo.Create(bigGoal);

            if (await _repo.SaveAsync(cancellationToken))
            {
                return CommandResponse.Success(bigGoal.Id);
            }

            return CommandResponse.Failure(500);
        }
    }
}
