using Application.Repositories;
using Domain;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Business.HardwareEquipments;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class HardwareEquipmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IRepository<HardwareEquipment> _repo;

        public HardwareEquipmentController(IMediator mediator, IRepository<HardwareEquipment> repo)
        {
            _mediator = mediator;
            _repo = repo;
        }

        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<HardwareEquipment>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<HardwareEquipment>(query, cancellationToken);
        }

        [HttpGet("Find/{id}")]
        public async Task<HardwareEquipment> Find(Guid id, CancellationToken cancellation)
        {
            return await _repo.FirstOrDefaultAsync<HardwareEquipment>(d => d.Id == id, cancellationToken: cancellation);
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.Company_AddHardwareEquipment)]
        public async Task<CommandResponse> Create([FromBody] CreateHardwareEquipmentCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
