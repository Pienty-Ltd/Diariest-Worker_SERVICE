using System;
using Pienty.Diariest.Core.Models.Database;

namespace Pienty.Diariest.Core.Models.Database.Redis
{
    public class AuthenticationToken
    {
        public long UserId { get; set; }
        public UserPermission Permission { get; set; }
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
    }
}