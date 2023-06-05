using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Application.Interfaces;
using Domain.Common.Models;
using Domain.Enums;
using Infrastructure.Utility;
using Domain.Entities;
using Domain.Entities;

namespace Infrastructure.Authentication
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;
        private readonly IMapper _mapper;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger, IMapper mapper)
        {
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<AuthToken> GenerateDeveloperToken(string userName, string Id, int accessLevel = 0, string systemAccessLevel = "")
        {

            try
            {
                List<Claim> claims = new List<Claim>()
                {
                new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim (JwtRegisteredClaimNames.Email, userName),
                new Claim (JwtRegisteredClaimNames.Sub, Id),
                    //new Claim (ClaimType.Role, Customer.Role.Name),
                };
                if (accessLevel > 0 && Enum.IsDefined(typeof(AccessLevel), accessLevel))
                {
                    var accessLevelClaim = (AccessLevel)Enum.Parse(typeof(AccessLevel), accessLevel.ToString());
                    claims.Add(new Claim("AccessLevel", accessLevel.ToString()));
                    claims.Add(new Claim("AccessLevelDesc", accessLevelClaim.ToString()));
                }
                if (!string.IsNullOrWhiteSpace(systemAccessLevel))
                {
                    claims.Add(new Claim("SystemAccessLevel", systemAccessLevel));
                }
                JwtSecurityToken token = new TokenBuilder()
                .AddAudience(_configuration["Token:aud"])
                .AddIssuer(_configuration["Token:issuer"])
                .AddExpiry(Convert.ToInt32(_configuration["TokenConstants:ExpiryInMinutes"]))
                .AddKey(Encoding.UTF8.GetBytes(_configuration["TokenConstants:key"]))
                .AddClaims(claims)
                .Build();
                string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                var newToken = new AuthToken()
                {
                    AccessToken = accessToken,
                    ExpiresIn = Convert.ToInt32(_configuration["TokenConstants:ExpiryInMinutes"])
                };
                return await Task.FromResult(newToken);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<AuthToken> GenerateAccessToken(User user)
        {
            try
            {
                _logger.LogInformation($"About to generate access token");
                List<Claim> claims = new List<Claim>() {
                new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim (JwtRegisteredClaimNames.Email, user.Email),
                 new Claim ("userid", user.UserId),
                };
                if (user.Role != null)
                {
                    claims.Add(new Claim("role", user.Role.Name));
                }
                if (user.FirstName != null && user.LastName != null)
                {
                    claims.Add(new Claim("name", user.FirstName + " " + user.LastName));
                }
                JwtSecurityToken token = new TokenBuilder()
               .AddAudience(_configuration["Token:aud"])
               .AddIssuer(_configuration["Token:issuer"])
               .AddExpiry(Convert.ToInt32(_configuration["TokenConstants:ExpiryInMinutes"]))
               .AddKey(Encoding.UTF8.GetBytes(_configuration["TokenConstants:key"]))
               .AddClaims(claims)
               .Build();

                string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                var newToken = new AuthToken()
                {
                    AccessToken = accessToken,
                    ExpiresIn = Convert.ToInt32(_configuration["TokenConstants:ExpiryInMinutes"]),
                };
                return await Task.FromResult(newToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<AuthToken> GenerateRabbitMQToken(User user)
        {
            try
            {
                List<Claim> claims = new List<Claim>()
                {
                new Claim ("email", user.Email),
                new Claim ("Firstname", user.FirstName),
                 new Claim ("Lastname", user.LastName),
                 new Claim ("userid", user.UserId),
                };
                JwtSecurityToken token = new TokenBuilder()
               .AddAudience(_configuration["Token:aud"])
               .AddIssuer(_configuration["Token:issuer"])
               .AddExpiry(Convert.ToInt32(_configuration["TokenConstants:ExpiryInMinutes"]))
               .AddKey(Encoding.UTF8.GetBytes(_configuration["TokenConstants:key"]))
               .AddClaims(claims)
               .Build();

                string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                var newToken = new AuthToken()
                {
                    UserToken = accessToken,
                };
                return await Task.FromResult(newToken);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<AuthToken> GeneratepAccessToken(User user)
        {
            try
            {
                _logger.LogInformation($"About to generate access token");
                List<Claim> claims = new List<Claim>() {
                new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim (JwtRegisteredClaimNames.Email, user.Email),
                 new Claim ("userid", user.UserId),
                };
                if (user.Role != null)
                {
                    claims.Add(new Claim("role", user.Role.Name));
                }
                JwtSecurityToken token = new TokenBuilder()
               .AddAudience(_configuration["Token:aud"])
               .AddIssuer(_configuration["Token:issuer"])
               .AddExpiry(Convert.ToInt32(_configuration["TokenConstants:ExpiryInMinutes"]))
               .AddKey(Encoding.UTF8.GetBytes(_configuration["TokenConstants:key"]))
               .AddClaims(claims)
               .Build();

                string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                var newToken = new AuthToken()
                {
                    AccessToken = accessToken,
                    ExpiresIn = Convert.ToInt32(_configuration["TokenConstants:ExpiryInMinutes"]),
                };
                return await Task.FromResult(newToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<string> GenerateUserToken(User user, List<RolePermission> rolePermissions)
        {
            try
            {
                var mappedRolePermissions = (rolePermissions.Where(p => p.Status == Status.Active));
                var permissions = JsonConvert.SerializeObject(mappedRolePermissions);
                var staffEntity = JsonConvert.SerializeObject(user);
                List<Claim> claims = new()
                {
                    new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim (JwtRegisteredClaimNames.Email, user.Email),
                    new Claim ("UserEntity", staffEntity),
                };
                if (rolePermissions != null && rolePermissions.Count > 0)
                {
                    claims.Add(new Claim("RolePermissions", permissions));
                };
                if (user.Role != null)
                {
                    claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Role.Name));
                }
                JwtSecurityToken token = new TokenBuilder()
               .AddAudience(_configuration["Token:aud"])
               .AddIssuer(_configuration["Token:issuer"])
               .AddExpiry(Convert.ToInt32(_configuration["TokenConstants:ExpiryInMinutes"]))
               .AddKey(Encoding.UTF8.GetBytes(_configuration["TokenConstants:key"]))
               .AddClaims(claims)
               .Build();

                string accessToken = new JwtSecurityTokenHandler().WriteToken(token);


                return await Task.FromResult(accessToken);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
