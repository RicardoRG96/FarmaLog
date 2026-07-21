using FarmaLog.Nucleo.Domain.Solicitudes;
using FarmaLog.Nucleo.Domain.Solicitudes.Exceptions;

namespace FarmaLog.Nucleo.Domain.Tests.Solicitudes
{
    [TestClass]
    public class DireccionDespachoTests
    {
        [TestMethod]
        [DataRow("BI")]
        [DataRow("A1")]
        [DataRow("1")]
        public void DireccionDespacho_ShouldThrow_When_CodigoLaboratorioIsInvalid(
            string codigoLaboratorio)
        {
            //Arrange
            string formatedDireccionDespacho = $"{codigoLaboratorio}-778903671D1";

            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(formatedDireccionDespacho));
        }

        [TestMethod]
        public void DireccionDespacho_ShouldThrow_When_DashIsNotPresent()
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create("23778903671D1"));
        }

        [TestMethod]
        [DataRow("2377-8903671D1")]
        [DataRow("2377890-3671D1")]
        [DataRow("23778-903671D112")]
        [DataRow("23778903671D112-")]
        [DataRow("-23778903671D112")]
        public void DireccionDespacho_ShouldThrow_When_DashIsNotInTheRightPosition(
            string direccionDespacho)
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(direccionDespacho));
        }

        [TestMethod]
        public void DireccionDespacho_ShouldThrow_When_LetterD_IsNotPresent()
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create("23-7789036711"));
        }

        [TestMethod]
        [DataRow("23-77890D36711")]
        [DataRow("23-7789D036711")]
        [DataRow("23-778D9036711")]
        [DataRow("23-7D789036711")]
        [DataRow("23-7789036711D")]
        [DataRow("23-D7789036711")]
        public void DireccionDespacho_ShouldThrow_When_LetterD_IsNotInTheRightPosition(
            string direccion)
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(direccion));
        }

        [TestMethod]
        public void DireccionDespacho_ShouldConstruct_When_DireccionDespachoHasALowercaseLetterD()
        {
            //Arrange
            string expected = "23-778903671D1";

            //Act
            DireccionDespacho direccionDespacho = DireccionDespacho.Create("23-778903671d1");

            //Assert
            Assert.AreEqual(expected, direccionDespacho.Direccion);
        }

        [TestMethod]
        [DataRow("23-7789HJ671D1")]
        [DataRow("23-7789HJ67KD1")]
        [DataRow("23-.7/95667KD1")]
        public void DireccionDespacho_ShouldThrow_When_RutDoesNotContainsOnlyDigitsExceptTheLastOneChar(
            string direccion)
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(direccion));
        }

        [TestMethod]
        public void DireccionDespacho_ShouldConstruct_WithLastCharOfTheRutNormalized_When_RutHasALetterAsDV()
        {
            //Arrange
            string expected = "23-77890367kD1";

            //Act
            DireccionDespacho direccionDespacho = DireccionDespacho.Create("23-77890367KD1");

            //Assert
            Assert.AreEqual(expected, direccionDespacho.Direccion);
        }

        [TestMethod]
        [DataRow("23-77890367QD1")]
        [DataRow("23-77890367$D1")]
        [DataRow("23-77890367.D1")]
        public void DireccionDespacho_ShouldThrow_When_ARutHasNotANumberOrALetterKAsDigitoVerificador(
            string direccion)
        {
            //Act + Assert
            Assert.ThrowsExactly<DireccionDespachoInvalidaException>(
                () => DireccionDespacho.Create(direccion));
        }
    }
}
