namespace FarmaLog.Nucleo.Application.UseCases.RegistrarSolicitudDeIngreso
{
    public sealed record LineaDeSolicitudCommand(
        string Sku,
        int Cantidad,
        string EstadoInventario,
        string? Lote);
}
