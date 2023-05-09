using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using LightsOut.Models;

namespace LightsOut
{
    internal class TimeRangeJsonConverter : JsonConverter<TimeRange>
    {
        public override TimeRange Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        )
        {
            return TimeRange.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, TimeRange value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}