using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using SSOService.Models.Constants;
using SSOService.Models.DTOs.User;
using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace SSOService.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method), AllowAnonymous]
    public class AuthorizedRequest : Attribute, IAuthorizationFilter
    {
        private const string UserAuthorized = "not authorized";
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var endpoint = context.HttpContext.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                return;
            }
            var account = (GetUserDTO)context.HttpContext.Items[HttpConstants.CurrentUser];
            if (account == null)
            {
                // not logged in
                context.Result = new JsonResult(new { message = UserAuthorized }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
