namespace CostCalculator
{
    interface IProductComparable<T>
    {
        T Id { get; set; }
        decimal Cost { get; set; }
        bool IsMatch(T label);
    }
}
