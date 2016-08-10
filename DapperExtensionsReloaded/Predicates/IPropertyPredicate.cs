namespace DapperExtensions.Predicates
{
    public interface IPropertyPredicate : IComparePredicate
    {
        string PropertyName2 { get; set; }
    }
}