
namespace Task2_Class { 
    public static class Program
{
    private static readonly Dictionary<string, string> ToLookDict = new Dictionary<string, string>
        {
            {"St", "OrderStatus"},
            {"Am", "Amount"},
            {"Dis", "Discount"},
            {"OrDa", "OrderDate"},
            {"ShDa", "ShippedDate"},
            {"Em", "CustomerEmail"}
        };

    private static void PrintParameter()
    {
        Console.WriteLine("Choose parameter");
        Console.WriteLine("St - Status");
        Console.WriteLine("Am - Amount");
        Console.WriteLine("Dis - Discount");
        Console.WriteLine("OrDa - Order date");
        Console.WriteLine("ShDa - Shipped date");
        Console.WriteLine("Em - Email\n");
    }

    private static void Menu()
    {
        string toReadFile = "../../../Restaurant.json";
        OrderCollection<Restaurant> collection = new OrderCollection<Restaurant>();
        Dictionary<string, Delegate> fieldValid;
        fieldValid = collection.GetValidFields();
        collection.ReadFromFile(toReadFile);
        Console.WriteLine("Order collection is : \n");
        Console.WriteLine(collection);
        while (true)
        {
            Console.WriteLine(@"
----Menu----
Input 
1 to Add
2 to Search
3 to Delete
4 to Edit
5 to Sort
6 to Print Json
7 to Exit/Quit
                ");
            Console.WriteLine("Choose option");
            string inputNum = Console.ReadLine()!;
            if (inputNum == "1")
            {
                Restaurant toAdd = new Restaurant();
                toAdd.ToWrite(collection.Count);
                collection.Append(toAdd);
                collection.Rewrite(toReadFile);
            }
            else if (inputNum == "2") // else if
            {
                Console.WriteLine("Input what do you want to search: ");
                string lookingFor = Console.ReadLine()!;
                string toPrint = collection.ToSearch(lookingFor);
                Console.WriteLine(toPrint);
            }
            else if (inputNum == "3")
            {
                while (true)
                {
                    Console.WriteLine("Input id of Order to delete: ");
                    string getId = Console.ReadLine()!;
                    
                    if (collection.DeleteById(getId))
                    {
                        Console.WriteLine("Delete successful");
                        break;
                    }
                }
                collection.Rewrite(toReadFile);
            }
            else if (inputNum == "4")
            {
                while (true)
                {
                    try
                    {
                        Console.WriteLine("Input id to edit: ");
                        string? getId = Console.ReadLine();
                        if (Validation.ValidInt(getId!))
                        {
                            if (collection.FindById(int.Parse(getId!)) != null)
                            {
                                Console.WriteLine("Input attribute to edit :");
                                string elementToEdit = Console.ReadLine()!;
                                if (fieldValid.ContainsKey(elementToEdit))
                                {
                                    Console.WriteLine("Input value :");
                                    string value = Console.ReadLine()!; 
                                    var validValue = fieldValid[elementToEdit].DynamicInvoke(value);
                                    collection.ToEdit(int.Parse(getId!),elementToEdit,validValue!);
                                    collection.Rewrite(toReadFile);
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("There is no such attribute\n");
                                }
                            }
                            else
                            {
                                Console.WriteLine("Can't edit unexciting order by id " + getId + "\n");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{getId} have to be integer\n");
                        }
                    }
                    catch(Exception er)
                    {
                        Console.WriteLine(er.InnerException?.Message);
                    }
                }
            }
            else if (inputNum == "5")
            {
                Console.WriteLine("Input element to sort by");
                List<string> attributesName = collection.GetListOfPropertys();
                while (true)
                {
                    
                    Console.WriteLine("[Attribute]: ");
                    string elementToSort = Console.ReadLine()!;
                    if (attributesName.Contains(elementToSort))
                    {
                        collection.ToSort(elementToSort);
                        Console.WriteLine("Sorted order collection is : \n");
                        Console.WriteLine(collection);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("There is no such attribute in class");
                    }
                    
                }
            }
            else if (inputNum == "6")
            {
                Console.WriteLine("Order collection is : \n");
                Console.WriteLine(collection);
            }
            else if (inputNum == "7")
            {
                break;
            }
        }
    }

    public static void Main(string[] args)
    {
        Menu();
    }
}}

