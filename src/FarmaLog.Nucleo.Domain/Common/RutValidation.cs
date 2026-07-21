using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Common
{
    public static class RutValidation
    {
        public static bool IsValidRutFormat(string rut)
        {
            rut = rut.ToLower();

            string rutPortionWithoutDigitoVerificador = rut[..^1];

            bool rutPortionWithoutDigitoVerificadorContainsOnlyDigits =
                rutPortionWithoutDigitoVerificador.All(x => Char.IsAsciiDigit(x));

            char digitoVerificador = rut[rut.Length - 1];

            bool isAValidDigitoVerificador = char.IsAsciiDigit(digitoVerificador) ||
                digitoVerificador == 'k';

            return rutPortionWithoutDigitoVerificadorContainsOnlyDigits &&
                isAValidDigitoVerificador;
        }
    }
}
