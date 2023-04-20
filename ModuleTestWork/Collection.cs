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
        /*public static bool CanAddBooking(List<Booking> bookings, Booking newBooking)
        {
            int maxSeats = 15;
            TimeSpan minDuration = TimeSpan.FromMinutes(30);
            TimeSpan maxDuration = TimeSpan.FromMinutes(90);

            // Check duration constraints
            TimeSpan duration = newBooking.EndTime - newBooking.StartTime;
            if (duration < minDuration || duration > maxDuration)
            {
                return false;
            }

            // Check overlapping bookings
            foreach (var booking in bookings)
            {
                if (newBooking.StartTime < booking.EndTime && newBooking.EndTime > booking.StartTime)
                {
                    int totalSeats = newBooking.NoOfPeople + booking.NoOfPeople;
                    if (totalSeats > maxSeats)
                    {
                        return false;
                    }
                }
            }

            return true;
        }*/
        
    }    
}
