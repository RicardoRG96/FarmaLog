using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Application.Tests.Doubles;
using FarmaLog.Nucleo.Application.UseCases.SolicitarReplicacionEnD365;
using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Events;

namespace FarmaLog.Nucleo.Application.Tests.UseCases.SolicitarReplicacionEnD365
{
    [TestClass]
    public class SolicitarReplicacionEnD365HandlerTests
    {
        private const string MessageDestination = "replicar-pedido-d365";

        private static SolicitudDeIngresoRegistrada CrearEventoRegistrado()
        {
            SolicitudDeIngresoPedido solicitud = SolicitudDeIngresoPedido.Create(
                id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
                codigoLaboratorio: CodigoLaboratorio.Create("23"),
                cuentaCliente: CuentaCliente.Create("23-0778903671"),
                direccionDespacho: DireccionDespacho.Create("23-778903671D1"),
                tipoOrdenVenta: TipoOrdenVenta.Create("23F1"),
                esCenabast: false,
                documentoVentaCenabast: null,
                lineas: CrearLineas(1),
                observacion: "Entregar en bodega 2",
                fechaEntrega: new DateOnly(2026, 11, 27),
                hoy: new DateOnly(2026, 11, 12),
                numeroDelivery: NumeroDelivery.Create("123456789"),
                ordenCompra: "OC-1",
                urgencia: true);

            return (SolicitudDeIngresoRegistrada)solicitud.DomainEvents.Single();
        }

        private static IReadOnlyCollection<LineaSolicitud> CrearLineas(int cantidad) =>
            Enumerable.Range(1, cantidad)
                .Select(i => LineaSolicitud.Create($"SKU-{i}", 1, "DISPONIBLE", null))
                .ToArray();

        [TestMethod]
        public async Task SolicitarReplicacionEnD365Handler_Should_AddOutboxMessageForReplicacion_When_SolicitudIsRegistrada()
        {
            //Arrange
            InMemoryOutbox outbox = new();
            SolicitarReplicacionEnD365Handler handler = new(outbox);
            SolicitudDeIngresoRegistrada domainEvent = CrearEventoRegistrado();

            //Act
            await handler.HandleAsync(domainEvent, CancellationToken.None);

            //Assert
            OutboxMessage message = Assert.ContainsSingle(outbox.Added);
            Assert.AreEqual(MessageDestination, message.Destination);
            Assert.AreEqual(domainEvent.SolicitudId.ToString(), message.MessageId);
        }

        [TestMethod]
        public async Task SolicitarReplicacionEnD365Handler_Should_AddPedidoToPayload_When_SolicitudIsRegistrada()
        {
            //Arrange
            InMemoryOutbox outbox = new();
            SolicitarReplicacionEnD365Handler handler = new(outbox);
            SolicitudDeIngresoRegistrada domainEvent = CrearEventoRegistrado();

            //Act
            await handler.HandleAsync(domainEvent, CancellationToken.None);

            //Assert
            OutboxMessage message = Assert.ContainsSingle(outbox.Added);
            ReplicarPedidoEnD365Message payload =
                Assert.IsInstanceOfType<ReplicarPedidoEnD365Message>(message.Payload);
            Assert.AreEqual("23", payload.CodigoLaboratorio);
            Assert.AreEqual("23-0778903671", payload.CuentaCliente);
            Assert.AreEqual("23-778903671D1", payload.DireccionDespacho);
            Assert.AreEqual("23F1", payload.TipoOrdenVenta);
            Assert.IsFalse(payload.EsCenabast);
            Assert.IsNull(payload.DocumentoVentaCenabast);
            Assert.AreEqual("Entregar en bodega 2", payload.Observacion);
            Assert.AreEqual(new DateOnly(2026, 11, 27), payload.FechaEntregaSolicitada);
            Assert.AreEqual("123456789", payload.NumeroDelivery);
            Assert.AreEqual("OC-1", payload.OrdenCompra);
            Assert.IsTrue(payload.Urgencia);

            LineaDePedidoMessage lineaPayload = Assert.ContainsSingle(payload.Lineas);
            Assert.AreEqual("SKU-1", lineaPayload.Sku);
            Assert.AreEqual(1, lineaPayload.Cantidad);
            Assert.AreEqual("DISPONIBLE", lineaPayload.EstadoInventario);
            Assert.IsNull(lineaPayload.Lote);
        }
    }
}
