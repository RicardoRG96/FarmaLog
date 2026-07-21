using FarmaLog.Nucleo.Domain.Common;
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

            string rutCliente = splitCuentaCliente[1];

            if (!RutValidation.IsValidRutFormat(rutCliente))
            {
                throw new CuentaClienteInvalidaException(
                    "La cuenta de cliente debe tener un Rut válido");
            }

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
    }
}
