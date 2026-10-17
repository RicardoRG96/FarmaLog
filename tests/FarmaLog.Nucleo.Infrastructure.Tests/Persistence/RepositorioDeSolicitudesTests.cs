using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Tests.Solicitudes;
using FarmaLog.Nucleo.Infrastructure.Persistence;
using static FarmaLog.Nucleo.Infrastructure.Tests.Persistence.SqlServerTestDatabase;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Persistence;

[TestClass]
[DoNotParallelize]
public class RepositorioDeSolicitudesTests
{
    [TestInitialize]
    public void Inicializar() => Reset();

    [TestMethod]
    public async Task RepositorioDeSolicitudes_ShouldRehydrateAnIdenticalSolicitud_When_ASolicitudIsSaved()
    {
        // Arrange
        SolicitudDeIngresoPedido solicitud = SolicitudDeIngresoPedidoTests.CrearSolicitud();

        await using NucleoDbContext writingContext = new(Options());
        RepositorioDeSolicitudes repositorio = new(writingContext);

        // Act
        await repositorio.Guardar(solicitud, CancellationToken.None);

        await using NucleoDbContext readingContext = new(Options());
        SolicitudDeIngresoPedido solicitudRehidratada = readingContext
            .Solicitudes
            .Single(s => s.Id == solicitud.Id);

        // Assert
        Assert.AreEqual(solicitud.Id, solicitudRehidratada.Id);
        Assert.AreEqual(solicitud.Estado, solicitudRehidratada.Estado);
        Assert.AreEqual(solicitud.CodigoLaboratorio, solicitudRehidratada.CodigoLaboratorio);
        Assert.AreEqual(solicitud.NumeroDelivery, solicitudRehidratada.NumeroDelivery);
        Assert.AreEqual(solicitud.FechaEntregaSolicitada, solicitudRehidratada.FechaEntregaSolicitada);
        Assert.HasCount(solicitud.Lineas.Count, solicitudRehidratada.Lineas);
    }

    [TestMethod]
    public async Task RepositorioDeSolicitudes_ShouldReturnYaExistia_When_TheSameDeliveryAndCodigoLaboratorioIsSavedTwice()
    {
        // Arrange
        SolicitudDeIngresoPedido primeraSolicitud = SolicitudDeIngresoPedidoTests.CrearSolicitud();
        SolicitudDeIngresoPedido segundaSolicitud = SolicitudDeIngresoPedidoTests.CrearSolicitud(
            id: Guid.Parse("22222222-2222-2222-2222-222222222222"));

        await using NucleoDbContext primerContexto = new(Options());
        await using NucleoDbContext segundoContexto = new(Options());

        // Act
        ResultadoGuardado primerResultado =
            await new RepositorioDeSolicitudes(primerContexto).Guardar(primeraSolicitud, CancellationToken.None);

        ResultadoGuardado segundoResultado =
            await new RepositorioDeSolicitudes(segundoContexto).Guardar(segundaSolicitud, CancellationToken.None);

        // Assert
        Assert.AreEqual(ResultadoGuardado.Guardada, primerResultado);
        Assert.AreEqual(ResultadoGuardado.YaExistia, segundoResultado);
    }

    [TestMethod]
    public async Task RepositorioDeSolicitudes_Should_LeaveNoTrackedEntries_When_SolicitudIsDuplicated()
    {
        //Arrange
        SolicitudDeIngresoPedido primeraSolicitud = SolicitudDeIngresoPedidoTests.CrearSolicitud();
        SolicitudDeIngresoPedido segundaSolicitud = SolicitudDeIngresoPedidoTests.CrearSolicitud(
            id: Guid.Parse("22222222-2222-2222-2222-222222222222"));
        OutboxMessage outboxMessage = new(
            Destination: "cola-de-prueba", MessageId: primeraSolicitud.Id.ToString(), Payload: "test");

        await using NucleoDbContext primerContexto = new(Options());
        await using NucleoDbContext segundoContexto = new(Options());
        EfOutbox outbox = new(segundoContexto, TimeProvider.System);

        //Act
        await new RepositorioDeSolicitudes(primerContexto).Guardar(primeraSolicitud, CancellationToken.None);
        outbox.Add(outboxMessage);
        await new RepositorioDeSolicitudes(segundoContexto).Guardar(segundaSolicitud, CancellationToken.None);

        //Assert
        Assert.IsEmpty(segundoContexto.ChangeTracker.Entries());
    }
}