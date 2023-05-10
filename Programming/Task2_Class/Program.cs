
namespace Task2_Class
{
    public static class Program
    {
        private static void Menu()
        {
            Logging:
            User currentUser = new User();
            OrderCollection<Order> collection = new OrderCollection<Order>();
            string pathToCollection = "../../../Order.json";
            Console.Clear();
            Console.WriteLine("--Menu--");
            Console.WriteLine("1.Sign in");
            Console.WriteLine("2.Sign up");
            Console.WriteLine("3.Exit");
            Console.Write($"? : ");
            string input = Console.ReadLine()!;
            if (input == "1")
            {
                currentUser = MenuFunction.Login();
                if (currentUser.HasDefaultValues())
                {
                    goto Logging;
                }
            }

            else if (input == "2")
            {
                currentUser = MenuFunction.Registration();
            }
            else if (input != "3")
            {
                goto Logging;
            }
            else
            {
                return;
            }
            collection.user = currentUser;
            collection.ReadFromFile(pathToCollection);
            LoggerProxy<Order> userProxy =
                new LoggerProxy<Order>(currentUser, new PermissionProxy<Order>(currentUser, collection));
            Console.Clear();
            string option;
            while (true)
            {
                Console.WriteLine("\n---Menu---");
                Console.WriteLine("1.View list");
                Console.WriteLine("2.View list by id");
                Console.WriteLine("3.Sort list");
                Console.WriteLine("4.Search in list");
                Console.WriteLine("5.Delete element in list");
                Console.WriteLine("6.Add element in list");
                Console.WriteLine("7.Edit element in list");
                Console.WriteLine("8.Publish");  
                Console.WriteLine("9.Sign out");   
                

                Console.Write("? : ");
                option = Console.ReadLine()!;
                Console.Clear();
                try
                {
                    if (option == "1")
                    {
                        var item = userProxy.ViewList();
                        Console.WriteLine("Your list:");
                        foreach (var element in item)
                        {
                            Console.WriteLine(element);
                        }
                    }
                    else if (option == "2")
                    {
                        Console.Write("Id : ");
                        string line = Console.ReadLine()!;
                        int id = Validation.ValidPositiveInt(line);
                        var item = userProxy.ViewById(id);
                        Console.WriteLine($"Found by id {id}");
                        Console.WriteLine(item);
                    }
                    else if (option == "3")
                    {
                        Console.Write("Sort by attribute : ");
                        string attribute = Console.ReadLine()!;
                        var item = userProxy.Sort(attribute);
                        Console.WriteLine("Sorted List:");
                        foreach (var element in item)
                        {
                            Console.WriteLine(element);
                        }

                    }
                    else if (option == "4")
                    {
                        Console.Write("Search by: ");
                        string attribute = Console.ReadLine()!;
                        var item = userProxy.Search(attribute);
                        Console.WriteLine("--Found element(s)--:");
                        foreach (var element in item)
                        {
                            Console.WriteLine(element);
                        }
                    }

                    if (option == "5")
                    {
                        Console.Write("Delete by id: ");
                        string line = Console.ReadLine()!;
                        int id = Validation.ValidPositiveInt(line);
                        userProxy.Delete(id);
                        collection.Rewrite(pathToCollection);
                    }
                    else if (option == "6")
                    {
                        Console.WriteLine("Input new element : ");
                        Order item = new Order();
                        item.ToWrite(collection.Count);
                        userProxy.Create(item);
                        collection.Rewrite(pathToCollection);
                    }
                    else if (option == "7")
                    {
                        var validFields = collection.GetValidFields();
                        Console.Write("Id element to edit: ");
                        string id = Console.ReadLine()!;
                        int validId = Validation.ValidPositiveInt(id);
                        Console.Write("Attribute of element to edit: ");
                        string attribute = Console.ReadLine()!;
                        Console.Write("Value to change: ");
                        string value = Console.ReadLine()!;
                        if (validFields.ContainsKey(attribute))
                        {
                            try
                            {
                                if (attribute == "ShippedDate")
                                {
                                    var validValue = validFields[attribute].DynamicInvoke(value,
                                        collection.Collection[validId].GetType().GetProperty("OrderDate"));
                                    userProxy.Edit(validId, attribute, validValue!);
                                }
                                else
                                {
                                    var validValue = validFields[attribute].DynamicInvoke(value);
                                    userProxy.Edit(validId, attribute, validValue!);
                                }

                                collection.Rewrite(pathToCollection);
                            }
                            catch (ArgumentOutOfRangeException)
                            {
                                Console.WriteLine("Incorrect id input!");
                            }
                            catch (Exception er)
                            {
                                Console.WriteLine(er.Message);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Incorrect attribute name of element!");
                        }

                    }
                    else if (option == "8")
                    {
                        Console.Write("Id: ");
                        string id = Console.ReadLine()!;
                        int Id;
                        try
                        {
                            Id = Validation.ValidPositiveInt(id);
                            userProxy.Publishing(Id);
                            collection.Rewrite(pathToCollection);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                    else if (option == "9")
                    {
                        goto Logging;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                

            }
        }
    
    public static void Main(string[] args)
    {
        Menu();
    }
}}

