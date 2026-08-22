namespace FarmaLog.Portal.Cargas
{
    public interface ICargaProcessor
    {
        Task<ResultadoDeCarga> ProcesarAsync(ArchivoDeCarga archivo, CancellationToken ct);
    }
}
