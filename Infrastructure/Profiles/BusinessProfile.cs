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
using Infrastructure.CQRS.Business.BigGoals.Commands;
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
                .ForMember(b => b.OperationalObjectiveCount, d => d.MapFrom(d => d.OperationalObjectives.Count()));
            CreateMap<BigGoal, BigGoalsListDto>()
                .ForMember(b => b.Year, d => d.MapFrom(e => e.ProgramYear.Year));
            CreateMap<CreateBigGoalCommand, BigGoal>()
               .ForMember(b => b.Id, d => d.MapFrom(e => Guid.NewGuid()));

            // Operational objective
            CreateMap<CreateOperationalObjectiveCommand, OperationalObjective>()
                .ForMember(b => b.Id, d => d.MapFrom(e => Guid.NewGuid()))
                .ForMember(b => b.Budget, d => d.MapFrom(e => -1));
            CreateMap<OperationalObjective, OperationalObjectiveSummary>();
            CreateMap<OperationalObjective, OperationalObjectiveDetails>()
                .ForMember(b => b.Active, d => d.MapFrom(b => b.Deadline >= DateTime.Now))
                .ForMember(b => b.Projects, d => d.MapFrom(b => b.Projects))
                .ForMember(b => b.Actions, d => d.MapFrom(b => b.PracticalActions));


            // Person
            CreateMap<Person, PersonSummary>()
                .ForMember(b => b.Expertises, d => d.MapFrom(b => b.Expertises.Select(d => d.Title)));
            CreateMap<CreatePersonCommand, Person>()
                .ForMember(b => b.Expertises, d => d.MapFrom(b => b.Expertises.Select(d => new Expertise { Title = d })))
                .ForMember(b => b.Id, d => d.MapFrom(d => Guid.NewGuid()));

            // Hardware equipment
            CreateMap<HardwareEquipment, HardwareEquipment>();
            CreateMap<CreateHardwareEquipmentCommand, HardwareEquipment>()
                .ForMember(b => b.Id, d => d.MapFrom(r => Guid.NewGuid()));

            // System
            CreateMap<Domain.Entities.Business.System, Domain.Entities.Business.System>();
            CreateMap<Domain.Entities.Business.System, SystemDetails>();
            CreateMap<CreateSystemCommand, Domain.Entities.Business.System>()
                .ForMember(b => b.Id, d => d.MapFrom(r => Guid.NewGuid()))
                .ForMember(b => b.BuildInCompany, d => d.MapFrom(e => e.Company));

            // Practical action
            CreateMap<CreatePracticalActionCommand, PracticalAction>()
               .ForMember(b => b.Financials, d => d.MapFrom(b => b.Financials.Select(d => new Financial { Title = d })))
               .ForMember(b => b.Id, d => d.MapFrom(d => Guid.NewGuid()));

            CreateMap<PracticalAction, PracticalActionSummary>()
               .ForMember(b => b.Financials, d => d.MapFrom(b => b.Financials.Select(d => d.Title)))
               .ForMember(b => b.LeaderName, d => d.MapFrom(d => String.Concat(d.Leader.Name, ' ', d.Leader.Family)))
               .ForMember(b => b.OperationalObjectiveTitle, d => d.MapFrom(d => d.OperationalObjective.Title));

            CreateMap<PracticalAction, ProjectActionCard>()
                .ForMember(b => b.Active, b => b.MapFrom(d => d.Deadline >= DateTime.Now))
                .ForMember(b => b.Type, d => d.MapFrom(b => "اقدام کاربردی"))
                .ForMember(b => b.Financials, d => d.MapFrom(b => b.Financials.Select(s => s.Title)));

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
                .ForMember(b => b.Deadline, d => d.MapFrom(b => b.GuaranteedFulfillmentAt));

            // Company
            CreateMap<Company, CompanyBigGoals>()
                .ForMember(b => b.CompanyName, d => d.MapFrom(e => e.NameUniversity))
                .ForMember(b => b.CompanyId, d => d.MapFrom(e => e.Id))
                .ForMember(b => b.BigGoals, d => d.MapFrom(e => e.BigGoals));


            CreateMap<Company, CompanySummary>()
                .ForMember(b => b.Title, d => d.MapFrom(e => e.NameUniversity))
                .ForMember(b => b.Province, d => d.MapFrom(e => e.ProvinceName))
                .ForMember(b => b.City, d => d.MapFrom(e => e.CityName));

        }

    }
}
