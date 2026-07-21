using FarmaLog.Nucleo.Domain.Common;
using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed record DireccionDespacho
    {
        public CodigoLaboratorio Codigo { get; }
        public string Direccion { get; }

        private DireccionDespacho(CodigoLaboratorio codigo, string direccion)
        {
            Codigo = codigo;
            Direccion = direccion;
        }

        public static DireccionDespacho Create(string direccion)
        {
            direccion = direccion.ToUpper();

            VerifyDireccionContainsDashAndLetterD(direccion);

            string rutCliente = direccion.Split('-')[1].Split('D')[0];

            VerifyRutHasAValidLength(rutCliente);

            CodigoLaboratorio codigo = VerifyForCorrectCodigoLaboratorioFormat(direccion);

            if (!RutValidation.IsValidRutFormat(rutCliente))
            {
                throw new DireccionDespachoInvalidaException(
                    "La dirección de despacho debe tener un Rut válido");
            }

            direccion = NormalizeDigitoVerificador(direccion);

            return new DireccionDespacho(codigo, direccion);
        }

        private static void VerifyDireccionContainsDashAndLetterD(string direccion)
        {
            if (!direccion.Contains('-'))
                throw new DireccionDespachoInvalidaException("La dirección de despacho es inválida");

            if (!direccion.Contains('D'))
                throw new DireccionDespachoInvalidaException("La dirección de despacho es inválida");
        }

        private static void VerifyRutHasAValidLength(string rutCliente)
        {
            if (rutCliente.Length < 8 || rutCliente.Length > 9)
                throw new DireccionDespachoInvalidaException("La dirección de despacho es inválida");
        }

        private static CodigoLaboratorio VerifyForCorrectCodigoLaboratorioFormat(string direccion)
        {
            string codigoLaboratorioPrefix = direccion.Split('-')[0];

            CodigoLaboratorio codigo;

            try
            {
                codigo = CodigoLaboratorio.Create(codigoLaboratorioPrefix);
            }
            catch (CodigoLaboratorioInvalidoException ex)
            {
                throw new DireccionDespachoInvalidaException(ex.Message);
            }

            return codigo;
        }

        private static string NormalizeDigitoVerificador(string direccion)
        {
            string rutCliente = direccion.Split('-')[1].Split('D')[0];

            char digitoVerificador = rutCliente[rutCliente.Length - 1];

            int indexOfDigitoVerificador = direccion.IndexOf(char.ToUpper(digitoVerificador));

            char lowerDigitoVerificador = char.ToLower(direccion[indexOfDigitoVerificador]);

            return direccion[..indexOfDigitoVerificador] + 
                lowerDigitoVerificador + 
                direccion.Substring(indexOfDigitoVerificador + 1);
        }
    }
}
