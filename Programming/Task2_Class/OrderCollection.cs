using System.Text.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Task2_Class
{
    public interface IIdentifier
    {
        int ID { get; set; }
    }

    public interface IValidators
    {
        Dictionary<string, Delegate> ToValidFields();
    }
    public class OrderCollection<T> : IUserAction<T> where T: class, IIdentifier, IValidators, new()
    {
        public List<T> Collection;
        public OrderCollection()
        {
            Collection = new List<T>();
            
        }
        public T? this[int x]
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
        public void Append(T element)
        {
            Collection.Add(element!);
        }
        public List<string> GetListOfPropertys()
        {
            T item = new T();
            Type orderType = typeof(T);
            List<string> fieldNames = new List<string>();
            foreach (var propertyInfo in orderType.GetProperties())
            {
                fieldNames.Add(propertyInfo.Name);
            }
            return fieldNames;
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
           else if (typeof(T).GetProperty(element).PropertyType == typeof(string))
           {
               
               Collection = Collection.OrderBy(x => x.GetType().GetProperty(element)?.GetValue(x)?.ToString()?.ToLower()).ToList();
           }
           else
           {
               Collection = Collection.OrderBy(x => x.GetType().GetProperty(element)?.GetValue(x)).ToList();
           } 
            
        }
        public T? FindById(int getId)
        {
            for (int i = 0; i < Collection.Count; i++)
            { 
                if (getId == Collection[i].ID)
                {
                    return Collection[i];
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
                        else
                        {
                            Console.WriteLine("Can't delete unexciting order by id " + getId + "\n");
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
            T? classItem = FindById(getId);
            PropertyInfo? someProperty = classItem!.GetType().GetProperty(attribute);
            object toSet = Convert.ChangeType(value, someProperty?.PropertyType!);
            someProperty?.SetValue(classItem, toSet);
        }
        public Dictionary<string, Delegate> GetValidFields()
        {
            T item = new T();
            Dictionary<string, Delegate> fieldValid = item.ToValidFields();
            return fieldValid;
        }
        public void Rewrite(string path)
        {
            Collection = Collection.OrderBy(x => x.GetType().GetProperty("ID")?.GetValue(x)).ToList(); //s
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(Collection, options);
    
            File.WriteAllText(path, jsonString);
        }
        public void ReadFromFile(string fileName)
        {
            string jsonString = File.ReadAllText(fileName);
            List<JsonElement>? data = JsonSerializer.Deserialize<List<JsonElement>>(jsonString);
            Dictionary<string, Delegate> fieldValid = GetValidFields();
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
                            fieldValid[tempKey].DynamicInvoke(element.GetProperty(tempKey).ToString()); //
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
                    T tempOrder = JsonSerializer.Deserialize<T>(element)!;
                    Collection.Add(tempOrder);
                }
                else
                {
                    Console.WriteLine("Previous Order had problems during validation.");
                }
            }
            
        }
        public List<T> ViewList()
        {
            return Collection;
        }
        public T ViewById(int id)
        {
            return Collection.FirstOrDefault(s => s.ID == id)!;
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
        public List<T> Search(string query)
        {
            List<T> toReturnFound = new List<T>();
            foreach (var order in Collection)
            {
                foreach (var valueAttr in order.GetType().GetProperties())
                {
                    if (valueAttr.GetValue(order)!.ToString()!.Contains(query))
                    {
                        toReturnFound.Add(order);
                        break;
                    }
                }
            }
            return toReturnFound;
        }
        public List<T> Sort(string sortBy)
        {
            List<T> sortedCollection = Collection;
            if (sortBy == "Discount")
            {
                sortedCollection = sortedCollection
                    .OrderBy(x => {
                        string strValue = x.GetType().GetProperty(sortBy)!.GetValue(x)!.ToString()!;
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
            else if (typeof(T).GetProperty(sortBy)!.PropertyType == typeof(string))
            {
               
                sortedCollection = sortedCollection.OrderBy(x => x.GetType().GetProperty(sortBy)?.GetValue(x)?.ToString()?.ToLower()).ToList();
            }
            else
            {
                sortedCollection = sortedCollection.OrderBy(x => x.GetType().GetProperty(sortBy)?.GetValue(x)).ToList();
            }

            return sortedCollection;
        }

        public T Create(T element)
        {
            Console.WriteLine("asda");
            return element;
        }

        public T Edit(T element)
        {
            return element;
        }

        public void Delete(int id)
        {
            return;
        }
    }
}


