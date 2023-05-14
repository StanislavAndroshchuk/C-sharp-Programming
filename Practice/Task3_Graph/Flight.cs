namespace Task3_Graph
{
  
    public class Flight
    {
        public int Id { get; set; }
        public City DepartureCity { get; set; }
        public Country DepartureCountry { get; set; }
        public City ArrivalCity { get; set; }
        public Country ArrivalCountry { get; set; }
        public DateTime DepartureDatetime { get; set; }
        public DateTime ArrivalDatetime { get; set; }
        public Airlines Airline { get; set; }
        public double Price { get; set; }
        public override string ToString()
        {
            return $"({DepartureCountry}:{DepartureCity}) - ({ArrivalCountry}:{ArrivalCity}), {DepartureDatetime} - {ArrivalDatetime}, {Airline}, ${Price} , (id: ${Id})";
        }
        public Flight(){}
        public Flight(int id ,City departureCity, Country departureCountry, City arrivalCity, Country arrivalCountry, DateTime departureDatetime, DateTime arrivalDatetime,
            Airlines airline, double price)
        {
            Id = id;
            DepartureCity = departureCity;
            DepartureCountry = departureCountry;
            ArrivalCity = arrivalCity;
            ArrivalCountry = arrivalCountry;
            DepartureDatetime = departureDatetime;
            ArrivalDatetime = arrivalDatetime;
            Airline = airline;
            Price = price;
        }
    }
}

