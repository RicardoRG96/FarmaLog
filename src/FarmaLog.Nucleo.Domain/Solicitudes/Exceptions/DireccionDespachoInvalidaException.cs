using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Domain.Solicitudes.Exceptions
{
    public class DireccionDespachoInvalidaException : DomainException
    {
        public DireccionDespachoInvalidaException(string message) 
            : base(message) {}
    }
}
