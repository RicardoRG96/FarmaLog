using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed record CodigoLaboratorio
    {
        public string Codigo { get; }

        private CodigoLaboratorio(string codigo) => Codigo = codigo;

        public static CodigoLaboratorio Create(string codigo)
        {
            bool containsOnlyDigits = codigo.All(x => char.IsAsciiDigit(x));

            if (!containsOnlyDigits)
                throw new CodigoLaboratorioInvalidoException("Código inválido");

            return new CodigoLaboratorio(codigo);
        }
    }   
}
