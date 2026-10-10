using FarmaLog.Nucleo.Application.Tests.Doubles;
using FarmaLog.Nucleo.Application.UseCases.RegistrarSolicitudDeIngreso;
using FarmaLog.Nucleo.Domain.Common;
using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Events;

namespace FarmaLog.Nucleo.Application.Tests.UseCases
{
    [TestClass]
    public class RegistrarSolicitudDeIngresoTests
    {
        private static RegistrarSolicitudDeIngresoCommand CrearComandoValido() =>
            new(
                CodigoLaboratorio: "53",
                CuentaCliente: "53-0778903451",
                DireccionDespacho: "53-778903451D1",
                TipoOrdenVenta: "53F1",
                EsCenabast: false,
                DocumentoVentaCenabast: null,
                Observacion: null,
                FechaEntregaSolicitada: new DateOnly(2026, 8, 4),
                NumeroDelivery: "123456789",
                OrdenCompra: "800567",
                Urgencia: false,
                Lineas: new[] { new LineaDeSolicitudCommand("SKU-1", 10, "DISPONIBLE", null) });

        [TestMethod]
        public async Task RegistrarSolicitudDeIngreso_ShouldPersistSolicitudWithEstadoRecibida_When_CommandIsValid()
        {
            //Arrange
            RepositorioDeSolicitudesEnMemoria repositorio = new();
            RelojFijo reloj = new(new DateOnly(2026, 8, 1));
            GeneradorDeIdentificadoresFijo generador = new(Guid.Parse("11111111-1111-1111-1111-111111111111"));
            DomainEventDispatcherSpy eventDispatcher = new();
            RegistrarSolicitudDeIngresoHandler handler = new(repositorio, reloj, generador, eventDispatcher);

            //Act
            await handler.Handle(CrearComandoValido());

            //Assert
            Assert.HasCount(1, repositorio.Guardadas);
            Assert.AreEqual(EstadoSolicitud.Recibida, repositorio.Guardadas[0].Estado);
        }

        [TestMethod]
        public async Task RegistrarSolicitudDeIngreso_Should_DeliverDomainEventsToDispatcher_When_CommandIsValid()
        {
            //Arrange
            RepositorioDeSolicitudesEnMemoria repositorio = new();
            RelojFijo reloj = new(new DateOnly(2026, 8, 1));
            Guid id = Guid.Parse("11111111-1111-1111-1111-111111111111");
            GeneradorDeIdentificadoresFijo generador = new(id);
            DomainEventDispatcherSpy eventDispatcher = new();
            RegistrarSolicitudDeIngresoHandler handler = new(repositorio, reloj, generador, eventDispatcher);

            //Act
            await handler.Handle(CrearComandoValido());

            //Assert
            IDomainEvent domainEvent = Assert.ContainsSingle(eventDispatcher.Dispatched);
            SolicitudDeIngresoRegistrada registrada =
                Assert.IsInstanceOfType<SolicitudDeIngresoRegistrada>(domainEvent);
            Assert.AreEqual(id, registrada.SolicitudId);
        }

        [TestMethod]
        public async Task RegistrarSolicitudDeIngreso_Should_DispatchEventsBeforeSave_When_CommandIsValid()
        {
            //Arrange
            RepositorioDeSolicitudesEnMemoria repositorio = new();
            RelojFijo reloj = new(new DateOnly(2026, 8, 1));
            Guid id = Guid.Parse("11111111-1111-1111-1111-111111111111");
            GeneradorDeIdentificadoresFijo generador = new(id);
            DomainEventDispatcherSpy eventDispatcher = new(repositorio);
            RegistrarSolicitudDeIngresoHandler handler = new(repositorio, reloj, generador, eventDispatcher);

            //Act
            await handler.Handle(CrearComandoValido());

            //Assert
            Assert.AreEqual(0, eventDispatcher.SavedCountAtDispatch);
        }
    }
}
