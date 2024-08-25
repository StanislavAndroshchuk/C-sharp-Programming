using System.Reflection;
using System.Text.Json;

namespace Task3_Graph
{
    public class GraphCollection
    {
        public Dictionary<City, Dictionary<Country, List<Flight>>> Flights;
        private const int MinTimeBetweenFlightsHours = 1;
        private const int MaxTimeBetweenFlightsHours = 7;

        public GraphCollection()
        {
            Flights = new Dictionary<City, Dictionary<Country, List<Flight>>>();
        }

        /*public void AddFlight(Flight flight)
        {
            if (!Flights.ContainsKey(flight.DepartureCity))
            {
                Flights[flight.DepartureCity] = new Dictionary<Country, List<Flight>>();
            }

            if (!Flights[flight.DepartureCity].ContainsKey(flight.ArrivalCountry))
            {
                Flights[flight.DepartureCity][flight.ArrivalCountry] = new List<Flight>();
            }

            Flights[flight.DepartureCity][flight.ArrivalCountry].Add(flight);
        }*/
        public void ReadFromFile(string path)
        {
            string jsonString = File.ReadAllText(path);

            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new EnumIgnoreCaseConverter<City>(),
                    new EnumIgnoreCaseConverter<Airlines>(),
                    new EnumIgnoreCaseConverter<Country>()
                }
            };
            List<JsonElement>? data = JsonSerializer.Deserialize<List<JsonElement>>(jsonString, options);
            Dictionary<string, Delegate> fieldValid = ValidDict.ToValidFields();
            List<Flight> flights = new List<Flight>();
            if (data != null)
            {
                foreach (JsonElement element in data)
                {
                    bool passed = true;
                    for (int i = 0; i < fieldValid.Count; i++)
                    {
                        try
                        {
                            string tempKey = fieldValid.Keys.ElementAt(i);

                            if (tempKey == "ArrivalDatetime")
                            {
                                fieldValid[tempKey].DynamicInvoke(element.GetProperty(tempKey).ToString(),
                                    element.GetProperty("DepartureDatetime").ToString());
                            }
                            
                            else
                            {
                                fieldValid[tempKey].DynamicInvoke(element.GetProperty(tempKey).ToString());
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.InnerException?.Message);
                            passed = false;
                        }
                    }

                    if (passed)
                    {
                        Flight flight = JsonSerializer.Deserialize<Flight>(element.GetRawText(), options)!;
                        flights.Add(flight);
                        AddFlight(flight);
                    }
                    else
                    {
                        Console.WriteLine("Previous flights had problems during validation.");
                    }
                }
            }
        }
        public void OutputAllFlights()
        {
            foreach (var departurePair in Flights)
            {
                City departureCity = departurePair.Key;
                Dictionary<Country, List<Flight>> countryToFlights = departurePair.Value;

                Console.WriteLine("Departure City: {0}", departureCity);
                Console.WriteLine("Available flights: ");

                foreach (var countryFlightPair in countryToFlights)
                {
                    Country arrivalCountry = countryFlightPair.Key;
                    List<Flight> flights = countryFlightPair.Value;

                    Console.WriteLine("Arrival Country: {0}", arrivalCountry);

                    foreach (var flight in flights)
                    {
                        Console.WriteLine(flight);
                        Console.WriteLine();
                    }
                }
            }
        }
        public Dictionary<City, List<List<Flight>>> FindCheapestFlightsToCountry(City departureCity, Country destinationCountry, DateTime targetDate)
        {
            Dictionary<City, List<List<Flight>>> cheapestFlightsToCountry = new Dictionary<City, List<List<Flight>>>();
            Dictionary<Country, List<City>> CountryCitys = CountryCity();
            foreach (City arrivalCity in CountryCitys[destinationCountry])
            {
                List<List<Flight>> cheapestFlights = FindCheapestRoutes(departureCity, arrivalCity, targetDate);
                if (cheapestFlights != null)
                {
                    if (cheapestFlights.Count > 0)
                    {
                        cheapestFlightsToCountry[arrivalCity] = cheapestFlights;
                    }
                }
            }

            return cheapestFlightsToCountry;
        }
        public Dictionary<Country, List<City>> CountryCity()
        {
            Dictionary<Country, List<City>> CountryCitys = new Dictionary<Country, List<City>>();
            List<City> Citys = new List<City>();
            Citys.Add(City.Manchester);
            Citys.Add(City.London);
            CountryCitys.Add(Country.UK,Citys);
            Citys = new List<City>();
            Citys.Add(City.Paris);
            Citys.Add(City.Marseille);
            CountryCitys.Add(Country.France,Citys);
            Citys = new List<City>();
            Citys.Add(City.Hamburg);
            Citys.Add(City.Berlin);
            CountryCitys.Add(Country.Germany,Citys);
            Citys = new List<City>();
            Citys.Add(City.Kyiv);
            Citys.Add(City.Ternopil);
            Citys.Add(City.Lviv);
            CountryCitys.Add(Country.Ukraine,Citys);
            Citys = new List<City>();
            Citys.Add(City.Ohio);
            Citys.Add(City.Washington);
            CountryCitys.Add(Country.USA,Citys);
            return CountryCitys;
        }
        public Country? GetCountryByCity(object? city)
        {
            var countryCityDict = CountryCity();

            foreach (var pair in countryCityDict)
            {
                if (pair.Value.Contains((City)city))
                {
                    return pair.Key;
                }
            }

            return null;
        }
        public Dictionary<DateTime, List<List<Flight>>> FindCheapestFlightsInMonth(City departureCity, City arrivalCity, int month)
        {
            var result = new Dictionary<DateTime, List<List<Flight>>>();

            for (int day = 1; day <= DateTime.DaysInMonth(DateTime.Now.Year, month); day++)
            {
                var date = new DateTime(DateTime.Now.Year, month, day);
                var cheapestFlightsOnDate = FindCheapestRoutes(departureCity, arrivalCity, date);
                if (cheapestFlightsOnDate != null)
                {
                    if (cheapestFlightsOnDate.Count > 0)
                    {
                        result.Add(date, cheapestFlightsOnDate);
                    }
                }
            }

            return result;
        }
        public List<List<Flight>>? FindCheapestRoutes(City departureCity, City arrivalCity, DateTime targetDate)
        {
            List<List<Flight>> allPaths = new List<List<Flight>>();
            List<Flight> currentPath = new List<Flight>();
            HashSet<City> visitedCities = new HashSet<City>();
            try
            {
                FindRoutesDFS(departureCity, arrivalCity, targetDate, visitedCities, currentPath, allPaths);
                return FindCheapestRoutes(allPaths);
            }
            catch (Exception e)
            {
                return null;
            }
        }
        private void FindRoutesDFS(City currentCity, City destinationCity, DateTime targetDate, HashSet<City> visitedCities, List<Flight> currentPath, List<List<Flight>> allPaths)
        {
            visitedCities.Add(currentCity);

            if (currentCity.Equals(destinationCity))
            {
                allPaths.Add(new List<Flight>(currentPath));
            }
            else
            {
                if (Flights.ContainsKey(currentCity))
                {
                    foreach (var flightPair in Flights[currentCity])
                    {
                        foreach (Flight flight in flightPair.Value)
                        {
                            if (!visitedCities.Any(city => city.Equals(flight.ArrivalCity)) && EqualDate(flight, targetDate) && FlightValidTime(currentPath, flight))
                            {
                                currentPath.Add(flight);
                                FindRoutesDFS(flight.ArrivalCity, destinationCity, targetDate, visitedCities, currentPath, allPaths);
                                currentPath.RemoveAt(currentPath.Count - 1);
                            }
                        }
                    }
                    
                }
            }

            visitedCities.Remove(currentCity);
        }
        private bool EqualDate(Flight flight, DateTime targetDate)
        {
        
            return flight.DepartureDatetime.Date == targetDate.Date;
        }
        private bool FlightValidTime(List<Flight> currentPath, Flight nextFlight)
        {
            if (currentPath.Count == 0)
            {
                return true;
            }

            Flight lastFlight = currentPath[currentPath.Count - 1];
            TimeSpan layoverTime = nextFlight.DepartureDatetime - lastFlight.ArrivalDatetime;
        
            return layoverTime.TotalHours >= MinTimeBetweenFlightsHours && layoverTime.TotalHours <= MaxTimeBetweenFlightsHours;
        }
        private List<List<Flight>> FindCheapestRoutes(List<List<Flight>> allPaths)
        {
            List<List<Flight>> cheapestPaths = new List<List<Flight>>();
            double cheapestPrice = double.MaxValue;

            foreach (List<Flight> path in allPaths)
            {
                double totalPrice = 0;

                foreach (Flight flight in path)
                {
                    totalPrice += flight.Price;
                }

                if (totalPrice < cheapestPrice)
                {
                    cheapestPrice = totalPrice;
                    cheapestPaths.Clear();
                    cheapestPaths.Add(path);
                }
                else if (totalPrice == cheapestPrice)
                {
                    cheapestPaths.Add(path);
                }
            }

            return cheapestPaths;
        }
        public void RemoveFlightById(int id)
        {
            KeyValuePair<City, KeyValuePair<Country, List<Flight>>>? cityCountryFlightsPair = null;
            Flight flightToRemove = null;

            foreach (var cityCountryPair in Flights)
            {
                foreach (var countryFlightsPair in cityCountryPair.Value)
                {
                    flightToRemove = countryFlightsPair.Value.FirstOrDefault(flight => flight.Id == id);
                    if (flightToRemove != null)
                    {
                        cityCountryFlightsPair = new KeyValuePair<City, KeyValuePair<Country, List<Flight>>>(cityCountryPair.Key, countryFlightsPair);
                        break;
                    }
                }
                if (flightToRemove != null) 
                {
                    break;
                }
            }

            if (cityCountryFlightsPair.HasValue && flightToRemove != null)
            {
                Flights[cityCountryFlightsPair.Value.Key][cityCountryFlightsPair.Value.Value.Key].Remove(flightToRemove);
                Console.WriteLine($"Flight with ID {id} was successfully removed.");
            }
            else
            {
                Console.WriteLine($"Flight with ID {id} not found.");
            }
        }
        private Flight? FindById(int id)
        {
            foreach (var cityCountryPair in Flights)
            {
                foreach (var countryFlightsPair in cityCountryPair.Value)
                {
                    foreach (var flight in countryFlightsPair.Value)
                    {
                        Console.WriteLine($"Current flight ID: {flight.Id}");
                        if (flight.Id == id)
                        {
                            return flight;
                        }
                    }
                }
            }

            return null;
        }
        public void EditById(int id, string attribute, object value)
        {
            Flight? flightToEdit = FindById(id);
    
            if (flightToEdit != null)
            {
                PropertyInfo? property = flightToEdit.GetType().GetProperty(attribute);
        
                if (property != null)
                {
                    object convertedValue = Convert.ChangeType(value, property.PropertyType);
                    property.SetValue(flightToEdit, convertedValue);
                    Console.WriteLine($"Flight with ID {id} has been successfully edited.");
                }
                else
                {
                    Console.WriteLine($"Error: Attribute '{attribute}' not found.");
                }
            }
            else
            {
                Console.WriteLine($"Error: Flight with ID {id} not found.");
            }
        }
        private void AddFlight(Flight flight)
        {
            if (flight.DepartureCity == flight.ArrivalCity)
            {
                Console.WriteLine("Error: Departure and arrival cities cannot be the same.");
                return;
            }
            /*if (flight.DepartureCountry == flight.ArrivalCountry)
            {
                Console.WriteLine("Error: Departure and arrival country's cannot be the same.");
                return;
            }*/

            City departureCity = flight.DepartureCity;
            Country arrivalCountry = flight.ArrivalCountry;

            if (Flights.ContainsKey(departureCity))
            {
                if (Flights[departureCity].ContainsKey(arrivalCountry))
                {
                    Flights[departureCity][arrivalCountry].Add(flight);
                }
                else
                {
                    Flights[departureCity][arrivalCountry] = new List<Flight> { flight };
                }
            }
            else
            {
                Flights[departureCity] = new Dictionary<Country, List<Flight>> 
                {
                    { arrivalCountry, new List<Flight> { flight } }
                };
            }
        }
        public void ToWrite(int count)
        {
            Flight flight = new Flight();
            Dictionary<string, Delegate> fieldValid = ValidDict.ToValidFields();
            flight.GetType().GetProperty("Id")!.SetValue(flight, count+1);
            foreach (var element in fieldValid)
            {
                while (true)
                {
                    if (element.Key != "DepartureCountry" && element.Key != "ArrivalCountry")
                    {
                        Console.Write($"[{element.Key}]: ");
                        try
                        {
                            if (element.Key == "ArrivalDatetime")
                            {
                                var value = fieldValid[element.Key].DynamicInvoke(Console.ReadLine(), flight.DepartureDatetime.ToString());
                                flight.GetType().GetProperty(element.Key)!.SetValue(flight, value);
                                break;
                            }
                            else
                            {
                                string elementInput = Console.ReadLine();
                                var value = fieldValid[element.Key].DynamicInvoke(elementInput);
                                flight.GetType().GetProperty(element.Key)!.SetValue(flight, value);
                                if (element.Key == "DepartureCity")
                                {
                                    flight.GetType().GetProperty("DepartureCountry")!.SetValue(flight,
                                        GetCountryByCity(value));
                                }

                                if (element.Key == "ArrivalCity")
                                {
                                    flight.GetType().GetProperty("ArrivalCountry")!.SetValue(flight,
                                        GetCountryByCity(value));
                                }

                                break;
                            }


                        }
                        catch (Exception err)
                        {
                            if (err.InnerException != null)
                            {
                                Console.WriteLine($"Error: {err.InnerException.Message}");
                            }
                        }    
                    }
                    else
                    {
                        break;
                    }
                    
                }
            }
    
            AddFlight(flight);
        }
        public int GetMaxFlightId()
        {
            return Flights.Values
                .SelectMany(countryDict => countryDict.Values)
                .SelectMany(flightList => flightList)
                .Max(flight => flight.Id);
        }
        public void SaveFlightsToJson(string filePath)
        {
            var allFlights = Flights.Values
                .SelectMany(countryDict => countryDict.Values)
                .SelectMany(flightList => flightList)
                .OrderBy(flight => flight.Id)
                .ToList();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters =
                {
                    new CityEnumConverter(),
                    new AirlinesEnumConverter(),
                    new CountryEnumConverter()
                }
            };
                
            string jsonString = JsonSerializer.Serialize(allFlights, options);

            File.WriteAllText(filePath, jsonString);
        }

    }
}

