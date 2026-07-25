using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Domain.Solicitudes.Exceptions
{
    public class SolicitudConLineasInvalidasException : DomainException
    {
        public SolicitudConLineasInvalidasException(string message) 
            : base(message) {}
    }
}
