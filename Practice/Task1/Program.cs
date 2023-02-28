using System;
namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int k = 0;
            int length = 0;
            int[][] matrix = new int[0][];
            while (true)
            {
                Console.WriteLine(@"
----Menu----
1.Input n for matrix
2.Get result
3.Exit/Quit
            ");
                Console.WriteLine("Choose option");
                string input_num = Console.ReadLine();
                if (input_num == "1")
                {
                    Console.WriteLine("Enter length of matrix (even length)");
                    string length_input = Console.ReadLine();
                    if (CheckIsInt(length_input))
                    {
                        length = int.Parse(length_input);
                   
                        if (CheckInput(length))
                        {
                            k++;
                            matrix = GenerateMatrix(length);
                        }
                    }
                }
                else if (input_num == "2")
                {
                    if (k > 0)
                    {
                        for (int i = 0; i < length; i++)
                        {
                            Console.WriteLine(string.Join(" ", matrix[i]));
                        }
                        k = 0;
                    }
                    else
                    {
                        Console.WriteLine("Firstly input numbers before get result");
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
        }

        static bool CheckInput(int length)
        {
            bool passed = true;
            if (length % 2 == 1)
            {
                passed = false;
                Console.WriteLine("Writen length have to be even!");
            }
            else if (length < 1)
            {
                passed = false;
                Console.WriteLine("Wrong input! Length cant be negative or 0!");
            }
            return passed;
        }

        static bool CheckIsInt(string length)
        {
            if (int.TryParse(length, out int result))
            {
                return true;
            }
            else
            {
                Console.WriteLine("Length have to be integer");
                return false;
            }
        }

        static int[][] GenerateMatrix(int length)
        {
            int[][] matrix = new int[length][];
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i] = new int[length];
                int a = 1;
                int b = length;
                for (int j = 0; j < matrix[0].Length; j++)
                {
                    if (i % 2 == 0)
                    {
                        matrix[i][j] = a;
                        if (a < length)
                        {
                            a++;
                        }
                    }
                    else if (i % 2 == 1)
                    {
                        matrix[i][j] = b;
                        if (b > 1)
                        {
                            b--;
                        }
                    }
                }
            }
            return matrix;
        }
    }
}
