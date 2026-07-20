using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed record CuentaCliente
    {
        public static CodigoLaboratorio? CodigoLaboratorio { get; private set; }
        public string Cuenta { get; }

        private CuentaCliente(string cuenta)
        {
            Cuenta = cuenta;
        }

        public static CuentaCliente Create(string cuenta)
        {
            string[] splitedCuentaCliente = cuenta.Split("-");

            if (!cuenta.Contains('-') || splitedCuentaCliente[1].Length != 10)
                throw new CuentaClienteInvalidaException(
                    "La cuenta de cliente debe tener un separador válido entre el codigo del laboratorio y el Rut del cliente");

            string codigoLaboratorioPrefix = splitedCuentaCliente[0];

            try
            {
                CodigoLaboratorio = CodigoLaboratorio.Create(codigoLaboratorioPrefix);
            }
            catch (CodigoLaboratorioInvalidoException ex)
            {
                throw new CuentaClienteInvalidaException(ex.Message);
            }
            
            return new CuentaCliente(cuenta);
        }
    }
}
