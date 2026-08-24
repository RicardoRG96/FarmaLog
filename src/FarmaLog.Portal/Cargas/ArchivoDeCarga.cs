namespace FarmaLog.Portal.Cargas
{
    public sealed record ArchivoDeCarga(
        string Name, Stream Content, string CodigoLaboratorio, string TipoOrdenVenta);
}
