using Azure.Messaging.ServiceBus;
using FarmaLog.Nucleo.Application.Ports;
using FarmaLog.Nucleo.Application.UseCases.RegistrarSolicitudDeIngreso;
using FarmaLog.Nucleo.Infrastructure.Clock;
using FarmaLog.Nucleo.Infrastructure.Identity;
using FarmaLog.Nucleo.Infrastructure.Messaging;
using FarmaLog.Nucleo.Infrastructure.Persistence;
using FarmaLog.Nucleo.Worker;
using Microsoft.EntityFrameworkCore;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<NucleoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NucleoDb")));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IReloj, RelojDelSistema>();
builder.Services.AddSingleton<IGeneradorDeIdentificadores, GeneradorDeIdentificadores>();

builder.Services.AddScoped<IRepositorioDeSolicitudes, RepositorioDeSolicitudes>();
builder.Services.AddScoped<RegistrarSolicitudDeIngresoHandler>();
builder.Services.AddScoped<RegistrarSolicitudMessageHandler>();
builder.Services.AddScoped<RegistrarSolicitudMessageDispatcher>();

builder.Services.AddSingleton(_ =>
    new ServiceBusClient(builder.Configuration.GetConnectionString("ServiceBus")));

builder.Services.AddHostedService<RegistrarSolicitudWorker>();

IHost host = builder.Build();
host.Run();
