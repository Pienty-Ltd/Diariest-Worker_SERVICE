using System;
using Newtonsoft.Json;
using Pienty.Diariest.Core.Models.Database;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Pienty.Diariest.Core.Converters
{
    
    public class AdStatusConverter : JsonConverter<AdStatus>
    {
        public override void WriteJson(JsonWriter writer, AdStatus value, JsonSerializer serializer)
        {
            string status = value switch
            {
                AdStatus.Active => "ACTIVE",
                AdStatus.Paused => "PAUSED",
                AdStatus.Deleted => "DELETED",
                AdStatus.Archived => "ARCHIVED",
                AdStatus.AdsetPaused => "ADSET_PAUSED",
                AdStatus.CampaignPaused => "CAMPAIGN_PAUSED",
                _ => throw new ArgumentOutOfRangeException()
            };
            writer.WriteValue(status);
        }

        public override AdStatus ReadJson(JsonReader reader, Type objectType, AdStatus existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string status = reader.Value.ToString();
            return status switch
            {
                "ACTIVE" => AdStatus.Active,
                "PAUSED" => AdStatus.Paused,
                "DELETED" => AdStatus.Deleted,
                "ARCHIVED" => AdStatus.Archived,
                "ADSET_PAUSED" => AdStatus.AdsetPaused,
                "CAMPAIGN_PAUSED" => AdStatus.CampaignPaused,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
    
    public class StringToDoubleConverter : JsonConverter<double>
    {
        public override void WriteJson(JsonWriter writer, double value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        public override double ReadJson(JsonReader reader, Type objectType, double existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null || string.IsNullOrWhiteSpace(reader.Value.ToString()))
            {
                return 0.0;
            }

            double.TryParse(reader.Value.ToString(), out double result);
            return result;
        }
    }
    
}