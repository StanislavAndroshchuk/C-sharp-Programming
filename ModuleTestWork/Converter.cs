using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModuleTestWork
{
    public class TimeJsonConverter : JsonConverter<Booking.Time>
    {
        public override Booking.Time Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            string timeString = reader.GetString();
            string[] timeParts = timeString.Split(':');
            int hour = int.Parse(timeParts[0]);
            int minute = int.Parse(timeParts[1]);
            return new Booking.Time(hour, minute);
        }

        public override void Write(Utf8JsonWriter writer, Booking.Time value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}