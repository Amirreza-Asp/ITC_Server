using AutoMapper;
using Domain.Dtos.BigGoals;
using Domain.Dtos.Companies;
using Domain.Dtos.OperationalObjectives;
using Domain.Dtos.People;
using Domain.Dtos.PracticalActions;
using Domain.Dtos.Projects;
using Domain.Dtos.Refrences;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using Domain.Entities.Business;
using Domain.SubEntities;
using Infrastructure.CQRS.Business.BigGoals;
using Infrastructure.CQRS.Business.HardwareEquipments;
using Infrastructure.CQRS.Business.OperationalObjectives;
using Infrastructure.CQRS.Business.People;
using Infrastructure.CQRS.Business.PracticalActions;
using Infrastructure.CQRS.Business.Projects;
using Infrastructure.CQRS.Business.Systems;

namespace Infrastructure.Profiles
{
    public class BusinessProfile : Profile
    {

        public BusinessProfile()
        {
            // Big goal
            CreateMap<BigGoal, BigGoal>();
            CreateMap<BigGoal, BigGoalSummary>()
                .ForMember(b => b.OperationalObjectiveCount, d => d.MapFrom(d => d.OperationalObjectives.Count()))
                .ForMember(b => b.IndicatorsCount, d => d.MapFrom(d => d.Indicators.Count()));

            CreateMap<BigGoal, BigGoalsListDto>()
                .ForMember(b => b.Year, d => d.MapFrom(e => e.ProgramYear.Year));
            CreateMap<CreateBigGoalCommand, BigGoal>()
               .ForMember(b => b.Id, d => d.MapFrom(e => Guid.NewGuid()));
            CreateMap<BigGoal, BigGoalDetails>()
                .ForMember(b => b.ProgramYear, d => d.MapFrom(e => e.ProgramYear.Year))
                .ForMember(b => b.Progress, d => d.Ignore())
                .ForMember(b => b.OperationalObjectiveCount, d => d.MapFrom(e => e.OperationalObjectives.Count()))
                .ForMember(b => b.Indicators, d => d.MapFrom(e => e.Indicators.Select(e => e.Indicator)));

            // Operational objective
            CreateMap<CreateOperationalObjectiveCommand, OperationalObjective>()
                .ForMember(b => b.Id, d => d.MapFrom(e => Guid.NewGuid()))
                .ForMember(b => b.Budget, d => d.MapFrom(e => -1));
            CreateMap<OperationalObjective, OperationalObjectiveSummary>();

            CreateMap<OperationalObjective, OperationalObjectiveCard>()
                .ForMember(b => b.Active, d => d.MapFrom(b => b.Deadline >= DateTime.Now))
                .ForMember(b => b.Projects, d => d.MapFrom(b => b.Projects))
                .ForMember(b => b.Actions, d => d.MapFrom(b => b.PracticalActions))
                .ForMember(b => b.IndicatorsCount, d => d.MapFrom(b => b.Indicators.Count()))
                .ForMember(b => b.Progress, d => d.Ignore());

            CreateMap<OperationalObjectiveIndicator, OperationalObjectiveIndicator>();
            CreateMap<OperationalObjective, OperationalObjectiveDetails>()
              .ForMember(b => b.ProjectsCount, d => d.MapFrom(b => b.Projects.Count()))
              .ForMember(b => b.Active, d => d.MapFrom(b => b.Deadline >= DateTime.Now))
              .ForMember(b => b.Title, d => d.MapFrom(e => e.BigGoal.Title + " > " + e.Title))
              .ForMember(b => b.PracticalActionsCount, d => d.MapFrom(b => b.PracticalActions.Count()))
              .ForMember(b => b.Indicators, d => d.MapFrom(b => b.Indicators.Select(e => e.Indicator)))
              .ForMember(b => b.Progress, d => d.Ignore());

            // Person
            CreateMap<Person, PersonSummary>()
                .ForMember(b => b.Expertises, d => d.MapFrom(b => b.Expertises.Select(d => d.Title)));
            CreateMap<Person, SelectSummary>()
                .ForMember(b => b.Title, d => d.MapFrom(e => e.Name + " " + e.Family));
            CreateMap<CreatePersonCommand, Person>()
                .ForMember(b => b.Expertises, d => d.MapFrom(b => b.Expertises.Select(d => new Expertise { Title = d })))
                .ForMember(b => b.Id, d => d.MapFrom(d => Guid.NewGuid()));

            // Hardware equipment
            CreateMap<HardwareEquipment, HardwareEquipment>();
            CreateMap<HardwareEquipment, SelectSummary>();
            CreateMap<CreateHardwareEquipmentCommand, HardwareEquipment>()
                .ForMember(b => b.Id, d => d.MapFrom(r => Guid.NewGuid()));

            // System
            CreateMap<Domain.Entities.Business.System, Domain.Entities.Business.System>();
            CreateMap<Domain.Entities.Business.System, SelectSummary>();
            CreateMap<Domain.Entities.Business.System, SystemDetails>();
            CreateMap<CreateSystemCommand, Domain.Entities.Business.System>()
                .ForMember(b => b.Id, d => d.MapFrom(r => Guid.NewGuid()))
                .ForMember(b => b.BuildInCompany, d => d.MapFrom(e => e.Company))
                .ForMember(b => b.Company, d => d.Ignore());

            // Practical action
            CreateMap<CreatePracticalActionCommand, PracticalAction>()
               .ForMember(b => b.Financials, d => d.MapFrom(b => b.Financials.Select(d => new Financial { Title = d })))
               .ForMember(b => b.Id, d => d.MapFrom(d => Guid.NewGuid()));

            CreateMap<PracticalAction, PracticalActionSummary>()
               .ForMember(b => b.Financials, d => d.MapFrom(b => b.Financials.Select(d => d.Title)))
               .ForMember(b => b.LeaderName, d => d.MapFrom(d => String.Concat(d.Leader.Name, ' ', d.Leader.Family)))
               .ForMember(b => b.OperationalObjectiveTitle, d => d.MapFrom(d => d.OperationalObjective.Title));

            CreateMap<PracticalAction, PracticalActionDetails>()
               .ForMember(b => b.Financials, d => d.MapFrom(b => b.Financials.Select(d => d.Title)))
               .ForMember(b => b.LeaderName, d => d.MapFrom(d => String.Concat(d.Leader.Name, ' ', d.Leader.Family)))
               .ForMember(b => b.OperationalObjectiveTitle, d => d.MapFrom(d => d.OperationalObjective.Title))
               .ForMember(b => b.Title, d => d.MapFrom(e => e.OperationalObjective.BigGoal.Title + " > " + e.OperationalObjective.Title + " > " + e.Title))
               .ForMember(b => b.Indicators, d => d.MapFrom(d => d.Indicators.Select(e => e.Indicator)));

            CreateMap<PracticalAction, ProjectActionCard>()
                .ForMember(b => b.Active, b => b.MapFrom(d => d.Deadline >= DateTime.Now))
                .ForMember(b => b.Type, d => d.MapFrom(b => "اقدام"))
                .ForMember(b => b.Financials, d => d.MapFrom(b => b.Financials.Select(s => s.Title)))
                .ForMember(b => b.IndicatorsCount, d => d.MapFrom(e => e.Indicators.Count()));

            // Project
            CreateMap<CreateProjectCommand, Project>()
               .ForMember(b => b.Financials, d => d.MapFrom(b => b.Financials.Select(d => new Financial { Title = d })))
               .ForMember(b => b.Id, d => d.MapFrom(d => Guid.NewGuid()));
            CreateMap<Project, ProjectSummary>()
               .ForMember(b => b.Financials, d => d.MapFrom(b => b.Financials.Select(d => d.Title)))
               .ForMember(b => b.LeaderName, d => d.MapFrom(d => String.Concat(d.Leader.Name, ' ', d.Leader.Family)))
               .ForMember(b => b.OperationalObjectiveTitle, d => d.MapFrom(d => d.OperationalObjective.Title));
            CreateMap<Project, ProjectActionCard>()
                .ForMember(b => b.Active, b => b.MapFrom(d => d.GuaranteedFulfillmentAt >= DateTime.Now))
                .ForMember(b => b.Type, b => b.MapFrom(d => "پروژه"))
                .ForMember(b => b.Financials, d => d.MapFrom(b => b.Financials.Select(s => s.Title)))
                .ForMember(b => b.Deadline, d => d.MapFrom(b => b.GuaranteedFulfillmentAt))
                .ForMember(b => b.IndicatorsCount, d => d.MapFrom(e => e.Indicators.Count()));
            CreateMap<Project, ProjectDetails>()
               .ForMember(b => b.Financials, d => d.MapFrom(b => b.Financials.Select(d => d.Title)))
               .ForMember(b => b.LeaderName, d => d.MapFrom(d => String.Concat(d.Leader.Name, ' ', d.Leader.Family)))
               .ForMember(b => b.Indicators, d => d.MapFrom(d => d.Indicators.Select(e => e.Indicator)))
               .ForMember(b => b.Title, d => d.MapFrom(e => e.OperationalObjective.BigGoal.Title + " > " + e.OperationalObjective.Title + " > " + e.Title))
               .ForMember(b => b.OperationalObjectiveTitle, d => d.MapFrom(d => d.OperationalObjective.Title));


            // Company
            CreateMap<Company, CompanyBigGoals>()
                .ForMember(b => b.CompanyName, d => d.MapFrom(e => e.Title))
                .ForMember(b => b.CompanyId, d => d.MapFrom(e => e.Id))
                .ForMember(b => b.BigGoals, d => d.MapFrom(e => e.BigGoals));


            CreateMap<Company, CompanySummary>()
                .ForMember(b => b.Title, d => d.MapFrom(e => e.Title))
                .ForMember(b => b.Province, d => d.MapFrom(e => e.Province))
                .ForMember(b => b.City, d => d.MapFrom(e => e.City));


            CreateMap<Indicator, IndicatorCard>()
                .ForMember(b => b.Title, e => e.MapFrom(e => e.Category.Title))
                .ForMember(b => b.RealCurrentValue, e => e.MapFrom(e =>
                     e.Progresses.Any() ?
                        e.Progresses.OrderByDescending(b => b.ProgressTime).First().Value : 0))
                .ForMember(b => b.RealProgress, e => e.MapFrom(e =>
                     e.Progresses.Any() ?
                        Convert.ToInt32((e.Progresses.OrderByDescending(b => b.ProgressTime).First().Value - e.InitValue) * 100 /
                        (e.GoalValue - e.InitValue)) : 0));


        }
    }
}
