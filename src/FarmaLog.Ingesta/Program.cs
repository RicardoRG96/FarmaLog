using Azure.Messaging.ServiceBus;
using FarmaLog.Ingesta;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

string? serviceBusConnectionString = builder.Configuration.GetConnectionString("ServiceBus");
if (string.IsNullOrWhiteSpace(serviceBusConnectionString) || 
    !serviceBusConnectionString.Contains("SharedAccessKey="))
    throw new InvalidOperationException(
        "ConnectionStrings:ServiceBus ausente o truncada. ¿Cargaste .env.local con el bucle while, no con source?");

builder.Services.AddSingleton(_ => new ServiceBusClient(serviceBusConnectionString));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<ServiceBusClient>().CreateSender("registrar-solicitud-ingreso"));

builder.Services.AddSingleton<PlanillaReader>();

builder.Services.AddSingleton<PlanillaReader>();
builder.Services.AddSingleton<CargaProcessor>();
var app = builder.Build();

app.MapGet("/", () => "Ingesta viva");

// DisableAntiforgery: este endpoint es servicio-a-servicio, no un formulario de
// navegador. El token CSRF protege contra un browser que adjunta cookies de
// sesión por su cuenta, acá el emisor es el portal desde el servidor.
app.MapPost("/cargas", async (
    IFormFile archivo,
    [FromForm] string codigoLaboratorio,
    [FromForm] string tipoOrdenVenta,
    PlanillaReader reader, CargaProcessor procesador, CancellationToken ct) =>
{
    using MemoryStream buffer = new();
    await archivo.CopyToAsync(buffer, ct);
    buffer.Position = 0;

    ProcessingResult result = await procesador.ProcesarAsync(
        buffer, codigoLaboratorio, tipoOrdenVenta, ct);

    return Results.Ok(new CargaResponse(result.PublishedPedidos, result.Errors));
})
.DisableAntiforgery();

app.Run();

internal sealed record CargaResponse(int PublishedPedidos, IReadOnlyList<string> Errors);
