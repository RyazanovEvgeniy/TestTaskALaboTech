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


            //foreach (var item in table)
            //    Console.Write(item + " ");
            //Console.WriteLine();

            // Заводим листы поставщиков и потребителей
            // Первый индекс места за столом, второе кол-во фишек необходимое или излишнее
            List<Tuple<int, double>> suppliers = new List<Tuple<int, double>>();
            List<Tuple<int, double>> consumers = new List<Tuple<int, double>>();

            // Заполняем листы поставщиков и потребителей
            for (int i = 0; i < table.Count; i++)
            {
                if (table[i] > equilibrium)
                    suppliers.Add(new Tuple<int, double>(i, table[i] - equilibrium));
                if (table[i] < equilibrium)
                    consumers.Add(new Tuple<int, double>(i, equilibrium - table[i]));
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
                for (int j = 0; j < consumers.Count; j++)
                    pricesOfTransport[i, j] = Math.Min(
                        Math.Abs(suppliers[i].Item1 - consumers[j].Item1),
                        quantityPlaces - Math.Abs(suppliers[i].Item1 - consumers[j].Item1));

            // Подготавливаем матрицу доставки
            double[,] quantityOfTransport = new double[suppliers.Count, consumers.Count];
            for (int i = 0; i < suppliers.Count; i++)
                for (int j = 0; j < consumers.Count; j++)
                    quantityOfTransport[i, j] = double.NaN;

            // Работаем пока не сформируем опорный план
            while (true)
            {
                // Ищем перевозку по минимальной стоимости
                // С незаблокированной осью
                int min = int.MaxValue;
                int indexOfSupplier = 0, indexOfConsumer = 0;

                for (int i = 0; i < suppliers.Count; i++)
                {
                    for (int j = 0; j < consumers.Count; j++)
                    {
                        if (min > pricesOfTransport[i, j] 
                            && !double.IsNaN(suppliers[i].Item2)
                            && !double.IsNaN(consumers[j].Item2))
                        {
                            min = pricesOfTransport[i, j];
                            indexOfSupplier = i;
                            indexOfConsumer = j;
                        }
                    }
                }

                if (min == int.MaxValue)
                    break;

                Console.WriteLine("indexOfSupplier == " + indexOfSupplier);
                Console.WriteLine("indexOfConsumer == " + indexOfConsumer);
                Console.WriteLine("quantitySup == " + suppliers[indexOfSupplier].Item2);
                Console.WriteLine("quantityCon == " + consumers[indexOfConsumer].Item2);

                if (suppliers[indexOfSupplier].Item2 >= consumers[indexOfConsumer].Item2)
                {
                    quantityOfTransport[indexOfSupplier, indexOfConsumer] = consumers[indexOfConsumer].Item2;

                    suppliers[indexOfSupplier] = new Tuple<int, double>
                        (suppliers[indexOfSupplier].Item1, 
                         suppliers[indexOfSupplier].Item2 - consumers[indexOfConsumer].Item2);
                    consumers[indexOfConsumer] = new Tuple<int, double>
                        (consumers[indexOfConsumer].Item1,
                         double.NaN);
                }
                else
                {
                    quantityOfTransport[indexOfSupplier, indexOfConsumer] = suppliers[indexOfSupplier].Item2;

                    consumers[indexOfConsumer] = new Tuple<int, double>
                        (consumers[indexOfConsumer].Item1,
                         consumers[indexOfConsumer].Item2 - suppliers[indexOfSupplier].Item2);
                    suppliers[indexOfSupplier] = new Tuple<int, double>
                        (suppliers[indexOfSupplier].Item1,
                         double.NaN);
                }

                Console.WriteLine("suppliers");
                foreach (var item in suppliers)
                    Console.WriteLine("place: " + item.Item1 + " chips: " + item.Item2);
                Console.WriteLine("consumers");
                foreach (var item in consumers)
                    Console.WriteLine("place: " + item.Item1 + " chips: " + item.Item2);
            }

            int priceOfTransport = 0;

            for (int i = 0; i < suppliers.Count; i++)
            {
                for (int j = 0; j < consumers.Count; j++)
                {
                    Console.Write(quantityOfTransport[i, j]);

                    if (!double.IsNaN(quantityOfTransport[i, j]))
                        priceOfTransport += pricesOfTransport[i, j] * (int)quantityOfTransport[i, j];
                }
                Console.WriteLine();
            }

            return priceOfTransport;
        }

    }
}