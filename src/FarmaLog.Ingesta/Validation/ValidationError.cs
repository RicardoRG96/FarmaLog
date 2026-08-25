namespace FarmaLog.Ingesta.Validation
{
    internal sealed record ValidationError(
        string Problem,
        string OffendingValue,
        string Location,
        string HowToFix)
    {
        public override string ToString() =>
            $"{Problem}: '{OffendingValue}' en el pedido {Location}. {HowToFix}";
    }
}
