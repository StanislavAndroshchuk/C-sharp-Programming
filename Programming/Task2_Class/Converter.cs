using System.Text.Json;
using System.Text.Json.Serialization;

namespace Task2_Class
{
    public class EnumIgnoreCaseConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (Enum.TryParse(typeToConvert, value, true, out object? result))
            {
                return (T)result;
            }
            throw new JsonException($"The value '{value}' could not be converted to the enumeration '{typeToConvert.Name}'.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
    public class StatesEnumConverter : JsonConverter<StatesEnum>
    {
        public override StatesEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();
        
            return value switch
            {
                "Draft" => StatesEnum.Draft,
                "Moderation" => StatesEnum.Moderation,
                "Published" => StatesEnum.Published,
                _ => throw new JsonException($"Value '{value}' is not valid for the 'StatesEnum' enum.")
            };
        }

        public override void Write(Utf8JsonWriter writer, StatesEnum value, JsonSerializerOptions options)
        {
            string stateStr = value switch
            {
                StatesEnum.Draft => "Draft",
                StatesEnum.Moderation => "Moderation",
                StatesEnum.Published => "Published",
                _ => throw new JsonException($"Invalid state value: {value}")
            };

            writer.WriteStringValue(stateStr);
        }
    }

}