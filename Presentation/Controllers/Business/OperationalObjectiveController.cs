﻿using Application.Repositories;
using AutoMapper;
using Domain;
using Domain.Dtos.OperationalObjectives;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using Domain.Queries.Shared;
using Domain.Utiltiy;
using Infrastructure.CQRS.Business.OperationalObjectives;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomeAttributes;

namespace Presentation.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationalObjectiveController : ControllerBase
    {
        private readonly IRepository<OperationalObjective> _repo;
        private readonly IRepository<OperationalObjectiveIndicator> _increpo;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public OperationalObjectiveController(IRepository<OperationalObjective> repo, IMediator mediator, IMapper mapper, IRepository<OperationalObjectiveIndicator> increpo)
        {
            _repo = repo;
            _mediator = mediator;
            _mapper = mapper;
            _increpo = increpo;
        }


        [Route("GetAll")]
        [HttpPost]
        public async Task<ListActionResult<OperationalObjectiveSummary>> GetAll(GridQuery query, CancellationToken cancellationToken)
        {
            return await _repo.GetAllAsync<OperationalObjectiveSummary>(query, cancellationToken);
        }

        [Route("GetAllByBigGoalId")]
        [HttpGet]
        public async Task<List<OperationalObjectiveCard>> GetAll([FromQuery] GetOperationalObjectivesByBigGoalIdQuery query, CancellationToken cancellationToken)
        {
            return await _mediator.Send(query, cancellationToken);
        }

        [Route("Find")]
        [HttpGet]
        public async Task<OperationalObjectiveCard> Find([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            return
                await _repo.FirstOrDefaultAsync<OperationalObjectiveCard>(b => b.Id == id);
        }

        [Route("Details")]
        [HttpGet]
        public async Task<OperationalObjectiveDetails> Details([FromQuery] Guid id, CancellationToken cancellationToken)
        {
            var data = await _repo.FirstOrDefaultAsync<OperationalObjectiveDetails>(b => b.Id == id, cancellationToken);

            data.Progress = Calculator.CalcProgress(data.Indicators);

            data.Indicators.ForEach(item =>
            {
                item.ScheduleProgress = Calculator.CalcProgress(item);
                item.ScheduleCurrentValue = Calculator.CalcCurrentValue(item);
            });

            return data;
        }

        [Route("Create")]
        [HttpPost]
        [AccessControl(PermissionsSD.Company_AddOperationalObjective)]
        public async Task<CommandResponse> Create([FromBody] CreateOperationalObjectiveCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }


        [Route("Update")]
        [HttpPut]
        [AccessControl(PermissionsSD.Company_EditOperationalObjective)]
        public async Task<CommandResponse> Update([FromBody] UpdateOperationalObjectiveCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [HttpDelete]
        [Route("Delete")]
        [AccessControl(PermissionsSD.Company_RemoveOperationalObjective)]
        public async Task<CommandResponse> Remove([FromQuery] DeleteOperationObjectiveCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [HttpPost]
        [Route("AddIndicator")]
        [AccessControl(PermissionsSD.Company_ManageOperationalObjectiveIndicator)]
        public async Task<CommandResponse> AddIndicator([FromBody] AddOperationalObjectiveIndicatorCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }


        [HttpDelete]
        [Route("RemoveIndicator")]
        [AccessControl(PermissionsSD.Company_ManageOperationalObjectiveIndicator)]
        public async Task<CommandResponse> RemoveIndicator([FromQuery] RemoveOperationalObjectiveIndicatorCommand command, CancellationToken cancellation)
        {
            return await _mediator.Send(command, cancellation);
        }
    }
}
