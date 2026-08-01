using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Domain.Solicitudes;

namespace FarmaLog.Nucleo.Application.UseCases.RegistrarSolicitudDeIngreso
{
    public sealed class RegistrarSolicitudDeIngresoHandler
    {
        private readonly IRepositorioDeSolicitudes _repositorio;
        private readonly IReloj _reloj;
        private readonly IGeneradorDeIdentificadores _generador;

        public RegistrarSolicitudDeIngresoHandler(
            IRepositorioDeSolicitudes repositorio, 
            IReloj reloj, 
            IGeneradorDeIdentificadores generador)
        {
            _repositorio = repositorio;
            _reloj = reloj;
            _generador = generador;
        }

        public async Task Handle(RegistrarSolicitudDeIngresoCommand command)
        {
            DateOnly hoy = _reloj.Hoy;

            Guid id = _generador.Nuevo();

            IReadOnlyCollection<LineaSolicitud> lineas = command.Lineas
                .Select(x => LineaSolicitud.Create(x.Sku, x.Cantidad, x.EstadoInventario, x.Lote))
                .ToArray();

            SolicitudDeIngresoPedido solicitud = SolicitudDeIngresoPedido.Create(
                id,
                CodigoLaboratorio.Create(command.CodigoLaboratorio),
                CuentaCliente.Create(command.CuentaCliente),
                DireccionDespacho.Create(command.DireccionDespacho),
                TipoOrdenVenta.Create(command.TipoOrdenVenta),
                command.EsCenabast,
                MapearDocumentoCenabast(command.DocumentoVentaCenabast),
                lineas,
                command.Observacion,
                command.FechaEntregaSolicitada,
                hoy,
                NumeroDelivery.Create(command.NumeroDelivery),
                command.OrdenCompra,
                command.Urgencia);

            await _repositorio.Guardar(solicitud);
        }

        private static DocumentoVentaCenabast? MapearDocumentoCenabast(string? valor) =>
            valor is null ? null : DocumentoVentaCenabast.Create(valor);
    }
}
