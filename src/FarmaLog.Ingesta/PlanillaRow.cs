namespace FarmaLog.Ingesta
{
    internal sealed record PlanillaRow(
        int NumeroDeFila,
        string NumeroDelivery,          // A  cabecera
        string CuentaCliente,           // B  cabecera
        string DocumentoVentaCenabast,  // C  cabecera, obligatoria si J = SI
        string Sku,                     // D  línea
        string Cantidad,                // E  línea
        string Lote,                    // F  línea, opcional
        string DireccionDespacho,       // G  cabecera
        string OrdenCompra,             // H  cabecera, opcional
        string EstadoInventario,        // I  línea
        string EsCenabast,              // J  cabecera
        string FechaEntrega,            // K  cabecera, opcional
        string Urgencia,                // L  cabecera, opcional
        string Observacion);
}
