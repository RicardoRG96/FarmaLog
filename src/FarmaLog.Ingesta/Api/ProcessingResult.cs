namespace FarmaLog.Ingesta.Api
{
    internal sealed record ProcessingResult(
        int PublishedPedidos,
        IReadOnlyList<string> Errors)
    {
        public bool Success => Errors.Count == 0;
    }
}
