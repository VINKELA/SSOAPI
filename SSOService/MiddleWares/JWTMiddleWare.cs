using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SSOService.Constants;
using SSOService.Models.Constants;
using SSOService.Services.Repositories.Relational.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSOService.MiddleWares
{


    public class JWTMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JWTMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context, IUserRepository userService)
        {
            var token = context.Request.Headers[HttpConstants.Authorization].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await AttachAccountToContext(context, token, userService);

            await _next(context);
        }

        private async Task AttachAccountToContext(HttpContext context, string token, IUserRepository _userService)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration[JWTConstants.Key]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidIssuer = _configuration[JWTConstants.Issuer],

                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var email = jwtToken.Claims.First(x => x.Type == AppClaimTypes.Email).Value;

                // attach account to context on successful jwt validation
                context.Items[HttpConstants.CurrentUser] = await _userService.GetUserByEmailOrUsername(email);
            }
            catch (Exception e)
            {
                // do nothing if jwt validation fails
                Console.WriteLine(e);
                // account is not attached to context so request won't have access to secure routes
            }
        }
    }


}

