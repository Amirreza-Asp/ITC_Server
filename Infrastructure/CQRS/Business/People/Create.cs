using Application.Utility;
using AutoMapper;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Infrastructure.CQRS.Business.People
{
    public class CreatePersonCommand : IRequest<CommandResponse>
    {
        [Required]
        public String Name { get; set; }

        [Required]
        public String Family { get; set; }

        [Required]
        public String JobTitle { get; set; }

        [Required]
        public String Education { get; set; }

        [Required]
        public List<String> Expertises { get; set; }

    }

    public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreatePersonCommandHandler(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResponse> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var person = _mapper.Map<Person>(request);

            var comapnyId = (_httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity).GetCompanyId();
            person.CompanyId = comapnyId.Value;

            _context.Add(person);


            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return CommandResponse.Success(person.Id);
            }

            return CommandResponse.Failure(500);
        }
    }
}
