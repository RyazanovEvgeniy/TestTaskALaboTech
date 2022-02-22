using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTaskALaboTech.TransportTask
{
    public static class TransportChips
    {
        // Метод по уравновешиванию системы с помощью метода наименьшей цены
        public static int GetPriceAndTransportChips(List<int> table)
        {
            // Считаем точку равновесия
            int equilibrium = table.Sum() / table.Count;
            // Количество мест, в дальнейшем для расчета
            int quantityPlaces = table.Count;

            // Заводим счетчик перемещения фишек
            int counter = 0;

            while (table.Min() != table.Max())
            {
                //foreach (var item in table)
                //    Console.Write(item + " ");
                //Console.WriteLine();

                // Заводим листы поставщиков и потребителей
                // Первый индекс места за столом, второе кол-во фишек необходимое или излишнее
                List<Tuple<int, int>> suppliers = new List<Tuple<int, int>>();
                List<Tuple<int, int>> consumers = new List<Tuple<int, int>>();

                // Заполняем листы поставщиков и потребителей
                for (int i = 0; i < table.Count; i++)
                {
                    if (table[i] > equilibrium)
                        suppliers.Add(new Tuple<int, int>(i, table[i] - equilibrium));
                    if (table[i] < equilibrium)
                        consumers.Add(new Tuple<int, int>(i, equilibrium - table[i]));
                }

                /*Console.WriteLine("suppliers");
                foreach (var item in suppliers)
                    Console.WriteLine("place: " + item.Item1 + " chips: " + item.Item2);
                Console.WriteLine("consumers");
                foreach (var item in consumers)
                    Console.WriteLine("place: " + item.Item1 + " chips: " + item.Item2);*/

                // Строим матрицу цен транспортировки
                int[,] pricesOfTransport = new int[suppliers.Count, consumers.Count];

                for (int i = 0; i < suppliers.Count; i++)
                {
                    for (int j = 0; j < consumers.Count; j++)
                    {
                        pricesOfTransport[i, j] = Math.Min(
                            Math.Abs(suppliers[i].Item1 - consumers[j].Item1),
                            quantityPlaces - Math.Abs(suppliers[i].Item1 - consumers[j].Item1));
                        //Console.Write(pricesOfTransport[i, j]);
                    }
                    //Console.WriteLine();
                }

                // Ищем перевозку по минимальной стоимости
                int min = int.MaxValue;
                int indexOfSupplier = 0, indexOfConsumer = 0;

                for (int i = 0; i < suppliers.Count; i++)
                {
                    for (int j = 0; j < consumers.Count; j++)
                    {
                        if (min > pricesOfTransport[i, j])
                        {
                            min = pricesOfTransport[i, j];
                            indexOfSupplier = i;
                            indexOfConsumer = j;
                        }
                    }
                }

                //Console.WriteLine("indexOfSupplier: " + indexOfSupplier);
                //Console.WriteLine("indexOfConsumer: " + indexOfConsumer);
                //Console.WriteLine("supplier: " + suppliers[indexOfSupplier].Item1);
                //Console.WriteLine("consumer: " + consumers[indexOfConsumer].Item1);

                // Перемещаем фишку
                table[suppliers[indexOfSupplier].Item1]--;
                table[consumers[indexOfConsumer].Item1]++;

                // Добавляем количество произведенных перемещений
                counter += pricesOfTransport[indexOfSupplier, indexOfConsumer];

                //Console.WriteLine("counter: " + counter);
                //Console.WriteLine("___________________________");
            }

            return counter;
        }

    }
}