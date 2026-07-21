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

            rutCliente = rutCliente.ToLower();

            string rutClienteWithoutDigitoVerificador = rutCliente[..^1];

            bool rutClienteWithoutDigitoVerificadorContainsOnlyDigits =
                rutClienteWithoutDigitoVerificador.All(x => char.IsAsciiDigit(x));

            char digitoVerificador = rutCliente[rutCliente.Length - 1];

            bool isAValidDigitoVerificador = char.IsAsciiDigit(digitoVerificador) ||
                digitoVerificador == 'k';

            if (!rutClienteWithoutDigitoVerificadorContainsOnlyDigits ||
                !isAValidDigitoVerificador)
                throw new DireccionDespachoInvalidaException("La dirección de despacho es inválida");

            int indexOfDigitoVerificador = direccion.IndexOf(char.ToUpper(digitoVerificador));

            char lowerDigitoVerificador = char.ToLower(direccion[indexOfDigitoVerificador]);

            direccion = direccion[..indexOfDigitoVerificador] + lowerDigitoVerificador + direccion.Substring(indexOfDigitoVerificador + 1);

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
            if (rutCliente.Length < 8)
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
    }
}
