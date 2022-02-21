using System;
using System.Collections.Generic;
using System.Linq;
using TestTaskALaboTech.StringExtensions;

namespace TestTaskALaboTech
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Jose set up a circular poker table for his friends,\n" +
                              "so that each of the seats at the table has the same number of poker chips.\n" +
                              "But when Jose wasn’t looking, someone rearranged all of the chips,\n" +
                              "so that they are no longer evenly distributed!\n" +
                              "Now Jose needs to redistribute the chips,\n" +
                              "so that every seat has the same number before his friends arrive.\n" +
                              "But Jose is very meticulous: to ensure that he doesn’t lose any chips in the process,\n" +
                              "he only moves chips between adjacent seats.\n" +
                              "Moreover, he only moves chips one at a time.What is the minimum number of chip moves\n" +
                              "Jose will need to make to bring the chips back to equilibrium?");
            Console.WriteLine("(Input format: 'chips: [1, 5, 9, 10, 5]')");

            while (true)
            {
                Console.WriteLine("Input:");

                // Читаем вводимую строку
                string inputString = Console.ReadLine();

                // Пытаем преобразовать ее в массив интов - кол-во мест с их количеством фишек на этих местах
                if (inputString.TryParse(out List<int> chips))
                {
                    // Проверяем что равновесие возможно в принципе
                    if (chips.Sum() % chips.Count == 0)
                    {
                        // Заводим счетчик
                        int counter = 0;

                        // Пока минимум и максимум в массиве не сравняются уравниваем
                        while (chips.Min() != chips.Max())
                        {
                            // Вычисляем индексы минимума и максимума
                            int indexOfMin = chips.IndexOf(chips.Min());
                            int indexOfMax = chips.IndexOf(chips.Max());
                            // Передаем одну фишку от минимума к максимуму
                            chips[indexOfMax]--;
                            chips[indexOfMin]++;
                            // Наращиваем счетчик
                            counter++;
                        }

                        // Выводим количество проведенных итераций
                        Console.WriteLine("Output:");
                        Console.WriteLine(counter);
                    }
                    else
                        Console.WriteLine("Balance impossible with this numbers. Enter another.");
                }
            }
        }


    }
}