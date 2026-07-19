namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed record CuentaCliente
    {
        public CodigoLaboratorio? CodigoLaboratorio { get; set; }
        public string Cuenta { get; }

        private CuentaCliente(string cuenta)
        {
            Cuenta = cuenta;
        }

        public static CuentaCliente Create(string cuenta)
        {
            return new CuentaCliente(cuenta);
        }
    }
}
