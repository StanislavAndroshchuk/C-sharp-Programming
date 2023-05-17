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
    public class CityEnumConverter : JsonConverter<City>
    {
        public override City Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();
        
            return value switch
            {
                "Paris" => City.Paris,
                "London" => City.London,
                "Kyiv" => City.Kyiv,
                "Berlin" => City.Berlin,
                "Hamburg" => City.Hamburg,
                "Ternopil" => City.Ternopil,
                "Marseille" => City.Marseille,
                "Manchester" => City.Manchester,
                "Washington" => City.Washington,
                "Lviv" => City.Lviv,
                "Ohio" => City.Ohio,
                _ => throw new JsonException($"Value '{value}' is not valid for the 'StatesEnum' enum.")
            };
        }

        public override void Write(Utf8JsonWriter writer, City value, JsonSerializerOptions options)
        {
            string stateStr = value switch
            {
                City.Paris => "Paris",
                City.London => "London" ,
                City.Kyiv => "Kyiv"  ,
                City.Berlin => "Berlin" ,
                City.Hamburg => "Hamburg",
                City.Ternopil =>"Ternopil" ,
                City.Marseille =>"Marseille" ,
                City.Manchester => "Manchester" ,
                City.Washington => "Washington",
                City.Lviv =>"Lviv"  ,
                City.Ohio =>"Ohio"  ,
                _ => throw new JsonException($"Invalid state value: {value}")
            };

            writer.WriteStringValue(stateStr);
        }
    }
    public class CountryEnumConverter : JsonConverter<Country>
    {
        public override Country Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();
        
            return value switch
            {
                "France" => Country.France,
                "UK" => Country.UK,
                "Ukraine" => Country.Ukraine,
                "Germany" => Country.Germany,
                "USA" => Country.USA,
                _ => throw new JsonException($"Value '{value}' is not valid for the 'StatesEnum' enum.")
            };
        }

        public override void Write(Utf8JsonWriter writer, Country value, JsonSerializerOptions options)
        {
            string stateStr = value switch
            {
                Country.France => "France",
                Country.UK => "UK",
                Country.Ukraine => "Ukraine",
                Country.Germany => "Germany",
                Country.USA => "USA",
                _ => throw new JsonException($"Invalid state value: {value}")
            };

            writer.WriteStringValue(stateStr);
        }
    }
    public class AirlinesEnumConverter : JsonConverter<Airlines>
    {
        public override Airlines Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string value = reader.GetString();
        
            return value switch
            {
                "AmericanAir" => Airlines.AmericanAir,
                "TurkishAirlines" => Airlines.TurkishAirlines,
                "DeltaAir" => Airlines.DeltaAir,
                "AirFrance" => Airlines.AirFrance,
                _ => throw new JsonException($"Value '{value}' is not valid for the 'StatesEnum' enum.")
            };
        }

        public override void Write(Utf8JsonWriter writer, Airlines value, JsonSerializerOptions options)
        {
            string stateStr = value switch
            {
                Airlines.AmericanAir => "AmericanAir",
                Airlines.TurkishAirlines => "TurkishAirlines",
                Airlines.DeltaAir => "DeltaAir",
                Airlines.AirFrance => "AirFrance",
                _ => throw new JsonException($"Invalid state value: {value}")
            };

            writer.WriteStringValue(stateStr);
        }
    }


}