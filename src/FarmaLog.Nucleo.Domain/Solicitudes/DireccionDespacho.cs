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
            return new DireccionDespacho(CodigoLaboratorio.Create("23"), "23-778903671D1");
        }
    }
}
