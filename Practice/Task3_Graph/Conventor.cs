using System.Text.Json;
using System.Text.Json.Serialization;

namespace Task3_Graph
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
}