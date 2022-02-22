using System;
using System.Collections.Generic;
using System.Linq;

namespace TestTaskALaboTech
{
    public static class Transport
    {
        // Метод по уравновешиванию системы с помощью метода наименьшей цены
        public static int GetPriceOfTransport(List<int> table)
        {
            // Считаем точку равновесия
            int equilibrium = table.Sum() / table.Count;
            // Количество мест, в дальнейшем для расчета
            int quantityPlaces = table.Count;

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

            // Строим матрицу цен транспортировки
            int[,] pricesOfTransport = new int[suppliers.Count, consumers.Count];

            for (int i = 0; i < suppliers.Count; i++)
                for (int j = 0; j < consumers.Count; j++)
                    pricesOfTransport[i, j] = Math.Min(
                        Math.Abs(suppliers[i].Item1 - consumers[j].Item1),
                        quantityPlaces - Math.Abs(suppliers[i].Item1 - consumers[j].Item1));

            // Расчитываем первоначальный опорный план
            double[,] quantityOfTransport = CalculateBasePlan(pricesOfTransport, suppliers, consumers);

            /*for (int i = 0; i < suppliers.Count; i++)
            {
                for (int j = 0; j < consumers.Count; j++)
                {
                    Console.Write(quantityOfTransport[i, j] + " ");
                }
                Console.WriteLine();
            }*/

            // Проверяем оптимален ли план и если нужно оптимизируем
            OptimizePlan(pricesOfTransport, quantityOfTransport, suppliers.Count, consumers.Count);

            /*for (int i = 0; i < suppliers.Count; i++)
            {
                for (int j = 0; j < consumers.Count; j++)
                {
                    Console.Write(quantityOfTransport[i, j] + " ");
                }
                Console.WriteLine();
            }*/

            // Возвращаем цену транспортировки после оптимизиации
            return CalculatePriceOfTransportation(pricesOfTransport, quantityOfTransport, suppliers.Count, consumers.Count);
        }

        // Метод расчета опорного плана
        private static double[,] CalculateBasePlan(int[,] pricesOfTransport, List<Tuple<int, double>> suppliers, List<Tuple<int, double>> consumers)
        {
            // Подготавливаем матрицу доставки
            double[,] quantityOfTransport = new double[suppliers.Count, consumers.Count];
            for (int i = 0; i < suppliers.Count; i++)
                for (int j = 0; j < consumers.Count; j++)
                    quantityOfTransport[i, j] = double.NaN;

            // Работаем пока не сформируем опорный план
            while (true)
            {
                // Ищем перевозку по минимальной стоимости
                // с незаблокированными осями
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

                // Если не осталось ячеек со свободными осями выходим из цикла
                if (min == int.MaxValue)
                    break;

                // Добавляем отгрузку на план и доставляем товар
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
            }

            return quantityOfTransport;
        }

        // Метод вычисления стоимости транспортировки
        private static int CalculatePriceOfTransportation(int[,] pricesOfTransport, double[,] quantityOfTransport, int suppliersCount, int consumersCount)
        {
            int priceOfTransport = 0;

            for (int i = 0; i < suppliersCount; i++)
                for (int j = 0; j < consumersCount; j++)
                    if (!double.IsNaN(quantityOfTransport[i, j]))
                        priceOfTransport += pricesOfTransport[i, j] * (int)quantityOfTransport[i, j];

            return priceOfTransport;
        }

