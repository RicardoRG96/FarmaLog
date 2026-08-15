using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed record CodigoLaboratorio
    {
        public string Codigo { get; }

        private CodigoLaboratorio(string codigo) => Codigo = codigo;

        public static CodigoLaboratorio Create(string? codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new CodigoLaboratorioInvalidoException("El Código de laboratorio no puede ser nulo");

            if (codigo.Length < 2)
                throw new CodigoLaboratorioInvalidoException("El Código de laboratorio debe tener una longitud de al menos dos caracteres");

            bool containsOnlyDigits = codigo.All(x => char.IsAsciiDigit(x));

            if (!containsOnlyDigits)
                throw new CodigoLaboratorioInvalidoException("El Código de laboratorio solo debe estar compuesto de números enteros");

            return new CodigoLaboratorio(codigo);
        }
    }   
}
