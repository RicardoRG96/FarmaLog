using FarmaLog.Nucleo.Application.Tests.Doubles;
using FarmaLog.Nucleo.Application.UseCases.RegistrarSolicitudDeIngreso;
using FarmaLog.Nucleo.Infrastructure.Messaging;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Messaging
{
    [TestClass]
    public class RegistrarSolicitudMessageHandlerTests
    {
        private static RegistrarSolicitudMessage CrearMensajeValido() =>
            new(
                CodigoLaboratorio: "53",
                CuentaCliente: "53-0778903451",
                DireccionDespacho: "53-778903451D1",
                TipoOrdenVenta: "53F1",
                EsCenabast: false,
                DocumentoVentaCenabast: null,
                Observacion: null,
                FechaEntrega: new DateOnly(2026, 8, 4),
                NumeroDelivery: "123456789",
                OrdenCompra: "800567",
                Urgencia: false,
                Lineas: new[] { new LineaMessage("SKU-1", 10, "DISPONIBLE", null) });

        [TestMethod]
        public async Task RegistrarSolicitudMessageHandler_ShouldReturnCompletar_When_TheMessageIsValid()
        {
            //Arrange
            RegistrarSolicitudMessage mensaje = new(
                CodigoLaboratorio: "53",
                CuentaCliente: "53-0778903451",
                DireccionDespacho: "53-778903451D1",
                TipoOrdenVenta: "53F1",
                EsCenabast: false,
                DocumentoVentaCenabast: null,
                Observacion: null,
                FechaEntrega: new DateOnly(2026, 8, 4),
                NumeroDelivery: "123456789",
                OrdenCompra: "800567",
                Urgencia: false,
                Lineas: new[] { new LineaMessage("SKU-1", 10, "DISPONIBLE", null) });

            RepositorioDeSolicitudesEnMemoria repositorio = new();
            RelojFijo reloj = new(new DateOnly(2026, 8, 1));
            GeneradorDeIdentificadoresFijo generador = new(
                Guid.Parse("11111111-1111-1111-1111-111111111111"));

            RegistrarSolicitudMessageHandler handler = new(repositorio, reloj, generador);

            //Act
            MessageDestination destination = await handler.Handle(mensaje, CancellationToken.None);

            //Assert
            Assert.AreEqual(MessageDestination.Completar, destination);
            Assert.HasCount(1, repositorio.Guardadas);
        }

        [TestMethod]
        public async Task RegistrarSolicitudMessageHandler_ShouldReturnDescartarADeadLetter_When_TheMessageViolatesAnInvariant()
        {
            //Arrange
            RegistrarSolicitudMessage mensaje = new(
                CodigoLaboratorio: "53",
                CuentaCliente: "53-0778903451",
                DireccionDespacho: "53-778903451D1",
                TipoOrdenVenta: "53F1",
                EsCenabast: true,
                DocumentoVentaCenabast: null,
                Observacion: null,
                FechaEntrega: new DateOnly(2026, 8, 4),
                NumeroDelivery: "123456789",
                OrdenCompra: "800567",
                Urgencia: false,
                Lineas: new[] { new LineaMessage("SKU-1", 10, "DISPONIBLE", null) });

            RepositorioDeSolicitudesEnMemoria repositorio = new();
            RelojFijo reloj = new(new DateOnly(2026, 8, 1));
            GeneradorDeIdentificadoresFijo generador = new(
                Guid.Parse("11111111-1111-1111-1111-111111111111"));

            RegistrarSolicitudMessageHandler handler = new(repositorio, reloj, generador);

            //Act
            MessageDestination destination = await handler.Handle(mensaje, CancellationToken.None);

            //Assert
            Assert.AreEqual(MessageDestination.DescartarADeadLetter, destination);
            Assert.HasCount(0, repositorio.Guardadas);
        }
    }
}
