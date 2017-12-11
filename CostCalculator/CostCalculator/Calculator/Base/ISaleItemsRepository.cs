namespace CostCalculator
{
    interface ISaleItemsRepository<Tout, Tindex>
    {
        Tout ElementAt(Tindex index);
    }
}
