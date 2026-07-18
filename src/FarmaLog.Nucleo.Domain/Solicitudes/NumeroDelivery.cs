using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed record NumeroDelivery
    {
        public string Numero { get; }

        private NumeroDelivery(string numero)
        {
            Numero = numero;
        }

        public static NumeroDelivery Create(string numero)
        {
            if (numero == "")
                throw new NumeroDeliveryInvalidoException("El número de Delivery es obligatorio");

            return new NumeroDelivery(numero);
        }
    }
}
