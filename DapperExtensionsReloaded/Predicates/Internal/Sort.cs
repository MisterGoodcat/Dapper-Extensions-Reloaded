namespace DapperExtensionsReloaded.Predicates.Internal
{
    internal sealed class Sort : ISort
    {
        public string PropertyName { get; set; }
        public bool Ascending { get; set; }
    }
}