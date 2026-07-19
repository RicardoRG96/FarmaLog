using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed record DocumentoVentaCenabast
    {
        public string Documento { get; }

        private DocumentoVentaCenabast(string documento)
        {
            Documento = documento;
        }

        public static DocumentoVentaCenabast Create(string? documento)
        {
            if (string.IsNullOrWhiteSpace(documento))
                throw new DocumentoVentaCenabastInvalidoException(
                    "El documento de venta Cenabast es obligatorio");

            documento = documento.Trim();

            return new DocumentoVentaCenabast(documento);
        }
    }
}
