using System;
using System.Collections.Generic;

namespace TestTaskALaboTech.InputFile
{
    public static class InputFile
    {
        public static bool TryParse(this string inputString, out List<int> chips)
        {
            chips = new List<int>();

            if (inputString.StartsWith("chips: [") && inputString.EndsWith(']'))
            {
                inputString = inputString.Replace("chips: [", "");
                inputString = inputString.Replace("]", "");

                string[] inputValues = inputString.Split(", ");

                foreach (var item in inputValues)
                {
                    if (int.TryParse(item, out int value))
                    {
                        chips.Add(value);
                    }
                    else
                    {
                        Console.WriteLine("Incorrect line format");
                        return false;
                    }
                }
                return true;
            }
            else
            {
                Console.WriteLine("Incorrect line format");
                return false;
            }
        }
    }
}