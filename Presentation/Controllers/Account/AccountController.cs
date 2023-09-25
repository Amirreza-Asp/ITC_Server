﻿using Application.Repositories;
using Application.Services.Interfaces;
using Application.Utility;
using AutoMapper;
using Domain;
using Domain.Dtos.Account.User;
using Domain.Entities.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Presentation.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ISSOService _ssoService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IRepository<Company> _companyRepo;
        private readonly IHttpContextAccessor _contextAccessor;

        public AccountController(IHttpClientFactory clientFactory, ISSOService ssoService, IAuthService authService, IMapper mapper, IRepository<Company> companyRepo, IHttpContextAccessor contextAccessor)
        {
            _clientFactory = clientFactory;
            _ssoService = ssoService;
            _authService = authService;
            _mapper = mapper;
            _companyRepo = companyRepo;
            _contextAccessor = contextAccessor;
        }

        [Route("Login")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromQuery] String redirectUrl, [FromQuery] bool logout)
        {
            if (logout)
                await _authService.LogoutAsync();


            await _authService.LoginAsync(
               nationalId: "0123456789",
               uswToken: "sdlkasdklasdjklasdklasdkl");

            return Redirect(redirectUrl);


            var state = Guid.NewGuid().ToString().Replace("-", "");
            HttpContext.Session.SetString("state", state);
            HttpContext.Session.SetString("redirect-url", redirectUrl);

            var domain = logout ? "https://usw.msrt.ir/oauth2/logout?" : "https://usw.msrt.ir/oauth2/authorize?";

            var url = domain +
                $"response_type=code" +
                $"&scope=openid profile" +
                $"&client_id={SD.ClientId}" +
                $"&state={state}" +
                //$"&client-secret={SD.ClientSecret}" +
                $"&redirect_uri={SD.GetRedirectUrl(_contextAccessor)}";

            return Redirect(url);
        }

        [Route("authorizeLogin")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AuthorizeLogin([FromQuery] string code, [FromQuery] string state)
        {
            var stateCheck = HttpContext.Session.GetString("state");
            if (string.IsNullOrEmpty(stateCheck) || stateCheck != state)
            {
                return BadRequest();
            }

            HttpContext.Session.Remove("state");
            HttpContext.Session.SetString("code", code);

            var uswToken = await _ssoService.GetTokenAsync(code);

            if (uswToken == null)
                return BadRequest();

            var userProfile = await _ssoService.GetProfileAsync(uswToken.access_token);

            if (!await _authService.ExistsAsync(userProfile.data.nationalId))
            {
                await _authService.CreateAsync(userProfile);
            }

            await _authService.LoginAsync(
                nationalId: userProfile.data.nationalId,
                uswToken: uswToken.access_token);


            var redirect = HttpContext.Session.GetString("redirect-url");
            HttpContext.Session.Remove("redirect-url");

            return Redirect(redirect);
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            var httpClient = _clientFactory.CreateClient();

            var response = await httpClient.GetAsync("https://usw.msrt.ir/oauth2/logout");
            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {
            return Ok(new UserProfile
            {
                Company = "رازی",
                FullName = "امیررضا محمدی",
                Gender = "مرد",
                Mobile = "09211573936",
                NationalId = SD.DefaultNationalId,
                Permissions = await _authService.GetPermissionsAsync(SD.DefaultNationalId)
            });

            var hashedUswToken = HttpContext.Request.Cookies[SD.UswToken];
            var uswToken = ProtectorData.Decrypt(hashedUswToken);

            try
            {
                var userProfile = await _ssoService.GetProfileAsync(uswToken);
                if (userProfile == null || !userProfile.isSuccess)
                    return Unauthorized();

                var user = _mapper.Map<UserProfile>(userProfile.data);

                var company = await _companyRepo.FirstOrDefaultAsync(filter: b => b.Users.Any(b => b.NationalId == user.NationalId));
                user.Company = company.Title;
                user.Permissions = await _authService.GetPermissionsAsync(user.NationalId);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }
    }
}
