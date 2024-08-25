
using System.Reflection;
using System.Text.Json.Serialization;

namespace Task2_Class
{
    public enum StatesEnum {Draft, Moderation, Published}
    public class Order : IClassEntity
    {
        private int _id;
        public int Id
        {
            get => _id; 
            set { _id = value; }
        }

        public OrderTypes OrderStatus { get; set; }
        public int Amount { get; set; }
        public string Discount { get; set; } 
        public DateOnly OrderDate { get; set; }
        public DateOnly ShippedDate { get; set; }
        public string CustomerEmail { get; set; }
        public StatesEnum ReadedState { get; set; }
        public Order()
        {
        }
        public Order(int id, OrderTypes orderStatus, int amount, string discount,
            DateOnly orderDate, DateOnly shippedDate, string customerEmail, StatesEnum state = StatesEnum.Draft){
            Id = id;
            OrderStatus = orderStatus;
            Amount = amount;
            Discount = discount;
            OrderDate = orderDate;
            ShippedDate = shippedDate;
            CustomerEmail = customerEmail;
            ReadedState = state;
        }
        public override string ToString()
        {
            string to_return = "";
            foreach (PropertyInfo x in this.GetType().GetProperties())
            {
                to_return += x.Name + " - " + x.GetValue(this) + "\n"; 
            }
            return to_return;
        }
        public Dictionary<string, Delegate> ToValidFields()
        {
            Dictionary<string, Delegate> fieldValid = new Dictionary<string, Delegate>
            {
                {"Id", Validation.ValidPositiveInt},
                {"OrderStatus", Validation.ValidOrder},
                {"Amount", Validation.ValidPositiveInt},
                {"Discount", Validation.ValidDiscount},
                {"OrderDate", Validation.ValidDate},
                {"ShippedDate", Validation.ValidBothDate},
                {"CustomerEmail", Validation.ValidEmail}
            };
            return fieldValid;

        }
        public void ToWrite(int count)
        {
            ReadedState = StatesEnum.Draft;
            Dictionary<string, Delegate> fieldValid;
            fieldValid = ToValidFields();
            foreach (var element in fieldValid)
            {
                while (true) 
                {
                    Console.Write($"[{element.Key}]: ");
                    if (element.Key == "OrderStatus")
                    {
                        Console.WriteLine("1 - Paid, 2 - NotPaid, 3 - Refunded-3");
                    }

                    if (element.Key == "Id") 
                    {
                        Console.WriteLine($"{count+1}"); 
                    }
                    
                    try
                    {
                        if (element.Key == "Id")
                        {
                            this.GetType().GetProperty(element.Key)!.SetValue(this, count+1);
                            break;
                        }
                        else if (element.Key == "ShippedDate")
                        {
                            var value = fieldValid[element.Key].DynamicInvoke(Console.ReadLine(),this.OrderDate.ToString());
                            this.GetType().GetProperty(element.Key)!.SetValue(this, value);
                            break;
                        }
                        else
                        {
                            var value = fieldValid[element.Key].DynamicInvoke(Console.ReadLine());
                            this.GetType().GetProperty(element.Key)!.SetValue(this, value);
                            break;
                        }
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine($"Error: {err.InnerException?.Message}");
                    }
                }
            }
            
        }
        private State _currentState => State.ConvertEnum(ReadedState, this);
       
        public void ChangeState(StatesEnum state)
        {
            ReadedState = state;
        }
        public void Publishing(User user)
        {
            _currentState.Publishing(user);
        }
    }
}