using System.Reflection;
using System.Text.Json.Serialization;
namespace ModuleTestWork
{
    public class Booking
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public int NoOfPeople { get; set; }
        [JsonConverter(typeof(TimeJsonConverter))]
        public Time StartTime { get; set; }
        [JsonConverter(typeof(TimeJsonConverter))]
        public Time EndTime { get; set; }
        
        public double Price { get; set; }
        
        public Booking()
        {
        }
        
        [JsonConstructor]
        public Booking(int id, string name, int noOfPeople, Time startTime, Time endTime, double price)
        {
            Id = id;
            Name = name;
            NoOfPeople = noOfPeople;
            StartTime = startTime;
            EndTime = endTime;
            Price = price;
        }
        public void ToWrite(int count)
        {
            Dictionary<string, Delegate> fieldValid;
            fieldValid = ToValidFields();
            this.GetType().GetProperty("Id")!.SetValue(this, count+1);
            foreach (var element in fieldValid)
            {
                while (true) 
                {
                    Console.Write($"[{element.Key}]: ");
                    if (element.Key == "ID") 
                    {
                        Console.WriteLine($"{count+1}"); 
                    }
                    
                    try
                    {
                        
                        if (element.Key == "EndTime")
                        {
                            var value = fieldValid[element.Key].DynamicInvoke(Console.ReadLine(),this.StartTime.ToString());
                            this.GetType().GetProperty(element.Key)!.SetValue(this, value);
                            break;
                        }
                        else
                        {
                            var value = fieldValid[element.Key].DynamicInvoke(Console.ReadLine());
                            this.GetType().GetProperty(element.Key)!.SetValue(this, value);
                            break;
                        }
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine($"Error: {err.InnerException?.Message}");
                    }
                }
            }
            
        }

        
        public override string ToString()
        {
            string toReturn = "";
            foreach (PropertyInfo x in this.GetType().GetProperties())
            {
                toReturn += x.Name + " - " + x.GetValue(this) + "\n"; 
            }
            return toReturn;
        }
        public class Time
        {
            public int Hour;
            public int Minute;
            public Time(int hour, int minute)
            {
                Hour = hour;
                Minute = minute;
            }

            public override string ToString()
            {
                return $"{Hour.ToString("D2")}:{Minute.ToString("D2")}";
            }
        }
        public Dictionary<string, Delegate> ToValidFields()
        {
            Dictionary<string, Delegate> fieldValid = new Dictionary<string, Delegate>
            {
                {"Id", Validation.ValidPositiveInt},
                {"Name", Validation.ValidString},
                {"NoOfPeople", Validation.ValidNoOfPeople},
                {"StartTime", Validation.ValidTime},
                {"EndTime", Validation.ValidBothDate},
                {"Price", Validation.ValidPrice}
            };
            return fieldValid;

        }
    }
}