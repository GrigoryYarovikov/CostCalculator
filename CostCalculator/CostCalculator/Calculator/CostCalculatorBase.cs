using System;
using System.Collections.Generic;
using System.Linq;

namespace CostCalculator
{
    abstract class CostCalculatorBase<T>
    {
        /// <summary>
        /// represent set of ids used to discount calculation
        /// </summary>
        protected ISaleItemsRepository<T> _repository;
        /// <summary>
        /// list of rules used to discount calculation
        /// </summary>
        protected List<Func<CalculateUnit<T>, CalculateUnit<T>>> _rules;

        protected CostCalculatorBase(ISaleItemsRepository<T> repository)
        {
            _repository = repository;
            _rules = new List<Func<CalculateUnit<T>, CalculateUnit<T>>>();
        }

        /// <summary>
        /// Get cost with discount
        /// </summary>
        public decimal CalculateCost(IEnumerable<IProductComparable<T>> products)
        {
            if (products == null)
            {
                throw new ArgumentNullException();
            }

            SetRules();
            var unit = new CalculateUnit<T>
            {
                NotCalculatedProducts = products.ToList()
            };

            foreach (var f in _rules)
            {
                unit = f(unit);
            }

            return unit.Cost + unit.NotCalculatedProducts.Sum(x => x.Cost);
        }

        /// <summary>
        /// set calculate rules
        /// </summary>
        protected abstract void SetRules();

        /// <summary>
        /// get cost with discount by product count
        /// </summary>
        protected CalculateUnit<T> SaleCountProcess(CalculateUnit<T> unit, int count, decimal discount)
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

        /// <summary>
        /// get cost with discount by product ids
        /// </summary>
        protected CalculateUnit<T> SaleGroupProcess(CalculateUnit<T> unit, IEnumerable<T> ids, decimal discount)
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
