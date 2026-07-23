using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Domain.Solicitudes.Exceptions
{
    public class TipoOrdenVentaInvalidoException : DomainException
    {
        public TipoOrdenVentaInvalidoException(string message) 
            : base(message) {}
    }
}
