using Pienty.Diariest.Core.Models.Database;
using Pienty.Diariest.Core.Models.Database.Redis;

namespace Pienty.Diariest.Core.Models.API
{
    public class APIResponse
    {
        
        /*
         *
         *
         * [JsonProperty(PropertyName = "TI")]
           [JsonPropertyName("TI")]
           [Key("TI")]
           public string? Message { get; set; }
         *
         * 
         */

        #region Base

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

        #endregion

        #region AuthController

        public class LoginResponse
        {
            public AuthenticationToken Authentication { get; set; }   
        }

        #endregion

        #region GeneralController

        public class GetGeneralResponse
        {
            
            public User User { get; set; }
        }

        #endregion

        #region AgencyController

        public class GetAgencyResponse
        {
            public Agency Agency { get; set; }
        }

        #endregion
        
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