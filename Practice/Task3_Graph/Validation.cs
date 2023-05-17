using System.Globalization;

namespace Task3_Graph
{
    public static class Validation
    {
        private const int MaxTime = 1;
        private const int MinTime = 7;
        public static DateTime ParseDateOnly(string date)
        {
            if (DateTime.TryParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }
            else
            {
                throw new Exception("Invalid date format.");
            }
        }

        public static City ValidateCity(string eEnumItem) 
        {

            if 
                (Enum.TryParse(eEnumItem, out City validCity))
            {
                
                return validCity;
            }
            else
            {
                throw new Exception("There is no such city");
            }
        }
        public static Country ValidateCountry(string eEnumItem) 
        {

            if 
                (Enum.TryParse(eEnumItem, out Country validCity))
            {
                
                return validCity;
            }
            else
            {
                throw new Exception("There is no such country");
            }
        }
        public static bool ValidInt(string data)
        {
            if (int.TryParse(data, out int _))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static Airlines ValidateAirline(string eEnumItem)
        {
            if (Enum.TryParse(eEnumItem, out Airlines validitem))
            {
                return validitem;
            }
            else
            {
                throw new Exception("There is no such airline");
            }
        }
        
        public static DateTime ValidDate(string date) 
        {
            
            if (DateTime.TryParse(date, out DateTime result))
            {
                return result;
            }
            else
            {
                throw new Exception("Date is not valid!");
            }
        }
        public static DateTime ValidBothDate(string shippedDate, string orderDate)
        {
            if (DateTime.TryParse(shippedDate, out DateTime result) && DateTime.TryParse(orderDate, out DateTime result2))
            {
                if (result > result2)
                {
                    return result;
                }
                else
                {
                    throw new Exception("Arrival date cant be before departure date!");
                }
            }
            else
            {
                throw new Exception("Arrival date is not valid!");
            }
        }


        public static double ValidPrice(string price)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            bool isNumeric = double.TryParse(price, NumberStyles.Any, culture, out double parsedPrice);
            if (isNumeric)
            {
                if (parsedPrice > 0)
                {
                    return parsedPrice;
                }
                else
                {
                    throw new Exception("Price have to be positive amount!");
                }
            }
            else
            {
                throw new Exception("Price is not valid!");
            }
        }

        public static int ValidPositiveInt(string amount)
        {
            if (int.TryParse(amount,out int result))
            {
                if (result >= 0)
                {
                    return result;
                }
                else
                {
                    throw new Exception($"{result} have to be positive!");
                }
            }
            else
            {
                throw new Exception($"{result} have to be integer");
            }
        }
        public static bool IsValidEnumValue<T>(string value) where T : struct, Enum
        {
            if (Enum.TryParse<T>(value, out var result) && Enum.IsDefined(typeof(T), result))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Error: Incorrect enum value");
                return false;
            }
        }

        public static Country IsValidCountry(string value)
        {
            if (Enum.TryParse<Country>(value, out var result) && Enum.IsDefined(typeof(Country), result))
            {
                return result;
            }
            else
            {
                throw new Exception("Incorrect enum value");
            }
        }
        public static City IsValidCity(string value)
        {
            if (Enum.TryParse<City>(value, out var result) && Enum.IsDefined(typeof(City), result))
            {
                return result;
            }
            else
            {
                throw new Exception("Incorrect enum value");
            }
        }
        public static Airlines IsValidAirlines(string value)
        {
            if (Enum.TryParse<Airlines>(value, out var result) && Enum.IsDefined(typeof(Airlines), result))
            {
                return result;
            }
            else
            {
                throw new Exception("Incorrect enum value");
            }
        }

    }
}

