using System;

namespace CostCalculator
{
    interface IProductComparable<T> where T: IComparable
    {
        T Id { get; set; }
        decimal Cost { get; set; }
    }
}
