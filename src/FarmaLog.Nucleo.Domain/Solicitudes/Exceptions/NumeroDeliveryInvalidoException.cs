using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Domain.Solicitudes.Exceptions
{
    public sealed class NumeroDeliveryInvalidoException : DomainException
    {
        public NumeroDeliveryInvalidoException(string message) 
            : base(message) {}
    }
}
