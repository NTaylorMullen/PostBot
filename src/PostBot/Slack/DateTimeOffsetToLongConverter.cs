using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PostBot.Slack
{
    public class DateTimeOffsetToLongConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DateTimeOffset).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(((DateTimeOffset)value).ToUnixTimeSeconds().ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var textReader = reader as JsonTextReader;
            var unixTime = long.Parse(textReader.Value.ToString());

            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTime);

            return dateTimeOffset;
        }
    }
}
