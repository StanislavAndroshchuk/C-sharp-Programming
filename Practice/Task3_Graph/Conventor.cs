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
    public class FlightListEqualityComparer : IEqualityComparer<List<Flight>>
    {
        public bool Equals(List<Flight> x, List<Flight> y)
        {
            if (ReferenceEquals(x, y)) return true;

            if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                return false;

            return x.SequenceEqual(y);
        }

        public int GetHashCode(List<Flight> flightList)
        {
            if (ReferenceEquals(flightList, null)) return 0;

            unchecked // Overflow is fine, just wrap
            {
                int hash = 27;
                foreach (var flight in flightList)
                {
                    hash = (hash * 31) + flight.GetHashCode();
                }
                return hash;
            }
        }
    }


}