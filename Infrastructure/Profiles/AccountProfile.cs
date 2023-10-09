using AutoMapper;
using Domain.Dtos.Account.Permissions;
using Domain.Dtos.Account.Roles;
using Domain.Dtos.Account.SSO;
using Domain.Dtos.Account.User;
using Domain.Dtos.Account.Users;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using System.Data;

namespace Infrastructure.Profiles
{
    public class AccountProfile : Profile
    {


        public AccountProfile()
        {
            CreateMap<SSOUserInfo, UserProfile>()
                .ForMember(b => b.NationalId, d => d.MapFrom(e => e.nationalId))
                .ForMember(b => b.FullName, d => d.MapFrom(e => String.Concat(e.firstName, " ", e.lastName)))
                .ForMember(b => b.Mobile, d => d.MapFrom(e => e.mobile))
                .ForMember(b => b.Gender, d => d.MapFrom(e => e.gender));

            var rnd = new Random();

            CreateMap<User, UserSummary>()
                .ForMember(b => b.FullName, d => d.MapFrom(e => String.Concat(Names[rnd.Next(6)], ' ', Families[rnd.Next(6)])))
                .ForMember(b => b.Role, d => d.MapFrom(e => e.Role.Title))
                .ForMember(b => b.NationalId, d => d.MapFrom(e => e.NationalId));
            CreateMap<User, UserListDto>()
                .ForMember(b => b.FullName, d => d.MapFrom(e => String.Concat(Names[rnd.Next(6)], ' ', Families[rnd.Next(6)])))
                .ForMember(b => b.NationalId, d => d.MapFrom(e => e.NationalId))
                .ForMember(b => b.PhoneNumber, d => d.MapFrom(e => "0" + rnd.NextInt64(1000000000, 9999999999)));

            CreateMap<PermissionItem, PermissionSummary>()
                .ForMember(b => b.Value, d => d.MapFrom(e => e.PageValue));

            CreateMap<Role, RoleSummary>();
            CreateMap<Role, SelectSummary>()
                .ForMember(b => b.Id, e => e.MapFrom(s => s.Id))
                .ForMember(b => b.Title, e => e.MapFrom(s => s.Title));
            CreateMap<Role, RoleDetails>()
                .ForMember(b => b.Permissios, d => d.MapFrom(e => e.Permissions.Select(b => b.PermissionId)));


            CreateMap<UserJoinRequest, UserRequestsSummary>()
                .ForMember(b => b.RequestTime, d => d.MapFrom(e => e.CreatedAt));
        }

        private List<String> Names =>
            new List<string>
            {
                "امیر",
                "محمد",
                "زهرا",
                "پریسا",
                "پیمان",
                "رضا",
            };

        private List<String> Families =>
            new List<string>
            {
                "قادری",
                "محمدی",
                "مرادی",
                "حیدری",
                "منوچهری",
                "نویدی",
            };
    }
}
