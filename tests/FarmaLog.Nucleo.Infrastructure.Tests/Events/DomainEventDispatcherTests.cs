using FarmaLog.Nucleo.Application.Common;
using FarmaLog.Nucleo.Infrastructure.Events;
using Microsoft.Extensions.DependencyInjection;

namespace FarmaLog.Nucleo.Infrastructure.Tests.Events
{
    [TestClass]
    public class DomainEventDispatcherTests
    {
        [TestMethod]
        public async Task DomainEventDispatcher_Should_DeliverEventToItsHandler_When_HandlerIsRegistered()
        {
            //Arrange
            DomainEventHandlerSpy spy = new();
            ServiceCollection services = new();
            services.AddSingleton<IDomainEventHandler<EventoDePrueba>>(spy);
            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            DomainEventDispatcher dispatcher = new(serviceProvider);
            EventoDePrueba evento = new();

            //Act
            await dispatcher.DispatchAsync([evento], CancellationToken.None);

            //Assert
            EventoDePrueba recibido = Assert.ContainsSingle(spy.Events);
            Assert.AreSame(evento, recibido);
        }
    }
}
