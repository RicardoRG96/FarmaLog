using FarmaLog.Replicacion.Messaging;
using FarmaLog.Replicacion.Tests.D365;
using FarmaLog.Replicacion.Tests.Publishing;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace FarmaLog.Replicacion.Tests.Messaging
{
    [TestClass]
    public class ReplicarPedidoMessageDispatcherTests
    {
        private const string MessageId = "11111111-1111-1111-1111-111111111111";

        private static string CreateValidBody() =>
            """
            {
                "CodigoLaboratorio": "23",
                "NumeroDelivery": "123456789",
                "CuentaCliente": "23-0778903671",
                "DireccionDespacho": "23-778903671D1",
                "TipoOrdenVenta": "23F1",
                "EsCenabast": false,
                "DocumentoVentaCenabast": null,
                "Observacion": "Entregar en bodega 1",
                "FechaEntregaSolicitada": "2026-11-15",
                "OrdenCompra": "OC-1",
                "Urgencia": true,
                "Lineas": [
                    { "Sku": "100234", "Cantidad": 5, "EstadoInventario": "DISPONIBLE", "Lote": "LOTE-1B" }
                ]
            }
            """;

        private static string CreateBodyWithout(string property)
        {
            JsonNode body = JsonNode.Parse(CreateValidBody())!;
            body.AsObject().Remove(property);
            return body.ToJsonString();
        }

        private static string CreateBodyWithoutLineProperty(string property)
        {
            JsonNode body = JsonNode.Parse(CreateValidBody())!;
            body["Lineas"]![0]!.AsObject().Remove(property);
            return body.ToJsonString();
        }

        public static IEnumerable<object[]> BodiesMissingARequiredField =>
        [
            ["CodigoLaboratorio", CreateBodyWithout("CodigoLaboratorio")],
            ["CuentaCliente", CreateBodyWithout("CuentaCliente")],
            ["DireccionDespacho", CreateBodyWithout("DireccionDespacho")],
            ["TipoOrdenVenta", CreateBodyWithout("TipoOrdenVenta")],
            ["FechaEntregaSolicitada", CreateBodyWithout("FechaEntregaSolicitada")],
            ["NumeroDelivery", CreateBodyWithout("NumeroDelivery")],
            ["Lineas", CreateBodyWithout("Lineas")],
            ["Lineas[0].Sku", CreateBodyWithoutLineProperty("Sku")],
            ["Lineas[0].Cantidad", CreateBodyWithoutLineProperty("Cantidad")],
            ["Lineas[0].EstadoInventario", CreateBodyWithoutLineProperty("EstadoInventario")],
        ];

        [TestMethod]
        public async Task ReplicarPedidoMessageDispatcher_Should_ReturnDeadLetterWithoutCallingD365ClientOrPublishing_When_TheBodyIsNotValidJson()
        {
            //Arrange
            D365ClientSpy spy = new();
            ResultadoReplicacionPublisherSpy publisher = new();
            ReplicarPedidoMessageHandler handler = new(spy, publisher);

            ReplicarPedidoMessageDispatcher dispatcher = new(handler);
            string malformedBody = "{ esto no es json }";

            //Act
            MessageDestination destination = await dispatcher.DispatchAsync(MessageId, malformedBody, CancellationToken.None);

            //Assert
            Assert.AreEqual(MessageDestination.DeadLetter, destination);
            Assert.IsEmpty(spy.Received);
            Assert.IsEmpty(publisher.Received);
        }

        [TestMethod]
        public async Task ReplicarPedidoMessageDispatcher_Should_ReturnCompleteAndCallD365ClientAndPublish_When_TheMessageIsWellFormed()
        {
            //Arrange
            D365ClientSpy spy = new();
            ResultadoReplicacionPublisherSpy publisher = new();
            ReplicarPedidoMessageHandler handler = new(spy, publisher);

            ReplicarPedidoMessageDispatcher dispatcher = new(handler);
            string validBody = CreateValidBody();

            //Act
            MessageDestination destination = await dispatcher.DispatchAsync(
                MessageId, validBody, CancellationToken.None);

            //Assert
            Assert.AreEqual(MessageDestination.Complete, destination);
            Assert.ContainsSingle(spy.Received);
            PublishedResultado published = Assert.ContainsSingle(publisher.Received);
            Assert.AreEqual(MessageId, published.CorrelationId);
        }

        [TestMethod]
        public async Task ReplicarPedidoMessageDispatcher_Should_PropagateTheException_When_D365ClientThrowsJsonException()
        {
            //Arrange
            ThrowingD365Client client = new(new JsonException("Unreadable D365 response"));
            ResultadoReplicacionPublisherSpy publisher = new();
            ReplicarPedidoMessageHandler handler = new(client, publisher);

            ReplicarPedidoMessageDispatcher dispatcher = new(handler);

            //Act + Assert
            await Assert.ThrowsAsync<JsonException>(
                () => dispatcher.DispatchAsync(MessageId, CreateValidBody(), CancellationToken.None));
            Assert.IsEmpty(publisher.Received);
        }

        [TestMethod]
        [DynamicData(nameof(BodiesMissingARequiredField))]
        public async Task ReplicarPedidoMessageDispatcher_Should_ReturnDeadLetter_When_ARequiredFieldIsMissing(
            string missingField, string body)
        {
            //Arrange
            D365ClientSpy spy = new();
            ResultadoReplicacionPublisherSpy publisher = new();
            ReplicarPedidoMessageHandler handler = new(spy, publisher);

            ReplicarPedidoMessageDispatcher dispatcher = new(handler);

            //Act
            MessageDestination destination = await dispatcher.DispatchAsync(
                MessageId, body, CancellationToken.None);

            //Assert
            Assert.AreEqual(MessageDestination.DeadLetter, destination, $"Falta {missingField}");
            Assert.IsEmpty(spy.Received);
            Assert.IsEmpty(publisher.Received);
        }
    }
}
