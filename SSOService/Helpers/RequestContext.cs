using Microsoft.AspNetCore.Http;
using SSOService.Models.Constants;
using SSOService.Models.DTOs.User;

namespace SSOService.Helpers
{
    public static class RequestContext
    {
        private readonly static HttpContextAccessor httpContextAccessor = new();
        public static GetUserDTO GetCurrentUser
            => httpContextAccessor.HttpContext != null? (GetUserDTO)httpContextAccessor.HttpContext.Items[HttpConstants.CurrentUser]: null;
    }
}
