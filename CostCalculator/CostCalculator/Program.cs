//Яровиков Григорий, ПСм-21
//Затраченное время:
//первая версия - 2 часа
//написание тестов и проверка - 1 час
//добавление переносимости (generic) - 1.5 часа
//добовление переносимости (вынос основы калькулятора в отдельный класс) - 0.5 часа

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CostCalculator
{
    class Program
    {
        /// <summary>
        /// тестирование класса калькулятора
        /// </summary>
        static void Main(string[] args)
        {
            var repository = new ConstStringRepository();
            var calculator = new CostCalculatorVer1<string>(repository);
            {//тест A B, сумма до скидки 100, 10%
                var products = new List<Product>
                {
                    new Product { Id = "A", Cost = 40 },
                    new Product { Id = "B", Cost = 60 },
                };
                var cost = calculator.CalculateCost(products);
                Debug.Assert(cost == 90);
            }
            {//тест A B, сумма до скидки 110, 10% [A,B]
                var products = new List<Product>
                {
                    new Product { Id = "Z", Cost = 10 },
                    new Product { Id = "A", Cost = 40 },
                    new Product { Id = "B", Cost = 60 },
                };
                var cost = calculator.CalculateCost(products);
                Debug.Assert(cost == 100);
            }
            {//тест A B, сумма до скидки 200, 10%
                var products = new List<Product>
                {
                    new Product { Id = "A", Cost = 50 },
                    new Product { Id = "B", Cost = 60 },
                    new Product { Id = "A", Cost = 40 },
                    new Product { Id = "B", Cost = 50 },
                };
                var cost = calculator.CalculateCost(products);
                Debug.Assert(cost == 180);
            }
            {//тест D E, сумма до скидки 100, 5%
                var products = new List<Product>
                {
                    new Product { Id = "D", Cost = 40 },
                    new Product { Id = "E", Cost = 60 },
                };
                var cost = calculator.CalculateCost(products);
                Debug.Assert(cost == 95);
            }
            {//тест E F G, сумма до скидки 100, 5%
                var products = new List<Product>
                {
                    new Product { Id = "E", Cost = 40 },
                    new Product { Id = "F", Cost = 25 },
                    new Product { Id = "G", Cost = 35 },
                };
                var cost = calculator.CalculateCost(products);
                Debug.Assert(cost == 95);
            }
            {//тест A [K,L,M], сумма до скидки 100, 5%
                var products = new List<Product>
                {
                    new Product { Id = "A", Cost = 40 },
                    new Product { Id = "K", Cost = 60 },
                };
                var cost = calculator.CalculateCost(products);
                Debug.Assert(cost == 95);
            }
            {//тест A [K,L,M], сумма до скидки 100, 5%
                var products = new List<Product>
                {
                    new Product { Id = "A", Cost = 40 },
                    new Product { Id = "L", Cost = 60 },
                };
                var cost = calculator.CalculateCost(products);
                Debug.Assert(cost == 95);
            }
            {//тест A [K,L,M], сумма до скидки 110, 5% [A,L]
                var products = new List<Product>
                {
                    new Product { Id = "A", Cost = 40 },
                    new Product { Id = "M", Cost = 10 },
                    new Product { Id = "L", Cost = 60 },
                };
                var cost = calculator.CalculateCost(products);
                Debug.Assert(cost == 105);
            }
            {//тест A [K,L,M], сумма до скидки 210, 5% [A,L] 5% [A,M]
                var products = new List<Product>
                {
                    new Product { Id = "A", Cost = 90 },
                    new Product { Id = "A", Cost = 40 },
                    new Product { Id = "M", Cost = 10 },
                    new Product { Id = "L", Cost = 60 },
                    new Product { Id = "O", Cost = 10 },
                };
                var cost = calculator.CalculateCost(products);
                Debug.Assert(cost == 200);
            }
            {//тест количество = 3, сумма до скидки 100, 5%
                var products = new List<Product>
                {
                    new Product { Id = "X", Cost = 40 },
                    new Product { Id = "Y", Cost = 25 },
                    new Product { Id = "Z", Cost = 35 },
                };
                var cost = calculator.CalculateCost(products);
                Debug.Assert(cost == 95);
            }
            {//тест количество = 4, сумма до скидки 100, 10%
                var products = new List<Product>
                {
                    new Product { Id = "U", Cost = 20 },
                    new Product { Id = "X", Cost = 20 },
                    new Product { Id = "Y", Cost = 25 },
                    new Product { Id = "Z", Cost = 35 },
                };
                var cost = calculator.CalculateCost(products);
                Debug.Assert(cost == 90);
            }
            {//тест количество = 5, сумма до скидки 100, 20%
                var products = new List<Product>
                {
                    new Product { Id = "Q", Cost = 10 },
                    new Product { Id = "U", Cost = 10 },
                    new Product { Id = "X", Cost = 20 },
                    new Product { Id = "Y", Cost = 25 },
                    new Product { Id = "Z", Cost = 35 },
                };
                var cost = calculator.CalculateCost(products);
                Debug.Assert(cost == 80);
            }
            {//тест количество = 3, сумма до скидки 120, 5% [X,Y,Z]
                var products = new List<Product>
                {
                    new Product { Id = "A", Cost = 10 },
                    new Product { Id = "C", Cost = 10 },
                    new Product { Id = "X", Cost = 40 },
                    new Product { Id = "Y", Cost = 25 },
                    new Product { Id = "Z", Cost = 35 },
                };
                var cost = calculator.CalculateCost(products);
                Debug.Assert(cost == 115);
            }
            {//тест количество = 5 + 3 = 8, сумма до скидки 200, 20% [Q,U,X,Y,Z] 5% [P,R,S]
                var products = new List<Product>
                {
                    new Product { Id = "Q", Cost = 10 },
                    new Product { Id = "U", Cost = 10 },
                    new Product { Id = "X", Cost = 20 },
                    new Product { Id = "Y", Cost = 25 },
                    new Product { Id = "Z", Cost = 35 },
                    new Product { Id = "P", Cost = 10 },
                    new Product { Id = "R", Cost = 70 },
                    new Product { Id = "S", Cost = 20 },
                };
                var cost = calculator.CalculateCost(products);
                Debug.Assert(cost == 175);
            }
            {//тест A B, количество = 3, сумма до скидки 200, 10% [A,B] 5% [X,Y,Z]
                var products = new List<Product>
                {
                    new Product { Id = "A", Cost = 50 },
                    new Product { Id = "B", Cost = 50 },
                    new Product { Id = "X", Cost = 40 },
                    new Product { Id = "Y", Cost = 25 },
                    new Product { Id = "Z", Cost = 35 },
                };
                var cost = calculator.CalculateCost(products);
                Debug.Assert(cost == 185);
            }
            Console.WriteLine("Tests passed successed. Press any key...");
            Console.Read();
        }
    }
}
