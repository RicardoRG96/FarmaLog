using FarmaLog.Nucleo.Application.Common;
using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Application.UseCases.RegistrarSolicitudDeIngreso;
using FarmaLog.Nucleo.Application.UseCases.SolicitarReplicacionEnD365;
using FarmaLog.Nucleo.Domain.Solicitudes.Events;
using FarmaLog.Nucleo.Infrastructure.Clock;
using FarmaLog.Nucleo.Infrastructure.Events;
using FarmaLog.Nucleo.Infrastructure.Identity;
using FarmaLog.Nucleo.Infrastructure.Messaging;
using FarmaLog.Nucleo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FarmaLog.Nucleo.Infrastructure.DependencyInjection
{
    public static class NucleoServiceCollectionExtensions
    {
        public static IServiceCollection AddNucleo(this IServiceCollection services, string? connectionString)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            services.AddDbContext<NucleoDbContext>(options => options.UseSqlServer(connectionString));

            // Si el host ya registró un reloj (los tests usan FakeTimeProvider), se respeta
            services.TryAddSingleton(TimeProvider.System);
            services.AddSingleton<IReloj, RelojDelSistema>();
            services.AddSingleton<IGeneradorDeIdentificadores, GeneradorDeIdentificadores>();

            services.AddScoped<IRepositorioDeSolicitudes, RepositorioDeSolicitudes>();
            services.AddScoped<RegistrarSolicitudDeIngresoHandler>();
            services.AddScoped<RegistrarSolicitudMessageHandler>();
            services.AddScoped<RegistrarSolicitudMessageDispatcher>();

            // Los tres Scoped ya que comparten el NucleoDbContext del scope del mensaje
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddScoped<IDomainEventHandler<SolicitudDeIngresoRegistrada>, SolicitarReplicacionEnD365Handler>();
            services.AddScoped<IOutbox, EfOutbox>();
            services.AddScoped<OutboxPublisher>();

            return services;
        }
    }
}
