using Application.Repositories;
using Application.Utility;
using AutoMapper;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateBigGoalCommandHandler(IRepository<BigGoal> repo, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResponse> Handle(CreateBigGoalCommand request, CancellationToken cancellationToken)
        {
            if (request.Deadline < request.StartedAt)
                return CommandResponse.Failure(400, "تاریخ شروع نمیتواند از مهلت انجام بیشتر باشد");

            var comapnyId = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).GetCompanyId();

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
