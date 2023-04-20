namespace ModuleTestWork
{
    public static class Program
    {
        public static void Menu()
        {
            string toReadFile = "../../../Booking.json";
            CollectionBooking collection = new CollectionBooking();
            Dictionary<string, Delegate> fieldValid;
            fieldValid = collection.GetValidFields();
            bool readed = false;
            while (true)
        {
            Console.WriteLine(@"
----Menu----
Input 
1 to Read from file
2 to Search hour
3 to Add
4 to Nothing
5 to Nothing
6 to Print Json
7 to Exit/Quit
                ");
            Console.WriteLine("Choose option");
            string inputNum = Console.ReadLine()!;
            if (inputNum == "1")
            {
                collection.ReadFromFile(toReadFile);
                Console.WriteLine("Order collection is : \n");
                Console.WriteLine(collection);
                readed = true;
            }
            else if (inputNum == "2" && readed) // else if
            {
                List<int> mostBookedHours = CollectionBooking.FindMostBookedHours(collection);
                Console.WriteLine("Hour of max bookings:");
                foreach (int hour in mostBookedHours)
                {
                    Console.WriteLine($"{hour}:00");
                }
            }
            else if (inputNum == "3" && readed)
            {
                Booking toAdd = new Booking();
                toAdd.ToWrite(collection.Collection.Count);
                if (CollectionBooking.CanAddBooking(collection.Collection, toAdd))
                {
                    collection.Append(toAdd);
                }
            }
            else if (inputNum == "4" && readed)
            {
                
            }
            else if (inputNum == "5" && readed)
            {
                
            }
            else if (inputNum == "6" && readed)
            {
                Console.WriteLine("Booking collection is : \n");
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
    }
}

