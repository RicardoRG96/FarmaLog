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
        private static SolicitudDeIngresoRegistrada CrearEventoRegistrado()
        {
            SolicitudDeIngresoPedido solicitud = SolicitudDeIngresoPedido.Create(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CodigoLaboratorio.Create("23"),
                CuentaCliente.Create("23-0778903671"),
                DireccionDespacho.Create("23-778903671D1"),
                TipoOrdenVenta.Create("23F1"),
                false,
                null,
                CrearLineas(1),
                null,
                new DateOnly(2026, 11, 27),
                new DateOnly(2026, 11, 12),
                NumeroDelivery.Create("123456789"),
                "OC-1",
                false);

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
            string messageDestination = "replicar-pedido-d365";

            //Assert
            OutboxMessage message = Assert.ContainsSingle(outbox.Added);
            Assert.AreEqual(messageDestination, message.Destination);
            Assert.AreEqual(domainEvent.SolicitudId.ToString(), message.MessageId);
        }
    }
}
