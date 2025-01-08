namespace Pienty.Diariest.Core.Configurations
{
    public class ApplicationConfig
    {
        public const string KeyName = "ApplicationConfig";
        public ServerConfig ServerConfig { get; set; }
        public AIConfig AIConfig { get; set; }
    }

    public class ServerConfig
    {
        public bool TestMode { get; set; }
        public string TestToken { get; set; }
    }

    public class AIConfig
    {
        public string GeminiAPIKey { get; set; }
    }
}