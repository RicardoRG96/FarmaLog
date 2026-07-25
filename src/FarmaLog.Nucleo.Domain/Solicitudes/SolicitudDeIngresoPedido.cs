namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed class SolicitudDeIngresoPedido
    {
        public bool EsCenabast { get; set; }
        public DocumentoVentaCenabast? DVCenabast { get; set; }

        private SolicitudDeIngresoPedido(
            bool esCenabast, DocumentoVentaCenabast documentoVentaCenabast)
        {
            EsCenabast = esCenabast;
            DVCenabast = documentoVentaCenabast;
        }

        public static SolicitudDeIngresoPedido Create(
            bool esCenabast, DocumentoVentaCenabast documentoVentaCenabast)
        {
            return new SolicitudDeIngresoPedido(esCenabast, documentoVentaCenabast);
        }
    }
}
