
using System.Text.Json.Serialization;

namespace Task2_Class
{
    public class Order
    {
        private int _id;
        public int ID
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
        
        public Order()
        {
        }

        [JsonConstructor]
        public Order(int id, OrderTypes orderStatus, int amount, string discount, DateOnly orderDate, DateOnly shippedDate, string customerEmail)
        {
            ID = id;
            OrderStatus = orderStatus;
            Amount = amount;
            Discount = discount;
            OrderDate = orderDate;
            ShippedDate = shippedDate;
            CustomerEmail = customerEmail;
        }

        public override string ToString()
        {
            return $"ID: {ID}\nOrder Status: {OrderStatus}\nAmount: {Amount}\nDiscount: {Discount}\nOrder Date: {OrderDate}\nShipped Date: {ShippedDate}\nCustomer Email: {CustomerEmail}";
        }

        public void ToWrite(int count)
        {
            
            
            Dictionary<string, Delegate> fieldValid;
            fieldValid = ValidDict.ToValidFields();
            foreach (var element in fieldValid)
            {
                while (true) //todo: пофіксити, зробити зміну , щоб потім можна було вийти з циклу
                {
                    Console.Write($"[{element.Key}]: ");
                    if (element.Key == "OrderStatus")
                    {
                        Console.WriteLine("1 - Paid, 2 - NotPaid, 3 - Refunded-3");
                    }

                    if (element.Key == "ID")
                    {
                        Console.WriteLine($"{count+1}");
                    }
                    
                    try
                    {
                        if (element.Key == "ID")
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

    }
}