namespace Pienty.Diariest.Core.Models.API
{
    public class APIRequest
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
        
        #region Auth
        
        public class LoginRequest
        {
            public string Email { get; set; } //e
            public string Password { get; set; } //p
        }
        
        #endregion

        #region Agency

        public class GetAgencyRequest
        {
            public long AgencyId { get; set; } //aid
        }

        #endregion
        
        #region TestRequests

        public class GetUserRequest
        {
            public long Id { get; set; }
        }

        public class CreateUserRequest
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Password { get; set; }
        }
        
        #endregion

        #region GenerativeAI

        public class SendMessageToGenerativeAIRequest
        {
            public string Prompt { get; set; }
        }

        #endregion

        #region AppPage

        #endregion
        
    }
}