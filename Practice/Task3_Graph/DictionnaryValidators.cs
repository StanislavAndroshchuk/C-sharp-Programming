namespace Task3_Graph
{
    public static class ValidDict
    {
        public static Dictionary<string, Delegate> ToValidFields()
        {
            Dictionary<string, Delegate> fieldValid = 
            new Dictionary<string, Delegate>
            {
                {"DepartureCity", Validation.ValidateCity},
                {"ArrivalCity", Validation.ValidateCity},
                {"DepartureDatetime", Validation.ValidDate},
                {"ArrivalDatetime", Validation.ValidBothDate},
                {"Airline", Validation.ValidateAirline},
                {"Price", Validation.ValidPrice}
            };
            return fieldValid;

        }
    }
}

