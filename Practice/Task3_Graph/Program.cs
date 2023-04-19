using System.Globalization;

namespace Task3_Graph
{
    public static class Program
    {
        private static void Menu()
        {
            string toReadFile = "../../../Flights.json";
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
6 to Print Collection
7 to Exit/Quit
                ");
                Console.WriteLine("Choose option");
                string option = Console.ReadLine()!;
                if (option == "7")
                {
                    menuContinue = false;
                }
                if (option == "1")
                {
                    collection.ReadFromFile(toReadFile);
                    collection.OutputAllFlights();
                    readed = true;
                }
                if (readed || option == "7")
                {
                    if (option == "2")
                    {
                        int count = collection.GetFlightCount();
                        collection.ToWrite(count);
                    }

                    if (option == "3")
                    {
                        Console.Write("Enter the ID of the flight you want to remove: ");
                        string input = Console.ReadLine();
                        if (Validation.ValidInt(input))
                        {
                            int id = int.Parse(input);
                            collection.RemoveFlightById(id);
                        }
                        else
                        {
                            Console.WriteLine("Incorrect input");
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
                    if (option == "5")
                    {
                        bool continueInput = true;
                        while (continueInput)
                        {
                            try
                            {
                                Console.WriteLine("Input:");
                                Console.WriteLine("DepartureCity:");
                                string input = Console.ReadLine()!;
                                City departureCity = (City)fields["DepartureCity"].DynamicInvoke(input)!;
                                Console.WriteLine("ArrivalCity:");
                                input = Console.ReadLine()!;
                                City arrivalCity = (City)fields["ArrivalCity"].DynamicInvoke(input)!;
                                Console.WriteLine("Date:");
                                input = Console.ReadLine()!;
                                var date = Validation.ParseDateOnly(input);
                                //collection.FindCheapestFlight(departureCity, arrivalCity, date);
                                var routes = collection.FindCheapestRoutes(departureCity, arrivalCity, date);
                                Console.WriteLine($"Cheapest routes from {departureCity} to {arrivalCity} on {date:dd-MM-yyyy}:");
                                if (routes.Count == 0)
                                {
                                    Console.WriteLine("No routes found.");
                                }
                                else
                                {
                                    foreach (var route in routes)
                                    {
                                        Console.WriteLine(string.Join("\n", route));
                                        Console.WriteLine();
                                    }
                                }
                                continueInput = false;
                            }
                            catch(Exception er)
                            {
                                Console.WriteLine(er.InnerException?.Message);
                            }
                        }
                        
                    }

                    if (option == "6")
                    {
                        collection.OutputAllFlights();
                    }

                    if (option == "7")
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

