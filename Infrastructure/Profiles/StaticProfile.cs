using AutoMapper;
using Domain.Dtos.Static;
using Domain.Entities.Business;

namespace Infrastructure.Profiles
{
    public class StaticProfile : Profile
    {
        public StaticProfile()
        {
            CreateMap<ProgramYear, ProgramYearSummary>();
        }
    }
}
