using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed record DocumentoVentaCenabast
    {
        public string Valor { get; }

        private DocumentoVentaCenabast(string valor)
        {
            Valor = valor;
        }

        public static DocumentoVentaCenabast Create(string? valor)
        {
            if (string.IsNullOrEmpty(valor))
                throw new DocumentoVentaCenabastInvalidoException(
                    "El documento de venta Cenabast es obligatorio");

            return new DocumentoVentaCenabast(valor);
        }
    }
}
