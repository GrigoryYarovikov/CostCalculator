﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace CostCalculator.RulesSet1
{
    class CostCalculatorVer1<T>: CostCalculatorBase<T, Alphabet> where T: IComparable
    {
        public CostCalculatorVer1(ISaleItemsRepository<T, Alphabet> repository): base(repository) { }

        /// <summary>
        /// set calculate rules
        /// </summary>
        protected override void SetRules()
        {
            _rules = new List<Func<CalculateUnit<T>, CalculateUnit<T>>>
            {
                SaleAB,
                SaleDE,
                SaleEFG,
                SaleAKLM,
                Sale4A5Z,
                SaleByCount
            };
        }

        /// <summary>
        /// A && B
        /// 10%
        /// </summary>
        CalculateUnit<T> SaleAB(CalculateUnit<T> unit)
        {
            var ids = new List<T> { _repository.ElementAt(Alphabet.A), _repository.ElementAt(Alphabet.B) };
            var discount = 0.1m;
            return SaleGroupProcess(unit, ids, discount);
        }

        /// <summary>
        /// D && E
        /// 5%
        /// </summary>
        CalculateUnit<T> SaleDE(CalculateUnit<T> unit)
        {
            var ids = new List<T> { _repository.ElementAt(Alphabet.D), _repository.ElementAt(Alphabet.E) };
            var discount = 0.05m;
            return SaleGroupProcess(unit, ids, discount);
        }

        /// <summary>
        /// E && F && G
        /// 5%
        /// </summary>
        CalculateUnit<T> SaleEFG(CalculateUnit<T> unit)
        {
            var ids = new List<T> {
                _repository.ElementAt(Alphabet.E),
                _repository.ElementAt(Alphabet.G),
                _repository.ElementAt(Alphabet.F)
            };
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
            var ids = new List<T> { _repository.ElementAt(Alphabet.A), _repository.ElementAt(Alphabet.K) };
            var result = SaleGroupProcess(unit, ids, discount);

            ids = new List<T> { _repository.ElementAt(Alphabet.A), _repository.ElementAt(Alphabet.L) };
            result = SaleGroupProcess(result, ids, discount);

            ids = new List<T> { _repository.ElementAt(Alphabet.A), _repository.ElementAt(Alphabet.M) };
            return SaleGroupProcess(result, ids, discount);
        }

        /// <summary>
        /// count == 5 || 4 || 3
        /// except A, C
        /// </summary>
        CalculateUnit<T> SaleByCount(CalculateUnit<T> unit)
        {
            var exceptNames = new List<T> { _repository.ElementAt(Alphabet.A), _repository.ElementAt(Alphabet.C) };
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

        /// <summary>
        /// count == 5
        /// 20%
        /// </summary>
        CalculateUnit<T> Sale4A5Z(CalculateUnit<T> unit)
        {
            var discount = 0.08m;
            var ids = new List<T> {
                //4a
                _repository.ElementAt(Alphabet.A),
                _repository.ElementAt(Alphabet.A),
                _repository.ElementAt(Alphabet.A),
                _repository.ElementAt(Alphabet.A),
                //5z
                _repository.ElementAt(Alphabet.Z),
                _repository.ElementAt(Alphabet.Z),
                _repository.ElementAt(Alphabet.Z),
                _repository.ElementAt(Alphabet.Z),
                _repository.ElementAt(Alphabet.Z),
            };
            return SaleGroupProcess(unit, ids, discount);
        }
    }
}
