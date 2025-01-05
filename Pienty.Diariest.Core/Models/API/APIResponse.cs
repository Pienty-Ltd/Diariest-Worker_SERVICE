using Pienty.Diariest.Core.Models.Database;
using Pienty.Diariest.Core.Models.Database.Redis;

namespace Pienty.Diariest.Core.Models.API
{
    public class APIResponse
    {
        public class ErrorResponse
        {
            public string? Message { get; set; }
            public bool Logout { get; set; }
            public DateTime Date { get; set; } = DateTime.Now;

        }

        public class BaseResponse<T>
        {
            public T? Data { get; set; }
            public bool Success { get; set; }
            public string? Message { get; set; }

            public ErrorResponse? Error { get; set; }
        }

        public class LoginResponse
        {
            public AuthenticationToken Authentication { get; set; }   
        }
    
        public class LogoutResponse
        {
            
        }

        public class GetGeneralResponse
        {
            
            public User User { get; set; }
        }
        
        #region TestResponses 
        
        public class CreateUserResponse
        {
            public long Id { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
        
        #endregion
    }
}