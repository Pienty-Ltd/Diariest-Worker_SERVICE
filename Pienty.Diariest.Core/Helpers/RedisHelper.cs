namespace Pienty.Diariest.Core.Helpers
{
    public class RedisHelper
    {

        public static string GetKey_User(object userId)
        {
            var key = "User";
            return $"{key}:{userId}";
        }

        public static string GetKey_Limit(string ipAddress)
        {
            var key = "Limit";
            return $"{key}:{ipAddress}";
        }

        public static string GetKey_AuthToken(string authToken)
        {
            var key = "AuthToken";
            return $"{key}:{authToken}";
        }

        public static string GetKey_Ping(string authToken)
        {
            var key = "Ping";
            return $"{key}:{authToken}";
        }

        public static string GetKey_AppPages()
        {
            var key = "AppPages";
            return key;
        }

        #region GenerativeAI

        public static string GetKey_GenerativeAIChat(string chatId)
        {
            var key = "GenerativeAI_Chat";
            return $"{key}:{chatId}";
        }

        #endregion
    }
}