using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Application.UseCases.RegistrarSolicitudDeIngreso;
using FarmaLog.Nucleo.Domain.Common;

namespace FarmaLog.Nucleo.Infrastructure.Messaging
{
    public sealed class RegistrarSolicitudMessageHandler(
        RegistrarSolicitudDeIngresoHandler handler)
    {
        public async Task<MessageDestination> Handle(
            RegistrarSolicitudMessage message, CancellationToken cancellationToken)
        {
            try
            {
                await handler.Handle(Map(message));
                return MessageDestination.Completar;
            }
            catch (DomainException)
            {
                return MessageDestination.DescartarADeadLetter;
            }
        }

        private static RegistrarSolicitudDeIngresoCommand Map(RegistrarSolicitudMessage message) =>
            new(
            message.CodigoLaboratorio,
            message.CuentaCliente,
            message.DireccionDespacho,
            message.TipoOrdenVenta,
            message.EsCenabast,
            message.DocumentoVentaCenabast,
            message.Observacion,
            message.FechaEntrega,
            message.NumeroDelivery,
            message.OrdenCompra,
            message.Urgencia,
            [.. message.Lineas.Select(l =>
                new LineaDeSolicitudCommand(l.Sku, l.Cantidad, l.EstadoInventario, l.Lote))]);
    }
}
