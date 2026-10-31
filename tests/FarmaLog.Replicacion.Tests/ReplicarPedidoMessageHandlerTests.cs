using FarmaLog.Replicacion.Contracts;
using FarmaLog.Replicacion.D365;
using FarmaLog.Replicacion.Tests.D365;
using FarmaLog.Replicacion.Tests.Publishing;

namespace FarmaLog.Replicacion.Tests
{
    [TestClass]
    public class ReplicarPedidoMessageHandlerTests
    {
        [TestMethod]
        public async Task ReplicarPedidoMessageHandler_Should_SendOrderWithAllMessageFieldsAndWarehouseFLC1_When_MessageIsHandled()
        {
            //Arrange
            ReplicarPedidoEnD365Message message = new(
                CodigoLaboratorio: "23",
                CuentaCliente: "23-0773456789",
                DireccionDespacho: "23-773456789D1",
                TipoOrdenVenta: "23F1",
                EsCenabast: false,
                DocumentoVentaCenabast: null,
                Observacion: "Entregar en bodega 1",
                FechaEntregaSolicitada: new DateOnly(2026, 11, 4),
                NumeroDelivery: "123456789",
                OrdenCompra: "OC-1",
                Urgencia: true,
                Lineas: [new LineaDePedidoMessage(Sku: "SKU-1", Cantidad: 100, EstadoInventario: "DISPONIBLE", Lote: "LOTE-1")]);

            D365ClientSpy spy = new();
            ResultadoReplicacionPublisherSpy publisher = new();
            string messageId = "11111111-1111-1111-1111-111111111111";
            ReplicarPedidoMessageHandler handler = new(spy, publisher);

            //Act
            await handler.HandleAsync(messageId, message, CancellationToken.None);

            //Assert
            PedidoVentaD365 received = Assert.ContainsSingle(spy.Received);
            LineaPedidoVentaD365 lineaReceived = Assert.ContainsSingle(received.Lineas);

            Assert.AreEqual("23", received.CodigoLaboratorio);
            Assert.AreEqual("23-0773456789", received.CuentaCliente);
            Assert.AreEqual("23-773456789D1", received.DireccionDespacho);
            Assert.AreEqual("23F1", received.TipoOrdenVenta);
            Assert.IsFalse(received.EsCenabast);
            Assert.IsNull(received.DocumentoVentaCenabast);
            Assert.AreEqual("Entregar en bodega 1", received.Observacion);
            Assert.AreEqual(new DateOnly(2026, 11, 4), received.FechaEntregaSolicitada);
            Assert.AreEqual("123456789", received.NumeroDelivery);
            Assert.AreEqual("OC-1", received.OrdenCompra);
            Assert.IsTrue(received.Urgencia);
            Assert.AreEqual("FLC1", received.Almacen);
            Assert.AreEqual("SKU-1", lineaReceived.Sku);
            Assert.AreEqual(100, lineaReceived.Cantidad);
            Assert.AreEqual("DISPONIBLE", lineaReceived.EstadoInventario);
            Assert.AreEqual("LOTE-1", lineaReceived.Lote);
        }

        [TestMethod]
        public async Task ReplicarPedidoMessageHandler_Should_PublishAceptadaWithD365OrderNumberAndCorrelationId_When_D365AcceptsTheOrder()
        {
            //Arrange
            ReplicarPedidoEnD365Message message = new(
                CodigoLaboratorio: "23",
                CuentaCliente: "23-0773456789",
                DireccionDespacho: "23-773456789D1",
                TipoOrdenVenta: "23F1",
                EsCenabast: false,
                DocumentoVentaCenabast: null,
                Observacion: "Entregar en bodega 1",
                FechaEntregaSolicitada: new DateOnly(2026, 11, 4),
                NumeroDelivery: "123456789",
                OrdenCompra: "OC-1",
                Urgencia: true,
                Lineas: [new LineaDePedidoMessage(Sku: "SKU-1", Cantidad: 100, EstadoInventario: "DISPONIBLE", Lote: "LOTE-1")]);

            D365ClientSpy spy = new();
            ResultadoReplicacionPublisherSpy publisher = new();
            string messageId = "11111111-1111-1111-1111-111111111111";
            ReplicarPedidoMessageHandler handler = new(spy, publisher);

            //Act
            await handler.HandleAsync(messageId, message, CancellationToken.None);

            //Assert
            PublishedResultado received = Assert.ContainsSingle(publisher.Received);
            Assert.AreEqual(messageId, received.CorrelationId);
            Assert.AreEqual("Aceptada", received.Resultado.Resultado);
            Assert.AreEqual("PV-0000000038", received.Resultado.NumeroPedidoD365);
            Assert.AreEqual("23", received.Resultado.CodigoLaboratorio);
            Assert.AreEqual("123456789", received.Resultado.NumeroDelivery);
            Assert.IsEmpty(received.Resultado.MotivosDeRechazo);
        }
    }
}
