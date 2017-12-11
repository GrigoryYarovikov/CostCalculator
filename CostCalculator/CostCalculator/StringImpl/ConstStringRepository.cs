using CostCalculator.RulesSet1;
using System;
using System.Collections.Generic;

namespace CostCalculator
{
    class ConstStringRepository: ISaleItemsRepository<string, Alphabet>
    {
        Dictionary<Alphabet, string> _dict = new Dictionary<Alphabet, string> {
            { Alphabet.A, Constants.A },
            { Alphabet.B, Constants.B },
            { Alphabet.C, Constants.C },
            { Alphabet.D, Constants.D },
            { Alphabet.E, Constants.E },
            { Alphabet.F, Constants.F },
            { Alphabet.G, Constants.G },
            { Alphabet.K, Constants.K },
            { Alphabet.L, Constants.L },
            { Alphabet.M, Constants.M }
        };

        public string ElementAt(Alphabet index)
        {
            string result;
            _dict.TryGetValue(index, out result);
            return result;
        }
    }
}
