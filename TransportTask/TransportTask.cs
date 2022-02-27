using System;
using System.Collections.Generic;
using System.Linq;

namespace TransportTaskLibrary
{
    public static class TransportTask
    {
        // Метод расчета опорного плана
        public static double[,] CalculateBasePlan(int[,] pricesOfTransport, List<double> reserves, List<double> needs)
        {
            // Подготавливаем матрицу доставки
            double[,] quantityOfTransport = new double[reserves.Count, needs.Count];
            for (int i = 0; i < reserves.Count; i++)
                for (int j = 0; j < needs.Count; j++)
                    quantityOfTransport[i, j] = double.NaN;

            // Работаем пока не сформируем опорный план
            while (true)
            {
                // Ищем перевозку по минимальной стоимости c максимальной нагрузкой
                // с незаблокированными осями
                int min = int.MaxValue;
                int indexOfSupplier = 0, indexOfConsumer = 0;

                for (int i = 0; i < reserves.Count; i++)
                {
                    for (int j = 0; j < needs.Count; j++)
                    {
                        if (!double.IsNaN(reserves[i])
                            && !double.IsNaN(needs[j]))
                        {
                            if (min > pricesOfTransport[i, j])
                            {
                                min = pricesOfTransport[i, j];
                                indexOfSupplier = i;
                                indexOfConsumer = j;
                            }

                            // Если минимум уже есть, но есть перевозка по той же цене, но в большем количестве
                            if (min == pricesOfTransport[i, j]
                                && reserves[indexOfSupplier] - needs[indexOfConsumer] > reserves[i] - needs[j])
                            {
                                min = pricesOfTransport[i, j];
                                indexOfSupplier = i;
                                indexOfConsumer = j;
                            }
                        }
                    }
                }

                // Если не осталось ячеек со свободными осями выходим из цикла
                if (min == int.MaxValue)
                    break;

                // Добавляем отгрузку на план и доставляем товар
                if (reserves[indexOfSupplier] >= needs[indexOfConsumer])
                {
                    quantityOfTransport[indexOfSupplier, indexOfConsumer] = needs[indexOfConsumer];

                    reserves[indexOfSupplier] = reserves[indexOfSupplier] - needs[indexOfConsumer];
                    needs[indexOfConsumer] = double.NaN;
                }
                else
                {
                    quantityOfTransport[indexOfSupplier, indexOfConsumer] = reserves[indexOfSupplier];

                    needs[indexOfConsumer] = needs[indexOfConsumer] - reserves[indexOfSupplier];
                    reserves[indexOfSupplier] = double.NaN;
                }
                Console.WriteLine("base|min:" + min + " i:" + indexOfSupplier + " j:" + indexOfConsumer + " quantity:" + quantityOfTransport[indexOfSupplier, indexOfConsumer]);
            }

            return quantityOfTransport;
        }

        // Метод вычисления стоимости транспортировки
        public static int CalculatePriceOfTransportation(int[,] pricesOfTransport, double[,] quantityOfTransport, int suppliersCount, int consumersCount)
        {
            int priceOfTransport = 0;

            for (int i = 0; i < suppliersCount; i++)
                for (int j = 0; j < consumersCount; j++)
                    if (!double.IsNaN(quantityOfTransport[i, j]))
                        priceOfTransport += pricesOfTransport[i, j] * (int)quantityOfTransport[i, j];

            return priceOfTransport;
        }

        // Метод оптимизации плана доставки, методом потенциалов
        private static void OptimizePlan(int[,] pricesOfTransport, double[,] quantityOfTransport, int suppliersCount, int consumersCount)
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
                while (suppliersPotenial.Contains(double.NaN) || consumerPotenial.Contains(double.NaN))
                    for (int i = 0; i < suppliersCount; i++)
                        for (int j = 0; j < consumersCount; j++)
                            if (!double.IsNaN(quantityOfTransport[i, j]))
                            {
                                if (double.IsNaN(consumerPotenial[j]))
                                    consumerPotenial[j] = pricesOfTransport[i, j] - suppliersPotenial[i];
                                if (double.IsNaN(suppliersPotenial[i]))
                                    suppliersPotenial[i] = pricesOfTransport[i, j] - consumerPotenial[j];
                            }

                Console.WriteLine("suppliersPotenial");
                for (int i = 0; i < suppliersPotenial.Length; i++)
                    Console.Write(suppliersPotenial[i] + " ");
                Console.WriteLine();

                Console.WriteLine("consumerPotenial");
                for (int i = 0; i < consumerPotenial.Length; i++)
                    Console.Write(consumerPotenial[i] + " ");
                Console.WriteLine();

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

                Console.WriteLine("\nmin:" + min + " i:" + indexOfMinI + " y:" + indexOfMinJ);
                // Если есть маршрут с отрицательной оценкой оптимизируем план
                if (min < 0.0)
                {
                    return;
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
                    if (quantityOfTransport[indexOfHorizontalI, indexOfMinJ] == quantityProduct)
                        quantityOfTransport[indexOfHorizontalI, indexOfMinJ] = double.NaN;
                    else
                        quantityOfTransport[indexOfHorizontalI, indexOfMinJ] -= quantityProduct;
                    // Вторая положительная вершина
                    if (double.IsNaN(quantityOfTransport[indexOfMinI, indexOfMinJ]))
                        quantityOfTransport[indexOfMinI, indexOfMinJ] = quantityProduct;
                    else
                        quantityOfTransport[indexOfMinI, indexOfMinJ] += quantityProduct;
                    // Вторая отрицательная вершина
                    if (quantityOfTransport[indexOfMinI, indexOfVerticalJ] == quantityProduct)
                        quantityOfTransport[indexOfMinI, indexOfVerticalJ] = double.NaN;
                    else
                        quantityOfTransport[indexOfMinI, indexOfVerticalJ] -= quantityProduct;
                }
                else
                    return;
            }
        }
    }
}
