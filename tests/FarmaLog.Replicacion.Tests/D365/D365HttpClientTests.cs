using FarmaLog.Replicacion.D365;
using System.Net;
using System.Text.Json.Nodes;

namespace FarmaLog.Replicacion.Tests.D365
{
    [TestClass]
    public class D365HttpClientTests
    {
        private static (D365HttpClient Client, StubHttpMessageHandler Stub) CreateClient(
            HttpStatusCode statusCode, string responseBody = "")
        {
            StubHttpMessageHandler stub = new(statusCode, responseBody);
            HttpClient httpClient = new(stub) { BaseAddress = new Uri("https://d365.test/") };

            return (new D365HttpClient(httpClient), stub);
        }

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
            (D365HttpClient client, StubHttpMessageHandler stub) = 
                CreateClient(HttpStatusCode.OK, """{ "NumeroPedido": "PV-0000000038" }""");
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

        [TestMethod]
        public async Task D365HttpClient_Should_ReturnAceptadoWithTheOrderNumber_When_D365Responds200()
        {
            //Arrange
            (D365HttpClient client, _) =
                CreateClient(HttpStatusCode.OK, """{ "NumeroPedido": "PV-0000000099" }""");
            PedidoVentaD365 pedido = CreatePedidoVentaD365();

            //Act
            RespuestaD365 response = await client.CrearPedidoVentaAsync(pedido, CancellationToken.None);

            //Assert
            Assert.AreEqual(new RespuestaD365.Aceptado("PV-0000000099"), response);
        }

        [TestMethod]
        public async Task D365HttpClient_Should_ReturnRechazadoWitAllMotivosInOrder_When_D365Responds400()
        {
            //Arrange
            (D365HttpClient client, _) =
                CreateClient(HttpStatusCode.BadRequest, 
                    """{ "Motivos": ["La cuenta 23-0773456789 no existe.", "El SKU SKU-1 no existe."] }""");
            PedidoVentaD365 pedido = CreatePedidoVentaD365();

            //Act
            RespuestaD365 response = await client.CrearPedidoVentaAsync(pedido, CancellationToken.None);

            //Assert
            if (response is not RespuestaD365.Rechazado rechazado)
            {
                Assert.Fail($"Se esperaba Rechazado y llegó {response.GetType().Name}");
                return;
            }

            CollectionAssert.AreEqual(
                new[] { "La cuenta 23-0773456789 no existe.", "El SKU SKU-1 no existe." },
                rechazado.Motivos.ToList());
        }
    }
}