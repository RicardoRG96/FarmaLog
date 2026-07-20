using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed record CuentaCliente
    {
        public CodigoLaboratorio Codigo { get; }
        public string Cuenta { get; }

        private CuentaCliente(CodigoLaboratorio codigo, string cuenta)
        {
            Codigo = codigo;
            Cuenta = cuenta;
        }

        public static CuentaCliente Create(string cuenta)
        {
            string[] splitCuentaCliente = cuenta.Split('-');

            VerifyForCorrectDashFormat(splitCuentaCliente);

            CodigoLaboratorio codigo = VerifyForCorrectCodigoLaboratorioFormat(splitCuentaCliente);

            VerifyRutHasOnlyDigitsExceptForTheLastOneChar(splitCuentaCliente);

            if (HasRutADigitoVerificadorK(cuenta))
                cuenta = cuenta.ToLower();

            return new CuentaCliente(codigo, cuenta);
        }

        private static void VerifyForCorrectDashFormat(string[] splitCuentaCliente)
        {
            if (splitCuentaCliente.Length != 2 || splitCuentaCliente[1].Length != 10)
                throw new CuentaClienteInvalidaException(
                    "La cuenta de cliente debe tener un separador válido entre el codigo del laboratorio y el Rut del cliente");
        }

        private static CodigoLaboratorio VerifyForCorrectCodigoLaboratorioFormat(
            string[] splitCuentaCliente)
        {
            string codigoLaboratorioPrefix = splitCuentaCliente[0];

            CodigoLaboratorio codigo;

            try
            {
                codigo = CodigoLaboratorio.Create(codigoLaboratorioPrefix);
            }
            catch (CodigoLaboratorioInvalidoException ex)
            {
                throw new CuentaClienteInvalidaException(ex.Message);
            }

            return codigo;
        }

        private static void VerifyRutHasOnlyDigitsExceptForTheLastOneChar(
            string[] splitCuentaCliente)
        {
            string rutPortion = splitCuentaCliente[1];

            bool containsOnlyDigits = rutPortion[..^1].All(x => Char.IsAsciiDigit(x));

            if (!containsOnlyDigits)
                throw new CuentaClienteInvalidaException(
                    "La cuenta de cliente debe tener un Rut válido");
        }

        private static bool HasRutADigitoVerificadorK(string cuentaCliente)
        {
            char lastCuentaClienteCharacter = cuentaCliente[cuentaCliente.Length - 1];

            return lastCuentaClienteCharacter == 'k' || lastCuentaClienteCharacter == 'K';
        }
    }
}
