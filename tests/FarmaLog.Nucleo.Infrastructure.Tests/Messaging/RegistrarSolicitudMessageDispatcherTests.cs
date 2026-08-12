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
            MessageDestination destino = await dispatcher.Dispatch(bodyRoto, CancellationToken.None);

            //Assert
            Assert.AreEqual(MessageDestination.DescartarADeadLetter, destino);
            Assert.HasCount(0, repositorio.Guardadas);
        }
    }
}
