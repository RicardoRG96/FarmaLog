using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed class SolicitudDeIngresoPedido
    {
        private const int MaximoDeLineasPermitidas = 15;
        private const int MaximoDeCaracteresObservacion = 300;

        public Guid Id { get; }
        public CodigoLaboratorio CodigoLaboratorio { get; }
        public CuentaCliente CuentaCliente { get; }
        public DireccionDespacho DireccionDespacho { get; }
        public TipoOrdenVenta TipoOrdenVenta { get; }
        public bool EsCenabast { get; }
        public DocumentoVentaCenabast? DocumentoVentaCenabast { get; }
        private readonly List<LineaSolicitud> _lineas = [];
        public IReadOnlyCollection<LineaSolicitud> Lineas => _lineas.AsReadOnly();
        public string? Observacion { get; }
        public DateOnly FechaEntregaSolicitada { get; }
        public NumeroDelivery NumeroDelivery { get; }
        public string? OrdenCompra { get; }
        public bool Urgencia { get; }
        public EstadoSolicitud Estado { get; private set; }
        private readonly List<string> _motivosDeRechazo = [];
        public IReadOnlyCollection<string> MotivosDeRechazo => _motivosDeRechazo.AsReadOnly();

        #pragma warning disable CS8618 // EF Core materializa los campos por reflexión
        private SolicitudDeIngresoPedido() { }
        #pragma warning restore CS8618

        private SolicitudDeIngresoPedido(
            Guid id,
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
            bool urgencia,
            EstadoSolicitud estado,
            IReadOnlyCollection<string> motivosDeRechazo)
        {
            Id = id;
            CodigoLaboratorio = codigoLaboratorio;
            CuentaCliente = cuentaCliente;
            DireccionDespacho = direccionDespacho;
            TipoOrdenVenta = tipoOrdenVenta;
            EsCenabast = esCenabast;
            DocumentoVentaCenabast = documentoVentaCenabast;
            _lineas.AddRange(lineas);
            Observacion = observacion;
            FechaEntregaSolicitada = fechaEntregaSolicitada;
            NumeroDelivery = numeroDelivery;
            OrdenCompra = ordenCompra;
            Urgencia = urgencia;
            Estado = estado;
            _motivosDeRechazo = [.. motivosDeRechazo];
        }

        public static SolicitudDeIngresoPedido Create(
            Guid id,
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
            if (id == Guid.Empty)
                throw new ArgumentException("El pedido debe tener un ID");

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
                id,
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
                urgencia,
                EstadoSolicitud.Recibida,
                []);
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

        public static SolicitudDeIngresoPedido Rehidratar(
            Guid id,
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
            bool urgencia,
            EstadoSolicitud estado,
            IReadOnlyCollection<string> motivosDeRechazo)
            => new(
                id,
                codigoLaboratorio,
                cuentaCliente,
                direccionDespacho,
                tipoOrdenVenta,
                esCenabast,
                documentoVentaCenabast,
                lineas,
                observacion,
                fechaEntregaSolicitada,
                numeroDelivery,
                ordenCompra,
                urgencia,
                estado,
                motivosDeRechazo);

        public void Aceptar()
        {
            if (Estado == EstadoSolicitud.Aceptada)
            {
                return;
            }

            if (Estado == EstadoSolicitud.Rechazada)
            {
                throw new SolicitudYaResueltaException(
                    "La solicitud ya fue rechazada y no puede aceptarse.");
            }

            Estado = EstadoSolicitud.Aceptada;
        }

        public void Rechazar(IReadOnlyCollection<string> motivos)
        {
            if (Estado == EstadoSolicitud.Rechazada)
            {
                return;
            }
            
            if (Estado == EstadoSolicitud.Aceptada)
            {
                throw new SolicitudYaResueltaException(
                    "La solicitud ya fue aceptada y no puede rechazarse.");
            }

            Estado = EstadoSolicitud.Rechazada;
            _motivosDeRechazo.Clear();
            _motivosDeRechazo.AddRange(motivos);
        }
    }
}