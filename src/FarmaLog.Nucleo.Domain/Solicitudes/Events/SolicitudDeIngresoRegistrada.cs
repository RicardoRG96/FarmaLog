using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Domain.Solicitudes.Events
{
    public sealed record SolicitudDeIngresoRegistrada(Guid SolicitudId) : IDomainEvent;
}
