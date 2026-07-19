using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Domain.Solicitudes.Exceptions
{
    public class CuentaClienteInvalidaException : DomainException
    {
        public CuentaClienteInvalidaException(string message) 
            : base(message) {}
    }
}
