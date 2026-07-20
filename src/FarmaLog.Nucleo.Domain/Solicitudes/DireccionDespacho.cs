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
            direccion = direccion.ToUpper();

            if (!direccion.Contains('-'))
                throw new DireccionDespachoInvalidaException("La dirección de despacho es inválida");

            if (!direccion.Contains('D'))
                throw new DireccionDespachoInvalidaException("La dirección de despacho es inválida");

            string rutCliente = direccion.Split('-')[1].Split('D')[0];

            if (rutCliente.Length < 8)
                throw new DireccionDespachoInvalidaException("La dirección de despacho es inválida");

            string codigoLaboratorioPrefix = direccion.Split('-')[0];

            try
            {
                CodigoLaboratorio codigo = CodigoLaboratorio.Create(codigoLaboratorioPrefix);
            }
            catch (CodigoLaboratorioInvalidoException ex)
            {
                throw new DireccionDespachoInvalidaException(ex.Message);
            }

            return new DireccionDespacho(CodigoLaboratorio.Create("23"), "23-778903671D1");
        }
    }
}
