using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed class SolicitudDeIngresoPedido
    {
        private const int MaximoDeLineasPermitidas = 15;

        public bool EsCenabast { get; }
        public DocumentoVentaCenabast? DocumentoVentaCenabast { get; }
        public IReadOnlyCollection<LineaSolicitud> Lineas { get; }

        private SolicitudDeIngresoPedido(
            bool esCenabast, 
            DocumentoVentaCenabast? documentoVentaCenabast,
            IReadOnlyCollection<LineaSolicitud> lineas)
        {
            EsCenabast = esCenabast;
            DocumentoVentaCenabast = documentoVentaCenabast;
            Lineas = lineas;
        }

        public static SolicitudDeIngresoPedido Create(
            bool esCenabast, 
            DocumentoVentaCenabast? documentoVentaCenabast,
            IReadOnlyCollection<LineaSolicitud> lineas)
        {
            if (lineas.Count < 1)
                throw new SolicitudConLineasInvalidasException("El pedido debe tener al menos una línea");

            if (lineas.Count > MaximoDeLineasPermitidas)
                throw new SolicitudConLineasInvalidasException("El pedido debe tener como máximo 15 líneas");

            bool esCoherenteConCenabast = esCenabast == (documentoVentaCenabast is not null);

            if (!esCoherenteConCenabast)
            {
                throw new SolicitudIncoherenteRespectoACenabastException(
                    "La información proporcionada respecto a Cenabast es incoherente");
            }

            return new SolicitudDeIngresoPedido(esCenabast, documentoVentaCenabast, lineas);
        }
    }
}
