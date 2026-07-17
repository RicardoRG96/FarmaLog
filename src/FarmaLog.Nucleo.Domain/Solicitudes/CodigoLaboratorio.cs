using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed record CodigoLaboratorio
    {
        public string Code { get; }

        private CodigoLaboratorio(string code) => Code = code;

        public static CodigoLaboratorio Create(string code)
        {
            foreach (char value in code)
            {
                if (!char.IsDigit(value))
                    throw new CodigoLaboratorioInvalidoException("Código inválido");
            }

            return new CodigoLaboratorio(code);
        }
    }   
}
