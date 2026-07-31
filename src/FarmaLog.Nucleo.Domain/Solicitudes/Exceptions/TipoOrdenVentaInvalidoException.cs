using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Domain.Solicitudes.Exceptions
{
    public sealed class TipoOrdenVentaInvalidoException : DomainException
    {
        public TipoOrdenVentaInvalidoException(string message) 
            : base(message) {}
    }
}
