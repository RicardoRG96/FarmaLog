using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed class SolicitudDeIngresoPedido
    {
        private const int MaximoDeLineasPermitidas = 15;
        private const int MaximoDeCaracteresObservacion = 300;

        public bool EsCenabast { get; }
        public DocumentoVentaCenabast? DocumentoVentaCenabast { get; }
        public IReadOnlyCollection<LineaSolicitud> Lineas { get; }
        public string? Observacion { get; }
        public DateOnly? FechaEntregaSolicitada { get; }

        private SolicitudDeIngresoPedido(
            bool esCenabast,
            DocumentoVentaCenabast? documentoVentaCenabast,
            IReadOnlyCollection<LineaSolicitud> lineas,
            string? observacion,
            DateOnly? fechaEntregaSolicitada)
        {
            EsCenabast = esCenabast;
            DocumentoVentaCenabast = documentoVentaCenabast;
            Lineas = lineas;
            Observacion = observacion;
            FechaEntregaSolicitada = fechaEntregaSolicitada;
        }

        public static SolicitudDeIngresoPedido Create(
            bool esCenabast,
            DocumentoVentaCenabast? documentoVentaCenabast,
            IReadOnlyCollection<LineaSolicitud> lineas,
            string? observacion,
            DateOnly? fechaEntrega,
            DateOnly hoy)
        {
            if (lineas.Count < 1)
                throw new SolicitudInvalidaException("El pedido debe tener al menos una línea");

            if (lineas.Count > MaximoDeLineasPermitidas)
                throw new SolicitudInvalidaException("El pedido debe tener como máximo 15 líneas");

            bool esCoherenteConCenabast = esCenabast == (documentoVentaCenabast is not null);

            if (!esCoherenteConCenabast)
                throw new SolicitudInvalidaException(
                    "La información proporcionada respecto a Cenabast es incoherente");

            if (fechaEntrega is not null && fechaEntrega < hoy)
                throw new SolicitudInvalidaException(
                    "La fecha de entrega solicitada no puede ser una fecha pasada");

            if (observacion is not null && observacion.Length > MaximoDeCaracteresObservacion)
                throw new SolicitudInvalidaException(
                    "La observación del pedido no puede tener más de 300 caracteres");

            return new SolicitudDeIngresoPedido(
                esCenabast, documentoVentaCenabast, lineas, observacion, fechaEntrega);
        }
    }
}
