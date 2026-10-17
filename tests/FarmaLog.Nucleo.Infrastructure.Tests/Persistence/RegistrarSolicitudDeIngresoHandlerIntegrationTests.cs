using FarmaLog.Nucleo.Application.UseCases.RegistrarSolicitudDeIngreso;
using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Infrastructure.DependencyInjection;
using FarmaLog.Nucleo.Infrastructure.Messaging;
using FarmaLog.Nucleo.Infrastructure.Persistence;
using FarmaLog.Nucleo.Infrastructure.Tests.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using static FarmaLog.Nucleo.Infrastructure.Tests.Persistence.SqlServerTestDatabase;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Persistence
{
    [TestClass]
    [DoNotParallelize]
    public class RegistrarSolicitudDeIngresoHandlerIntegrationTests
    {
        private static ServiceProvider CrearProveedor()
        {
            FakeTimeProvider time = new(new DateTimeOffset(2026, 10, 16, 15, 0, 0, TimeSpan.Zero));

            ServiceCollection services = new();

            services.AddSingleton<TimeProvider>(time);
            services.AddLogging();
            services.AddSingleton<IOutboxMessageSender>(new OutboxMessageSenderSpy());
            services.AddNucleo(ConnectionString);

            return services.BuildServiceProvider(
                new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
        }

        private static RegistrarSolicitudDeIngresoCommand CreateValidCommand() =>
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

        [TestInitialize]
        public void Inicializar() => Reset();

        [TestMethod]
        public async Task RegistrarSolicitudDeIngresoHandler_Should_PersistASolicitudAndOutboxRow_When_CommandIsValid()
        {
            //Arrange
            using ServiceProvider serviceProvider = CrearProveedor();

            await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();

            RegistrarSolicitudDeIngresoHandler? handler =
                scope.ServiceProvider.GetRequiredService<RegistrarSolicitudDeIngresoHandler>();

            //Act
            await handler.Handle(CreateValidCommand());

            //Assert
            await using NucleoDbContext readingContext = new(Options());
            OutboxEntry entry = readingContext.Set<OutboxEntry>().Single();
            SolicitudDeIngresoPedido solicitud = readingContext.Set<SolicitudDeIngresoPedido>().Single();

            Assert.AreEqual(solicitud.Id.ToString(), entry.MessageId);
        }

        [TestMethod]
        public async Task RegistrarSolicitudDeIngresoHandler_Should_KeepASingleOutboxEntry_When_DeliveryIsDuplicated()
        {
            //Arrange
            using ServiceProvider serviceProvider = CrearProveedor();

            //Act
            await using (AsyncServiceScope firstScope = serviceProvider.CreateAsyncScope())
            {
                await firstScope.ServiceProvider
                    .GetRequiredService<RegistrarSolicitudDeIngresoHandler>()
                    .Handle(CreateValidCommand());
            }

            await using (AsyncServiceScope secondScope = serviceProvider.CreateAsyncScope())
            {
                await secondScope.ServiceProvider
                    .GetRequiredService<RegistrarSolicitudDeIngresoHandler>()
                    .Handle(CreateValidCommand());
            }

            //Assert
            await using NucleoDbContext readingContext = new(Options());
            Assert.HasCount(1, readingContext.Set<SolicitudDeIngresoPedido>());
            Assert.HasCount(1, readingContext.Set<OutboxEntry>());
        }
    }
}
