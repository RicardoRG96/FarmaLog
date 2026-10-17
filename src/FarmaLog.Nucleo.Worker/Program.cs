using Azure.Messaging.ServiceBus;
using FarmaLog.Nucleo.Infrastructure.DependencyInjection;
using FarmaLog.Nucleo.Worker;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddNucleo(builder.Configuration.GetConnectionString("NucleoDb"));

builder.Services.AddSingleton(_ =>
    new ServiceBusClient(builder.Configuration.GetConnectionString("ServiceBus")));

builder.Services.AddHostedService<RegistrarSolicitudWorker>();

IHost host = builder.Build();
host.Run();
