namespace FarmaLog.Ingesta.Publishing
{
    internal sealed class CodigosD365Mapper
    {
        // 10 NO es constante mágica: es la especificación del VO CuentaCliente,
        // que exige exactamente 10 caracteres tras el guion.
        private const int CuentaBlockWidth = 10;

        public static string MapCuentaCliente(string codigoLaboratorio, string rut) =>
            $"{codigoLaboratorio}-{rut.Trim().ToUpperInvariant().PadLeft(CuentaBlockWidth, '0')}";

        // DireccionDespacho acepta RUT de 8 o 9 y lleva sufijo D+contador.
        public static string MapDireccionDespacho(string codigoLaboratorio, string codigo) =>
            $"{codigoLaboratorio}-{codigo.Trim().ToUpperInvariant()}";
    }
}
