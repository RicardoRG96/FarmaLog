namespace FarmaLog.D365Fake.Configuration
{
    public sealed class D365FakeOptions
    {
        public const string SectionName = "D365Fake";

        public bool SimulateOutage { get; set; }
        public string[] UnknownCuentasCliente { get; set; } = [];
        public string[] UnknownSkus { get; set; } = [];
    }
}