using FarmaLog.Replicacion.Messaging;
using FarmaLog.Replicacion.Tests.D365;
using FarmaLog.Replicacion.Tests.Publishing;

namespace FarmaLog.Replicacion.Tests.Messaging
{
    [TestClass]
    public class ReplicarPedidoMessageDispatcherTests
    {
        private const string MessageId = "11111111-1111-1111-1111-111111111111";

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
    }
}
