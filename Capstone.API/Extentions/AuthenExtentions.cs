using Capstone.Common.DTOs.User;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Capstone.API.Extentions
{
    public static class AuthenExtentions
    {
        public static LoginResponse GetCurrentLoginUserId(this ControllerBase controller)
        {
            if (controller.HttpContext.User.Identity is ClaimsIdentity identity)
            {
                var isAdmin = identity?.FindFirst("IsAdmin")?.Value;
                var userIdString = identity?.FindFirst("UserId")?.Value;
                var userClaim = identity?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(userIdString))
                {
                    return null;
                }

                var isUserIdValid = Guid.TryParse(userIdString, out Guid userId);

                if (!isUserIdValid)
                {
                    return null;
                }

                return new LoginResponse
                {
                    IsAdmin = isAdmin,
                    UserId = userId,
                    UserName = userClaim,
                };
            }
            else
            {
                return null;
            }
        }
    }
}
