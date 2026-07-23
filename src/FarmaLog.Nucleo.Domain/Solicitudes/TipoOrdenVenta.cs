using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

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

        public static TipoOrdenVenta Create(string? tipoOrden)
        {
            if (string.IsNullOrWhiteSpace(tipoOrden))
                throw new TipoOrdenVentaInvalidoException("El código del tipo de orden es inválido");

            tipoOrden = tipoOrden.ToUpper();

            if (!tipoOrden.Contains('F') &&
                !tipoOrden.Contains('G'))
                throw new TipoOrdenVentaInvalidoException("El código del tipo de orden es inválido");

            //agregar validacion para ver si hay algo despues de la letra.

            string codigoLaboratorioPrefix = tipoOrden.Split('F')[0]; //revisar con la letra 'G'

            CodigoLaboratorio codigo;

            try
            {
                codigo = CodigoLaboratorio.Create(codigoLaboratorioPrefix);
            } 
            catch (CodigoLaboratorioInvalidoException ex)
            {
                throw new TipoOrdenVentaInvalidoException(ex.Message);
            }

            return new TipoOrdenVenta(
                CodigoLaboratorio.Create("23"), "23F1");
        }
    }
}
