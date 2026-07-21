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

            try
            {
                string direccionCounter = direccion.Split('-')[1].Split('D')[1];

                bool theCounterHasOnlyDigits = direccionCounter.All(x => char.IsAsciiDigit(x));

                if (!theCounterHasOnlyDigits)
                    throw new DireccionDespachoInvalidaException(
                        "La dirección de despacho es inválida");

                bool counterHasLeadingZero = direccionCounter[0] == '0';

                if (counterHasLeadingZero)
                    throw new DireccionDespachoInvalidaException(
                        "La dirección de despacho es inválida");
            } 
            catch (IndexOutOfRangeException)
            {
                throw new DireccionDespachoInvalidaException(
                    "La dirección de despacho es inválida");
            }

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
    }
}
