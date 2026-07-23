namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed record TipoOrdenVenta
    {
        public CodigoLaboratorio Codigo { get; }
        public string Tipo { get; }

        private TipoOrdenVenta(CodigoLaboratorio codigo, string tipo)
        {
            Codigo = codigo;
            Tipo = tipo;
        }

        public static TipoOrdenVenta Create(string tipoOrden)
        {
            return new TipoOrdenVenta(
                CodigoLaboratorio.Create("23"), "23F1");
        }
    }
}
