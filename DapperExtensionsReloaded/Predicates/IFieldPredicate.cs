namespace DapperExtensions.Predicates
{
    public interface IFieldPredicate : IComparePredicate
    {
        object Value { get; set; }
    }
}