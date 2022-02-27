using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TransportTaskLibrary
{
    public static class TransportTask
    {
        // Метод расчета опорного плана
        public static double[,] CalculateBasePlan(int[,] deliveryPrices, List<double> reserves, List<double> needs)
        {
            // Подготавливаем матрицу доставки
            double[,] deliveryPlan = new double[reserves.Count, needs.Count];
            for (int i = 0; i < reserves.Count; i++)
                for (int j = 0; j < needs.Count; j++)
                    deliveryPlan[i, j] = double.NaN;

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
                            if (min > deliveryPrices[i, j])
                            {
                                min = deliveryPrices[i, j];
                                indexOfSupplier = i;
                                indexOfConsumer = j;
                            }

                            // Если минимум уже есть, но есть перевозка по той же цене, но в большем количестве
                            if (min == deliveryPrices[i, j]
                                && reserves[indexOfSupplier] - needs[indexOfConsumer] > reserves[i] - needs[j])
                            {
                                min = deliveryPrices[i, j];
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
                    deliveryPlan[indexOfSupplier, indexOfConsumer] = needs[indexOfConsumer];

                    reserves[indexOfSupplier] = reserves[indexOfSupplier] - needs[indexOfConsumer];
                    needs[indexOfConsumer] = double.NaN;
                }
                else
                {
                    deliveryPlan[indexOfSupplier, indexOfConsumer] = reserves[indexOfSupplier];

                    needs[indexOfConsumer] = needs[indexOfConsumer] - reserves[indexOfSupplier];
                    reserves[indexOfSupplier] = double.NaN;
                }
            }

            return deliveryPlan;
        }

        // Метод оптимизации плана доставки, методом потенциалов
        public static void OptimizePlan(int[,] deliveryPrices, double[,] deliveryPlan)
        {
            // U потенциалы
            double[] suppliersPotenial = new double[deliveryPrices.GetLength(0)];
            // V потенциалы
            double[] consumerPotenial = new double[deliveryPrices.GetLength(1)];

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
                    for (int i = 0; i < deliveryPrices.GetLength(0); i++)
                        for (int j = 0; j < deliveryPrices.GetLength(1); j++)
                            if (!double.IsNaN(deliveryPlan[i, j]))
                            {
                                if (double.IsNaN(consumerPotenial[j]))
                                    consumerPotenial[j] = deliveryPrices[i, j] - suppliersPotenial[i];
                                if (double.IsNaN(suppliersPotenial[i]))
                                    suppliersPotenial[i] = deliveryPrices[i, j] - consumerPotenial[j];
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
                double[,] grades = new double[deliveryPrices.GetLength(0), deliveryPrices.GetLength(1)];
                for (int i = 0; i < grades.GetLength(0); i++)
                    for (int j = 0; j < grades.GetLength(1); j++)
                        grades[i, j] = double.NaN;

                for (int i = 0; i < deliveryPrices.GetLength(0); i++)
                    for (int j = 0; j < deliveryPrices.GetLength(1); j++)
                        if (double.IsNaN(deliveryPlan[i, j]))
                            grades[i, j] = deliveryPrices[i, j] - suppliersPotenial[i] - consumerPotenial[j];

                // Ищем минимальную оценку незадействованного маршрута и его индексы
                double min = double.MaxValue;
                int indexOfMinI = 0;
                int indexOfMinJ = 0;
                for (int i = 0; i < grades.GetLength(0); i++)
                    for (int j = 0; j < grades.GetLength(1); j++)
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
                    // Добавляем новый маршрут по месту маршрута с отрицательной оценкой
                    deliveryPlan[indexOfMinI, indexOfMinJ] = 0.0;

                    List<Point> optimizationRoute = FindOptimizationRoute(deliveryPlan, new Point(indexOfMinI, indexOfMinJ));
                    return;
                }
                else
                    return;
            }
        }

        public static List<Point> FindOptimizationRoute(double[,] deliveryPlan, Point startPoint)
        {
            List<Point> optimizationRoute = new List<Point>();

            return optimizationRoute;
        }

        // Метод вычисления стоимости доставки
        public static int CalculatePriceOfDelivery(int[,] deliveryPrices, double[,] deliveryPlan)
        {
            int priceOfTransport = 0;

            for (int i = 0; i < deliveryPrices.GetLength(0); i++)
                for (int j = 0; j < deliveryPrices.GetLength(1); j++)
                    if (!double.IsNaN(deliveryPlan[i, j]))
                        priceOfTransport += deliveryPrices[i, j] * (int)deliveryPlan[i, j];

            return priceOfTransport;
        }
    }
}
