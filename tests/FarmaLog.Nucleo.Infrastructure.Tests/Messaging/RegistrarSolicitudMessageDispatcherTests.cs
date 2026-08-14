using FarmaLog.Nucleo.Application.Tests.Doubles;
using FarmaLog.Nucleo.Application.UseCases.RegistrarSolicitudDeIngreso;
using FarmaLog.Nucleo.Infrastructure.Messaging;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Messaging
{
    [TestClass]
    public class RegistrarSolicitudMessageDispatcherTests
    {
        [TestMethod]
        public async Task RegistrarSolicitudMessageDispatcher_ShouldReturnDescartarADeadLetter_When_TheBodyIsNotValidJson()
        {
            //Arrange
            RepositorioDeSolicitudesEnMemoria repositorio = new();
            RelojFijo reloj = new(new DateOnly(2026, 8, 1));
            GeneradorDeIdentificadoresFijo generador = new(
                Guid.Parse("11111111-1111-1111-1111-111111111111"));

            RegistrarSolicitudDeIngresoHandler applicationHandler = new(repositorio, reloj, generador);

            RegistrarSolicitudMessageHandler messageHandler = new(applicationHandler);

            RegistrarSolicitudMessageDispatcher dispatcher = new(messageHandler);
            string bodyRoto = "{ esto no es json }";

            //Act
            MessageDestination destino = await dispatcher.DispatchAsync(bodyRoto, CancellationToken.None);

            //Assert
            Assert.AreEqual(MessageDestination.DescartarADeadLetter, destino);
            Assert.HasCount(0, repositorio.Guardadas);
        }

        [TestMethod]
        public async Task RegistrarSolicitudMessageDispatcher_ShouldReturnDescartarADeadLetter_When_TheBodyDeserializesToNull()
        {
            //Arrange
            RepositorioDeSolicitudesEnMemoria repositorio = new();
            RelojFijo reloj = new(new DateOnly(2026, 8, 1));
            GeneradorDeIdentificadoresFijo generador = new(
                Guid.Parse("11111111-1111-1111-1111-111111111111"));

            RegistrarSolicitudDeIngresoHandler applicationHandler = new(repositorio, reloj, generador);

            RegistrarSolicitudMessageHandler messageHandler = new(applicationHandler);

            RegistrarSolicitudMessageDispatcher dispatcher = new(messageHandler);
            string bodyNulo = "null";

            //Act
            MessageDestination destino = await dispatcher.DispatchAsync(bodyNulo, CancellationToken.None);

            //Assert
            Assert.AreEqual(MessageDestination.DescartarADeadLetter, destino);
            Assert.HasCount(0, repositorio.Guardadas);
        }

        [TestMethod]
        public async Task RegistrarSolicitudMessageDispatcher_ShouldReturnDescartarADeadLetter_When_ARequiredFieldIsMissing()
        {
            //Arrange
            RepositorioDeSolicitudesEnMemoria repositorio = new();
            RelojFijo reloj = new(new DateOnly(2026, 8, 1));
            GeneradorDeIdentificadoresFijo generador = new(
                Guid.Parse("11111111-1111-1111-1111-111111111111"));

            RegistrarSolicitudDeIngresoHandler applicationHandler = new(repositorio, reloj, generador);

            RegistrarSolicitudMessageHandler messageHandler = new(applicationHandler);

            RegistrarSolicitudMessageDispatcher dispatcher = new(messageHandler);
            string bodySinNumeroDelivery = """
            {
                "CodigoLaboratorio": "23",
                "CuentaCliente": "23-0778903671",
                "DireccionDespacho": "23-778903671D1",
                "TipoOrdenVenta": "23F1",
                "EsCenabast": false,
                "DocumentoVentaCenabast": null,
                "Observacion": null,
                "FechaEntrega": "2026-08-15",
                "OrdenCompra": null,
                "Urgencia": false,
                "Lineas": [
                    { "Sku": "100234", "Cantidad": 5, "EstadoInventario": "DISPONIBLE", "Lote": null }
                ]
            }
            """;

            //Act
            MessageDestination destino = await dispatcher.DispatchAsync(
                bodySinNumeroDelivery, CancellationToken.None);

            //Assert
            Assert.AreEqual(MessageDestination.DescartarADeadLetter, destino);
            Assert.HasCount(0, repositorio.Guardadas);
        }
    }
}
