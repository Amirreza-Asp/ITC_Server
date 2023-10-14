﻿using Application.Repositories;
using Application.Services.Interfaces;
using AutoMapper;
using Domain;
using Domain.Dtos.Account.User;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly ApplicationDbContext _context;
        private readonly IUserAccessor _userAccessor;
        private readonly IMemoryCache _memoryCache;


        public AccountController(IHttpClientFactory clientFactory, ISSOService ssoService, IAuthService authService, IMapper mapper, IRepository<Company> companyRepo, IHttpContextAccessor contextAccessor, ApplicationDbContext context, IUserAccessor userAccessor, IMemoryCache memoryCache)
        {
            _clientFactory = clientFactory;
            _ssoService = ssoService;
            _authService = authService;
            _mapper = mapper;
            _companyRepo = companyRepo;
            _contextAccessor = contextAccessor;
            _context = context;
            _userAccessor = userAccessor;
            _memoryCache = memoryCache;
        }

        [Route("Login")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromQuery] String redirectUrl)
        {
            var user = await _context.Users.FirstOrDefaultAsync(b => b.CompanyId == null);

            await _authService.LoginAsync(
               nationalId: user.NationalId,
               uswToken: "sdlkasdklasdjklasdklasdkl");

            return Ok();
            return Redirect(redirectUrl);


            var state = Guid.NewGuid().ToString().Replace("-", "");
            HttpContext.Session.SetString("state", state);
            HttpContext.Session.SetString("redirect-url", redirectUrl);


            var url = "https://usw.msrt.ir/oauth2/authorize?" +
                $"response_type=code" +
                $"&scope=openid profile" +
                $"&client_id={SD.ClientId}" +
                $"&state={state}" +
                //$"&client-secret={SD.ClientSecret}" +
                $"&redirect_uri={SD.GetRedirectUrl(_contextAccessor)}";

            return Redirect(url);
        }


        [Route("RaziLogin")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RaziLogin([FromQuery] String redirectUrl)
        {
            var user = await _context.Users.FirstOrDefaultAsync(b => b.CompanyId == Guid.Parse("aa12b4d2-652c-407a-a569-9edcd1e2c467"));

            await _authService.LoginAsync(
               nationalId: user.NationalId,
               uswToken: "sdlkasdklasdjklasdklasdkl");

            return Ok();
            return Redirect(redirectUrl);
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
        public async Task<CommandResponse> Logout()
        {
            return await _authService.LogoutAsync();
            //var httpClient = _clientFactory.CreateClient();

            //var response = await httpClient.GetAsync("https://usw.msrt.ir/oauth2/logout");
            //if (response.IsSuccessStatusCode)
            //{
            //return CommandResponse.Success();
            //}

            //return CommandResponse.Failure((int)response.StatusCode);
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {

            var company = await _companyRepo.FirstOrDefaultAsync(b => b.Id == _userAccessor.GetCompanyId());
            return Ok(new UserProfile
            {
                Company = company == null ? "وزارت علوم" : company.NameUniversity,
                CompanyId = company?.Id,
                FullName = "امیررضا محمدی",
                Gender = "مرد",
                Mobile = "09211573936",
                NationalId = SD.DefaultNationalId,
                Permissions = await _authService.GetPermissionsAsync(SD.DefaultNationalId)
            });

            //var hashedUswToken = HttpContext.Request.Cookies[SD.UswToken];
            //var uswToken = ProtectorData.Decrypt(hashedUswToken);

            //try
            //{
            //    var userProfile = await _ssoService.GetProfileAsync(uswToken);
            //    if (userProfile == null || !userProfile.isSuccess)
            //        return Unauthorized();

            //    var user = _mapper.Map<UserProfile>(userProfile.data);

            //    var company = await _companyRepo.FirstOrDefaultAsync(filter: b => b.Users.Any(b => b.NationalId == user.NationalId));
            //    user.Company = company.NameUniversity;
            //    user.Permissions = await _authService.GetPermissionsAsync(user.NationalId);

            //    return Ok(user);
            //}
            //catch (Exception ex)
            //{
            //    return Unauthorized();
            //}
        }
    }
}
