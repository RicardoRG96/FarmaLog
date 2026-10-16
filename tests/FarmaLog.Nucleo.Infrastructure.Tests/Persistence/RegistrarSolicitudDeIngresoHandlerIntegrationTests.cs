using FarmaLog.Nucleo.Application.Common;
using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Application.UseCases.RegistrarSolicitudDeIngreso;
using FarmaLog.Nucleo.Application.UseCases.SolicitarReplicacionEnD365;
using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Events;
using FarmaLog.Nucleo.Infrastructure.Clock;
using FarmaLog.Nucleo.Infrastructure.Events;
using FarmaLog.Nucleo.Infrastructure.Identity;
using FarmaLog.Nucleo.Infrastructure.Messaging;
using FarmaLog.Nucleo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Persistence
{
    [TestClass]
    [DoNotParallelize]
    public class RegistrarSolicitudDeIngresoHandlerIntegrationTests
    {
        private const string ConnectionString =
        "Server=localhost,1433;Database=FarmaLogTests;User Id=sa;Password=Farmalog!2026;TrustServerCertificate=True";

        private static RegistrarSolicitudDeIngresoCommand CrearComandoValido() =>
            new(
                CodigoLaboratorio: "53",
                CuentaCliente: "53-0778903451",
                DireccionDespacho: "53-778903451D1",
                TipoOrdenVenta: "53F1",
                EsCenabast: false,
                DocumentoVentaCenabast: null,
                Observacion: null,
                FechaEntregaSolicitada: new DateOnly(2026, 10, 20),
                NumeroDelivery: "123456789",
                OrdenCompra: "800567",
                Urgencia: false,
                Lineas: new[] { new LineaDeSolicitudCommand("SKU-1", 10, "DISPONIBLE", null) });

        private static DbContextOptions<NucleoDbContext> Options() =>
            new DbContextOptionsBuilder<NucleoDbContext>()
                .UseSqlServer(ConnectionString)
                .Options;

        [TestInitialize]
        public void Inicializar()
        {
            using NucleoDbContext contexto = new(Options());
            contexto.Database.EnsureDeleted();
            contexto.Database.Migrate();
        }

        [TestMethod]
        public async Task RegistrarSolicitudDeIngresoHandler_Should_PersistASolicitudAndOutboxRow_When_CommandIsValid()
        {
            //Arrange
            FakeTimeProvider time = new(new DateTimeOffset(2026, 10, 16, 15, 0, 0, TimeSpan.Zero));

            ServiceCollection services = new();

            services.AddDbContext<NucleoDbContext>(options =>
                options.UseSqlServer(ConnectionString));
            services.AddSingleton<TimeProvider>(time);
            services.AddSingleton<IReloj, RelojDelSistema>();
            services.AddSingleton<IGeneradorDeIdentificadores, GeneradorDeIdentificadores>();
            services.AddScoped<IRepositorioDeSolicitudes, RepositorioDeSolicitudes>();
            services.AddScoped<RegistrarSolicitudDeIngresoHandler>();
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddScoped<IDomainEventHandler<SolicitudDeIngresoRegistrada>, SolicitarReplicacionEnD365Handler>();
            services.AddScoped<IOutbox, EfOutbox>();

            await using ServiceProvider serviceProvider = services.BuildServiceProvider(
                new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });

            await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();

            RegistrarSolicitudDeIngresoHandler? handler =
                scope.ServiceProvider.GetRequiredService<RegistrarSolicitudDeIngresoHandler>();

            //Act
            await handler.Handle(CrearComandoValido());

            //Assert
            await using NucleoDbContext readingContext = new(Options());
            OutboxEntry entry = readingContext.Set<OutboxEntry>().Single();
            SolicitudDeIngresoPedido solicitud = readingContext.Set<SolicitudDeIngresoPedido>().Single();

            Assert.AreEqual(solicitud.Id.ToString(), entry.MessageId);
        }
    }
}
