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
    public class GraphCollection2
    {
        // Ключ - місто відправлення, значення - словник, де ключ - країна прибуття, значення - список рейсів.
        public Dictionary<City, Dictionary<Country, List<Flight>>> Flights;

        public GraphCollection2()
        {
            Flights = new Dictionary<City, Dictionary<Country, List<Flight>>>();
        }

        public void AddFlight(Flight flight)
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
        }
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
                            else if(tempKey == "ArrivalCountry" && !Enum.TryParse<Country>(element.GetProperty(tempKey).ToString(), out _))
                            {
                                throw new Exception("Invalid country name in ArrivalCountry.");
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
        public void FindCheapestFlight(City departureCity, City arrivalCity, DateTime date)
        {
            List<List<Flight>> cheapestFlights = new List<List<Flight>>();
            double cheapestPrice = double.MaxValue;

            var allFlights = FindAllRoutes(departureCity, arrivalCity, date);

            // Видаляємо маршрути, в яких час відправлення наступного рейсу раніше, ніж час прибуття попереднього плюс 1 година, або пізніше, ніж час прибуття попереднього плюс 7 годин.
            var validFlights = allFlights.Where(flightPath =>
            {
                for (int i = 0; i < flightPath.Count - 1; i++)
                {
                    if (flightPath[i].ArrivalDatetime.AddHours(1) > flightPath[i + 1].DepartureDatetime || 
                        flightPath[i].ArrivalDatetime.AddHours(7) < flightPath[i + 1].DepartureDatetime)
                    {
                        return false;
                    }
                }

                return true;
            }).ToList();

            foreach (var flightPath in validFlights)
            {
                double totalPrice = flightPath.Sum(flight => flight.Price);
                if (totalPrice < cheapestPrice)
                {
                    cheapestPrice = totalPrice;
                    cheapestFlights.Clear();
                    cheapestFlights.Add(new List<Flight>(flightPath));
                }
                else if (totalPrice == cheapestPrice)
                {
                    cheapestFlights.Add(new List<Flight>(flightPath));
                }
            }

            // Виводимо найдешевші рейси
            if (cheapestFlights.Count > 0)
            {
                Console.WriteLine($"Cheapest flights from {departureCity} to {arrivalCity} on {date.ToShortDateString()} are: ");
                foreach (var flightPath in cheapestFlights)
                {
                    Console.WriteLine();
                    Console.WriteLine("Flight path: ");
                    foreach (var flight in flightPath)
                    {
                        Console.WriteLine(flight);
                    }
                    Console.WriteLine($"Total price: {flightPath.Sum(flight => flight.Price)}");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine($"There is no available flight from {departureCity} to {arrivalCity} on {date.ToShortDateString()}.");
            }
        }

        private void FindAllRoutesDFS(City currentCity, City targetCity, DateTime date, List<Flight> currentRoute, List<List<Flight>> allRoutes)
        {
            if (currentCity == targetCity)
            {
                allRoutes.Add(new List<Flight>(currentRoute));
                return;
            }

            if (!Flights.ContainsKey(currentCity))
            {
                return;
            }

            foreach (var nextFlight in Flights[currentCity].SelectMany(x => x.Value))
            {
                if (nextFlight.DepartureDatetime >= date && (currentRoute.Count == 0 || nextFlight.DepartureDatetime > currentRoute.Last().ArrivalDatetime))
                {
                    currentRoute.Add(nextFlight);
                    FindAllRoutesDFS(nextFlight.ArrivalCity, targetCity, date, currentRoute, allRoutes);
                    currentRoute.RemoveAt(currentRoute.Count - 1);
                }
            }
        }

        public List<List<Flight>> FindAllRoutes(City departureCity, City arrivalCity, DateTime date)
        {
            var allRoutes = new List<List<Flight>>();
            FindAllRoutesDFS(departureCity, arrivalCity, date, new List<Flight>(), allRoutes);
            return allRoutes;
        }

        private void FindRoutesDFS(City currentCity, City targetCity, DateTime date, List<Flight> currentPath, 
            List<List<Flight>> allRoutes, HashSet<City> visitedCities, ref double cheapestPrice)
        {
            if (currentCity == targetCity)
            {
                double currentPrice = currentPath.Sum(flight => flight.Price);
                if (currentPrice <= cheapestPrice)
                {
                    if (currentPrice < cheapestPrice)
                    {
                        allRoutes.Clear();
                        cheapestPrice = currentPrice;
                    }
                    allRoutes.Add(new List<Flight>(currentPath));
                }
                return;
            }

            if (!Flights.ContainsKey(currentCity) || visitedCities.Contains(currentCity))
            {
                return;
            }

            visitedCities.Add(currentCity);
            foreach (var countryFlightsPair in Flights[currentCity])
            {
                foreach (var flight in countryFlightsPair.Value)
                {
                    if (flight.DepartureDatetime.Date == date.Date)
                    {
                        currentPath.Add(flight);
                        FindRoutesDFS(flight.ArrivalCity, targetCity, date, currentPath, allRoutes, visitedCities, ref cheapestPrice);
                        currentPath.RemoveAt(currentPath.Count - 1);
                    }
                }
            }
            visitedCities.Remove(currentCity);
        }
        public void FindCheapestFlightsToCountry(City departureCity, Country arrivalCountry, DateTime date)
        {
            // Знайдіть всі міста в цільовій країні
            var arrivalCities = Flights.Values
                .SelectMany(countryToFlightsDict => countryToFlightsDict[arrivalCountry])
                .Select(flight => flight.ArrivalCity)
                .Distinct();

            // Для кожного міста в цільовій країні знайдіть найдешевший рейс
            foreach (var arrivalCity in arrivalCities)
            {
                Console.WriteLine($"Finding cheapest flights from {departureCity} to {arrivalCity} on {date.ToShortDateString()}:");
                FindCheapestFlight(departureCity, arrivalCity, date);
                Console.WriteLine();
            }
        }
        public void FindCheapestFlightsForMonth(City departureCity, City arrivalCity, int year, int month)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(year, month, day);
                Console.WriteLine($"Finding cheapest flights on {date.ToShortDateString()}:");
                FindCheapestFlight(departureCity, arrivalCity, date);
                Console.WriteLine();
            }
        }



    


        // Інші методи...
    }
}

