using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Domain.Solicitudes.Exceptions
{
    public sealed class SolicitudInvalidaException : DomainException
    {
        public SolicitudInvalidaException(string message) 
            : base(message) {}
    }
}
