using Azure.Messaging.ServiceBus;
using FarmaLog.Ingesta;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddSingleton(_ => new ServiceBusClient(
    builder.Configuration.GetConnectionString("ServiceBus")
        ?? throw new InvalidOperationException("Falta ConnectionString:ServiceBus")));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<ServiceBusClient>().CreateSender("registrar-solicitud-ingreso"));

builder.Services.AddSingleton<LectorDePlanilla>();

app.MapGet("/", () => "Ingesta viva");

// DisableAntiforgery: este endpoint es servicio-a-servicio, no un formulario de
// navegador. El token CSRF protege contra un browser que adjunta cookies de
// sesión por su cuenta, acá el emisor es el portal desde el servidor.
app.MapPost("/cargas", (IFormFile archivo) =>
    Results.Ok(new RespuestaDeCarga(archivo.FileName, archivo.Length)))
    .DisableAntiforgery();

app.Run();

internal sealed record RespuestaDeCarga(string NombreArchivo, long Bytes);
