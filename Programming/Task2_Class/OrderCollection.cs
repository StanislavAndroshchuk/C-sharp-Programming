using System.Text.Json;

namespace Task2_Class
{
    public class OrderCollection
    {
        public string? SomeFile { get; set; }
        public List<Order> Collection;
    
        public OrderCollection()
        {
            Collection = new List<Order>();
        }
    
        public Order this[int x]
        {
            get => Collection[x];
            set => Collection[x] = value;
        }

        public int Count => Collection.Count;
    
        public override string ToString()
        {
            string toPrint = "";
            foreach (var order in Collection)
            {
                toPrint += order + "\n";
            }
            return toPrint;
        }
    
        public void Append(Order element)
        {
            Collection.Add(element);
        }
    
        public string ToSearch(string lookingFor)
        {
            string toReturnFound = "";
            toReturnFound += "Looking for " + lookingFor + ":\n\n";
            foreach (var order in Collection)
            {
                foreach (var valueAttr in order.GetType().GetProperties())
                {
                    if (valueAttr.GetValue(order)!.ToString()!.Contains(lookingFor))
                    {
                        toReturnFound += "Order with Id " + order.ID + " have coincidence:\n";
                        toReturnFound += order + "\n";
                        break;
                    }
                }
            }
            return toReturnFound;
        }
    
        public void ToSort(string element)
        {
            if (element == "Discount")
            {
                Collection = Collection
                    .OrderBy(x => {
                        string strValue = x.GetType().GetProperty(element)!.GetValue(x)!.ToString()!;
                        if (strValue.Length > 1) {
                            strValue = strValue.Substring(0, strValue.Length - 1);
                            if (int.TryParse(strValue, out int intValue)) {
                                return intValue;
                            }
                        }
                        // return a default value if the substring cannot be converted to an integer
                        return 0;
                    })
                    .ToList();
            }
            else if (element != "CustomerEmail")
            {
                Collection = Collection.OrderBy(x => x.GetType().GetProperty(element)?.GetValue(x)).ToList();
            }
            else
            {
                Collection = Collection.OrderBy(x => x.CustomerEmail.ToLower()).ToList();
            }
        }
        public int? FindById(int getId)
        {
            for (int i = 0; i < Collection.Count; i++)
            { 
                if (getId == Collection[i].ID)
                {
                    return i;
                }
            }
            return null;
        }
    
        public bool DeleteById(string getId)
        {
            for (int i = 0; i < Collection.Count; i++)
            {
                try
                {
                    if (int.TryParse(getId, out int _)) // try catch
                    {
                        if (Collection[i].ID == int.Parse(getId))
                        {
                            Collection.RemoveAt(i);
                            Console.WriteLine("Order has been deleted\n");
                            return true;
                        }
                    }
                    else
                    {
                        throw new Exception($"{getId} have to be integer");
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.InnerException?.Message);
                    return false;
                }
                
            }

            return false;
        }
    
        public void ToEdit(int getId, string attribute, object value)
        {
            var propertyInfo = Collection[getId].GetType().GetProperty(attribute);
            propertyInfo?.SetValue(Collection[getId], value);
        }
    
        public void Rewrite(string path)
        {
            Collection = Collection.OrderBy(x => x.GetType().GetProperty("ID")?.GetValue(x)).ToList(); //s
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Collection, options);
    
            File.WriteAllText(path, jsonString);
        }
    
        
    }
    public abstract class WorkWithFile
    {
        public static OrderCollection ReadFromFile(string fileName)
        {
            OrderCollection collection = new OrderCollection();
            string jsonString = File.ReadAllText(fileName);
            List<JsonElement>? data = JsonSerializer.Deserialize<List<JsonElement>>(jsonString);
            Dictionary<string, Delegate> fieldValid = ValidDict.ToValidFields();
            foreach (JsonElement element in data!)
            {
                bool passed = true;
                for (int i = 0; i < fieldValid.Count; i++)
                {   
                    
                    try
                    {
                        string tempKey = fieldValid.Keys.ElementAt(i); // string?
                        
                        if (tempKey == "ShippedDate")
                        {
                            fieldValid[tempKey].DynamicInvoke(element.GetProperty(tempKey).ToString(),
                                element.GetProperty("OrderDate").ToString() );
                        }
                        else
                        {
                            fieldValid[tempKey].DynamicInvoke(element.GetProperty(tempKey).ToString());
                            /*if (fieldValid[temp_key] == Validation.ValidPositiveInt)
                            {
                                fieldValid[temp_key]?.DynamicInvoke(element.GetProperty(temp_key));
                                
                            }
                            else
                            {
                                fieldValid[temp_key]?.DynamicInvoke(element.GetProperty(temp_key).ToString());
                            }*/
                            
                        }
                        
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.InnerException?.Message);
                        passed = false;
                    }
                }

                if (passed)
                {
                    Order tempOrder = JsonSerializer.Deserialize<Order>(element)!;
                    collection.Append(tempOrder);
                }
                else
                {
                    Console.WriteLine("Previous Order had problems during validation.");
                }
            }
    
            return collection;
        }
    }
}


