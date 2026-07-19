using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Domain.Solicitudes.Exceptions
{
    public class DocumentoVentaCenabastInvalidoException : DomainException
    {
        public DocumentoVentaCenabastInvalidoException(string message) 
            : base(message) {}
    }
}
