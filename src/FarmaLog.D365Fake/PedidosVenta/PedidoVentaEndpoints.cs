using FarmaLog.D365Fake.Configuration;
using FarmaLog.D365Fake.Contracts;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace FarmaLog.D365Fake.PedidosVenta
{
    public static class PedidoVentaEndpoints
    {
        private const string MotivoPedidoYaExiste = "Ya existe un pedido con ese numero de pedido del cliente: ";

        public static IEndpointRouteBuilder MapPedidoVentaEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/salesorders", CreatePedidoVenta);
            app.MapGet("/api/salesorders", ListPedidosVenta);
            return app;
        }

        private static Results<Ok<PedidoAceptadoResponse>, BadRequest<PedidoRechazadoResponse>, StatusCodeHttpResult> CreatePedidoVenta(
            PedidoVentaD365 pedido,
            PedidoVentaStore store,
            PedidoVentaValidator validator,
            IOptionsMonitor<D365FakeOptions> options,
            ILoggerFactory loggerFactory)
        {
            ILogger logger = loggerFactory.CreateLogger("FarmaLog.D365Fake");

            if (options.CurrentValue.SimulateOutage)
            {
                logger.LogWarning("Caída simulada: 503 para el delivery {NumeroDelivery} del laboratorio {CodigoLaboratorio}",
                    pedido.NumeroDelivery, pedido.CodigoLaboratorio);
                return TypedResults.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (store.TryFind(pedido.CodigoLaboratorio, pedido.NumeroDelivery, out PedidoVentaRegistrado? existing))
            {
                return AlreadyExists(existing, logger);
            }

            IReadOnlyList<string> motivos = validator.Validate(pedido);

            if (motivos.Count > 0)
            {
                logger.LogInformation("Rechazado el delivery {NumeroDelivery}: {Motivos}",
                    pedido.NumeroDelivery, string.Join(" | ", motivos));
                return TypedResults.BadRequest(new PedidoRechazadoResponse(motivos));
            }

            if (!store.TryAdd(pedido, out PedidoVentaRegistrado registrado))
            {
                return AlreadyExists(registrado, logger);
            }

            logger.LogInformation("Creado {NumeroPedido} para el delivery {NumeroDelivery} del laboratorio {CodigoLaboratorio}",
                registrado.NumeroPedido, pedido.NumeroDelivery, pedido.CodigoLaboratorio);
            return TypedResults.Ok(new PedidoAceptadoResponse(registrado.NumeroPedido));
        }

        private static BadRequest<PedidoRechazadoResponse> AlreadyExists(PedidoVentaRegistrado existing, ILogger logger)
        {
            logger.LogInformation("Pedido duplicado: ya existe {NumeroPedido}", existing.NumeroPedido);
            return TypedResults.BadRequest(new PedidoRechazadoResponse([MotivoPedidoYaExiste + existing.NumeroPedido]));
        }

        private static Ok<IReadOnlyList<PedidoVentaRegistrado>> ListPedidosVenta(PedidoVentaStore store) =>
            TypedResults.Ok(store.GetAll());
    }
}