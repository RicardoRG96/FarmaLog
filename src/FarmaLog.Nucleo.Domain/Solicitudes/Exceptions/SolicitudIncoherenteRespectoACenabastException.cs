using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Domain.Solicitudes.Exceptions
{
    public class SolicitudIncoherenteRespectoACenabastException : DomainException
    {
        public SolicitudIncoherenteRespectoACenabastException(string message) 
            : base(message) {}
    }
}
