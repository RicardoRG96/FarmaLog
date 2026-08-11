using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Domain.Solicitudes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FarmaLog.Nucleo.Infrastructure.Persistence
{
    public sealed class RepositorioDeSolicitudes(NucleoDbContext context) : IRepositorioDeSolicitudes
    {
        private const int ViolacionDeIndiceUnico = 2601;
        private const int ViolacionDeConstraintUnica = 2627;

        public async Task<ResultadoGuardado> Guardar(
            SolicitudDeIngresoPedido solicitud, CancellationToken cancellationToken)
        {
            context.Solicitudes.Add(solicitud);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
                return ResultadoGuardado.Guardada;
            }
            catch (DbUpdateException exception) when (EsDeliveryDuplicado(exception))
            {
                context.Entry(solicitud).State = EntityState.Detached;
                return ResultadoGuardado.YaExistia;
            }
        }

        private static bool EsDeliveryDuplicado(DbUpdateException exception) => 
            exception.InnerException is SqlException sql &&
            sql.Number is ViolacionDeIndiceUnico or ViolacionDeConstraintUnica;
    }
}
