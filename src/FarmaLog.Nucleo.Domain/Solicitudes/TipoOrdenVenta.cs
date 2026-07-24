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

            char[] validTipoOrdenes = ['F', 'G'];

            bool tipoOrdenContainsLetterF = tipoOrden.Contains(validTipoOrdenes[0]);

            bool tipoOrdenContainsLetterG = tipoOrden.Contains(validTipoOrdenes[1]);

            if (!tipoOrdenContainsLetterF &&
                !tipoOrdenContainsLetterG)
                throw new TipoOrdenVentaInvalidoException("El código del tipo de orden es inválido");

            string[] splitTipoOrden = tipoOrden.Split(validTipoOrdenes);

            bool containsOnlyNumbersAfterTheLetter =
                splitTipoOrden[1].All(x => char.IsAsciiDigit(x));

            bool areTheCharactersAfterTheLetterValid =
                splitTipoOrden.Length == 2 &&
                splitTipoOrden[1] != "" &&
                containsOnlyNumbersAfterTheLetter;

            if (!areTheCharactersAfterTheLetterValid)
                throw new TipoOrdenVentaInvalidoException("El código del tipo de orden es inválido");

            string codigoLaboratorioPrefix = 
                tipoOrdenContainsLetterF ? tipoOrden.Split('F')[0] : tipoOrden.Split('G')[0];

            bool test = tipoOrden.Length > 1;

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
