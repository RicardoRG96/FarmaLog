namespace FarmaLog.Nucleo.Domain.Tests.Solicitudes
{
    [TestClass]
    public class TipoOrdenVentaTests
    {
        [TestMethod]
        public void TipoOrdenVenta_ShouldThrow_When_CodigoLaboratorioIsInvalid()
        {
            //Act + Assert
            Assert.ThrowsExactly<TipoOrdenVentaInvalidoException>(
                () => TipoOrdenVenta.Create("BI"));
        }
    }
}
