using System.Collections.Generic;
using System;

namespace progLab3
{
    public class Menu
    {
        public readonly List<MenuItem> Items;


        public Menu()
        {
            Items = new List<MenuItem>();
        }

        public void Display()
        {
            MenuItem result;

            do
            {
                foreach (var item in Items)
                {
                    Console.WriteLine($"{item.Key}. {item.Label}");
                }

                string key;

                do
                {
                    Console.Write(">");
                    key = Console.ReadLine()?.Trim().ToLower();
                } while (key != null && (key.Length != 1 || Items.Find(search => search.Key == key[0]) == null));

                Console.WriteLine("");

                result = Items.Find(search => key != null && search.Key == key[0]);
            } while (result.Function());
        }
    }
}