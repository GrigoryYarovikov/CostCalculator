using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CostCalculator
{
    class CalculateUnit<T>
    {
        public List<IProductComparable<T>> NotCalculatedProducts { get; set; }
        public decimal Cost { get; set; }
    }
}
