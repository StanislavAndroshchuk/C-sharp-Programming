namespace Task3_Graph
{
    public static class ValidDict
    {
        public static Dictionary<string, Delegate> ToValidFields()
        {
            Dictionary<string, Delegate> fieldValid = 
            new Dictionary<string, Delegate>
            {
                {"DepartureCity", Validation.IsValidCity},
                {"ArrivalCity", Validation.IsValidCity},
                {"DepartureDatetime", Validation.ValidDate},
                {"ArrivalDatetime", Validation.ValidBothDate},
                {"Airline", Validation.IsValidAirlines},
                {"Price", Validation.ValidPrice},
                {"ArrivalCountry",Validation.IsValidCountry},
                {"DepartureCountry",Validation.IsValidCountry}
            };
            return fieldValid;

        }
        
    }
}

