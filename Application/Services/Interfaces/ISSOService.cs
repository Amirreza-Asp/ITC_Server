using Domain.Dtos.Account.SSO;

namespace Application.Services.Interfaces
{
    public interface ISSOService
    {
        Task<ProfileRequest> GetProfileAsync(string token);

        Task<OAuthResponseToken> GetTokenAsync(string code);
    }
}
