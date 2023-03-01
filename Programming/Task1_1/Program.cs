using System;
using System.Security.Cryptography.X509Certificates;

public class Program
{
    const int MYCONSTANT1 = 1000;
    const int MYCONSTANT2 = 9999;

    static void Main()
    {
        menu();
    }

    static void menu()
    {
        int k = 0;
        int[] elements = null;
        int quantity = 0;

        while (true)
        {
            Console.WriteLine(@"
----Menu----
1.Input numbers
2.Get result
3.Exit/Quit
            ");
            Console.WriteLine("Choose option");
            string input_num = Console.ReadLine();

            if (input_num == "1")
            {
                Console.WriteLine("\nEnter the quantity of four-digit numbers");
                string input_quantity = Console.ReadLine();
                if (CheckIsInt(input_quantity))
                {
                    quantity = int.Parse(input_quantity);
                    Console.WriteLine("\nEnter four-digit numbers separated by space:");

                    string input_elements = Console.ReadLine();
                    string[] validation_elements = input_elements.Split(' ');
                    if (CheckIntArray(validation_elements))
                    {
                        elements = Array.ConvertAll(input_elements.Split(' '), int.Parse);
                        if (check_input(quantity, elements))
                        {
                            k++;
                        }
                    }
                }
                
            }
            else if (input_num == "2")
            {
                if (k > 0)
                {
                    int[] result = game(elements, quantity);
                    Console.WriteLine(string.Join(", ", result));
                }
                else
                {
                    Console.WriteLine("Firstly input numbers before getting result");
                }
            }
            else if (input_num == "3")
            {
                break;
            }
            else
            {
                Console.WriteLine("\n Not Valid Choice Try again");
            }
        }
        bool CheckIntArray(string[] values)
        {

            int[] arr = new int[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                if (!int.TryParse(values[i], out arr[i]))
                {
                    Console.WriteLine("\n Have to be integer!");
                    return false;
                }
            }

            return true;
        }
    }
    
    
    static bool CheckIsInt(string length)
    {
        if (int.TryParse(length, out int result))
        {
            if (int.Parse(length) < 1)
            {
                Console.WriteLine("Wrong input! Cant be negative or 0!");
                return false;
            }
            return true;
        }
       
        else
        {
            Console.WriteLine("Have to be integer");
            return false;
        }
        
    }
    static bool check_input(int quantity, int[] elements)
    {
        try
        {
            if (quantity < 0)
            {
                throw new ArgumentException();
            }
            if (elements.Length < quantity)
            {
                throw new ArgumentException();
            }
            for (int x = 0; x < elements.Length; x++)
            {
                if (elements[x] < MYCONSTANT1 || elements[x] > MYCONSTANT2)
                {
                    throw new ArgumentException();
                }
            }
            return true;
        }
        catch (ArgumentException)
        {
            Console.WriteLine("Wrong input, enter a positive quantity or a four-digit number within range, or not enough numbers.");
            return false;
        }
    }
    static int[] game(int[] numbers, int size)
    {
        int c4 = 0, c3 = 0, c2 = 0, c1 = 0;
        List<int> answers_reverse = new List<int>();
        List<int> answers_not_reverse = new List<int>();
        List<int> indexes_not_change = new List<int>();
        List<int> indexes = new List<int>();
        List<int> to_answer = new List<int>();

        if (numbers.Length > size)
        {
            Array.Resize(ref numbers, size);
        }

        for (int i = 0; i < numbers.Length; i++)
        {
            c4 = numbers[i] % 10;
            c3 = (numbers[i] / 10) % 10;
            c2 = (numbers[i] / 100) % 10;
            c1 = numbers[i] / 1000;

            if ((c1 + c2) == (c3 + c4))
            {
                int to_append = c2 * 1000 + c1 * 100 + c4 * 10 + c3;
                answers_reverse.Add(to_append);
                indexes.Add(i);
                c4 = 0; c3 = 0; c2 = 0; c1 = 0;
            }
            else
            {
                indexes_not_change.Add(i);
                answers_not_reverse.Add(numbers[i]);
            }
        }

        answers_reverse.Reverse();

        for (int i = 0; i < numbers.Length; i++)
        {
            for (int j = 0; j < indexes_not_change.Count; j++)
            {
                if (i == indexes_not_change[j])
                {
                    to_answer.Insert(i, answers_not_reverse[0]);
                    answers_not_reverse.RemoveAt(0);
                }
            }

            for (int j = 0; j < indexes.Count; j++)
            {
                if (i == indexes[j])
                {
                    if (answers_reverse.Count > 0)
                    {
                        to_answer.Insert(i, answers_reverse[0]);
                        answers_reverse.RemoveAt(0);
                    }
                }
            }
        }

        return to_answer.ToArray();
    }
}
    