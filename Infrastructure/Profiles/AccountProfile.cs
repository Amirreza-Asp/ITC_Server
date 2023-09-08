using AutoMapper;
using Domain.Dtos.Account.Permissions;
using Domain.Dtos.Account.Roles;
using Domain.Dtos.Account.SSO;
using Domain.Dtos.Account.User;
using Domain.Entities.Account;

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

            CreateMap<PermissionItem, PermissionSummary>()
                .ForMember(b => b.Value, d => d.MapFrom(e => e.PageValue));

            CreateMap<Role, RoleSummary>();
            CreateMap<Role, RoleDetails>()
                .ForMember(b => b.Permissios, d => d.MapFrom(e => e.Permissions.Select(b => b.PermissionId)));

        }
    }
}
