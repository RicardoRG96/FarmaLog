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
            try
            {
                CodigoLaboratorio codigo = CodigoLaboratorio.Create("BI-778903671D1");
            } 
            catch (CodigoLaboratorioInvalidoException ex)
            {
                throw new DireccionDespachoInvalidaException(ex.Message);
            }

            return new DireccionDespacho(CodigoLaboratorio.Create("23"), "23-778903671D1");
        }
    }
}
