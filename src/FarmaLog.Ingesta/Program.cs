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

var app = builder.Build();

app.MapGet("/", () => "Ingesta viva");

// DisableAntiforgery: este endpoint es servicio-a-servicio, no un formulario de
// navegador. El token CSRF protege contra un browser que adjunta cookies de
// sesión por su cuenta, acá el emisor es el portal desde el servidor.
app.MapPost("/cargas", async (
    IFormFile archivo, PlanillaReader reader, ServiceBusSender emisor, CancellationToken ct) =>
{
    using MemoryStream buffer = new();
    await archivo.CopyToAsync(buffer, ct);
    buffer.Position = 0;

    IReadOnlyList <FilaCruda> rows = reader.Read(buffer);
    string delivery = rows[0].NumeroDelivery;

    RegistrarSolicitudIngreso mensaje = new RegistrarSolicitudIngreso(
        CodigoLaboratorio: "23",
        NumeroDelivery: delivery,
        CuentaCliente: "23-0778903671",
        DireccionDespacho: "23-778903671D1",
        TipoOrdenVenta: "23F1",
        EsCenabast: false,
        DocumentoVentaCenabast: null,
        Observacion: "Esqueleto s17,",
        FechaEntrega: DateOnly.FromDateTime(DateTime.Today).AddDays(5),
        OrdenCompra: null,
        Urgencia: false,
        Lineas: [new LineaDeMensaje("SKU-000123", 10, "DISPONIBLE", "L2026A")]);

    await emisor.SendMessageAsync(
        new ServiceBusMessage(BinaryData.FromObjectAsJson(mensaje)) { MessageId = delivery },
        ct);

    return Results.Ok(new RespuestaDeCarga(archivo.FileName, archivo.Length));
})
.DisableAntiforgery();

app.Run();

internal sealed record RespuestaDeCarga(string NombreArchivo, long Bytes);
