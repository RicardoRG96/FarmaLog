using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Domain.Solicitudes.Exceptions
{
    public sealed class SolicitudYaResueltaException : DomainException
    {
        public SolicitudYaResueltaException(string message) 
            : base(message) {}
    }
}
