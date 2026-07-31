using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Domain.Solicitudes.Exceptions
{
    public sealed class CodigoLaboratorioInvalidoException : DomainException
    {
        public CodigoLaboratorioInvalidoException(string message) 
            : base(message) { }
    }
}
