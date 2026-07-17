namespace FarmaLog.Nucleo.Domain.Solicitudes
{
    public sealed record CodigoLaboratorio
    {
        public string Code { get; }

        private CodigoLaboratorio(string code) => Code = code;

        public static CodigoLaboratorio Create(string code)
        {
            return new CodigoLaboratorio(code);
        }
    }   
}
