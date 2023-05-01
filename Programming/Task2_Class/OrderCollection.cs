using System.Collections.ObjectModel;
using System.Text.Json;
using System.Reflection;

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
            set => Collection[x] = value!;
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
            if (Collection.Count != 0)
            {
                return Collection;
            }
            else
            {
                throw new Exception("List is empty!");
            }
            
        }
        public T ViewById(int id)
        {
            var toCheck = Collection.FirstOrDefault(s => s.ID == id);
            if (toCheck != null)
            {
                return toCheck;
            }
            else
            {
                throw new Exception("There is no such elements by this id!");
            }
           
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
            if (toReturnFound.Count == 0)
            {
                throw new Exception("Cannot found by this paramether");
            }
            return toReturnFound;
        }
        public List<T> Sort(string sortBy)
        {
            List<string> listOfParam = GetListOfPropertys();
            if (listOfParam.Contains(sortBy))
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
            else
            {
                throw new Exception("There is no such parameter to sort by!");
            }
            
        }

        public T Create(T element)
        {
            Collection.Add(element);
            return element;
        }

        public T Edit(int getId, string attribute, object value)
        {
            var classItem = FindById(getId);
            if (classItem != null)
            {
                var fieldValid = GetValidFields();
                if (fieldValid.ContainsKey(attribute))
                {
                    
                    PropertyInfo? someProperty = classItem!.GetType().GetProperty(attribute);
                    object toSet = Convert.ChangeType(value, someProperty?.PropertyType!);
                    someProperty?.SetValue(classItem, toSet);
                    return classItem;
                }
                else
                {
                    throw new Exception("Cant edit by this attribute!");
                }
            }
            else
            {
                throw new Exception("There is no such element by this id");
            }
            
        }

        public T Delete(int id)
        {
            var deleteElement = FindById(id);
            if (deleteElement != null)
            {
                Collection.RemoveAt(id-1);
                Console.WriteLine("Order has been deleted\n");
                return deleteElement;
            }

            throw new Exception("There's no such element by id");
        }
    }
}


