using System.Security.Claims;
using Pienty.Diariest.Core.Models.Database;

namespace Pienty.Diariest.API.Authentication
{
    public class AuthenticatedUserModel
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public UserPermission Role { get; set; }
    }
    
    public class AuthenticatedUserHandler
    {
        
        public static AuthenticatedUserModel GetAuthenticatedUser(HttpContext httpContext)
        {
            if (httpContext.User.Identity is ClaimsIdentity identity)
            {
                var userId = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var email = identity.FindFirst(ClaimTypes.Name)?.Value;
                var roleString = identity.FindFirst(ClaimTypes.Role)?.Value;

                if (long.TryParse(userId, out var id) && Enum.TryParse<UserPermission>(roleString, out var role))
                {
                    return new AuthenticatedUserModel
                    {
                        Id = id,
                        Email = email,
                        Role = role
                    };
                }
            }
            return null;
        }
        
    }
}