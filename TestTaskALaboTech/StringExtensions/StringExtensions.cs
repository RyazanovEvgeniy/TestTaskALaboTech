using System;
using System.Collections.Generic;

namespace TestTaskALaboTech
{
    public static class StringExtensions
    {
        // Метод расширения пытающийся разобрать верно ли введена строка
        public static bool TryParse(this string inputString, out List<int> table)
        {
            table = new List<int>();

            // Проверяем начало и конец строки
            if (inputString.StartsWith("chips: [") && inputString.EndsWith(']'))
            {
                // Удаляем начало и конец строки
                inputString = inputString.Replace("chips: [", "");
                inputString = inputString.Replace("]", "");

                // Разделяем строку на подстроки
                string[] inputValues = inputString.Split(", ");

                // Пытаемся разобрать каждый int в подстроке, заполняя ими выходной лист
                foreach (var item in inputValues)
                {
                    if (int.TryParse(item, out int value))
                        table.Add(value);
                    else
                    {
                        Console.WriteLine("Incorrect massive of numbers");
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