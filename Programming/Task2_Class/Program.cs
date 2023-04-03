
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
        string toReadFile = "../../../Order.json";
        OrderCollection collection ;
        Dictionary<string, Delegate> fieldValid;
        fieldValid = ValidDict.ToValidFields();
        collection = WorkWithFile.ReadFromFile(toReadFile);
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
                Order toAdd = new Order();
                toAdd.ToWrite(collection.Count);
                collection.Collection.Add(toAdd);
                collection.Rewrite(toReadFile);
            }
            if (inputNum == "2") // else if
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
                    else
                    {
                        Console.WriteLine("Can't delete unexciting order by id " + getId + "\n");
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
                        Console.WriteLine("Input id of Order to edit: ");
                        string? getId = Console.ReadLine();
                        if (Validation.ValidInt(getId!))
                        {
                            if (collection.FindById(int.Parse(getId!)) != null)
                            {
                                
                                PrintParameter();
                                string? keyElement = Console.ReadLine();
                                if (ToLookDict.ContainsKey(keyElement!))
                                {
                                    int? index = collection.FindById(int.Parse(getId!));
                                    Console.WriteLine($"[{keyElement}] : ");
                                    string? elementToEdit = Console.ReadLine();
                                    var value = fieldValid[ToLookDict[keyElement!]].DynamicInvoke(elementToEdit);
                                    collection.ToEdit((int)index!,ToLookDict[keyElement!],value!);
                                    //collection[(int)index!].GetType().GetProperty(toLookDict[keyElement!])!.SetValue(collection[(int)index], value);
                                    collection.Rewrite(toReadFile);
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("There is no such key");
                                }

                            }
                            else
                            {
                                Console.WriteLine("Can't edit unexciting order by id " + getId + "\n");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"{getId} have to be integer");
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
                while (true)
                {
                    PrintParameter();
                    string? elementToSort = Console.ReadLine();
                    if (ToLookDict.ContainsKey(elementToSort!))
                    {
                        collection.ToSort(ToLookDict[elementToSort!]);
                        Console.WriteLine("Sorted order collection is : \n");
                        Console.WriteLine(collection);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("There is no such paramether!");
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

