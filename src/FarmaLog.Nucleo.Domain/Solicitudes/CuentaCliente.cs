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
            string codigoLaboratorioPrefix = cuenta.Split("-")[0];
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
