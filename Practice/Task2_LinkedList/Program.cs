using System;
using System.ComponentModel;
using System.Runtime.Intrinsics.Arm;
using System.Security;

namespace ConsoleApp;

public class Program
{
    static void Main()
    {
        int k = 0;
        bool passed = false;
        LinkedList get_list = new LinkedList();
        while (true)
        {
            Console.WriteLine(@"
--Choose--
1.Input list from keyboard
2.Get random list");
            Console.WriteLine("Choose action");
            string option = Console.ReadLine();
            if (option == "1")
            {
                Console.Write("Input size of list : ");
                string size = Console.ReadLine();
                int size_int;
                if (CheckIsInt(size, out size_int))
                {
                    if (CheckIsPositive(size))
                    {
                        for (int i = 0; i < size_int; i++)
                        {
                            Console.Write($"{i + 1}: ");
                            string x = Console.ReadLine();
                            int x_int;
                            if (CheckForList(x, out x_int))
                            {
                                get_list.append(new Node(x_int));
                                passed = true;
                            }
                            else
                            {
                                passed = false;
                                break;
                            }
                        }
                        if (passed)
                        {
                            //Console.Write($"Your list: {get_list}");
                            Console.Write("Your list: ");
                            get_list.Print();
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Wrong input!");
                    }
                }
            }
            else if (option == "2")
            {
                Console.Write("Input size of list: ");
                string size = Console.ReadLine();
                Console.Write("Lower bound: ");
                string a = Console.ReadLine();
                Console.Write("Upper bound: ");
                string b = Console.ReadLine();
                int size_int, a_int, b_int;
                if (CheckIsInt(size, out size_int) && CheckIsInt(a, out a_int) && CheckIsInt(b, out b_int))
                {
                    if (CheckIsPositive(size) && a_int < b_int)
                    {
                        get_list.RandomArray(size_int, a_int, b_int);
                        Console.Write($"Your list: ");
                        get_list.Print();
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Wrong input!");
                    }
                }
            }
            else
            {
                Console.WriteLine("There is no such option!");
            }
        }
        while (true)
        {
            Console.WriteLine(@"
--Choose--
1.Add item at k position
2.Delete item at k position
3.Use method
4.Exit");
            Console.WriteLine("Choose option");
            string input_num = Console.ReadLine();
            if (input_num == "1")
            {
                Console.WriteLine("Input k position to add item");
                string k3 = Console.ReadLine();
                Console.WriteLine("What to add at this position?");
                string x3 = Console.ReadLine();
                int k_int,x_int;
                if (CheckIsInt(x3, out x_int) && CheckIsInt(k3, out k_int)){
                    if (CheckIsPositive(k3) && CheckIsPositive(x3))
                    {
                        get_list.insert(k_int,new Node(x_int));
                        Console.WriteLine("Your list: ");
                        get_list.Print();
                    }
                }
            }
            else if (input_num == "2")
            {
                Console.WriteLine("Input k position to delete");
                string k2 = Console.ReadLine();
                int k_int;
                if (CheckIsInt(k2, out k_int))
                {
                    if (CheckIsPositive(k2))
                    {
                        get_list.delete(k_int);
                        Console.WriteLine("Your list: ");
                        get_list.Print();
                    }
                }
            }
            else if (input_num == "3")
            {
                get_list.game();
                Console.WriteLine("Your list: ");
                get_list.Print();
            }
            else if (input_num == "4")
            {
                break;
            }
            else
            {
                Console.WriteLine(@"Not valid choice try again");
            }
        }

        }
    static int LOWER_CONST = 1000;
    static int Upper_CONST = 9999;

    static bool CheckIsPositive(string x)
    {
        if (int.Parse(x) >= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    static bool CheckIsInt(string x, out int result)
    {
        if (int.TryParse(x, out result))
        {
            return true;
        }
        else
        {
            Console.WriteLine($"{x} have to be integer");
            return false;
        }
    }

    static bool CheckForList(string x, out int result)
    {
        if (int.TryParse(x, out result))
        {
            if (LOWER_CONST <= result && result <= Upper_CONST)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Incorrect input!");
                return false;
            }
        }
        else
        {
            Console.WriteLine($"{x} have to be integer");
            return false;
        }
    }
    public class Node
    {
        public int value;
        public Node next;

        public Node(int value)
        {
            this.value = value;
            this.next = null;
        }
    }
    public class LinkedList
    {
        public Node head;
        public void Print()
        {
            Node current = head;
            while (current != null)
            {
                Console.Write(current.value + " ");
                current = current.next;
            }
        }
        public override string ToString()
        {
            Node current = head;
            while (current != null)
            {
                Console.Write(current.value + " ");
                current = current.next;
            }
            return base.ToString();
        }
        public void append(Node new_node) {
            Node current = head;
            if (current != null)
            {
                while (current.next != null)
                {
                    current = current.next;
                }
                current.next = new_node;
            }
            else
            {
                head = new_node;
            }
        }
        public void delete(int index)
        {
            Node current = this.head;
            int count = 1;
            if (index == 1)
            {
                head = current.next;
            }
            else
            {
                Node prev = null;
                while (current != null)
                {
                    if (index == count)
                    {
                        break;
                    }
                    prev = current;
                    current = current.next;
                    count++;
                }
                if (current == null)
                {
                    return;
                }
                if (prev != null)
                {
                    prev.next = current.next;
                }
                current = null;
            }
        }
        public void insert(int index, Node node)
        {
            int count = 1;
            Node curre = this.head;
            if (index == 1)
            {
                node.next = this.head;
                this.head = node;
            }
            while (curre != null)
            {
                if ((count+1) == index)
                {
                    node.next = curre.next;
                    curre.next = node;
                    break;
                }
                else
                {
                    count++;
                    curre = curre.next;
                }

            }
            { }
        }
        public void game()
        {
            int c4 = 0;
            int c3 = 0;
            int c2 = 0;
            int c1 = 0;
            List<int> numbers = new List<int>();
            Node current = head;
            while (current != null)
            {
                numbers.Add(current.value);
                current = current.next;
            }
            List<int> answersReverse = new List<int>();
            List<int> answersNotReverse = new List<int>();
            List<int> indexesNotChange = new List<int>();
            List<int> indexes = new List<int>();
            List<int> toAnswer = new List<int>();
            for (int i = 0; i < numbers.Count; i++)
            {
                c4 = numbers[i] % 10;
                c3 = (int)(numbers[i] / 10 % 10);
                c2 = (int)(numbers[i] / 100 % 10);
                c1 = (int)(numbers[i] / 1000);
                if ((c1 + c2) == (c3 + c4))
                {
                    int toAppend = c2 * 1000 + c1 * 100 + c4 * 10 + c3;
                    answersReverse.Add(toAppend);
                    indexes.Add(i);
                    c4 = 0;
                    c3 = 0;
                    c2 = 0;
                    c1 = 0;
                }
                else
                {
                    indexesNotChange.Add(i);
                    answersNotReverse.Add(numbers[i]);
                }
            }
            answersReverse.Reverse();

            for (int i = 0; i < numbers.Count; i++)
            {
                for (int j = 0; j < indexesNotChange.Count; j++)
                {
                    if (i == indexesNotChange[j])
                    {
                        toAnswer.Insert(i, answersNotReverse[0]);
                        answersNotReverse.RemoveAt(0);
                    }
                }
                for (int j = 0; j < indexes.Count; j++)
                {
                    if (i == indexes[j])
                    {
                        if (answersReverse.Count > 0)
                        {
                            toAnswer.Insert(i, answersReverse[0]);
                            answersReverse.RemoveAt(0);
                        }
                    }
                }
            }
            while (head != null)
            {
                Node temp = head;
                head = head.next;
                temp = null;
            }
            for (int i = 0; i < toAnswer.Count; i++)
            {
                Node newNode = new Node(toAnswer[i]);
                current = head;
                if (current != null)
                {
                    while (current.next != null)
                    {
                        current = current.next;
                    }
                    current.next = newNode;
                }
                else
                {
                    head = newNode;
                }
            }

        }
        public void RandomArray(int size, int lowerBound, int upperBound)
        {
            Random random = new Random();
            for (int g = 0; g < size; g++)
            {
                int x = random.Next(lowerBound, upperBound + 1);
                Node newNode = new Node(x);
                Node current = head;
                if (current != null)
                {
                    while (current.next != null)
                    {
                        current = current.next;
                    }
                    current.next = newNode;
                }
                else
                {
                    head = newNode;
                }
            }
        }
    }
}