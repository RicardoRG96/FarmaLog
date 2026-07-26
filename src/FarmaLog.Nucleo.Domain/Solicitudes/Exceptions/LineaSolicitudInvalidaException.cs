using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Domain.Solicitudes.Exceptions
{
    public class LineaSolicitudInvalidaException : DomainException
    {
        public LineaSolicitudInvalidaException(string message) 
            : base(message) {}
    }
}
