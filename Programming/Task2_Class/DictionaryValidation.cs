namespace Task2_Class
{
    public abstract class ValidDict
    {
        public static Dictionary<string, Delegate> ToValidFields()
        {
            Dictionary<string, Delegate> fieldValid = new Dictionary<string, Delegate>
            {
            {"ID", Validation.ValidPositiveInt},
            //{"OrderStatus", new Func<dynamic,OrderTypes>(Validation.ValidOrder)}
            {"OrderStatus", Validation.ValidOrder},
            {"Amount", Validation.ValidPositiveInt},
            {"Discount", Validation.ValidDiscount},
            {"OrderDate", Validation.ValidDate},
            {"ShippedDate", Validation.ValidBothDate},
            {"CustomerEmail", Validation.ValidEmail}
            };
            return fieldValid;

        }

        
    }
}