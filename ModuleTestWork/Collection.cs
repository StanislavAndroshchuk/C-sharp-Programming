using System.Text.Json;

namespace ModuleTestWork
{
    public class CollectionBooking
    {
        public string? SomeFile { get; set; }
        public List<Booking> Collection;
        public CollectionBooking()
        {
            Collection = new List<Booking>();
        }
        public void Append(Booking element)
        {
            Collection.Add(element!);
        }
        public Dictionary<string, Delegate> GetValidFields()
        {
            Booking item = new Booking();
            Dictionary<string, Delegate> fieldValid = item.ToValidFields();
            return fieldValid;
        }
        public override string ToString()
        {
            string toPrint = "";
            foreach (var order in Collection)
            {
                toPrint += order + "\n";
            }
            return toPrint;
        }
        public void ReadFromFile(string fileName)
        {
            string jsonString = File.ReadAllText(fileName);
            List<JsonElement>? data = JsonSerializer.Deserialize<List<JsonElement>>(jsonString);
            Dictionary<string, Delegate> fieldValid = GetValidFields();
            foreach (JsonElement element in data!)
            {
                bool passed = true;
                
                for (int i = 0; i < fieldValid.Count; i++)
                {   
                    
                    try
                    {
                        string tempKey = fieldValid.Keys.ElementAt(i); // string?
                        
                        if (tempKey == "EndTime")
                        {
                            fieldValid[tempKey].DynamicInvoke(element.GetProperty(tempKey).ToString(),
                                element.GetProperty("StartTime").ToString() );
                        }
                        else
                        {
                            fieldValid[tempKey].DynamicInvoke(element.GetProperty(tempKey).ToString()); //
                        }
                        
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.InnerException?.Message);
                        passed = false;
                    }
                }

                
                
                if (passed)
                {
                    Booking tempOrder = JsonSerializer.Deserialize<Booking>(element)!;
                    Collection.Add(tempOrder);
                }
                else
                {
                    Console.WriteLine("Previous Booking had problems during validation.");
                }
            }
            
        }
        public static List<int> FindMostBookedHours(CollectionBooking bookings)
        {
            Dictionary<int, int> hourCounts = new Dictionary<int, int>();

            foreach (var booking in bookings.Collection)
            {
                int startHour = booking.StartTime.Hour;
                int endHour = booking.EndTime.Hour;

                for (int hour = startHour; hour < endHour; hour++)
                {
                    if (hourCounts.ContainsKey(hour))
                    {
                        hourCounts[hour]++;
                    }
                    else
                    {
                        hourCounts.Add(hour, 1);
                    }
                }
            }

            int maxBookings = hourCounts.Values.Max();

            List<int> mostBookedHours = hourCounts
                .Where(pair => pair.Value == maxBookings)
                .Select(pair => pair.Key)
                .ToList();

            return mostBookedHours;
        }
        public static bool CanAddBooking(List<Booking> bookings, Booking newBooking)
        {
            int maxSeats = 15;
            int minDuration = 30;
            int maxDuration = 90;
            
            /*TimeSpan duration = newBooking.EndTime - newBooking.StartTime;*/
            int durationhour = newBooking.EndTime.Hour - newBooking.StartTime.Hour;
            int durationminutes =newBooking.EndTime.Minute - newBooking.StartTime.Minute;
            int duration = durationminutes + (durationhour * 60);
            if (duration < minDuration || duration > maxDuration)
            {
                return false;
            }
            
            foreach (var booking in bookings)
            {
                if (newBooking.StartTime.Hour < booking.EndTime.Hour && newBooking.EndTime.Hour > booking.StartTime.Hour && newBooking.StartTime.Minute < booking.EndTime.Minute && newBooking.EndTime.Minute > booking.StartTime.Minute)
                {
                    int totalSeats = newBooking.NoOfPeople + booking.NoOfPeople;
                    if (totalSeats > maxSeats)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
    }    
}
