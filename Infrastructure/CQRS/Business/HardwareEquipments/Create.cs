using AutoMapper;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.HardwareEquipments
{
    public class CreateHardwareEquipmentCommand : IRequest<CommandResponse>
    {
        [Required]
        public String Title { get; set; }

        [Required]
        public String BrandName { get; set; }

        [Range(1, int.MaxValue)]
        public int Count { get; set; }

        public String Description { get; set; }

        [Required]
        public String SupportType { get; set; }
    }

    public class CreateHardwareEquipmentCommandHandler : IRequestHandler<CreateHardwareEquipmentCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreateHardwareEquipmentCommandHandler(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CommandResponse> Handle(CreateHardwareEquipmentCommand request, CancellationToken cancellationToken)
        {
            var heq = _mapper.Map<HardwareEquipment>(request);

            _context.Add(heq);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(heq.Id);

            return CommandResponse.Failure(500);
        }
    }
}
