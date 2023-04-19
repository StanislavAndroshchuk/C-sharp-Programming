namespace Task3_Graph
{
  
    public class Flight
    {
        public int Id { get; set; }
        public City DepartureCity { get; set; }
        public City ArrivalCity { get; set; }
        public DateTime DepartureDatetime { get; set; }
        public DateTime ArrivalDatetime { get; set; }
        public Airlines Airline { get; set; }
        public double Price { get; set; }
        public override string ToString()
        {
            return $"{DepartureCity} - {ArrivalCity}, {DepartureDatetime} - {ArrivalDatetime}, {Airline}, ${Price} , (id: ${Id})";
        }
        public Flight(){}
        public Flight(int id ,City departureCity, City arrivalCity, DateTime departureDatetime, DateTime arrivalDatetime,
            Airlines airline, double price)
        {
            Id = id;
            DepartureCity = departureCity;
            ArrivalCity = arrivalCity;
            DepartureDatetime = departureDatetime;
            ArrivalDatetime = arrivalDatetime;
            Airline = airline;
            Price = price;
        }
    }
}

