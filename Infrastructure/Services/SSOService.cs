using Application.Services.Interfaces;
using Domain;
using Domain.Dtos.Account.SSO;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class SSOService : ISSOService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _accessor;

        public SSOService(IHttpClientFactory clientFactory, IHttpContextAccessor accessor)
        {
            _clientFactory = clientFactory;
            _accessor = accessor;
        }

        public async Task<ProfileRequest> GetProfileAsync(string token)
        {
            var httpClient = _clientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jwksResponse = await httpClient.GetAsync("https://usw.msrt.ir/oauth2/jwks");
            if (jwksResponse.IsSuccessStatusCode)
            {
                // user info
                var userInfoResponse = await httpClient.GetAsync("https://usw.msrt.ir/api/v1/User/userinfo");
                if (userInfoResponse.IsSuccessStatusCode)
                {
                    var userInfoReadAsString = await userInfoResponse.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ProfileRequest>(userInfoReadAsString);
                }
            }
            else
            {
                throw new Exception("کاربر در سیستم وچود ندارد");
            }

            return null;
        }

        public async Task<OAuthResponseToken> GetTokenAsync(String code)
        {
            var httpClient = _clientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(180);
            //تعریف پارامترهای درخواست به صورت فرم دیتا برای دریافت توکن
            using MultipartFormDataContent multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent("authorization_code", Encoding.UTF8, MediaTypeNames.Text.Plain), "grant_type");
            multipartContent.Add(new StringContent(code, Encoding.UTF8, MediaTypeNames.Text.Plain), "code");
            multipartContent.Add(new StringContent("openid profile", Encoding.UTF8, MediaTypeNames.Text.Plain), "scope");
            multipartContent.Add(new StringContent(SD.GetRedirectUrl(_accessor), Encoding.UTF8,
            MediaTypeNames.Text.Plain), "redirect_uri");
            multipartContent.Add(new StringContent(SD.ClientId, Encoding.UTF8, MediaTypeNames.Text.Plain), "client_id");
            multipartContent.Add(new StringContent(SD.ClientSecret, Encoding.UTF8, MediaTypeNames.Text.Plain), "client_secret");

            var tokenResponse = await httpClient.PostAsync("https://usw.msrt.ir/oauth2/token", multipartContent);
            if (tokenResponse.IsSuccessStatusCode)
            {
                var tokenReadAsString = await tokenResponse.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OAuthResponseToken>(tokenReadAsString);
            }

            return null;
        }
    }
}
