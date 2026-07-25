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
        [DataRow("230778903671-")]
        [DataRow("230778903-671")]
        [DataRow("-230778903671")]
        [DataRow("230-778903671")]
        public void CuentaCliente_ShouldThrow_When_DashIsNotInTheRightPosition(string cuentaCliente)
        {
            //Act + Assert
            Assert.ThrowsExactly<CuentaClienteInvalidaException>(
                () => CuentaCliente.Create(cuentaCliente));
        }

        [TestMethod]
        [DataRow("23-00778903671")]
        [DataRow("23-007789031")]
        public void CuentaCliente_ShouldThrow_When_ThePortionAfterTheDashIsNotTen(string cuentaCliente)
        {
            //Act + Assert
            Assert.ThrowsExactly<CuentaClienteInvalidaException>(
                () => CuentaCliente.Create(cuentaCliente));
        }

        [TestMethod]
        public void CuentaCliente_ShouldThrow_When_ThePortionAfterTheDashDoesNotContainsOnlyDigitsExceptTheLastOneChar()
        {
            //Act + Assert
            Assert.ThrowsExactly<CuentaClienteInvalidaException>(
                () => CuentaCliente.Create("23-0778TH367K"));
        }

        [TestMethod]
        public void CuentaCliente_ShouldConstruct_WithLastCharacterNormalized_When_RutHasALetterAsDV()
        {
            //Arrange
            string expected = "23-077890367K";

            //Act
            CuentaCliente cuentaCliente = CuentaCliente.Create("23-077890367k");

            //Assert
            Assert.AreEqual(expected, cuentaCliente.Cuenta);
        }

        [TestMethod]
        [DataRow("23-077890367Q")]
        [DataRow("23-077890367$")]
        [DataRow("23-077890367.")]
        public void CuentaCliente_ShouldThrow_When_ARutHasNotANumberOrALetterKAsDigitoVerificador(
            string cuentaCliente)
        {
            //Act + Assert
            Assert.ThrowsExactly<CuentaClienteInvalidaException>(
                () => CuentaCliente.Create(cuentaCliente));
        }

        [TestMethod]
        public void CuentaCliente_ShouldThrow_When_AnEmptyStringIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<CuentaClienteInvalidaException>(
                () => CuentaCliente.Create(""));
        }

        [TestMethod]
        public void CuentaCliente_ShouldThrow_When_NullIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<CuentaClienteInvalidaException>(
                () => CuentaCliente.Create(null));
        }

        [TestMethod]
        public void CuentaCliente_ShouldThrow_When_AWhiteSpaceStringIsPassed()
        {
            //Act + Assert
            Assert.ThrowsExactly<CuentaClienteInvalidaException>(
                () => CuentaCliente.Create("    "));
        }

        [TestMethod]
        public void CuentaCliente_ShouldConstruct_When_AValidStringWithWhiteSpacesIsPassed()
        {
            //Arrange
            string expected = "23-077890367K";

            //Act
            CuentaCliente cuentaCliente = CuentaCliente.Create("    23-077890367K   ");

            //Assert
            Assert.AreEqual(expected, cuentaCliente.Cuenta);
        }

        [TestMethod]
        public void CuentaCliente_ShouldConstructAndNormalize_WhenAValidStringWithWhiteSpacesAndLowerCaseLetterDIsPassed()
        {
            //Arrange
            string expected = "23-077890367K";

            //Act
            CuentaCliente cuentaCliente = CuentaCliente.Create("    23-077890367k   ");

            //Assert
            Assert.AreEqual(expected, cuentaCliente.Cuenta);
        }

        [TestMethod]
        [DataRow("23- 077890367K")]
        [DataRow("23 -077890367K")]
        [DataRow("23-07789 0367K")]
        [DataRow("23-077890367 K")]
        [DataRow("23- 07789 0367K")]
        public void CuentaCliente_ShouldThrow_When_AStringWithIntermediateSpacesIsPassed(
            string cuentaCliente)
        {
            //Act + Assert
            Assert.ThrowsExactly<CuentaClienteInvalidaException>(
                () => CuentaCliente.Create(cuentaCliente));
        }
    }
}
