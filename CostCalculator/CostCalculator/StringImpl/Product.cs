namespace CostCalculator
{
    class Product: IProductComparable<string>
    {
        public string Id { get; set; }
        public decimal Cost { get; set; }
    }
}
