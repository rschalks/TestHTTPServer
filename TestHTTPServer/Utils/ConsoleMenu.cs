using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LWHTTP.Utils
{
    public class ConsoleMenu
    {
        public static T SelectFromArray<T>(T[] arr, Predicate<T> filter)
        {
            bool selected = false;
            List<T> filteredItems = new List<T>();
            T item = default(T);
            while (!selected)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (filter != null)
                    {
                        if (filter(arr[i]))
                        {
                            filteredItems.Add(arr[i]);
                        }
                    }
                    else
                    {
                        filteredItems.Add(arr[i]);
                    }
                }
                if (filteredItems.Count == 0)
                {
                    return default(T);
                }

                for (int i = 0; i < filteredItems.Count; i++)
                {
                    Console.WriteLine("{0}: {1}", i, filteredItems[i].ToString());
                }

                int input = 0;
                bool valid = false;
                while (!valid)
                {
                    Console.Write(">");
                    if (!int.TryParse(Console.ReadLine(), out input))
                    {
                        Console.WriteLine("Invalid input.");
                        continue;
                    }
                    if (input < 0 || input >= filteredItems.Count)
                    {
                        Console.WriteLine("Input must be 0 or larger, and smaller than or equal to {0}", filteredItems.Count);
                        continue;
                    }
                    valid = true;
                }

                item = filteredItems.ElementAt(input);
                selected = true;
            }
            return item;
        }

        public static Int32 GetInt32()
        {
            return GetInt32(Int32.MinValue, Int32.MaxValue);
        }

        public static Int32 GetInt32(Int32 min, Int32 max)
        {
            Int32 ret = 0;
            bool valid = false;
            while (!valid)
            {
                Console.WriteLine("Insert a number between {0} and {1}.", min.ToString(), max.ToString());

                Console.Write(">");
                if (!Int32.TryParse(Console.ReadLine(), out ret))
                {
                    Console.WriteLine("Invalid input.");
                    continue;
                }

                if (ret < min || ret > max)
                {
                    Console.WriteLine("Input must be between {0} and {1}.", min.ToString(), max.ToString());
                    continue;
                }

                valid = true;
            }

            return ret;
        }
    }
}
