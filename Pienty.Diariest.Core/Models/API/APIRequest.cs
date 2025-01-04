namespace Pienty.Diariest.Core.Models.API
{
    public class APIRequest
    {
        
        public class LoginRequest
        {
            public string Email { get; set; } //E
            public string Password { get; set; } //P
        }
        
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
        
    }
}