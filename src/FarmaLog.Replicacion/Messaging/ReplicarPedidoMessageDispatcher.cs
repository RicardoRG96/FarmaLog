using FarmaLog.Replicacion.Contracts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace FarmaLog.Replicacion.Messaging
{
    public sealed class ReplicarPedidoMessageDispatcher(
        ReplicarPedidoMessageHandler handler)
    {
        public async Task<MessageDestination> DispatchAsync(string messageId, string body, CancellationToken cancellationToken)
        {
            ReplicarPedidoEnD365Message? message;

            try
            {
                message = JsonSerializer.Deserialize<ReplicarPedidoEnD365Message>(body);
            }
            catch (JsonException)
            {
                return MessageDestination.DeadLetter;
            }

            if (string.IsNullOrWhiteSpace(messageId))
            {
                return MessageDestination.DeadLetter;
            }

            if (!IsWellFormed(message))
            {
                return MessageDestination.DeadLetter;
            }

            return await handler.HandleAsync(messageId, message, cancellationToken);
        }

        private static bool IsWellFormed([NotNullWhen(true)] ReplicarPedidoEnD365Message? message)
        {
            if (message is null) return false;
            if (string.IsNullOrWhiteSpace(message.CodigoLaboratorio)) return false;
            if (string.IsNullOrWhiteSpace(message.CuentaCliente)) return false;
            if (string.IsNullOrWhiteSpace(message.DireccionDespacho)) return false;
            if (string.IsNullOrWhiteSpace(message.TipoOrdenVenta)) return false;
            if (string.IsNullOrWhiteSpace(message.NumeroDelivery)) return false;
            if (message.FechaEntregaSolicitada == default) return false;
            if (message.Lineas is null) return false;
            if (message.Lineas.Count == 0) return false;

            return !message.Lineas.Any(linea =>
                string.IsNullOrWhiteSpace(linea.Sku) ||
                linea.Cantidad <= 0 ||
                string.IsNullOrWhiteSpace(linea.EstadoInventario));
        }
    }
}
