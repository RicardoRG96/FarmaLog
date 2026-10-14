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

        [TestMethod]
        public async Task DomainEventDispatcher_Should_DeliverEventsToItsMultipleHandlers_When_HandlersAreRegistered()
        {
            //Arrange
            DomainEventHandlerSpy firstSpy = new();
            DomainEventHandlerSpy secondSpy = new();
            ServiceCollection services = new();
            services.AddSingleton<IDomainEventHandler<EventoDePrueba>>(firstSpy);
            services.AddSingleton<IDomainEventHandler<EventoDePrueba>>(secondSpy);
            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            DomainEventDispatcher dispatcher = new(serviceProvider);
            EventoDePrueba evento = new();

            //Act
            await dispatcher.DispatchAsync([evento], CancellationToken.None);

            //Assert
            EventoDePrueba receivedFirst = Assert.ContainsSingle(firstSpy.Events);
            EventoDePrueba receivedSecond = Assert.ContainsSingle(secondSpy.Events);
            Assert.AreSame(evento, receivedFirst);
            Assert.AreSame(evento, receivedSecond);
        }

        [TestMethod]
        public async Task DomainEventDispatcher_ShouldNot_Throw_When_AnEventHasNotHandlersRegistered()
        {
            //Arrange
            ServiceCollection services = new();
            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            DomainEventDispatcher dispatcher = new(serviceProvider);
            EventoDePrueba evento = new();

            //Act
            await dispatcher.DispatchAsync([evento], CancellationToken.None);
        }
    }
}
