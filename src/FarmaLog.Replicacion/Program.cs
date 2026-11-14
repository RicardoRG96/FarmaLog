using Azure.Messaging.ServiceBus;
using Azure.Monitor.OpenTelemetry.Exporter;
using FarmaLog.Replicacion.D365;
using FarmaLog.Replicacion.Messaging;
using FarmaLog.Replicacion.Publishing;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")))
{
    builder.Services.AddOpenTelemetry()
        .UseFunctionsWorkerDefaults()
        .UseAzureMonitorExporter();
}

builder.Services.AddSingleton(_ => new ServiceBusClient(builder.Configuration["ServiceBus"]));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<ServiceBusClient>().CreateSender("resultado-replicacion"));
builder.Services.AddSingleton<IResultadoReplicacionPublisher, ServiceBusResultadoReplicacionPublisher>();

builder.Services.AddHttpClient<ID365Client, D365HttpClient>(http =>
    http.BaseAddress = new Uri(builder.Configuration["D365:BaseUrl"]!));

builder.Services.AddTransient<ReplicarPedidoMessageHandler>();
builder.Services.AddTransient<ReplicarPedidoMessageDispatcher>();

builder.Build().Run();
