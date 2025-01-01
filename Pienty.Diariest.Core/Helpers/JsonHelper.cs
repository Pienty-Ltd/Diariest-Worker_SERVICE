using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Pienty.CRM.Core.Helpers
{
    public class JsonHelper
    {
        public static string Serialize<T>(T model)
        {
            return JsonConvert.SerializeObject(model);
        }

        public static T Deserialize<T>(string json)
        {
            if (json == null)
            {
                return default(T);
            }

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}