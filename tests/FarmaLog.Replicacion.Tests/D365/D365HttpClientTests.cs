using FarmaLog.Replicacion.D365;
using System.Net;
using System.Text.Json.Nodes;

namespace FarmaLog.Replicacion.Tests.D365
{
    [TestClass]
    public class D365HttpClientTests
    {
        private static PedidoVentaD365 CreatePedidoVentaD365() =>
            new(
                CodigoLaboratorio: "23",
                CuentaCliente: "23-0773456789",
                DireccionDespacho: "23-773456789D1",
                TipoOrdenVenta: "23F1",
                EsCenabast: false,
                DocumentoVentaCenabast: null,
                Observacion: "Entregar en bodega 1",
                FechaEntregaSolicitada: new DateOnly(2026, 11, 15),
                NumeroDelivery: "123456789",
                OrdenCompra: "OC-1",
                Urgencia: true,
                Lineas: [new LineaPedidoVentaD365(Sku: "SKU-1", Cantidad: 100, EstadoInventario: "DISPONIBLE", Lote: "LOTE-1")],
                Almacen: "FLC1");

        [TestMethod]
        public async Task D365HttpClient_Should_PostTheSerializedOrderToSalesOrders_When_CreatingAnOrder()
        {
            //Arrange
            StubHttpMessageHandler stub = new(HttpStatusCode.OK, """{ "NumeroPedido": "PV-0000000038" }""");
            HttpClient httpClient = new(stub) { BaseAddress = new Uri("https://d365.test/") };
            D365HttpClient client = new(httpClient);
            PedidoVentaD365 pedido = CreatePedidoVentaD365();

            //Act
            await client.CrearPedidoVentaAsync(pedido, CancellationToken.None);

            //Assert
            ReceivedRequest received = Assert.ContainsSingle(stub.Received);
            Assert.AreEqual(HttpMethod.Post, received.Method);
            Assert.AreEqual("/api/salesorders", received.Uri!.AbsolutePath);
            JsonNode body = JsonNode.Parse(received.Body!)!;
            Assert.AreEqual("123456789", body["NumeroDelivery"]!.ToString());
            Assert.AreEqual("FLC1", body["Almacen"]!.ToString());
        }
    }
}