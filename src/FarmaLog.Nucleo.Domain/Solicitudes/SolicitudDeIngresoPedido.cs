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
            bool esCoherenteConCenabast = esCenabast == (documentoVentaCenabast is not null);

            if (!esCoherenteConCenabast)
            {
                throw new SolicitudIncoherenteRespectoACenabastException(
                    "La información proporcionada respecto a Cenabast es incoherente");
            }

            return new SolicitudDeIngresoPedido(esCenabast, documentoVentaCenabast);
        }
    }
}
