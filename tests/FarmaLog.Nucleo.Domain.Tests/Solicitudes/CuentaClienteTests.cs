using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

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

        [TestMethod]
        public void CuentaCliente_ShouldThrow_When_DashIsNotPresent()
        {
            //Act + Assert
            Assert.ThrowsExactly<CuentaClienteInvalidaException>(
                () => CuentaCliente.Create("230778903671"));
        }

        [TestMethod]
        public void CuentaCliente_ShouldThrow_When_DashIsNotInTheRightPosition()
        {
            //Act + Assert
            Assert.ThrowsExactly<CuentaClienteInvalidaException>(
                () => CuentaCliente.Create("230778903671-"));
        }

        [TestMethod]
        public void CuentaCliente_ShouldThrow_When_ThePortionAfterTheDashIsNotTen()
        {
            //Act + Assert
            Assert.ThrowsExactly<CuentaClienteInvalidaException>(
                () => CuentaCliente.Create("23-00778903671"));
        }
    }
}
