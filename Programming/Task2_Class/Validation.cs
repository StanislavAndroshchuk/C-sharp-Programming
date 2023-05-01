
using System.Text.RegularExpressions;


namespace Task2_Class
{
    public static class Validation
    {
        public static bool ValidInt(string data)
        {
            if (int.TryParse(data, out int _))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // Validation of init
        public static string ValidString(string text)
        {
            string regex = @"^[A-Za-z ]+$";
            if (Regex.IsMatch(text, regex))
            {
                return text;
            }
            else
            {
                throw new Exception("Element is not valid string!");
            }
        }
        public static string ValidUrl(string url)
        {
            string regex = @"^(http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?$";
            if (Regex.IsMatch(url, regex))
            {
                return url;
            }
            else
            {
                throw new Exception("URL is not valid!");
            }
        }
        public static string ValidEmail(string email)
        {
            string regex = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b";
            if (Regex.IsMatch(email, regex))
            {
                return email;
            }
            else
            {
                throw new Exception("Email is not valid!");
            }
        }

        public static int ValidPositiveInt(string amount)
        {
            if (int.TryParse(amount,out int result))
            {
                if (result >= 0)
                {
                    return result;
                }
                else
                {
                    throw new Exception($"{result} have to be positive!");
                }
            }
            else
            {
                throw new Exception($"{result} have to be integer");
            }
        }

        public static string ValidDiscount(string discount)
        {
            if (discount.EndsWith("%"))
            {
                discount = discount.Substring(0, discount.Length - 1); // for example "5%" -> "5" and after validation return to "5%"
            }

            if (int.TryParse(discount,out int temp))
            {
                if (temp >= 0)
                {
                    if (temp <= 100)
                    {
                        return discount + "%";
                    }
                    else
                    {
                        throw new Exception("Discount is wrong, cant be more that 100%");
                    }
                }
                else
                {
                    throw new Exception("Discount have to be positive!");
                }
            }
            else
            {
                throw new Exception("Discount have to be integer!");
            }
        }

        public static OrderTypes ValidOrder(dynamic order)
        {
            int newValue = Convert.ChangeType(order, typeof(int));
            if (!typeof(OrderTypes).IsEnumDefined(newValue))
            {
                throw new Exception("No such order type.");
            }
            return (OrderTypes)newValue;
            
        }
        /*public static string ValidOrder(string order)
        {
            if (ValidInt(order))
            {
                Console.WriteLine("Order cant be a number!");
                return null;
            }
            else
            {
                if (order == "paid" || order == "refunded" || order == "not paid")
                {
                    return order;
                }
                else
                {
                    Console.WriteLine("Order can be only paid, refunded or not paid");
                    return null;
                }
            }
        }*/

        public static DateOnly ValidDate(string date)
        {
            
            if (DateOnly.TryParse(date, out DateOnly result))
            {
                return result;
            }
            else
            {
                throw new Exception("Date is not valid!");
            }
        }
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new Exception("Password is empty or null!");
            }
            if (password.Length < 8)
            {
                throw new Exception("Password must be at least 8 characters long!");
            }
            if (!Regex.IsMatch(password, @"\d"))
            {
                throw new Exception("Password must contain at least one digit!");
            }
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                throw new Exception("Password must contain at least one uppercase letter!");
            }
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                throw new Exception("Password must contain at least one lowercase letter!");
            }
            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]"))
            {
                throw new Exception("Password must contain at least one special character!");
            }
            return true;
        }

        public static DateTime ValidDateTime(string date)
        {
            
            if (DateTime.TryParse(date, out DateTime result))
            {
                return result;
            }
            else
            {
                throw new Exception("Date is not valid!");
            }
        }

        public static DateOnly ValidBothDate(string shippedDate, string orderDate)
        {
            if (DateOnly.TryParse(shippedDate, out DateOnly result) && DateOnly.TryParse(orderDate, out DateOnly result2))
            {
                if (result >= result2)
                {
                    return result;
                }
                else
                {
                    throw new Exception("Shipped date cant be before order date!");
                }
            }
            else
            {
                throw new Exception("Shipped date is not valid!");
            }
        }

        /*public static T ValidScopeInput<T>(Func<string, T> func, string input)
        {
            T result = func(input);
            while (result == null || result.Equals(-1) || result.Equals(DateOnly.MinValue))
            {
                input = Console.ReadLine();
                result = func(input!);
            }
            return result;
        }

        public static DateOnly ValidScopeDoubleInput(Func<string, DateOnly, DateOnly> func, string input, DateOnly orderDate)
        {
            DateOnly result = func(input, orderDate);
            while (result == null || result.Equals(-1) || result.Equals(DateOnly.MinValue))
            {
                input = Console.ReadLine();
                result = func(input, orderDate);
            }
            return result;
        }*/
    }
}