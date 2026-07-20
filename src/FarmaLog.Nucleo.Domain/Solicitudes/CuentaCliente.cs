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

            VerifyForValidRut(splitCuentaCliente);
            
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

        private static void VerifyForValidRut(string[] splitCuentaCliente)
        {
            string rutPortion = splitCuentaCliente[1].ToLower();

            string rutPortionWithoutDigitoVerificador = rutPortion[..^1];

            bool rutPortionWithoutDigitoVerificadorContainsOnlyDigits = 
                rutPortionWithoutDigitoVerificador.All(x => Char.IsAsciiDigit(x));

            char digitoVerificador = rutPortion[rutPortion.Length - 1];

            bool isAValidDigitoVerificador = char.IsAsciiDigit(digitoVerificador) ||
                digitoVerificador == 'k';

            if (!rutPortionWithoutDigitoVerificadorContainsOnlyDigits ||
                !isAValidDigitoVerificador)
                throw new CuentaClienteInvalidaException(
                    "La cuenta de cliente debe tener un Rut válido");
        }
    }
}
