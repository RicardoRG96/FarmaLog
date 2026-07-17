using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed record CodigoLaboratorio
    {
        public string Codigo { get; }

        private CodigoLaboratorio(string codigo) => Codigo = codigo;

        public static CodigoLaboratorio Create(string codigo)
        {
            foreach (char value in codigo)
            {
                if (!char.IsDigit(value))
                    throw new CodigoLaboratorioInvalidoException("Código inválido");
            }

            return new CodigoLaboratorio(codigo);
        }
    }   
}
