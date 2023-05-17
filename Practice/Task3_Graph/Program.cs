using System.Globalization;

namespace Task3_Graph
{
    public static class Program
    {
        private static void Menu()
        {
            
            string toReadFile = "../../../Flights.json";
            string toWriteFile = "../../../Flights2.json";
            GraphCollection collection = new GraphCollection();
            Dictionary<string, Delegate> fields = ValidDict.ToValidFields();
            bool menuContinue = true;
            bool readed = false;

            while (menuContinue)
            {
                Console.WriteLine(@"
----Menu----
Input 
1 to Read from file
2 to Add
3 to Delete
4 to Edit
5 to Find Cheapest
6 to Find Cheapest to Country
7 to Find Cheapest for Month
8 to Print Collection
9 to Exit/Quit
                ");
                Console.WriteLine("Choose option");
                string option = Console.ReadLine()!;
                if (option == "9")
                {
                    menuContinue = false;
                }
                if (option == "1")
                {
                    Console.Clear();
                    collection.ReadFromFile(toReadFile);
                    collection.OutputAllFlights();
                    readed = true;
                }
                if (readed || option == "9")
                {
                    if (option == "2")
                    {
                        Console.Clear();
                        int count = collection.GetMaxFlightId();
                        collection.ToWrite(count);
                        collection.SaveFlightsToJson(toWriteFile);
                    }

                    if (option == "3")
                    {
                        Console.Clear();
                        Console.Write("Enter the ID of the flight you want to remove: ");
                        string input = Console.ReadLine()!;
                        if (Validation.ValidInt(input))
                        {
                            if (int.Parse(input) > 0)
                            {
                                int id = int.Parse(input);
                                collection.RemoveFlightById(id);
                            }
                            else
                            {
                                Console.WriteLine("Id have to be positive number!");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Incorrect input of id");
                        }
                    }
                    if (option == "4")
                    {
                        Console.Write("Enter flight ID to edit: ");
                        if (int.TryParse(Console.ReadLine(), out int flightId))
                        {
                            Console.Write("Enter attribute to edit (e.g. DepartureCity): ");
                            string attribute = Console.ReadLine();
        
                            Console.Write("Enter new value: ");
                            string newValue = Console.ReadLine();
        
                            try
                            {
                                object parsedValue;
                                if (attribute == "DepartureCity" || attribute == "ArrivalCity")
                                {
                                    parsedValue = Enum.Parse(typeof(City), newValue, true);
                                }
                                else if (attribute == "Airline")
                                {
                                    parsedValue = Enum.Parse(typeof(Airlines), newValue, true);
                                }
                                else if (attribute == "DepartureDatetime" || attribute == "ArrivalDatetime")
                                {
                                    string format = "dd.MM.yyyy HH:mm";
                                    DateTime parsedDateTime;
                                    if (DateTime.TryParseExact(newValue, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDateTime))
                                    {
                                        parsedValue = parsedDateTime;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Error: Invalid datetime format.");
                                        continue;
                                    }
                                }
                                else if (attribute == "ArrivalCountry" || attribute == "DepartureCountry")
                                {
                                    parsedValue = Enum.Parse(typeof(Country), newValue, true);
                                }
                                else if (attribute == "Price")
                                {
                                    parsedValue = double.Parse(newValue);
                                }
                                else
                                {
                                    Console.WriteLine($"Error: Attribute '{attribute}' not found.");
                                    continue;
                                }
            
                                collection.EditById(flightId, attribute, parsedValue);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Error: {e.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Error: Invalid flight ID format.");
                        }
                    }
                    if (option == "5")//07-05-2023 Paris London
                    {
                        bool continueInput = true;
                        while (continueInput)
                        {
                            try
                            {
                                Console.Clear();
                                Console.WriteLine("------Input-------");
                                Console.Write("DepartureCity -> ");
                                string input = Console.ReadLine()!;
                                City departureCity = (City)fields["DepartureCity"].DynamicInvoke(input)!;
                                Console.Write("ArrivalCity -> ");
                                input = Console.ReadLine()!;
                                City arrivalCity = (City)fields["ArrivalCity"].DynamicInvoke(input)!;
                                Console.Write("Date (dd-MM-yyyy) ->");
                                input = Console.ReadLine()!;
                                var date = Validation.ParseDateOnly(input);
                                var cheapestFlights =collection.FindCheapestRoutes(departureCity, arrivalCity, date);
                                Console.Clear();
                                if (cheapestFlights != null)
                                {
                                    Console.WriteLine($"Cheapest flights from {departureCity} to {arrivalCity} on {date.ToShortDateString()}:\n");
                                    foreach (var lCheapestFlight in cheapestFlights)
                                    {
                                        Console.WriteLine("-------------------");
                                        if (lCheapestFlight.Count > 0)
                                        {
                                            foreach (var flight in lCheapestFlight)
                                            {
                                                Console.WriteLine("\n" + flight + "\n") ;
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("There is no flights by request params");
                                        }
                                        Console.WriteLine("-------------------");
                                    }
                                    continueInput = false;
                                }
                                else
                                {
                                    Console.WriteLine("There is no flights by request params!");
                                }
                            }
                            catch (Exception err)
                            {
                                Console.WriteLine(err.InnerException?.Message);
                            }
                        }
                    }
                    if (option == "6")//07-04-2023 Paris Germany
                    {
                        bool continueInput = true;
                        while (continueInput)
                        {
                            try
                            {
                                Console.Clear();
                                Console.WriteLine("------Input-------");
                                Console.Write("DepartureCity -> ");
                                string input = Console.ReadLine()!;
                                City departureCity = (City)fields["DepartureCity"].DynamicInvoke(input)!;
                                Console.Write("ArrivalCountry -> ");
                                input = Console.ReadLine()!;
                                Country arrivalCountry = (Country)fields["ArrivalCountry"].DynamicInvoke(input)!;
                                Console.Write("Date (dd-MM-yyyy) ->");
                                input = Console.ReadLine()!;
                                var date = Validation.ParseDateOnly(input);
                                var dictionaryFlights =collection.FindCheapestFlightsToCountry(departureCity, arrivalCountry, date);
                                Console.WriteLine($"Cheapest flights from {departureCity} to {arrivalCountry} on {date.ToShortDateString()}:\n");
                                foreach (var flights in dictionaryFlights)
                                {
                                    Console.WriteLine($"Flights to {flights.Key}:\n");
                                    foreach (var listFlight in flights.Value)
                                    {
                                        Console.WriteLine("-------------------");
                                        if (listFlight.Count > 0)
                                        {
                                            foreach (var flight in listFlight)
                                            {
                                                Console.WriteLine("\n" + flight + "\n") ;
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("There is no flights by request params");
                                        }
                                    }
                                }

                                continueInput = false;
                            }
                            catch (Exception err)
                            {
                                Console.WriteLine(err.InnerException?.Message);
                            }
                        }
                        
                    }
                    if (option == "7")//Paris London 5 or 7
                    {
                        bool continueInput = true;
                        while (continueInput)
                        {
                            try
                            {
                                Console.Clear();
                                Console.WriteLine("------Input-------");
                                Console.Write("DepartureCity -> ");
                                string input = Console.ReadLine()!;
                                City departureCity = (City)fields["DepartureCity"].DynamicInvoke(input)!;
                                Console.Write("ArrivalCity -> ");
                                input = Console.ReadLine()!;
                                City arrivalCity = (City)fields["ArrivalCity"].DynamicInvoke(input)!;
                                Console.Write("Month (1-12) ->");
                                input = Console.ReadLine()!;
                                var date = Validation.ValidPositiveInt(input);
                                if (date is >= 1 and <= 12)
                                {
                                    string monthName = new DateTime(DateTime.Now.Year, date, 1)
                                        .ToString("MMM", CultureInfo.InvariantCulture);
                                    var x = collection.FindCheapestFlightsInMonth(departureCity, arrivalCity, date);
                                    Console.WriteLine($"Cheapest flights from {departureCity} to {arrivalCity} on {monthName}:\n");
                                    foreach (var y in x)
                                    {
                                        Console.WriteLine($"Flights on {y.Key.Date.ToShortDateString()}:\n");
                                        foreach (var z in y.Value)
                                        {
                                            Console.WriteLine("-------------------");
                                            if (z.Count > 0)
                                            {
                                                foreach (var flight in z)
                                                {
                                                    Console.WriteLine("\n" + flight + "\n") ;
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("There is no flights by request params");
                                            }
                                        }
                                    }
                                    continueInput = false;
                                }
                            }
                            catch (Exception err)
                            {
                                Console.WriteLine(err.InnerException?.Message);
                            }
                        }
                        
                    }
                    if (option == "8")
                    {
                        collection.OutputAllFlights();
                    }
                    if (option == "9")
                    {
                        menuContinue = false;
                    }
                }
                
                else
                {
                    Console.WriteLine("Cant work with collection before reading from file");
                }
                
            }
        }

        public static void Main(string[] args)
        {
            Menu();
            Console.WriteLine();
        }
    }
}

