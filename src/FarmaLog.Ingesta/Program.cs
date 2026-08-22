using Azure.Messaging.ServiceBus;
using FarmaLog.Ingesta;

var builder = WebApplication.CreateBuilder(args);

string? serviceBusConnectionString = builder.Configuration.GetConnectionString("ServiceBus");
if (string.IsNullOrWhiteSpace(serviceBusConnectionString))
    throw new InvalidOperationException(
        "Falta ConnectionStrings:ServiceBus.");

builder.Services.AddSingleton(_ => new ServiceBusClient(serviceBusConnectionString));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<ServiceBusClient>().CreateSender("registrar-solicitud-ingreso"));

builder.Services.AddSingleton<PlanillaReader>();

builder.Services.AddSingleton<PlanillaReader>();
builder.Services.AddSingleton<ProcesadorDeCarga>();

var app = builder.Build();

app.MapGet("/", () => "Ingesta viva");

// DisableAntiforgery: este endpoint es servicio-a-servicio, no un formulario de
// navegador. El token CSRF protege contra un browser que adjunta cookies de
// sesión por su cuenta, acá el emisor es el portal desde el servidor.
app.MapPost("/cargas", async (
    IFormFile archivo, PlanillaReader reader, ProcesadorDeCarga procesador, CancellationToken ct) =>
{
    using MemoryStream buffer = new();
    await archivo.CopyToAsync(buffer, ct);
    buffer.Position = 0;

    ResultadoDeProcesamiento resultado = await procesador.ProcesarAsync(buffer, ct);

    return Results.Ok(new RespuestaDeCarga(resultado.PedidosPublicados, resultado.Errores));
})
.DisableAntiforgery();

app.Run();

internal sealed record RespuestaDeCarga(int PedidosPublicados, IReadOnlyList<string> Errores);
