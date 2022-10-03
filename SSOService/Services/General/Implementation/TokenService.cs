using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SSOService.Constants;
using SSOService.Models.Constants;
using SSOService.Models.DbContexts;
using SSOService.Models.Domains;
using SSOService.Models.DTOs.Application;
using SSOService.Models.DTOs.Auth;
using SSOService.Models.DTOs.User;
using SSOService.Services.General.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SSOService.Services.General.Implementation
{
    public class TokenService : IToken
    {
        private const double EXPIRY_DURATION_MINUTES = 30;
        public IConfiguration Configuration { get; }
        private readonly SSODbContext _db;
        public TokenService(IConfiguration configuration, SSODbContext db)
        {
            Configuration = configuration;
            _db = db;
        }

        public async Task<TokenDTO> BuildToken(GetUserDTO user)
        {
            var code = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(AppClaimTypes.FirstName, user.FirstName),
                new Claim(AppClaimTypes.SurName, user.LastName),
                new Claim(AppClaimTypes.UserName, user.UserName),
                new Claim(AppClaimTypes.Email, user.Email),
                new Claim(AppClaimTypes.Phone, user.PhoneNumber)
             };
            claims.AddRange(user.UserRoles
                .Select(x => new Claim(ClaimTypes.Role, x.RoleName,
                ClaimValueTypes.String, x.Code)).ToList());
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration[JWTConstants.Key]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(Configuration[JWTConstants.Issuer], null, claims,
                expires: DateTime.Now.AddMinutes(EXPIRY_DURATION_MINUTES), signingCredentials: credentials);
            await SaveRefreshToken(code, user.Id);
            var tokenDTO = new TokenDTO
            {
                RefreshToken = code,
                Token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor)
            };
            return tokenDTO;
        }
        public async Task<TokenDTO> BuildToken(GetApplicationDTO app)
        {
            var code = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(AppClaimTypes.AppName, app.Name),
             };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration[JWTConstants.Key]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(Configuration[JWTConstants.Issuer], null, claims,
                expires: DateTime.Now.AddMinutes(EXPIRY_DURATION_MINUTES), signingCredentials: credentials);
            await SaveRefreshToken(code, app.Id);
            var tokenDTO = new TokenDTO
            {
                RefreshToken = code,
                Token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor)
            };
            return tokenDTO;
        }

        public bool IsTokenValid(string token)
        {
            var mySecret = Encoding.UTF8.GetBytes(Configuration[JWTConstants.Key]);
            var mySecurityKey = new SymmetricSecurityKey(mySecret);
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = Configuration[JWTConstants.Issuer],
                    IssuerSigningKey = mySecurityKey,
                }, out SecurityToken validatedToken);
            }
            catch
            {
                return false;
            }
            return true;
        }
        private async Task SaveRefreshToken(string token, long userId)
        {
            var newToken = new RefreshToken
            {
                UserId = userId,
                Token = token
            };
            _db.RefreshTokens.Add(newToken);
            await _db.SaveChangesAsync();
        }


    }

}
