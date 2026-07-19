using FarmaLog.Nucleo.Domain.Solicitudes;

namespace FarmaLog.Nucleo.Domain.Tests.Solicitudes
{
    [TestClass]
    public class CuentaClienteTests
    {
        [TestMethod]
        [DataRow("BI")]
        [DataRow("A1")]
        [DataRow("1")]
        public void CuentaCliente_ShouldThrow_When_CodigoLaboratorioIsInvalid(string codigoLaboratorio)
        {
            //Arrange
            string formatedCuentaCliente = $"{codigoLaboratorio}-0778903671";

            //Act + Assert
            Assert.ThrowsExactly<CuentaClienteInvalidaException>(
                () => CuentaCliente.Create(formatedCuentaCliente));
        }
    }
}
