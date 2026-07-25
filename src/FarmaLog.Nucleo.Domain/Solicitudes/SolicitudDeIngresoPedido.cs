using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed class SolicitudDeIngresoPedido
    {
        public bool EsCenabast { get; }
        public DocumentoVentaCenabast? DocumentoVentaCenabast { get; }

        private SolicitudDeIngresoPedido(
            bool esCenabast, DocumentoVentaCenabast? documentoVentaCenabast)
        {
            EsCenabast = esCenabast;
            DocumentoVentaCenabast = documentoVentaCenabast;
        }

        public static SolicitudDeIngresoPedido Create(
            bool esCenabast, DocumentoVentaCenabast? documentoVentaCenabast)
        {
            if (esCenabast && documentoVentaCenabast is null)
            {
                throw new SolicitudIncoherenteRespectoACenabastException(
                    "El documento de venta Cenabast es obligatorio");
            }

            return new SolicitudDeIngresoPedido(esCenabast, documentoVentaCenabast);
        }
    }
}
