using FarmaLog.D365Fake.Configuration;
using FarmaLog.D365Fake.PedidosVenta;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Minimal API usa camelCase por defecto; el adaptador lee PascalCase (decisión 117).
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.PropertyNamingPolicy = null);

builder.Services.Configure<D365FakeOptions>(
    builder.Configuration.GetSection(D365FakeOptions.SectionName));
builder.Services.AddSingleton<PedidoVentaStore>();
builder.Services.AddSingleton<PedidoVentaValidator>();

WebApplication app = builder.Build();

app.MapPedidoVentaEndpoints();

app.Run();