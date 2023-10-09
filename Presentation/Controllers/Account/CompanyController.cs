using Application.Repositories;
using Domain;
using Domain.Dtos.Companies;
using Domain.Dtos.Shared;
using Domain.Queries.Shared;
using Infrastructure.CQRS.Account.Companies;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _repo;
        private readonly IMediator _mediator;

        public CompanyController(IMediator mediator, ICompanyRepository repo)
        {
            _mediator = mediator;
            _repo = repo;
        }

        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<CompanySummary>> GetAll([FromBody] GridQuery query, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<CompanySummary>(query, cancellationToken);
        }

        [Route("BigGoals")]
        [HttpPost]
        [AccessControl(PermissionsSD.System_ShowBigGoals)]
        public async Task<List<CompanyBigGoals>> BigGoals([FromBody] CompanyBigGoalsQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        [Route("OperationalObjective")]
        [HttpPost]
        [AccessControl(PermissionsSD.System_ShowOperationalObjectives)]
        public async Task<List<CompanyOperationalObjectives>> OperationalObjectives([FromBody] OperationalObjectiveQuery query, CancellationToken cancellation)
        {
            return await _mediator.Send(query, cancellation);
        }

        [Route("Projects")]
        [HttpPost]
        [AccessControl(PermissionsSD.System_ShowProjects)]
        public async Task<List<CompanyProjects>> Projects([FromBody] CompanyProjectsQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        [Route("PracticalActions")]
        [HttpPost]
        [AccessControl(PermissionsSD.System_ShowPracticalActions)]
        public async Task<List<CompanyPracticalActions>> PracticalActions([FromBody] CompanyPracticalActionQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        [Route("Manpowers")]
        [HttpPost]
        [AccessControl(PermissionsSD.System_ShowManpowers)]
        public async Task<List<CompanyManpower>> Manpowers([FromBody] CompaniesManpowerQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        [Route("HardwareEquipments")]
        [HttpPost]
        [AccessControl(PermissionsSD.System_ShowHardwareEquipments)]
        public async Task<List<CompanyHardwareEquipments>> HardwareEquipments([FromBody] CompaniesHardwareEquipentsQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        [Route("Systems")]
        [HttpPost]
        [AccessControl(PermissionsSD.System_ShowSystems)]
        public async Task<List<CompanySystem>> Systems([FromBody] CompaniesSystemQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        [Route("Users")]
        [HttpPost]
        [AccessControl(PermissionsSD.System_ShowUsers)]
        public async Task<List<CompanyUsers>> Users([FromBody] CompaniesUsersQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }


        [Route("Provinces")]
        [HttpGet]
        public async Task<List<String>> GetProvices(CancellationToken cancellationToken)
        {
            return await _repo.GetProvincesAsync(cancellationToken);
        }

        [Route("ProvinceCities")]
        [HttpGet]
        public async Task<List<String>> GetProvinceCities([FromQuery] String province, CancellationToken cancellationToken)
        {
            return await _repo.GetProvinceCitiesAsync(province, cancellationToken);
        }

        [Route("Types")]
        [HttpGet]
        public async Task<List<String>> GetTypes(CancellationToken cancellationToken)
        {
            return await _repo.GetTypesAsync(cancellationToken);
        }

    }
}
