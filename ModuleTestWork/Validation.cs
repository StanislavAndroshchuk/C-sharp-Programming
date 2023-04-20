using System.Globalization;
using System.Text.RegularExpressions;

namespace ModuleTestWork
{
    public static class Validation
    {
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
        
        public static string ValidString(string text)
        {
            string regex = @"^[A-Za-z ]+$";
            if (Regex.IsMatch(text, regex))
            {
                return text;
            }
            else
            {
                throw new Exception("Name is not valid!");
            }
        }

        public static int ValidNoOfPeople(string amount)
        {
            if (int.TryParse(amount, out int result))
            {
                if (result < 1 || result > 10)
                {
                    throw new Exception($"NoOfPeople have to be equal to 1-10");
                }
                else
                {
                    return result;
                }
            }
            else
            {
                throw new Exception($"{result} have to be integer");
            }
        }
        public static Booking.Time ValidTime(string time)
        {
            string pattern = @"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$";
            if (Regex.IsMatch(time, pattern))
            {
                string[] timeParts = time.Split(':');
                int hour = int.Parse(timeParts[0]);
                int minute = int.Parse(timeParts[1]);
                return new Booking.Time(hour, minute);
            }
            else
            {
                throw new Exception("Time format is not valid! Use HH:mm format.");
            }
        }
        public static Booking.Time ValidBothDate(string endtime,string starttime)
        {
            try
            {
                Booking.Time time1 = ValidTime(starttime);
                Booking.Time time2 = ValidTime(endtime);
                if (time1.Hour > time2.Hour || (time1.Hour == time2.Hour && time1.Minute >= time2.Minute))
                {
                    throw new Exception("EndTime must be greater than StartTime.");
                }
                TimeSpan duration = new TimeSpan(time2.Hour - time1.Hour, time2.Minute - time1.Minute, 0);

                if (duration.TotalMinutes < 30 || duration.TotalMinutes > 90)
                {
                    throw new Exception("Booking duration must be between 30 minutes and 1 hour 30 minutes.");
                }

                return time1;
            }
            catch (Exception e)
            {
                throw new Exception(e.InnerException?.Message);
            }
        }
        public static double ValidPrice(string price)
        {
            CultureInfo culture = CultureInfo.InvariantCulture;
            bool isNumeric = double.TryParse(price, NumberStyles.Any, culture, out double parsedPrice);
            if (isNumeric)
            {
                string decimalSeparator = culture.NumberFormat.NumberDecimalSeparator;
                string[] splitPrice = price.Split(decimalSeparator);
                if (splitPrice.Length == 1 || (splitPrice.Length == 2 && splitPrice[1].Length == 2))
                {
                    return parsedPrice;
                }
                else
                {
                    throw new Exception("Price must have exactly 2 decimal places or be a whole number!");
                }
            }
            else
            {
                throw new Exception("Price is not valid!");
            }
        }
        
    }
}