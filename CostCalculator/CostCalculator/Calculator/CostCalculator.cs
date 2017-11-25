using System;
using System.Collections.Generic;
using System.Linq;

namespace CostCalculator
{
    class CostCalculator<T>
    {
        public CostCalculator(ISaleItemsRepository<T> repository)
        {
            _repository = repository;
        }
        ISaleItemsRepository<T> _repository;

        /// <summary>
        /// Подсчитывает стоимость с учетом скидок
        /// </summary>
        public decimal CalculateCost(IEnumerable<IProductComparable<T>> products)
        {
            if (products == null)
            {
                throw new ArgumentNullException();
            }

            var unit = new CalculateUnit<T>
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
        CalculateUnit<T> SaleAB(CalculateUnit<T> unit)
        {
            var ids = new List<T> { _repository.A, _repository.B };
            var discount = 0.1m;
            return SaleGroupProcess(unit, ids, discount);
        }

        /// <summary>
        /// D && E
        /// 5%
        /// </summary>
        CalculateUnit<T> SaleDE(CalculateUnit<T> unit)
        {
            var ids = new List<T> { _repository.D, _repository.E };
            var discount = 0.05m;
            return SaleGroupProcess(unit, ids, discount);
        }

        /// <summary>
        /// E && F && G
        /// 5%
        /// </summary>
        CalculateUnit<T> SaleEFG(CalculateUnit<T> unit)
        {
            var ids = new List<T> { _repository.E, _repository.G, _repository.F };
            var discount = 0.05m;
            return SaleGroupProcess(unit, ids, discount);
        }

        /// <summary>
        /// A && [K || L || M]
        /// 5%
        /// </summary>
        CalculateUnit<T> SaleAKLM(CalculateUnit<T> unit)
        {
            var discount = 0.05m;
            var ids = new List<T> { _repository.A, _repository.K };
            var result = SaleGroupProcess(unit, ids, discount);

            ids = new List<T> { _repository.A, _repository.L };
            result = SaleGroupProcess(result, ids, discount);

            ids = new List<T> { _repository.A, _repository.M };
            return SaleGroupProcess(result, ids, discount);
        }

        /// <summary>
        /// count == 5 || 4 || 3
        /// except A, C
        /// </summary>
        CalculateUnit<T> SaleByCount(CalculateUnit<T> unit)
        {
            var exceptNames = new List<T> { _repository.A, _repository.C };
            var exceptProducts = unit.NotCalculatedProducts.Where(x => exceptNames.Contains(x.Id));
            var filteredUnit = new CalculateUnit<T>
            {
                Cost = unit.Cost,
                NotCalculatedProducts = unit.NotCalculatedProducts.Where(x => !exceptNames.Contains(x.Id)).ToList()
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
        CalculateUnit<T> Sale3(CalculateUnit<T> unit)
        {
            var count = 3;
            var discount = 0.05m;
            return SaleCountProcess(unit, count, discount);
        }

        /// <summary>
        /// count == 4
        /// 10%
        /// </summary>
        CalculateUnit<T> Sale4(CalculateUnit<T> unit)
        {
            var count = 4;
            var discount = 0.1m;
            return SaleCountProcess(unit, count, discount);
        }

        /// <summary>
        /// count == 5
        /// 20%
        /// </summary>
        CalculateUnit<T> Sale5(CalculateUnit<T> unit)
        {
            var count = 5;
            var discount = 0.2m;
            return SaleCountProcess(unit, count, discount);
        }

        CalculateUnit<T> SaleCountProcess(CalculateUnit<T> unit, int count, decimal discount)
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

        CalculateUnit<T> SaleGroupProcess(CalculateUnit<T> unit, IEnumerable<T> ids, decimal discount)
        {
            var useInDiscount = new List<IProductComparable<T>>();
            foreach (var id in ids)
            {
                useInDiscount.Add(unit.NotCalculatedProducts.FirstOrDefault(x => x.IsMatch(id)));
            }
            if (!useInDiscount.Any(x => x == null))
            {
                var costTotal = useInDiscount.Sum(x => x.Cost);
                unit.Cost += costTotal - costTotal * discount;
                unit.NotCalculatedProducts = unit.NotCalculatedProducts.Except(useInDiscount).ToList();
                return SaleGroupProcess(unit, ids, discount);
            }
            return unit;
        }
    }
}
