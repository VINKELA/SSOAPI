using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SSOService.Models.Constants;
using SSOService.Models.DTOs.User;
using System;

namespace SSOService.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizedRequest : Attribute, IAuthorizationFilter
    {
        private const string UserAuthorized = "not authorized";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var account = (GetUserDTO)context.HttpContext.Items[HttpConstants.CurrentUser];
            if (account == null)
            {
                // not logged in
                context.Result = new JsonResult(new { message = UserAuthorized }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
