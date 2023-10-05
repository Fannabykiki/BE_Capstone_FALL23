﻿using Capstone.API.Extentions;
using Capstone.Common.DTOs.User;
using Capstone.Common.Jwt;
using Capstone.Service.LoggerService;
using Capstone.Service.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GoogleAuthentication.Services;

using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using Newtonsoft.Json;

namespace Capstone.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ILoggerManager _logger;
        private readonly IUserService _usersService;
        private readonly ClaimsIdentity? _identity;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationController(IUserService usersService, ILoggerManager logger, IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _config = config;
            _usersService = usersService;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            var identity = httpContextAccessor.HttpContext?.User?.Identity;
            if (identity == null)
            {
                _identity = null;
            }
            else
            {
                _identity = identity as ClaimsIdentity;
            }
        }

        [HttpPost("token")]
        public async Task<ActionResult<LoginResponse>> LoginInternal(LoginRequest request)
        {
            var user = await _usersService.LoginUser(request.UserName, request.Password);
            if (user == null)
            {
                return NotFound();
            }
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
                new Claim("IsAdmin",user.IsAdmin.ToString()),
                new Claim("UserId",user.UserId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserName.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConstant.Key));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expired = DateTime.UtcNow.AddMinutes(JwtConstant.ExpiredTime);

            var token = new JwtSecurityToken(JwtConstant.Issuer,
                JwtConstant.Audience, claims,
                expires: expired, signingCredentials: signIn);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse
            {
                IsAdmin = user.IsAdmin,
                UserId = user.UserId,
                UserName = user.UserName,
                Token = tokenString,
                IsFirstTime = user.IsFirstTime
            };
        }

        [HttpGet("external-login")]
        public IActionResult Login()
        {

            var clientId = _config["Authentication:Google:ClientId"];

            var redirectUrl = _config["Authentication:Google:CallBackUrl"];

            var authUrl = GoogleAuth.GetAuthUrl(clientId, redirectUrl);

            return Redirect(authUrl);

        }

        [HttpGet("external-login/token")]
        public async Task<ActionResult<LoginResponse>> LoginExternalCallback(string? code)
        {
            GoogleProfile googleUser = new GoogleProfile();
            try
            {
                var ClientSecret = _config["Authentication:Google:ClientSecret"];
                var ClientID = _config["Authentication:Google:ClientId"];
                var url = _config["Authentication:Google:CallBackUrl"];
                var ggToken = await GoogleAuth.GetAuthAccessToken(code, ClientID, ClientSecret, url);
                var userProfile = await GoogleAuth.GetProfileResponseAsync(ggToken.AccessToken.ToString());
                googleUser = JsonConvert.DeserializeObject<GoogleProfile>(userProfile);

            }
            catch (Exception ex)
            {

            }

            var user = await _usersService.GetUserByEmailAsync(googleUser.Email);
            if (user == null)
            {
                return NotFound();
            }
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
                new Claim("IsAdmin",user.IsAdmin.ToString()),
                new Claim("UserId",user.UserId.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserName.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConstant.Key));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expired = DateTime.UtcNow.AddMinutes(JwtConstant.ExpiredTime);

            var token = new JwtSecurityToken(JwtConstant.Issuer,
                JwtConstant.Audience, claims,
                expires: expired, signingCredentials: signIn);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse
            {
                IsAdmin = user.IsAdmin,
                UserId = user.UserId,
                UserName = user.UserName,
                Token = tokenString,
                IsFirstTime = user.IsFirstTime
            };
        }
    }
}
