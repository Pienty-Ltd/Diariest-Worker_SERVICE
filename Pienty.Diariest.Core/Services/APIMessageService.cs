using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Pienty.Diariest.Core.Models.Database;

namespace Pienty.Diariest.Core.Services
{
    
    public class APIMessageService
    {
        private readonly Dictionary<string, Dictionary<string, string>> _messages;

        public APIMessageService()
        {
            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "api_messages.json");
            var json = File.ReadAllText(jsonPath);
            _messages = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
        }

        public string GetMessage(APIMessage key, Language language = Language.Turkish)
        {
            string languageKey = language == Language.Turkish ? "tr" : "en";
            if (_messages.TryGetValue(languageKey, out var languageMessages) &&
                languageMessages.TryGetValue(key.ToString(), out var message))
            {
                return message;
            }

            return "Message not found.";
        }
    }
    
}