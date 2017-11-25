using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var calculator = new CostCalculator();
            var products = new List<Product>
            {

            };
            var cost = calculator.CalculateCost(products);
        }
    }
}
