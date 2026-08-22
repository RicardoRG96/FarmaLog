namespace FarmaLog.Portal.Cargas
{
    public interface IProcesadorDeCargas
    {
        Task<ResultadoDeCarga> ProcesarAsync(ArchivoDeCarga archivo, CancellationToken ct);
    }
}
