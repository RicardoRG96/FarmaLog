using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;
using System.Collections.Immutable;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed class SolicitudDeIngresoPedido
    {
        private const int MaximoDeLineasPermitidas = 15;
        private const int MaximoDeCaracteresObservacion = 300;

        public CodigoLaboratorio CodigoLaboratorio { get; }
        public CuentaCliente CuentaCliente { get; }
        public DireccionDespacho DireccionDespacho { get; }
        public TipoOrdenVenta TipoOrdenVenta { get; }
        public bool EsCenabast { get; }
        public DocumentoVentaCenabast? DocumentoVentaCenabast { get; }
        public IReadOnlyCollection<LineaSolicitud> Lineas { get; }
        public string? Observacion { get; }
        public DateOnly FechaEntregaSolicitada { get; }
        public NumeroDelivery NumeroDelivery { get; }
        public string? OrdenCompra { get; }
        public bool Urgencia { get; }
        public EstadoSolicitud Estado { get; private set; }
        public IReadOnlyCollection<string> MotivosDeRechazo { get; private set; } = [];

        private SolicitudDeIngresoPedido(
            CodigoLaboratorio codigoLaboratorio,
            CuentaCliente cuentaCliente,
            DireccionDespacho direccionDespacho,
            TipoOrdenVenta tipoOrdenVenta,
            bool esCenabast,
            DocumentoVentaCenabast? documentoVentaCenabast,
            IReadOnlyCollection<LineaSolicitud> lineas,
            string? observacion,
            DateOnly fechaEntregaSolicitada,
            NumeroDelivery numeroDelivery,
            string? ordenCompra,
            bool urgencia)
        {
            CodigoLaboratorio = codigoLaboratorio;
            CuentaCliente = cuentaCliente;
            DireccionDespacho = direccionDespacho;
            TipoOrdenVenta = tipoOrdenVenta;
            EsCenabast = esCenabast;
            DocumentoVentaCenabast = documentoVentaCenabast;
            Lineas = lineas.ToImmutableList();
            Observacion = observacion;
            FechaEntregaSolicitada = fechaEntregaSolicitada;
            NumeroDelivery = numeroDelivery;
            OrdenCompra = ordenCompra;
            Urgencia = urgencia;
            Estado = EstadoSolicitud.Recibida;
        }

        public static SolicitudDeIngresoPedido Create(
            CodigoLaboratorio codigoLaboratorio,
            CuentaCliente cuentaCliente,
            DireccionDespacho direccionDespacho,
            TipoOrdenVenta tipoOrdenVenta,
            bool esCenabast,
            DocumentoVentaCenabast? documentoVentaCenabast,
            IReadOnlyCollection<LineaSolicitud> lineas,
            string? observacion,
            DateOnly? fechaEntrega,
            DateOnly hoy,
            NumeroDelivery numeroDelivery,
            string? ordenCompra,
            bool urgencia)
        {
            ArgumentNullException.ThrowIfNull(codigoLaboratorio);
            ArgumentNullException.ThrowIfNull(cuentaCliente);
            ArgumentNullException.ThrowIfNull(direccionDespacho);
            ArgumentNullException.ThrowIfNull(tipoOrdenVenta);
            ArgumentNullException.ThrowIfNull(numeroDelivery);
            ArgumentNullException.ThrowIfNull(lineas);

            VerificarPertenenciaAlLaboratorio(
                codigoLaboratorio, cuentaCliente, direccionDespacho, tipoOrdenVenta);

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

            DateOnly fechaEntregaFinal = fechaEntrega ?? hoy;

            return new SolicitudDeIngresoPedido(
                codigoLaboratorio,
                cuentaCliente,
                direccionDespacho,
                tipoOrdenVenta,
                esCenabast, 
                documentoVentaCenabast, 
                lineas, 
                observacion, 
                fechaEntregaFinal,
                numeroDelivery,
                ordenCompra,
                urgencia);
        }

        private static void VerificarPertenenciaAlLaboratorio(
            CodigoLaboratorio codigoLaboratorio,
            CuentaCliente cuentaCliente,
            DireccionDespacho direccionDespacho,
            TipoOrdenVenta tipoOrdenVenta)
        {
            if (cuentaCliente.Codigo != codigoLaboratorio)
                throw new SolicitudInvalidaException("La cuenta del cliente debe pertenecer al mismo laboratorio");

            if (direccionDespacho.Codigo != codigoLaboratorio)
                throw new SolicitudInvalidaException("La dirección de despacho debe pertenecer al mismo laboratorio");

            if (tipoOrdenVenta.Codigo != codigoLaboratorio)
                throw new SolicitudInvalidaException("El tipo de orden de venta debe pertenecer al mismo laboratorio");
        }

        public void Aceptar()
        {
            Estado = EstadoSolicitud.Aceptada;
        }

        public void Rechazar(IReadOnlyCollection<string> motivos)
        {
            Estado = EstadoSolicitud.Rechazada;
            MotivosDeRechazo = motivos.ToImmutableList();
        }
    }
}