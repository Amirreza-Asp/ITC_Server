using AutoMapper;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using System.ComponentModel.DataAnnotations;

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

        public CreatePersonCommandHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CommandResponse> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
        {
            var person = _mapper.Map<Person>(request);

            _context.Add(person);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
            {
                return CommandResponse.Success(person.Id);
            }

            return CommandResponse.Failure(500);
        }
    }
}