        // Метод оптимизации плана доставки, методом потенциалов
        private static void OptimizePlan(int[,] pricesOfTransport, double[,] quantityOfTransport,  int suppliersCount, int consumersCount)
        {
            // U потенциалы
            double[] suppliersPotenial = new double[suppliersCount];
            // V потенциалы
            double[] consumerPotenial = new double[consumersCount];

            while (true)
            {
                // Подготавливаем массивы под итерацию цикла
                for (int i = 0; i < suppliersPotenial.Length; i++)
                    suppliersPotenial[i] = double.NaN;
                for (int i = 0; i < consumerPotenial.Length; i++)
                    consumerPotenial[i] = double.NaN;
                // U0 = 0 - опорный потенциал
                suppliersPotenial[0] = 0;

                // Вычисляем потенциалы
                for (int i = 0; i < suppliersCount; i++)
                    for (int j = 0; j < consumersCount; j++)
                        if (!double.IsNaN(quantityOfTransport[i, j]))
                        {
                            if (double.IsNaN(consumerPotenial[j]))
                                consumerPotenial[j] = pricesOfTransport[i, j] - suppliersPotenial[i];
                            if (double.IsNaN(suppliersPotenial[i]))
                                suppliersPotenial[i] = pricesOfTransport[i, j] - consumerPotenial[j];
                        }

                // Оценка не задействованных маршрутов
                // Цена доставки минус сумма потенциалов незадействованного маршрута
                double[,] grades = new double[suppliersCount, consumersCount];
                for (int i = 0; i < suppliersCount; i++)
                    for (int j = 0; j < consumersCount; j++)
                        grades[i, j] = double.NaN;

                for (int i = 0; i < suppliersCount; i++)
                    for (int j = 0; j < consumersCount; j++)
                        if (double.IsNaN(quantityOfTransport[i, j]))
                            grades[i, j] = pricesOfTransport[i, j] - suppliersPotenial[i] - consumerPotenial[j];

                // Ищем минимальную оценку незадействованного маршрута и его индексы
                double min = double.MaxValue;
                int indexOfMinI = 0;
                int indexOfMinJ = 0;
                for (int i = 0; i < suppliersCount; i++)
                    for (int j = 0; j < consumersCount; j++)
                        if (min > grades[i, j])
                        {
                            min = grades[i, j];
                            indexOfMinI = i;
                            indexOfMinJ = j;
                        }

                // Если есть маршрут с отрицательной оценкой оптимизируем план
                if (min < 0.0)
                {
                    // Меняем 4 вершины прямоугольника против часовой стрелки
                    // Ищем вершину по горизонтали от незадействованного маршрута с низкой оценкой
                    int indexOfHorizontalI = 0;
                    for (int i = 0; i < suppliersCount; i++)
                    {
                        if (!double.IsNaN(quantityOfTransport[i, indexOfMinJ]))
                        {
                            indexOfHorizontalI = i;
                            break;
                        }
                    }
                    // Ищем вершину по вертикали от незадействованного маршрута с низкой оценкой
                    int indexOfVerticalJ = 0;
                    for (int j = 0; j < consumersCount; j++)
                    {
                        if (!double.IsNaN(quantityOfTransport[indexOfMinI, j]))
                        {
                            indexOfVerticalJ = j;
                            break;
                        }
                    }

                    // Определяем количество продукта, которое можем переместить по прямоугольнику
                    double quantityProduct = Math.Min(
                        quantityOfTransport[indexOfHorizontalI, indexOfMinJ], 
                        quantityOfTransport[indexOfMinI, indexOfVerticalJ]);

                    // Перемещаем продукты по прямоугольнику
                    // Первая положительная вершина
                    if (double.IsNaN(quantityOfTransport[indexOfHorizontalI, indexOfVerticalJ]))
                        quantityOfTransport[indexOfHorizontalI, indexOfVerticalJ] = quantityProduct;
                    else
                        quantityOfTransport[indexOfHorizontalI, indexOfVerticalJ] += quantityProduct;
                    // Первая отрицательная вершина
                    quantityOfTransport[indexOfHorizontalI, indexOfMinJ] -= quantityProduct;
                    // Вторая положительная вершина
                    if (double.IsNaN(quantityOfTransport[indexOfMinI, indexOfMinJ]))
                        quantityOfTransport[indexOfMinI, indexOfMinJ] = quantityProduct;
                    else
                        quantityOfTransport[indexOfMinI, indexOfMinJ] += quantityProduct;
                    // Вторая отрицательная вершина
                    quantityOfTransport[indexOfMinI, indexOfVerticalJ] -= quantityProduct;
                }
                else
                    return;
            }
        }
    }
}