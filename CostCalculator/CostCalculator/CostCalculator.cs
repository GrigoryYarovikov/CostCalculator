using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostCalculator
{
    class CostCalculator
    {
        public decimal CalculateCost(IEnumerable<Product> products)
        {
            if (products == null)
            {
                throw new ArgumentNullException();
            }

            var unit = new CalculateUnit
            {
                NotCalculatedProducts = products.ToList()
            };

            var result = SaleAB(unit);
            result = SaleDE(result);
            result = SaleEFG(result);
            result = SaleAKLM(result);
            result = SaleByCount(result);

            return result.Cost + result.NotCalculatedProducts.Sum(x => x.Cost);
        }

        /// <summary>
        /// A && B
        /// 10%
        /// </summary>
        CalculateUnit SaleAB(CalculateUnit unit)
        {
            var names = new List<string> { Constants.A, Constants.B };
            var discount = 0.1m;
            return SaleGroupProcess(unit, names, discount);
        }

        /// <summary>
        /// D && E
        /// 5%
        /// </summary>
        CalculateUnit SaleDE(CalculateUnit unit)
        {
            var names = new List<string> { Constants.D, Constants.E };
            var discount = 0.05m;
            return SaleGroupProcess(unit, names, discount);
        }

        /// <summary>
        /// E && F && G
        /// 5%
        /// </summary>
        CalculateUnit SaleEFG(CalculateUnit unit)
        {
            var names = new List<string> { Constants.E, Constants.G, Constants.F };
            var discount = 0.05m;
            return SaleGroupProcess(unit, names, discount);
        }

        /// <summary>
        /// A && [K || L || M]
        /// 5%
        /// </summary>
        CalculateUnit SaleAKLM(CalculateUnit unit)
        {
            var discount = 0.05m;
            var names = new List<string> { Constants.A, Constants.K };
            var result = SaleGroupProcess(unit, names, discount);

            names = new List<string> { Constants.A, Constants.L };
            result = SaleGroupProcess(result, names, discount);

            names = new List<string> { Constants.A, Constants.M };
            return SaleGroupProcess(result, names, discount);
        }

        /// <summary>
        /// count == 5 || 4 || 3
        /// except A, C
        /// </summary>
        CalculateUnit SaleByCount(CalculateUnit unit)
        {
            var exceptNames = new List<string> { Constants.A, Constants.C };
            var exceptProducts = unit.NotCalculatedProducts.Where(x => exceptNames.Contains(x.Name));
            var filteredUnit = new CalculateUnit
            {
                Cost = unit.Cost,
                NotCalculatedProducts = unit.NotCalculatedProducts.Where(x => !exceptNames.Contains(x.Name)).ToList()
            };
            var result = Sale5(filteredUnit);
            result = Sale4(result);
            result = Sale3(result);

            result.NotCalculatedProducts.AddRange(exceptProducts);
            return result;
        }

        /// <summary>
        /// count == 3
        /// 5%
        /// </summary>
        CalculateUnit Sale3(CalculateUnit unit)
        {
            var count = 3;
            var discount = 0.05m;
            return SaleCountProcess(unit, count, discount);
        }

        /// <summary>
        /// count == 4
        /// 10%
        /// </summary>
        CalculateUnit Sale4(CalculateUnit unit)
        {
            var count = 4;
            var discount = 0.1m;
            return SaleCountProcess(unit, count, discount);
        }

        /// <summary>
        /// count == 5
        /// 20%
        /// </summary>
        CalculateUnit Sale5(CalculateUnit unit)
        {
            var count = 5;
            var discount = 0.2m;
            return SaleCountProcess(unit, count, discount);
        }

        CalculateUnit SaleCountProcess(CalculateUnit unit, int count, decimal discount)
        {
            if (unit.NotCalculatedProducts.Count >= count)
            {
                var costTotal = unit.NotCalculatedProducts.Take(count).Sum(x => x.Cost);
                unit.Cost += costTotal - costTotal * discount;
                unit.NotCalculatedProducts.RemoveRange(0, count);
                return SaleCountProcess(unit, count, discount);
            }
            return unit;
        }

        CalculateUnit SaleGroupProcess(CalculateUnit unit, IEnumerable<string> names, decimal discount)
        {
            var useInDiscount = new List<Product>();
            foreach (var name in names)
            {
                useInDiscount.Add(unit.NotCalculatedProducts.FirstOrDefault(x => x.Name == name));
            }
            if (!useInDiscount.Any(x => x == null))
            {
                var costTotal = useInDiscount.Sum(x => x.Cost);
                unit.Cost += costTotal - costTotal * discount;
                unit.NotCalculatedProducts = unit.NotCalculatedProducts.Except(useInDiscount).ToList();
                return SaleGroupProcess(unit, names, discount);
            }
            return unit;
        }
    }
}
