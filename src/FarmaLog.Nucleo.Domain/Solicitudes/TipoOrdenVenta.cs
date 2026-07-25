using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed record TipoOrdenVenta
    {
        public CodigoLaboratorio Codigo { get; }
        public string Tipo { get; }

        private static readonly char[] _validTipoOrdenes = ['F', 'G'];

        private TipoOrdenVenta(CodigoLaboratorio codigo, string tipo)
        {
            Codigo = codigo;
            Tipo = tipo;
        }

        public static TipoOrdenVenta Create(string? tipoOrden)
        {
            if (string.IsNullOrWhiteSpace(tipoOrden))
                throw new TipoOrdenVentaInvalidoException("El código del tipo de orden es inválido");

            tipoOrden = tipoOrden.Trim().ToUpper();

            VerifyTipoOrdenContainsTheValidLetters(tipoOrden);

            string[] splitTipoOrden = tipoOrden.Split(_validTipoOrdenes);

            VerifyForValidCharactersAfterTheLetter(splitTipoOrden);

            CodigoLaboratorio codigo = VerifyForCorrectCodigoLaboratorioFormat(splitTipoOrden);

            return new TipoOrdenVenta(codigo, tipoOrden);
        }

        private static void VerifyTipoOrdenContainsTheValidLetters(string tipoOrden)
        {
            bool tipoOrdenContainsLetterF = tipoOrden.Contains(_validTipoOrdenes[0]);

            bool tipoOrdenContainsLetterG = tipoOrden.Contains(_validTipoOrdenes[1]);

            if (!tipoOrdenContainsLetterF &&
                !tipoOrdenContainsLetterG)
                throw new TipoOrdenVentaInvalidoException("El código del tipo de orden es inválido");
        }

        private static void VerifyForValidCharactersAfterTheLetter(string[] splitTipoOrden)
        {
            string charactersAfterTheLetter = splitTipoOrden[1];

            if (string.IsNullOrEmpty(charactersAfterTheLetter))
                throw new TipoOrdenVentaInvalidoException("El código del tipo de orden es inválido");

            char firstCharacterAfterTheLetter = splitTipoOrden[1][0];

            bool splitTipoOrdenHaveACorrectLength = splitTipoOrden.Length == 2;

            if (!charactersAfterTheLetter.All(char.IsAsciiDigit))
                throw new TipoOrdenVentaInvalidoException("El código del tipo de orden es inválido");

            if (firstCharacterAfterTheLetter == '0')
                throw new TipoOrdenVentaInvalidoException("El código del tipo de orden es inválido");

            if (!splitTipoOrdenHaveACorrectLength)
                throw new TipoOrdenVentaInvalidoException("El código del tipo de orden es inválido");
        }

        private static CodigoLaboratorio VerifyForCorrectCodigoLaboratorioFormat(string[] splitTipoOrden)
        {
            string codigoLaboratorioPrefix = splitTipoOrden[0];

            CodigoLaboratorio codigo;

            try
            {
                codigo = CodigoLaboratorio.Create(codigoLaboratorioPrefix);
            }
            catch (CodigoLaboratorioInvalidoException ex)
            {
                throw new TipoOrdenVentaInvalidoException(ex.Message);
            }

            return codigo;
        }
    }
}
