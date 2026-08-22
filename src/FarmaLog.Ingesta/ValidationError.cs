namespace FarmaLog.Ingesta
{
    internal sealed record ValidationError(
        string Problem,
        string OffendingValue,
        string location,
        string HowToFix)
    {
        public override string ToString() =>
            $"{Problem}: '{OffendingValue}' en el pedido {location}. {HowToFix}";
    }
}
