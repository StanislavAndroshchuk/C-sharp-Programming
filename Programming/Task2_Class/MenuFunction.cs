using System.Reflection;
using System.Text.Json;

namespace Task2_Class
{
    public static class MenuFunction
    {
        public static User Registration()
        {
            string fileName = "../../../users.json";
            User register = new User();
            try
            {
                List<User> users = ReadFromFileUsers(fileName);
                Console.WriteLine("--Registration--");
                PropertyInfo[] properties = register.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name != "Role")
                    {
                        string input;
                        while(true)
                        {
                        
                            Console.WriteLine("Enter value for " + property.Name + " : ");
                            input = Console.ReadLine()!;
                            try
                            {
                                object valid;
                                if (property.Name == "Email")
                                {
                                
                                    valid =  Validation.ValidEmail(input);
                                    if (users.Any(s => s.Email == (string)valid))
                                    {
                                        throw new Exception("This email already used!");
                                        
                                    }
                                    break;
                                }
                                if (property.Name == "FirstName" || property.Name == "LastName")
                                {
                                
                                    Validation.ValidString(input);
                                    break;
                                }
                                if (property.Name == "Password")
                                {
                                    if (Validation.IsValidPassword(input))
                                    {
                                        break;
                                    }
                                }
                                
                            }
                            catch(Exception err)
                            {
                                Console.WriteLine(err.Message);
                            }
                        }
                        object value = Convert.ChangeType(input, property.PropertyType);
                        property.SetValue(register, value);
                    }
                    else
                    {
                        property.SetValue(register, Roles.Customer);
                    }
                }
                users.Add(register);
                WriteToFileUsers(fileName,users);
            }
            
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return register;
        }

        public static User Login()
        {
            User login = new User();
            string fileName = "../../../users.json";
            try
            {
                List<User> users = ReadFromFileUsers(fileName);
                bool continuer = true;
                while (continuer)
                {
                    Console.WriteLine("Input email :");
                    string email = Console.ReadLine()!;
                    Console.WriteLine("Input password :");
                    string password = Console.ReadLine()!;
                    var userLogin = users.FirstOrDefault(s => s.Email == email && s.Password == password);
                    if (userLogin == null)
                    {
                        Console.WriteLine("Invalid email or password");
                        while (true)
                        {
                            Console.WriteLine("Try Again , y - yes, n - no");
                            string check = Console.ReadLine()!;
                            if (check == "y")
                            {
                                break;
                            }
                            if (check == "n")
                            {
                                continuer = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        login = userLogin;
                        continuer = false;
                    }
                }
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
            }
            return login; 
        }

        private static List<User> ReadFromFileUsers(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new Exception("There is no such file name");
            }
            //else
            string jsonString = File.ReadAllText(filepath);
            var options = new JsonSerializerOptions
            {
                Converters =
                {
                    new EnumIgnoreCaseConverter<Roles>()
                }
            };
            List<User>? userData = JsonSerializer.Deserialize <List<User>>(jsonString, options);
            if (userData != null)
            {
                return userData;
            }
            //else
            throw new Exception("Cant read this file, list is empty");
        }

        private static void WriteToFileUsers(string filepath, List<User> users)
        {
            if (!File.Exists(filepath))
            {
                throw new Exception("There is no such file path");
            }
            else
            {
                var options = new JsonSerializerOptions { WriteIndented = true, Converters =
                {
                    new EnumIgnoreCaseConverter<Roles>()
                } };
                string jsonString = JsonSerializer.Serialize(users, options);
                File.WriteAllText(filepath, jsonString);
            }
        }
    }
}