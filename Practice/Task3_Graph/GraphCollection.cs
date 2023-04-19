using System.Reflection;
using System.Text.Json;

namespace Task3_Graph
{
    public class GraphCollection
    {
        private readonly Dictionary<City, List<Flight>> _graph;
        public GraphCollection()
        {
            _graph = new Dictionary<City, List<Flight>>();
        }
        private const int MinTimeBetweenFlightsHours = 1;
        private const int MaxTimeBetweenFlightsHours = 7;
        public int GetFlightCount()
        {
            return _graph.Values.SelectMany(flightList => flightList).Count();
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
                            var value = fieldValid[element.Key].DynamicInvoke(Console.ReadLine());
                            flight.GetType().GetProperty(element.Key)!.SetValue(flight, value);
                            break;
                        }
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine($"Error: {err.InnerException?.Message}");
                    }
                }
            }
    
            AddFlight(flight);
        }
        private void AddFlight(Flight flight)
        {
            if (flight.DepartureCity == flight.ArrivalCity)
            {
                Console.WriteLine("Error: Departure and arrival cities cannot be the same.");
                return;
            }

            City departureCity = flight.DepartureCity;
            if (_graph.ContainsKey(departureCity))
            {
                _graph[departureCity].Add(flight);
            }
            else
            {
                _graph[departureCity] = new List<Flight> { flight };
            }
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
        /*public void EditFlightById(int id, string attribute, object value)
    {
        int i = 0;
        while (true)
        {
            int id = Validate.ValidateItem<int>("id", Validate.CheckId);
            string fieldName = AdditionalFun.IsFieldNameOfFlight();
            
            foreach (var kvp in _graph)
            {
                List<Flight> flights = kvp.Value;
            
                Flight flightToEdit = flights.FirstOrDefault(f => f.Id == id);

                if (flightToEdit != null)
                {
                    PropertyInfo? property = typeof(Flight).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (property == null && property?.Name == "Id")
                    {
                        Console.WriteLine("Invalid field name!");
                    }
                    else
                    {
                        i++;
                        if(property.PropertyType == typeof(double))
                        {
                            double newValue = Validate.ValidateItem<double>("Price", Validate.CheckPrice);
                            property.SetValue(flightToEdit, newValue);
                            return;
                        }
                        else if (property.PropertyType == typeof(City))
                        {
                            City newValue = Validate.ValidateItem<City>("City (name of city or number from 0 to 6)", Validate.IsValidEnumValue<City>);
                            property.SetValue(flightToEdit, newValue);
                            return;
                        }
                        else if (property.PropertyType == typeof(Airline))
                        {
                            Airline newValue = Validate.ValidateItem<Airline>("Airline (name of airlane or number from 0 to 6)", Validate.IsValidEnumValue<Airline>);
                            property.SetValue(flightToEdit, newValue);
                            return;
                        }
                        else if(property.PropertyType == typeof(DateTime))
                        {
                            
                            if (property.Name == "ArrivalDatetime")
                            {
                                DateTime newValue = Validate.IsDepartureBeforeArrival(flightToEdit.DepartureDatetime);
                                property.SetValue(flightToEdit, newValue);
                                return;
                            }
                            else
                            {
                                DateTime newValue = Validate.ValidateItem<DateTime>("DepartureDatetime", Validate.IsValidDateTimeString);
                                property.SetValue(flightToEdit, newValue);
                                return;
                            }
                        }
                    }
                }
                
            }
            if(i==0) Console.WriteLine("There is not element with id - {0} .", id);
        }
    }*/

        private Flight? FindById(int id)
        {
            foreach (var flightList in _graph.Values)
            {
                foreach (var flight in flightList)
                {
                    Console.WriteLine($"Current flight ID: {flight.Id}");
                    if (flight.Id == id)
                    {
                        return flight;
                    }
                }
            }

            return null;
        }
        
        public void FindCheapestFlight(City departureCity, City arrivalCity, DateTime date)
        {
            Flight cheapestFlight = null;
            double cheapestPrice = double.MaxValue;
            Dictionary<Flight, List<Flight>> paths = new Dictionary<Flight, List<Flight>>();
            HashSet<City> visitedCities = new HashSet<City>();

            // Знайти всі рейси з міста відправлення
            var flightsFromDepartureCity = _graph[departureCity].Where(flight =>
                flight.DepartureCity == departureCity && flight.DepartureDatetime.Date == date.Date);

            // Додати кожен рейс з міста відправлення до списку шляхів
            foreach (var flight in flightsFromDepartureCity)
            {
                paths.Add(flight, new List<Flight> { flight });
            }

            while (paths.Count > 0)
            {
                // Отримати шлях із найнижчою вартістю
                KeyValuePair<Flight, List<Flight>> currentPath =
                    paths.OrderBy(path => path.Value.Sum(flight => flight.Price)).First();

                // Видалити поточний шлях зі списку шляхів
                paths.Remove(currentPath.Key);

                // Отримати місто прибуття останнього рейсу в поточному шляху
                City currentCity = currentPath.Value.Last().ArrivalCity;

                if (currentCity == arrivalCity)
                {
                    // Знайти найнижчу вартість рейсу
                    double currentPrice = currentPath.Value.Sum(flight => flight.Price);
                    if (currentPrice < cheapestPrice)
                    {
                        cheapestPrice = currentPrice;
                        cheapestFlight = currentPath.Key;
                    }
                }
                else if (!visitedCities.Contains(currentCity))
                {
                    // Знайти всі рейси з поточного міста
                    var flightsFromCurrentCity = _graph[departureCity].Where(flight =>
                        flight.DepartureCity == currentCity && !visitedCities.Contains(flight.ArrivalCity));

                    // Додати кожен рейс з поточного міста до списку шляхів
                    foreach (var flight in flightsFromCurrentCity)
                    {
                        var newPath = new List<Flight>(currentPath.Value) { flight };
                        paths.Add(flight, newPath);
                    }
                }

                visitedCities.Add(currentCity);
            }

            if (cheapestFlight != null)
            {
                Console.WriteLine(
                    $"Cheapest flight from {departureCity} to {arrivalCity} on {date.ToShortDateString()} is: ");
                Console.WriteLine($"Departure: {cheapestFlight.DepartureCity} - {cheapestFlight.DepartureDatetime}");
                Console.WriteLine($"Arrival: {cheapestFlight.ArrivalCity} - {cheapestFlight.ArrivalDatetime}");
                Console.WriteLine($"Airline: {cheapestFlight.Airline}");
                Console.WriteLine($"Price: {cheapestFlight.Price}");
            }
            else
            {
                Console.WriteLine(
                    $"There is no available flight from {departureCity} to {arrivalCity} on {date.ToShortDateString()}.");
            }
        }
        
        private List<List<Flight>> FindRoutes(City departureCity, City arrivalCity, DateTime date, List<Flight> currentRoute = null, List<List<Flight>> routes = null)
        {
            if (currentRoute == null) currentRoute = new List<Flight>();
            if (routes == null) routes = new List<List<Flight>>();

            if (!_graph.ContainsKey(departureCity))
            {
                return routes;
            }

            var possibleFlights = _graph[departureCity].Where(flight =>
                flight.DepartureDatetime.Date == date.Date &&
                (currentRoute.Count == 0 || (flight.DepartureDatetime >= currentRoute.Last().ArrivalDatetime.AddHours(MinTimeBetweenFlightsHours) &&
                                             flight.DepartureDatetime <= currentRoute.Last().ArrivalDatetime.AddHours(MaxTimeBetweenFlightsHours)))
            ).ToList();

            foreach (var flight in possibleFlights)
            {
                if (flight.ArrivalCity == arrivalCity)
                {
                    var newRoute = new List<Flight>(currentRoute) { flight };
                    routes.Add(newRoute);
                }
                else if (!currentRoute.Any(f => f.ArrivalCity == flight.ArrivalCity))
                {
                    var newRoute = new List<Flight>(currentRoute) { flight };
                    FindRoutes(flight.ArrivalCity, arrivalCity, date, newRoute, routes);
                }
            }

            return routes;
        }
        private void FindRoutesDFS(City currentCity, City targetCity, DateTime date, List<Flight> currentRoute, 
            List<List<Flight>> allRoutes, HashSet<City> visitedCities)
        {
            if (currentCity == targetCity)
            {
                allRoutes.Add(new List<Flight>(currentRoute));
                return;
            }

            if (!_graph.ContainsKey(currentCity) || visitedCities.Contains(currentCity))
            {
                return;
            }

            visitedCities.Add(currentCity);
            foreach (var flight in _graph[currentCity])
            {
                if (flight.DepartureDatetime.Date == date.Date)
                {
                    currentRoute.Add(flight);
                    FindRoutesDFS(flight.ArrivalCity, targetCity, date, currentRoute, allRoutes, visitedCities);
                    currentRoute.RemoveAt(currentRoute.Count - 1);
                }
            }
            visitedCities.Remove(currentCity);
        }
        public List<List<Flight>> FindCheapestRoutes(City departureCity, City arrivalCity, DateTime date)
        {
            var routes = FindRoutes(departureCity, arrivalCity, date);
            if (routes.Count == 0) return new List<List<Flight>>();

            var minTotalPrice = routes.Min(route => route.Sum(flight => flight.Price));
            var cheapestRoutes = routes.Where(route => route.Sum(flight => flight.Price) == minTotalPrice).ToList();

            return cheapestRoutes;
        }
        public void RemoveFlightById(int id)
        {
            KeyValuePair<City, List<Flight>>? cityFlightsPair = null;
            Flight flightToRemove = null;

            foreach (var pair in _graph)
            {
                flightToRemove = pair.Value.FirstOrDefault(flight => flight.Id == id);
                if (flightToRemove != null)
                {
                    cityFlightsPair = pair;
                    break;
                }
            }

            if (cityFlightsPair.HasValue && flightToRemove != null)
            {
                _graph[cityFlightsPair.Value.Key].Remove(flightToRemove);
                Console.WriteLine($"Flight with ID {id} was successfully removed.");
            }
            else
            {
                Console.WriteLine($"Flight with ID {id} not found.");
            }
        }


        public void OutputAllFlights()
        {
            foreach (var kvp in _graph)
            {
                City city = kvp.Key;
                List<Flight> flights = kvp.Value;

                Console.WriteLine("City: {0}", city);
                Console.WriteLine("Available flights: ");

                foreach (var flight in flights)
                {
                    Console.WriteLine(flight);
                }

                Console.WriteLine();
            }
        }
        public void ReadFromFile(string path)
        {
            string jsonString = File.ReadAllText(path);

            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new EnumIgnoreCaseConverter<City>(),
                    new EnumIgnoreCaseConverter<Airlines>()
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
    }
}

