namespace FarmaLog.Ingesta
{
    internal sealed record ValidationError(
        string Problema,
        string ValorOfensor,
        string Ubicacion,
        string ComoCorregir)
    {
        public override string ToString() =>
            $"{Problema}: '{ValorOfensor}' en el pedido {Ubicacion}. {ComoCorregir}";
    }
}
