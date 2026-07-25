using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Domain.Solicitudes.Exceptions
{
    public class SolicitudIncoherenteRespesctoACenabastException : DomainException
    {
        public SolicitudIncoherenteRespesctoACenabastException(string message) 
            : base(message) {}
    }
}
