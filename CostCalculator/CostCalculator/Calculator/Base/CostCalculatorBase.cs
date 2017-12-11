using System;
using System.Collections.Generic;
using System.Linq;

namespace CostCalculator
{
    abstract class CostCalculatorBase<T, Tindex> where Tindex: struct, IConvertible where T: IComparable
    {
        /// <summary>
        /// represent set of ids used to discount calculation
        /// </summary>
        protected ISaleItemsRepository<T, Tindex> _repository;
        /// <summary>
        /// list of rules used to discount calculation
        /// </summary>
        protected List<Func<CalculateUnit<T>, CalculateUnit<T>>> _rules;

        protected CostCalculatorBase(ISaleItemsRepository<T, Tindex> repository)
        {
            if (!typeof(Tindex).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
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
            if (!ValidateAlphabet())
            {
                throw new Exception("repository do not contains required items");
            }

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
        /// check calculate rules
        /// </summary>
        protected bool ValidateAlphabet()
        {
            foreach (Tindex i in Enum.GetValues(typeof(Tindex)))
            {
                if (_repository.ElementAt(i) == null)
                {
                    return false;
                }
            }
            return true;
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
                useInDiscount.Add(unit.NotCalculatedProducts.FirstOrDefault(x => x.Id.CompareTo(id) == 0));
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
