using FarmaLog.Nucleo.Application.Common;
using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Domain.Common;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FarmaLog.Nucleo.Infrastructure.Events
{
    public sealed class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken ct = default)
        {
            foreach (IDomainEvent domainEvent in domainEvents)
            {
                Type handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                MethodInfo handleAsync = handlerType.GetMethod(
                    nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!;

                foreach (object? handler in _serviceProvider.GetServices(handlerType))
                {
                    await (Task)handleAsync.Invoke(
                        handler,
                        BindingFlags.DoNotWrapExceptions,
                        binder: null,
                        parameters: [domainEvent, ct],
                        culture: null)!;
                }
            }
        }
    }
}
